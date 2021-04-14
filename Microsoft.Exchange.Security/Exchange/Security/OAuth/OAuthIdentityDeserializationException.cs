using System;
using System.Runtime.Serialization;
using System.Security;

namespace Microsoft.Exchange.Security.OAuth
{
	[Serializable]
	internal class OAuthIdentityDeserializationException : SecurityException
	{
		public OAuthIdentityDeserializationException(string message) : base(message)
		{
		}

		public OAuthIdentityDeserializationException(string message, Exception inner) : base(message, inner)
		{
		}

		protected OAuthIdentityDeserializationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
