using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class TlsProtocolFailureException : LocalizedException
	{
		public TlsProtocolFailureException() : base(NetException.TlsProtocolFailureException)
		{
		}

		public TlsProtocolFailureException(Exception innerException) : base(NetException.TlsProtocolFailureException, innerException)
		{
		}

		protected TlsProtocolFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
