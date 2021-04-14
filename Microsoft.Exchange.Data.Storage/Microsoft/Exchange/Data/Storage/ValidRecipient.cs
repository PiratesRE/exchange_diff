using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ValidRecipient
	{
		internal ValidRecipient(string smtpAddress, ADRecipient adRecipient)
		{
			Util.ThrowOnNullOrEmptyArgument(smtpAddress, "smtpAddress");
			if (!Microsoft.Exchange.Data.SmtpAddress.IsValidSmtpAddress(smtpAddress))
			{
				throw new ArgumentOutOfRangeException(ServerStrings.InvalidSmtpAddress(smtpAddress));
			}
			this.SmtpAddress = smtpAddress;
			this.ADRecipient = adRecipient;
			SmtpProxyAddress smtpProxyAddress = (adRecipient != null) ? (adRecipient.ExternalEmailAddress as SmtpProxyAddress) : null;
			if (smtpProxyAddress != null)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<ADObjectId, SmtpProxyAddress>((long)this.GetHashCode(), "{0}: using ExternalEmailAddress as SmtpAddress: {1}", adRecipient.Id, smtpProxyAddress);
				this.SmtpAddressForEncryption = smtpProxyAddress.SmtpAddress;
				return;
			}
			this.SmtpAddressForEncryption = smtpAddress;
		}

		internal string SmtpAddress { get; private set; }

		internal string SmtpAddressForEncryption { get; private set; }

		internal ADRecipient ADRecipient { get; private set; }

		public override string ToString()
		{
			return this.SmtpAddress;
		}

		internal static string[] ConvertToStringArray(ValidRecipient[] recipients)
		{
			return Array.ConvertAll<ValidRecipient, string>(recipients, (ValidRecipient recipient) => recipient.ToString());
		}

		internal static ValidRecipient[] ConvertFromStringArray(string[] recipients)
		{
			return Array.ConvertAll<string, ValidRecipient>(recipients, (string recipient) => new ValidRecipient(recipient, null));
		}

		internal static readonly ValidRecipient[] EmptyRecipients = Array<ValidRecipient>.Empty;
	}
}
