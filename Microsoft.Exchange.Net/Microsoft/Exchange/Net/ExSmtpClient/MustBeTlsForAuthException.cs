using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MustBeTlsForAuthException : LocalizedException
	{
		public MustBeTlsForAuthException() : base(NetException.MustBeTlsForAuthException)
		{
		}

		public MustBeTlsForAuthException(Exception innerException) : base(NetException.MustBeTlsForAuthException, innerException)
		{
		}

		protected MustBeTlsForAuthException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
