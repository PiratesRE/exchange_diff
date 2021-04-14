using System;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class ADStoreProvider : IStoreProvider
	{
		private ADStoreProvider()
		{
		}

		public static ADStoreProvider Instance
		{
			get
			{
				return ADStoreProvider.instance;
			}
		}

		public TObject FindByExchangeGuidIncludingAlternate<TObject>(Guid mailboxGuid, TenantPartitionHint tenantPartitionHint) where TObject : ADObject, new()
		{
			IRecipientSession tenantOrRootOrgRecipientSession;
			if (tenantPartitionHint == null)
			{
				tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 68, "FindByExchangeGuidIncludingAlternate", "f:\\15.00.1497\\sources\\dev\\MailboxTransport\\src\\MailboxTransportSubmission\\StoreDriverSubmission\\Providers\\ADStoreProvider.cs");
			}
			else
			{
				tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromTenantPartitionHint(tenantPartitionHint), 74, "FindByExchangeGuidIncludingAlternate", "f:\\15.00.1497\\sources\\dev\\MailboxTransport\\src\\MailboxTransportSubmission\\StoreDriverSubmission\\Providers\\ADStoreProvider.cs");
			}
			return tenantOrRootOrgRecipientSession.FindByExchangeGuidIncludingAlternate<TObject>(mailboxGuid);
		}

		public MailboxSession OpenStore(OrganizationId organizationId, string displayName, string mailboxFqdn, string mailboxServerDN, Guid mailboxGuid, Guid mdbGuid, MultiValuedProperty<CultureInfo> senderLocales, MultiValuedProperty<Guid> aggregatedMailboxGuids)
		{
			ExchangePrincipal mailboxOwner = ExchangePrincipal.FromMailboxData(ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), displayName, mailboxFqdn, mailboxServerDN, string.Empty, mailboxGuid, mdbGuid, string.Empty, senderLocales, aggregatedMailboxGuids);
			return MailboxSession.OpenAsTransport(mailboxOwner, "Client=HUB");
		}

		public PublicFolderSession OpenStore(OrganizationId organizationId, Guid mailboxGuid)
		{
			return PublicFolderSession.OpenAsTransport(organizationId, mailboxGuid);
		}

		public MessageItem GetMessageItem(StoreSession storeSession, StoreId storeId, StorePropertyDefinition[] contentConversionProperties)
		{
			return Item.BindAsMessage(storeSession, storeId, contentConversionProperties);
		}

		public Exception CallDoneWithMessageWithRetry(StoreSession session, MessageItem item, int retryCount, MailItemSubmitter context)
		{
			TraceHelper.MapiStoreDriverSubmissionTracer.TracePass(TraceHelper.MessageProbeActivityId, 0L, "Move message from outbox");
			Exception result = null;
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3689295165U);
			if (!session.Capabilities.CanSend)
			{
				string message = "Store session does not have CanSend capability, DoneWithMessage is not called.";
				TraceHelper.MapiStoreDriverSubmissionTracer.TraceFail(TraceHelper.MessageProbeActivityId, 0L, message);
				return new InvalidOperationException(message);
			}
			for (int i = 0; i < retryCount; i++)
			{
				ExDateTime dt = (context == null) ? default(ExDateTime) : ExDateTime.UtcNow;
				try
				{
					session.DoneWithMessage(item);
					return null;
				}
				catch (StoragePermanentException ex)
				{
					TraceHelper.MapiStoreDriverSubmissionTracer.TraceFail<StoragePermanentException>(TraceHelper.MessageProbeActivityId, 0L, "DoneWithMessage failed with permanent exception {0}", ex);
					result = ex;
				}
				catch (StorageTransientException ex2)
				{
					TraceHelper.MapiStoreDriverSubmissionTracer.TraceFail<int, StorageTransientException>(TraceHelper.MessageProbeActivityId, 0L, "DoneWithMessage failed at attempt {0} with transient exception {1}", i, ex2);
					result = ex2;
					if (retryCount - 1 > i)
					{
						Thread.Sleep(TimeSpan.FromMilliseconds((double)(i * 10)));
					}
				}
				finally
				{
					if (context != null)
					{
						TimeSpan additionalLatency = ExDateTime.UtcNow - dt;
						context.AddRpcLatency(additionalLatency, "Done with message");
					}
				}
			}
			return result;
		}

		public bool TryGetSendAsSubscription(MessageItem item, SendAsManager sendAsManager, out ISendAsSource subscription)
		{
			subscription = null;
			MailboxSession mailboxSession = item.Session as MailboxSession;
			object obj = item.TryGetProperty(MessageItemSchema.SharingInstanceGuid);
			if (PropertyError.IsPropertyNotFound(obj))
			{
				if (mailboxSession != null && mailboxSession.MailboxOwner != null && mailboxSession.MailboxOwner.MailboxInfo.IsAggregated)
				{
					TraceHelper.MapiStoreDriverSubmissionTracer.TracePass(TraceHelper.MessageProbeActivityId, 0L, "The message was submitted from an aggregated mailbox. There must be one associated send as subscription.");
					bool flag;
					if (!sendAsManager.TryGetSendAsSubscription(mailboxSession, out subscription, out flag))
					{
						TraceHelper.MapiStoreDriverSubmissionTracer.TraceFail(TraceHelper.MessageProbeActivityId, 0L, "Could not find a unique send as subscription.  Rejecting the message.");
						if (flag)
						{
							throw new SmtpResponseException(AckReason.AmbiguousSubscription);
						}
						throw new SmtpResponseException(AckReason.SubscriptionNotFound);
					}
				}
			}
			else
			{
				TraceHelper.MapiStoreDriverSubmissionTracer.TracePass(TraceHelper.MessageProbeActivityId, 0L, "The message has a subscription id on it. Subscription id: {0}", new object[]
				{
					obj
				});
				if (mailboxSession == null)
				{
					TraceHelper.MapiStoreDriverSubmissionTracer.TraceFail<Type>(TraceHelper.MessageProbeActivityId, 0L, "The session is not the right type. Actual type: {0}", item.Session.GetType());
					throw new SmtpResponseException(AckReason.UnrecognizedSendAsMessage);
				}
				if (!sendAsManager.TryGetSendAsSubscription(item, mailboxSession, out subscription))
				{
					TraceHelper.MapiStoreDriverSubmissionTracer.TraceFail(TraceHelper.MessageProbeActivityId, 0L, "The subscription could not be found. Rejecting the message. Subscription id: {0}", new object[]
					{
						obj
					});
					throw new SmtpResponseException(AckReason.SubscriptionNotFound);
				}
				if (!sendAsManager.IsSubscriptionEnabled(subscription))
				{
					TraceHelper.MapiStoreDriverSubmissionTracer.TraceFail(TraceHelper.MessageProbeActivityId, 0L, "The subscription is not enabled. Rejecting the message. Subscription id: {0}", new object[]
					{
						obj
					});
					throw new SmtpResponseException(AckReason.SubscriptionDisabled);
				}
				if (!sendAsManager.IsSendAsEnabled(subscription))
				{
					TraceHelper.MapiStoreDriverSubmissionTracer.TraceFail(TraceHelper.MessageProbeActivityId, 0L, "The subscription is not enabled for send as. Rejecting the message. Subscription id: {0}", new object[]
					{
						obj
					});
					throw new SmtpResponseException(AckReason.SubscriptionNotEnabledForSendAs);
				}
				if (!sendAsManager.IsValidSendAsMessage(subscription, item))
				{
					TraceHelper.MapiStoreDriverSubmissionTracer.TraceFail(TraceHelper.MessageProbeActivityId, 0L, "The message does not have valid sent representing properties. Rejecting the message. Subscription id: {0}", new object[]
					{
						obj
					});
					throw new SmtpResponseException(AckReason.InvalidSendAsProperties);
				}
			}
			return subscription != null;
		}

		private static ADStoreProvider instance = new ADStoreProvider();
	}
}
