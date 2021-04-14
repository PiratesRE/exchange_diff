using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoPAMDesignatedException : ThirdPartyReplicationException
	{
		public NoPAMDesignatedException() : base(ThirdPartyReplication.NoPAMDesignated)
		{
		}

		public NoPAMDesignatedException(Exception innerException) : base(ThirdPartyReplication.NoPAMDesignated, innerException)
		{
		}

		protected NoPAMDesignatedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
