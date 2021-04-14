using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Mobility;

namespace Microsoft.Exchange.Management.PushNotifications
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CannotResolveFallbackPartition : LocalizedException
	{
		public CannotResolveFallbackPartition(string appId, string currentPartition) : base(Strings.PushNotificationFailedToResolveFallbackPartition(appId, currentPartition))
		{
			this.appId = appId;
			this.currentPartition = currentPartition;
		}

		public CannotResolveFallbackPartition(string appId, string currentPartition, Exception innerException) : base(Strings.PushNotificationFailedToResolveFallbackPartition(appId, currentPartition), innerException)
		{
			this.appId = appId;
			this.currentPartition = currentPartition;
		}

		protected CannotResolveFallbackPartition(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.appId = (string)info.GetValue("appId", typeof(string));
			this.currentPartition = (string)info.GetValue("currentPartition", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("appId", this.appId);
			info.AddValue("currentPartition", this.currentPartition);
		}

		public string AppId
		{
			get
			{
				return this.appId;
			}
		}

		public string CurrentPartition
		{
			get
			{
				return this.currentPartition;
			}
		}

		private readonly string appId;

		private readonly string currentPartition;
	}
}
