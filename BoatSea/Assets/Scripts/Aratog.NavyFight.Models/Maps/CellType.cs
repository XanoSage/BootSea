using System;

namespace Aratog.NavyFight.Models.Maps
{
	[Flags]
	public enum CellType
	{
		None = 0x00000,
		Static = 0x00001,
		StaticGround = 0x00002,
		Destructable = 0x00004,
		SpawnPoint = 0x00008,
		FlagPoint = 0x00010,
		AINavigatePoint = 0x00020,
		GunForwarding = 0x00040,
		GunRotating = 0x00080,
		GunMounted = 0x00100,
		CamperPoint = 0x00200,
		Tree = 0x00400,
		Bonus = 0x00800,
		Bomb = 0x01000,
		Shell = 0x02000,
		Ship = 0x04000,
	}
}

//how to work with flags
/*
private void DisplayUi (MenuUiControllsType ui, bool show) {
		if (show)
			displayingMenuUis |= ui;
		else 
			displayingMenuUis &= ~ui;
	}

	public void SimulateUiResponse (UiResponseType type, params int[] args) {
		CallResponse(type, args);
	}

	private bool IsCommonUiDisplaying (CommonUiControllsType ui) {
		return (displayingCommonUis & ui) == ui;
	}*/