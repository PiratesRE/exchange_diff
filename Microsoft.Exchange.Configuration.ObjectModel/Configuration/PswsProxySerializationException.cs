using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PswsProxySerializationException : PswsProxyException
	{
		public PswsProxySerializationException(LocalizedString message) : base(message)
		{
		}

		public PswsProxySerializationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected PswsProxySerializationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
