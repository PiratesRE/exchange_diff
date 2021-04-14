using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Migration.Rpc;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationServiceRpcImpl : IMigrationService
	{
		public UpdateSyncSubscriptionResult UpdateSyncSubscription(UpdateSyncSubscriptionArgs args)
		{
			MigrationServiceRpcMethodCode methodCode = MigrationServiceRpcMethodCode.UpdateSyncSubscription;
			string userLegacyDN = args.UserLegacyDN;
			StoreId storeId = args.SubscriptionMessageId;
			AggregationSubscriptionType subscriptionType = args.SubscriptionType;
			UpdateSyncSubscriptionAction action = args.Action;
			UpdateSyncSubscriptionResult result;
			using (MailboxSession mailboxSession = MigrationServiceRpcImpl.OpenMailbox(args.OrganizationalUnit, userLegacyDN))
			{
				MigrationServiceRpcImpl.SetLoggingContext(mailboxSession);
				if (storeId == null)
				{
					if (args.SubscriptionId == null)
					{
						throw new MissingRequiredSubscriptionIdException();
					}
					storeId = SubscriptionManager.FindSubscription(mailboxSession, args.SubscriptionId.Value);
				}
				MigrationLogger.Log(MigrationEventType.Verbose, "UpdateSyncSubscription, action={0}, type={1}", new object[]
				{
					action,
					subscriptionType
				});
				AggregationSubscription aggregationSubscription;
				try
				{
					aggregationSubscription = SubscriptionManager.LoadSubscription(mailboxSession, storeId, subscriptionType);
					MigrationLogger.Log(MigrationEventType.Information, "UpdateSyncSubscription, Subscription Loaded. args={0}, SubscriptionID: {1}, MessageID: {2}", new object[]
					{
						args,
						aggregationSubscription.SubscriptionGuid,
						aggregationSubscription.SubscriptionMessageId
					});
				}
				catch (ObjectNotFoundException ex)
				{
					MigrationLogger.Log(MigrationEventType.Warning, ex, "UpdateSyncSubscription, Unable to Load Subscription. Args={0}", new object[]
					{
						args
					});
					return new UpdateSyncSubscriptionResult(methodCode, MigrationServiceRpcResultCode.SubscriptionNotFound, ex.Message);
				}
				catch (ArgumentException ex2)
				{
					MigrationLogger.Log(MigrationEventType.Warning, ex2, "UpdateSyncSubscription, Unable to Load Subscription. Args={0}", new object[]
					{
						args
					});
					return new UpdateSyncSubscriptionResult(methodCode, MigrationServiceRpcResultCode.SubscriptionNotFound, ex2.Message);
				}
				if (aggregationSubscription.Status == AggregationStatus.Poisonous || (aggregationSubscription.Status == AggregationStatus.InvalidVersion && action != UpdateSyncSubscriptionAction.Delete))
				{
					MigrationLogger.Log(MigrationEventType.Warning, "Unable to process action {0} on Subscription for user {1} because it is in state {2}", new object[]
					{
						action,
						userLegacyDN,
						aggregationSubscription.Status
					});
					result = new UpdateSyncSubscriptionResult(methodCode, MigrationServiceRpcResultCode.SubscriptionNotFound, null);
				}
				else
				{
					Exception ex3 = null;
					switch (action)
					{
					case UpdateSyncSubscriptionAction.Disable:
						break;
					case UpdateSyncSubscriptionAction.Delete:
						try
						{
							SubscriptionManager.Instance.DeleteSubscription(mailboxSession, aggregationSubscription, true);
							goto IL_24B;
						}
						catch (LocalizedException ex4)
						{
							ex3 = ex4;
							goto IL_24B;
						}
						break;
					case UpdateSyncSubscriptionAction.Finalize:
						if (aggregationSubscription.SyncPhase == SyncPhase.Completed)
						{
							MigrationLogger.Log(MigrationEventType.Information, "Args: {0}. Subscription already finalized. Last Successful Sync Time{1}", new object[]
							{
								args,
								aggregationSubscription.LastSuccessfulSyncTime
							});
							return new UpdateSyncSubscriptionResult(methodCode, MigrationServiceRpcResultCode.SubscriptionAlreadyFinalized, null);
						}
						aggregationSubscription.SyncPhase = SyncPhase.Finalization;
						MigrationServiceRpcImpl.UpdateSubscriptionAndSyncNow(mailboxSession, aggregationSubscription);
						goto IL_24B;
					default:
						return new UpdateSyncSubscriptionResult(methodCode, MigrationServiceRpcResultCode.InvalidSubscriptionAction, null);
					}
					ex3 = MigrationServiceRpcImpl.DisableSyncSubscription(mailboxSession, aggregationSubscription);
					IL_24B:
					if (ex3 == null)
					{
						result = new UpdateSyncSubscriptionResult(methodCode);
					}
					else
					{
						MigrationLogger.Log(MigrationEventType.Error, "UpdateSyncSubscription, exception={0}", new object[]
						{
							ex3
						});
						result = new UpdateSyncSubscriptionResult(methodCode, MigrationServiceRpcResultCode.SubscriptionUpdateFailed, ex3.Message);
					}
				}
			}
			return result;
		}

		public GetSyncSubscriptionStateResult GetSyncSubscriptionState(GetSyncSubscriptionStateArgs args)
		{
			MigrationServiceRpcMethodCode methodCode = MigrationServiceRpcMethodCode.GetSyncSubscriptionState;
			string userLegacyDN = args.UserLegacyDN;
			StoreObjectId subscriptionMessageId = args.SubscriptionMessageId;
			AggregationSubscriptionType subscriptionType = args.SubscriptionType;
			GetSyncSubscriptionStateResult result;
			using (MailboxSession mailboxSession = MigrationServiceRpcImpl.OpenMailbox(args.OrganizationalUnit, userLegacyDN))
			{
				MigrationServiceRpcImpl.SetLoggingContext(mailboxSession);
				MigrationLogger.Log(MigrationEventType.Verbose, "GetSyncSubscriptionState, type={0}", new object[]
				{
					subscriptionType
				});
				AggregationSubscription aggregationSubscription = null;
				try
				{
					aggregationSubscription = SubscriptionManager.LoadSubscription(mailboxSession, subscriptionMessageId, subscriptionType);
					MigrationLogger.Log(MigrationEventType.Verbose, "GetSyncSubscriptionState, Subscription Loaded. Type: {0} Guid: {1}, MessageID: {2}, State: {3}, Detailed State {4}", new object[]
					{
						aggregationSubscription.SubscriptionType,
						aggregationSubscription.SubscriptionGuid,
						aggregationSubscription.SubscriptionMessageId,
						aggregationSubscription.Status,
						aggregationSubscription.DetailedAggregationStatus
					});
				}
				catch (ObjectNotFoundException ex)
				{
					MigrationLogger.Log(MigrationEventType.Error, ex, "Unable to Load Subscription for user={0}", new object[]
					{
						userLegacyDN
					});
					return new GetSyncSubscriptionStateResult(methodCode, MigrationServiceRpcResultCode.SubscriptionNotFound, ex.Message);
				}
				catch (ArgumentException ex2)
				{
					MigrationLogger.Log(MigrationEventType.Error, ex2, "Unable to Load Subscription for user={0}", new object[]
					{
						userLegacyDN
					});
					return new GetSyncSubscriptionStateResult(methodCode, MigrationServiceRpcResultCode.SubscriptionNotFound, ex2.Message);
				}
				GetSyncSubscriptionStateResult getSyncSubscriptionStateResult = new GetSyncSubscriptionStateResult(methodCode, aggregationSubscription.Status, aggregationSubscription.DetailedAggregationStatus, MigrationSubscriptionStatus.None, aggregationSubscription.IsInitialSyncDone, aggregationSubscription.LastSyncTime, aggregationSubscription.LastSuccessfulSyncTime, new long?(aggregationSubscription.ItemsSynced), new long?(aggregationSubscription.ItemsSkipped), aggregationSubscription.LastSyncNowRequestTime);
				result = getSyncSubscriptionStateResult;
			}
			return result;
		}

		public CreateSyncSubscriptionResult CreateSyncSubscription(AbstractCreateSyncSubscriptionArgs createSubscriptionArgs)
		{
			CreateSyncSubscriptionResult result;
			using (MailboxSession mailboxSession = MigrationServiceRpcImpl.OpenMailbox(createSubscriptionArgs.OrganizationalUnit, createSubscriptionArgs.UserLegacyDn))
			{
				MigrationServiceRpcImpl.SetLoggingContext(mailboxSession);
				AggregationSubscription aggregationSubscription = MigrationServiceRpcImpl.LoadMigrationSubscription(mailboxSession, createSubscriptionArgs.SubscriptionType);
				try
				{
					if (aggregationSubscription != null)
					{
						bool flag = createSubscriptionArgs.ForceNew;
						if (!flag)
						{
							switch (aggregationSubscription.Status)
							{
							case AggregationStatus.Succeeded:
							case AggregationStatus.InProgress:
							case AggregationStatus.Delayed:
							case AggregationStatus.Disabled:
								MigrationLogger.Log(MigrationEventType.Warning, "CreateSyncSubscription: updating subscription. Address={0}, SubscriptionAddress={1}, id={2}, status={3}, IsInitialSyncDone={4}", new object[]
								{
									createSubscriptionArgs.SmtpAddress,
									aggregationSubscription.Email,
									aggregationSubscription.SubscriptionGuid,
									aggregationSubscription.Status,
									aggregationSubscription.IsInitialSyncDone
								});
								MigrationServiceRpcImpl.ApplyCreateDataToSubscription(mailboxSession, aggregationSubscription, createSubscriptionArgs);
								break;
							case AggregationStatus.Poisonous:
							case AggregationStatus.InvalidVersion:
								flag = true;
								break;
							default:
								throw new UnexpectedSubscriptionStatusException(aggregationSubscription.Status.ToString());
							}
						}
						if (flag)
						{
							MigrationLogger.Log(MigrationEventType.Information, "CreateSyncSubscription: recreating corrupt subscription.Address={0}, SubscriptionAddress={1}, id={2}, status={3}, IsInitialSyncDone={4}", new object[]
							{
								createSubscriptionArgs.SmtpAddress,
								aggregationSubscription.Email,
								aggregationSubscription.SubscriptionGuid,
								aggregationSubscription.Status,
								aggregationSubscription.IsInitialSyncDone
							});
							SubscriptionManager.TryDeleteSubscription(mailboxSession, aggregationSubscription);
							aggregationSubscription = null;
						}
					}
					if (aggregationSubscription == null)
					{
						aggregationSubscription = createSubscriptionArgs.CreateInMemorySubscription();
						SubscriptionManager.CreateSubscription(mailboxSession, aggregationSubscription);
						MigrationLogger.Log(MigrationEventType.Information, "CreateSyncSubscription: created subscription name={0}, address={1}, id={2}, MessageID={3}, subscription={4}", new object[]
						{
							createSubscriptionArgs.SubscriptionName,
							createSubscriptionArgs.SmtpAddress,
							aggregationSubscription.SubscriptionGuid,
							aggregationSubscription.SubscriptionMessageId,
							aggregationSubscription
						});
					}
					CreateSyncSubscriptionResult createSyncSubscriptionResult = new CreateSyncSubscriptionResult(MigrationServiceRpcMethodCode.CreateSyncSubscription, aggregationSubscription.SubscriptionMessageId, aggregationSubscription.SubscriptionGuid);
					result = createSyncSubscriptionResult;
				}
				catch (LocalizedException ex)
				{
					string text = string.Format(CultureInfo.InvariantCulture, "Unable to Create Subscription, user={0}, exception={1}", new object[]
					{
						createSubscriptionArgs.UserLegacyDn,
						ex
					});
					if (ex is StorageTransientException)
					{
						MigrationApplication.NotifyOfTransientException(ex, text);
						throw new MigrationTargetInvocationException(MigrationServiceRpcResultCode.StorageTransientError, text);
					}
					if (ex is StoragePermanentException)
					{
						MigrationApplication.NotifyOfPermanentException(ex, text);
						throw new MigrationTargetInvocationException(MigrationServiceRpcResultCode.StoragePermanentError, text);
					}
					MigrationLogger.Log(MigrationEventType.Warning, text, new object[0]);
					throw new MigrationTargetInvocationException(MigrationServiceRpcResultCode.SubscriptionCreationFailed, text);
				}
			}
			return result;
		}

		private static MailboxSession OpenMailbox(ADObjectId organizationId, string userLegacyDN)
		{
			MailboxSession mailboxSession = null;
			MailboxSession result;
			try
			{
				ADSessionSettings adSettings = ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(organizationId);
				ExchangePrincipal owner = ExchangePrincipal.FromLegacyDN(adSettings, userLegacyDN, RemotingOptions.LocalConnectionsOnly);
				mailboxSession = SubscriptionManager.OpenMailbox(owner, ExchangeMailboxOpenType.AsTransport, "Client=TransportSync;Action=MigrationService");
				string serverFullyQualifiedDomainName = mailboxSession.ServerFullyQualifiedDomainName;
				string localServerFqdn = MigrationServiceFactory.Instance.GetLocalServerFqdn();
				if (localServerFqdn == null || !localServerFqdn.Equals(serverFullyQualifiedDomainName, StringComparison.OrdinalIgnoreCase))
				{
					string text = string.Format(CultureInfo.InvariantCulture, "Connection could be established to the mailbox for user identified by {0}, but the mailbox is now on server {1} instead of {2}", new object[]
					{
						userLegacyDN,
						serverFullyQualifiedDomainName,
						localServerFqdn
					});
					MigrationLogger.Log(MigrationEventType.Warning, text, new object[0]);
					throw new MigrationTargetInvocationException(MigrationServiceRpcResultCode.MailboxNotFound, text);
				}
				MailboxSession mailboxSession2 = mailboxSession;
				mailboxSession = null;
				result = mailboxSession2;
			}
			catch (ObjectNotFoundException ex)
			{
				MigrationLogger.Log(MigrationEventType.Warning, ex, "Unable to open mailbox for user {0}", new object[]
				{
					userLegacyDN
				});
				throw new MigrationTargetInvocationException(MigrationServiceRpcResultCode.MailboxNotFound, ex.Message);
			}
			finally
			{
				if (mailboxSession != null)
				{
					mailboxSession.Dispose();
				}
			}
			return result;
		}

		private static void ApplyCreateDataToSubscription(MailboxSession mailboxSession, AggregationSubscription subscription, AbstractCreateSyncSubscriptionArgs createSubscriptionArgs)
		{
			createSubscriptionArgs.FillSubscription(subscription);
			subscription.AdjustedLastSuccessfulSyncTime = DateTime.UtcNow;
			MigrationServiceRpcImpl.UpdateSubscriptionAndSyncNow(mailboxSession, subscription);
		}

		private static void UpdateSubscriptionAndSyncNow(MailboxSession mailboxSession, AggregationSubscription subscription)
		{
			subscription.Status = AggregationStatus.InProgress;
			subscription.AdjustedLastSuccessfulSyncTime = DateTime.UtcNow;
			subscription.DetailedAggregationStatus = DetailedAggregationStatus.None;
			bool flag = false;
			Exception ex = SubscriptionManager.TryUpdateSubscriptionAndSyncNow(mailboxSession, subscription, out flag);
			if (ex != null)
			{
				string diagnosticInfo = MigrationLogger.GetDiagnosticInfo(ex, "An existing subscription could not be updated");
				throw new MigrationTargetInvocationException(MigrationServiceRpcResultCode.SubscriptionCreationFailed, diagnosticInfo);
			}
			if (!flag)
			{
				SubscriptionManager.TrySubscriptionSyncNow(mailboxSession, subscription);
			}
		}

		private static void SetLoggingContext(MailboxSession mailboxSession)
		{
			MigrationLogContext.Current.Source = "RPC";
			if (mailboxSession != null && mailboxSession.MailboxOwner != null)
			{
				if (mailboxSession.MailboxOwner.MailboxInfo.OrganizationId != null)
				{
					MigrationLogContext.Current.Organization = mailboxSession.MailboxOwner.MailboxInfo.OrganizationId.OrganizationalUnit;
				}
				MigrationLogContext.Current.JobItem = mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			}
		}

		private static AggregationSubscription LoadMigrationSubscription(MailboxSession mailboxSession, AggregationSubscriptionType subscriptionType)
		{
			List<AggregationSubscription> allSubscriptions = SubscriptionManager.GetAllSubscriptions(mailboxSession, subscriptionType);
			AggregationSubscription aggregationSubscription = null;
			foreach (AggregationSubscription aggregationSubscription2 in allSubscriptions)
			{
				if (aggregationSubscription2.IsMigration)
				{
					if (aggregationSubscription == null)
					{
						aggregationSubscription = aggregationSubscription2;
					}
					else
					{
						MigrationLogger.Log(MigrationEventType.Warning, "Unexpected internal error: Found more than one subscription for User {0}. Deleting subscription with Guid {1} and name {2}", new object[]
						{
							aggregationSubscription2.UserLegacyDN,
							aggregationSubscription2.SubscriptionGuid,
							aggregationSubscription2.Name
						});
						SubscriptionManager.Instance.DeleteSubscription(mailboxSession, aggregationSubscription2, true);
					}
				}
			}
			return aggregationSubscription;
		}

		private static Exception DisableSyncSubscription(MailboxSession mailboxSession, AggregationSubscription subscription)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "ATTENTION: MigrationServiceRpcImpl: Disabling subscription.  Previous status: {0} Date: {1}", new object[]
			{
				subscription.Status,
				DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffZ", DateTimeFormatInfo.InvariantInfo)
			});
			subscription.Status = AggregationStatus.Disabled;
			if (string.IsNullOrEmpty(subscription.Diagnostics))
			{
				subscription.Diagnostics = text;
			}
			else
			{
				subscription.Diagnostics = subscription.Diagnostics + Environment.NewLine + text;
			}
			return SubscriptionManager.UpdateSubscription(mailboxSession, subscription);
		}

		private const string ClientInfoString = "Client=TransportSync;Action=MigrationService";
	}
}
