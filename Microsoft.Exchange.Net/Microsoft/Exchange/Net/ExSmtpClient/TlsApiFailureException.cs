using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class TlsApiFailureException : LocalizedException
	{
		public TlsApiFailureException(string error) : base(NetException.TlsApiFailureException(error))
		{
			this.error = error;
		}

		public TlsApiFailureException(string error, Exception innerException) : base(NetException.TlsApiFailureException(error), innerException)
		{
			this.error = error;
		}

		protected TlsApiFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
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
