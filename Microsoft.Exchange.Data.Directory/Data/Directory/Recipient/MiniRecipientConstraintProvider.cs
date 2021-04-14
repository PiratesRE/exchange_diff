using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MiniRecipientConstraintProvider : UserConstraintProvider
	{
		public MiniRecipientConstraintProvider(MiniRecipient recipient, string rampId, bool isFirstRelease) : this(recipient, rampId, isFirstRelease, UserConstraintProvider.SupportedLocales)
		{
		}

		internal MiniRecipientConstraintProvider(MiniRecipient recipient, string rampId, bool isFirstRelease, List<CultureInfo> supportedLocales) : base(MiniRecipientConstraintProvider.GetUserName(recipient), MiniRecipientConstraintProvider.GetOrganization(recipient), MiniRecipientConstraintProvider.GetLocale(recipient, supportedLocales), rampId, isFirstRelease, recipient.ReleaseTrack == ReleaseTrack.Preview, MiniRecipientConstraintProvider.GetUserType(recipient))
		{
		}

		private static VariantConfigurationUserType GetUserType(MiniRecipient recipient)
		{
			if (!(recipient.OrganizationId != null))
			{
				return VariantConfigurationUserType.None;
			}
			if (!recipient.IsConsumerOrganization())
			{
				return VariantConfigurationUserType.Business;
			}
			return VariantConfigurationUserType.Consumer;
		}

		private static string GetLocale(MiniRecipient recipient, List<CultureInfo> supportedLocales)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			foreach (CultureInfo cultureInfo in recipient.Languages)
			{
				if (supportedLocales.Contains(cultureInfo))
				{
					return cultureInfo.Name;
				}
			}
			return "en-US";
		}

		private static string GetOrganization(MiniRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (recipient.OrganizationId != null && recipient.OrganizationId.OrganizationalUnit != null)
			{
				return recipient.OrganizationId.OrganizationalUnit.Name;
			}
			return string.Empty;
		}

		private static string GetUserName(MiniRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (recipient.UserPrincipalName != null)
			{
				return recipient.UserPrincipalName.Split(new char[]
				{
					'@'
				})[0];
			}
			return string.Empty;
		}
	}
}
