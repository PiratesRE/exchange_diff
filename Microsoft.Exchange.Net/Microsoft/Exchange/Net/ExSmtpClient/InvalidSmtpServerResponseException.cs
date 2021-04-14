using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidSmtpServerResponseException : LocalizedException
	{
		public InvalidSmtpServerResponseException(string response) : base(NetException.InvalidSmtpServerResponseException(response))
		{
			this.response = response;
		}

		public InvalidSmtpServerResponseException(string response, Exception innerException) : base(NetException.InvalidSmtpServerResponseException(response), innerException)
		{
			this.response = response;
		}

		protected InvalidSmtpServerResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.response = (string)info.GetValue("response", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("response", this.response);
		}

		public string Response
		{
			get
			{
				return this.response;
			}
		}

		private readonly string response;
	}
}
