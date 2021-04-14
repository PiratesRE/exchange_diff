using System;
using System.Net;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Provisioning.Responders
{
	public class ProvisioningSTXEscalateResponder : EscalateResponder
	{
		public static void InitTypeNameAndAssemblyPath(ResponderDefinition definition)
		{
			definition.AssemblyPath = ProvisioningSTXEscalateResponder.AssemblyPath;
			definition.TypeName = ProvisioningSTXEscalateResponder.TypeName;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			bool flag = Convert.ToBoolean(base.Definition.ExtensionAttributes);
			string stateAttribute = string.Empty;
			if (flag)
			{
				StxLogType logType = this.GetLogType();
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
					StxLoggerBase.GetLoggerInstance(logType).BeginAppend(Dns.GetHostName(), false, new TimeSpan(0L), 1, ex.Message, "escalate", stateAttribute, null, null);
					throw;
				}
				StxLoggerBase.GetLoggerInstance(logType).BeginAppend(Dns.GetHostName(), true, new TimeSpan(0L), 0, null, "escalate", stateAttribute, null, null);
				return;
			}
			base.DoResponderWork(cancellationToken);
		}

		private StxLogType GetLogType()
		{
			if (base.Definition.Name == "ForwardSyncCompanyEscalate")
			{
				return StxLogType.TestForwardSyncCompanyResponder;
			}
			if (base.Definition.Name == "ForwardSyncCookieNotUpToDateEscalate")
			{
				return StxLogType.TestForwardSyncCookieResponder;
			}
			throw new Exception("The log Type is not supported yet");
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ProvisioningSTXEscalateResponder).FullName;
	}
}
