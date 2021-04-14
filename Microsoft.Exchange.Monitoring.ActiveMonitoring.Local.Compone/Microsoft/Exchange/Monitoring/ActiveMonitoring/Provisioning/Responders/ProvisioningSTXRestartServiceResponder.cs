using System;
using System.Net;
using System.Reflection;
using System.Threading;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Provisioning.Responders
{
	public class ProvisioningSTXRestartServiceResponder : RestartServiceResponder
	{
		public static void InitTypeNameAndAssemblyPath(ResponderDefinition definition)
		{
			definition.AssemblyPath = ProvisioningSTXRestartServiceResponder.AssemblyPath;
			definition.TypeName = ProvisioningSTXRestartServiceResponder.TypeName;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			bool flag = Convert.ToBoolean(base.Definition.ExtensionAttributes);
			if (flag)
			{
				string stateAttribute = string.Empty;
				try
				{
					FowardSyncEventRecord arbitrationEventLog = ForwardSyncEventlogUtil.GetArbitrationEventLog();
					if (arbitrationEventLog != null)
					{
						stateAttribute = arbitrationEventLog.ServiceInstanceName;
					}
					base.DoResponderWork(cancellationToken);
				}
				catch (Exception ex)
				{
					StxLoggerBase.GetLoggerInstance(StxLogType.TestForwardSyncCookieResponder).BeginAppend(Dns.GetHostName(), false, new TimeSpan(0L), 1, ex.Message, "restart service", base.WindowsServiceName, stateAttribute, null);
					throw;
				}
				StxLoggerBase.GetLoggerInstance(StxLogType.TestForwardSyncCookieResponder).BeginAppend(Dns.GetHostName(), true, new TimeSpan(0L), 0, null, "restart service", base.WindowsServiceName, stateAttribute, null);
				return;
			}
			base.DoResponderWork(cancellationToken);
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ProvisioningSTXRestartServiceResponder).FullName;
	}
}
