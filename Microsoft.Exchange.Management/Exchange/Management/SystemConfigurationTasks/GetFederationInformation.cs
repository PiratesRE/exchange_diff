using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.SoapWebClient;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "FederationInformation")]
	public sealed class GetFederationInformation : Task
	{
		[Parameter(Mandatory = true)]
		public SmtpDomain DomainName { get; set; }

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> TrustedHostnames { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter Force { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter BypassAdditionalDomainValidation { get; set; }

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 66, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Federation\\GetFederationInformation.cs");
			string[] autodiscoverTrustedHosters = topologyConfigurationSession.GetAutodiscoverTrustedHosters();
			using (AutodiscoverClient autodiscoverClient = new AutodiscoverClient())
			{
				Server localServer = LocalServerCache.LocalServer;
				if (localServer != null && localServer.InternetWebProxy != null)
				{
					base.WriteVerbose(Strings.GetFederationInformationProxy(localServer.InternetWebProxy.ToString()));
					autodiscoverClient.Proxy = new WebProxy(localServer.InternetWebProxy);
				}
				if (this.TrustedHostnames != null)
				{
					autodiscoverClient.AllowedHostnames.AddRange(this.TrustedHostnames);
				}
				if (autodiscoverTrustedHosters != null)
				{
					autodiscoverClient.AllowedHostnames.AddRange(autodiscoverTrustedHosters);
				}
				base.WriteVerbose(Strings.GetFederationInformationTrustedHostnames(autodiscoverClient.AllowedHostnames.ToString()));
				base.WriteProgress(Strings.GetFederationInformationProgress, Strings.DiscoveringDomain(this.DomainName.Domain), 5);
				List<GetFederationInformationResult> list = new List<GetFederationInformationResult>(GetFederationInformationClient.Discover(autodiscoverClient, this.DomainName.Domain));
				base.WriteProgress(Strings.GetFederationInformationProgress, Strings.DiscoveringDomain(this.DomainName.Domain), 25);
				base.WriteVerbose(Strings.GetFederationInformationResults(GetFederationInformation.GetFormattedResults(list)));
				GetFederationInformationResult result = this.GetResult(list);
				if (result == null)
				{
					base.WriteError(new GetFederationInformationFailedException(list.ToArray()), (ErrorCategory)1001, null);
				}
				else
				{
					ICollection domainNames;
					if (this.BypassAdditionalDomainValidation)
					{
						domainNames = result.Domains;
					}
					else
					{
						domainNames = this.GetValidatedAdditionalDomains(autodiscoverClient, result.ApplicationUri, result.Domains);
					}
					if (result.TokenIssuerUris != null && result.TokenIssuerUris.Length > 0)
					{
						bool flag = false;
						Uri[] array = Array.ConvertAll<string, Uri>(result.TokenIssuerUris, (string uri) => new Uri(uri, UriKind.RelativeOrAbsolute));
						ExternalAuthentication current = ExternalAuthentication.GetCurrent();
						if (current.Enabled)
						{
							foreach (SecurityTokenService securityTokenService in current.SecurityTokenServices)
							{
								Uri tokenIssuerUri = securityTokenService.TokenIssuerUri;
								foreach (Uri uri2 in array)
								{
									if (tokenIssuerUri.Equals(uri2))
									{
										base.WriteVerbose(Strings.GetFederationInformationTokenIssuerMatches(tokenIssuerUri.ToString(), uri2.ToString()));
										flag = true;
										break;
									}
									base.WriteVerbose(Strings.GetFederationInformationTokenIssuerDoesntMatch(tokenIssuerUri.ToString(), uri2.ToString()));
								}
							}
							if (!flag)
							{
								StringBuilder stringBuilder = new StringBuilder(100);
								foreach (SecurityTokenService securityTokenService2 in current.SecurityTokenServices)
								{
									if (stringBuilder.Length > 0)
									{
										stringBuilder.Append(",");
									}
									stringBuilder.Append(securityTokenService2.TokenIssuerUri.ToString());
								}
								this.WriteWarning(Strings.GetFederationInformationTokenIssuerDoesntMatchAny(this.DomainName.ToString(), string.Join(",", result.TokenIssuerUris), stringBuilder.ToString()));
							}
						}
					}
					base.WriteObject(new FederationInformation(this.DomainName, new Uri(result.ApplicationUri, UriKind.RelativeOrAbsolute), result.TokenIssuerUris, domainNames, EwsWsSecurityUrl.Fix(result.Url)));
				}
				base.WriteProgress(Strings.GetFederationInformationProgress, Strings.ProgressStatusFinished, 100);
			}
			TaskLogger.LogExit();
		}

		private GetFederationInformationResult GetResult(List<GetFederationInformationResult> results)
		{
			GetFederationInformationResult successfulResult = this.GetSuccessfulResult(results, this.DomainName.Domain);
			if (successfulResult != null)
			{
				return successfulResult;
			}
			if (!this.Force)
			{
				foreach (GetFederationInformationResult getFederationInformationResult in results)
				{
					if (getFederationInformationResult.Type == AutodiscoverResult.UnsecuredRedirect && this.IsValidResult(getFederationInformationResult.Alternate, this.DomainName.Domain))
					{
						LocalizedString federationInformationShouldUseUnsecureRedirect = Strings.GetFederationInformationShouldUseUnsecureRedirect(getFederationInformationResult.Url.ToString(), getFederationInformationResult.RedirectUrl.ToString(), getFederationInformationResult.RedirectUrl.Host);
						if (base.ShouldContinue(federationInformationShouldUseUnsecureRedirect))
						{
							return getFederationInformationResult.Alternate;
						}
					}
				}
				foreach (GetFederationInformationResult getFederationInformationResult2 in results)
				{
					if (getFederationInformationResult2.Type == AutodiscoverResult.InvalidSslHostname && this.IsValidResult(getFederationInformationResult2.Alternate, this.DomainName.Domain))
					{
						LocalizedString federationInformationShouldUseInvalidCertificate = Strings.GetFederationInformationShouldUseInvalidCertificate(getFederationInformationResult2.Url.ToString(), getFederationInformationResult2.Url.Host, getFederationInformationResult2.SslCertificateHostnames.ToString());
						if (base.ShouldContinue(federationInformationShouldUseInvalidCertificate))
						{
							return getFederationInformationResult2.Alternate;
						}
					}
				}
			}
			return null;
		}

		private ICollection GetValidatedAdditionalDomains(AutodiscoverClient client, string applicationUri, StringList domains)
		{
			if (domains.Count <= 1)
			{
				return domains;
			}
			int num = 25;
			int num2 = 75 / (domains.Count - 1);
			List<string> list = new List<string>(domains.Count);
			foreach (string text in domains)
			{
				if (StringComparer.OrdinalIgnoreCase.Equals(this.DomainName.Domain, text))
				{
					list.Add(text);
				}
				else
				{
					base.WriteProgress(Strings.GetFederationInformationProgress, Strings.DiscoveringAdditionalDomain(text), num);
					if (this.IsValidAdditionalDomain(client, applicationUri, text))
					{
						list.Add(text);
					}
					num += num2;
					base.WriteProgress(Strings.GetFederationInformationProgress, Strings.DiscoveringAdditionalDomain(text), num);
				}
			}
			return list;
		}

		private bool IsValidAdditionalDomain(AutodiscoverClient client, string applicationUri, string domain)
		{
			IEnumerable<GetFederationInformationResult> results = GetFederationInformationClient.Discover(client, domain);
			GetFederationInformationResult successfulResult = this.GetSuccessfulResult(results, domain);
			if (successfulResult == null || successfulResult.Type != AutodiscoverResult.Success)
			{
				base.WriteVerbose(Strings.DomainIgnoredBecauseUnableToDiscover(domain, GetFederationInformation.GetFormattedResults(results)));
				return false;
			}
			if (!StringComparer.OrdinalIgnoreCase.Equals(applicationUri, successfulResult.ApplicationUri))
			{
				base.WriteVerbose(Strings.DomainIgnoredBecauseApplicationUriInconsitent(domain, successfulResult.ApplicationUri, applicationUri));
				return false;
			}
			return true;
		}

		private GetFederationInformationResult GetSuccessfulResult(IEnumerable<GetFederationInformationResult> results, string expectedDomain)
		{
			foreach (GetFederationInformationResult result in results)
			{
				if (this.IsValidResult(result, expectedDomain))
				{
					return result;
				}
			}
			return null;
		}

		private bool IsValidResult(GetFederationInformationResult result, string expectedDomain)
		{
			if (result == null)
			{
				return false;
			}
			if (result.Type != AutodiscoverResult.Success)
			{
				return false;
			}
			bool flag = false;
			foreach (string y in result.Domains)
			{
				if (StringComparer.OrdinalIgnoreCase.Equals(expectedDomain, y))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				TaskLogger.Trace("Response from is not valid because domain {0} is not present in Domains element: {1}", new object[]
				{
					expectedDomain,
					result.Domains
				});
				return false;
			}
			return true;
		}

		private static string GetFormattedResults(IEnumerable<GetFederationInformationResult> results)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.AppendLine();
			foreach (GetFederationInformationResult getFederationInformationResult in results)
			{
				stringBuilder.AppendLine(getFederationInformationResult.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
