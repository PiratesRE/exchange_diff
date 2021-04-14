using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ExchangePrincipalConstraintProvider : UserConstraintProvider
	{
		public ExchangePrincipalConstraintProvider(IExchangePrincipal exchangePrincipal, string rampId, bool isFirstRelease) : this(exchangePrincipal, rampId, isFirstRelease, UserConstraintProvider.SupportedLocales)
		{
		}

		internal ExchangePrincipalConstraintProvider(IExchangePrincipal exchangePrincipal, string rampId, bool isFirstRelease, List<CultureInfo> supportedLocales) : base(exchangePrincipal.PrincipalId, ExchangePrincipalConstraintProvider.GetOrganization(exchangePrincipal), ExchangePrincipalConstraintProvider.GetLocale(exchangePrincipal, supportedLocales), rampId, isFirstRelease, false, ExchangePrincipalConstraintProvider.GetUserType(exchangePrincipal))
		{
		}

		private static VariantConfigurationUserType GetUserType(IExchangePrincipal exchangePrincipal)
		{
			if (exchangePrincipal.MailboxInfo == null || exchangePrincipal.MailboxInfo.OrganizationId == null)
			{
				return VariantConfigurationUserType.None;
			}
			if (!Globals.IsConsumerOrganization(exchangePrincipal.MailboxInfo.OrganizationId))
			{
				return VariantConfigurationUserType.Business;
			}
			return VariantConfigurationUserType.Consumer;
		}

		private static string GetOrganization(IExchangePrincipal exchangePrincipal)
		{
			if (exchangePrincipal == null)
			{
				throw new ArgumentNullException("exchangePrincipal");
			}
			if (exchangePrincipal.MailboxInfo != null && exchangePrincipal.MailboxInfo.OrganizationId != null && exchangePrincipal.MailboxInfo.OrganizationId.OrganizationalUnit != null)
			{
				return exchangePrincipal.MailboxInfo.OrganizationId.OrganizationalUnit.Name;
			}
			return string.Empty;
		}

		private static string GetLocale(IExchangePrincipal exchangePrincipal, List<CultureInfo> supportedLocales)
		{
			if (exchangePrincipal == null)
			{
				throw new ArgumentNullException("exchangePrincipal");
			}
			foreach (CultureInfo cultureInfo in exchangePrincipal.PreferredCultures)
			{
				if (supportedLocales.Contains(cultureInfo))
				{
					return cultureInfo.Name;
				}
			}
			return "en-US";
		}
	}
}
