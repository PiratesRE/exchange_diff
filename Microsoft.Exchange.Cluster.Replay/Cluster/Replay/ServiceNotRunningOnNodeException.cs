using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServiceNotRunningOnNodeException : SeedPrepareException
	{
		public ServiceNotRunningOnNodeException(string serviceName, string nodeName) : base(ReplayStrings.ServiceNotRunningOnNodeException(serviceName, nodeName))
		{
			this.serviceName = serviceName;
			this.nodeName = nodeName;
		}

		public ServiceNotRunningOnNodeException(string serviceName, string nodeName, Exception innerException) : base(ReplayStrings.ServiceNotRunningOnNodeException(serviceName, nodeName), innerException)
		{
			this.serviceName = serviceName;
			this.nodeName = nodeName;
		}

		protected ServiceNotRunningOnNodeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serviceName = (string)info.GetValue("serviceName", typeof(string));
			this.nodeName = (string)info.GetValue("nodeName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serviceName", this.serviceName);
			info.AddValue("nodeName", this.nodeName);
		}

		public string ServiceName
		{
			get
			{
				return this.serviceName;
			}
		}

		public string NodeName
		{
			get
			{
				return this.nodeName;
			}
		}

		private readonly string serviceName;

		private readonly string nodeName;
	}
}
