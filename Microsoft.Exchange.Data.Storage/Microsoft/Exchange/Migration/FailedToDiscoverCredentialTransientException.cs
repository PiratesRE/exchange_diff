using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FailedToDiscoverCredentialTransientException : FailedToDiscoverNspiServerTransientException
	{
		public FailedToDiscoverCredentialTransientException() : base(Strings.MigrationExchangeCredentialFailure)
		{
		}

		public FailedToDiscoverCredentialTransientException(Exception innerException) : base(Strings.MigrationExchangeCredentialFailure, innerException)
		{
		}

		protected FailedToDiscoverCredentialTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
