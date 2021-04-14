using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage.ServerLocator
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServerLocatorClientException : LocalizedException
	{
		public ServerLocatorClientException(LocalizedString message) : base(message)
		{
		}

		public ServerLocatorClientException(LocalizedString message, Exception inner) : base(message, inner)
		{
		}

		protected ServerLocatorClientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
