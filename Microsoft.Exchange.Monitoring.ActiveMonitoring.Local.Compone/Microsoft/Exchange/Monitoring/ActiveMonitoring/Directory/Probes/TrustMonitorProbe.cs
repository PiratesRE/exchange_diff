using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory.Probes
{
	public class TrustMonitorProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.Logger(this, StxLogType.TrustMonitorProbe, delegate
			{
				this.DomainControllerName = new ServerIdParameter().ToString();
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Starting TrustMonitorProbe on DC: {0}", this.DomainControllerName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\TrustMonitorProbe.cs", 59);
				this.ValidateTrusts();
			});
		}

		private void ValidateTrusts()
		{
			List<string> trustedDomains = DirectoryGeneralUtils.GetTrustedDomains();
			StringBuilder stringBuilder = new StringBuilder();
			if (trustedDomains != null && trustedDomains.Count > 0)
			{
				foreach (string text in trustedDomains)
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Working on domain: {0}", text, null, "ValidateTrusts", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\TrustMonitorProbe.cs", 81);
					if (text.ToLower().Contains("mgt") || text.ToLower().Contains("apcprd05"))
					{
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Skipping domain: {0}", text, null, "ValidateTrusts", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\TrustMonitorProbe.cs", 91);
					}
					else if (!this.VerifyDomainReadAccess(text))
					{
						stringBuilder.AppendFormat("{0} ", text);
					}
				}
			}
			if (!string.IsNullOrEmpty(stringBuilder.ToString()))
			{
				throw new Exception(string.Format("Found some domains for which trusts are created, but read operation failed: {0}", stringBuilder.ToString()));
			}
		}

		private bool VerifyDomainReadAccess(string domain)
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "VerifyDomainReadAccess domain: {0}", domain, null, "VerifyDomainReadAccess", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\TrustMonitorProbe.cs", 118);
			bool result = false;
			string text = string.Empty;
			string text2 = string.Empty;
			try
			{
				using (DirectoryEntry directoryEntry = new DirectoryEntry(domain))
				{
					if (directoryEntry != null)
					{
						WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "VerifyDomainReadAccess:: Success in domain: {0}", domain, null, "VerifyDomainReadAccess", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\TrustMonitorProbe.cs", 134);
						directoryEntry.Properties["distinguishedName"].Value.ToString();
						result = true;
						text2 = string.Format("Success in domain {0}\n", domain);
						if (string.IsNullOrEmpty(base.Result.StateAttribute2))
						{
							base.Result.StateAttribute2 = text2;
						}
						else
						{
							ProbeResult result2 = base.Result;
							result2.StateAttribute2 += text2;
						}
					}
				}
			}
			catch (Exception ex)
			{
				text = string.Format("Failed in domain {0}.  Error message: {1}\n", domain, ex.Message);
				WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.DirectoryTracer, base.TraceContext, "VerifyDomainReadAccess:: Exception in domain: {0}, Exception: {1}", domain, ex.Message, null, "VerifyDomainReadAccess", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\TrustMonitorProbe.cs", 161);
				if (string.IsNullOrEmpty(base.Result.StateAttribute1))
				{
					base.Result.StateAttribute1 = text;
				}
				else
				{
					ProbeResult result3 = base.Result;
					result3.StateAttribute1 += text;
				}
			}
			return result;
		}

		public string DomainControllerName = string.Empty;
	}
}
