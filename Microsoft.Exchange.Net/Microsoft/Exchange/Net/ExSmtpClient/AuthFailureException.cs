using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class AuthFailureException : LocalizedException
	{
		public AuthFailureException() : base(NetException.AuthFailureException)
		{
		}

		public AuthFailureException(Exception innerException) : base(NetException.AuthFailureException, innerException)
		{
		}

		protected AuthFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
