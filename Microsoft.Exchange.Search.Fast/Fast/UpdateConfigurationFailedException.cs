using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Fast
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UpdateConfigurationFailedException : ComponentFailedPermanentException
	{
		public UpdateConfigurationFailedException() : base(Strings.UpdateConfigurationFailed)
		{
		}

		public UpdateConfigurationFailedException(Exception innerException) : base(Strings.UpdateConfigurationFailed, innerException)
		{
		}

		protected UpdateConfigurationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
