using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public class IisService
	{
		public IisService(string name)
		{
			this.serviceName = name;
		}

		internal IisService(ADVirtualDirectory virtualDirectory) : this(virtualDirectory.Name)
		{
			this.internalUrl = virtualDirectory.InternalUrl;
			this.externalUrl = virtualDirectory.ExternalUrl;
		}

		internal IisService(IisService item, IEnumerable<SmtpDomainWithSubdomains> certificateDomains) : this(item.ServiceName)
		{
			this.internalUrl = item.InternalUrl;
			this.externalUrl = item.ExternalUrl;
			if (this.InternalUrl != null || this.ExternalUrl != null)
			{
				foreach (SmtpDomainWithSubdomains smtpDomainWithSubdomains in certificateDomains)
				{
					if (this.InternalUrl != null && string.Equals(this.InternalUrl.Host, smtpDomainWithSubdomains.Address, StringComparison.OrdinalIgnoreCase))
					{
						this.internalUrlValid = UrlValidation.Valid;
					}
					if (this.ExternalUrl != null && string.Equals(this.ExternalUrl.Host, smtpDomainWithSubdomains.Address, StringComparison.OrdinalIgnoreCase))
					{
						this.externalUrlValid = UrlValidation.Valid;
					}
				}
			}
		}

		public string ServiceName
		{
			get
			{
				return this.serviceName;
			}
		}

		public Uri InternalUrl
		{
			get
			{
				return this.internalUrl;
			}
		}

		public UrlValidation InternalUrlValid
		{
			get
			{
				return this.internalUrlValid;
			}
		}

		public Uri ExternalUrl
		{
			get
			{
				return this.externalUrl;
			}
		}

		public UrlValidation ExternalUrlValid
		{
			get
			{
				return this.externalUrlValid;
			}
		}

		private readonly string serviceName;

		private Uri internalUrl;

		private UrlValidation internalUrlValid;

		private Uri externalUrl;

		private UrlValidation externalUrlValid;
	}
}
