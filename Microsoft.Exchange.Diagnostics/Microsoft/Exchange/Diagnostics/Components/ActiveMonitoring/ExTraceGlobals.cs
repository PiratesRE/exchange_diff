using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring
{
	public static class ExTraceGlobals
	{
		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace AzureTracer
		{
			get
			{
				if (ExTraceGlobals.azureTracer == null)
				{
					ExTraceGlobals.azureTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.azureTracer;
			}
		}

		public static Trace CommonComponentsTracer
		{
			get
			{
				if (ExTraceGlobals.commonComponentsTracer == null)
				{
					ExTraceGlobals.commonComponentsTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.commonComponentsTracer;
			}
		}

		public static Trace HeartbeatTracer
		{
			get
			{
				if (ExTraceGlobals.heartbeatTracer == null)
				{
					ExTraceGlobals.heartbeatTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.heartbeatTracer;
			}
		}

		public static Trace HTTPTracer
		{
			get
			{
				if (ExTraceGlobals.hTTPTracer == null)
				{
					ExTraceGlobals.hTTPTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.hTTPTracer;
			}
		}

		public static Trace OWATracer
		{
			get
			{
				if (ExTraceGlobals.oWATracer == null)
				{
					ExTraceGlobals.oWATracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.oWATracer;
			}
		}

		public static Trace RPCHTTPTracer
		{
			get
			{
				if (ExTraceGlobals.rPCHTTPTracer == null)
				{
					ExTraceGlobals.rPCHTTPTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.rPCHTTPTracer;
			}
		}

		public static Trace ActiveSyncTracer
		{
			get
			{
				if (ExTraceGlobals.activeSyncTracer == null)
				{
					ExTraceGlobals.activeSyncTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.activeSyncTracer;
			}
		}

		public static Trace EWSTracer
		{
			get
			{
				if (ExTraceGlobals.eWSTracer == null)
				{
					ExTraceGlobals.eWSTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.eWSTracer;
			}
		}

		public static Trace AutoDiscoverTracer
		{
			get
			{
				if (ExTraceGlobals.autoDiscoverTracer == null)
				{
					ExTraceGlobals.autoDiscoverTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.autoDiscoverTracer;
			}
		}

		public static Trace LiveIdTracer
		{
			get
			{
				if (ExTraceGlobals.liveIdTracer == null)
				{
					ExTraceGlobals.liveIdTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.liveIdTracer;
			}
		}

		public static Trace RIMTracer
		{
			get
			{
				if (ExTraceGlobals.rIMTracer == null)
				{
					ExTraceGlobals.rIMTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.rIMTracer;
			}
		}

		public static Trace ServiceTracer
		{
			get
			{
				if (ExTraceGlobals.serviceTracer == null)
				{
					ExTraceGlobals.serviceTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.serviceTracer;
			}
		}

		public static Trace WorkerTracer
		{
			get
			{
				if (ExTraceGlobals.workerTracer == null)
				{
					ExTraceGlobals.workerTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.workerTracer;
			}
		}

		public static Trace OABTracer
		{
			get
			{
				if (ExTraceGlobals.oABTracer == null)
				{
					ExTraceGlobals.oABTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.oABTracer;
			}
		}

		public static Trace StoreTracer
		{
			get
			{
				if (ExTraceGlobals.storeTracer == null)
				{
					ExTraceGlobals.storeTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.storeTracer;
			}
		}

		public static Trace AvailabilityServiceTracer
		{
			get
			{
				if (ExTraceGlobals.availabilityServiceTracer == null)
				{
					ExTraceGlobals.availabilityServiceTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.availabilityServiceTracer;
			}
		}

		public static Trace NetworkTracer
		{
			get
			{
				if (ExTraceGlobals.networkTracer == null)
				{
					ExTraceGlobals.networkTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.networkTracer;
			}
		}

		public static Trace UnifiedMessagingTracer
		{
			get
			{
				if (ExTraceGlobals.unifiedMessagingTracer == null)
				{
					ExTraceGlobals.unifiedMessagingTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.unifiedMessagingTracer;
			}
		}

		public static Trace RPSTracer
		{
			get
			{
				if (ExTraceGlobals.rPSTracer == null)
				{
					ExTraceGlobals.rPSTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.rPSTracer;
			}
		}

		public static Trace OfficeTracer
		{
			get
			{
				if (ExTraceGlobals.officeTracer == null)
				{
					ExTraceGlobals.officeTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.officeTracer;
			}
		}

		public static Trace IMAP4Tracer
		{
			get
			{
				if (ExTraceGlobals.iMAP4Tracer == null)
				{
					ExTraceGlobals.iMAP4Tracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.iMAP4Tracer;
			}
		}

		public static Trace POP3Tracer
		{
			get
			{
				if (ExTraceGlobals.pOP3Tracer == null)
				{
					ExTraceGlobals.pOP3Tracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.pOP3Tracer;
			}
		}

		public static Trace SearchTracer
		{
			get
			{
				if (ExTraceGlobals.searchTracer == null)
				{
					ExTraceGlobals.searchTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.searchTracer;
			}
		}

		public static Trace MigrationTracer
		{
			get
			{
				if (ExTraceGlobals.migrationTracer == null)
				{
					ExTraceGlobals.migrationTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.migrationTracer;
			}
		}

		public static Trace DirectoryTracer
		{
			get
			{
				if (ExTraceGlobals.directoryTracer == null)
				{
					ExTraceGlobals.directoryTracer = new Trace(ExTraceGlobals.componentGuid, 25);
				}
				return ExTraceGlobals.directoryTracer;
			}
		}

		public static Trace HighAvailabilityTracer
		{
			get
			{
				if (ExTraceGlobals.highAvailabilityTracer == null)
				{
					ExTraceGlobals.highAvailabilityTracer = new Trace(ExTraceGlobals.componentGuid, 26);
				}
				return ExTraceGlobals.highAvailabilityTracer;
			}
		}

		public static Trace ProvisioningTracer
		{
			get
			{
				if (ExTraceGlobals.provisioningTracer == null)
				{
					ExTraceGlobals.provisioningTracer = new Trace(ExTraceGlobals.componentGuid, 27);
				}
				return ExTraceGlobals.provisioningTracer;
			}
		}

		public static Trace TransportTracer
		{
			get
			{
				if (ExTraceGlobals.transportTracer == null)
				{
					ExTraceGlobals.transportTracer = new Trace(ExTraceGlobals.componentGuid, 28);
				}
				return ExTraceGlobals.transportTracer;
			}
		}

		public static Trace MonitoringTracer
		{
			get
			{
				if (ExTraceGlobals.monitoringTracer == null)
				{
					ExTraceGlobals.monitoringTracer = new Trace(ExTraceGlobals.componentGuid, 29);
				}
				return ExTraceGlobals.monitoringTracer;
			}
		}

		public static Trace CalendarSharingTracer
		{
			get
			{
				if (ExTraceGlobals.calendarSharingTracer == null)
				{
					ExTraceGlobals.calendarSharingTracer = new Trace(ExTraceGlobals.componentGuid, 30);
				}
				return ExTraceGlobals.calendarSharingTracer;
			}
		}

		public static Trace CafeTracer
		{
			get
			{
				if (ExTraceGlobals.cafeTracer == null)
				{
					ExTraceGlobals.cafeTracer = new Trace(ExTraceGlobals.componentGuid, 31);
				}
				return ExTraceGlobals.cafeTracer;
			}
		}

		public static Trace EventAssistantsTracer
		{
			get
			{
				if (ExTraceGlobals.eventAssistantsTracer == null)
				{
					ExTraceGlobals.eventAssistantsTracer = new Trace(ExTraceGlobals.componentGuid, 32);
				}
				return ExTraceGlobals.eventAssistantsTracer;
			}
		}

		public static Trace FIPSTracer
		{
			get
			{
				if (ExTraceGlobals.fIPSTracer == null)
				{
					ExTraceGlobals.fIPSTracer = new Trace(ExTraceGlobals.componentGuid, 33);
				}
				return ExTraceGlobals.fIPSTracer;
			}
		}

		public static Trace AntimalwareTracer
		{
			get
			{
				if (ExTraceGlobals.antimalwareTracer == null)
				{
					ExTraceGlobals.antimalwareTracer = new Trace(ExTraceGlobals.componentGuid, 34);
				}
				return ExTraceGlobals.antimalwareTracer;
			}
		}

		public static Trace TransportSyncTracer
		{
			get
			{
				if (ExTraceGlobals.transportSyncTracer == null)
				{
					ExTraceGlobals.transportSyncTracer = new Trace(ExTraceGlobals.componentGuid, 35);
				}
				return ExTraceGlobals.transportSyncTracer;
			}
		}

		public static Trace ECPTracer
		{
			get
			{
				if (ExTraceGlobals.eCPTracer == null)
				{
					ExTraceGlobals.eCPTracer = new Trace(ExTraceGlobals.componentGuid, 36);
				}
				return ExTraceGlobals.eCPTracer;
			}
		}

		public static Trace SecurityTracer
		{
			get
			{
				if (ExTraceGlobals.securityTracer == null)
				{
					ExTraceGlobals.securityTracer = new Trace(ExTraceGlobals.componentGuid, 37);
				}
				return ExTraceGlobals.securityTracer;
			}
		}

		public static Trace RWSTracer
		{
			get
			{
				if (ExTraceGlobals.rWSTracer == null)
				{
					ExTraceGlobals.rWSTracer = new Trace(ExTraceGlobals.componentGuid, 38);
				}
				return ExTraceGlobals.rWSTracer;
			}
		}

		public static Trace EDSTracer
		{
			get
			{
				if (ExTraceGlobals.eDSTracer == null)
				{
					ExTraceGlobals.eDSTracer = new Trace(ExTraceGlobals.componentGuid, 39);
				}
				return ExTraceGlobals.eDSTracer;
			}
		}

		public static Trace ProcessIsolationTracer
		{
			get
			{
				if (ExTraceGlobals.processIsolationTracer == null)
				{
					ExTraceGlobals.processIsolationTracer = new Trace(ExTraceGlobals.componentGuid, 40);
				}
				return ExTraceGlobals.processIsolationTracer;
			}
		}

		public static Trace ActiveMonitoringRpcTracer
		{
			get
			{
				if (ExTraceGlobals.activeMonitoringRpcTracer == null)
				{
					ExTraceGlobals.activeMonitoringRpcTracer = new Trace(ExTraceGlobals.componentGuid, 41);
				}
				return ExTraceGlobals.activeMonitoringRpcTracer;
			}
		}

		public static Trace RecoveryActionTracer
		{
			get
			{
				if (ExTraceGlobals.recoveryActionTracer == null)
				{
					ExTraceGlobals.recoveryActionTracer = new Trace(ExTraceGlobals.componentGuid, 42);
				}
				return ExTraceGlobals.recoveryActionTracer;
			}
		}

		public static Trace GenericRpcTracer
		{
			get
			{
				if (ExTraceGlobals.genericRpcTracer == null)
				{
					ExTraceGlobals.genericRpcTracer = new Trace(ExTraceGlobals.componentGuid, 43);
				}
				return ExTraceGlobals.genericRpcTracer;
			}
		}

		public static Trace MapiSubmitLAMTracer
		{
			get
			{
				if (ExTraceGlobals.mapiSubmitLAMTracer == null)
				{
					ExTraceGlobals.mapiSubmitLAMTracer = new Trace(ExTraceGlobals.componentGuid, 44);
				}
				return ExTraceGlobals.mapiSubmitLAMTracer;
			}
		}

		public static Trace PublicFoldersTracer
		{
			get
			{
				if (ExTraceGlobals.publicFoldersTracer == null)
				{
					ExTraceGlobals.publicFoldersTracer = new Trace(ExTraceGlobals.componentGuid, 45);
				}
				return ExTraceGlobals.publicFoldersTracer;
			}
		}

		public static Trace SiteMailboxTracer
		{
			get
			{
				if (ExTraceGlobals.siteMailboxTracer == null)
				{
					ExTraceGlobals.siteMailboxTracer = new Trace(ExTraceGlobals.componentGuid, 46);
				}
				return ExTraceGlobals.siteMailboxTracer;
			}
		}

		public static Trace MailboxTransportTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxTransportTracer == null)
				{
					ExTraceGlobals.mailboxTransportTracer = new Trace(ExTraceGlobals.componentGuid, 47);
				}
				return ExTraceGlobals.mailboxTransportTracer;
			}
		}

		public static Trace WACTracer
		{
			get
			{
				if (ExTraceGlobals.wACTracer == null)
				{
					ExTraceGlobals.wACTracer = new Trace(ExTraceGlobals.componentGuid, 48);
				}
				return ExTraceGlobals.wACTracer;
			}
		}

		public static Trace ClassificationTracer
		{
			get
			{
				if (ExTraceGlobals.classificationTracer == null)
				{
					ExTraceGlobals.classificationTracer = new Trace(ExTraceGlobals.componentGuid, 49);
				}
				return ExTraceGlobals.classificationTracer;
			}
		}

		public static Trace ResultCacheTracer
		{
			get
			{
				if (ExTraceGlobals.resultCacheTracer == null)
				{
					ExTraceGlobals.resultCacheTracer = new Trace(ExTraceGlobals.componentGuid, 50);
				}
				return ExTraceGlobals.resultCacheTracer;
			}
		}

		public static Trace CentralAdminTracer
		{
			get
			{
				if (ExTraceGlobals.centralAdminTracer == null)
				{
					ExTraceGlobals.centralAdminTracer = new Trace(ExTraceGlobals.componentGuid, 51);
				}
				return ExTraceGlobals.centralAdminTracer;
			}
		}

		public static Trace DeploymentTracer
		{
			get
			{
				if (ExTraceGlobals.deploymentTracer == null)
				{
					ExTraceGlobals.deploymentTracer = new Trace(ExTraceGlobals.componentGuid, 52);
				}
				return ExTraceGlobals.deploymentTracer;
			}
		}

		public static Trace HDPhotoTracer
		{
			get
			{
				if (ExTraceGlobals.hDPhotoTracer == null)
				{
					ExTraceGlobals.hDPhotoTracer = new Trace(ExTraceGlobals.componentGuid, 54);
				}
				return ExTraceGlobals.hDPhotoTracer;
			}
		}

		public static Trace RBATracer
		{
			get
			{
				if (ExTraceGlobals.rBATracer == null)
				{
					ExTraceGlobals.rBATracer = new Trace(ExTraceGlobals.componentGuid, 55);
				}
				return ExTraceGlobals.rBATracer;
			}
		}

		public static Trace UserThrottlingTracer
		{
			get
			{
				if (ExTraceGlobals.userThrottlingTracer == null)
				{
					ExTraceGlobals.userThrottlingTracer = new Trace(ExTraceGlobals.componentGuid, 56);
				}
				return ExTraceGlobals.userThrottlingTracer;
			}
		}

		public static Trace InferenceTracer
		{
			get
			{
				if (ExTraceGlobals.inferenceTracer == null)
				{
					ExTraceGlobals.inferenceTracer = new Trace(ExTraceGlobals.componentGuid, 57);
				}
				return ExTraceGlobals.inferenceTracer;
			}
		}

		public static Trace PswsTracer
		{
			get
			{
				if (ExTraceGlobals.pswsTracer == null)
				{
					ExTraceGlobals.pswsTracer = new Trace(ExTraceGlobals.componentGuid, 58);
				}
				return ExTraceGlobals.pswsTracer;
			}
		}

		public static Trace PeopleConnectTracer
		{
			get
			{
				if (ExTraceGlobals.peopleConnectTracer == null)
				{
					ExTraceGlobals.peopleConnectTracer = new Trace(ExTraceGlobals.componentGuid, 59);
				}
				return ExTraceGlobals.peopleConnectTracer;
			}
		}

		public static Trace CrossPremiseTracer
		{
			get
			{
				if (ExTraceGlobals.crossPremiseTracer == null)
				{
					ExTraceGlobals.crossPremiseTracer = new Trace(ExTraceGlobals.componentGuid, 60);
				}
				return ExTraceGlobals.crossPremiseTracer;
			}
		}

		public static Trace E15InterruptionTracer
		{
			get
			{
				if (ExTraceGlobals.e15InterruptionTracer == null)
				{
					ExTraceGlobals.e15InterruptionTracer = new Trace(ExTraceGlobals.componentGuid, 61);
				}
				return ExTraceGlobals.e15InterruptionTracer;
			}
		}

		public static Trace FEPTracer
		{
			get
			{
				if (ExTraceGlobals.fEPTracer == null)
				{
					ExTraceGlobals.fEPTracer = new Trace(ExTraceGlobals.componentGuid, 62);
				}
				return ExTraceGlobals.fEPTracer;
			}
		}

		public static Trace EdiscoveryTracer
		{
			get
			{
				if (ExTraceGlobals.ediscoveryTracer == null)
				{
					ExTraceGlobals.ediscoveryTracer = new Trace(ExTraceGlobals.componentGuid, 63);
				}
				return ExTraceGlobals.ediscoveryTracer;
			}
		}

		public static Trace OnlineMeetingTracer
		{
			get
			{
				if (ExTraceGlobals.onlineMeetingTracer == null)
				{
					ExTraceGlobals.onlineMeetingTracer = new Trace(ExTraceGlobals.componentGuid, 64);
				}
				return ExTraceGlobals.onlineMeetingTracer;
			}
		}

		public static Trace MailboxSpaceTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxSpaceTracer == null)
				{
					ExTraceGlobals.mailboxSpaceTracer = new Trace(ExTraceGlobals.componentGuid, 65);
				}
				return ExTraceGlobals.mailboxSpaceTracer;
			}
		}

		public static Trace PushNotificationTracer
		{
			get
			{
				if (ExTraceGlobals.pushNotificationTracer == null)
				{
					ExTraceGlobals.pushNotificationTracer = new Trace(ExTraceGlobals.componentGuid, 66);
				}
				return ExTraceGlobals.pushNotificationTracer;
			}
		}

		public static Trace E4ETracer
		{
			get
			{
				if (ExTraceGlobals.e4ETracer == null)
				{
					ExTraceGlobals.e4ETracer = new Trace(ExTraceGlobals.componentGuid, 67);
				}
				return ExTraceGlobals.e4ETracer;
			}
		}

		public static Trace LockBoxTracer
		{
			get
			{
				if (ExTraceGlobals.lockBoxTracer == null)
				{
					ExTraceGlobals.lockBoxTracer = new Trace(ExTraceGlobals.componentGuid, 68);
				}
				return ExTraceGlobals.lockBoxTracer;
			}
		}

		public static Trace PersistentStateTracer
		{
			get
			{
				if (ExTraceGlobals.persistentStateTracer == null)
				{
					ExTraceGlobals.persistentStateTracer = new Trace(ExTraceGlobals.componentGuid, 69);
				}
				return ExTraceGlobals.persistentStateTracer;
			}
		}

		public static Trace AuditTracer
		{
			get
			{
				if (ExTraceGlobals.auditTracer == null)
				{
					ExTraceGlobals.auditTracer = new Trace(ExTraceGlobals.componentGuid, 70);
				}
				return ExTraceGlobals.auditTracer;
			}
		}

		public static Trace EndpointTracer
		{
			get
			{
				if (ExTraceGlobals.endpointTracer == null)
				{
					ExTraceGlobals.endpointTracer = new Trace(ExTraceGlobals.componentGuid, 71);
				}
				return ExTraceGlobals.endpointTracer;
			}
		}

		public static Trace EndpointMaintenanceTracer
		{
			get
			{
				if (ExTraceGlobals.endpointMaintenanceTracer == null)
				{
					ExTraceGlobals.endpointMaintenanceTracer = new Trace(ExTraceGlobals.componentGuid, 72);
				}
				return ExTraceGlobals.endpointMaintenanceTracer;
			}
		}

		public static Trace MonitoringEndpointTracer
		{
			get
			{
				if (ExTraceGlobals.monitoringEndpointTracer == null)
				{
					ExTraceGlobals.monitoringEndpointTracer = new Trace(ExTraceGlobals.componentGuid, 73);
				}
				return ExTraceGlobals.monitoringEndpointTracer;
			}
		}

		public static Trace LocalEndpointManagerTracer
		{
			get
			{
				if (ExTraceGlobals.localEndpointManagerTracer == null)
				{
					ExTraceGlobals.localEndpointManagerTracer = new Trace(ExTraceGlobals.componentGuid, 74);
				}
				return ExTraceGlobals.localEndpointManagerTracer;
			}
		}

		public static Trace MailboxDatabaseEndpointTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxDatabaseEndpointTracer == null)
				{
					ExTraceGlobals.mailboxDatabaseEndpointTracer = new Trace(ExTraceGlobals.componentGuid, 75);
				}
				return ExTraceGlobals.mailboxDatabaseEndpointTracer;
			}
		}

		public static Trace OfflineAddressBookEndpointTracer
		{
			get
			{
				if (ExTraceGlobals.offlineAddressBookEndpointTracer == null)
				{
					ExTraceGlobals.offlineAddressBookEndpointTracer = new Trace(ExTraceGlobals.componentGuid, 76);
				}
				return ExTraceGlobals.offlineAddressBookEndpointTracer;
			}
		}

		public static Trace OverrideEndpointTracer
		{
			get
			{
				if (ExTraceGlobals.overrideEndpointTracer == null)
				{
					ExTraceGlobals.overrideEndpointTracer = new Trace(ExTraceGlobals.componentGuid, 77);
				}
				return ExTraceGlobals.overrideEndpointTracer;
			}
		}

		public static Trace RecoveryActionsEnabledEndpointTracer
		{
			get
			{
				if (ExTraceGlobals.recoveryActionsEnabledEndpointTracer == null)
				{
					ExTraceGlobals.recoveryActionsEnabledEndpointTracer = new Trace(ExTraceGlobals.componentGuid, 78);
				}
				return ExTraceGlobals.recoveryActionsEnabledEndpointTracer;
			}
		}

		public static Trace ExchangeServerRoleEndpointTracer
		{
			get
			{
				if (ExTraceGlobals.exchangeServerRoleEndpointTracer == null)
				{
					ExTraceGlobals.exchangeServerRoleEndpointTracer = new Trace(ExTraceGlobals.componentGuid, 79);
				}
				return ExTraceGlobals.exchangeServerRoleEndpointTracer;
			}
		}

		public static Trace SubjectListEndpointTracer
		{
			get
			{
				if (ExTraceGlobals.subjectListEndpointTracer == null)
				{
					ExTraceGlobals.subjectListEndpointTracer = new Trace(ExTraceGlobals.componentGuid, 80);
				}
				return ExTraceGlobals.subjectListEndpointTracer;
			}
		}

		public static Trace UnifiedMessagingEndpointTracer
		{
			get
			{
				if (ExTraceGlobals.unifiedMessagingEndpointTracer == null)
				{
					ExTraceGlobals.unifiedMessagingEndpointTracer = new Trace(ExTraceGlobals.componentGuid, 81);
				}
				return ExTraceGlobals.unifiedMessagingEndpointTracer;
			}
		}

		public static Trace WindowsServerRoleEndpointTracer
		{
			get
			{
				if (ExTraceGlobals.windowsServerRoleEndpointTracer == null)
				{
					ExTraceGlobals.windowsServerRoleEndpointTracer = new Trace(ExTraceGlobals.componentGuid, 82);
				}
				return ExTraceGlobals.windowsServerRoleEndpointTracer;
			}
		}

		public static Trace ScopeMappingLocalEndpointTracer
		{
			get
			{
				if (ExTraceGlobals.scopeMappingLocalEndpointTracer == null)
				{
					ExTraceGlobals.scopeMappingLocalEndpointTracer = new Trace(ExTraceGlobals.componentGuid, 83);
				}
				return ExTraceGlobals.scopeMappingLocalEndpointTracer;
			}
		}

		public static Trace TimeBasedAssistantsTracer
		{
			get
			{
				if (ExTraceGlobals.timeBasedAssistantsTracer == null)
				{
					ExTraceGlobals.timeBasedAssistantsTracer = new Trace(ExTraceGlobals.componentGuid, 84);
				}
				return ExTraceGlobals.timeBasedAssistantsTracer;
			}
		}

		public static Trace RemoteStoreTracer
		{
			get
			{
				if (ExTraceGlobals.remoteStoreTracer == null)
				{
					ExTraceGlobals.remoteStoreTracer = new Trace(ExTraceGlobals.componentGuid, 85);
				}
				return ExTraceGlobals.remoteStoreTracer;
			}
		}

		public static Trace WasclTracer
		{
			get
			{
				if (ExTraceGlobals.wasclTracer == null)
				{
					ExTraceGlobals.wasclTracer = new Trace(ExTraceGlobals.componentGuid, 86);
				}
				return ExTraceGlobals.wasclTracer;
			}
		}

		public static Trace GenericRusTracer
		{
			get
			{
				if (ExTraceGlobals.genericRusTracer == null)
				{
					ExTraceGlobals.genericRusTracer = new Trace(ExTraceGlobals.componentGuid, 87);
				}
				return ExTraceGlobals.genericRusTracer;
			}
		}

		public static Trace OfficeGraphTracer
		{
			get
			{
				if (ExTraceGlobals.officeGraphTracer == null)
				{
					ExTraceGlobals.officeGraphTracer = new Trace(ExTraceGlobals.componentGuid, 88);
				}
				return ExTraceGlobals.officeGraphTracer;
			}
		}

		public static Trace PUMTracer
		{
			get
			{
				if (ExTraceGlobals.pUMTracer == null)
				{
					ExTraceGlobals.pUMTracer = new Trace(ExTraceGlobals.componentGuid, 89);
				}
				return ExTraceGlobals.pUMTracer;
			}
		}

		private static Guid componentGuid = new Guid("EAF36C57-87B9-4D84-B551-3537A14A62B9");

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace azureTracer = null;

		private static Trace commonComponentsTracer = null;

		private static Trace heartbeatTracer = null;

		private static Trace hTTPTracer = null;

		private static Trace oWATracer = null;

		private static Trace rPCHTTPTracer = null;

		private static Trace activeSyncTracer = null;

		private static Trace eWSTracer = null;

		private static Trace autoDiscoverTracer = null;

		private static Trace liveIdTracer = null;

		private static Trace rIMTracer = null;

		private static Trace serviceTracer = null;

		private static Trace workerTracer = null;

		private static Trace oABTracer = null;

		private static Trace storeTracer = null;

		private static Trace availabilityServiceTracer = null;

		private static Trace networkTracer = null;

		private static Trace unifiedMessagingTracer = null;

		private static Trace rPSTracer = null;

		private static Trace officeTracer = null;

		private static Trace iMAP4Tracer = null;

		private static Trace pOP3Tracer = null;

		private static Trace searchTracer = null;

		private static Trace migrationTracer = null;

		private static Trace directoryTracer = null;

		private static Trace highAvailabilityTracer = null;

		private static Trace provisioningTracer = null;

		private static Trace transportTracer = null;

		private static Trace monitoringTracer = null;

		private static Trace calendarSharingTracer = null;

		private static Trace cafeTracer = null;

		private static Trace eventAssistantsTracer = null;

		private static Trace fIPSTracer = null;

		private static Trace antimalwareTracer = null;

		private static Trace transportSyncTracer = null;

		private static Trace eCPTracer = null;

		private static Trace securityTracer = null;

		private static Trace rWSTracer = null;

		private static Trace eDSTracer = null;

		private static Trace processIsolationTracer = null;

		private static Trace activeMonitoringRpcTracer = null;

		private static Trace recoveryActionTracer = null;

		private static Trace genericRpcTracer = null;

		private static Trace mapiSubmitLAMTracer = null;

		private static Trace publicFoldersTracer = null;

		private static Trace siteMailboxTracer = null;

		private static Trace mailboxTransportTracer = null;

		private static Trace wACTracer = null;

		private static Trace classificationTracer = null;

		private static Trace resultCacheTracer = null;

		private static Trace centralAdminTracer = null;

		private static Trace deploymentTracer = null;

		private static Trace hDPhotoTracer = null;

		private static Trace rBATracer = null;

		private static Trace userThrottlingTracer = null;

		private static Trace inferenceTracer = null;

		private static Trace pswsTracer = null;

		private static Trace peopleConnectTracer = null;

		private static Trace crossPremiseTracer = null;

		private static Trace e15InterruptionTracer = null;

		private static Trace fEPTracer = null;

		private static Trace ediscoveryTracer = null;

		private static Trace onlineMeetingTracer = null;

		private static Trace mailboxSpaceTracer = null;

		private static Trace pushNotificationTracer = null;

		private static Trace e4ETracer = null;

		private static Trace lockBoxTracer = null;

		private static Trace persistentStateTracer = null;

		private static Trace auditTracer = null;

		private static Trace endpointTracer = null;

		private static Trace endpointMaintenanceTracer = null;

		private static Trace monitoringEndpointTracer = null;

		private static Trace localEndpointManagerTracer = null;

		private static Trace mailboxDatabaseEndpointTracer = null;

		private static Trace offlineAddressBookEndpointTracer = null;

		private static Trace overrideEndpointTracer = null;

		private static Trace recoveryActionsEnabledEndpointTracer = null;

		private static Trace exchangeServerRoleEndpointTracer = null;

		private static Trace subjectListEndpointTracer = null;

		private static Trace unifiedMessagingEndpointTracer = null;

		private static Trace windowsServerRoleEndpointTracer = null;

		private static Trace scopeMappingLocalEndpointTracer = null;

		private static Trace timeBasedAssistantsTracer = null;

		private static Trace remoteStoreTracer = null;

		private static Trace wasclTracer = null;

		private static Trace genericRusTracer = null;

		private static Trace officeGraphTracer = null;

		private static Trace pUMTracer = null;
	}
}
