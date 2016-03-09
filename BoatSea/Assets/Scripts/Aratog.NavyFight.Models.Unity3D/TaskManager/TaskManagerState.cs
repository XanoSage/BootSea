using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;

namespace Aratog.NavyFight.Models.Unity3D.TaskManager {
	public enum TaskManagerState {
		None, 
		Ready, 
		Working, 
		Finished,
 		Pause,
		Error
	}
}
