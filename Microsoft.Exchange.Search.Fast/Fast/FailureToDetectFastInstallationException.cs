using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Fast
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FailureToDetectFastInstallationException : ComponentFailedPermanentException
	{
		public FailureToDetectFastInstallationException() : base(Strings.FailureToDetectFastInstallation)
		{
		}

		public FailureToDetectFastInstallationException(Exception innerException) : base(Strings.FailureToDetectFastInstallation, innerException)
		{
		}

		protected FailureToDetectFastInstallationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
