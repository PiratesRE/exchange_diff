using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.Cluster.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SimpleMiniServerLookup : IFindMiniServer
	{
		public SimpleMiniServerLookup(IADToplogyConfigurationSession adSession)
		{
			this.AdSession = adSession;
		}

		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ActiveManagerClientTracer;
			}
		}

		public IADToplogyConfigurationSession AdSession { get; set; }

		public void Clear()
		{
		}

		public IADServer FindMiniServerByFqdn(string serverFqdn)
		{
			string nodeNameFromFqdn = MachineName.GetNodeNameFromFqdn(serverFqdn);
			return this.FindMiniServerByShortName(nodeNameFromFqdn);
		}

		public IADServer FindMiniServerByShortName(string shortName)
		{
			Exception ex;
			return this.FindMiniServerByShortNameEx(shortName, out ex);
		}

		public IADServer FindMiniServerByShortNameEx(string shortName, out Exception ex)
		{
			IADServer result = null;
			ex = ADUtils.RunADOperation(delegate()
			{
				result = this.AdSession.FindMiniServerByName(shortName);
			}, 2);
			if (ex != null)
			{
				SimpleMiniServerLookup.Tracer.TraceDebug<Exception>((long)this.GetHashCode(), "SimpleMiniServerLookup.FindMiniServerByFqdn got an exception: {0}", ex);
			}
			return result;
		}

		public IADServer ReadMiniServerByObjectId(ADObjectId serverId)
		{
			IADServer result = null;
			Exception ex = ADUtils.RunADOperation(delegate()
			{
				result = this.AdSession.ReadMiniServer(serverId);
			}, 2);
			if (ex != null)
			{
				SimpleMiniServerLookup.Tracer.TraceDebug<Exception>((long)this.GetHashCode(), "SimpleMiniServerLookup.ReadMiniServerByObjectId got an exception: {0}", ex);
			}
			return result;
		}
	}
}
