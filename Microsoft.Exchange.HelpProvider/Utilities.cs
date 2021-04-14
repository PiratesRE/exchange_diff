using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.CommonHelpProvider
{
	public static class Utilities
	{
		internal static bool IsMicrosoftHostedOnly()
		{
			bool result = false;
			try
			{
				result = Datacenter.IsMicrosoftHostedOnly(true);
			}
			catch (CannotDetermineExchangeModeException ex)
			{
				ExTraceGlobals.CoBrandingTracer.TraceError<string>(0L, "HelpProvider::IsMicrosoftHostedOnly CannotDetermineExchangeModeException {0}.", ex.Message);
			}
			return result;
		}

		internal static string GetServicePlanName(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			if (rbacConfiguration == null || rbacConfiguration.OrganizationId == null)
			{
				return string.Empty;
			}
			OrganizationProperties organizationProperties;
			if (!OrganizationPropertyCache.TryGetOrganizationProperties(rbacConfiguration.OrganizationId, out organizationProperties))
			{
				return string.Empty;
			}
			return organizationProperties.ServicePlan;
		}

		internal static Uri NormalizeUrl(Uri inputUri)
		{
			if (inputUri.ToString().Contains("{0}"))
			{
				return inputUri;
			}
			Regex regex = new Regex("\\(.*?\\)");
			Match match = regex.Match(inputUri.ToString());
			string value = match.Value;
			if (match.Success)
			{
				inputUri = new Uri(regex.Replace(inputUri.ToString(), string.Empty));
			}
			string leftPart = inputUri.GetLeftPart(UriPartial.Authority);
			string text = inputUri.PathAndQuery;
			if (!text.EndsWith("/"))
			{
				text += "/";
			}
			string uriString = string.Concat(new string[]
			{
				leftPart,
				"/{0}",
				text,
				"{1}",
				value,
				".aspx"
			});
			return new Uri(uriString);
		}

		private const string ContentIDSuffix = ".aspx";
	}
}
