using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Cluster.Replay
{
	public static class ExTraceGlobals
	{
		public static Trace ReplayApiTracer
		{
			get
			{
				if (ExTraceGlobals.replayApiTracer == null)
				{
					ExTraceGlobals.replayApiTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.replayApiTracer;
			}
		}

		public static Trace EseutilWrapperTracer
		{
			get
			{
				if (ExTraceGlobals.eseutilWrapperTracer == null)
				{
					ExTraceGlobals.eseutilWrapperTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.eseutilWrapperTracer;
			}
		}

		public static Trace StateTracer
		{
			get
			{
				if (ExTraceGlobals.stateTracer == null)
				{
					ExTraceGlobals.stateTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.stateTracer;
			}
		}

		public static Trace LogReplayerTracer
		{
			get
			{
				if (ExTraceGlobals.logReplayerTracer == null)
				{
					ExTraceGlobals.logReplayerTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.logReplayerTracer;
			}
		}

		public static Trace ReplicaInstanceTracer
		{
			get
			{
				if (ExTraceGlobals.replicaInstanceTracer == null)
				{
					ExTraceGlobals.replicaInstanceTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.replicaInstanceTracer;
			}
		}

		public static Trace CmdletsTracer
		{
			get
			{
				if (ExTraceGlobals.cmdletsTracer == null)
				{
					ExTraceGlobals.cmdletsTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.cmdletsTracer;
			}
		}

		public static Trace ShipLogTracer
		{
			get
			{
				if (ExTraceGlobals.shipLogTracer == null)
				{
					ExTraceGlobals.shipLogTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.shipLogTracer;
			}
		}

		public static Trace LogCopyTracer
		{
			get
			{
				if (ExTraceGlobals.logCopyTracer == null)
				{
					ExTraceGlobals.logCopyTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.logCopyTracer;
			}
		}

		public static Trace LogInspectorTracer
		{
			get
			{
				if (ExTraceGlobals.logInspectorTracer == null)
				{
					ExTraceGlobals.logInspectorTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.logInspectorTracer;
			}
		}

		public static Trace ReplayManagerTracer
		{
			get
			{
				if (ExTraceGlobals.replayManagerTracer == null)
				{
					ExTraceGlobals.replayManagerTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.replayManagerTracer;
			}
		}

		public static Trace CReplicaSeederTracer
		{
			get
			{
				if (ExTraceGlobals.cReplicaSeederTracer == null)
				{
					ExTraceGlobals.cReplicaSeederTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.cReplicaSeederTracer;
			}
		}

		public static Trace NetShareTracer
		{
			get
			{
				if (ExTraceGlobals.netShareTracer == null)
				{
					ExTraceGlobals.netShareTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.netShareTracer;
			}
		}

		public static Trace ReplicaVssWriterInteropTracer
		{
			get
			{
				if (ExTraceGlobals.replicaVssWriterInteropTracer == null)
				{
					ExTraceGlobals.replicaVssWriterInteropTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.replicaVssWriterInteropTracer;
			}
		}

		public static Trace StateLockTracer
		{
			get
			{
				if (ExTraceGlobals.stateLockTracer == null)
				{
					ExTraceGlobals.stateLockTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.stateLockTracer;
			}
		}

		public static Trace FileCheckerTracer
		{
			get
			{
				if (ExTraceGlobals.fileCheckerTracer == null)
				{
					ExTraceGlobals.fileCheckerTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.fileCheckerTracer;
			}
		}

		public static Trace ClusterTracer
		{
			get
			{
				if (ExTraceGlobals.clusterTracer == null)
				{
					ExTraceGlobals.clusterTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.clusterTracer;
			}
		}

		public static Trace SeederWrapperTracer
		{
			get
			{
				if (ExTraceGlobals.seederWrapperTracer == null)
				{
					ExTraceGlobals.seederWrapperTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.seederWrapperTracer;
			}
		}

		public static Trace PFDTracer
		{
			get
			{
				if (ExTraceGlobals.pFDTracer == null)
				{
					ExTraceGlobals.pFDTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.pFDTracer;
			}
		}

		public static Trace IncrementalReseederTracer
		{
			get
			{
				if (ExTraceGlobals.incrementalReseederTracer == null)
				{
					ExTraceGlobals.incrementalReseederTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.incrementalReseederTracer;
			}
		}

		public static Trace DumpsterTracer
		{
			get
			{
				if (ExTraceGlobals.dumpsterTracer == null)
				{
					ExTraceGlobals.dumpsterTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.dumpsterTracer;
			}
		}

		public static Trace CLogShipContextTracer
		{
			get
			{
				if (ExTraceGlobals.cLogShipContextTracer == null)
				{
					ExTraceGlobals.cLogShipContextTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.cLogShipContextTracer;
			}
		}

		public static Trace ClusDBWriteTracer
		{
			get
			{
				if (ExTraceGlobals.clusDBWriteTracer == null)
				{
					ExTraceGlobals.clusDBWriteTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.clusDBWriteTracer;
			}
		}

		public static Trace ReplayConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.replayConfigurationTracer == null)
				{
					ExTraceGlobals.replayConfigurationTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.replayConfigurationTracer;
			}
		}

		public static Trace NetPathTracer
		{
			get
			{
				if (ExTraceGlobals.netPathTracer == null)
				{
					ExTraceGlobals.netPathTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.netPathTracer;
			}
		}

		public static Trace HealthChecksTracer
		{
			get
			{
				if (ExTraceGlobals.healthChecksTracer == null)
				{
					ExTraceGlobals.healthChecksTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.healthChecksTracer;
			}
		}

		public static Trace ReplayServiceRpcTracer
		{
			get
			{
				if (ExTraceGlobals.replayServiceRpcTracer == null)
				{
					ExTraceGlobals.replayServiceRpcTracer = new Trace(ExTraceGlobals.componentGuid, 25);
				}
				return ExTraceGlobals.replayServiceRpcTracer;
			}
		}

		public static Trace ActiveManagerTracer
		{
			get
			{
				if (ExTraceGlobals.activeManagerTracer == null)
				{
					ExTraceGlobals.activeManagerTracer = new Trace(ExTraceGlobals.componentGuid, 26);
				}
				return ExTraceGlobals.activeManagerTracer;
			}
		}

		public static Trace SeederServerTracer
		{
			get
			{
				if (ExTraceGlobals.seederServerTracer == null)
				{
					ExTraceGlobals.seederServerTracer = new Trace(ExTraceGlobals.componentGuid, 27);
				}
				return ExTraceGlobals.seederServerTracer;
			}
		}

		public static Trace SeederClientTracer
		{
			get
			{
				if (ExTraceGlobals.seederClientTracer == null)
				{
					ExTraceGlobals.seederClientTracer = new Trace(ExTraceGlobals.componentGuid, 28);
				}
				return ExTraceGlobals.seederClientTracer;
			}
		}

		public static Trace LogTruncaterTracer
		{
			get
			{
				if (ExTraceGlobals.logTruncaterTracer == null)
				{
					ExTraceGlobals.logTruncaterTracer = new Trace(ExTraceGlobals.componentGuid, 29);
				}
				return ExTraceGlobals.logTruncaterTracer;
			}
		}

		public static Trace FailureItemTracer
		{
			get
			{
				if (ExTraceGlobals.failureItemTracer == null)
				{
					ExTraceGlobals.failureItemTracer = new Trace(ExTraceGlobals.componentGuid, 30);
				}
				return ExTraceGlobals.failureItemTracer;
			}
		}

		public static Trace LogCopyServerTracer
		{
			get
			{
				if (ExTraceGlobals.logCopyServerTracer == null)
				{
					ExTraceGlobals.logCopyServerTracer = new Trace(ExTraceGlobals.componentGuid, 31);
				}
				return ExTraceGlobals.logCopyServerTracer;
			}
		}

		public static Trace LogCopyClientTracer
		{
			get
			{
				if (ExTraceGlobals.logCopyClientTracer == null)
				{
					ExTraceGlobals.logCopyClientTracer = new Trace(ExTraceGlobals.componentGuid, 32);
				}
				return ExTraceGlobals.logCopyClientTracer;
			}
		}

		public static Trace TcpChannelTracer
		{
			get
			{
				if (ExTraceGlobals.tcpChannelTracer == null)
				{
					ExTraceGlobals.tcpChannelTracer = new Trace(ExTraceGlobals.componentGuid, 33);
				}
				return ExTraceGlobals.tcpChannelTracer;
			}
		}

		public static Trace TcpClientTracer
		{
			get
			{
				if (ExTraceGlobals.tcpClientTracer == null)
				{
					ExTraceGlobals.tcpClientTracer = new Trace(ExTraceGlobals.componentGuid, 34);
				}
				return ExTraceGlobals.tcpClientTracer;
			}
		}

		public static Trace TcpServerTracer
		{
			get
			{
				if (ExTraceGlobals.tcpServerTracer == null)
				{
					ExTraceGlobals.tcpServerTracer = new Trace(ExTraceGlobals.componentGuid, 35);
				}
				return ExTraceGlobals.tcpServerTracer;
			}
		}

		public static Trace RemoteDataProviderTracer
		{
			get
			{
				if (ExTraceGlobals.remoteDataProviderTracer == null)
				{
					ExTraceGlobals.remoteDataProviderTracer = new Trace(ExTraceGlobals.componentGuid, 36);
				}
				return ExTraceGlobals.remoteDataProviderTracer;
			}
		}

		public static Trace MonitoredDatabaseTracer
		{
			get
			{
				if (ExTraceGlobals.monitoredDatabaseTracer == null)
				{
					ExTraceGlobals.monitoredDatabaseTracer = new Trace(ExTraceGlobals.componentGuid, 37);
				}
				return ExTraceGlobals.monitoredDatabaseTracer;
			}
		}

		public static Trace NetworkManagerTracer
		{
			get
			{
				if (ExTraceGlobals.networkManagerTracer == null)
				{
					ExTraceGlobals.networkManagerTracer = new Trace(ExTraceGlobals.componentGuid, 38);
				}
				return ExTraceGlobals.networkManagerTracer;
			}
		}

		public static Trace NetworkChannelTracer
		{
			get
			{
				if (ExTraceGlobals.networkChannelTracer == null)
				{
					ExTraceGlobals.networkChannelTracer = new Trace(ExTraceGlobals.componentGuid, 39);
				}
				return ExTraceGlobals.networkChannelTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 40);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace GranularWriterTracer
		{
			get
			{
				if (ExTraceGlobals.granularWriterTracer == null)
				{
					ExTraceGlobals.granularWriterTracer = new Trace(ExTraceGlobals.componentGuid, 41);
				}
				return ExTraceGlobals.granularWriterTracer;
			}
		}

		public static Trace GranularReaderTracer
		{
			get
			{
				if (ExTraceGlobals.granularReaderTracer == null)
				{
					ExTraceGlobals.granularReaderTracer = new Trace(ExTraceGlobals.componentGuid, 42);
				}
				return ExTraceGlobals.granularReaderTracer;
			}
		}

		public static Trace ThirdPartyClientTracer
		{
			get
			{
				if (ExTraceGlobals.thirdPartyClientTracer == null)
				{
					ExTraceGlobals.thirdPartyClientTracer = new Trace(ExTraceGlobals.componentGuid, 43);
				}
				return ExTraceGlobals.thirdPartyClientTracer;
			}
		}

		public static Trace ThirdPartyManagerTracer
		{
			get
			{
				if (ExTraceGlobals.thirdPartyManagerTracer == null)
				{
					ExTraceGlobals.thirdPartyManagerTracer = new Trace(ExTraceGlobals.componentGuid, 44);
				}
				return ExTraceGlobals.thirdPartyManagerTracer;
			}
		}

		public static Trace ThirdPartyServiceTracer
		{
			get
			{
				if (ExTraceGlobals.thirdPartyServiceTracer == null)
				{
					ExTraceGlobals.thirdPartyServiceTracer = new Trace(ExTraceGlobals.componentGuid, 45);
				}
				return ExTraceGlobals.thirdPartyServiceTracer;
			}
		}

		public static Trace ClusterEventsTracer
		{
			get
			{
				if (ExTraceGlobals.clusterEventsTracer == null)
				{
					ExTraceGlobals.clusterEventsTracer = new Trace(ExTraceGlobals.componentGuid, 46);
				}
				return ExTraceGlobals.clusterEventsTracer;
			}
		}

		public static Trace AmNetworkMonitorTracer
		{
			get
			{
				if (ExTraceGlobals.amNetworkMonitorTracer == null)
				{
					ExTraceGlobals.amNetworkMonitorTracer = new Trace(ExTraceGlobals.componentGuid, 47);
				}
				return ExTraceGlobals.amNetworkMonitorTracer;
			}
		}

		public static Trace AmConfigManagerTracer
		{
			get
			{
				if (ExTraceGlobals.amConfigManagerTracer == null)
				{
					ExTraceGlobals.amConfigManagerTracer = new Trace(ExTraceGlobals.componentGuid, 48);
				}
				return ExTraceGlobals.amConfigManagerTracer;
			}
		}

		public static Trace AmSystemManagerTracer
		{
			get
			{
				if (ExTraceGlobals.amSystemManagerTracer == null)
				{
					ExTraceGlobals.amSystemManagerTracer = new Trace(ExTraceGlobals.componentGuid, 49);
				}
				return ExTraceGlobals.amSystemManagerTracer;
			}
		}

		public static Trace AmServiceMonitorTracer
		{
			get
			{
				if (ExTraceGlobals.amServiceMonitorTracer == null)
				{
					ExTraceGlobals.amServiceMonitorTracer = new Trace(ExTraceGlobals.componentGuid, 50);
				}
				return ExTraceGlobals.amServiceMonitorTracer;
			}
		}

		public static Trace ServiceOperationsTracer
		{
			get
			{
				if (ExTraceGlobals.serviceOperationsTracer == null)
				{
					ExTraceGlobals.serviceOperationsTracer = new Trace(ExTraceGlobals.componentGuid, 51);
				}
				return ExTraceGlobals.serviceOperationsTracer;
			}
		}

		public static Trace AmServerNameCacheTracer
		{
			get
			{
				if (ExTraceGlobals.amServerNameCacheTracer == null)
				{
					ExTraceGlobals.amServerNameCacheTracer = new Trace(ExTraceGlobals.componentGuid, 53);
				}
				return ExTraceGlobals.amServerNameCacheTracer;
			}
		}

		public static Trace KernelWatchdogTimerTracer
		{
			get
			{
				if (ExTraceGlobals.kernelWatchdogTimerTracer == null)
				{
					ExTraceGlobals.kernelWatchdogTimerTracer = new Trace(ExTraceGlobals.componentGuid, 54);
				}
				return ExTraceGlobals.kernelWatchdogTimerTracer;
			}
		}

		public static Trace FailureItemHealthMonitorTracer
		{
			get
			{
				if (ExTraceGlobals.failureItemHealthMonitorTracer == null)
				{
					ExTraceGlobals.failureItemHealthMonitorTracer = new Trace(ExTraceGlobals.componentGuid, 55);
				}
				return ExTraceGlobals.failureItemHealthMonitorTracer;
			}
		}

		public static Trace ReplayServiceDiagnosticsTracer
		{
			get
			{
				if (ExTraceGlobals.replayServiceDiagnosticsTracer == null)
				{
					ExTraceGlobals.replayServiceDiagnosticsTracer = new Trace(ExTraceGlobals.componentGuid, 56);
				}
				return ExTraceGlobals.replayServiceDiagnosticsTracer;
			}
		}

		public static Trace LogRepairTracer
		{
			get
			{
				if (ExTraceGlobals.logRepairTracer == null)
				{
					ExTraceGlobals.logRepairTracer = new Trace(ExTraceGlobals.componentGuid, 57);
				}
				return ExTraceGlobals.logRepairTracer;
			}
		}

		public static Trace PassiveBlockModeTracer
		{
			get
			{
				if (ExTraceGlobals.passiveBlockModeTracer == null)
				{
					ExTraceGlobals.passiveBlockModeTracer = new Trace(ExTraceGlobals.componentGuid, 58);
				}
				return ExTraceGlobals.passiveBlockModeTracer;
			}
		}

		public static Trace LogCopierTracer
		{
			get
			{
				if (ExTraceGlobals.logCopierTracer == null)
				{
					ExTraceGlobals.logCopierTracer = new Trace(ExTraceGlobals.componentGuid, 59);
				}
				return ExTraceGlobals.logCopierTracer;
			}
		}

		public static Trace DiskHeartbeatTracer
		{
			get
			{
				if (ExTraceGlobals.diskHeartbeatTracer == null)
				{
					ExTraceGlobals.diskHeartbeatTracer = new Trace(ExTraceGlobals.componentGuid, 60);
				}
				return ExTraceGlobals.diskHeartbeatTracer;
			}
		}

		public static Trace MonitoringTracer
		{
			get
			{
				if (ExTraceGlobals.monitoringTracer == null)
				{
					ExTraceGlobals.monitoringTracer = new Trace(ExTraceGlobals.componentGuid, 61);
				}
				return ExTraceGlobals.monitoringTracer;
			}
		}

		public static Trace ServerLocatorServiceTracer
		{
			get
			{
				if (ExTraceGlobals.serverLocatorServiceTracer == null)
				{
					ExTraceGlobals.serverLocatorServiceTracer = new Trace(ExTraceGlobals.componentGuid, 62);
				}
				return ExTraceGlobals.serverLocatorServiceTracer;
			}
		}

		public static Trace ServerLocatorServiceClientTracer
		{
			get
			{
				if (ExTraceGlobals.serverLocatorServiceClientTracer == null)
				{
					ExTraceGlobals.serverLocatorServiceClientTracer = new Trace(ExTraceGlobals.componentGuid, 63);
				}
				return ExTraceGlobals.serverLocatorServiceClientTracer;
			}
		}

		public static Trace LatencyCheckerTracer
		{
			get
			{
				if (ExTraceGlobals.latencyCheckerTracer == null)
				{
					ExTraceGlobals.latencyCheckerTracer = new Trace(ExTraceGlobals.componentGuid, 64);
				}
				return ExTraceGlobals.latencyCheckerTracer;
			}
		}

		public static Trace VolumeManagerTracer
		{
			get
			{
				if (ExTraceGlobals.volumeManagerTracer == null)
				{
					ExTraceGlobals.volumeManagerTracer = new Trace(ExTraceGlobals.componentGuid, 65);
				}
				return ExTraceGlobals.volumeManagerTracer;
			}
		}

		public static Trace AutoReseedTracer
		{
			get
			{
				if (ExTraceGlobals.autoReseedTracer == null)
				{
					ExTraceGlobals.autoReseedTracer = new Trace(ExTraceGlobals.componentGuid, 66);
				}
				return ExTraceGlobals.autoReseedTracer;
			}
		}

		public static Trace DiskReclaimerTracer
		{
			get
			{
				if (ExTraceGlobals.diskReclaimerTracer == null)
				{
					ExTraceGlobals.diskReclaimerTracer = new Trace(ExTraceGlobals.componentGuid, 67);
				}
				return ExTraceGlobals.diskReclaimerTracer;
			}
		}

		public static Trace ADCacheTracer
		{
			get
			{
				if (ExTraceGlobals.aDCacheTracer == null)
				{
					ExTraceGlobals.aDCacheTracer = new Trace(ExTraceGlobals.componentGuid, 68);
				}
				return ExTraceGlobals.aDCacheTracer;
			}
		}

		public static Trace DbTrackerTracer
		{
			get
			{
				if (ExTraceGlobals.dbTrackerTracer == null)
				{
					ExTraceGlobals.dbTrackerTracer = new Trace(ExTraceGlobals.componentGuid, 69);
				}
				return ExTraceGlobals.dbTrackerTracer;
			}
		}

		public static Trace DatabaseCopyLayoutTracer
		{
			get
			{
				if (ExTraceGlobals.databaseCopyLayoutTracer == null)
				{
					ExTraceGlobals.databaseCopyLayoutTracer = new Trace(ExTraceGlobals.componentGuid, 70);
				}
				return ExTraceGlobals.databaseCopyLayoutTracer;
			}
		}

		public static Trace CompositeKeyTracer
		{
			get
			{
				if (ExTraceGlobals.compositeKeyTracer == null)
				{
					ExTraceGlobals.compositeKeyTracer = new Trace(ExTraceGlobals.componentGuid, 71);
				}
				return ExTraceGlobals.compositeKeyTracer;
			}
		}

		public static Trace ClusdbKeyTracer
		{
			get
			{
				if (ExTraceGlobals.clusdbKeyTracer == null)
				{
					ExTraceGlobals.clusdbKeyTracer = new Trace(ExTraceGlobals.componentGuid, 72);
				}
				return ExTraceGlobals.clusdbKeyTracer;
			}
		}

		public static Trace DxStoreKeyTracer
		{
			get
			{
				if (ExTraceGlobals.dxStoreKeyTracer == null)
				{
					ExTraceGlobals.dxStoreKeyTracer = new Trace(ExTraceGlobals.componentGuid, 73);
				}
				return ExTraceGlobals.dxStoreKeyTracer;
			}
		}

		private static Guid componentGuid = new Guid("404a3308-17e1-4ac3-9167-1b09c36850bd");

		private static Trace replayApiTracer = null;

		private static Trace eseutilWrapperTracer = null;

		private static Trace stateTracer = null;

		private static Trace logReplayerTracer = null;

		private static Trace replicaInstanceTracer = null;

		private static Trace cmdletsTracer = null;

		private static Trace shipLogTracer = null;

		private static Trace logCopyTracer = null;

		private static Trace logInspectorTracer = null;

		private static Trace replayManagerTracer = null;

		private static Trace cReplicaSeederTracer = null;

		private static Trace netShareTracer = null;

		private static Trace replicaVssWriterInteropTracer = null;

		private static Trace stateLockTracer = null;

		private static Trace fileCheckerTracer = null;

		private static Trace clusterTracer = null;

		private static Trace seederWrapperTracer = null;

		private static Trace pFDTracer = null;

		private static Trace incrementalReseederTracer = null;

		private static Trace dumpsterTracer = null;

		private static Trace cLogShipContextTracer = null;

		private static Trace clusDBWriteTracer = null;

		private static Trace replayConfigurationTracer = null;

		private static Trace netPathTracer = null;

		private static Trace healthChecksTracer = null;

		private static Trace replayServiceRpcTracer = null;

		private static Trace activeManagerTracer = null;

		private static Trace seederServerTracer = null;

		private static Trace seederClientTracer = null;

		private static Trace logTruncaterTracer = null;

		private static Trace failureItemTracer = null;

		private static Trace logCopyServerTracer = null;

		private static Trace logCopyClientTracer = null;

		private static Trace tcpChannelTracer = null;

		private static Trace tcpClientTracer = null;

		private static Trace tcpServerTracer = null;

		private static Trace remoteDataProviderTracer = null;

		private static Trace monitoredDatabaseTracer = null;

		private static Trace networkManagerTracer = null;

		private static Trace networkChannelTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace granularWriterTracer = null;

		private static Trace granularReaderTracer = null;

		private static Trace thirdPartyClientTracer = null;

		private static Trace thirdPartyManagerTracer = null;

		private static Trace thirdPartyServiceTracer = null;

		private static Trace clusterEventsTracer = null;

		private static Trace amNetworkMonitorTracer = null;

		private static Trace amConfigManagerTracer = null;

		private static Trace amSystemManagerTracer = null;

		private static Trace amServiceMonitorTracer = null;

		private static Trace serviceOperationsTracer = null;

		private static Trace amServerNameCacheTracer = null;

		private static Trace kernelWatchdogTimerTracer = null;

		private static Trace failureItemHealthMonitorTracer = null;

		private static Trace replayServiceDiagnosticsTracer = null;

		private static Trace logRepairTracer = null;

		private static Trace passiveBlockModeTracer = null;

		private static Trace logCopierTracer = null;

		private static Trace diskHeartbeatTracer = null;

		private static Trace monitoringTracer = null;

		private static Trace serverLocatorServiceTracer = null;

		private static Trace serverLocatorServiceClientTracer = null;

		private static Trace latencyCheckerTracer = null;

		private static Trace volumeManagerTracer = null;

		private static Trace autoReseedTracer = null;

		private static Trace diskReclaimerTracer = null;

		private static Trace aDCacheTracer = null;

		private static Trace dbTrackerTracer = null;

		private static Trace databaseCopyLayoutTracer = null;

		private static Trace compositeKeyTracer = null;

		private static Trace clusdbKeyTracer = null;

		private static Trace dxStoreKeyTracer = null;
	}
}
