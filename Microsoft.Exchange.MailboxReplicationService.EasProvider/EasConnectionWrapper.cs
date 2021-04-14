using System;
using System.Collections.Generic;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Eas;
using Microsoft.Exchange.Connections.Eas.Commands;
using Microsoft.Exchange.Connections.Eas.Commands.Connect;
using Microsoft.Exchange.Connections.Eas.Commands.Disconnect;
using Microsoft.Exchange.Connections.Eas.Commands.FolderCreate;
using Microsoft.Exchange.Connections.Eas.Commands.FolderDelete;
using Microsoft.Exchange.Connections.Eas.Commands.FolderSync;
using Microsoft.Exchange.Connections.Eas.Commands.FolderUpdate;
using Microsoft.Exchange.Connections.Eas.Commands.GetItemEstimate;
using Microsoft.Exchange.Connections.Eas.Commands.ItemOperations;
using Microsoft.Exchange.Connections.Eas.Commands.MoveItems;
using Microsoft.Exchange.Connections.Eas.Commands.SendMail;
using Microsoft.Exchange.Connections.Eas.Commands.Sync;
using Microsoft.Exchange.Connections.Eas.Model.Extensions;
using Microsoft.Exchange.Connections.Eas.Model.Response.ItemOperations;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class EasConnectionWrapper
	{
		internal EasConnectionWrapper(IEasConnection easConnection)
		{
			ArgumentValidator.ThrowIfNull("easConnection", easConnection);
			this.wrappedObject = easConnection;
		}

		internal string ServerName
		{
			get
			{
				return this.wrappedObject.ServerName;
			}
		}

		internal UserSmtpAddress UserSmtpAddress
		{
			get
			{
				return this.wrappedObject.UserSmtpAddress;
			}
		}

		internal void Connect()
		{
			ConnectResponse connectResponse = this.wrappedObject.Connect(ConnectRequest.Default, null);
			if (connectResponse.ConnectStatus != ConnectStatus.Success)
			{
				throw new EasConnectFailedException(connectResponse.ConnectStatusString, connectResponse.HttpStatusString, connectResponse.UserSmtpAddressString);
			}
		}

		internal void Disconnect()
		{
			this.wrappedObject.Disconnect(DisconnectRequest.Default);
		}

		internal FolderSyncResponse FolderSync()
		{
			FolderSyncRequest initialSyncRequest = FolderSyncRequest.InitialSyncRequest;
			return this.FolderSync(initialSyncRequest);
		}

		internal FolderSyncResponse FolderSync(string syncKey)
		{
			FolderSyncRequest folderSyncRequest = new FolderSyncRequest
			{
				SyncKey = syncKey
			};
			return this.FolderSync(folderSyncRequest);
		}

		internal int GetCountOfItemsToSync(string folderId, EasSyncOptions options)
		{
			GetItemEstimateRequest getItemEstimateRequest = EasRequestGenerator.CreateEstimateRequest(options.SyncKey, folderId, options.RecentOnly);
			GetItemEstimateResponse itemEstimate = this.GetItemEstimate(getItemEstimateRequest);
			if (itemEstimate.Estimate == null)
			{
				return 0;
			}
			return itemEstimate.Estimate.Value;
		}

		internal ItemOperationsResponse LookupItems(IReadOnlyCollection<string> messageIds, string folderId)
		{
			ItemOperationsRequest itemOperationsRequest = EasRequestGenerator.CreateItemOpsRequestForSelectedMessages(messageIds, folderId);
			return this.ItemOperations(itemOperationsRequest);
		}

		internal Properties FetchMessageItem(string messageId, string folderId)
		{
			ItemOperationsRequest itemOperationsRequest = EasRequestGenerator.CreateItemOpsRequest(messageId, folderId);
			ItemOperationsResponse response = this.ItemOperations(itemOperationsRequest);
			ItemOperationsStatus status;
			Properties messageProperties = response.GetMessageProperties(0, out status);
			EasConnectionWrapper.WrapException(delegate()
			{
				response.ThrowIfStatusIsFailed(status);
			}, (ConnectionsTransientException e) => new EasFetchFailedTransientException(e.Message, e), (ConnectionsPermanentException e) => new EasFetchFailedPermanentException(e.Message, e));
			return messageProperties;
		}

		internal Properties FetchCalendarItem(string itemId, string folderId)
		{
			ItemOperationsRequest itemOperationsRequest = EasRequestGenerator.CreateItemOpsRequestForCalendarItem(itemId, folderId);
			ItemOperationsResponse itemOperationsResponse = this.ItemOperations(itemOperationsRequest);
			return itemOperationsResponse.GetCalendarItemProperties();
		}

		internal string MoveItem(string messageId, string sourceFolderId, string destinationFolderId)
		{
			MoveItemsRequest moveItemsRequest = EasRequestGenerator.CreateMoveRequestForMessages(new string[]
			{
				messageId
			}, sourceFolderId, destinationFolderId);
			MoveItemsResponse moveItemsResponse = this.MoveItems(moveItemsRequest);
			return moveItemsResponse.Responses[0].DstMsgId;
		}

		internal SendMailResponse SendMail(string clientId, string mimeString)
		{
			SendMailRequest sendMailRequest = new SendMailRequest
			{
				ClientId = clientId,
				SaveInSentItems = true,
				Mime = mimeString
			};
			return this.SendMail(sendMailRequest);
		}

		internal SyncResponse Sync(string folderId, EasSyncOptions options, bool recentOnly)
		{
			SyncRequest syncRequest = EasRequestGenerator.CreateSyncRequestForAllMessages(options.SyncKey, folderId, options.MaxNumberOfMessage, recentOnly);
			return this.Sync(syncRequest);
		}

		internal void SyncRead(string messageId, string syncKey, string folderId, bool isRead)
		{
			SyncRequest syncRequest = EasRequestGenerator.CreateSyncRequestForReadUnreadMessages(new string[]
			{
				messageId
			}, syncKey, folderId, isRead);
			this.SyncUpdate(messageId, syncRequest);
		}

		internal void SyncFlag(string messageId, string syncKey, string folderId, FlagStatus flagStatus)
		{
			SyncRequest syncRequest = EasRequestGenerator.CreateSyncRequestForFlagMessages(new string[]
			{
				messageId
			}, syncKey, folderId, flagStatus);
			this.SyncUpdate(messageId, syncRequest);
		}

		internal void DeleteItem(string messageId, string syncKey, string folderId)
		{
			SyncRequest syncRequest = EasRequestGenerator.CreateSyncRequestForDeleteMessages(new string[]
			{
				messageId
			}, syncKey, folderId);
			this.Sync(syncRequest);
		}

		internal void UpdateCalendarEvent(string calendarEventId, string syncKey, string folderId, Event theEvent, IList<Event> exceptionalEvents, IList<string> deletedOccurrences, UserSmtpAddress userSmtpAddress)
		{
			SyncRequest syncRequest = EasRequestGenerator.CreateSyncRequestForUpdateCalendarEvent(syncKey, calendarEventId, folderId, theEvent, exceptionalEvents, deletedOccurrences, userSmtpAddress);
			this.SyncUpdate(calendarEventId, syncRequest);
		}

		internal byte[] CreateCalendarEvent(string clientId, string syncKey, out string newSyncKey, string folderId, Event theEvent, IList<Event> exceptionalEvents, IList<string> deletedOccurrences, UserSmtpAddress userSmtpAddress)
		{
			SyncRequest syncRequest = EasRequestGenerator.CreateSyncRequestForCreateCalendarEvent(syncKey, clientId, folderId, theEvent, exceptionalEvents, deletedOccurrences, userSmtpAddress);
			SyncResponse syncResponse = this.SyncCreation(clientId, syncRequest);
			newSyncKey = syncResponse.Collections[0].SyncKey;
			return EasMailbox.GetEntryId(syncResponse.AddResponses[0].ServerId);
		}

		internal FolderCreateResponse CreateFolder(string syncKey, string displayName, string parentId, EasFolderType folderType)
		{
			FolderCreateRequest folderCreateRequest = new FolderCreateRequest
			{
				SyncKey = syncKey,
				DisplayName = displayName,
				ParentId = parentId,
				Type = (int)folderType
			};
			return this.FolderCreate(folderCreateRequest);
		}

		internal FolderDeleteResponse DeleteFolder(string syncKey, string folderId)
		{
			FolderDeleteRequest folderDeleteRequest = new FolderDeleteRequest
			{
				SyncKey = syncKey,
				ServerId = folderId
			};
			return this.FolderDelete(folderDeleteRequest);
		}

		internal FolderUpdateResponse MoveOrRenameFolder(string syncKey, string folderId, string destParentId, string displayName)
		{
			FolderUpdateRequest folderUpdateRequest = new FolderUpdateRequest
			{
				SyncKey = syncKey,
				ServerId = folderId,
				ParentId = destParentId,
				DisplayName = displayName
			};
			return this.FolderUpdate(folderUpdateRequest);
		}

		private static T WrapException<T>(Func<T> function, Func<ConnectionsTransientException, MailboxReplicationTransientException> wrapTransientException, Func<ConnectionsPermanentException, MailboxReplicationPermanentException> wrapPermanentException) where T : class
		{
			T result = default(T);
			EasConnectionWrapper.WrapException(delegate()
			{
				result = function();
			}, wrapTransientException, wrapPermanentException);
			return result;
		}

		private static void WrapException(Action action, Func<ConnectionsTransientException, MailboxReplicationTransientException> wrapTransientException, Func<ConnectionsPermanentException, MailboxReplicationPermanentException> wrapPermanentException)
		{
			try
			{
				action();
			}
			catch (EasRequiresSyncKeyResetException)
			{
				throw;
			}
			catch (EasRetryAfterException ex)
			{
				throw new RelinquishJobServerBusyTransientException(ex.LocalizedString, ex.Delay, ex);
			}
			catch (ConnectionsTransientException arg)
			{
				throw wrapTransientException(arg);
			}
			catch (ConnectionsPermanentException arg2)
			{
				throw wrapPermanentException(arg2);
			}
		}

		private void SyncUpdate(string messageId, SyncRequest syncRequest)
		{
			SyncResponse response = this.Sync(syncRequest);
			SyncStatus status = response.GetChangeResponseStatus(0);
			if (status == SyncStatus.SyncItemNotFound)
			{
				MrsTracer.Provider.Warning("Source message {0} doesn't exist", new object[]
				{
					messageId
				});
				throw new EasObjectNotFoundException(messageId);
			}
			EasConnectionWrapper.WrapException(delegate()
			{
				response.ThrowIfStatusIsFailed(status);
			}, (ConnectionsTransientException e) => new EasSyncFailedTransientException(e.Message, e), (ConnectionsPermanentException e) => new EasSyncFailedPermanentException(e.Message, e));
		}

		private SyncResponse SyncCreation(string itemId, SyncRequest syncRequest)
		{
			SyncResponse response = this.Sync(syncRequest);
			SyncStatus status = response.GetAddResponseStatus(0);
			EasConnectionWrapper.WrapException(delegate()
			{
				response.ThrowIfStatusIsFailed(status);
			}, (ConnectionsTransientException e) => new EasSyncFailedTransientException(e.Message, e), (ConnectionsPermanentException e) => new EasSyncFailedPermanentException(e.Message, e));
			return response;
		}

		private FolderCreateResponse FolderCreate(FolderCreateRequest folderCreateRequest)
		{
			return EasConnectionWrapper.WrapException<FolderCreateResponse>(() => this.wrappedObject.FolderCreate(folderCreateRequest), (ConnectionsTransientException e) => new EasFolderCreateFailedTransientException(e.Message, e), (ConnectionsPermanentException e) => new EasFolderCreateFailedPermanentException(e.Message, e));
		}

		private FolderDeleteResponse FolderDelete(FolderDeleteRequest folderDeleteRequest)
		{
			return EasConnectionWrapper.WrapException<FolderDeleteResponse>(() => this.wrappedObject.FolderDelete(folderDeleteRequest), (ConnectionsTransientException e) => new EasFolderDeleteFailedTransientException(e.Message, e), (ConnectionsPermanentException e) => new EasFolderDeleteFailedPermanentException(e.Message, e));
		}

		private FolderSyncResponse FolderSync(FolderSyncRequest folderSyncRequest)
		{
			return EasConnectionWrapper.WrapException<FolderSyncResponse>(() => this.wrappedObject.FolderSync(folderSyncRequest), (ConnectionsTransientException e) => new EasFolderSyncFailedTransientException(e.Message, e), (ConnectionsPermanentException e) => new EasFolderSyncFailedPermanentException(e.Message, e));
		}

		private FolderUpdateResponse FolderUpdate(FolderUpdateRequest folderUpdateRequest)
		{
			return EasConnectionWrapper.WrapException<FolderUpdateResponse>(() => this.wrappedObject.FolderUpdate(folderUpdateRequest), (ConnectionsTransientException e) => new EasFolderUpdateFailedTransientException(e.Message, e), (ConnectionsPermanentException e) => new EasFolderUpdateFailedPermanentException(e.Message, e));
		}

		private GetItemEstimateResponse GetItemEstimate(GetItemEstimateRequest getItemEstimateRequest)
		{
			return EasConnectionWrapper.WrapException<GetItemEstimateResponse>(() => this.wrappedObject.GetItemEstimate(getItemEstimateRequest), (ConnectionsTransientException e) => new EasCountFailedTransientException(e.Message, e), (ConnectionsPermanentException e) => new EasCountFailedPermanentException(e.Message, e));
		}

		private ItemOperationsResponse ItemOperations(ItemOperationsRequest itemOperationsRequest)
		{
			return EasConnectionWrapper.WrapException<ItemOperationsResponse>(() => this.wrappedObject.ItemOperations(itemOperationsRequest), (ConnectionsTransientException e) => new EasFetchFailedTransientException(e.Message, e), (ConnectionsPermanentException e) => new EasFetchFailedPermanentException(e.Message, e));
		}

		private MoveItemsResponse MoveItems(MoveItemsRequest moveItemsRequest)
		{
			return EasConnectionWrapper.WrapException<MoveItemsResponse>(() => this.wrappedObject.MoveItems(moveItemsRequest), (ConnectionsTransientException e) => new EasMoveFailedTransientException(e.Message, e), (ConnectionsPermanentException e) => new EasMoveFailedPermanentException(e.Message, e));
		}

		private SendMailResponse SendMail(SendMailRequest sendMailRequest)
		{
			return EasConnectionWrapper.WrapException<SendMailResponse>(() => this.wrappedObject.SendMail(sendMailRequest), (ConnectionsTransientException e) => new EasSendFailedTransientException(e.Message, e), (ConnectionsPermanentException e) => new EasSendFailedPermanentException(e.Message, e));
		}

		private SyncResponse Sync(SyncRequest syncRequest)
		{
			return EasConnectionWrapper.WrapException<SyncResponse>(() => this.wrappedObject.Sync(syncRequest), (ConnectionsTransientException e) => new EasSyncFailedTransientException(e.Message, e), (ConnectionsPermanentException e) => new EasSyncFailedPermanentException(e.Message, e));
		}

		private readonly IEasConnection wrappedObject;
	}
}
