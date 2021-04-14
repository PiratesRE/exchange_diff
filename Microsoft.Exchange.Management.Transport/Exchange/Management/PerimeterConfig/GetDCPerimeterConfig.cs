using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Ehf;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services;

namespace Microsoft.Exchange.Management.PerimeterConfig
{
	[OutputType(new Type[]
	{
		typeof(DCPerimeterConfig)
	})]
	[Cmdlet("Get", "DCPerimeterConfig")]
	public class GetDCPerimeterConfig : DCPerimeterConfigTask
	{
		internal override void InvokeWebService(IConfigurationSession session, EhfTargetServerConfig config, EhfProvisioningService provisioningService)
		{
			Company reseller = provisioningService.GetReseller(config.ResellerId);
			List<IPAddress> list = new List<IPAddress>();
			if (reseller.Settings.InboundIPList != null && reseller.Settings.InboundIPList.IPList != null && reseller.Settings.InboundIPList.IPList.Length == 1 && reseller.Settings.InboundIPList.IPList[0].IPList != null)
			{
				SmtpProfileEntry[] iplist = reseller.Settings.InboundIPList.IPList[0].IPList;
				foreach (SmtpProfileEntry smtpProfileEntry in iplist)
				{
					IPAddress item;
					if (IPAddress.TryParse(smtpProfileEntry.IP, out item))
					{
						list.Add(item);
					}
					else
					{
						this.WriteWarning(new LocalizedString("Unable to parse inbound IP address: " + smtpProfileEntry.IP));
					}
				}
			}
			List<IPAddress> list2 = new List<IPAddress>();
			if (reseller.Settings.OutboundIPList != null && reseller.Settings.OutboundIPList.IPList != null)
			{
				foreach (string text in reseller.Settings.OutboundIPList.IPList)
				{
					IPAddress item2;
					if (IPAddress.TryParse(text, out item2))
					{
						list2.Add(item2);
					}
					else
					{
						this.WriteWarning(new LocalizedString("Unable to parse outbound IP address: " + text));
					}
				}
			}
			base.WriteObject(new DCPerimeterConfig(list.ToArray(), list2.ToArray()));
		}
	}
}
