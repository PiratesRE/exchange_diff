using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class EmailMessage
	{
		public EmailMessage(bool isRead, string subject, string from)
		{
			this.IsRead = isRead;
			this.Subject = subject;
			this.From = from;
		}

		[DataMember]
		public bool IsRead { get; set; }

		[DataMember]
		public string Subject { get; set; }

		[DataMember]
		public string From { get; set; }
	}
}
