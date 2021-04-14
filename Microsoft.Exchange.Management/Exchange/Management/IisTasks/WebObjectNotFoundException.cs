using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.IisTasks
{
	[Serializable]
	public class WebObjectNotFoundException : LocalizedException
	{
		public WebObjectNotFoundException(LocalizedString message) : base(message)
		{
		}

		public WebObjectNotFoundException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected WebObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
