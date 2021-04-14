using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.AirSync.Wbxml
{
	[Serializable]
	internal class WbxmlException : LocalizedException
	{
		public WbxmlException(string message) : base(new LocalizedString(message))
		{
		}

		public WbxmlException(string message, Exception innerException) : base(new LocalizedString(message), innerException)
		{
		}

		protected WbxmlException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
