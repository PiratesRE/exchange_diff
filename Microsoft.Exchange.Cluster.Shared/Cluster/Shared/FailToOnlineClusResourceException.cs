using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailToOnlineClusResourceException : ClusCommonFailException
	{
		public FailToOnlineClusResourceException(string groupName, string resourceId, string reason) : base(Strings.FailToOnlineClusResourceException(groupName, resourceId, reason))
		{
			this.groupName = groupName;
			this.resourceId = resourceId;
			this.reason = reason;
		}

		public FailToOnlineClusResourceException(string groupName, string resourceId, string reason, Exception innerException) : base(Strings.FailToOnlineClusResourceException(groupName, resourceId, reason), innerException)
		{
			this.groupName = groupName;
			this.resourceId = resourceId;
			this.reason = reason;
		}

		protected FailToOnlineClusResourceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.groupName = (string)info.GetValue("groupName", typeof(string));
			this.resourceId = (string)info.GetValue("resourceId", typeof(string));
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("groupName", this.groupName);
			info.AddValue("resourceId", this.resourceId);
			info.AddValue("reason", this.reason);
		}

		public string GroupName
		{
			get
			{
				return this.groupName;
			}
		}

		public string ResourceId
		{
			get
			{
				return this.resourceId;
			}
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly string groupName;

		private readonly string resourceId;

		private readonly string reason;
	}
}
