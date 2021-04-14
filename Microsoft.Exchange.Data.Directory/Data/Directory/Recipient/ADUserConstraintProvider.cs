using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ADUserConstraintProvider : UserConstraintProvider
	{
		public ADUserConstraintProvider(ADUser user, string rampId, bool isFirstRelease) : this(user, rampId, isFirstRelease, UserConstraintProvider.SupportedLocales)
		{
		}

		internal ADUserConstraintProvider(ADUser user, string rampId, bool isFirstRelease, List<CultureInfo> supportedLocales) : base(ADUserConstraintProvider.GetUserName(user), ADUserConstraintProvider.GetOrganization(user), ADUserConstraintProvider.GetLocale(user, supportedLocales), rampId, isFirstRelease, user.ReleaseTrack == ReleaseTrack.Preview, ADUserConstraintProvider.GetUserType(user))
		{
		}

		private static VariantConfigurationUserType GetUserType(ADUser user)
		{
			if (!(user.OrganizationId != null))
			{
				return VariantConfigurationUserType.None;
			}
			if (!user.IsConsumerOrganization())
			{
				return VariantConfigurationUserType.Business;
			}
			return VariantConfigurationUserType.Consumer;
		}

		private static string GetLocale(ADUser user, List<CultureInfo> supportedLocales)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			foreach (CultureInfo cultureInfo in user.Languages)
			{
				if (supportedLocales.Contains(cultureInfo))
				{
					return cultureInfo.Name;
				}
			}
			return "en-US";
		}

		private static string GetOrganization(ADUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			if (user.OrganizationId != null && user.OrganizationId.OrganizationalUnit != null)
			{
				return user.OrganizationId.OrganizationalUnit.Name;
			}
			return string.Empty;
		}

		private static string GetUserName(ADUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			if (user.UserPrincipalName != null)
			{
				return user.UserPrincipalName.Split(new char[]
				{
					'@'
				})[0];
			}
			return string.Empty;
		}
	}
}
