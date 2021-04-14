using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiExtendedNotification : MapiNotification
	{
		public int EventType
		{
			get
			{
				return this.eventType;
			}
		}

		public byte[] EventParameters
		{
			get
			{
				return this.eventParameters;
			}
		}

		internal unsafe MapiExtendedNotification(NOTIFICATION* notification) : base(notification)
		{
			this.eventType = notification->info.ext.ulEvent;
			this.eventParameters = new byte[notification->info.ext.cb];
			if (this.eventParameters.Length > 0)
			{
				Marshal.Copy(notification->info.ext.pbEventParameters, this.eventParameters, 0, this.eventParameters.Length);
			}
		}

		private readonly int eventType;

		private readonly byte[] eventParameters;
	}
}
