using UnityEngine;
using System.Collections.Generic;

namespace ShipsGlobal
{
	public enum GameState
	{
		GAME,
		PRESTART
	}	

    public enum Direction
    {
		NONE,
        //
        UPLEFT,
        UPRIGHT,
        DOWNLEFT,
        DOWNRIGHT,
        //
        RIGHT,
        UP,
        LEFT,
        DOWN,
		DNIWE
    }

	public enum ShipType : byte
	{
		Small = 0,
		Middle = 1,
		Big = 2,
		BigShip=3,
		SmallMetal=4,
		MiddleMetal=5,
		BigMetal=6,
		SmallAtlant=7,
		MiddleAtlant=8,
		BigAtlant=9,
		SmallDark=10,
		MiddleDark=11,
		BigDark=12,

	}
	
	public enum TeamType : byte
	{
		Red = 1,
		Blue = 2
	}
	
	public enum AIType : byte
	{
		HumanControls = 0,
		BaseDefense = 1,
		FollowMe = 2,
		CaptureEnemy = 3,
		GuardZone = 4
	}
	
	public enum AITactics : byte
	{
		ThreeD_OneA = 0, // 3 охраняют флаг - 1 нападает
		TwoD_TwoA = 1, // 2 охраняют флаг - 2 нападают
		OneD_ThreeA = 2, // 1 охраняет флаг - 3 нападают
		TEST = 3
	}
} 