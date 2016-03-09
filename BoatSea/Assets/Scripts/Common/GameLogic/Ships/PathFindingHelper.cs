using System.Collections.Generic;
using System.Collections.ObjectModel;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D.Debugger;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Players;
using LinqTools;
using UnityEngine;
using System.Collections;

public class PathFindingHelper : MonoBehaviour
{

	#region Variables

	public static PathFindingHelper Instance { get; private set; }

	public static event System.Action<List<Point>, int, AIPlayer.AIPathTask> PathReturn;

	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER
	private const int FrameIterators = 50;
	private const int FrameIteratorsAdd = 20;

#elif UNITY_IPHONE || UNITY_ANDROID
	private const int FrameIterators = 4;
	private const int FrameIteratorsAdd = 4;
#endif

	#region Pathfind task manager, task priority

	

	public enum TaskPriority
	{
		LowPriority,
		MiddlePriority,
		HighPriority
	}

	public enum TaskState
	{
		None,
		AddToQueue,
		InProcess,
		Done,
	}

	public class PathfindTask
	{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER
		private const float TaskExecutingTime = 5.0f;
#elif UNITY_IPHONE || UNITY_ANDROID
		private const float TaskExecutingTime = 10.0f;
#endif

		private readonly Map _map;
		private readonly Point _start;
		private readonly Point _goal;
		public readonly int PlayerId;
		private readonly AIPlayer.AIPathTask _task;

		private TaskPriority _priority;

		public TaskState State;

		private float _startTimeExecute;

		private Coroutine _executingTask;

		public PathfindTask(Map map, Point start, Point goal, int playerId, AIPlayer.AIPathTask task)
		{
			_map = map;
			_start = start;
			_goal = goal;
			PlayerId = playerId;
			_task = task;
			_priority = TaskPriority.HighPriority;
			State = TaskState.AddToQueue;
			_executingTask = null;
		}

		public static PathfindTask CreatePathFindTask(Map map, Point start, Point goal, int playerId, AIPlayer.AIPathTask task)
		{
			return new PathfindTask(map, start, goal, playerId, task);
		}

		public void ExecuteTask(float startTime)
		{
			State = TaskState.InProcess;

			ExecutingTaskCounter++;

			_startTimeExecute = startTime;
			
			//Debug.Log(string.Format("Start task executing, player: {1}, count: {0}, currentexecuting task: {2}",
			//					_pathfindTasks.Count, PlayerId, ExecutingTaskCounter));

			_executingTask = Instance.StartCoroutine(FindPath(_map, _start, _goal, PlayerId, _task));
		}

		public void DoneTask(List<Point> wayPoints)
		{
			State = TaskState.Done;

			ExecutingTaskCounter--;
			if (ExecutingTaskCounter < 0)
				ExecutingTaskCounter = 0;

			//Debug.Log(string.Format("Task was done, player: {1}, count of way point is: {0}, currentexecuting task: {2}",
			//					wayPoints.Count, PlayerId, ExecutingTaskCounter));

			AIPlayer player = GameSetObserver.Instance.GetPlayer(PlayerId) as AIPlayer;

			if (PathReturn != null)
			{
		//		Debug.Log(string.Format("Task was done, player: {1}, count of way point is: {0}, currentexecuting task: {2}",
		//		                        wayPoints.Count, PlayerId, ExecutingTaskCounter));

				if (player != null)
				{
					player.PathfindState = AIPlayer.PathfinderState.ReturnPathOk;
				}

				PathReturn(wayPoints, PlayerId, _task);
			}
			else
			{
				if (player != null)
				{
					player.PathfindState = AIPlayer.PathfinderState.ReturnPathError;
				}

		//		Debug.LogError(string.Format("Pathreturn is null! Id: {0}", PlayerId));
			}

			StopTask(false);
		}

