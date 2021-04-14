using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClusterNotInstalledException : ClusterException
	{
		public ClusterNotInstalledException(string nodeName) : base(Strings.ClusterNotInstalledException(nodeName))
		{
			this.nodeName = nodeName;
		}

		public ClusterNotInstalledException(string nodeName, Exception innerException) : base(Strings.ClusterNotInstalledException(nodeName), innerException)
		{
			this.nodeName = nodeName;
		}

		protected ClusterNotInstalledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.nodeName = (string)info.GetValue("nodeName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("nodeName", this.nodeName);
		}

		public string NodeName
		{
			get
			{
				return this.nodeName;
			}
		}

		private readonly string nodeName;
	}
}
