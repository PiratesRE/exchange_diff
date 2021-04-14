using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Worker.Throttling;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TransportSyncStorageProvider : NativeSyncStorageProvider
	{
		public override NativeSyncStorageProviderState Bind(SyncMailboxSession syncMailboxSession, ISyncWorkerData subscription, INativeStateStorage stateStorage, MailSubmitter mailSubmitter, SyncLogSession syncLogSession, bool underRecovery)
		{
			return new TransportSyncStorageProviderState(syncMailboxSession, subscription, stateStorage, mailSubmitter, syncLogSession, underRecovery);
		}

		public override void Unbind(NativeSyncStorageProviderState state)
		{
		}

		public override IAsyncResult BeginApplyChanges(NativeSyncStorageProviderState state, IList<SyncChangeEntry> changeList, ISyncStorageProviderItemRetriever itemRetriever, object itemRetrieverState, AsyncCallback callback, object callbackState, object syncPoisonContext)
		{
			TransportSyncStorageProviderState transportSyncStorageProviderState = (TransportSyncStorageProviderState)state;
			AsyncResult<TransportSyncStorageProviderState, SyncProviderResultData> asyncResult = new AsyncResult<TransportSyncStorageProviderState, SyncProviderResultData>(this, transportSyncStorageProviderState, callback, callbackState, syncPoisonContext);
			if (changeList == null || changeList.Count == 0)
			{
				transportSyncStorageProviderState.SyncLogSession.LogDebugging((TSLID)1060UL, TransportSyncStorageProvider.Tracer, "Empty Change List, so we're skipping applying changes.", new object[0]);
				asyncResult.SetCompletedSynchronously();
				asyncResult.ProcessCompleted(new SyncProviderResultData(changeList, false, false));
			}
			else
			{
				transportSyncStorageProviderState.ItemRetriever = itemRetriever;
				transportSyncStorageProviderState.ItemRetrieverState = itemRetrieverState;
				transportSyncStorageProviderState.Changes = changeList;
				transportSyncStorageProviderState.ItemsToProcess = new Queue<SyncChangeEntry>(changeList.Count);
				foreach (SyncChangeEntry item in changeList)
				{
					transportSyncStorageProviderState.ItemsToProcess.Enqueue(item);
				}
				ThreadPool.QueueUserWorkItem(asyncResult.GetWaitCallbackWithPoisonContext(new WaitCallback(this.BeginApplyItem)), asyncResult);
			}
			return asyncResult;
		}

		public override AsyncOperationResult<SyncProviderResultData> EndApplyChanges(IAsyncResult asyncResult)
		{
			AsyncResult<TransportSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<TransportSyncStorageProviderState, SyncProviderResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public override void Cancel(IAsyncResult asyncResult)
		{
			AsyncResult<TransportSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<TransportSyncStorageProviderState, SyncProviderResultData>)asyncResult;
			if (asyncResult2.State.TryCancel)
			{
				return;
			}
			asyncResult2.State.SetTryCancel();
			IAsyncResult pendingAsyncResult = asyncResult2.PendingAsyncResult;
			if (pendingAsyncResult != null && asyncResult2.State.ItemRetriever != null)
			{
				asyncResult2.State.ItemRetriever.CancelGetItem(pendingAsyncResult);
			}
		}

		protected virtual void SubmitEmail(TransportSyncStorageProviderState transportState, SyncChangeEntry change, string deliveryFolderHexId)
		{
			string userExchangeMailboxSmtpAddress = transportState.Subscription.UserExchangeMailboxSmtpAddress;
			ISyncEmail syncEmail = (ISyncEmail)change.SyncObject;
			TransportMailItem transportMailItem = transportState.MailSubmitter.CreateNewMail();
			Guid mailboxGuid = transportState.SyncMailboxSession.MailboxSession.MailboxGuid;
			Guid subscriptionGuid = transportState.Subscription.SubscriptionGuid;
			SyncStoreLoadManager.TrackMailItem(transportMailItem, mailboxGuid, subscriptionGuid, change.CloudId);
			using (Stream writeStream = transportState.MailSubmitter.GetWriteStream(transportMailItem))
			{
				using (Stream mimeStream = syncEmail.MimeStream)
				{
					byte[] array = new byte[2048];
					for (;;)
					{
						int num = mimeStream.Read(array, 0, array.Length);
						if (num == 0)
						{
							break;
						}
						writeStream.Write(array, 0, num);
					}
				}
			}
			transportMailItem.DateReceived = DateTime.UtcNow;
			List<string> list = new List<string>(1);
			list.Add(userExchangeMailboxSmtpAddress);
			transportState.SyncLogSession.LogDebugging((TSLID)1061UL, TransportSyncStorageProvider.Tracer, "Submitting item to transport pipeline without needing to get it.", new object[0]);
			string text = syncEmail.SourceSession.SessionId;
			if (string.IsNullOrEmpty(text))
			{
				text = transportState.Subscription.SubscriptionGuid.ToString();
			}
			MsgTrackReceiveInfo msgTrackInfo = new MsgTrackReceiveInfo(null, syncEmail.SourceSession.Server, null, text, syncEmail.SourceSession.Protocol, null, transportMailItem.MessageTrackingSecurityInfo);
			DateTime originalReceivedTime = (syncEmail.ReceivedTime != null) ? syncEmail.ReceivedTime.Value.UniversalTime : transportMailItem.DateReceived;
			Exception ex = null;
			if (MultiTenantTransport.MultiTenancyEnabled)
			{
				transportState.SyncLogSession.LogDebugging((TSLID)1107UL, TransportSyncStorageProvider.Tracer, "Attributing mail item", new object[0]);
				ex = TransportSyncStorageProvider.AttributeMailItem(transportState, transportMailItem);
			}
			if (ex == null)
			{
				transportState.SyncLogSession.LogDebugging((TSLID)1109UL, TransportSyncStorageProvider.Tracer, "Submitting mail", new object[0]);
				ex = transportState.MailSubmitter.SubmitMail(syncEmail.SourceSession.Protocol, transportState.SyncLogSession, transportMailItem, syncEmail, list, deliveryFolderHexId, transportState.Subscription.SubscriptionGuid, change.CloudId, change.CloudVersion, originalReceivedTime, msgTrackInfo);
			}
			if (ex == null)
			{
				change.Submitted = true;
				return;
			}
			transportState.SyncLogSession.LogDebugging((TSLID)1110UL, TransportSyncStorageProvider.Tracer, "Encountered exception {0}", new object[]
			{
				ex
			});
			if (ex is TransientException)
			{
				change.Exception = SyncTransientException.CreateItemLevelException(ex);
				return;
			}
			change.Exception = SyncPermanentException.CreateItemLevelException(ex);
		}

		protected override void UpdateMapping(NativeSyncStorageProviderState providerState, SyncChangeEntry change)
		{
			if (!change.Persist)
			{
				providerState.SyncLogSession.LogDebugging((TSLID)1080UL, TransportSyncStorageProvider.Tracer, "Skipping mapping table update operation as persist is false for: {0}.", new object[]
				{
					change
				});
				return;
			}
			if (change.Exception != null && (change.ChangeType != ChangeType.Delete || !(change.Exception is SyncPermanentException)))
			{
				providerState.SyncLogSession.LogDebugging((TSLID)1081UL, TransportSyncStorageProvider.Tracer, "Failed to apply change and so skipping mapping table update operation for: {0}.", new object[]
				{
					change
				});
				return;
			}
			switch (change.ChangeType)
			{
			case ChangeType.Add:
				providerState.SyncLogSession.LogDebugging((TSLID)1082UL, TransportSyncStorageProvider.Tracer, "Adding change to sync state: {0}.", new object[]
				{
					change
				});
				if (SchemaType.Folder == change.SchemaType)
				{
					if (providerState.UnderRecovery)
					{
						providerState.StateStorage.TryRemoveFolder(change.CloudId);
					}
					bool isInbox = providerState.IsInboxFolderId(change.NativeId);
					if (!providerState.StateStorage.TryAddFolder(isInbox, change.CloudId, change.CloudFolderId, change.NativeId, change.ChangeKey, change.NativeFolderId, change.CloudVersion, change.Properties))
					{
						providerState.SyncLogSession.LogError((TSLID)1083UL, TransportSyncStorageProvider.Tracer, "Could not map change as it conflicts with an existing mapping. This generally means a provider bug or malicious content.", new object[0]);
						change.Exception = SyncPermanentException.CreateItemLevelException(new SyncPoisonItemFoundException(change.ToString(), providerState.Subscription.SubscriptionGuid));
						return;
					}
				}
				else
				{
					if (providerState.UnderRecovery)
					{
						providerState.StateStorage.TryRemoveItem(change.CloudId);
					}
					if (!providerState.StateStorage.TryAddItem(change.CloudId, change.CloudFolderId, change.NativeId, change.ChangeKey, change.NativeFolderId, change.CloudVersion, change.Properties))
					{
						providerState.SyncLogSession.LogError((TSLID)1084UL, TransportSyncStorageProvider.Tracer, "Could not map change as it conflicts with an existing mapping. This generally means a provider bug or malicious content.", new object[0]);
						change.Exception = SyncPermanentException.CreateItemLevelException(new SyncPoisonItemFoundException(change.ToString(), providerState.Subscription.SubscriptionGuid));
						return;
					}
				}
				break;
			case ChangeType.Change:
			{
				bool isInbox2 = providerState.IsInboxFolderId(change.NewNativeId ?? change.NativeId);
				if (!providerState.StateStorage.TryUpdateFolder(isInbox2, change.NativeId, change.NewNativeId, change.CloudId, change.NewCloudId, null, change.ChangeKey, null, change.CloudVersion, change.Properties))
				{
					providerState.SyncLogSession.LogError((TSLID)1086UL, TransportSyncStorageProvider.Tracer, "Could not map recreation of folder. Generally this means a provider bug or malicious content.", new object[0]);
					change.Exception = SyncPermanentException.CreateItemLevelException(new SyncPoisonItemFoundException(change.ToString(), providerState.Subscription.SubscriptionGuid));
				}
				break;
			}
			case (ChangeType)3:
				break;
			case ChangeType.Delete:
				providerState.SyncLogSession.LogDebugging((TSLID)1085UL, TransportSyncStorageProvider.Tracer, "Removing change from sync state: {0}.", new object[]
				{
					change
				});
				if (SchemaType.Folder == change.SchemaType)
				{
					providerState.StateStorage.TryRemoveFolder(change.CloudId);
					return;
				}
				providerState.StateStorage.TryRemoveItem(change.CloudId);
				return;
			default:
				return;
			}
		}

		private static Exception AttributeMailItem(TransportSyncStorageProviderState transportState, TransportMailItem mailItem)
		{
			mailItem.Directionality = MailDirectionality.Incoming;
			if (transportState.SyncMailboxSession != null && transportState.SyncMailboxSession.MailboxSession != null && transportState.SyncMailboxSession.MailboxSession.MailboxOwner != null && transportState.SyncMailboxSession.MailboxSession.MailboxOwner.MailboxInfo.OrganizationId != null)
			{
				byte[] persistableTenantPartitionHint = transportState.SyncMailboxSession.MailboxSession.PersistableTenantPartitionHint;
				OrganizationId organizationId = transportState.SyncMailboxSession.MailboxSession.MailboxOwner.MailboxInfo.OrganizationId;
				if (persistableTenantPartitionHint != null)
				{
					TenantPartitionHint tenantPartitionHint = TenantPartitionHint.FromPersistablePartitionHint(persistableTenantPartitionHint);
					try
					{
						mailItem.ExternalOrganizationId = tenantPartitionHint.GetExternalDirectoryOrganizationId();
						goto IL_135;
					}
					catch (CannotResolveExternalDirectoryOrganizationIdException ex)
					{
						MultiTenantTransport.TraceAttributionError("Error {0} attributing from partition id for org {1}. Resolving to first org", new object[]
						{
							ex,
							organizationId
						});
						mailItem.ExternalOrganizationId = TenantPartitionHint.ExternalDirectoryOrganizationIdForRootOrg;
						goto IL_135;
					}
				}
				if (organizationId == OrganizationId.ForestWideOrgId)
				{
					mailItem.ExternalOrganizationId = MultiTenantTransport.SafeTenantId;
				}
				else
				{
					Guid externalOrganizationId = TenantPartitionHint.ExternalDirectoryOrganizationIdForRootOrg;
					try
					{
						externalOrganizationId = ADAccountPartitionLocator.GetExternalDirectoryOrganizationIdByTenantName(organizationId.OrganizationalUnit.Name, organizationId.PartitionId);
					}
					catch (CannotResolveExternalDirectoryOrganizationIdException ex2)
					{
						MultiTenantTransport.TraceAttributionError("Error {0} attributing from partition id for org {1}. Resolving to first org", new object[]
						{
							ex2,
							organizationId
						});
					}
					mailItem.ExternalOrganizationId = externalOrganizationId;
				}
			}
			IL_135:
			if (mailItem.ExternalOrganizationId == Guid.Empty)
			{
				ADOperationResult adoperationResult = MultiTenantTransport.TryAttributeFromDomain(mailItem);
				if (!adoperationResult.Succeeded)
				{
					MultiTenantTransport.TraceAttributionError("Error {0} attributing from domain id for message {1}", new object[]
					{
						adoperationResult.Exception,
						MultiTenantTransport.ToString(mailItem)
					});
				}
				return adoperationResult.Exception;
			}
			return null;
		}

		private bool TryEnsureDeliveryFolder(TransportSyncStorageProviderState transportState, SyncChangeEntry itemChange, out string folderHexId)
		{
			folderHexId = null;
			if (itemChange.CloudFolderId == null)
			{
				return true;
			}
			if (transportState.TryGetExistingFolder(itemChange.CloudFolderId, out folderHexId))
			{
				return true;
			}
			if (!base.TryEnsureFullParentFolderHierarchy(transportState, itemChange))
			{
				if (itemChange.Exception is TransientException)
				{
					transportState.SyncLogSession.LogError((TSLID)1074UL, TransportSyncStorageProvider.Tracer, "Item being skipped due to exception {0}", new object[]
					{
						itemChange
					});
					return false;
				}
				transportState.SyncLogSession.LogError((TSLID)1064UL, TransportSyncStorageProvider.Tracer, "Item being mapped to INBOX due to exception {0}", new object[]
				{
					itemChange
				});
				itemChange.NativeFolderId = null;
			}
			if (!transportState.IsInboxFolderId(itemChange.NativeFolderId))
			{
				folderHexId = itemChange.NativeFolderId.ToHexEntryId();
				transportState.SyncLogSession.LogDebugging((TSLID)1065UL, TransportSyncStorageProvider.Tracer, "Marking folder {0} as existent.", new object[]
				{
					itemChange.NativeFolderId
				});
				transportState.AddExistingFolder(itemChange.CloudFolderId, folderHexId);
			}
			return true;
		}

		private void BeginApplyItem(object state)
		{
			AsyncResult<TransportSyncStorageProviderState, SyncProviderResultData> asyncResult = (AsyncResult<TransportSyncStorageProviderState, SyncProviderResultData>)state;
			TransportSyncStorageProviderState state2 = asyncResult.State;
			state2.SyncLogSession.LogDebugging((TSLID)1066UL, TransportSyncStorageProvider.Tracer, "Processing Items.", new object[0]);
			while (state2.ItemsToProcess.Count > 0)
			{
				if (state2.TryCancel)
				{
					asyncResult.ProcessCanceled();
					return;
				}
				SyncChangeEntry syncChangeEntry = state2.ItemsToProcess.Dequeue();
				bool flag = true;
				try
				{
					if (this.ResolveChange(state2, syncChangeEntry))
					{
						state2.SyncLogSession.LogDebugging((TSLID)1067UL, TransportSyncStorageProvider.Tracer, "Processing change with Cloud ID: {0}", new object[]
						{
							syncChangeEntry.CloudId
						});
						ChangeType changeType = syncChangeEntry.ChangeType;
						if (changeType == ChangeType.Add)
						{
							flag = false;
							asyncResult.PendingAsyncResult = state2.ItemRetriever.BeginGetItem(state2.ItemRetrieverState, syncChangeEntry, new AsyncCallback(this.EndApplyItem), asyncResult, asyncResult.SyncPoisonContext);
							return;
						}
						if (changeType != ChangeType.Delete)
						{
							throw new InvalidOperationException("Invalid ChangeType attempting to be applied in the Transport Provider: " + syncChangeEntry.ChangeType);
						}
						this.UpdateMapping(state2, syncChangeEntry);
					}
				}
				catch (StorageTransientException exception)
				{
					this.UpdateStateBasedOnItemException(state2, syncChangeEntry, exception);
					flag = true;
				}
				catch (StoragePermanentException exception2)
				{
					this.UpdateStateBasedOnItemException(state2, syncChangeEntry, exception2);
					flag = true;
				}
				catch (SyncStoreUnhealthyException exception3)
				{
					this.UpdateStateBasedOnItemException(state2, syncChangeEntry, exception3);
					flag = true;
				}
				finally
				{
					if (flag && syncChangeEntry != null && syncChangeEntry.SyncObject != null)
					{
						syncChangeEntry.SyncObject.Dispose();
						syncChangeEntry.SyncObject = null;
					}
				}
			}
			state2.SyncLogSession.LogDebugging((TSLID)1068UL, TransportSyncStorageProvider.Tracer, "Finished applying all items", new object[0]);
			state2.ItemRetriever = null;
			state2.ItemRetrieverState = null;
			state2.ItemsToProcess = null;
			asyncResult.PendingAsyncResult = null;
			asyncResult.ProcessCompleted(new SyncProviderResultData(state2.Changes, state2.HasPermanentSyncErrors, state2.HasTransientSyncErrors));
		}

		private void EndApplyItem(IAsyncResult asyncResult)
		{
			AsyncResult<TransportSyncStorageProviderState, SyncProviderResultData> asyncResult2 = (AsyncResult<TransportSyncStorageProviderState, SyncProviderResultData>)asyncResult.AsyncState;
			TransportSyncStorageProviderState state = asyncResult2.State;
			AsyncOperationResult<SyncChangeEntry> asyncOperationResult = null;
			Exception ex = null;
			try
			{
				asyncOperationResult = state.ItemRetriever.EndGetItem(asyncResult);
				if (asyncOperationResult.Data != null)
				{
					asyncOperationResult.Data.ApplyAttempted = true;
				}
				ExTraceGlobals.FaultInjectionTracer.TraceTest(3026595133U);
				if (asyncOperationResult.IsSucceeded)
				{
					state.SyncLogSession.LogDebugging((TSLID)1070UL, TransportSyncStorageProvider.Tracer, "Item fetched successfully and applying change: {0}", new object[]
					{
						asyncOperationResult.Data
					});
					switch (asyncOperationResult.Data.SchemaType)
					{
					case SchemaType.Email:
					{
						string deliveryFolderHexId = null;
						if (this.TryEnsureDeliveryFolder(state, asyncOperationResult.Data, out deliveryFolderHexId))
						{
							this.SubmitEmail(state, asyncOperationResult.Data, deliveryFolderHexId);
							goto IL_167;
						}
						goto IL_167;
					}
					case SchemaType.Folder:
						if (!base.ContinueWithFolder(state, asyncOperationResult.Data, out ex))
						{
							asyncOperationResult.Data.Exception = ex;
							goto IL_167;
						}
						if (base.TryEnsureFullParentFolderHierarchy(state, asyncOperationResult.Data))
						{
							this.ApplyAddFolderChange(state, asyncOperationResult.Data);
							goto IL_167;
						}
						goto IL_167;
					}
					throw new InvalidOperationException("Invalid SchemaType being applied in the Transport Provider: " + asyncOperationResult.Data.SchemaType);
				}
				ex = asyncOperationResult.Exception;
				state.SyncLogSession.LogError((TSLID)1069UL, TransportSyncStorageProvider.Tracer, "Failed to get item: {0}", new object[]
				{
					asyncOperationResult.Exception
				});
				IL_167:
				if (asyncOperationResult.Data != null && asyncOperationResult.Data.Exception != null)
				{
					ex = asyncOperationResult.Data.Exception;
				}
				if (ex == null)
				{
					this.UpdateMapping(state, asyncOperationResult.Data);
				}
			}
			catch (IOException innerException)
			{
				ex = SyncTransientException.CreateItemLevelException(innerException);
			}
			catch (ExchangeDataException innerException2)
			{
				ex = SyncTransientException.CreateItemLevelException(innerException2);
			}
			catch (ADTransientException innerException3)
			{
				ex = SyncTransientException.CreateItemLevelException(innerException3);
			}
			catch (StorageTransientException innerException4)
			{
				ex = SyncTransientException.CreateItemLevelException(innerException4);
			}
			catch (StoragePermanentException ex2)
			{
				if (ExceptionUtilities.ShouldPermanentMapiOrXsoExceptionBeTreatedAsTransient(state.SyncLogSession, ex2))
				{
					ex = SyncTransientException.CreateItemLevelException(ex2);
				}
				else
				{
					ex = SyncPermanentException.CreateItemLevelException(ex2);
				}
			}
			catch (SyncStoreUnhealthyException innerException5)
			{
				ex = SyncTransientException.CreateItemLevelException(innerException5);
			}
			finally
			{
				if (asyncOperationResult != null && asyncOperationResult.Data != null && asyncOperationResult.Data.SyncObject != null)
				{
					asyncOperationResult.Data.SyncObject.Dispose();
					asyncOperationResult.Data.SyncObject = null;
				}
			}
			if (ex != null)
			{
				this.UpdateStateBasedOnItemException(state, asyncOperationResult.Data, ex);
			}
			this.BeginApplyItem(asyncResult2);
		}

		private void UpdateStateBasedOnItemException(TransportSyncStorageProviderState transportState, SyncChangeEntry change, Exception exception)
		{
			transportState.SyncLogSession.LogError((TSLID)1071UL, TransportSyncStorageProvider.Tracer, "Hit exception while apply item{0}: {1}", new object[]
			{
				(change == null) ? "<NULL>" : change.ToString(),
				exception
			});
			if (!(exception is SyncTransientException))
			{
				transportState.HasPermanentSyncErrors = true;
			}
			else
			{
				transportState.HasTransientSyncErrors = true;
			}
			if (change != null)
			{
				change.Exception = exception;
				if (change.SchemaType == SchemaType.Folder)
				{
					transportState.AddFailedFolderCreation(change.CloudId, change.Exception);
				}
			}
		}

		private bool ResolveChange(TransportSyncStorageProviderState providerState, SyncChangeEntry change)
		{
			if (change.SchemaType == SchemaType.Folder && change.ChangeType == ChangeType.SoftDelete && providerState.Subscription.FolderSupport == FolderSupport.RootFoldersOnly)
			{
				change.ChangeType = ChangeType.Delete;
				providerState.SyncLogSession.LogDebugging((TSLID)1076UL, TransportSyncStorageProvider.Tracer, "Converting Folder SoftDelete to HardDelete (ChangeType.Delete) for Subscriptions that do not support sub-folders. Updated Change : {0}", new object[]
				{
					change
				});
			}
			if ((change.SchemaType != SchemaType.Email && change.SchemaType != SchemaType.Folder) || (change.ChangeType != ChangeType.Delete && change.ChangeType != ChangeType.Add))
			{
				providerState.SyncLogSession.LogDebugging((TSLID)1077UL, TransportSyncStorageProvider.Tracer, "Skipped change as it was not an Add/Delete for a Folder or Item: {0}", new object[]
				{
					change
				});
				return false;
			}
			StoreObjectId storeObjectId;
			byte[] changeKey;
			StoreObjectId nativeFolderId;
			Dictionary<string, string> properties;
			bool flag;
			if (SchemaType.Folder == change.SchemaType)
			{
				string text;
				string text2;
				flag = providerState.StateStorage.TryFindFolder(change.CloudId, out text, out storeObjectId, out changeKey, out nativeFolderId, out text2, out properties);
			}
			else
			{
				string text;
				string text2;
				flag = providerState.StateStorage.TryFindItem(change.CloudId, out text, out storeObjectId, out changeKey, out nativeFolderId, out text2, out properties);
			}
			if (change.ChangeType == ChangeType.Add && flag)
			{
				providerState.SyncLogSession.LogVerbose((TSLID)1078UL, TransportSyncStorageProvider.Tracer, "Skipped change was already in source provider with Cloud ID: {0}", new object[]
				{
					change.CloudId
				});
				return false;
			}
			if (flag)
			{
				change.NativeId = storeObjectId;
				change.NativeFolderId = nativeFolderId;
				change.ChangeKey = changeKey;
				change.Properties = properties;
			}
			else if (!string.IsNullOrEmpty(change.CloudFolderId))
			{
				string text;
				string text2;
				if (providerState.StateStorage.TryFindFolder(change.CloudFolderId, out text, out storeObjectId, out changeKey, out nativeFolderId, out text2, out properties))
				{
					change.NativeFolderId = storeObjectId;
				}
				else
				{
					providerState.SyncLogSession.LogError((TSLID)1079UL, TransportSyncStorageProvider.Tracer, "Cloud not find folder containing change, so we will go with Inbox: {0}", new object[]
					{
						change
					});
				}
			}
			return true;
		}

		private const int BufferSize = 2048;

		private static readonly Trace Tracer = ExTraceGlobals.TransportSyncStorageProviderTracer;
	}
}
