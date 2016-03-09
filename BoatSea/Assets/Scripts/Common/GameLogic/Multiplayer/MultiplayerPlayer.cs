using Aratog.NavyFight.Models.Unity3D.Players;

namespace Assets.Scripts.Common.GameLogic.Multiplayer {
	public class MultiplayerPlayer : Player {

		public MultiplayerEntity Entity { get; private set; }

		public bool IsReady { get; private set; }

		public bool NeedToSetIsReady;

		public MultiplayerPlayer (MultiplayerEntity multiplayerEntity,TeamColor team = TeamColor.BlueTeam) : base(true, team) {
			Type = PlayerType.MultiplayerPlayer;
			Entity = multiplayerEntity;
			IsReady = false;
			NeedToSetIsReady = false;
		}

		public static MultiplayerPlayer Create (MultiplayerEntity multiplayerEntity, TeamColor team = TeamColor.BlueTeam) {
		 	return new MultiplayerPlayer(multiplayerEntity, team);
		}

		public void SetIsReady (bool isReady, bool fromServer = false) {
			if (MyShip == null)
				return;

			IsReady = isReady;

			//TODO:: add sending IsReady information to the others player in the party
			if (fromServer)
				return;
			MultiplayerManager.Instance.SetPlayerIsReady(IsReady);
		}

		public override void Serialize (DataBuffer buffer) {
			base.Serialize(buffer);

			buffer.Write(IsReady);
		}

		public override void Deserialize (DataBuffer buffer) {
			base.Deserialize(buffer);

			IsReady = buffer.ReadBool();
		}
	}
}
