using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FailedToDiscoverRpcEndpointTransientException : FailedToDiscoverNspiServerTransientException
	{
		public FailedToDiscoverRpcEndpointTransientException() : base(Strings.MigrationExchangeRpcConnectionFailure)
		{
		}

		public FailedToDiscoverRpcEndpointTransientException(Exception innerException) : base(Strings.MigrationExchangeRpcConnectionFailure, innerException)
		{
		}

		protected FailedToDiscoverRpcEndpointTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
