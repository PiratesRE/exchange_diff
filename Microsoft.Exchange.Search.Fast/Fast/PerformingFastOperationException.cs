using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Fast
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class PerformingFastOperationException : ComponentFailedPermanentException
	{
		public PerformingFastOperationException() : base(Strings.PerformingFastOperationException)
		{
		}

		public PerformingFastOperationException(Exception innerException) : base(Strings.PerformingFastOperationException, innerException)
		{
		}

		protected PerformingFastOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
