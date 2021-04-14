using System;
using System.Collections;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.FfoReporting.Providers
{
	public interface ISmtpCheckerProvider
	{
		IEnumerable GetMxRecords(Fqdn domain, IConfigDataProvider configDataProvider);

		IEnumerable GetOutboundConnectors(Fqdn domain, IConfigDataProvider configDataProvider);

		IEnumerable GetServiceDeliveries(SmtpAddress recipient, IConfigDataProvider configDataProvider);
	}
}
