using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;

namespace Aratog.NavyFight.Models.Unity3D.TaskManager {
	public class Task
	{

		#region Variables

		public bool isFinished;

		public bool isCanceled;

		public bool ignoreCycle;

		private bool isRealTimeTask;
		private bool isNotRealTimeTaskIsExecited;

		public delegate bool TaskEventHandler(params object[] args);

		public event TaskEventHandler task;

		public delegate bool OnFinishedEventHandler();

		public event OnFinishedEventHandler OnFinishEvent;

		public object[] args;

		#endregion

		#region Constructor/Destructor

		public Task()
		{
			isFinished = false;
			isCanceled = false;
			ignoreCycle = true;
			args = null;
			isRealTimeTask = true;
		}

		public bool IsFinished
		{
			get { return OnFinishEvent == null || OnFinishEvent(); }
		}

		public Task (TaskEventHandler task, bool ignoreCycle = false, params object[] args) {
			isFinished = false;
			isCanceled = false;
			this.ignoreCycle = ignoreCycle;
			this.task = task;
			if (args != null) {
				this.args = args;
			}
			isRealTimeTask = true;
		}

		public Task (TaskEventHandler task,  System.Action onFinished, bool ignoreCycle = false, params object[] args) {
			isFinished = false;
			isCanceled = false;
			this.ignoreCycle = ignoreCycle;
			this.task = task;
			if (args != null) {
				this.args = args;
			}
			onFinished += OnFinished;

			isNotRealTimeTaskIsExecited = false;
			isRealTimeTask = false;
		}

		#endregion

		#region Events

		public void Reset (bool ignoreCycle = true) {
			isCanceled = false;
			isFinished = false;
			this.ignoreCycle = ignoreCycle;
		}

		public void Execute () {
			if (task != null && (isRealTimeTask || !isNotRealTimeTaskIsExecited))
			{
				isFinished = task(args);
				isNotRealTimeTaskIsExecited = true;
			}
				
		}

		private void OnFinished()
		{
			isFinished = true;

			isNotRealTimeTaskIsExecited = false;

		}

		#endregion
	}
}
