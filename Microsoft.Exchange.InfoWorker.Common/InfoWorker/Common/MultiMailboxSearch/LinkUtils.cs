using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.InfoWorker.EventLog;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal static class LinkUtils
	{
		public static Uri UpdateOwaLinkToItem(Uri baseLink, string itemId)
		{
			return LinkUtils.AppendQueryString(baseLink, new Dictionary<string, string>
			{
				{
					"ItemID",
					itemId
				}
			});
		}

		public static Uri UpdateOwaLinkToSearchId(Uri baseLink, string searchId)
		{
			Dictionary<string, string> queryParameters = new Dictionary<string, string>
			{
				{
					"cmd",
					"contents"
				},
				{
					"module",
					"discovery"
				},
				{
					"discoveryid",
					searchId
				},
				{
					"exsvurl",
					"1"
				}
			};
			Uri baseUri = LinkUtils.AppendRelativePath(baseLink, "default.aspx", false);
			return LinkUtils.AppendQueryString(baseUri, queryParameters);
		}

		public static Uri UpdateOwaLinkWithMailbox(Uri baseLink, SmtpAddress smtpAddress)
		{
			return LinkUtils.AppendRelativePath(baseLink, string.Format("{0}", smtpAddress), true);
		}

		public static Uri GetOwaMailboxItemLink(Action errorHandler, MailboxInfo mailboxInfo, bool supportsIntegratedAuth)
		{
			Uri uri = LinkUtils.GetOwaBaseLink(errorHandler, mailboxInfo, supportsIntegratedAuth);
			if (uri != null)
			{
				uri = LinkUtils.AppendQueryString(uri, LinkUtils.itemLinkParameters);
			}
			return uri;
		}

		public static Uri GetOwaBaseLink(Action errorHandler, MailboxInfo mailboxInfo, bool supportsIntegratedAuth)
		{
			Util.ThrowOnNull(mailboxInfo, "mailboxInfo");
			Util.ThrowOnNull(mailboxInfo.ExchangePrincipal, "mailboxInfo.ExchangePrincipal");
			return LinkUtils.GetOwaBaseLink(errorHandler, mailboxInfo.ExchangePrincipal, supportsIntegratedAuth);
		}

		public static Uri GetOwaBaseLink(Action errorHandler, ExchangePrincipal targetPrincipal, bool supportsIntegratedAuth)
		{
			Uri uri = null;
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				uri = FrontEndLocator.GetFrontEndOwaUrl(targetPrincipal);
				if (uri == null)
				{
					Factory.Current.EventLog.LogEvent(InfoWorkerEventLogConstants.Tuple_DiscoveryFailedToGetOWAUrl, null, new object[]
					{
						targetPrincipal.MailboxInfo.Location.ServerFqdn,
						targetPrincipal.MailboxInfo.OrganizationId.OrganizationalUnit.ObjectGuid,
						targetPrincipal.MailboxInfo.MailboxGuid
					});
				}
				else if (!string.IsNullOrEmpty(targetPrincipal.MailboxInfo.PrimarySmtpAddress.ToString()))
				{
					SmtpAddress primarySmtpAddress = targetPrincipal.MailboxInfo.PrimarySmtpAddress;
					if (!string.IsNullOrEmpty(primarySmtpAddress.Domain))
					{
						uri = LinkUtils.AppendRelativePath(uri, primarySmtpAddress.Domain, true);
					}
				}
			}
			else
			{
				OwaService owaService = LinkUtils.GetOwaService(targetPrincipal);
				if (owaService != null)
				{
					uri = owaService.Url;
					if (supportsIntegratedAuth && owaService.IntegratedFeaturesEnabled)
					{
						uri = LinkUtils.AppendRelativePath(uri, "integrated", true);
					}
				}
				else if (errorHandler != null)
				{
					errorHandler();
				}
			}
			return uri;
		}

		public static OwaService GetOwaService(ExchangePrincipal principal)
		{
			OwaService owaService = null;
			try
			{
				ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\MultiMailboxSearch\\LinkUtils.cs", "GetOwaService", 197);
				owaService = (from x in currentServiceTopology.FindAll<OwaService>(principal, ClientAccessType.External, "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\MultiMailboxSearch\\LinkUtils.cs", "GetOwaService", 200)
				where x.IsFrontEnd
				select x).FirstOrDefault<OwaService>();
				if (owaService == null)
				{
					owaService = (from x in currentServiceTopology.FindAll<OwaService>(principal, ClientAccessType.Internal, "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\MultiMailboxSearch\\LinkUtils.cs", "GetOwaService", 207)
					where x.IsFrontEnd
					select x).FirstOrDefault<OwaService>();
				}
				if (owaService == null)
				{
					Factory.Current.EventLog.LogEvent(InfoWorkerEventLogConstants.Tuple_DiscoveryFailedToGetOWAService, null, new object[]
					{
						principal.MailboxInfo.Location.ServerFqdn
					});
				}
			}
			catch (ReadTopologyTimeoutException ex)
			{
				Factory.Current.EventLog.LogEvent(InfoWorkerEventLogConstants.Tuple_DiscoveryFailedToGetOWAServiceWithException, null, new object[]
				{
					principal.MailboxInfo.Location.ServerFqdn,
					ex.ToString()
				});
			}
			return owaService;
		}

		public static Uri AppendRelativePath(Uri baseUri, string relativePath, bool terminateWithSlash)
		{
			UriBuilder uriBuilder = new UriBuilder(baseUri);
			uriBuilder.Path = VirtualPathUtility.Combine(VirtualPathUtility.AppendTrailingSlash(uriBuilder.Path), relativePath);
			if (terminateWithSlash)
			{
				uriBuilder.Path = VirtualPathUtility.AppendTrailingSlash(uriBuilder.Path);
			}
			return uriBuilder.Uri;
		}

		public static Uri AppendQueryString(Uri baseUri, IDictionary<string, string> queryParameters)
		{
			UriBuilder uriBuilder = new UriBuilder(baseUri);
			if (queryParameters != null && queryParameters.Count > 0)
			{
				NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(baseUri.Query);
				foreach (KeyValuePair<string, string> keyValuePair in queryParameters)
				{
					nameValueCollection[keyValuePair.Key] = keyValuePair.Value;
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < nameValueCollection.Count; i++)
				{
					stringBuilder.AppendFormat("{0}{1}={2}", (i > 0) ? "&" : string.Empty, nameValueCollection.AllKeys[i], Uri.EscapeDataString(nameValueCollection[i]));
				}
				uriBuilder.Query = stringBuilder.ToString();
			}
			return uriBuilder.Uri;
		}

		private static readonly Dictionary<string, string> itemLinkParameters = new Dictionary<string, string>
		{
			{
				"viewmodel",
				"ItemReadingPaneViewModelPopOutFactory"
			},
			{
				"IsDiscoveryView",
				"1"
			},
			{
				"exsvurl",
				"1"
			}
		};
	}
}
