using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskClusterServiceNotRunningOnNodeException : LocalizedException
	{
		public DagTaskClusterServiceNotRunningOnNodeException(string nodeName) : base(Strings.DagTaskClusterServiceNotRunningOnNodeException(nodeName))
		{
			this.nodeName = nodeName;
		}

		public DagTaskClusterServiceNotRunningOnNodeException(string nodeName, Exception innerException) : base(Strings.DagTaskClusterServiceNotRunningOnNodeException(nodeName), innerException)
		{
			this.nodeName = nodeName;
		}

		protected DagTaskClusterServiceNotRunningOnNodeException(SerializationInfo info, StreamingContext context) : base(info, context)
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