		public void StopTask(bool needReturnNullPath)
		{
			State = TaskState.Done;

			ExecutingTaskCounter--;
			if (ExecutingTaskCounter < 0)
				ExecutingTaskCounter = 0;
			if (_executingTask != null)
				Instance.StopCoroutine(_executingTask.ToString());

			if (needReturnNullPath && PathReturn != null)
			{
		//		Debug.Log(string.Format("Return null path for id: {0}", PlayerId));
				PathReturn(null, PlayerId, _task);
			}

			_executingTask = null;
		}

		public bool IsTheTaskIsExecutingLongTime(float currentTime)
		{
			return currentTime - _startTimeExecute >= TaskExecutingTime;
		}

		public override string ToString ()
		{
			string str = string.Format("Start: {0}, end:{1}, id:{2}, pathTask:{3}", _start, _goal, PlayerId, _task);
			return str;
		}
	}

	private static List<PathfindTask> _pathfindTasks;
	private static PathfindTask _currentTask;

	private const int ExecutingTaskCount = 3;
	private static int ExecutingTaskCounter = 0;

	public static void StartPathCalculate(Map map, Point start, Point goal, int playerId, AIPlayer.AIPathTask task)
	{
		//Instance.StartCoroutine(FindPath(map, start, goal, playerId, task));

		PathfindTask pathfindTask = Instance.GetPathfindTask(playerId);

		if (pathfindTask != null)
		{
			//Debug.Log(string.Format("Player with Id: {0} already has task to find the path", playerId));	
			pathfindTask.StopTask(false);
			return;
		}

		pathfindTask = PathfindTask.CreatePathFindTask(map, start, goal, playerId, task);
		
		_pathfindTasks.Add(pathfindTask);
		//Debug.Log(string.Format("Add path find task for player: {1}, count: {0}, currentexecuting task: {2}",
		//						_pathfindTasks.Count, playerId, ExecutingTaskCounter));
	}

	public void StopPathfind(int playerId, bool needReturnNullPathFind)
	{
		PathfindTask executingTask = GetPathfindTask(playerId);

		if (executingTask != null)
		{
			_pathfindTasks.Remove(executingTask);
			executingTask.StopTask(needReturnNullPathFind);

			//Debug.Log(string.Format("Stop task and remove from queue, player: {1}, count: {0}, currentexecuting task: {2}",
			//					_pathfindTasks.Count, playerId, ExecutingTaskCounter));
		}

	}

	public void EndTask(int playerId, List<Point> wayPoints)
	{
		PathfindTask endTask = GetPathfindTask(playerId);

		if (endTask != null)
		{
			_pathfindTasks.Remove(endTask);
			endTask.DoneTask(wayPoints);
		}

	//	Debug.Log(string.Format("Task done and remove from queue player: {0}, endTask: {1}", playerId, endTask));
	}

	private PathfindTask GetPathfindTask(int playerId)
	{
		return _pathfindTasks.FirstOrDefault(task => task.PlayerId == playerId);
	}


	#endregion

	#endregion


	#region MonoBehavior events

	// Use this for initialization

