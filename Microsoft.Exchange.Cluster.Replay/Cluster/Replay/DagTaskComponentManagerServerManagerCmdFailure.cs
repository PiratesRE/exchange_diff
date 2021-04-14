using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskComponentManagerServerManagerCmdFailure : DagTaskServerException
	{
		public DagTaskComponentManagerServerManagerCmdFailure(string error) : base(ReplayStrings.DagTaskComponentManagerServerManagerCmdFailure(error))
		{
			this.error = error;
		}

		public DagTaskComponentManagerServerManagerCmdFailure(string error, Exception innerException) : base(ReplayStrings.DagTaskComponentManagerServerManagerCmdFailure(error), innerException)
		{
			this.error = error;
		}

		protected DagTaskComponentManagerServerManagerCmdFailure(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
