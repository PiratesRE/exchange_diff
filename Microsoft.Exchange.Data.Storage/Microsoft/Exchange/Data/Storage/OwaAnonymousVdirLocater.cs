using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OwaAnonymousVdirLocater
	{
		private OwaAnonymousVdirLocater()
		{
			this.InitializeAnonymousCalendarHostUrl();
		}

		public static OwaAnonymousVdirLocater Instance
		{
			get
			{
				if (OwaAnonymousVdirLocater.instance == null)
				{
					lock (OwaAnonymousVdirLocater.syncLock)
					{
						if (OwaAnonymousVdirLocater.instance == null)
						{
							OwaAnonymousVdirLocater.instance = new OwaAnonymousVdirLocater();
						}
					}
				}
				return OwaAnonymousVdirLocater.instance;
			}
		}

		private bool IsMultitenancyEnabled
		{
			get
			{
				return VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
			}
		}

		public Uri GetOwaVdirUrl(IExchangePrincipal exchangePrincipal, IFrontEndLocator frontEndLocator)
		{
			Uri owaServiceUrl = this.GetOwaServiceUrl(exchangePrincipal, frontEndLocator);
			UriBuilder uriBuilder = new UriBuilder(owaServiceUrl);
			uriBuilder.Port = -1;
			uriBuilder.Scheme = Uri.UriSchemeHttp;
			if (this.anonymousCalendarHostUrl != null)
			{
				uriBuilder.Host = this.anonymousCalendarHostUrl.Host;
			}
			return uriBuilder.Uri;
		}

		public bool IsPublishingAvailable(IExchangePrincipal exchangePrincipal, IFrontEndLocator frontEndLocator)
		{
			bool result;
			try
			{
				this.GetOwaVdirUrl(exchangePrincipal, frontEndLocator);
				result = true;
			}
			catch (NoExternalOwaAvailableException)
			{
				result = false;
			}
			return result;
		}

		private void InitializeAnonymousCalendarHostUrl()
		{
			try
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 141, "InitializeAnonymousCalendarHostUrl", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\ServiceDiscovery\\OwaAnonymousVdirLocater.cs");
				ServiceEndpointContainer endpointContainer = topologyConfigurationSession.GetEndpointContainer();
				ServiceEndpoint endpoint = endpointContainer.GetEndpoint(ServiceEndpointId.AnonymousCalendarHostUrl);
				this.anonymousCalendarHostUrl = endpoint.Uri;
			}
			catch (EndpointContainerNotFoundException)
			{
			}
			catch (ServiceEndpointNotFoundException)
			{
			}
		}

		private Uri GetOwaServiceUrl(IExchangePrincipal exchangePrincipal, IFrontEndLocator frontEndLocator)
		{
			if (exchangePrincipal.MailboxInfo.Location.ServerVersion >= Server.E15MinVersion && this.IsMultitenancyEnabled)
			{
				return this.GetE15MultitenancyOwaServiceUrl(exchangePrincipal, frontEndLocator);
			}
			return this.GetEnterpriseOrE14OwaServiceUrl(exchangePrincipal);
		}

		private Uri GetE15MultitenancyOwaServiceUrl(IExchangePrincipal exchangePrincipal, IFrontEndLocator frontEndLocator)
		{
			Uri result = null;
			Exception ex = null;
			try
			{
				result = frontEndLocator.GetOwaUrl(exchangePrincipal);
			}
			catch (ServerNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (ADTransientException ex3)
			{
				ex = ex3;
			}
			catch (DataSourceOperationException ex4)
			{
				ex = ex4;
			}
			catch (DataValidationException ex5)
			{
				ex = ex5;
			}
			finally
			{
				if (ex != null)
				{
					throw new NoExternalOwaAvailableException(ex);
				}
			}
			return result;
		}

		private Uri GetEnterpriseOrE14OwaServiceUrl(IExchangePrincipal exchangePrincipal)
		{
			ServiceTopology serviceTopology = this.IsMultitenancyEnabled ? ServiceTopology.GetCurrentLegacyServiceTopology("f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\ServiceDiscovery\\OwaAnonymousVdirLocater.cs", "GetEnterpriseOrE14OwaServiceUrl", 230) : ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\ServiceDiscovery\\OwaAnonymousVdirLocater.cs", "GetEnterpriseOrE14OwaServiceUrl", 230);
			Predicate<OwaService> serviceFilter = (OwaService service) => service.AnonymousFeaturesEnabled;
			IList<OwaService> list = serviceTopology.FindAll<OwaService>(exchangePrincipal, ClientAccessType.External, serviceFilter, "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\ServiceDiscovery\\OwaAnonymousVdirLocater.cs", "GetEnterpriseOrE14OwaServiceUrl", 235);
			OwaService owaService;
			if (list.Count > 0)
			{
				owaService = list[0];
			}
			else
			{
				owaService = serviceTopology.FindAny<OwaService>(ClientAccessType.External, serviceFilter, "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\ServiceDiscovery\\OwaAnonymousVdirLocater.cs", "GetEnterpriseOrE14OwaServiceUrl", 247);
				if (owaService == null)
				{
					throw new NoExternalOwaAvailableException();
				}
			}
			return owaService.Url;
		}

		private const string OwaAnonymousVdirName = "calendar";

		private static OwaAnonymousVdirLocater instance;

		private static readonly object syncLock = new object();

		private Uri anonymousCalendarHostUrl;
	}
}
