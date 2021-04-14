using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class TestKDCServiceProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
			if (pDef == null)
			{
				throw new ArgumentException("Please specify a value for probeDefinition");
			}
			if (!propertyBag.ContainsKey("ServiceStartStopRetryCount"))
			{
				throw new ArgumentException("Please specify value forServiceStartStopRetryCount");
			}
			pDef.Attributes["ServiceStartStopRetryCount"] = propertyBag["ServiceStartStopRetryCount"].ToString().Trim();
			if (!propertyBag.ContainsKey("KDCStartOnProvisionDCEnabled"))
			{
				throw new ArgumentException("Please specify value forKDCStartOnProvisionDCEnabled");
			}
			pDef.Attributes["KDCStartOnProvisionDCEnabled"] = propertyBag["KDCStartOnProvisionDCEnabled"].ToString().Trim();
			if (propertyBag.ContainsKey("KDCStopOnMMDCEnabled"))
			{
				pDef.Attributes["KDCStopOnMMDCEnabled"] = propertyBag["KDCStopOnMMDCEnabled"].ToString().Trim();
				return;
			}
			throw new ArgumentException("Please specify value forKDCStopOnMMDCEnabled");
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.TestKDCService, delegate
			{
				string localFQDN = DirectoryGeneralUtils.GetLocalFQDN();
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = true;
				bool flag2 = true;
				bool flag3 = true;
				bool flag4 = false;
				if (!base.Definition.Attributes.ContainsKey("ServiceStartStopRetryCount"))
				{
					throw new ArgumentException("ServiceStartStopRetryCount");
				}
				int retryCount = int.Parse(base.Definition.Attributes["ServiceStartStopRetryCount"]);
				if (!base.Definition.Attributes.ContainsKey("KDCStartOnProvisionDCEnabled"))
				{
					throw new ArgumentException("KDCStartOnProvisionDCEnabled");
				}
				bool flag5 = bool.Parse(base.Definition.Attributes["KDCStartOnProvisionDCEnabled"]);
				if (!base.Definition.Attributes.ContainsKey("KDCStopOnMMDCEnabled"))
				{
					throw new ArgumentException("KDCStopOnMMDCEnabled");
				}
				bool flag6 = bool.Parse(base.Definition.Attributes["KDCStopOnMMDCEnabled"]);
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Starting KDC service check on server {0}", localFQDN, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\TestKDCServiceProbe.cs", 120);
				try
				{
					flag2 = DirectoryGeneralUtils.CheckIfDCProvisioned(localFQDN);
					flag3 = DirectoryGeneralUtils.CheckIfDCProvisionedLocal(localFQDN);
					flag4 = true;
				}
				catch (Exception ex)
				{
					stringBuilder.Append(string.Format("Probe caught exception {0} while trying to check Provisioning bit on DC. This might be transient Rid Master issue. Skip this instance.", ex.Message));
					flag = true;
					flag4 = false;
				}
				if (flag3 == flag2 && flag4)
				{
					stringBuilder.Append(string.Format("Local DC provisioning status {0};", flag2));
					ServiceControllerStatus localServiceStatus = DirectoryUtils.GetLocalServiceStatus("KDC");
					stringBuilder.Append(string.Format("Current KDC service Status {0};", localServiceStatus));
					if (flag2 && localServiceStatus != ServiceControllerStatus.Running && flag5)
					{
						stringBuilder.Append("DC is provisioned but KDC is not Running, startinng KDC service;");
						DirectoryUtils.GetServiceIntoExpectedStatus("KDC", ServiceControllerStatus.Running, retryCount);
					}
					if (!flag2 && localServiceStatus != ServiceControllerStatus.Stopped && flag6)
					{
						stringBuilder.Append("DC is not provisioned but KDC is not Stopped, Transfer FSMO role if required and stopping KDC service;");
						bool flag7 = DirectoryGeneralUtils.TransferFSMORoleFromDC(localFQDN, base.TraceContext);
						if (flag7)
						{
							DirectoryUtils.GetServiceIntoExpectedStatus("KDC", ServiceControllerStatus.Stopped, retryCount);
						}
						else
						{
							stringBuilder.Append("FSMO transfer from a DC that was in MM has failed.  Stopping KDC is skipped. ");
						}
					}
					localServiceStatus = DirectoryUtils.GetLocalServiceStatus("KDC");
					if (flag2 && localServiceStatus != ServiceControllerStatus.Running && flag5)
					{
						stringBuilder.Append(string.Format("After recovery KDC service Status {0}, expected Running. Probe recovery action Failed.", localServiceStatus));
						base.Result.StateAttribute2 = DirectoryUtils.ExceptionType.KDCNotRunningOnProvisionedDC.ToString();
						base.Result.Error = stringBuilder.ToString();
						flag = false;
					}
					if (!flag2 && localServiceStatus != ServiceControllerStatus.Stopped && flag6)
					{
						stringBuilder.Append(string.Format("After recovery KDC service Status {0}, expected Stopped. Probe recovery action Failed.", localServiceStatus));
						base.Result.StateAttribute2 = DirectoryUtils.ExceptionType.KDCNotStoppedOnMaintenanceDC.ToString();
						base.Result.Error = stringBuilder.ToString();
						flag = false;
					}
				}
				else
				{
					stringBuilder.Append(string.Format("Local DC provisioning status {0}, on RidMaster {1}. The provisioning bit does not match or failed to get provisioning bit, skip this test.", flag3, flag2));
				}
				base.Result.StateAttribute1 = stringBuilder.ToString();
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, base.TraceContext, base.Result.StateAttribute1, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\TestKDCServiceProbe.cs", 196);
				if (!flag)
				{
					throw new Exception(base.Result.Error);
				}
			});
		}
	}
}
