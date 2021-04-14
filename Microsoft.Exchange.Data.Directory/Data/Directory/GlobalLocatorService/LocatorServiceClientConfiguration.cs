using System;
using System.Configuration;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal class LocatorServiceClientConfiguration : ConfigurationSection
	{
		public static string SectionName
		{
			get
			{
				return "globalLocatorService";
			}
		}

		public static LocatorServiceClientConfiguration Instance
		{
			get
			{
				return LocatorServiceClientConfiguration.instance;
			}
		}

		public ServiceEndpoint Endpoint
		{
			get
			{
				if (!string.IsNullOrEmpty(this.EndpointUri))
				{
					return new ServiceEndpoint(new Uri(this.EndpointUri), this.EndpointUriTemplate ?? string.Empty, this.EndpointCertSubject, this.EndpointToken);
				}
				return null;
			}
		}

		[ConfigurationProperty("endpointUri", IsRequired = false)]
		public string EndpointUri
		{
			get
			{
				return base["endpointUri"] as string;
			}
		}

		[ConfigurationProperty("endpointUriTemplate", IsRequired = false)]
		public string EndpointUriTemplate
		{
			get
			{
				return base["endpointUriTemplate"] as string;
			}
		}

		[ConfigurationProperty("endpointCertSubject", IsRequired = false)]
		public string EndpointCertSubject
		{
			get
			{
				return base["endpointCertSubject"] as string;
			}
		}

		[ConfigurationProperty("endpointToken", IsRequired = false)]
		public string EndpointToken
		{
			get
			{
				return base["endpointToken"] as string;
			}
		}

		private const string EndpointUriKey = "endpointUri";

		private const string EndpointUriTemplateKey = "endpointUriTemplate";

		private const string EndpointCertSubjectKey = "endpointCertSubject";

		private const string EndpointTokenKey = "endpointToken";

		private static LocatorServiceClientConfiguration instance = ((LocatorServiceClientConfiguration)ConfigurationManager.GetSection(LocatorServiceClientConfiguration.SectionName)) ?? new LocatorServiceClientConfiguration();
	}
}
