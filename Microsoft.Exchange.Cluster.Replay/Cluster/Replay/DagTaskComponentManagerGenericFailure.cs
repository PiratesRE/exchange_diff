using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskComponentManagerGenericFailure : DagTaskServerException
	{
		public DagTaskComponentManagerGenericFailure(int error) : base(ReplayStrings.DagTaskComponentManagerGenericFailure(error))
		{
			this.error = error;
		}

		public DagTaskComponentManagerGenericFailure(int error, Exception innerException) : base(ReplayStrings.DagTaskComponentManagerGenericFailure(error), innerException)
		{
			this.error = error;
		}

		protected DagTaskComponentManagerGenericFailure(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (int)info.GetValue("error", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public int Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly int error;
	}
}
