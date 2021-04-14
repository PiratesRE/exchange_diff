using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RemoveDagFailedToDestroyClusterException : LocalizedException
	{
		public RemoveDagFailedToDestroyClusterException(string clusterName, string dagName, uint status) : base(Strings.RemoveDagFailedToDestroyClusterException(clusterName, dagName, status))
		{
			this.clusterName = clusterName;
			this.dagName = dagName;
			this.status = status;
		}

		public RemoveDagFailedToDestroyClusterException(string clusterName, string dagName, uint status, Exception innerException) : base(Strings.RemoveDagFailedToDestroyClusterException(clusterName, dagName, status), innerException)
		{
			this.clusterName = clusterName;
			this.dagName = dagName;
			this.status = status;
		}

		protected RemoveDagFailedToDestroyClusterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.clusterName = (string)info.GetValue("clusterName", typeof(string));
			this.dagName = (string)info.GetValue("dagName", typeof(string));
			this.status = (uint)info.GetValue("status", typeof(uint));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("clusterName", this.clusterName);
			info.AddValue("dagName", this.dagName);
			info.AddValue("status", this.status);
		}

		public string ClusterName
		{
			get
			{
				return this.clusterName;
			}
		}

		public string DagName
		{
			get
			{
				return this.dagName;
			}
		}

		public uint Status
		{
			get
			{
				return this.status;
			}
		}

		private readonly string clusterName;

		private readonly string dagName;

		private readonly uint status;
	}
}
