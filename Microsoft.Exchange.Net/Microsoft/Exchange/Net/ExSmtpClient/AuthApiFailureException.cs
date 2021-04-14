using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class AuthApiFailureException : LocalizedException
	{
		public AuthApiFailureException(string error) : base(NetException.AuthApiFailureException(error))
		{
			this.error = error;
		}

		public AuthApiFailureException(string error, Exception innerException) : base(NetException.AuthApiFailureException(error), innerException)
		{
			this.error = error;
		}

		protected AuthApiFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
