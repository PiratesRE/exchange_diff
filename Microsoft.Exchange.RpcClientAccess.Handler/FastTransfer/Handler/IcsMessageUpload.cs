using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.RpcClientAccess.Handler;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal sealed class IcsMessageUpload : IcsUpload
	{
		public IcsMessageUpload(ReferenceCount<CoreFolder> sourceFolder, Logon logonObject) : base(sourceFolder, logonObject)
		{
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<IcsMessageUpload>(this);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.contentsSynchronizationUploadContext);
			base.InternalDispose();
		}

		internal void ImportReads(MessageReadState[] messageReadStates)
		{
			Util.ThrowOnNullArgument(messageReadStates, "messageReadStates");
			this.ImportReadsByMarkAsReadFlag(true, messageReadStates);
			this.ImportReadsByMarkAsReadFlag(false, messageReadStates);
		}

		private void ImportReadsByMarkAsReadFlag(bool markAsReadFlag, MessageReadState[] messageReadStates)
		{
			long fidFromId = base.CoreFolder.Session.IdConverter.GetFidFromId(base.CoreFolder.Id.ObjectId);
			List<StoreObjectId> list = new List<StoreObjectId>(messageReadStates.Length);
			for (int i = 0; i < messageReadStates.Length; i++)
			{
				if (messageReadStates[i].MarkAsRead == markAsReadFlag)
				{
					IcsUpload.ValidateSourceKey(messageReadStates[i].MessageId, "MessageReadState.MessageId");
					list.Add(base.CoreFolder.Session.IdConverter.CreateMessageId(fidFromId, base.CoreFolder.Session.IdConverter.GetIdFromLongTermId(messageReadStates[i].MessageId)));
				}
			}
			StoreObjectId[] array = list.ToArray();
			if (array.Length > 0)
			{
				this.EnsureSynchronizationUploadContext();
				CoreItem.SetReadFlag(this.contentsSynchronizationUploadContext, markAsReadFlag, array);
			}
		}

		internal ErrorCode ImportMessageChange(ImportMessageChangeFlags importMessageChangeFlags, PropertyValue[] propertyValues, out StoreId messageId, out Message message)
		{
			Util.ThrowOnNullArgument(propertyValues, "propertyValues");
			RopHandler.CheckEnum<ImportMessageChangeFlags>(importMessageChangeFlags);
			message = null;
			bool failOnConflict = (byte)(importMessageChangeFlags & ImportMessageChangeFlags.FailOnConflict) == 64;
			if (propertyValues.Length != IcsMessageUpload.MessageChangePropertyTags.Length)
			{
				throw new RopExecutionException(string.Format("Incorrect number of Message Change properties. Expected = {0}. Found = {1}.", IcsMessageUpload.MessageChangePropertyTags.Length, propertyValues.Length), (ErrorCode)2147942487U);
			}
			for (int i = 0; i < IcsMessageUpload.MessageChangePropertyTags.Length; i++)
			{
				if (propertyValues[i].PropertyTag != IcsMessageUpload.MessageChangePropertyTags[i])
				{
					throw new RopExecutionException(string.Format("Unexpected property tag found in Message Change properties. Expected = {0}. Found = {1}.", IcsMessageUpload.MessageChangePropertyTags[i], propertyValues[i].PropertyTag), (ErrorCode)2147942487U);
				}
			}
			byte[] value = propertyValues[0].GetValue<byte[]>();
			IcsUpload.ValidateSourceKey(value, "sourceKey");
			long idFromLongTermId = base.CoreFolder.Session.IdConverter.GetIdFromLongTermId(value);
			long fidFromId = base.CoreFolder.Session.IdConverter.GetFidFromId(base.CoreFolder.Id.ObjectId);
			StoreObjectId storeObjectId = base.CoreFolder.Session.IdConverter.CreateMessageId(fidFromId, idFromLongTermId);
			byte[] value2 = propertyValues[2].GetValue<byte[]>();
			IcsUpload.ValidateChangeKey(value2, "changeKey");
			VersionedId itemId = VersionedId.FromStoreObjectId(storeObjectId, value2);
			ExDateTime value3 = propertyValues[1].GetValue<ExDateTime>();
			byte[] value4 = propertyValues[3].GetValue<byte[]>();
			bool flag = (byte)(importMessageChangeFlags & ImportMessageChangeFlags.Associated) == 16;
			this.EnsureSynchronizationUploadContext();
			ErrorCode result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CoreItem coreItem = null;
				ImportResult importResult = CoreItem.Import(this.contentsSynchronizationUploadContext, flag ? CreateMessageType.Associated : CreateMessageType.Normal, failOnConflict, itemId, value3, value4, out coreItem);
				disposeGuard.Add<CoreItem>(coreItem);
				messageId = StoreId.Empty;
				message = null;
				if (importResult != ImportResult.Success)
				{
					result = IcsMessageUpload.ConvertImportResultToErrorCode(importResult);
				}
				else
				{
					Message message2 = new Message(coreItem, base.LogonObject, this.String8Encoding);
					message2.IgnorePropertySaveErrors();
					disposeGuard.Success();
					message = message2;
					result = ErrorCode.None;
				}
			}
			return result;
		}

		internal ErrorCode ImportMessageMove(byte[] sourceFolder, byte[] sourceMessage, byte[] predecessorChangeList, byte[] destinationMessage, byte[] destinationChangeNumber, out StoreId storeId)
		{
			IcsUpload.ValidateSourceKey(sourceFolder, "sourceFolder");
			IcsUpload.ValidateSourceKey(sourceMessage, "sourceMessage");
			Util.ThrowOnNullArgument(predecessorChangeList, "predecessorChangeList");
			IcsUpload.ValidateSourceKey(destinationMessage, "destinationMessage");
			IcsUpload.ValidateChangeKey(destinationChangeNumber, "destinationChangeNumber");
			this.EnsureSynchronizationUploadContext();
			long idFromLongTermId = base.CoreFolder.Session.IdConverter.GetIdFromLongTermId(sourceFolder);
			long idFromLongTermId2 = base.CoreFolder.Session.IdConverter.GetIdFromLongTermId(sourceMessage);
			StoreObjectId sourceItemId = base.CoreFolder.Session.IdConverter.CreateMessageId(idFromLongTermId, idFromLongTermId2);
			long fidFromId = base.CoreFolder.Session.IdConverter.GetFidFromId(base.CoreFolder.Id.ObjectId);
			long idFromLongTermId3 = base.CoreFolder.Session.IdConverter.GetIdFromLongTermId(destinationMessage);
			StoreObjectId storeObjectId = base.CoreFolder.Session.IdConverter.CreateMessageId(fidFromId, idFromLongTermId3);
			VersionedId destinationItemId = VersionedId.FromStoreObjectId(storeObjectId, destinationChangeNumber);
			ImportResult result = CoreItem.Move(this.contentsSynchronizationUploadContext, sourceItemId, predecessorChangeList, destinationItemId);
			storeId = StoreId.Empty;
			return IcsMessageUpload.ConvertImportResultToErrorCode(result);
		}

		internal override ErrorCode InternalImportDelete(ImportDeleteFlags importDeleteFlags, byte[][] sourceKeys)
		{
			IcsMessageUpload.<>c__DisplayClass1 CS$<>8__locals1 = new IcsMessageUpload.<>c__DisplayClass1();
			CS$<>8__locals1.<>4__this = this;
			if ((byte)(importDeleteFlags & ImportDeleteFlags.Hierarchy) == 1)
			{
				throw new RopExecutionException("Unable to delete folder(s) using a contents collector.", (ErrorCode)2147746050U);
			}
			if (sourceKeys.Length == 0)
			{
				return ErrorCode.None;
			}
			long fidFromId = base.CoreFolder.Session.IdConverter.GetFidFromId(base.CoreFolder.Id.ObjectId);
			CS$<>8__locals1.storeObjectIds = new StoreObjectId[sourceKeys.Length];
			int num = 0;
			foreach (byte[] array in sourceKeys)
			{
				IcsUpload.ValidateSourceKey(array, "messageSourceKey");
				long idFromLongTermId = base.CoreFolder.Session.IdConverter.GetIdFromLongTermId(array);
				CS$<>8__locals1.storeObjectIds[num++] = base.CoreFolder.Session.IdConverter.CreateMessageId(fidFromId, idFromLongTermId);
			}
			this.EnsureSynchronizationUploadContext();
			if (!TeamMailboxClientOperations.IsLinkedFolder(base.CoreFolder, false))
			{
				OperationResult result = CoreItem.Delete(this.contentsSynchronizationUploadContext, IcsUpload.GetXsoDeleteItemFlags(importDeleteFlags), CS$<>8__locals1.storeObjectIds);
				return IcsUpload.ConvertGroupOperationResultToErrorCode(result);
			}
			if (!Configuration.ServiceConfiguration.TMPublishEnabled)
			{
				throw new RopExecutionException("Shortcut folder feature is turned off", (ErrorCode)2147746050U);
			}
			TeamMailboxClientOperations teamMailboxClientOperations = TeamMailboxExecutionHelper.GetTeamMailboxClientOperations(base.LogonObject.Connection);
			GroupOperationResult groupOperationResult = TeamMailboxExecutionHelper.RunGroupOperationsWithExecutionLimitHandler(() => teamMailboxClientOperations.OnImportDeleteMessages(CS$<>8__locals1.<>4__this.CoreFolder, CS$<>8__locals1.<>4__this.contentsSynchronizationUploadContext, CS$<>8__locals1.storeObjectIds), "TeamMailboxClientOperations.OnImportDeleteMessages");
			return IcsUpload.ConvertGroupOperationResultToErrorCode(groupOperationResult.OperationResult);
		}

		internal override void UpdateState()
		{
			this.EnsureSynchronizationUploadContext();
			ISession session = new SessionAdaptor(base.CoreFolder.Session);
			IPropertyBag propertyBag = new MemoryPropertyBag(session);
			IcsStateStream icsStateStream = new IcsStateStream(propertyBag);
			StorageIcsState state = icsStateStream.ToXsoState();
			this.contentsSynchronizationUploadContext.GetCurrentState(ref state);
			icsStateStream.FromXsoState(state);
			base.IcsStateHelper.IcsState.Load(IcsStateOrigin.ServerIncremental, propertyBag);
		}

		internal static ErrorCode ConvertImportResultToErrorCode(ImportResult result)
		{
			switch (result)
			{
			case ImportResult.Success:
				return ErrorCode.None;
			case ImportResult.SyncClientChangeNewer:
				return ErrorCode.SyncClientChangeNewer;
			case ImportResult.SyncObjectDeleted:
				return (ErrorCode)2147747840U;
			case ImportResult.SyncIgnore:
				return (ErrorCode)2147747841U;
			case ImportResult.SyncConflict:
				return (ErrorCode)2147747842U;
			case ImportResult.NotFound:
				return (ErrorCode)2147746063U;
			case ImportResult.ObjectModified:
				return (ErrorCode)2147746057U;
			case ImportResult.ObjectDeleted:
				return (ErrorCode)2147746058U;
			default:
				return (ErrorCode)2147500037U;
			}
		}

		private void EnsureSynchronizationUploadContext()
		{
			if (this.contentsSynchronizationUploadContext == null)
			{
				ISession session = new SessionAdaptor(base.CoreFolder.Session);
				IPropertyBag propertyBag = new MemoryPropertyBag(session);
				base.IcsStateHelper.IcsState.Checkpoint(propertyBag);
				IcsStateStream icsStateStream = new IcsStateStream(propertyBag);
				this.contentsSynchronizationUploadContext = new ContentsSynchronizationUploadContext(base.CoreFolder, icsStateStream.ToXsoState());
			}
		}

		private static readonly PropertyTag[] MessageChangePropertyTags = new PropertyTag[]
		{
			PropertyTag.SourceKey,
			PropertyTag.LastModificationTime,
			PropertyTag.ChangeKey,
			PropertyTag.PredecessorChangeList
		};

		private ContentsSynchronizationUploadContext contentsSynchronizationUploadContext;

		internal enum MessageChangePropertyTagIndex
		{
			SourceKey,
			LastModificationTime,
			ChangeKey,
			PredecessorChangeList
		}
	}
}
