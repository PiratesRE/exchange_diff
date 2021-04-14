using System;
using System.Collections.Generic;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Shared.Smtp
{
	internal class SmtpMailItemResult
	{
		public AckStatusAndResponse ConnectionResponse
		{
			get
			{
				return this.connectionResponse;
			}
			set
			{
				this.connectionResponse = value;
			}
		}

		public AckStatusAndResponse MessageResponse
		{
			get
			{
				return this.messageResponse;
			}
			set
			{
				this.messageResponse = value;
			}
		}

		public string RemoteHostName
		{
			get
			{
				return this.remoteHostName;
			}
			set
			{
				this.remoteHostName = value;
			}
		}

		public Dictionary<MailRecipient, AckStatusAndResponse> RecipientResponses
		{
			get
			{
				return this.recipientResponses;
			}
			set
			{
				this.recipientResponses = value;
			}
		}

		private AckStatusAndResponse connectionResponse;

		private AckStatusAndResponse messageResponse;

		private string remoteHostName;

		private Dictionary<MailRecipient, AckStatusAndResponse> recipientResponses;
	}
}
