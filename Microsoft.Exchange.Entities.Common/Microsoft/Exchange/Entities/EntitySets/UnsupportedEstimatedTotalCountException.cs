using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Entities.EntitySets
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnsupportedEstimatedTotalCountException : NotSupportedException
	{
		public UnsupportedEstimatedTotalCountException() : base(Strings.UnsupportedEstimatedTotalCount)
		{
		}

		public UnsupportedEstimatedTotalCountException(Exception innerException) : base(Strings.UnsupportedEstimatedTotalCount, innerException)
		{
		}

		protected UnsupportedEstimatedTotalCountException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
