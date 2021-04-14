using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClusterNoServerToConnectException : ClusterException
	{
		public ClusterNoServerToConnectException(string dagName) : base(Strings.ClusterNoServerToConnect(dagName))
		{
			this.dagName = dagName;
		}

		public ClusterNoServerToConnectException(string dagName, Exception innerException) : base(Strings.ClusterNoServerToConnect(dagName), innerException)
		{
			this.dagName = dagName;
		}

		protected ClusterNoServerToConnectException(SerializationInfo info, StreamingContext context) : base(info, context)
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
