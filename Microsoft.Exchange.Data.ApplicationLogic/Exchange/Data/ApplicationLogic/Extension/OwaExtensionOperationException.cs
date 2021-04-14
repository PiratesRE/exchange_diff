using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	[Serializable]
	internal class OwaExtensionOperationException : LocalizedException
	{
		public OwaExtensionOperationException(LocalizedString message) : base(message)
		{
		}

		public OwaExtensionOperationException(Exception innerException) : base(new LocalizedString(innerException.Message), innerException)
		{
		}

		public OwaExtensionOperationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected OwaExtensionOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
