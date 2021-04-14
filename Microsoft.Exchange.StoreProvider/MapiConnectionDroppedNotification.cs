using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiConnectionDroppedNotification : MapiNotification
	{
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

		internal unsafe MapiConnectionDroppedNotification(NOTIFICATION* notification) : base(notification)
		{
			if (notification->info.drop.lpszServerDN != IntPtr.Zero)
			{
				this.serverDN = Marshal.PtrToStringAnsi(notification->info.drop.lpszServerDN);
			}
			if (notification->info.drop.lpszUserDN != IntPtr.Zero)
			{
				this.userDN = Marshal.PtrToStringAnsi(notification->info.drop.lpszUserDN);
			}
			this.tickDeath = notification->info.drop.dwTickDeath;
		}

		private readonly string serverDN;

		private readonly string userDN;

		private readonly int tickDeath;
	}
}
