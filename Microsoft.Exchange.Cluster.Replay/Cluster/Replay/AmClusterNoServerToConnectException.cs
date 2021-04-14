using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmClusterNoServerToConnectException : ClusterException
	{
		public AmClusterNoServerToConnectException(string dagName) : base(ReplayStrings.AmClusterNoServerToConnect(dagName))
		{
			this.dagName = dagName;
		}

		public AmClusterNoServerToConnectException(string dagName, Exception innerException) : base(ReplayStrings.AmClusterNoServerToConnect(dagName), innerException)
		{
			this.dagName = dagName;
		}

		protected AmClusterNoServerToConnectException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dagName = (string)info.GetValue("dagName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dagName", this.dagName);
		}

		public string DagName
		{
			get
			{
				return this.dagName;
			}
		}

		private readonly string dagName;
	}
}
