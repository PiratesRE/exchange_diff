using System;
using System.Collections.Generic;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Rpc;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Notification;
using Microsoft.Exchange.Transport.Sync.Manager.Throttling;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SubscriptionNotificationProcessor
	{
		public SubscriptionNotificationProcessor() : this(ContentAggregationConfig.SyncLogSession, ExTraceGlobals.SubscriptionNotificationServerTracer)
		{
		}

		protected SubscriptionNotificationProcessor(SyncLogSession syncLogSession, Trace tracer)
		{
			this.syncLogSession = syncLogSession;
			this.tracer = tracer;
		}

		public virtual byte[] InvokeSubscriptionNotificationEndPoint(byte[] inputBlob)
		{
			MdbefPropertyCollection mdbefPropertyCollection = null;
			try
			{
				mdbefPropertyCollection = MdbefPropertyCollection.Create(inputBlob, 0, inputBlob.Length);
			}
			catch (MdbefException)
			{
				this.syncLogSession.LogInformation((TSLID)490UL, this.tracer, (long)this.GetHashCode(), "Subscription Notification RPC server will return {0} since input blob could not be parsed", new object[]
				{
					SubscriptionNotificationResult.PropertyBagError
				});
				return RpcHelper.CreateResponsePropertyCollection(2835349507U, 4);
			}
			SubscriptionNotificationRpcMethodCode subscriptionNotificationRpcMethodCode = (SubscriptionNotificationRpcMethodCode)((int)mdbefPropertyCollection[2684420099U]);
			byte[] result;
			switch (subscriptionNotificationRpcMethodCode)
			{
			case SubscriptionNotificationRpcMethodCode.SubscriptionAdd:
			case SubscriptionNotificationRpcMethodCode.SubscriptionDelete:
			{
				ExchangePrincipal mailboxOwner;
				if (!this.TryLoadMailboxOwner(mdbefPropertyCollection, out mailboxOwner, out result))
				{
					return result;
				}
				result = this.SubscriptionListChange(mailboxOwner);
				break;
			}
			case SubscriptionNotificationRpcMethodCode.SubscriptionSyncNowNeeded:
			case SubscriptionNotificationRpcMethodCode.SubscriptionUpdated:
			case SubscriptionNotificationRpcMethodCode.SubscriptionUpdatedAndSyncNowNeeded:
			{
				ExchangePrincipal mailboxOwner;
				if (!this.TryLoadMailboxOwner(mdbefPropertyCollection, out mailboxOwner, out result))
				{
					return result;
				}
				bool subscriptionUpdated = subscriptionNotificationRpcMethodCode == SubscriptionNotificationRpcMethodCode.SubscriptionUpdated || subscriptionNotificationRpcMethodCode == SubscriptionNotificationRpcMethodCode.SubscriptionUpdatedAndSyncNowNeeded;
				bool syncNowRequested = subscriptionNotificationRpcMethodCode == SubscriptionNotificationRpcMethodCode.SubscriptionSyncNowNeeded || subscriptionNotificationRpcMethodCode == SubscriptionNotificationRpcMethodCode.SubscriptionUpdatedAndSyncNowNeeded;
				result = this.ProcessSubscriptionUpdatedAndSyncNowRpcRequest(mdbefPropertyCollection, mailboxOwner, subscriptionUpdated, syncNowRequested);
				break;
			}
			case SubscriptionNotificationRpcMethodCode.OWALogonTriggeredSyncNow:
			case SubscriptionNotificationRpcMethodCode.OWAActivityTriggeredSyncNow:
			case SubscriptionNotificationRpcMethodCode.OWARefreshButtonTriggeredSyncNow:
				result = this.ProcessOwaTriggeredSyncNowRequest(mdbefPropertyCollection, subscriptionNotificationRpcMethodCode);
				break;
			default:
				this.syncLogSession.LogInformation((TSLID)1337UL, this.tracer, (long)this.GetHashCode(), "Subscription Notification RPC server will return {0} since an invalid method {1} was requested", new object[]
				{
					SubscriptionNotificationResult.UnknownMethodError,
					subscriptionNotificationRpcMethodCode
				});
				result = RpcHelper.CreateResponsePropertyCollection(2835349507U, 5);
				break;
			}
			return result;
		}

		protected virtual bool TryCreateMailboxOwner(string primarySmtpAddress, out ExchangePrincipal mailboxOwner, out byte[] errorCode)
		{
			mailboxOwner = null;
			errorCode = null;
			try
			{
				SmtpAddress smtpAddress = new SmtpAddress(primarySmtpAddress);
				ADSessionSettings adSettings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(smtpAddress.Domain);
				mailboxOwner = ExchangePrincipal.FromProxyAddress(adSettings, primarySmtpAddress);
			}
			catch (LocalizedException ex)
			{
				this.syncLogSession.LogError((TSLID)1372UL, this.tracer, "TryCreateMailboxOwner: {0} hit exception: {1}.", new object[]
				{
					primarySmtpAddress,
					ex
				});
				errorCode = RpcHelper.CreateResponsePropertyCollection(2835349507U, 3);
				return false;
			}
			return true;
		}

		protected virtual byte[] SubscriptionListChange(ExchangePrincipal mailboxOwner)
		{
			bool flag = DataAccessLayer.TryReportSubscriptionListChanged(mailboxOwner.MailboxInfo.MailboxGuid, mailboxOwner.MailboxInfo.GetDatabaseGuid());
			this.syncLogSession.LogDebugging((TSLID)1370UL, this.tracer, (long)this.GetHashCode(), "SubscriptionNotificationServer:SubscriptionListChange crawlEnqueued {0} returning success.", new object[]
			{
				flag
			});
			return SubscriptionNotificationProcessor.SuccessOutput;
		}

		protected virtual byte[] SubscriptionSyncNowNeeded(ExchangePrincipal mailboxOwner, Guid subscriptionId)
		{
			if (!DataAccessLayer.TryReportSyncNowNeeded(mailboxOwner.MailboxInfo.GetDatabaseGuid(), mailboxOwner.MailboxInfo.MailboxGuid, subscriptionId))
			{
				this.syncLogSession.LogDebugging((TSLID)1373UL, this.tracer, (long)this.GetHashCode(), "SubscriptionNotificationServer returning failed error code.", new object[0]);
				return RpcHelper.CreateResponsePropertyCollection(2835349507U, 3);
			}
			this.syncLogSession.LogDebugging((TSLID)1374UL, this.tracer, (long)this.GetHashCode(), "SubscriptionNotificationServer returning success error code.", new object[0]);
			return SubscriptionNotificationProcessor.SuccessOutput;
		}

		protected virtual bool TryReadSubscriptionsInformation(Guid mdbGuid, Guid mailboxGuid, out IDictionary<Guid, SubscriptionInformation> subscriptionsInformation)
		{
			return DataAccessLayer.TryReadSubscriptionsInformation(mdbGuid, mailboxGuid, out subscriptionsInformation);
		}

		private static WorkType GetOwaTriggeredSyncNowWorkTypeFromNotificationType(SubscriptionNotificationRpcMethodCode notificationType)
		{
			switch (notificationType)
			{
			case SubscriptionNotificationRpcMethodCode.OWALogonTriggeredSyncNow:
				return WorkType.OwaLogonTriggeredSyncNow;
			case SubscriptionNotificationRpcMethodCode.OWAActivityTriggeredSyncNow:
				return WorkType.OwaActivityTriggeredSyncNow;
			case SubscriptionNotificationRpcMethodCode.OWARefreshButtonTriggeredSyncNow:
				return WorkType.OwaRefreshButtonTriggeredSyncNow;
			default:
				throw new InvalidOperationException("No mapping found for notificationType:" + notificationType);
			}
		}

		private byte[] ProcessSubscriptionUpdatedAndSyncNowRpcRequest(MdbefPropertyCollection inputArgs, ExchangePrincipal mailboxOwner, bool subscriptionUpdated, bool syncNowRequested)
		{
			Guid subscriptionId;
			byte[] array;
			if (!this.TryExtractProperty<Guid>(inputArgs, (SubscriptionNotificationPropTag)2684551240U, out subscriptionId, out array))
			{
				return array;
			}
			if (subscriptionUpdated)
			{
				array = this.SubscriptionListChange(mailboxOwner);
				if (array != SubscriptionNotificationProcessor.SuccessOutput)
				{
					syncNowRequested = false;
				}
			}
			if (syncNowRequested)
			{
				array = this.SubscriptionSyncNowNeeded(mailboxOwner, subscriptionId);
			}
			return array;
		}

		private bool TryLoadMailboxOwner(MdbefPropertyCollection inputArgs, out ExchangePrincipal mailboxOwner, out byte[] errorCode)
		{
			mailboxOwner = null;
			string primarySmtpAddress;
			return this.TryExtractProperty<string>(inputArgs, (SubscriptionNotificationPropTag)2684616735U, out primarySmtpAddress, out errorCode) && this.TryCreateMailboxOwner(primarySmtpAddress, out mailboxOwner, out errorCode);
		}

		private bool TryLoadMailboxInformation(MdbefPropertyCollection inputArgs, out Guid mailboxGuid, out Guid mdbGuid, out byte[] errorCode)
		{
			mailboxGuid = Guid.Empty;
			mdbGuid = Guid.Empty;
			return this.TryExtractProperty<Guid>(inputArgs, (SubscriptionNotificationPropTag)2684682312U, out mailboxGuid, out errorCode) && this.TryExtractProperty<Guid>(inputArgs, (SubscriptionNotificationPropTag)2684747848U, out mdbGuid, out errorCode);
		}

		private bool TryExtractProperty<T>(MdbefPropertyCollection inputArgs, SubscriptionNotificationPropTag argName, out T outputVariable, out byte[] errorOutput)
		{
			string text;
			if (!RpcHelper.TryGetProperty<T>(inputArgs, (uint)argName, out outputVariable, out text))
			{
				this.syncLogSession.LogError((TSLID)1368UL, this.tracer, (long)this.GetHashCode(), "ServerVersion mismatch. Not found all required arguments. {0}", new object[]
				{
					text
				});
				errorOutput = RpcHelper.CreateResponsePropertyCollection(2835349507U, 1);
				return false;
			}
			errorOutput = null;
			return true;
		}

		private byte[] ProcessOwaTriggeredSyncNowRequest(MdbefPropertyCollection inputArgs, SubscriptionNotificationRpcMethodCode notificationType)
		{
			Guid guid;
			Guid guid2;
			byte[] result;
			if (!this.TryLoadMailboxInformation(inputArgs, out guid, out guid2, out result))
			{
				return result;
			}
			SyncHealthLogManager.TryLogMailboxNotification(guid, guid2, notificationType);
			IDictionary<Guid, SubscriptionInformation> dictionary = null;
			if (!this.TryReadSubscriptionsInformation(guid2, guid, out dictionary))
			{
				return SubscriptionNotificationProcessor.ProcessingFailedOutput;
			}
			if (dictionary == null || dictionary.Count == 0)
			{
				ContentAggregationConfig.SyncLogSession.LogVerbose((TSLID)114UL, Guid.Empty, guid, "No subscription found for the user in the cache. Mdb Guid:{0}", new object[]
				{
					guid2
				});
				return SubscriptionNotificationProcessor.SuccessOutput;
			}
			WorkType owaTriggeredSyncNowWorkTypeFromNotificationType = SubscriptionNotificationProcessor.GetOwaTriggeredSyncNowWorkTypeFromNotificationType(notificationType);
			foreach (SubscriptionInformation subscriptionInformation in dictionary.Values)
			{
				if (!subscriptionInformation.IsMigration && !subscriptionInformation.Disabled)
				{
					DataAccessLayer.OnWorkTypeBasedSyncNowHandler(owaTriggeredSyncNowWorkTypeFromNotificationType, subscriptionInformation);
				}
			}
			return SubscriptionNotificationProcessor.SuccessOutput;
		}

		public static readonly byte[] SuccessOutput = RpcHelper.CreateResponsePropertyCollection(2835349507U, 0);

		public static readonly byte[] ProcessingFailedOutput = RpcHelper.CreateResponsePropertyCollection(2835349507U, 3);

		private readonly SyncLogSession syncLogSession;

		private readonly Trace tracer;
	}
}
