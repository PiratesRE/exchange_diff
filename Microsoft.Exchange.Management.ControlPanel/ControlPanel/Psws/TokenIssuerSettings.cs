using System;
using System.Configuration;

namespace Microsoft.Exchange.Management.ControlPanel.Psws
{
	internal class TokenIssuerSettings
	{
		internal TokenIssuerSettings()
		{
		}

		internal TokenIssuerSettings(string partnerId, string serviceId, string serviceHostName, string acsId, string acsUrl, string certificateSubject)
		{
			this.PartnerId = partnerId;
			this.ServiceId = serviceId;
			this.ServiceHostName = serviceHostName;
			this.AcsId = acsId;
			this.AcsUrl = new Uri(acsUrl);
			this.CertificateSubject = certificateSubject;
		}

		internal string PartnerId { get; set; }

		internal string ServiceId { get; set; }

		internal string ServiceHostName { get; set; }

		internal string AcsId { get; set; }

		internal Uri AcsUrl { get; set; }

		internal string CertificateSubject { get; set; }

		internal static TokenIssuerSettings CreateFromConfiguration()
		{
			return new TokenIssuerSettings(ConfigurationManager.AppSettings["PswsPartnerId"], ConfigurationManager.AppSettings["PswsServiceId"], ConfigurationManager.AppSettings["PswsServiceHostName"], ConfigurationManager.AppSettings["PswsAcsId"], ConfigurationManager.AppSettings["PswsAcsUrl"], ConfigurationManager.AppSettings["PswsCertSubject"]);
		}
	}
}
