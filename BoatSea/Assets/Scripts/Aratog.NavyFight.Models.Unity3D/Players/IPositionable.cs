using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.Players
{
	public interface IPositionable
	{

		Vector3 Position { get; set; }
	}
}