	private void Awake()
	{

		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			Instance = this;
		}

		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		//AIPlayer.FindPathEvent += StartPathCalculate;
		_pathfindTasks = new List<PathfindTask>();
	}

	// Update is called once per frame
	private void Update()
	{
		if (_pathfindTasks.Count > 0)
		{
			if (ExecutingTaskCounter < ExecutingTaskCount)
			{
				_currentTask = _pathfindTasks.FirstOrDefault(task => task.State == TaskState.AddToQueue);
				if (_currentTask != null)
					_currentTask.ExecuteTask(Time.time);
			}

			for (int i = 0; i < _pathfindTasks.Count; i++)
			{
				PathfindTask pathfindTask = _pathfindTasks[i];

				if (pathfindTask.State == TaskState.InProcess && pathfindTask.IsTheTaskIsExecutingLongTime(Time.time))
				{
					StopPathfind(pathfindTask.PlayerId, true);
				}
			}
		}
	}

	#endregion

	public static int iteratorCount = 0;

	public static IEnumerator FindPath(Map map, Point start, Point goal, int playerId, AIPlayer.AIPathTask task)
	{

		//Debug.Log(string.Format("Path start to find, player: {0}, start point is: {1}, is it available o navigate: {2}", playerId,
		//						start,//Map.GetWorldPosition(map, start),
		//						Map.IsPointAccessableToNavigate(map, start, GameSetObserver.Instance.GetPlayer(playerId))));

		// Шаг 1.
		iteratorCount = 0;
		var closedSet = new Collection<PathNode>();
		var openSet = new Collection<PathNode>();
		// Шаг 2.
		PathNode startNode = new PathNode()
			{
				Position = start,
				CameFrom = null,
				PathLengthFromStart = 0,
				HeuristicEstimatePathLength = 3//Pathfinding.GetHeuristicPathLength(start, goal)
			};

		int counter = 0;

		openSet.Add(startNode);
		//Debug.Log(string.Format("Pathfind for player: {0}, step 2", playerId));
		while (openSet.Count > 0)
		{
			counter++;

			if (counter > 10000)
				counter = 0;

			if (counter%FrameIteratorsAdd == 0)
			{
				yield return null;
			}
			// Шаг 3.
			var currentNode = openSet.OrderBy(node => node.EstimateFullPathLength).First();
			// Шаг 4.
			if (currentNode.Position == goal)
			{
				//if (PathReturn != null)
				//{
				//	PathReturn(Pathfinding.GetPathForNode(currentNode), playerId, task);
				//}

				Instance.EndTask(playerId, new List<Point>());/*Pathfinding.GetPathForNode(currentNode)*/

				yield break;
			}
			//return Pathfinding.GetPathForNode(currentNode);
			// Шаг 5.
			openSet.Remove(currentNode);
			closedSet.Add(currentNode);
			Player player = GameSetObserver.Instance.GetPlayer(playerId);

			//Debug.Log(string.Format("Pathfind for player: {0}, step 5", playerId));

			Collection<PathNode> neighboursNode = new Collection<PathNode>();//Pathfinding.GetNeighbours(currentNode, goal, map, player,
			//																GameSetObserver.Instance.Mechanics ==
			//																MechanicsType.Classic);

			// Шаг 6.
			//Debug.Log(string.Format("Pathfind for player: {0}, step 6, neighbours count: {1}", playerId, neighboursNode.Count));
			for (int i = 0; i < neighboursNode.Count; i++)
			{
				iteratorCount ++;

				//yield return null;
				if (iteratorCount%FrameIterators == 0)
					yield return null;

				var neighbourNode = neighboursNode[i];
// Шаг 7.
				if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0)
				{
					continue;
				}
				PathNode openNode = null; // = openSet.FirstOrDefault(node => node.Position == neighbourNode.Position);

				for (int j = 0; j < openSet.Count; j++)
				{
					iteratorCount ++;
					//if (iteratorCount%FrameIterators == 0)
					//	yield return null;

					if (openSet[j].Position == neighbourNode.Position)
					{
						openNode = openSet[j];
						break;
					}
					//openNode = null;
				}
				// Шаг 8.
				//Debug.Log(string.Format("Pathfind for player: {0}, step 8", playerId));
				if (openNode == null)
				{
					openSet.Add(neighbourNode);
				}
				else if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
				{
					// Шаг 9.
					//Debug.Log(string.Format("Pathfind for player: {0}, step 9", playerId));
					openNode.CameFrom = currentNode;
					openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
				}


				//yield return new WaitForEndOfFrame();
			}
		}
		// Шаг 10.
		
		//if (PathReturn != null)
		//{
		//	PathReturn(null, playerId, task);
		//}

		//PathfindTask pathfindTask = Instance.GetPathfindTask(playerId);

		//f (pathfindTask != null)
		//Debug.Log(string.Format("Path was't find, player: {0}, start point is: {1}, is it available o navigate: {2}", playerId,
		//						start,//Map.GetWorldPosition(map, start),
		//						Map.IsPointAccessableToNavigate(map, start, GameSetObserver.Instance.GetPlayer(playerId))));
		Instance.StopPathfind(playerId, true);

		yield return null;
	}
}
