using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage
{
	[XmlType(TypeName = "InvalidRecipientType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public sealed class InvalidRecipient
	{
		public InvalidRecipient()
		{
		}

		internal InvalidRecipient(string smtpAddress, InvalidRecipientResponseCodeType responseCode) : this(smtpAddress, responseCode, null)
		{
		}

		internal InvalidRecipient(string smtpAddress, InvalidRecipientResponseCodeType responseCode, string messageText)
		{
			this.SmtpAddress = smtpAddress;
			this.ResponseCode = responseCode;
			this.MessageText = messageText;
		}

		public string SmtpAddress { get; set; }

		public InvalidRecipientResponseCodeType ResponseCode { get; set; }

		public string MessageText { get; set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmtpAddress=",
				this.SmtpAddress,
				", ResponseCode=",
				this.ResponseCode,
				", MessageText=",
				this.MessageText
			});
		}
	}
}
