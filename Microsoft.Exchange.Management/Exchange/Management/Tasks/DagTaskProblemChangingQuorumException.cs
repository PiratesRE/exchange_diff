using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskProblemChangingQuorumException : LocalizedException
	{
		public DagTaskProblemChangingQuorumException(string clusterName, Exception ex) : base(Strings.DagTaskProblemChangingQuorumException(clusterName, ex))
		{
			this.clusterName = clusterName;
			this.ex = ex;
		}

		public DagTaskProblemChangingQuorumException(string clusterName, Exception ex, Exception innerException) : base(Strings.DagTaskProblemChangingQuorumException(clusterName, ex), innerException)
		{
			this.clusterName = clusterName;
			this.ex = ex;
		}

		protected DagTaskProblemChangingQuorumException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.clusterName = (string)info.GetValue("clusterName", typeof(string));
			this.ex = (Exception)info.GetValue("ex", typeof(Exception));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("clusterName", this.clusterName);
			info.AddValue("ex", this.ex);
		}

		public string ClusterName
		{
			get
			{
				return this.clusterName;
			}
		}

		public Exception Ex
		{
			get
			{
				return this.ex;
			}
		}

		private readonly string clusterName;

		private readonly Exception ex;
	}
}
