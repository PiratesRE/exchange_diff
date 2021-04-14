using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Net;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ClientAccess
{
	internal class ClearLsassCacheResponder : ResponderWorkItem
	{
		public static ResponderDefinition CreateDefinition(string name, string alertMask, ServerComponentEnum serverComponentToVerifyState, ServiceHealthStatus responderTargetState)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			responderDefinition.TypeName = typeof(ClearLsassCacheResponder).FullName;
			responderDefinition.Name = name;
			responderDefinition.AlertMask = alertMask;
			responderDefinition.TargetHealthState = responderTargetState;
			responderDefinition.Attributes["ComponentStateServerComponentName"] = serverComponentToVerifyState.ToString();
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, "Cafe", RecoveryActionId.ClearLsassCache, Environment.MachineName, null);
			return responderDefinition;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.ClearLsassCache, Environment.MachineName, this, true, cancellationToken, null);
			recoveryActionRunner.Execute(delegate()
			{
				this.ClearLsassCache();
			});
		}

		private void ClearLsassCache()
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			int num = 0;
			try
			{
				num = ProcessRunner.Run(Path.Combine(Environment.SystemDirectory, "klist.exe"), " purge -li 0x3e7", 60000, null, out empty, out empty2);
			}
			catch (Exception ex)
			{
				base.Result.StateAttribute2 = ex.ToString();
				throw;
			}
			base.Result.StateAttribute1 = string.Format("klist.exe purge exit code: {0}, output: {1}, error: {2}", num, empty, empty2);
		}
	}
}
