using System;
using System.Configuration;
using System.ServiceModel.Configuration;
using System.Web.Configuration;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal class AutodiscoverWebConfiguration
	{
		internal AutodiscoverWebConfiguration()
		{
			Configuration config = WebConfigurationManager.OpenWebConfiguration("~/web.config");
			ServiceElementCollection services = ServiceModelSectionGroup.GetSectionGroup(config).Services.Services;
			foreach (object obj in services)
			{
				ServiceElement serviceElement = (ServiceElement)obj;
				foreach (object obj2 in serviceElement.Endpoints)
				{
					ServiceEndpointElement serviceEndpointElement = (ServiceEndpointElement)obj2;
					if (!string.IsNullOrEmpty(serviceEndpointElement.BindingConfiguration) && serviceEndpointElement.Contract.Equals(Common.EndpointContract, StringComparison.OrdinalIgnoreCase))
					{
						if (serviceEndpointElement.Address.OriginalString.Equals("wssecurity/symmetrickey", StringComparison.OrdinalIgnoreCase))
						{
							this.wsSecuritySymmetricKeyEndpointEnabled = true;
						}
						else if (serviceEndpointElement.Address.OriginalString.Equals("wssecurity/x509cert", StringComparison.OrdinalIgnoreCase))
						{
							this.wsSecurityX509CertEndpointEnabled = true;
						}
						else if (serviceEndpointElement.Address.OriginalString.Equals("wssecurity", StringComparison.OrdinalIgnoreCase))
						{
							this.wsSecurityEndpointEnabled = true;
						}
						else if (serviceEndpointElement.Address.OriginalString.Equals(AutodiscoverWebConfiguration.soapAddress, StringComparison.OrdinalIgnoreCase))
						{
							this.soapEndpointEnabled = true;
						}
					}
				}
			}
			this.oAuthEndpointEnabled = OAuthHttpModule.IsModuleLoaded.Value;
			this.mySiteServiceUrlTemplate = WebConfigurationManager.AppSettings["mySiteServiceUrlTemplate"];
			this.mySiteLocationUrlTemplate = WebConfigurationManager.AppSettings["mySiteLocationUrlTemplate"];
			this.projectSiteServiceUrl = WebConfigurationManager.AppSettings["projectSiteServiceUrl"];
			this.projectSiteLocationUrl = WebConfigurationManager.AppSettings["projectSiteLocationUrl"];
			this.documentTypesSupportedForSharing = WebConfigurationManager.AppSettings["documentTypesSupportedForSharing"];
		}

		public string MySiteServiceUrlTemplate
		{
			get
			{
				return this.mySiteServiceUrlTemplate;
			}
		}

		public string MySiteLocationUrlTemplate
		{
			get
			{
				return this.mySiteLocationUrlTemplate;
			}
		}

		public string ProjectSiteServiceUrl
		{
			get
			{
				return this.projectSiteServiceUrl;
			}
		}

		public string ProjectSiteLocationUrl
		{
			get
			{
				return this.projectSiteLocationUrl;
			}
		}

		public string DocumentTypesSupportedForSharing
		{
			get
			{
				return this.documentTypesSupportedForSharing;
			}
		}

		internal bool SoapEndpointEnabled
		{
			get
			{
				return this.soapEndpointEnabled;
			}
		}

		internal bool WsSecurityEndpointEnabled
		{
			get
			{
				return this.wsSecurityEndpointEnabled;
			}
		}

		internal bool WsSecuritySymmetricKeyEndpointEnabled
		{
			get
			{
				return this.wsSecuritySymmetricKeyEndpointEnabled;
			}
		}

		internal bool WsSecurityX509CertEndpointEnabled
		{
			get
			{
				return this.wsSecurityX509CertEndpointEnabled;
			}
		}

		internal bool OAuthEndpointEnabled
		{
			get
			{
				return this.oAuthEndpointEnabled;
			}
		}

		private static string soapAddress = string.Empty;

		private readonly bool soapEndpointEnabled;

		private readonly bool wsSecurityEndpointEnabled;

		private readonly bool wsSecuritySymmetricKeyEndpointEnabled;

		private readonly bool wsSecurityX509CertEndpointEnabled;

		private readonly bool oAuthEndpointEnabled;

		private string mySiteServiceUrlTemplate;

		private string mySiteLocationUrlTemplate;

		private string projectSiteServiceUrl;

		private string projectSiteLocationUrl;

		private string documentTypesSupportedForSharing;
	}
}
