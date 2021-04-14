using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskProblemChangingQuorumExceptionBadNetName : LocalizedException
	{
		public DagTaskProblemChangingQuorumExceptionBadNetName(string clusterName, string fsw, Exception ex) : base(Strings.DagTaskProblemChangingQuorumExceptionBadNetName(clusterName, fsw, ex))
		{
			this.clusterName = clusterName;
			this.fsw = fsw;
			this.ex = ex;
		}

		public DagTaskProblemChangingQuorumExceptionBadNetName(string clusterName, string fsw, Exception ex, Exception innerException) : base(Strings.DagTaskProblemChangingQuorumExceptionBadNetName(clusterName, fsw, ex), innerException)
		{
			this.clusterName = clusterName;
			this.fsw = fsw;
			this.ex = ex;
		}

		protected DagTaskProblemChangingQuorumExceptionBadNetName(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.clusterName = (string)info.GetValue("clusterName", typeof(string));
			this.fsw = (string)info.GetValue("fsw", typeof(string));
			this.ex = (Exception)info.GetValue("ex", typeof(Exception));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("clusterName", this.clusterName);
			info.AddValue("fsw", this.fsw);
			info.AddValue("ex", this.ex);
		}

		public string ClusterName
		{
			get
			{
				return this.clusterName;
			}
		}

		public string Fsw
		{
			get
			{
				return this.fsw;
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

		private readonly string fsw;

		private readonly Exception ex;
	}
}
