using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal interface IIdentityExchangeCertificateCmdlet
	{
		ExchangeCertificateIdParameter Identity { get; set; }

		ServerIdParameter Server { get; set; }

		string Thumbprint { get; set; }
	}
}
