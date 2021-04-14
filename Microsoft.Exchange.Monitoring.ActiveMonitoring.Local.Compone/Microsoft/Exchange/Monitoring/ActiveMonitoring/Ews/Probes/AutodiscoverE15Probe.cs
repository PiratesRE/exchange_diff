using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Ews.Probes
{
	public class AutodiscoverE15Probe : AutodiscoverCommon
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.RunAutodiscoverProbe(cancellationToken, "AutodiscoverE15Probe");
		}

		public override void PopulateDefinition<Definition>(Definition definition, Dictionary<string, string> propertyBag)
		{
			ProbeDefinition probeDefinition = definition as ProbeDefinition;
			string text = propertyBag["Name"];
			bool flag = true;
			bool isMbxProbe = true;
			string a;
			if ((a = text.ToLower()) != null)
			{
				if (!(a == "autodiscoverselftestProbe"))
				{
					if (a == "autodiscoverctpprobe")
					{
						flag = (LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled && !LocalEndpointManager.IsDataCenter);
						isMbxProbe = false;
					}
				}
				else
				{
					flag = LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled;
					isMbxProbe = true;
				}
			}
			if (!flag)
			{
				throw new InvalidOperationException(string.Format("The server role is not valid for probe type {0}", text));
			}
			string location = Assembly.GetExecutingAssembly().Location;
			string fullName = typeof(AutodiscoverE15Probe).FullName;
			string autodiscoverSvcEndpoint = EwsConstants.AutodiscoverSvcEndpoint;
			InvokeProbeUtils.PopulateDefinition(probeDefinition, propertyBag, location, fullName, autodiscoverSvcEndpoint, isMbxProbe);
			probeDefinition.SecondaryEndpoint = EwsConstants.AutodiscoverXmlEndpoint;
		}
	}
}
