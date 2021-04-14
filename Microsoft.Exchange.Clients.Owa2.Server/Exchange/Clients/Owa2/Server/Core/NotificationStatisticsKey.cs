using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class NotificationStatisticsKey
	{
		public NotificationLocation Location { get; private set; }

		public Type PayloadType { get; private set; }

		public bool IsReload { get; private set; }

		public NotificationStatisticsKey(NotificationLocation location, Type payloadType, bool isReload)
		{
			if (location == null)
			{
				throw new ArgumentNullException("location");
			}
			if (payloadType == null)
			{
				throw new ArgumentNullException("payloadType");
			}
			this.Location = location;
			this.PayloadType = payloadType;
			this.IsReload = isReload;
		}

		public override int GetHashCode()
		{
			return this.Location.GetHashCode() ^ this.PayloadType.GetHashCode() ^ this.IsReload.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			NotificationStatisticsKey notificationStatisticsKey = obj as NotificationStatisticsKey;
			return notificationStatisticsKey != null && (this.Location.Equals(notificationStatisticsKey.Location) && this.PayloadType.Equals(notificationStatisticsKey.PayloadType)) && this.IsReload.Equals(notificationStatisticsKey.IsReload);
		}
	}
}
