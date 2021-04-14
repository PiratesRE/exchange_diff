using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class BridgeheadPicker : ServerPickerBase<InternalExchangeServer, object>
	{
		private InternalExchangeServer LocalServer
		{
			get
			{
				if (this.localServer == null)
				{
					lock (this.localServerLock)
					{
						if (this.localServer == null)
						{
							this.localServer = this.GetLocalServer();
						}
					}
				}
				return this.localServer;
			}
		}

		internal BridgeheadPicker() : base(ServerPickerBase<InternalExchangeServer, object>.DefaultRetryInterval, ServerPickerBase<InternalExchangeServer, object>.DefaultRefreshInterval, ServerPickerBase<InternalExchangeServer, object>.DefaultRefreshIntervalOnFailure, ExTraceGlobals.VoiceMailTracer)
		{
		}

		public override InternalExchangeServer PickNextServer(object context)
		{
			InternalExchangeServer internalExchangeServer = base.PickNextServer(context);
			if (!base.ServerInRetry(this.LocalServer))
			{
				CallIdTracer.TraceDebug(base.Tracer, 0, "BridgeheadServerPicker::PickNextServer() local server not in retry. Using it.", new object[0]);
				internalExchangeServer = this.LocalServer;
			}
			CallIdTracer.TraceDebug(base.Tracer, 0, "BridgeheadServerPicker::PickNextServer() returning  {0}", new object[]
			{
				(internalExchangeServer != null) ? internalExchangeServer.Fqdn : "<null>"
			});
			return internalExchangeServer;
		}

		protected override List<InternalExchangeServer> LoadConfiguration()
		{
			CallIdTracer.TraceDebug(base.Tracer, 0, "BridgeheadServerPicker::LoadConfiguration() called", new object[0]);
			List<InternalExchangeServer> list = new List<InternalExchangeServer>(4);
			IEnumerable<Server> enumerable;
			if (this.LocalServer.Server.DatabaseAvailabilityGroup == null)
			{
				CallIdTracer.TraceDebug(base.Tracer, 0, "BridgeheadServerPicker: No DAG configured, adding servers in the site", new object[0]);
				enumerable = this.GetLocalSiteHubServers();
			}
			else
			{
				CallIdTracer.TraceDebug(base.Tracer, 0, "BridgeheadServerPicker: DAG configured, adding servers in the DAG", new object[0]);
				enumerable = this.GetDagHubServers();
			}
			foreach (Server s in enumerable)
			{
				InternalExchangeServer internalExchangeServer = new InternalExchangeServer(s);
				CallIdTracer.TraceDebug(base.Tracer, 0, "BridgeheadServerPicker: Adding Hub server {0} to active list", new object[]
				{
					internalExchangeServer.Fqdn
				});
				list.Add(internalExchangeServer);
			}
			CallIdTracer.TraceDebug(base.Tracer, 0, "BridgeheadServerPicker::LoadConfiguration() Finished refreshing hub servers", new object[0]);
			return list;
		}

		protected virtual InternalExchangeServer GetLocalServer()
		{
			return new InternalExchangeServer(ADTopologyLookup.CreateLocalResourceForestLookup().GetLocalServer());
		}

		protected virtual IEnumerable<Server> GetLocalSiteHubServers()
		{
			return ADTopologyLookup.CreateLocalResourceForestLookup().GetEnabledExchangeServers(VersionEnum.Compatible, ServerRole.HubTransport, this.LocalServer.ServerSite);
		}

		protected virtual IEnumerable<Server> GetDagHubServers()
		{
			return ADTopologyLookup.CreateLocalResourceForestLookup().GetDatabaseAvailabilityGroupServers(VersionEnum.Compatible, ServerRole.HubTransport, this.LocalServer.Server.DatabaseAvailabilityGroup);
		}

		private InternalExchangeServer localServer;

		private object localServerLock = new object();
	}
}
