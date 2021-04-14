using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Migration.Probes
{
	public class MRSQueueScanProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
			if (pDef == null)
			{
				throw new ArgumentException("Please specify a value for probeDefinition");
			}
			if (!propertyBag.ContainsKey("TargetResource"))
			{
				throw new ArgumentException("Please specify value for TargetResource");
			}
			pDef.TargetResource = propertyBag["TargetResource"].ToString().Trim();
			if (propertyBag.ContainsKey("TargetExtension"))
			{
				pDef.TargetExtension = propertyBag["TargetExtension"].ToString().Trim();
				return;
			}
			throw new ArgumentException("Please specify value for TargetExtension");
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			string targetResource = base.Definition.TargetResource;
			string targetExtension = base.Definition.TargetExtension;
			base.Result.StateAttribute1 = targetResource;
			base.Result.StateAttribute2 = targetExtension;
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "Starting MRS queue scan check against server {0}", targetResource, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Migration\\MRSQueueScanProbe.cs", 69);
			MRSHealthCheckOutcome mrshealthCheckOutcome = MRSHealth.VerifyServiceIsUp(targetResource, targetExtension, null);
			if (!mrshealthCheckOutcome.Passed)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.MigrationTracer, base.TraceContext, string.Format("MRS queue scan check for server {0} not run since the server was down: {1}", targetResource, mrshealthCheckOutcome.Message.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Migration\\MRSQueueScanProbe.cs", 78);
				base.Result.StateAttribute3 = mrshealthCheckOutcome.Message.ToString();
				return;
			}
			MRSHealthCheckOutcome mrshealthCheckOutcome2 = MRSHealth.VerifyServiceIsScanningForJobs(targetResource, 3600L, null);
			base.Result.StateAttribute3 = mrshealthCheckOutcome2.Message.ToString();
			if (mrshealthCheckOutcome2.Passed)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.MigrationTracer, base.TraceContext, string.Format("MRS queue scan check for server {0} succeeded with the following message: {1}", targetResource, mrshealthCheckOutcome2.Message.ToString()), null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Migration\\MRSQueueScanProbe.cs", 96);
				return;
			}
			string message = string.Format("MRS queue scan check for server {0} failed with the following error: {1}", targetResource, mrshealthCheckOutcome2.Message.ToString());
			WTFDiagnostics.TraceError(ExTraceGlobals.MigrationTracer, base.TraceContext, message, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Migration\\MRSQueueScanProbe.cs", 111);
			throw new Exception(message);
		}
	}
}
