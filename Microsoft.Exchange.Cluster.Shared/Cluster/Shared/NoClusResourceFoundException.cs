using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoClusResourceFoundException : ClusCommonFailException
	{
		public NoClusResourceFoundException(string groupName, string resourceName) : base(Strings.NoClusResourceFoundException(groupName, resourceName))
		{
			this.groupName = groupName;
			this.resourceName = resourceName;
		}

		public NoClusResourceFoundException(string groupName, string resourceName, Exception innerException) : base(Strings.NoClusResourceFoundException(groupName, resourceName), innerException)
		{
			this.groupName = groupName;
			this.resourceName = resourceName;
		}

		protected NoClusResourceFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.groupName = (string)info.GetValue("groupName", typeof(string));
			this.resourceName = (string)info.GetValue("resourceName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("groupName", this.groupName);
			info.AddValue("resourceName", this.resourceName);
		}

		public string GroupName
		{
			get
			{
				return this.groupName;
			}
		}

		public string ResourceName
		{
			get
			{
				return this.resourceName;
			}
		}

		private readonly string groupName;

		private readonly string resourceName;
	}
}
