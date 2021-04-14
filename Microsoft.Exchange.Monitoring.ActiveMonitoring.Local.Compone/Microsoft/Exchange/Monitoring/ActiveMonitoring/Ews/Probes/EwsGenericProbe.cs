using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Ews.Probes
{
	public class EwsGenericProbe : EWSGenericProbeCommon
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			base.RunEWSGenericProbe(cancellationToken);
		}

		public override void PopulateDefinition<Definition>(Definition definition, Dictionary<string, string> propertyBag)
		{
			ProbeDefinition probeDefinition = definition as ProbeDefinition;
			string text = propertyBag["Name"];
			string value = "GetFolder";
			bool flag = true;
			bool isMbxProbe = true;
			string a;
			if ((a = text.ToLower()) != null)
			{
				if (!(a == "ewsdeeptestprobe") && !(a == "ewsselftestprobe"))
				{
					if (a == "ewsctpprobe")
					{
						flag = (LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsCafeRoleInstalled && !LocalEndpointManager.IsDataCenter);
						isMbxProbe = false;
					}
				}
				else
				{
					flag = LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled;
					isMbxProbe = true;
					if (text.Equals("ewsselftestprobe"))
					{
						value = "ConvertId";
					}
				}
			}
			if (!flag)
			{
				throw new InvalidOperationException(string.Format("The server role is not valid for probe type {0}", text));
			}
			string location = Assembly.GetExecutingAssembly().Location;
			string fullName = typeof(EwsGenericProbe).FullName;
			string ewsEndpoint = EwsConstants.EwsEndpoint;
			InvokeProbeUtils.PopulateDefinition(probeDefinition, propertyBag, location, fullName, ewsEndpoint, isMbxProbe);
			probeDefinition.Attributes["OperationName"] = value;
		}
	}
}
