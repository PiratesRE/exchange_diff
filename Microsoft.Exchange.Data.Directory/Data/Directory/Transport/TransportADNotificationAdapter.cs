using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Directory.Transport
{
	internal class TransportADNotificationAdapter
	{
		private event ADNotificationCallback AcceptedDomainDeleted;

		private event ADNotificationCallback ADSiteLinkDeleted;

		private event ADNotificationCallback DatabaseDeleted;

		private event ADNotificationCallback DeliveryAgentConnectorDeleted;

		private event ADNotificationCallback ForeignConnectorDeleted;

		private event ADNotificationCallback JournalRuleDeleted;

		private event ADNotificationCallback ReceiveConnectorDeleted;

		private event ADNotificationCallback RemoteDomainDeleted;

		private event ADNotificationCallback SmtpSendConnectorDeleted;

		private event ADNotificationCallback TransportRuleDeleted;

		private event ADNotificationCallback ExchangeServerDeleted;

		private event ADNotificationCallback InterceptorRuleDeleted;

		private TransportADNotificationAdapter()
		{
			this.needExplicitDeletedObjectSubscription = VariantConfiguration.InvariantNoFlightingSnapshot.Transport.ExplicitDeletedObjectNotifications.Enabled;
			if (!this.NeedExplicitDeletedObjectSubscription)
			{
				return;
			}
			this.ADSiteLinkDeleted += TransportADNotificationAdapter.EventLogDeleteNotification;
			this.AcceptedDomainDeleted += TransportADNotificationAdapter.EventLogDeleteNotification;
			this.DatabaseDeleted += TransportADNotificationAdapter.EventLogDeleteNotification;
			this.DeliveryAgentConnectorDeleted += TransportADNotificationAdapter.EventLogDeleteNotification;
			this.ForeignConnectorDeleted += TransportADNotificationAdapter.EventLogDeleteNotification;
			this.JournalRuleDeleted += TransportADNotificationAdapter.EventLogDeleteNotification;
			this.ReceiveConnectorDeleted += TransportADNotificationAdapter.EventLogDeleteNotification;
			this.RemoteDomainDeleted += TransportADNotificationAdapter.EventLogDeleteNotification;
			this.SmtpSendConnectorDeleted += TransportADNotificationAdapter.EventLogDeleteNotification;
			this.TransportRuleDeleted += TransportADNotificationAdapter.EventLogDeleteNotification;
			this.ExchangeServerDeleted += TransportADNotificationAdapter.EventLogDeleteNotification;
			this.InterceptorRuleDeleted += TransportADNotificationAdapter.EventLogDeleteNotification;
		}

		public static TransportADNotificationAdapter Instance
		{
			get
			{
				return TransportADNotificationAdapter.singletonInstance;
			}
		}

		public static ADNotificationRequestCookie RegisterForNonDeletedNotifications<TConfig>(ADObjectId baseDN, ADNotificationCallback callback) where TConfig : ADConfigurationObject, new()
		{
			return ADNotificationAdapter.RegisterChangeNotification<TConfig>(baseDN, callback);
		}

		public static ADOperationResult TryRegisterNotifications(Func<ADObjectId> baseDNGetter, ADNotificationCallback callback, TransportADNotificationAdapter.TransportADNotificationRegister registerDelegate, int retryCount, out ADNotificationRequestCookie cookie)
		{
			ADNotificationRequestCookie adCookie = null;
			ADOperationResult result = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ADObjectId root = (baseDNGetter == null) ? null : baseDNGetter();
				adCookie = registerDelegate(root, callback);
			}, retryCount);
			cookie = adCookie;
			return result;
		}

		public void RegisterForMsExchangeTransportServiceDeletedEvents()
		{
			if (!this.NeedExplicitDeletedObjectSubscription)
			{
				return;
			}
			ADObjectId rootOrgContainerIdForLocalForest = TransportADNotificationAdapter.GetRootOrgContainerIdForLocalForest();
			ADObjectId childId = rootOrgContainerIdForLocalForest.GetChildId("Administrative Groups").GetChildId(AdministrativeGroup.DefaultName);
			ADObjectId childId2 = childId.GetChildId(ServersContainer.DefaultName);
			ADObjectId childId3 = childId2.GetChildId(Environment.MachineName).GetChildId(ProtocolsContainer.DefaultName).GetChildId(ReceiveConnector.DefaultName);
			this.RegisterChangeNotificationForDeletedObject<Server>(childId2, new ADNotificationCallback(this.HandleExchangeServerDeleted));
			this.RegisterChangeNotificationForDeletedObject<ReceiveConnector>(childId3, new ADNotificationCallback(this.HandleReceiveConnectorDeleted));
		}

		public void RegisterForEdgeTransportEvents()
		{
			if (!this.NeedExplicitDeletedObjectSubscription)
			{
				return;
			}
			ADObjectId rootOrgContainerIdForLocalForest = TransportADNotificationAdapter.GetRootOrgContainerIdForLocalForest();
			ADObjectId childId = rootOrgContainerIdForLocalForest.GetChildId("Administrative Groups").GetChildId(AdministrativeGroup.DefaultName);
			ADObjectId childId2 = rootOrgContainerIdForLocalForest.GetChildId(AcceptedDomain.AcceptedDomainContainer.Parent.Name);
			ADObjectId childId3 = childId.GetChildId(ServersContainer.DefaultName);
			ADObjectId childId4 = childId2.GetChildId(AcceptedDomain.AcceptedDomainContainer.Name);
			ADObjectId childId5 = childId3.GetChildId(Environment.MachineName).GetChildId(ProtocolsContainer.DefaultName).GetChildId(ReceiveConnector.DefaultName);
			ADObjectId childId6 = rootOrgContainerIdForLocalForest.GetChildId("Global Settings").GetChildId("Internet Message Formats");
			ADObjectId childId7 = childId.GetChildId(DatabasesContainer.DefaultName);
			ADObjectId childId8 = childId2.GetChildId("Rules").GetChildId("TransportVersioned");
			ADObjectId childId9 = childId2.GetChildId("Rules").GetChildId("JournalingVersioned");
			ADObjectId descendantId = rootOrgContainerIdForLocalForest.GetDescendantId(InterceptorRule.InterceptorRulesContainer);
			ADObjectId childId10 = ADSession.GetConfigurationNamingContextForLocalForest().GetChildId(SitesContainer.DefaultName);
			ADObjectId childId11 = childId10.GetChildId("Inter-Site Transports").GetChildId("IP");
			ADObjectId childId12 = childId.GetChildId(RoutingGroupsContainer.DefaultName).GetChildId(RoutingGroup.DefaultName).GetChildId("Connections");
			ADObjectId parentContainerId = childId12;
			ADObjectId parentContainerId2 = childId12;
			this.RegisterChangeNotificationForDeletedObject<AcceptedDomain>(childId4, new ADNotificationCallback(this.HandleAcceptedDomainDeleted));
			this.RegisterChangeNotificationForDeletedObject<ADSiteLink>(childId11, new ADNotificationCallback(this.HandleADSiteLinkDeleted));
			this.RegisterChangeNotificationForDeletedObject<DeliveryAgentConnector>(parentContainerId, new ADNotificationCallback(this.HandleDeliveryAgentConnectorDeleted));
			this.RegisterChangeNotificationForDeletedObject<DomainContentConfig>(childId6, new ADNotificationCallback(this.HandleRemoteDomainDeleted));
			this.RegisterChangeNotificationForDeletedObject<ForeignConnector>(parentContainerId2, new ADNotificationCallback(this.HandleForeignConnectorDeleted));
			this.RegisterChangeNotificationForDeletedObject<MailboxDatabase>(childId7, new ADNotificationCallback(this.HandleDatabaseDeleted));
			this.RegisterChangeNotificationForDeletedObject<ReceiveConnector>(childId5, new ADNotificationCallback(this.HandleReceiveConnectorDeleted));
			this.RegisterChangeNotificationForDeletedObject<Server>(childId3, new ADNotificationCallback(this.HandleExchangeServerDeleted));
			this.RegisterChangeNotificationForDeletedObject<SmtpSendConnectorConfig>(childId12, new ADNotificationCallback(this.HandleSmtpSendConnectorDeleted));
			this.RegisterChangeNotificationForDeletedObject<TransportRule>(childId8, new ADNotificationCallback(this.HandleTransportRuleDeleted));
			this.RegisterChangeNotificationForDeletedObject<TransportRule>(childId9, new ADNotificationCallback(this.HandleJournalRuleDeleted));
			this.RegisterChangeNotificationForDeletedObject<InterceptorRule>(descendantId, new ADNotificationCallback(this.HandleInterceptorRuleDeleted));
		}

		public void RegisterForSubmissionServiceEvents()
		{
			if (!this.NeedExplicitDeletedObjectSubscription)
			{
				return;
			}
			ADObjectId descendantId = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest().GetDescendantId(InterceptorRule.InterceptorRulesContainer);
			this.RegisterChangeNotificationForDeletedObject<InterceptorRule>(descendantId, new ADNotificationCallback(this.HandleInterceptorRuleDeleted));
		}

		public ADNotificationRequestCookie RegisterForExchangeServerNotifications(ADObjectId baseDN, ADNotificationCallback callback)
		{
			ADNotificationRequestCookie result = ADNotificationAdapter.RegisterChangeNotification<Server>(baseDN, callback);
			if (this.NeedExplicitDeletedObjectSubscription)
			{
				this.VerifyRootWasRegistered<Server>();
			}
			return result;
		}

		public ADNotificationRequestCookie RegisterForLocalServerReceiveConnectorNotifications(ADObjectId baseDN, ADNotificationCallback callback)
		{
			ADNotificationRequestCookie result = ADNotificationAdapter.RegisterChangeNotification<ReceiveConnector>(baseDN, callback);
			if (this.NeedExplicitDeletedObjectSubscription)
			{
				this.VerifyRootWasRegistered<ReceiveConnector>();
			}
			return result;
		}

		public ADNotificationRequestCookie RegisterForMailGatewayNotifications(ADObjectId rootId, ADNotificationCallback callback)
		{
			ADNotificationRequestCookie result = ADNotificationAdapter.RegisterChangeNotification<MailGateway>(rootId, callback);
			if (this.NeedExplicitDeletedObjectSubscription)
			{
				this.VerifyRootWasRegistered<SmtpSendConnectorConfig>();
				this.VerifyRootWasRegistered<DeliveryAgentConnector>();
				this.VerifyRootWasRegistered<ForeignConnector>();
			}
			return result;
		}

		public ADNotificationRequestCookie RegisterForRemoteDomainNotifications(ADObjectId baseDN, ADNotificationCallback callback)
		{
			ADNotificationRequestCookie result = ADNotificationAdapter.RegisterChangeNotification<DomainContentConfig>(baseDN, callback);
			if (this.NeedExplicitDeletedObjectSubscription)
			{
				this.VerifyRootWasRegistered<DomainContentConfig>();
			}
			return result;
		}

		public ADNotificationRequestCookie RegisterForTransportRuleNotifications(ADObjectId baseDN, ADNotificationCallback callback)
		{
			ADNotificationRequestCookie result = ADNotificationAdapter.RegisterChangeNotification<TransportRule>(baseDN, callback);
			if (this.NeedExplicitDeletedObjectSubscription)
			{
				this.VerifyRootWasRegistered<TransportRule>();
			}
			return result;
		}

		public ADNotificationRequestCookie RegisterForADSiteNotifications(ADObjectId baseDN, ADNotificationCallback callback)
		{
			return ADNotificationAdapter.RegisterChangeNotification<ADSite>(baseDN, callback);
		}

		public ADNotificationRequestCookie RegisterForADSiteLinkNotifications(ADObjectId baseDN, ADNotificationCallback callback)
		{
			ADNotificationRequestCookie result = ADNotificationAdapter.RegisterChangeNotification<ADSiteLink>(baseDN, callback);
			if (this.NeedExplicitDeletedObjectSubscription)
			{
				this.VerifyRootWasRegistered<ADSiteLink>();
			}
			return result;
		}

		public ADNotificationRequestCookie RegisterForDatabaseNotifications(ADObjectId baseDN, ADNotificationCallback callback)
		{
			ADNotificationRequestCookie result = ADNotificationAdapter.RegisterChangeNotification<MailboxDatabase>(baseDN, callback);
			if (this.NeedExplicitDeletedObjectSubscription)
			{
				this.VerifyRootWasRegistered<MailboxDatabase>();
			}
			return result;
		}

		public ADNotificationRequestCookie RegisterForAcceptedDomainNotifications(ADObjectId baseDN, ADNotificationCallback callback)
		{
			ADNotificationRequestCookie result = ADNotificationAdapter.RegisterChangeNotification<AcceptedDomain>(baseDN, callback);
			if (this.NeedExplicitDeletedObjectSubscription)
			{
				this.VerifyRootWasRegistered<AcceptedDomain>();
			}
			return result;
		}

		public ADNotificationRequestCookie RegisterForJournalRuleNotifications(ADObjectId baseDN, ADNotificationCallback callback)
		{
			ADNotificationRequestCookie result = ADNotificationAdapter.RegisterChangeNotification<TransportRule>(baseDN, callback);
			if (this.NeedExplicitDeletedObjectSubscription)
			{
				this.VerifyRootWasRegistered<TransportRule>();
			}
			return result;
		}

		public void UnregisterChangeNotification(ADNotificationRequestCookie cookie)
		{
			ADNotificationAdapter.UnregisterChangeNotification(cookie);
		}

		private static void EventLogDeleteNotification(ADNotificationEventArgs args)
		{
			if (args.ChangeType != ADNotificationChangeType.Delete)
			{
				return;
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_TransportDeletedADNotificationReceived, null, new object[]
			{
				args.ChangeType.ToString(),
				args.Context,
				args.Id,
				args.LastKnownParent,
				args.Type
			});
		}

		private static ADObjectId GetRootOrgContainerIdForLocalForest()
		{
			ADObjectId rootOrgContainerIdForLocalForest = null;
			ADNotificationAdapter.RunADOperation(delegate()
			{
				rootOrgContainerIdForLocalForest = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
			});
			return rootOrgContainerIdForLocalForest;
		}

		private void RegisterChangeNotificationForDeletedObject<TConfigObject>(ADObjectId parentContainerId, ADNotificationCallback adNotificationCallback) where TConfigObject : ADConfigurationObject, new()
		{
			TConfigObject tconfigObject = Activator.CreateInstance<TConfigObject>();
			ADNotificationRequest request = new ADNotificationRequest(tconfigObject.GetType(), tconfigObject.MostDerivedObjectClass, parentContainerId, adNotificationCallback, null);
			ADNotificationListener.RegisterChangeNotificationForDeletedObjects(request);
			this.typesRegisteredForDeleteNotifications.Add(tconfigObject.GetType());
		}

		private void VerifyRootWasRegistered<TConfigObject>() where TConfigObject : ADConfigurationObject, new()
		{
			if (!this.typesRegisteredForDeleteNotifications.Contains(typeof(TConfigObject)))
			{
				throw new InvalidOperationException(string.Format("Type '{0}' is not registered to receive deleted notifications.", typeof(TConfigObject)));
			}
		}

		private void HandleExchangeServerDeleted(ADNotificationEventArgs args)
		{
			if (this.ExchangeServerDeleted != null)
			{
				this.ExchangeServerDeleted(args);
			}
		}

		private void HandleAcceptedDomainDeleted(ADNotificationEventArgs args)
		{
			if (this.AcceptedDomainDeleted != null)
			{
				this.AcceptedDomainDeleted(args);
			}
		}

		private void HandleReceiveConnectorDeleted(ADNotificationEventArgs args)
		{
			if (this.ReceiveConnectorDeleted != null)
			{
				this.ReceiveConnectorDeleted(args);
			}
		}

		private void HandleRemoteDomainDeleted(ADNotificationEventArgs args)
		{
			if (this.RemoteDomainDeleted != null)
			{
				this.RemoteDomainDeleted(args);
			}
		}

		private void HandleTransportRuleDeleted(ADNotificationEventArgs args)
		{
			if (this.TransportRuleDeleted != null)
			{
				this.TransportRuleDeleted(args);
			}
		}

		private void HandleJournalRuleDeleted(ADNotificationEventArgs args)
		{
			if (this.JournalRuleDeleted != null)
			{
				this.JournalRuleDeleted(args);
			}
		}

		private void HandleInterceptorRuleDeleted(ADNotificationEventArgs args)
		{
			if (this.InterceptorRuleDeleted != null)
			{
				this.InterceptorRuleDeleted(args);
			}
		}

		private void HandleADSiteLinkDeleted(ADNotificationEventArgs args)
		{
			if (this.ADSiteLinkDeleted != null)
			{
				this.ADSiteLinkDeleted(args);
			}
		}

		private void HandleDatabaseDeleted(ADNotificationEventArgs args)
		{
			if (this.DatabaseDeleted != null)
			{
				this.DatabaseDeleted(args);
			}
		}

		private void HandleSmtpSendConnectorDeleted(ADNotificationEventArgs args)
		{
			if (this.SmtpSendConnectorDeleted != null)
			{
				this.SmtpSendConnectorDeleted(args);
			}
		}

		private void HandleDeliveryAgentConnectorDeleted(ADNotificationEventArgs args)
		{
			if (this.DeliveryAgentConnectorDeleted != null)
			{
				this.DeliveryAgentConnectorDeleted(args);
			}
		}

		private void HandleForeignConnectorDeleted(ADNotificationEventArgs args)
		{
			if (this.ForeignConnectorDeleted != null)
			{
				this.ForeignConnectorDeleted(args);
			}
		}

		private bool NeedExplicitDeletedObjectSubscription
		{
			get
			{
				return this.needExplicitDeletedObjectSubscription;
			}
		}

		private readonly bool needExplicitDeletedObjectSubscription;

		private static TransportADNotificationAdapter singletonInstance = new TransportADNotificationAdapter();

		private HashSet<Type> typesRegisteredForDeleteNotifications = new HashSet<Type>();

		public delegate ADNotificationRequestCookie TransportADNotificationRegister(ADObjectId root, ADNotificationCallback callback);
	}
}
