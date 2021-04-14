using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskOperationFailedWithEcException : DagTaskServerException
	{
		public DagTaskOperationFailedWithEcException(int ec) : base(ReplayStrings.DagTaskOperationFailedWithEcException(ec))
		{
			this.ec = ec;
		}

		public DagTaskOperationFailedWithEcException(int ec, Exception innerException) : base(ReplayStrings.DagTaskOperationFailedWithEcException(ec), innerException)
		{
			this.ec = ec;
		}

		protected DagTaskOperationFailedWithEcException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ec = (int)info.GetValue("ec", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ec", this.ec);
		}

		public int Ec
		{
			get
			{
				return this.ec;
			}
		}

		private readonly int ec;
	}
}
