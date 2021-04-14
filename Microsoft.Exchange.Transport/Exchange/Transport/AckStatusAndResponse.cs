using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Transport
{
	internal struct AckStatusAndResponse
	{
		public AckStatusAndResponse(AckStatus ackStatus, SmtpResponse smtpResponse)
		{
			this.AckStatus = ackStatus;
			this.SmtpResponse = smtpResponse;
		}

		public AckStatus AckStatus;

		public SmtpResponse SmtpResponse;
	}
}
