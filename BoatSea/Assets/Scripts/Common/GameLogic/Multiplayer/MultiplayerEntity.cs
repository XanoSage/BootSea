using Aratog.NavyFight.Models.Unity3D.Players;

namespace Assets.Scripts.Common.GameLogic.Multiplayer {
	public class MultiplayerEntity {
		public PhotonPlayer photonPlayer;
		public Player player;
		public int playerId;
	}
}