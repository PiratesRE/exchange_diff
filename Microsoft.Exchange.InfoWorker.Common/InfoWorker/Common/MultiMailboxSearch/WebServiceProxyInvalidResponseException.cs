using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class WebServiceProxyInvalidResponseException : MultiMailboxSearchException
	{
		public WebServiceProxyInvalidResponseException(LocalizedString message) : base(message)
		{
		}

		public WebServiceProxyInvalidResponseException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected WebServiceProxyInvalidResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
