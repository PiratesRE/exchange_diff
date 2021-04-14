using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal class JournalingConfigurationLoadException : LocalizedException
	{
		internal JournalingConfigurationLoadException(string errorString) : base(new LocalizedString(errorString))
		{
		}

		internal JournalingConfigurationLoadException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}
	}
}
