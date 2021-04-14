using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Data.Transport.Routing
{
	public struct RecipientExpansionInfo
	{
		public RecipientExpansionInfo(EnvelopeRecipient removeRecipient, RoutingAddress[] addresses)
		{
			this = new RecipientExpansionInfo(removeRecipient, addresses, SmtpResponse.Empty);
			this.generateDSN = false;
		}

		public RecipientExpansionInfo(EnvelopeRecipient removeRecipient, RoutingAddress[] addresses, SmtpResponse smtpResponse)
		{
			if (removeRecipient == null)
			{
				throw new ArgumentNullException("removeRecipient");
			}
			if (addresses == null)
			{
				throw new ArgumentNullException("addresses");
			}
			for (int i = 0; i < addresses.Length; i++)
			{
				if (!addresses[i].IsValid)
				{
					throw new ArgumentException(string.Format("The specified address is an invalid SMTP address - {0}", addresses[i]));
				}
			}
			this.removeRecipient = removeRecipient;
			this.addresses = addresses;
			this.response = smtpResponse;
			this.generateDSN = true;
		}

		public EnvelopeRecipient RemoveRecipient
		{
			get
			{
				return this.removeRecipient;
			}
		}

		public RoutingAddress[] Addresses
		{
			get
			{
				return this.addresses;
			}
		}

		public SmtpResponse SmtpResponse
		{
			get
			{
				return this.response;
			}
		}

		internal bool GenerateDSN
		{
			get
			{
				return this.generateDSN;
			}
		}

		private EnvelopeRecipient removeRecipient;

		private RoutingAddress[] addresses;

		private SmtpResponse response;

		private bool generateDSN;
	}
}
