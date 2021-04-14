using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskQuorumNotAchievedException : DagTaskServerException
	{
		public DagTaskQuorumNotAchievedException(string dagName) : base(ReplayStrings.DagTaskQuorumNotAchievedException(dagName))
		{
			this.dagName = dagName;
		}

		public DagTaskQuorumNotAchievedException(string dagName, Exception innerException) : base(ReplayStrings.DagTaskQuorumNotAchievedException(dagName), innerException)
		{
			this.dagName = dagName;
		}

		protected DagTaskQuorumNotAchievedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
