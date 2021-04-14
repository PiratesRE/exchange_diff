using System;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NotificationData
	{
		internal NotificationData(ServerObjectHandle handleValue, Logon logon, Notification notification, StoreId? rootFolderId, View view, Encoding string8Encoding)
		{
			this.notificationHandleValue = handleValue;
			this.logon = logon;
			this.notification = notification;
			this.rootFolderId = rootFolderId;
			this.view = view;
			this.string8Encoding = string8Encoding;
		}

		internal View View
		{
			get
			{
				return this.view;
			}
		}

		internal ServerObjectHandle NotificationHandleValue
		{
			get
			{
				return this.notificationHandleValue;
			}
		}

		internal Logon Logon
		{
			get
			{
				return this.logon;
			}
		}

		internal Notification Notification
		{
			get
			{
				return this.notification;
			}
		}

		internal StoreId? RootFolderId
		{
			get
			{
				return this.rootFolderId;
			}
		}

		internal Encoding String8Encoding
		{
			get
			{
				return this.string8Encoding;
			}
		}

		public override string ToString()
		{
			return string.Format("Logon = {0}, handle = {1}, type = {2}, encoding = {3}, data = {4}", new object[]
			{
				this.logon,
				this.notificationHandleValue,
				(this.view == null) ? "Object" : "Table",
				this.string8Encoding.EncodingName,
				this.notification
			});
		}

		private readonly ServerObjectHandle notificationHandleValue;

		private readonly Logon logon;

		private readonly Notification notification;

		private readonly StoreId? rootFolderId;

		private readonly View view;

		private readonly Encoding string8Encoding;
	}
}
