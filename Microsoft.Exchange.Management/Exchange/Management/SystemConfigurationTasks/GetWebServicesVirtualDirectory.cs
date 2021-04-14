using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "WebServicesVirtualDirectory", DefaultParameterSetName = "Identity")]
	public sealed class GetWebServicesVirtualDirectory : GetExchangeServiceVirtualDirectory<ADWebServicesVirtualDirectory>
	{
		protected override void ProcessMetabaseProperties(ExchangeVirtualDirectory dataObject)
		{
			TaskLogger.LogEnter();
			base.ProcessMetabaseProperties(dataObject);
			ADWebServicesVirtualDirectory adwebServicesVirtualDirectory = (ADWebServicesVirtualDirectory)dataObject;
			adwebServicesVirtualDirectory.GzipLevel = Gzip.GetGzipLevel(adwebServicesVirtualDirectory.MetabasePath);
			adwebServicesVirtualDirectory.CertificateAuthentication = base.GetCertificateAuthentication(dataObject, "Management");
			adwebServicesVirtualDirectory.LiveIdNegotiateAuthentication = base.GetLiveIdNegotiateAuthentication(dataObject, "Nego2");
			TaskLogger.LogExit();
		}
	}
}
