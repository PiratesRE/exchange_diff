using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class ADRecipientOrAddressExtensions
	{
		public static ADRecipientOrAddress ToADRecipientOrAddress(this SmtpAddress? smtpAddress)
		{
			if (smtpAddress == null)
			{
				return null;
			}
			Participant participant = new Participant(smtpAddress.Value.ToString(), smtpAddress.Value.ToString(), "SMTP");
			return new ADRecipientOrAddress(participant);
		}

		public static ADRecipientOrAddress ToADRecipientOrAddress(this ADIdParameter identity)
		{
			Participant participant = new Participant(identity.ToString(), identity.ToString(), "SMTP");
			return new ADRecipientOrAddress(participant);
		}

		public static IEnumerable<ADRecipientOrAddress> ToADRecipientOrAddresses(this IEnumerable<ADIdParameter> identities)
		{
			if (identities != null && identities.Count<ADIdParameter>() > 0)
			{
				return from identity in identities
				select identity.ToADRecipientOrAddress();
			}
			return null;
		}
	}
}
