using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MessageSecurity
{
	[Serializable]
	internal sealed class MessageSecurityException : ApplicationException
	{
		public MessageSecurityException()
		{
		}

		public MessageSecurityException(string message) : base(message)
		{
		}

		public MessageSecurityException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public MessageSecurityException(string message, int hr) : base(message)
		{
			base.HResult = hr;
		}

		internal MessageSecurityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
