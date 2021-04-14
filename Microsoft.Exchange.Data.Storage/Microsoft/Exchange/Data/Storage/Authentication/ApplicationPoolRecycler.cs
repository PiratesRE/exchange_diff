using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security.ExternalAuthentication;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Data.Storage.Authentication
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ApplicationPoolRecycler
	{
		public static void EnableOnFederationTrustCertificateChange()
		{
			if (!ApplicationPoolRecycler.enabled)
			{
				lock (ApplicationPoolRecycler.locker)
				{
					if (!ApplicationPoolRecycler.enabled)
					{
						ApplicationPoolRecycler.initialCertificates = ApplicationPoolRecycler.GetCertificatesThumbprint();
						FederationTrustCache.Change += ApplicationPoolRecycler.ChangeHandler;
						ApplicationPoolRecycler.enabled = true;
					}
				}
			}
		}

		private static void ChangeHandler()
		{
			HashSet<string> certificatesThumbprint = ApplicationPoolRecycler.GetCertificatesThumbprint();
			if (!certificatesThumbprint.SetEquals(ApplicationPoolRecycler.initialCertificates))
			{
				ApplicationPoolRecycler.RecycleThisApplicationPool();
			}
		}

		private static void RecycleThisApplicationPool()
		{
			Process currentProcess = Process.GetCurrentProcess();
			ApplicationPoolRecycler.Tracer.TraceDebug<int>(0L, "Searching for application pool of the current process {0}", currentProcess.Id);
			using (ServerManager serverManager = new ServerManager())
			{
				foreach (WorkerProcess workerProcess in serverManager.WorkerProcesses)
				{
					if (workerProcess.ProcessId == currentProcess.Id)
					{
						ApplicationPool applicationPool = serverManager.ApplicationPools[workerProcess.AppPoolName];
						if (applicationPool != null)
						{
							ApplicationPoolRecycler.Tracer.TraceDebug<int, string>(0L, "Found application pool current process {0}: {1}. Recycling application pool now.", currentProcess.Id, applicationPool.Name);
							applicationPool.Recycle();
							return;
						}
					}
				}
			}
			ApplicationPoolRecycler.Tracer.TraceError<int>(0L, "Unable to find application pool of the current process {0}. Application pool will continue to run without updated certificates", currentProcess.Id);
		}

		private static HashSet<string> GetCertificatesThumbprint()
		{
			IEnumerable<FederationTrust> federationTrusts = FederationTrustCache.GetFederationTrusts();
			HashSet<string> hashSet = new HashSet<string>();
			foreach (FederationTrust federationTrust in federationTrusts)
			{
				if (federationTrust.OrgPrivCertificate != null)
				{
					hashSet.Add(federationTrust.OrgPrivCertificate);
				}
				if (federationTrust.OrgPrevPrivCertificate != null)
				{
					hashSet.Add(federationTrust.OrgPrevPrivCertificate);
				}
				if (federationTrust.TokenIssuerCertificate != null)
				{
					hashSet.Add(federationTrust.TokenIssuerCertificate.Thumbprint);
				}
				if (federationTrust.TokenIssuerPrevCertificate != null)
				{
					hashSet.Add(federationTrust.TokenIssuerPrevCertificate.Thumbprint);
				}
			}
			return hashSet;
		}

		private static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.ConfigurationTracer;

		private static bool enabled;

		private static object locker = new object();

		private static HashSet<string> initialCertificates;
	}
}
