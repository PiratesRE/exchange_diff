using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiNotificationHandle
	{
		internal MapiNotificationHandle(MapiNotificationHandler handler)
		{
			this.handler = handler;
		}

		public IntPtr Connection
		{
			get
			{
				return this.connection;
			}
		}

		public ulong NotificationCallbackId
		{
			get
			{
				return this.notificationCallbackId;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.connection != IntPtr.Zero;
			}
		}

		internal MapiNotificationHandler Handler
		{
			get
			{
				return this.handler;
			}
		}

		internal void SetConnection(IntPtr connection, ulong notificationCallbackId)
		{
			if (this.IsValid)
			{
				throw new InvalidOperationException("SetConnection should only be called once");
			}
			this.connection = connection;
			this.notificationCallbackId = notificationCallbackId;
		}

		internal void MarkAsInvalid()
		{
			this.connection = IntPtr.Zero;
		}

		private IntPtr connection;

		private ulong notificationCallbackId;

		private readonly MapiNotificationHandler handler;
	}
}
