using System;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DecryptionResultData
	{
		public DecryptionResultData(EmailMessage decryptedMessage, string useLicense, Uri licenseUri)
		{
			this.decryptedMessage = decryptedMessage;
			this.useLicense = useLicense;
			this.licenseUri = licenseUri;
		}

		internal EmailMessage DecryptedMessage
		{
			get
			{
				return this.decryptedMessage;
			}
		}

		internal string UseLicense
		{
			get
			{
				return this.useLicense;
			}
		}

		internal Uri LicenseUri
		{
			get
			{
				return this.licenseUri;
			}
		}

		private readonly EmailMessage decryptedMessage;

		private readonly string useLicense;

		private readonly Uri licenseUri;
	}
}
