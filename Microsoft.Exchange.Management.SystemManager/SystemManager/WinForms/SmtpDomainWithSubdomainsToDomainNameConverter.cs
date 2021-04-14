using System;
using Microsoft.Exchange.Data;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class SmtpDomainWithSubdomainsToDomainNameConverter : ValueToDisplayObjectConverter
	{
		public object Convert(object valueObject)
		{
			SmtpDomainWithSubdomains smtpDomainWithSubdomains = (SmtpDomainWithSubdomains)valueObject;
			if (!SmtpDomainWithSubdomains.StarDomain.Equals(smtpDomainWithSubdomains))
			{
				return smtpDomainWithSubdomains.Domain;
			}
			return string.Empty;
		}
	}
}
