using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.IisTasks
{
	[Serializable]
	public class WebObjectAlreadyExistsException : LocalizedException
	{
		public WebObjectAlreadyExistsException(LocalizedString message) : base(message)
		{
		}

		public WebObjectAlreadyExistsException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected WebObjectAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
