using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;

namespace Aratog.NavyFight.Models.Unity3D.Players {
	/// <summary>
	///     Allowed information for user authentication.
	/// </summary>
	public class UserLogin /*: DataEntryBase*/ {

		#region Variables and Properties

		/// <summary>
		///     User record ID.
		/// </summary>
		public Guid UserId { get; set; }

		/// <summary>
		///     User authentication type for this login.
		/// </summary>
		//public UserAuthType AuthType { get; set; }

		/// <summary>
		///     User simple login name, device unique identifier, etc.
		/// </summary>
		public string AuthId { get; set; }

		/// <summary>
		///     Public key. You should generate new public key if you want to reset all authentication tickets on all your connected devices.
		/// </summary>
		public Guid AuthPublicKey { get; set; }

		/// <summary>
		///     Password salt for password hash, etc.
		/// </summary>
		public string AuthPrivateKey { get; set; }

		/// <summary>
		///     User password hash, auth token, etc.
		/// </summary>
		public string AuthToken { get; set; }

		/// <summary>
		///     Previous log on date and time.
		/// </summary>
		public DateTime PrevLogonDate { get; set; }

		/// <summary>
		///     Last log on date and time.
		/// </summary>
		public DateTime LastLogonDate { get; set; }

		#endregion

	}
}

	
