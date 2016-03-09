using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.TaskManager {
	public class TaskManager {
		#region Variable

		/// <summary>
		/// Список задач
		/// </summary>
		private List<Task> tasks;

		/// <summary>
		/// Текущая исполняемая задача
		/// </summary>
		private Task currentTask;
		
		/// <summary>
		/// Использовать циклический режим
		/// </summary>
		private bool cycle;

		public bool Cycle {
			get { return cycle; }
			set { cycle = value; }
		}
		
		/// <summary>
		/// Пауза в ДЗ, измеряемая в кадрах
		/// </summary>
		private int pauseInFrame;

		/// <summary>
		/// Сколько времени прошло во время паузы
		/// </summary>
		private int pausePassed;

		public TaskManagerState State { get; private set; }

		#endregion

		#region Constructor

		public TaskManager (bool cycle = false) {
			this.cycle = cycle;
			tasks = new List<Task>();
			State = TaskManagerState.Ready;
			pauseInFrame = 0;
			pausePassed = 0;
		}

		#endregion

		#region Events


		/// <summary>
		/// Добавить задачу в конец очереди
		/// </summary>
		/// <param name="func"> сама задача (функция)</param>
		/// <param name="ignoreCycle">в случае если у нас циклический режим, добавляемая задача может выполнятся всего один раз</param>
		/// <param name="args">набор параметров, необходимых для выполнения поставленной задачи</param>
		public void AddTask (Task.TaskEventHandler func, bool ignoreCycle = false, params object[] args) {
			tasks.Add(new Task(func, ignoreCycle, args));
			State = TaskManagerState.Working;
		}

		/// <summary>
		/// Добавить задачу в начало очереди
		/// </summary>
		/// <param name="func"></param>
		/// <param name="ignoreCycle"></param>
		/// <param name="args"></param>
		public void AddUrgentTask (Task.TaskEventHandler func, bool ignoreCycle = false, params object[] args) {
			tasks.Insert(0, new Task(func, ignoreCycle, args));
			State = TaskManagerState.Working;
		}

		/// <summary>
		/// Добавить одноразовую задачу в конец очереди
		/// </summary>
		/// <param name="func"></param>
		/// <param name="args"></param>
		public void AddInstantTask (Task.TaskEventHandler func, params object[] args) {
			tasks.Add(new Task(func, true, args));
			State = TaskManagerState.Working;
		}
		
		/// <summary>
		/// Добавить одноразовую задачу в начало очереди
		/// </summary>
		/// <param name="func"></param>
		/// <param name="args"></param>
		public void AddUrgentInstantTask (Task.TaskEventHandler func, params object[] args) {
			tasks.Insert(0, new Task(func, true, args));
			State = TaskManagerState.Working;
		}


		/// <summary>
		/// Добавить паузу в кадрах
		/// </summary>
		/// <param name="time"></param>
		/// <param name="ignoreCycle"></param>
		public void AddPause (int time, bool ignoreCycle = false) {
			//State = TaskManagerState.Pause;
			pauseInFrame = time;
			pausePassed = 0;
			AddTask(Pause, ignoreCycle);
		}

		public bool Pause (params object[] args) {
			if (args.Length > 0 && pauseInFrame == 0) {
				pauseInFrame = (int) args[0];
			}


			if (pausePassed <= pauseInFrame) {
				pausePassed ++;
				return false;
			}
			//pauseInFrame = 0;
			pausePassed = 0;
			return true;
		}

		/// <summary>
		/// Удалить все существующие задачи
		/// </summary>
		public void RemoveAllTask () {
			foreach (Task task in tasks) {
				task.Reset();
			}
			tasks.Clear();
			State = TaskManagerState.None;
		}

		/// <summary>
		/// Насильно перейти к следующей по очереди задачи
		/// </summary>
		public void NextTask () {
			
		}

		public void CancelCurrentTask () {
			currentTask.isCanceled = true;
		}

		public int TaskCount {
			get { return tasks.Count; }
		}

		public void Update () {
			switch (State) {
					//ДЗ стоит ничего не делаает, так как нет задач
				case TaskManagerState.None :
					State = TaskManagerState.Ready;
					break;
				case TaskManagerState.Ready :
					break;
				case TaskManagerState.Finished :
					State = TaskManagerState.Ready;
					break;

					//ДЗ находится в паузе
				case TaskManagerState.Pause :
					bool needResetPauseData = false;
					if (pausePassed <= pauseInFrame)
						pausePassed++;
					else if (tasks.Count > 0) {
						State = TaskManagerState.Working;
						needResetPauseData = true;
					}
					else {
						State = TaskManagerState.Ready;
						needResetPauseData = true;
					}

					if (needResetPauseData) {
						pauseInFrame = 0;
						pausePassed = 0;
					}
					break;

					//ДЗ обрабатывает задачи по очереди
				case TaskManagerState.Working :
					//Получаем тукущую задачу
					currentTask = GetCurrentTask();

					if (currentTask == null) {
						State = TaskManagerState.Ready;
						break;
					}

					//Проверяем выполнилась она или нет
					if (currentTask.isFinished/* || currentTask.IsFinished*/) {
						//Удаляем задачу из очереди
						//Debug.Log("TM: finished task " + ToString());
						tasks.Remove(currentTask);
						//если у нас циклический режим то сбрасываем параметры и добавляем задачу в конец очереди
						if (cycle && !currentTask.ignoreCycle) {
							//Debug.Log("TM: reset task " + ToString());
							currentTask.Reset(false);
							tasks.Add(currentTask);
						}
					}
					else if (currentTask.isCanceled) {
						Debug.Log("TM: cancel task " + ToString());
						tasks.Remove(currentTask);
					}
					else
						currentTask.Execute();

					//Если количество задач стало равным 0, переходим в режим готовности
					if (tasks.Count == 0)
						State = TaskManagerState.Ready;
					break;
				default :
					Debug.LogError("Some Error");
					break;
			}

		}

		private Task GetCurrentTask () {
			if (tasks.Count > 0)
				return tasks[0];
			return null;
		}

		public void Statistic () {
			Debug.Log(string.Format("TaskManager state: tasks count: {0}, current task: {1}", tasks.Count, currentTask.args[0] ?? ""));
		}

		public override string ToString () {
			string str = string.Format("TaskManager state: tasks count: {0}, current task: {1}",
			                           tasks.Count,
			                           currentTask.args[0] ?? "");
			return str;
		}
		#endregion
	}
}
