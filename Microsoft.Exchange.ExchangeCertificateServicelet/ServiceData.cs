using System;
using System.Collections.Generic;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Servicelets.ExchangeCertificate
{
	internal struct ServiceData
	{
		public ServiceData(string domain, AllowedServices flags)
		{
			this.domain = domain;
			this.thumbprint = null;
			this.flag = flags;
			this.iisServices = new List<IisService>();
		}

		public ServiceData(string domain, string thumbPrint, AllowedServices flags)
		{
			this.domain = domain;
			this.thumbprint = thumbPrint;
			this.flag = flags;
			this.iisServices = new List<IisService>();
		}

		public string Domain
		{
			get
			{
				return this.domain;
			}
		}

		public AllowedServices Flag
		{
			get
			{
				return this.flag;
			}
		}

		public string Thumbprint
		{
			get
			{
				return this.thumbprint;
			}
		}

		public List<IisService> IisServices
		{
			get
			{
				return this.iisServices;
			}
		}

		private string domain;

		private string thumbprint;

		private AllowedServices flag;

		private List<IisService> iisServices;
	}
}
