using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NotifyResult : Result
	{
		internal NotifyResult(ServerObjectHandle notificationHandle, byte logonId, Encoding string8Encoding, Notification notificationData) : base(RopId.Notify)
		{
			this.NotificationHandle = notificationHandle;
			this.LogonId = logonId;
			this.NotificationData = notificationData;
			base.String8Encoding = string8Encoding;
		}

		internal NotifyResult(ServerObjectHandle notificationHandle, byte logonId, int codePage, Notification notificationData) : base(RopId.Notify)
		{
			this.NotificationHandle = notificationHandle;
			this.LogonId = logonId;
			this.NotificationData = notificationData;
			base.String8Encoding = CodePageMap.GetEncoding(codePage);
		}

		internal static bool TryParse(Reader reader, IDictionary<ServerObjectHandle, PropertyTag[]> columnsDictionary, Func<ServerObjectHandle, Encoding> getEncoding, out NotifyResult notifyResult)
		{
			NotifyResult notifyResult2 = new NotifyResult(reader, columnsDictionary, getEncoding);
			if (notifyResult2.NotificationData == null)
			{
				notifyResult = null;
				return false;
			}
			notifyResult = notifyResult2;
			return true;
		}

		private NotifyResult(Reader reader, IDictionary<ServerObjectHandle, PropertyTag[]> columnsDictionary, Func<ServerObjectHandle, Encoding> getEncoding) : base(reader)
		{
			this.NotificationHandle = ServerObjectHandle.Parse(reader);
			this.LogonId = reader.ReadByte();
			PropertyTag[] originalPropertyTags;
			if (!columnsDictionary.TryGetValue(this.NotificationHandle, out originalPropertyTags))
			{
				originalPropertyTags = null;
			}
			this.NotificationData = Notification.Parse(reader, originalPropertyTags, getEncoding(this.NotificationHandle));
		}

		internal ServerObjectHandle NotificationHandle { get; private set; }

		internal byte LogonId { get; private set; }

		internal Notification NotificationData { get; private set; }

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			this.NotificationHandle.Serialize(writer);
			writer.WriteByte(this.LogonId);
			this.NotificationData.Serialize(writer, base.String8Encoding);
		}
	}
}
