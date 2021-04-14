using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ConnectionDroppedNotification : Notification
	{
		internal ConnectionDroppedNotification(string serverDN, string userDN, int tickDeath) : base(NotificationType.ConnectionDropped)
		{
			this.serverDN = serverDN;
			this.userDN = userDN;
			this.tickDeath = tickDeath;
		}

		public string ServerDN
		{
			get
			{
				return this.serverDN;
			}
		}

		public string UserDN
		{
			get
			{
				return this.userDN;
			}
		}

		public int TickDeath
		{
			get
			{
				return this.tickDeath;
			}
		}

		private readonly string serverDN;

		private readonly string userDN;

		private readonly int tickDeath;
	}
}
