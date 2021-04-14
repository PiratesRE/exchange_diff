using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GenericADUserConstraintProvider : UserConstraintProvider
	{
		public GenericADUserConstraintProvider(IGenericADUser adUser, string rampId, bool isFirstRelease) : this(adUser, rampId, isFirstRelease, UserConstraintProvider.SupportedLocales)
		{
		}

		internal GenericADUserConstraintProvider(IGenericADUser adUser, string rampId, bool isFirstRelease, List<CultureInfo> supportedLocales) : base(GenericADUserConstraintProvider.GetUserName(adUser), GenericADUserConstraintProvider.GetOrganization(adUser), GenericADUserConstraintProvider.GetLocale(adUser, supportedLocales), rampId, isFirstRelease, false, GenericADUserConstraintProvider.GetUserType(adUser))
		{
		}

		private static string GetUserName(IGenericADUser adUser)
		{
			if (adUser == null)
			{
				throw new ArgumentNullException("adUser");
			}
			if (adUser.UserPrincipalName != null)
			{
				return adUser.UserPrincipalName.Split(new char[]
				{
					'@'
				})[0];
			}
			SmtpAddress primarySmtpAddress = adUser.PrimarySmtpAddress;
			if (adUser.PrimarySmtpAddress.Local != null)
			{
				return adUser.PrimarySmtpAddress.Local;
			}
			return string.Empty;
		}

		private static string GetOrganization(IGenericADUser adUser)
		{
			if (adUser == null)
			{
				throw new ArgumentNullException("adUser");
			}
			if (adUser.OrganizationId != null && adUser.OrganizationId.OrganizationalUnit != null && adUser.OrganizationId.OrganizationalUnit.Name != null)
			{
				return adUser.OrganizationId.OrganizationalUnit.Name;
			}
			return string.Empty;
		}

		private static string GetLocale(IGenericADUser adUser, List<CultureInfo> supportedLocales)
		{
			if (adUser == null)
			{
				throw new ArgumentNullException("adUser");
			}
			foreach (CultureInfo cultureInfo in adUser.Languages)
			{
				if (supportedLocales.Contains(cultureInfo))
				{
					return cultureInfo.Name;
				}
			}
			return "en-US";
		}

		private static VariantConfigurationUserType GetUserType(IGenericADUser adUser)
		{
			if (adUser == null || adUser.OrganizationId == null)
			{
				return VariantConfigurationUserType.None;
			}
			if (!Globals.IsConsumerOrganization(adUser.OrganizationId))
			{
				return VariantConfigurationUserType.Business;
			}
			return VariantConfigurationUserType.Consumer;
		}
	}
}
