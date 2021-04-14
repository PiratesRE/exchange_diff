using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Services
{
	public static class ExTraceGlobals
	{
		public static Trace CalendarAlgorithmTracer
		{
			get
			{
				if (ExTraceGlobals.calendarAlgorithmTracer == null)
				{
					ExTraceGlobals.calendarAlgorithmTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.calendarAlgorithmTracer;
			}
		}

		public static Trace CalendarDataTracer
		{
			get
			{
				if (ExTraceGlobals.calendarDataTracer == null)
				{
					ExTraceGlobals.calendarDataTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.calendarDataTracer;
			}
		}

		public static Trace CalendarCallTracer
		{
			get
			{
				if (ExTraceGlobals.calendarCallTracer == null)
				{
					ExTraceGlobals.calendarCallTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.calendarCallTracer;
			}
		}

		public static Trace CommonAlgorithmTracer
		{
			get
			{
				if (ExTraceGlobals.commonAlgorithmTracer == null)
				{
					ExTraceGlobals.commonAlgorithmTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.commonAlgorithmTracer;
			}
		}

		public static Trace FolderAlgorithmTracer
		{
			get
			{
				if (ExTraceGlobals.folderAlgorithmTracer == null)
				{
					ExTraceGlobals.folderAlgorithmTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.folderAlgorithmTracer;
			}
		}

		public static Trace FolderDataTracer
		{
			get
			{
				if (ExTraceGlobals.folderDataTracer == null)
				{
					ExTraceGlobals.folderDataTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.folderDataTracer;
			}
		}

		public static Trace FolderCallTracer
		{
			get
			{
				if (ExTraceGlobals.folderCallTracer == null)
				{
					ExTraceGlobals.folderCallTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.folderCallTracer;
			}
		}

		public static Trace ItemAlgorithmTracer
		{
			get
			{
				if (ExTraceGlobals.itemAlgorithmTracer == null)
				{
					ExTraceGlobals.itemAlgorithmTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.itemAlgorithmTracer;
			}
		}

		public static Trace ItemDataTracer
		{
			get
			{
				if (ExTraceGlobals.itemDataTracer == null)
				{
					ExTraceGlobals.itemDataTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.itemDataTracer;
			}
		}

		public static Trace ItemCallTracer
		{
			get
			{
				if (ExTraceGlobals.itemCallTracer == null)
				{
					ExTraceGlobals.itemCallTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.itemCallTracer;
			}
		}

		public static Trace ExceptionTracer
		{
			get
			{
				if (ExTraceGlobals.exceptionTracer == null)
				{
					ExTraceGlobals.exceptionTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.exceptionTracer;
			}
		}

		public static Trace SessionCacheTracer
		{
			get
			{
				if (ExTraceGlobals.sessionCacheTracer == null)
				{
					ExTraceGlobals.sessionCacheTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.sessionCacheTracer;
			}
		}

		public static Trace ExchangePrincipalCacheTracer
		{
			get
			{
				if (ExTraceGlobals.exchangePrincipalCacheTracer == null)
				{
					ExTraceGlobals.exchangePrincipalCacheTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.exchangePrincipalCacheTracer;
			}
		}

		public static Trace SearchTracer
		{
			get
			{
				if (ExTraceGlobals.searchTracer == null)
				{
					ExTraceGlobals.searchTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.searchTracer;
			}
		}

		public static Trace UtilAlgorithmTracer
		{
			get
			{
				if (ExTraceGlobals.utilAlgorithmTracer == null)
				{
					ExTraceGlobals.utilAlgorithmTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.utilAlgorithmTracer;
			}
		}

		public static Trace UtilDataTracer
		{
			get
			{
				if (ExTraceGlobals.utilDataTracer == null)
				{
					ExTraceGlobals.utilDataTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.utilDataTracer;
			}
		}

		public static Trace UtilCallTracer
		{
			get
			{
				if (ExTraceGlobals.utilCallTracer == null)
				{
					ExTraceGlobals.utilCallTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.utilCallTracer;
			}
		}

		public static Trace ServerToServerAuthZTracer
		{
			get
			{
				if (ExTraceGlobals.serverToServerAuthZTracer == null)
				{
					ExTraceGlobals.serverToServerAuthZTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.serverToServerAuthZTracer;
			}
		}

		public static Trace ServiceCommandBaseCallTracer
		{
			get
			{
				if (ExTraceGlobals.serviceCommandBaseCallTracer == null)
				{
					ExTraceGlobals.serviceCommandBaseCallTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.serviceCommandBaseCallTracer;
			}
		}

		public static Trace ServiceCommandBaseDataTracer
		{
			get
			{
				if (ExTraceGlobals.serviceCommandBaseDataTracer == null)
				{
					ExTraceGlobals.serviceCommandBaseDataTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.serviceCommandBaseDataTracer;
			}
		}

		public static Trace FacadeBaseCallTracer
		{
			get
			{
				if (ExTraceGlobals.facadeBaseCallTracer == null)
				{
					ExTraceGlobals.facadeBaseCallTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.facadeBaseCallTracer;
			}
		}

		public static Trace CreateItemCallTracer
		{
			get
			{
				if (ExTraceGlobals.createItemCallTracer == null)
				{
					ExTraceGlobals.createItemCallTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.createItemCallTracer;
			}
		}

		public static Trace GetItemCallTracer
		{
			get
			{
				if (ExTraceGlobals.getItemCallTracer == null)
				{
					ExTraceGlobals.getItemCallTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.getItemCallTracer;
			}
		}

		public static Trace UpdateItemCallTracer
		{
			get
			{
				if (ExTraceGlobals.updateItemCallTracer == null)
				{
					ExTraceGlobals.updateItemCallTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.updateItemCallTracer;
			}
		}

		public static Trace DeleteItemCallTracer
		{
			get
			{
				if (ExTraceGlobals.deleteItemCallTracer == null)
				{
					ExTraceGlobals.deleteItemCallTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.deleteItemCallTracer;
			}
		}

		public static Trace SendItemCallTracer
		{
			get
			{
				if (ExTraceGlobals.sendItemCallTracer == null)
				{
					ExTraceGlobals.sendItemCallTracer = new Trace(ExTraceGlobals.componentGuid, 25);
				}
				return ExTraceGlobals.sendItemCallTracer;
			}
		}

		public static Trace MoveCopyCommandBaseCallTracer
		{
			get
			{
				if (ExTraceGlobals.moveCopyCommandBaseCallTracer == null)
				{
					ExTraceGlobals.moveCopyCommandBaseCallTracer = new Trace(ExTraceGlobals.componentGuid, 26);
				}
				return ExTraceGlobals.moveCopyCommandBaseCallTracer;
			}
		}

		public static Trace MoveCopyItemCommandBaseCallTracer
		{
			get
			{
				if (ExTraceGlobals.moveCopyItemCommandBaseCallTracer == null)
				{
					ExTraceGlobals.moveCopyItemCommandBaseCallTracer = new Trace(ExTraceGlobals.componentGuid, 27);
				}
				return ExTraceGlobals.moveCopyItemCommandBaseCallTracer;
			}
		}

		public static Trace CopyItemCallTracer
		{
			get
			{
				if (ExTraceGlobals.copyItemCallTracer == null)
				{
					ExTraceGlobals.copyItemCallTracer = new Trace(ExTraceGlobals.componentGuid, 28);
				}
				return ExTraceGlobals.copyItemCallTracer;
			}
		}

		public static Trace MoveItemCallTracer
		{
			get
			{
				if (ExTraceGlobals.moveItemCallTracer == null)
				{
					ExTraceGlobals.moveItemCallTracer = new Trace(ExTraceGlobals.componentGuid, 29);
				}
				return ExTraceGlobals.moveItemCallTracer;
			}
		}

		public static Trace CreateFolderCallTracer
		{
			get
			{
				if (ExTraceGlobals.createFolderCallTracer == null)
				{
					ExTraceGlobals.createFolderCallTracer = new Trace(ExTraceGlobals.componentGuid, 30);
				}
				return ExTraceGlobals.createFolderCallTracer;
			}
		}

		public static Trace GetFolderCallTracer
		{
			get
			{
				if (ExTraceGlobals.getFolderCallTracer == null)
				{
					ExTraceGlobals.getFolderCallTracer = new Trace(ExTraceGlobals.componentGuid, 31);
				}
				return ExTraceGlobals.getFolderCallTracer;
			}
		}

		public static Trace UpdateFolderCallTracer
		{
			get
			{
				if (ExTraceGlobals.updateFolderCallTracer == null)
				{
					ExTraceGlobals.updateFolderCallTracer = new Trace(ExTraceGlobals.componentGuid, 32);
				}
				return ExTraceGlobals.updateFolderCallTracer;
			}
		}

		public static Trace DeleteFolderCallTracer
		{
			get
			{
				if (ExTraceGlobals.deleteFolderCallTracer == null)
				{
					ExTraceGlobals.deleteFolderCallTracer = new Trace(ExTraceGlobals.componentGuid, 33);
				}
				return ExTraceGlobals.deleteFolderCallTracer;
			}
		}

		public static Trace MoveCopyFolderCommandBaseCallTracer
		{
			get
			{
				if (ExTraceGlobals.moveCopyFolderCommandBaseCallTracer == null)
				{
					ExTraceGlobals.moveCopyFolderCommandBaseCallTracer = new Trace(ExTraceGlobals.componentGuid, 34);
				}
				return ExTraceGlobals.moveCopyFolderCommandBaseCallTracer;
			}
		}

		public static Trace CopyFolderCallTracer
		{
			get
			{
				if (ExTraceGlobals.copyFolderCallTracer == null)
				{
					ExTraceGlobals.copyFolderCallTracer = new Trace(ExTraceGlobals.componentGuid, 35);
				}
				return ExTraceGlobals.copyFolderCallTracer;
			}
		}

		public static Trace MoveFolderCallTracer
		{
			get
			{
				if (ExTraceGlobals.moveFolderCallTracer == null)
				{
					ExTraceGlobals.moveFolderCallTracer = new Trace(ExTraceGlobals.componentGuid, 36);
				}
				return ExTraceGlobals.moveFolderCallTracer;
			}
		}

		public static Trace FindCommandBaseCallTracer
		{
			get
			{
				if (ExTraceGlobals.findCommandBaseCallTracer == null)
				{
					ExTraceGlobals.findCommandBaseCallTracer = new Trace(ExTraceGlobals.componentGuid, 37);
				}
				return ExTraceGlobals.findCommandBaseCallTracer;
			}
		}

		public static Trace FindItemCallTracer
		{
			get
			{
				if (ExTraceGlobals.findItemCallTracer == null)
				{
					ExTraceGlobals.findItemCallTracer = new Trace(ExTraceGlobals.componentGuid, 38);
				}
				return ExTraceGlobals.findItemCallTracer;
			}
		}

		public static Trace FindFolderCallTracer
		{
			get
			{
				if (ExTraceGlobals.findFolderCallTracer == null)
				{
					ExTraceGlobals.findFolderCallTracer = new Trace(ExTraceGlobals.componentGuid, 39);
				}
				return ExTraceGlobals.findFolderCallTracer;
			}
		}

		public static Trace UtilCommandBaseCallTracer
		{
			get
			{
				if (ExTraceGlobals.utilCommandBaseCallTracer == null)
				{
					ExTraceGlobals.utilCommandBaseCallTracer = new Trace(ExTraceGlobals.componentGuid, 40);
				}
				return ExTraceGlobals.utilCommandBaseCallTracer;
			}
		}

		public static Trace ExpandDLCallTracer
		{
			get
			{
				if (ExTraceGlobals.expandDLCallTracer == null)
				{
					ExTraceGlobals.expandDLCallTracer = new Trace(ExTraceGlobals.componentGuid, 41);
				}
				return ExTraceGlobals.expandDLCallTracer;
			}
		}

		public static Trace ResolveNamesCallTracer
		{
			get
			{
				if (ExTraceGlobals.resolveNamesCallTracer == null)
				{
					ExTraceGlobals.resolveNamesCallTracer = new Trace(ExTraceGlobals.componentGuid, 42);
				}
				return ExTraceGlobals.resolveNamesCallTracer;
			}
		}

		public static Trace SubscribeCallTracer
		{
			get
			{
				if (ExTraceGlobals.subscribeCallTracer == null)
				{
					ExTraceGlobals.subscribeCallTracer = new Trace(ExTraceGlobals.componentGuid, 43);
				}
				return ExTraceGlobals.subscribeCallTracer;
			}
		}

		public static Trace UnsubscribeCallTracer
		{
			get
			{
				if (ExTraceGlobals.unsubscribeCallTracer == null)
				{
					ExTraceGlobals.unsubscribeCallTracer = new Trace(ExTraceGlobals.componentGuid, 44);
				}
				return ExTraceGlobals.unsubscribeCallTracer;
			}
		}

		public static Trace GetEventsCallTracer
		{
			get
			{
				if (ExTraceGlobals.getEventsCallTracer == null)
				{
					ExTraceGlobals.getEventsCallTracer = new Trace(ExTraceGlobals.componentGuid, 45);
				}
				return ExTraceGlobals.getEventsCallTracer;
			}
		}

		public static Trace SubscriptionsTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionsTracer == null)
				{
					ExTraceGlobals.subscriptionsTracer = new Trace(ExTraceGlobals.componentGuid, 46);
				}
				return ExTraceGlobals.subscriptionsTracer;
			}
		}

		public static Trace SubscriptionBaseTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionBaseTracer == null)
				{
					ExTraceGlobals.subscriptionBaseTracer = new Trace(ExTraceGlobals.componentGuid, 47);
				}
				return ExTraceGlobals.subscriptionBaseTracer;
			}
		}

		public static Trace PushSubscriptionTracer
		{
			get
			{
				if (ExTraceGlobals.pushSubscriptionTracer == null)
				{
					ExTraceGlobals.pushSubscriptionTracer = new Trace(ExTraceGlobals.componentGuid, 48);
				}
				return ExTraceGlobals.pushSubscriptionTracer;
			}
		}

		public static Trace SyncFolderHierarchyCallTracer
		{
			get
			{
				if (ExTraceGlobals.syncFolderHierarchyCallTracer == null)
				{
					ExTraceGlobals.syncFolderHierarchyCallTracer = new Trace(ExTraceGlobals.componentGuid, 49);
				}
				return ExTraceGlobals.syncFolderHierarchyCallTracer;
			}
		}

		public static Trace SyncFolderItemsCallTracer
		{
			get
			{
				if (ExTraceGlobals.syncFolderItemsCallTracer == null)
				{
					ExTraceGlobals.syncFolderItemsCallTracer = new Trace(ExTraceGlobals.componentGuid, 50);
				}
				return ExTraceGlobals.syncFolderItemsCallTracer;
			}
		}

		public static Trace SynchronizationTracer
		{
			get
			{
				if (ExTraceGlobals.synchronizationTracer == null)
				{
					ExTraceGlobals.synchronizationTracer = new Trace(ExTraceGlobals.componentGuid, 51);
				}
				return ExTraceGlobals.synchronizationTracer;
			}
		}

		public static Trace PerformanceMonitorTracer
		{
			get
			{
				if (ExTraceGlobals.performanceMonitorTracer == null)
				{
					ExTraceGlobals.performanceMonitorTracer = new Trace(ExTraceGlobals.componentGuid, 52);
				}
				return ExTraceGlobals.performanceMonitorTracer;
			}
		}

		public static Trace ConvertIdCallTracer
		{
			get
			{
				if (ExTraceGlobals.convertIdCallTracer == null)
				{
					ExTraceGlobals.convertIdCallTracer = new Trace(ExTraceGlobals.componentGuid, 53);
				}
				return ExTraceGlobals.convertIdCallTracer;
			}
		}

		public static Trace GetDelegateCallTracer
		{
			get
			{
				if (ExTraceGlobals.getDelegateCallTracer == null)
				{
					ExTraceGlobals.getDelegateCallTracer = new Trace(ExTraceGlobals.componentGuid, 54);
				}
				return ExTraceGlobals.getDelegateCallTracer;
			}
		}

		public static Trace AddDelegateCallTracer
		{
			get
			{
				if (ExTraceGlobals.addDelegateCallTracer == null)
				{
					ExTraceGlobals.addDelegateCallTracer = new Trace(ExTraceGlobals.componentGuid, 55);
				}
				return ExTraceGlobals.addDelegateCallTracer;
			}
		}

		public static Trace RemoveDelegateCallTracer
		{
			get
			{
				if (ExTraceGlobals.removeDelegateCallTracer == null)
				{
					ExTraceGlobals.removeDelegateCallTracer = new Trace(ExTraceGlobals.componentGuid, 56);
				}
				return ExTraceGlobals.removeDelegateCallTracer;
			}
		}

		public static Trace UpdateDelegateCallTracer
		{
			get
			{
				if (ExTraceGlobals.updateDelegateCallTracer == null)
				{
					ExTraceGlobals.updateDelegateCallTracer = new Trace(ExTraceGlobals.componentGuid, 57);
				}
				return ExTraceGlobals.updateDelegateCallTracer;
			}
		}

		public static Trace ProxyEvaluatorTracer
		{
			get
			{
				if (ExTraceGlobals.proxyEvaluatorTracer == null)
				{
					ExTraceGlobals.proxyEvaluatorTracer = new Trace(ExTraceGlobals.componentGuid, 58);
				}
				return ExTraceGlobals.proxyEvaluatorTracer;
			}
		}

		public static Trace GetMailTipsCallTracer
		{
			get
			{
				if (ExTraceGlobals.getMailTipsCallTracer == null)
				{
					ExTraceGlobals.getMailTipsCallTracer = new Trace(ExTraceGlobals.componentGuid, 60);
				}
				return ExTraceGlobals.getMailTipsCallTracer;
			}
		}

		public static Trace AllRequestsTracer
		{
			get
			{
				if (ExTraceGlobals.allRequestsTracer == null)
				{
					ExTraceGlobals.allRequestsTracer = new Trace(ExTraceGlobals.componentGuid, 61);
				}
				return ExTraceGlobals.allRequestsTracer;
			}
		}

		public static Trace AuthenticationTracer
		{
			get
			{
				if (ExTraceGlobals.authenticationTracer == null)
				{
					ExTraceGlobals.authenticationTracer = new Trace(ExTraceGlobals.componentGuid, 62);
				}
				return ExTraceGlobals.authenticationTracer;
			}
		}

		public static Trace WCFTracer
		{
			get
			{
				if (ExTraceGlobals.wCFTracer == null)
				{
					ExTraceGlobals.wCFTracer = new Trace(ExTraceGlobals.componentGuid, 63);
				}
				return ExTraceGlobals.wCFTracer;
			}
		}

		public static Trace GetUserConfigurationCallTracer
		{
			get
			{
				if (ExTraceGlobals.getUserConfigurationCallTracer == null)
				{
					ExTraceGlobals.getUserConfigurationCallTracer = new Trace(ExTraceGlobals.componentGuid, 64);
				}
				return ExTraceGlobals.getUserConfigurationCallTracer;
			}
		}

		public static Trace CreateUserConfigurationCallTracer
		{
			get
			{
				if (ExTraceGlobals.createUserConfigurationCallTracer == null)
				{
					ExTraceGlobals.createUserConfigurationCallTracer = new Trace(ExTraceGlobals.componentGuid, 65);
				}
				return ExTraceGlobals.createUserConfigurationCallTracer;
			}
		}

		public static Trace DeleteUserConfigurationCallTracer
		{
			get
			{
				if (ExTraceGlobals.deleteUserConfigurationCallTracer == null)
				{
					ExTraceGlobals.deleteUserConfigurationCallTracer = new Trace(ExTraceGlobals.componentGuid, 66);
				}
				return ExTraceGlobals.deleteUserConfigurationCallTracer;
			}
		}

		public static Trace UpdateUserConfigurationCallTracer
		{
			get
			{
				if (ExTraceGlobals.updateUserConfigurationCallTracer == null)
				{
					ExTraceGlobals.updateUserConfigurationCallTracer = new Trace(ExTraceGlobals.componentGuid, 67);
				}
				return ExTraceGlobals.updateUserConfigurationCallTracer;
			}
		}

		public static Trace ThrottlingTracer
		{
			get
			{
				if (ExTraceGlobals.throttlingTracer == null)
				{
					ExTraceGlobals.throttlingTracer = new Trace(ExTraceGlobals.componentGuid, 68);
				}
				return ExTraceGlobals.throttlingTracer;
			}
		}

		public static Trace ExternalUserTracer
		{
			get
			{
				if (ExTraceGlobals.externalUserTracer == null)
				{
					ExTraceGlobals.externalUserTracer = new Trace(ExTraceGlobals.componentGuid, 69);
				}
				return ExTraceGlobals.externalUserTracer;
			}
		}

		public static Trace GetOrganizationConfigurationCallTracer
		{
			get
			{
				if (ExTraceGlobals.getOrganizationConfigurationCallTracer == null)
				{
					ExTraceGlobals.getOrganizationConfigurationCallTracer = new Trace(ExTraceGlobals.componentGuid, 70);
				}
				return ExTraceGlobals.getOrganizationConfigurationCallTracer;
			}
		}

		public static Trace GetRoomsCallTracer
		{
			get
			{
				if (ExTraceGlobals.getRoomsCallTracer == null)
				{
					ExTraceGlobals.getRoomsCallTracer = new Trace(ExTraceGlobals.componentGuid, 71);
				}
				return ExTraceGlobals.getRoomsCallTracer;
			}
		}

		public static Trace GetFederationInformationTracer
		{
			get
			{
				if (ExTraceGlobals.getFederationInformationTracer == null)
				{
					ExTraceGlobals.getFederationInformationTracer = new Trace(ExTraceGlobals.componentGuid, 72);
				}
				return ExTraceGlobals.getFederationInformationTracer;
			}
		}

		public static Trace ParticipantLookupBatchingTracer
		{
			get
			{
				if (ExTraceGlobals.participantLookupBatchingTracer == null)
				{
					ExTraceGlobals.participantLookupBatchingTracer = new Trace(ExTraceGlobals.componentGuid, 73);
				}
				return ExTraceGlobals.participantLookupBatchingTracer;
			}
		}

		public static Trace AllResponsesTracer
		{
			get
			{
				if (ExTraceGlobals.allResponsesTracer == null)
				{
					ExTraceGlobals.allResponsesTracer = new Trace(ExTraceGlobals.componentGuid, 74);
				}
				return ExTraceGlobals.allResponsesTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 75);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace GetInboxRulesCallTracer
		{
			get
			{
				if (ExTraceGlobals.getInboxRulesCallTracer == null)
				{
					ExTraceGlobals.getInboxRulesCallTracer = new Trace(ExTraceGlobals.componentGuid, 76);
				}
				return ExTraceGlobals.getInboxRulesCallTracer;
			}
		}

		public static Trace UpdateInboxRulesCallTracer
		{
			get
			{
				if (ExTraceGlobals.updateInboxRulesCallTracer == null)
				{
					ExTraceGlobals.updateInboxRulesCallTracer = new Trace(ExTraceGlobals.componentGuid, 77);
				}
				return ExTraceGlobals.updateInboxRulesCallTracer;
			}
		}

		public static Trace GetCASMailboxTracer
		{
			get
			{
				if (ExTraceGlobals.getCASMailboxTracer == null)
				{
					ExTraceGlobals.getCASMailboxTracer = new Trace(ExTraceGlobals.componentGuid, 78);
				}
				return ExTraceGlobals.getCASMailboxTracer;
			}
		}

		public static Trace FastTransferTracer
		{
			get
			{
				if (ExTraceGlobals.fastTransferTracer == null)
				{
					ExTraceGlobals.fastTransferTracer = new Trace(ExTraceGlobals.componentGuid, 79);
				}
				return ExTraceGlobals.fastTransferTracer;
			}
		}

		public static Trace SyncConversationCallTracer
		{
			get
			{
				if (ExTraceGlobals.syncConversationCallTracer == null)
				{
					ExTraceGlobals.syncConversationCallTracer = new Trace(ExTraceGlobals.componentGuid, 80);
				}
				return ExTraceGlobals.syncConversationCallTracer;
			}
		}

		public static Trace ELCTracer
		{
			get
			{
				if (ExTraceGlobals.eLCTracer == null)
				{
					ExTraceGlobals.eLCTracer = new Trace(ExTraceGlobals.componentGuid, 81);
				}
				return ExTraceGlobals.eLCTracer;
			}
		}

		public static Trace ActivityConverterTracer
		{
			get
			{
				if (ExTraceGlobals.activityConverterTracer == null)
				{
					ExTraceGlobals.activityConverterTracer = new Trace(ExTraceGlobals.componentGuid, 82);
				}
				return ExTraceGlobals.activityConverterTracer;
			}
		}

		public static Trace SyncPeopleCallTracer
		{
			get
			{
				if (ExTraceGlobals.syncPeopleCallTracer == null)
				{
					ExTraceGlobals.syncPeopleCallTracer = new Trace(ExTraceGlobals.componentGuid, 83);
				}
				return ExTraceGlobals.syncPeopleCallTracer;
			}
		}

		public static Trace GetCalendarFoldersCallTracer
		{
			get
			{
				if (ExTraceGlobals.getCalendarFoldersCallTracer == null)
				{
					ExTraceGlobals.getCalendarFoldersCallTracer = new Trace(ExTraceGlobals.componentGuid, 84);
				}
				return ExTraceGlobals.getCalendarFoldersCallTracer;
			}
		}

		public static Trace GetRemindersCallTracer
		{
			get
			{
				if (ExTraceGlobals.getRemindersCallTracer == null)
				{
					ExTraceGlobals.getRemindersCallTracer = new Trace(ExTraceGlobals.componentGuid, 85);
				}
				return ExTraceGlobals.getRemindersCallTracer;
			}
		}

		public static Trace SyncCalendarCallTracer
		{
			get
			{
				if (ExTraceGlobals.syncCalendarCallTracer == null)
				{
					ExTraceGlobals.syncCalendarCallTracer = new Trace(ExTraceGlobals.componentGuid, 86);
				}
				return ExTraceGlobals.syncCalendarCallTracer;
			}
		}

		public static Trace PerformReminderActionCallTracer
		{
			get
			{
				if (ExTraceGlobals.performReminderActionCallTracer == null)
				{
					ExTraceGlobals.performReminderActionCallTracer = new Trace(ExTraceGlobals.componentGuid, 87);
				}
				return ExTraceGlobals.performReminderActionCallTracer;
			}
		}

		public static Trace ProvisionCallTracer
		{
			get
			{
				if (ExTraceGlobals.provisionCallTracer == null)
				{
					ExTraceGlobals.provisionCallTracer = new Trace(ExTraceGlobals.componentGuid, 88);
				}
				return ExTraceGlobals.provisionCallTracer;
			}
		}

		public static Trace RenameCalendarGroupCallTracer
		{
			get
			{
				if (ExTraceGlobals.renameCalendarGroupCallTracer == null)
				{
					ExTraceGlobals.renameCalendarGroupCallTracer = new Trace(ExTraceGlobals.componentGuid, 89);
				}
				return ExTraceGlobals.renameCalendarGroupCallTracer;
			}
		}

		public static Trace DeleteCalendarGroupCallTracer
		{
			get
			{
				if (ExTraceGlobals.deleteCalendarGroupCallTracer == null)
				{
					ExTraceGlobals.deleteCalendarGroupCallTracer = new Trace(ExTraceGlobals.componentGuid, 90);
				}
				return ExTraceGlobals.deleteCalendarGroupCallTracer;
			}
		}

		public static Trace CreateCalendarCallTracer
		{
			get
			{
				if (ExTraceGlobals.createCalendarCallTracer == null)
				{
					ExTraceGlobals.createCalendarCallTracer = new Trace(ExTraceGlobals.componentGuid, 91);
				}
				return ExTraceGlobals.createCalendarCallTracer;
			}
		}

		public static Trace RenameCalendarCallTracer
		{
			get
			{
				if (ExTraceGlobals.renameCalendarCallTracer == null)
				{
					ExTraceGlobals.renameCalendarCallTracer = new Trace(ExTraceGlobals.componentGuid, 92);
				}
				return ExTraceGlobals.renameCalendarCallTracer;
			}
		}

		public static Trace DeleteCalendarCallTracer
		{
			get
			{
				if (ExTraceGlobals.deleteCalendarCallTracer == null)
				{
					ExTraceGlobals.deleteCalendarCallTracer = new Trace(ExTraceGlobals.componentGuid, 93);
				}
				return ExTraceGlobals.deleteCalendarCallTracer;
			}
		}

		public static Trace SetCalendarColorCallTracer
		{
			get
			{
				if (ExTraceGlobals.setCalendarColorCallTracer == null)
				{
					ExTraceGlobals.setCalendarColorCallTracer = new Trace(ExTraceGlobals.componentGuid, 94);
				}
				return ExTraceGlobals.setCalendarColorCallTracer;
			}
		}

		public static Trace SetCalendarGroupOrderCallTracer
		{
			get
			{
				if (ExTraceGlobals.setCalendarGroupOrderCallTracer == null)
				{
					ExTraceGlobals.setCalendarGroupOrderCallTracer = new Trace(ExTraceGlobals.componentGuid, 95);
				}
				return ExTraceGlobals.setCalendarGroupOrderCallTracer;
			}
		}

		public static Trace CreateCalendarGroupCallTracer
		{
			get
			{
				if (ExTraceGlobals.createCalendarGroupCallTracer == null)
				{
					ExTraceGlobals.createCalendarGroupCallTracer = new Trace(ExTraceGlobals.componentGuid, 96);
				}
				return ExTraceGlobals.createCalendarGroupCallTracer;
			}
		}

		public static Trace MoveCalendarCallTracer
		{
			get
			{
				if (ExTraceGlobals.moveCalendarCallTracer == null)
				{
					ExTraceGlobals.moveCalendarCallTracer = new Trace(ExTraceGlobals.componentGuid, 97);
				}
				return ExTraceGlobals.moveCalendarCallTracer;
			}
		}

		public static Trace GetFavoritesCallTracer
		{
			get
			{
				if (ExTraceGlobals.getFavoritesCallTracer == null)
				{
					ExTraceGlobals.getFavoritesCallTracer = new Trace(ExTraceGlobals.componentGuid, 98);
				}
				return ExTraceGlobals.getFavoritesCallTracer;
			}
		}

		public static Trace UpdateFavoriteFolderCallTracer
		{
			get
			{
				if (ExTraceGlobals.updateFavoriteFolderCallTracer == null)
				{
					ExTraceGlobals.updateFavoriteFolderCallTracer = new Trace(ExTraceGlobals.componentGuid, 99);
				}
				return ExTraceGlobals.updateFavoriteFolderCallTracer;
			}
		}

		public static Trace GetTimeZoneOffsetsCallTracer
		{
			get
			{
				if (ExTraceGlobals.getTimeZoneOffsetsCallTracer == null)
				{
					ExTraceGlobals.getTimeZoneOffsetsCallTracer = new Trace(ExTraceGlobals.componentGuid, 100);
				}
				return ExTraceGlobals.getTimeZoneOffsetsCallTracer;
			}
		}

		public static Trace AuthorizationTracer
		{
			get
			{
				if (ExTraceGlobals.authorizationTracer == null)
				{
					ExTraceGlobals.authorizationTracer = new Trace(ExTraceGlobals.componentGuid, 101);
				}
				return ExTraceGlobals.authorizationTracer;
			}
		}

		public static Trace SendCalendarSharingInviteCallTracer
		{
			get
			{
				if (ExTraceGlobals.sendCalendarSharingInviteCallTracer == null)
				{
					ExTraceGlobals.sendCalendarSharingInviteCallTracer = new Trace(ExTraceGlobals.componentGuid, 102);
				}
				return ExTraceGlobals.sendCalendarSharingInviteCallTracer;
			}
		}

		public static Trace GetCalendarSharingRecipientInfoCallTracer
		{
			get
			{
				if (ExTraceGlobals.getCalendarSharingRecipientInfoCallTracer == null)
				{
					ExTraceGlobals.getCalendarSharingRecipientInfoCallTracer = new Trace(ExTraceGlobals.componentGuid, 103);
				}
				return ExTraceGlobals.getCalendarSharingRecipientInfoCallTracer;
			}
		}

		public static Trace AddSharedCalendarCallTracer
		{
			get
			{
				if (ExTraceGlobals.addSharedCalendarCallTracer == null)
				{
					ExTraceGlobals.addSharedCalendarCallTracer = new Trace(ExTraceGlobals.componentGuid, 104);
				}
				return ExTraceGlobals.addSharedCalendarCallTracer;
			}
		}

		public static Trace FindPeopleCallTracer
		{
			get
			{
				if (ExTraceGlobals.findPeopleCallTracer == null)
				{
					ExTraceGlobals.findPeopleCallTracer = new Trace(ExTraceGlobals.componentGuid, 105);
				}
				return ExTraceGlobals.findPeopleCallTracer;
			}
		}

		public static Trace FindPlacesCallTracer
		{
			get
			{
				if (ExTraceGlobals.findPlacesCallTracer == null)
				{
					ExTraceGlobals.findPlacesCallTracer = new Trace(ExTraceGlobals.componentGuid, 106);
				}
				return ExTraceGlobals.findPlacesCallTracer;
			}
		}

		public static Trace UserPhotosTracer
		{
			get
			{
				if (ExTraceGlobals.userPhotosTracer == null)
				{
					ExTraceGlobals.userPhotosTracer = new Trace(ExTraceGlobals.componentGuid, 107);
				}
				return ExTraceGlobals.userPhotosTracer;
			}
		}

		public static Trace GetPersonaCallTracer
		{
			get
			{
				if (ExTraceGlobals.getPersonaCallTracer == null)
				{
					ExTraceGlobals.getPersonaCallTracer = new Trace(ExTraceGlobals.componentGuid, 108);
				}
				return ExTraceGlobals.getPersonaCallTracer;
			}
		}

		public static Trace GetExtensibilityContextCallTracer
		{
			get
			{
				if (ExTraceGlobals.getExtensibilityContextCallTracer == null)
				{
					ExTraceGlobals.getExtensibilityContextCallTracer = new Trace(ExTraceGlobals.componentGuid, 109);
				}
				return ExTraceGlobals.getExtensibilityContextCallTracer;
			}
		}

		public static Trace SubscribeInternalCalendarCallTracer
		{
			get
			{
				if (ExTraceGlobals.subscribeInternalCalendarCallTracer == null)
				{
					ExTraceGlobals.subscribeInternalCalendarCallTracer = new Trace(ExTraceGlobals.componentGuid, 110);
				}
				return ExTraceGlobals.subscribeInternalCalendarCallTracer;
			}
		}

		public static Trace SubscribeInternetCalendarCallTracer
		{
			get
			{
				if (ExTraceGlobals.subscribeInternetCalendarCallTracer == null)
				{
					ExTraceGlobals.subscribeInternetCalendarCallTracer = new Trace(ExTraceGlobals.componentGuid, 111);
				}
				return ExTraceGlobals.subscribeInternetCalendarCallTracer;
			}
		}

		public static Trace GetUserAvailabilityInternalCallTracer
		{
			get
			{
				if (ExTraceGlobals.getUserAvailabilityInternalCallTracer == null)
				{
					ExTraceGlobals.getUserAvailabilityInternalCallTracer = new Trace(ExTraceGlobals.componentGuid, 112);
				}
				return ExTraceGlobals.getUserAvailabilityInternalCallTracer;
			}
		}

		public static Trace ApplyConversationActionCallTracer
		{
			get
			{
				if (ExTraceGlobals.applyConversationActionCallTracer == null)
				{
					ExTraceGlobals.applyConversationActionCallTracer = new Trace(ExTraceGlobals.componentGuid, 113);
				}
				return ExTraceGlobals.applyConversationActionCallTracer;
			}
		}

		public static Trace GetCalendarSharingPermissionsCallTracer
		{
			get
			{
				if (ExTraceGlobals.getCalendarSharingPermissionsCallTracer == null)
				{
					ExTraceGlobals.getCalendarSharingPermissionsCallTracer = new Trace(ExTraceGlobals.componentGuid, 114);
				}
				return ExTraceGlobals.getCalendarSharingPermissionsCallTracer;
			}
		}

		public static Trace SetCalendarSharingPermissionsCallTracer
		{
			get
			{
				if (ExTraceGlobals.setCalendarSharingPermissionsCallTracer == null)
				{
					ExTraceGlobals.setCalendarSharingPermissionsCallTracer = new Trace(ExTraceGlobals.componentGuid, 115);
				}
				return ExTraceGlobals.setCalendarSharingPermissionsCallTracer;
			}
		}

		public static Trace SetCalendarPublishingCallTracer
		{
			get
			{
				if (ExTraceGlobals.setCalendarPublishingCallTracer == null)
				{
					ExTraceGlobals.setCalendarPublishingCallTracer = new Trace(ExTraceGlobals.componentGuid, 116);
				}
				return ExTraceGlobals.setCalendarPublishingCallTracer;
			}
		}

		public static Trace UCSTracer
		{
			get
			{
				if (ExTraceGlobals.uCSTracer == null)
				{
					ExTraceGlobals.uCSTracer = new Trace(ExTraceGlobals.componentGuid, 117);
				}
				return ExTraceGlobals.uCSTracer;
			}
		}

		public static Trace GetTaskFoldersCallTracer
		{
			get
			{
				if (ExTraceGlobals.getTaskFoldersCallTracer == null)
				{
					ExTraceGlobals.getTaskFoldersCallTracer = new Trace(ExTraceGlobals.componentGuid, 118);
				}
				return ExTraceGlobals.getTaskFoldersCallTracer;
			}
		}

		public static Trace CreateTaskFolderCallTracer
		{
			get
			{
				if (ExTraceGlobals.createTaskFolderCallTracer == null)
				{
					ExTraceGlobals.createTaskFolderCallTracer = new Trace(ExTraceGlobals.componentGuid, 119);
				}
				return ExTraceGlobals.createTaskFolderCallTracer;
			}
		}

		public static Trace RenameTaskFolderCallTracer
		{
			get
			{
				if (ExTraceGlobals.renameTaskFolderCallTracer == null)
				{
					ExTraceGlobals.renameTaskFolderCallTracer = new Trace(ExTraceGlobals.componentGuid, 120);
				}
				return ExTraceGlobals.renameTaskFolderCallTracer;
			}
		}

		public static Trace DeleteTaskFolderCallTracer
		{
			get
			{
				if (ExTraceGlobals.deleteTaskFolderCallTracer == null)
				{
					ExTraceGlobals.deleteTaskFolderCallTracer = new Trace(ExTraceGlobals.componentGuid, 121);
				}
				return ExTraceGlobals.deleteTaskFolderCallTracer;
			}
		}

		public static Trace MasterCategoryListCallTracer
		{
			get
			{
				if (ExTraceGlobals.masterCategoryListCallTracer == null)
				{
					ExTraceGlobals.masterCategoryListCallTracer = new Trace(ExTraceGlobals.componentGuid, 122);
				}
				return ExTraceGlobals.masterCategoryListCallTracer;
			}
		}

		public static Trace GetCalendarFolderConfigurationCallTracer
		{
			get
			{
				if (ExTraceGlobals.getCalendarFolderConfigurationCallTracer == null)
				{
					ExTraceGlobals.getCalendarFolderConfigurationCallTracer = new Trace(ExTraceGlobals.componentGuid, 123);
				}
				return ExTraceGlobals.getCalendarFolderConfigurationCallTracer;
			}
		}

		public static Trace OnlineMeetingTracer
		{
			get
			{
				if (ExTraceGlobals.onlineMeetingTracer == null)
				{
					ExTraceGlobals.onlineMeetingTracer = new Trace(ExTraceGlobals.componentGuid, 124);
				}
				return ExTraceGlobals.onlineMeetingTracer;
			}
		}

		public static Trace ModernGroupsTracer
		{
			get
			{
				if (ExTraceGlobals.modernGroupsTracer == null)
				{
					ExTraceGlobals.modernGroupsTracer = new Trace(ExTraceGlobals.componentGuid, 125);
				}
				return ExTraceGlobals.modernGroupsTracer;
			}
		}

		public static Trace CreateUnifiedMailboxTracer
		{
			get
			{
				if (ExTraceGlobals.createUnifiedMailboxTracer == null)
				{
					ExTraceGlobals.createUnifiedMailboxTracer = new Trace(ExTraceGlobals.componentGuid, 126);
				}
				return ExTraceGlobals.createUnifiedMailboxTracer;
			}
		}

		public static Trace AddAggregatedAccountTracer
		{
			get
			{
				if (ExTraceGlobals.addAggregatedAccountTracer == null)
				{
					ExTraceGlobals.addAggregatedAccountTracer = new Trace(ExTraceGlobals.componentGuid, 127);
				}
				return ExTraceGlobals.addAggregatedAccountTracer;
			}
		}

		public static Trace RemindersTracer
		{
			get
			{
				if (ExTraceGlobals.remindersTracer == null)
				{
					ExTraceGlobals.remindersTracer = new Trace(ExTraceGlobals.componentGuid, 128);
				}
				return ExTraceGlobals.remindersTracer;
			}
		}

		public static Trace GetAggregatedAccountTracer
		{
			get
			{
				if (ExTraceGlobals.getAggregatedAccountTracer == null)
				{
					ExTraceGlobals.getAggregatedAccountTracer = new Trace(ExTraceGlobals.componentGuid, 129);
				}
				return ExTraceGlobals.getAggregatedAccountTracer;
			}
		}

		public static Trace RemoveAggregatedAccountTracer
		{
			get
			{
				if (ExTraceGlobals.removeAggregatedAccountTracer == null)
				{
					ExTraceGlobals.removeAggregatedAccountTracer = new Trace(ExTraceGlobals.componentGuid, 130);
				}
				return ExTraceGlobals.removeAggregatedAccountTracer;
			}
		}

		public static Trace SetAggregatedAccountTracer
		{
			get
			{
				if (ExTraceGlobals.setAggregatedAccountTracer == null)
				{
					ExTraceGlobals.setAggregatedAccountTracer = new Trace(ExTraceGlobals.componentGuid, 131);
				}
				return ExTraceGlobals.setAggregatedAccountTracer;
			}
		}

		public static Trace WeatherTracer
		{
			get
			{
				if (ExTraceGlobals.weatherTracer == null)
				{
					ExTraceGlobals.weatherTracer = new Trace(ExTraceGlobals.componentGuid, 132);
				}
				return ExTraceGlobals.weatherTracer;
			}
		}

		public static Trace FederatedDirectoryTracer
		{
			get
			{
				if (ExTraceGlobals.federatedDirectoryTracer == null)
				{
					ExTraceGlobals.federatedDirectoryTracer = new Trace(ExTraceGlobals.componentGuid, 133);
				}
				return ExTraceGlobals.federatedDirectoryTracer;
			}
		}

		public static Trace GetPeopleIKnowGraphCallTracer
		{
			get
			{
				if (ExTraceGlobals.getPeopleIKnowGraphCallTracer == null)
				{
					ExTraceGlobals.getPeopleIKnowGraphCallTracer = new Trace(ExTraceGlobals.componentGuid, 134);
				}
				return ExTraceGlobals.getPeopleIKnowGraphCallTracer;
			}
		}

		public static Trace AddEventToMyCalendarTracer
		{
			get
			{
				if (ExTraceGlobals.addEventToMyCalendarTracer == null)
				{
					ExTraceGlobals.addEventToMyCalendarTracer = new Trace(ExTraceGlobals.componentGuid, 135);
				}
				return ExTraceGlobals.addEventToMyCalendarTracer;
			}
		}

		public static Trace ConversationAggregationTracer
		{
			get
			{
				if (ExTraceGlobals.conversationAggregationTracer == null)
				{
					ExTraceGlobals.conversationAggregationTracer = new Trace(ExTraceGlobals.componentGuid, 136);
				}
				return ExTraceGlobals.conversationAggregationTracer;
			}
		}

		public static Trace IsOffice365DomainTracer
		{
			get
			{
				if (ExTraceGlobals.isOffice365DomainTracer == null)
				{
					ExTraceGlobals.isOffice365DomainTracer = new Trace(ExTraceGlobals.componentGuid, 137);
				}
				return ExTraceGlobals.isOffice365DomainTracer;
			}
		}

		public static Trace RefreshGALContactsFolderTracer
		{
			get
			{
				if (ExTraceGlobals.refreshGALContactsFolderTracer == null)
				{
					ExTraceGlobals.refreshGALContactsFolderTracer = new Trace(ExTraceGlobals.componentGuid, 138);
				}
				return ExTraceGlobals.refreshGALContactsFolderTracer;
			}
		}

		public static Trace OptionsTracer
		{
			get
			{
				if (ExTraceGlobals.optionsTracer == null)
				{
					ExTraceGlobals.optionsTracer = new Trace(ExTraceGlobals.componentGuid, 139);
				}
				return ExTraceGlobals.optionsTracer;
			}
		}

		public static Trace OpenTenantManagerTracer
		{
			get
			{
				if (ExTraceGlobals.openTenantManagerTracer == null)
				{
					ExTraceGlobals.openTenantManagerTracer = new Trace(ExTraceGlobals.componentGuid, 140);
				}
				return ExTraceGlobals.openTenantManagerTracer;
			}
		}

		public static Trace MarkAllItemsAsReadTracer
		{
			get
			{
				if (ExTraceGlobals.markAllItemsAsReadTracer == null)
				{
					ExTraceGlobals.markAllItemsAsReadTracer = new Trace(ExTraceGlobals.componentGuid, 141);
				}
				return ExTraceGlobals.markAllItemsAsReadTracer;
			}
		}

		public static Trace GetConversationItemsTracer
		{
			get
			{
				if (ExTraceGlobals.getConversationItemsTracer == null)
				{
					ExTraceGlobals.getConversationItemsTracer = new Trace(ExTraceGlobals.componentGuid, 142);
				}
				return ExTraceGlobals.getConversationItemsTracer;
			}
		}

		public static Trace GetLikersTracer
		{
			get
			{
				if (ExTraceGlobals.getLikersTracer == null)
				{
					ExTraceGlobals.getLikersTracer = new Trace(ExTraceGlobals.componentGuid, 143);
				}
				return ExTraceGlobals.getLikersTracer;
			}
		}

		public static Trace GetUserUnifiedGroupsTracer
		{
			get
			{
				if (ExTraceGlobals.getUserUnifiedGroupsTracer == null)
				{
					ExTraceGlobals.getUserUnifiedGroupsTracer = new Trace(ExTraceGlobals.componentGuid, 144);
				}
				return ExTraceGlobals.getUserUnifiedGroupsTracer;
			}
		}

		public static Trace PeopleICommunicateWithTracer
		{
			get
			{
				if (ExTraceGlobals.peopleICommunicateWithTracer == null)
				{
					ExTraceGlobals.peopleICommunicateWithTracer = new Trace(ExTraceGlobals.componentGuid, 145);
				}
				return ExTraceGlobals.peopleICommunicateWithTracer;
			}
		}

		public static Trace SyncPersonaContactsBaseTracer
		{
			get
			{
				if (ExTraceGlobals.syncPersonaContactsBaseTracer == null)
				{
					ExTraceGlobals.syncPersonaContactsBaseTracer = new Trace(ExTraceGlobals.componentGuid, 146);
				}
				return ExTraceGlobals.syncPersonaContactsBaseTracer;
			}
		}

		public static Trace SyncAutoCompleteRecipientsTracer
		{
			get
			{
				if (ExTraceGlobals.syncAutoCompleteRecipientsTracer == null)
				{
					ExTraceGlobals.syncAutoCompleteRecipientsTracer = new Trace(ExTraceGlobals.componentGuid, 147);
				}
				return ExTraceGlobals.syncAutoCompleteRecipientsTracer;
			}
		}

		private static Guid componentGuid = new Guid("9041df24-db8f-4561-9ce6-75ee8dc21732");

		private static Trace calendarAlgorithmTracer = null;

		private static Trace calendarDataTracer = null;

		private static Trace calendarCallTracer = null;

		private static Trace commonAlgorithmTracer = null;

		private static Trace folderAlgorithmTracer = null;

		private static Trace folderDataTracer = null;

		private static Trace folderCallTracer = null;

		private static Trace itemAlgorithmTracer = null;

		private static Trace itemDataTracer = null;

		private static Trace itemCallTracer = null;

		private static Trace exceptionTracer = null;

		private static Trace sessionCacheTracer = null;

		private static Trace exchangePrincipalCacheTracer = null;

		private static Trace searchTracer = null;

		private static Trace utilAlgorithmTracer = null;

		private static Trace utilDataTracer = null;

		private static Trace utilCallTracer = null;

		private static Trace serverToServerAuthZTracer = null;

		private static Trace serviceCommandBaseCallTracer = null;

		private static Trace serviceCommandBaseDataTracer = null;

		private static Trace facadeBaseCallTracer = null;

		private static Trace createItemCallTracer = null;

		private static Trace getItemCallTracer = null;

		private static Trace updateItemCallTracer = null;

		private static Trace deleteItemCallTracer = null;

		private static Trace sendItemCallTracer = null;

		private static Trace moveCopyCommandBaseCallTracer = null;

		private static Trace moveCopyItemCommandBaseCallTracer = null;

		private static Trace copyItemCallTracer = null;

		private static Trace moveItemCallTracer = null;

		private static Trace createFolderCallTracer = null;

		private static Trace getFolderCallTracer = null;

		private static Trace updateFolderCallTracer = null;

		private static Trace deleteFolderCallTracer = null;

		private static Trace moveCopyFolderCommandBaseCallTracer = null;

		private static Trace copyFolderCallTracer = null;

		private static Trace moveFolderCallTracer = null;

		private static Trace findCommandBaseCallTracer = null;

		private static Trace findItemCallTracer = null;

		private static Trace findFolderCallTracer = null;

		private static Trace utilCommandBaseCallTracer = null;

		private static Trace expandDLCallTracer = null;

		private static Trace resolveNamesCallTracer = null;

		private static Trace subscribeCallTracer = null;

		private static Trace unsubscribeCallTracer = null;

		private static Trace getEventsCallTracer = null;

		private static Trace subscriptionsTracer = null;

		private static Trace subscriptionBaseTracer = null;

		private static Trace pushSubscriptionTracer = null;

		private static Trace syncFolderHierarchyCallTracer = null;

		private static Trace syncFolderItemsCallTracer = null;

		private static Trace synchronizationTracer = null;

		private static Trace performanceMonitorTracer = null;

		private static Trace convertIdCallTracer = null;

		private static Trace getDelegateCallTracer = null;

		private static Trace addDelegateCallTracer = null;

		private static Trace removeDelegateCallTracer = null;

		private static Trace updateDelegateCallTracer = null;

		private static Trace proxyEvaluatorTracer = null;

		private static Trace getMailTipsCallTracer = null;

		private static Trace allRequestsTracer = null;

		private static Trace authenticationTracer = null;

		private static Trace wCFTracer = null;

		private static Trace getUserConfigurationCallTracer = null;

		private static Trace createUserConfigurationCallTracer = null;

		private static Trace deleteUserConfigurationCallTracer = null;

		private static Trace updateUserConfigurationCallTracer = null;

		private static Trace throttlingTracer = null;

		private static Trace externalUserTracer = null;

		private static Trace getOrganizationConfigurationCallTracer = null;

		private static Trace getRoomsCallTracer = null;

		private static Trace getFederationInformationTracer = null;

		private static Trace participantLookupBatchingTracer = null;

		private static Trace allResponsesTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace getInboxRulesCallTracer = null;

		private static Trace updateInboxRulesCallTracer = null;

		private static Trace getCASMailboxTracer = null;

		private static Trace fastTransferTracer = null;

		private static Trace syncConversationCallTracer = null;

		private static Trace eLCTracer = null;

		private static Trace activityConverterTracer = null;

		private static Trace syncPeopleCallTracer = null;

		private static Trace getCalendarFoldersCallTracer = null;

		private static Trace getRemindersCallTracer = null;

		private static Trace syncCalendarCallTracer = null;

		private static Trace performReminderActionCallTracer = null;

		private static Trace provisionCallTracer = null;

		private static Trace renameCalendarGroupCallTracer = null;

		private static Trace deleteCalendarGroupCallTracer = null;

		private static Trace createCalendarCallTracer = null;

		private static Trace renameCalendarCallTracer = null;

		private static Trace deleteCalendarCallTracer = null;

		private static Trace setCalendarColorCallTracer = null;

		private static Trace setCalendarGroupOrderCallTracer = null;

		private static Trace createCalendarGroupCallTracer = null;

		private static Trace moveCalendarCallTracer = null;

		private static Trace getFavoritesCallTracer = null;

		private static Trace updateFavoriteFolderCallTracer = null;

		private static Trace getTimeZoneOffsetsCallTracer = null;

		private static Trace authorizationTracer = null;

		private static Trace sendCalendarSharingInviteCallTracer = null;

		private static Trace getCalendarSharingRecipientInfoCallTracer = null;

		private static Trace addSharedCalendarCallTracer = null;

		private static Trace findPeopleCallTracer = null;

		private static Trace findPlacesCallTracer = null;

		private static Trace userPhotosTracer = null;

		private static Trace getPersonaCallTracer = null;

		private static Trace getExtensibilityContextCallTracer = null;

		private static Trace subscribeInternalCalendarCallTracer = null;

		private static Trace subscribeInternetCalendarCallTracer = null;

		private static Trace getUserAvailabilityInternalCallTracer = null;

		private static Trace applyConversationActionCallTracer = null;

		private static Trace getCalendarSharingPermissionsCallTracer = null;

		private static Trace setCalendarSharingPermissionsCallTracer = null;

		private static Trace setCalendarPublishingCallTracer = null;

		private static Trace uCSTracer = null;

		private static Trace getTaskFoldersCallTracer = null;

		private static Trace createTaskFolderCallTracer = null;

		private static Trace renameTaskFolderCallTracer = null;

		private static Trace deleteTaskFolderCallTracer = null;

		private static Trace masterCategoryListCallTracer = null;

		private static Trace getCalendarFolderConfigurationCallTracer = null;

		private static Trace onlineMeetingTracer = null;

		private static Trace modernGroupsTracer = null;

		private static Trace createUnifiedMailboxTracer = null;

		private static Trace addAggregatedAccountTracer = null;

		private static Trace remindersTracer = null;

		private static Trace getAggregatedAccountTracer = null;

		private static Trace removeAggregatedAccountTracer = null;

		private static Trace setAggregatedAccountTracer = null;

		private static Trace weatherTracer = null;

		private static Trace federatedDirectoryTracer = null;

		private static Trace getPeopleIKnowGraphCallTracer = null;

		private static Trace addEventToMyCalendarTracer = null;

		private static Trace conversationAggregationTracer = null;

		private static Trace isOffice365DomainTracer = null;

		private static Trace refreshGALContactsFolderTracer = null;

		private static Trace optionsTracer = null;

		private static Trace openTenantManagerTracer = null;

		private static Trace markAllItemsAsReadTracer = null;

		private static Trace getConversationItemsTracer = null;

		private static Trace getLikersTracer = null;

		private static Trace getUserUnifiedGroupsTracer = null;

		private static Trace peopleICommunicateWithTracer = null;

		private static Trace syncPersonaContactsBaseTracer = null;

		private static Trace syncAutoCompleteRecipientsTracer = null;
	}
}
