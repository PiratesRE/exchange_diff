using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Storage.ServerLocator;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Net;
using www.outlook.com.highavailability.ServerLocator.v1;

namespace Microsoft.Exchange.HttpProxy.Routing.Providers
{
	internal class ActiveCopiesCacheProvider : IDatabaseLocationProvider, IDisposable
	{
		public ActiveCopiesCacheProvider()
		{
			this.activeCopiesList = new List<DatabaseServerInformation>();
			this.serverLocator = ServerLocatorServiceClient.Create("localhost");
			this.backgroundRefresh = new Timer((double)ActiveCopiesCacheProvider.DataRefreshIntervalInMilliseconds.Value);
			this.backgroundRefresh.Elapsed += this.OnBackgroundRefresh;
			this.backgroundRefresh.Enabled = true;
			this.Synchronize();
		}

		public void Synchronize()
		{
			ExWatson.SendReportOnUnhandledException(delegate()
			{
				lock (ActiveCopiesCacheProvider.synchronizeLock)
				{
					try
					{
						if (!this.serverLocator.IsUsable)
						{
							this.serverLocator.Dispose();
							this.serverLocator = ServerLocatorServiceClient.Create("localhost");
						}
						GetActiveCopiesForDatabaseAvailabilityGroupParameters getActiveCopiesForDatabaseAvailabilityGroupParameters = new GetActiveCopiesForDatabaseAvailabilityGroupParameters();
						getActiveCopiesForDatabaseAvailabilityGroupParameters.CachedData = false;
						this.activeCopiesList = new List<DatabaseServerInformation>(this.serverLocator.GetActiveCopiesForDatabaseAvailabilityGroupExtended(getActiveCopiesForDatabaseAvailabilityGroupParameters));
					}
					catch (ServerLocatorClientTransientException)
					{
					}
				}
			});
		}

		public BackEndServer GetBackEndServerForDatabase(Guid databaseGuid, string domainName, string resourceForest, IRoutingDiagnostics diagnostics)
		{
			DatabaseServerInformation databaseServerInformation = this.activeCopiesList.FirstOrDefault((DatabaseServerInformation info) => info.DatabaseGuid == databaseGuid);
			if (databaseServerInformation != null)
			{
				return new BackEndServer(databaseServerInformation.ServerFqdn, databaseServerInformation.ServerVersion);
			}
			return null;
		}

		public void Dispose()
		{
			if (this.serverLocator != null)
			{
				this.serverLocator.Dispose();
			}
			if (this.backgroundRefresh != null)
			{
				this.backgroundRefresh.Dispose();
			}
		}

		private void OnBackgroundRefresh(object source, ElapsedEventArgs args)
		{
			this.Synchronize();
		}

		internal static readonly IntAppSettingsEntry DataRefreshIntervalInMilliseconds = new IntAppSettingsEntry("RoutingUpdateModule.DataRefreshIntervalInMilliseconds", 15000, ExTraceGlobals.CommonAlgorithmTracer);

		private static object synchronizeLock = new object();

		private Timer backgroundRefresh;

		private ServerLocatorServiceClient serverLocator;

		private List<DatabaseServerInformation> activeCopiesList;
	}
}
