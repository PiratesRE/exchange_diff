using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal class ADObjIdToVDirMap : LazyLookupExactTimeoutCache<string, ADMobileVirtualDirectory>
	{
		internal ADObjIdToVDirMap() : base(1000, false, GlobalSettings.VdirCacheTimeoutSeconds, CacheFullBehavior.ExpireExisting)
		{
			AirSyncDiagnostics.TraceDebug<double>(ExTraceGlobals.AlgorithmTracer, null, "VDir object cache created with age-out interval of {0} minutes", GlobalSettings.VdirCacheTimeoutSeconds.TotalMinutes);
		}

		protected override ADMobileVirtualDirectory CreateOnCacheMiss(string vDirObjectId, ref bool shouldAdd)
		{
			if (string.IsNullOrEmpty(vDirObjectId))
			{
				throw new ArgumentNullException("vDirObjectId");
			}
			int num = vDirObjectId.LastIndexOf('/');
			string forestFqdn;
			ADObjectId adobjectId;
			if (num == -1)
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.AlgorithmTracer, null, "VDir header from Cafe server: {0} contains only vDirObjectId. Read data using local forest.", vDirObjectId);
				forestFqdn = string.Empty;
				adobjectId = this.ParseObjectGuid(vDirObjectId);
			}
			else
			{
				forestFqdn = vDirObjectId.Substring(num + 1);
				adobjectId = this.ParseObjectGuid(vDirObjectId.Substring(0, num));
			}
			if (adobjectId == null)
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.AlgorithmTracer, null, "ObjectGuid could not be parsed from vDir header: {0}", vDirObjectId.Substring(0, num));
				shouldAdd = false;
				return null;
			}
			ADMobileVirtualDirectory admobileVirtualDirectory = null;
			try
			{
				string suitableDomainController = this.GetSuitableDomainController(forestFqdn);
				if (!string.IsNullOrEmpty(suitableDomainController))
				{
					ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(suitableDomainController, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromRootOrgScopeSet(), 108, "CreateOnCacheMiss", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\ADObjIdToVDirMap.cs");
					admobileVirtualDirectory = topologyConfigurationSession.Read<ADMobileVirtualDirectory>(adobjectId);
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.AlgorithmTracer, null, "VDir object read from AD and cached: {0}", vDirObjectId);
				}
				else
				{
					AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.AlgorithmTracer, null, "Failure looking up vDir object {0}", vDirObjectId);
				}
			}
			catch (ADTransientException ex)
			{
				AirSyncDiagnostics.TraceError<string, string>(ExTraceGlobals.AlgorithmTracer, null, "ADTransientException looking up vDir object {0}: {1}", vDirObjectId, ex.Message);
			}
			if (admobileVirtualDirectory == null)
			{
				shouldAdd = false;
			}
			return admobileVirtualDirectory;
		}

		private ADObjectId ParseObjectGuid(string guid)
		{
			if (string.IsNullOrEmpty(guid))
			{
				return null;
			}
			ADObjectId result = null;
			try
			{
				Guid guid2 = new Guid(guid);
				result = new ADObjectId(guid2);
			}
			catch (FormatException ex)
			{
				AirSyncDiagnostics.TraceError<string, string>(ExTraceGlobals.AlgorithmTracer, null, "Failure parsing vDir objectGUID {0} - Exception {1}", guid, ex.Message);
			}
			catch (OverflowException ex2)
			{
				AirSyncDiagnostics.TraceError<string, string>(ExTraceGlobals.AlgorithmTracer, null, "Failure parsing vDir objectGUID {0} - Exception {1}", guid, ex2.Message);
			}
			return result;
		}

		private string GetSuitableDomainController(string forestFqdn)
		{
			ADForest adforest;
			if (string.IsNullOrEmpty(forestFqdn))
			{
				adforest = ADForest.GetLocalForest();
			}
			else
			{
				adforest = ADForest.GetForest(forestFqdn, null);
			}
			if (adforest != null)
			{
				ReadOnlyCollection<ADServer> readOnlyCollection = adforest.FindAllGlobalCatalogs();
				foreach (ADServer adserver in readOnlyCollection)
				{
					if (adserver.IsAvailable())
					{
						return adserver.ServerReference.Name;
					}
				}
			}
			return null;
		}

		private const string AccessSSlFlagsPropertyName = "AccessSSLFlags";

		private const uint AccessSSlFlagValue = 8U;
	}
}
