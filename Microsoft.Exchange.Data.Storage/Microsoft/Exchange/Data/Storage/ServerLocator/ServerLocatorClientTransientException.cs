using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage.ServerLocator
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServerLocatorClientTransientException : TransientException
	{
		public ServerLocatorClientTransientException(LocalizedString localizedString) : base(localizedString)
		{
		}

		public ServerLocatorClientTransientException(LocalizedString localizedString, Exception inner) : base(localizedString, inner)
		{
		}

		protected ServerLocatorClientTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
