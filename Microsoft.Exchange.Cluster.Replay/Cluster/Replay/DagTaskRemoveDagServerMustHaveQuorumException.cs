using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskRemoveDagServerMustHaveQuorumException : DagTaskServerException
	{
		public DagTaskRemoveDagServerMustHaveQuorumException(string dagName) : base(ReplayStrings.DagTaskRemoveDagServerMustHaveQuorumException(dagName))
		{
			this.dagName = dagName;
		}

		public DagTaskRemoveDagServerMustHaveQuorumException(string dagName, Exception innerException) : base(ReplayStrings.DagTaskRemoveDagServerMustHaveQuorumException(dagName), innerException)
		{
			this.dagName = dagName;
		}

		protected DagTaskRemoveDagServerMustHaveQuorumException(SerializationInfo info, StreamingContext context) : base(info, context)
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
