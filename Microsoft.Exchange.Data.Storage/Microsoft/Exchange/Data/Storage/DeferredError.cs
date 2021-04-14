using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DeferredError : IDisposable
	{
		private DeferredError()
		{
		}

		public static DeferredError Create(MailboxSession session, StoreObjectId folderId, string providerName, long ruleId, RuleAction.Type actionType, int actionNumber, DeferredError.RuleError ruleError)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(folderId, "folderId");
			Util.ThrowOnNullArgument(providerName, "providerName");
			EnumValidator.ThrowIfInvalid<RuleAction.Type>(actionType, "actionType");
			EnumValidator.ThrowIfInvalid<DeferredError.RuleError>(ruleError, "ruleError");
			if (!IdConverter.IsFolderId(folderId))
			{
				throw new ArgumentException(ServerStrings.InvalidFolderId(folderId.ToBase64String()));
			}
			DeferredError deferredError = new DeferredError();
			deferredError.message = MessageItem.Create(session, session.GetDefaultFolderId(DefaultFolderType.DeferredActionFolder));
			deferredError.message[InternalSchema.ItemClass] = "IPC.Microsoft Exchange 4.0.Deferred Error";
			deferredError.message[InternalSchema.RuleFolderEntryId] = folderId.ProviderLevelItemId;
			deferredError.message[InternalSchema.RuleId] = ruleId;
			deferredError.message[InternalSchema.RuleActionType] = (int)actionType;
			deferredError.message[InternalSchema.RuleActionNumber] = actionNumber;
			deferredError.message[InternalSchema.RuleError] = ruleError;
			deferredError.message[InternalSchema.RuleProvider] = providerName;
			return deferredError;
		}

		public byte[] Save()
		{
			this.CheckDisposed("Save");
			this.message.Save(SaveMode.NoConflictResolution);
			this.message.Load(DeferredError.EntryId);
			return this.message[InternalSchema.EntryId] as byte[];
		}

		public MessageItem Message
		{
			get
			{
				this.CheckDisposed("Message::get");
				return this.message;
			}
		}

		protected void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.InternalDispose(disposing);
			}
		}

		protected void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.message.Dispose();
			}
		}

		private const string DaeMsgClass = "IPC.Microsoft Exchange 4.0.Deferred Error";

		public static readonly PropertyDefinition[] EntryId = new PropertyDefinition[]
		{
			StoreObjectSchema.EntryId
		};

		private MessageItem message;

		private bool isDisposed;

		public enum RuleError
		{
			Unknown = 1,
			Load,
			Delivery,
			Parsing,
			CreateDae,
			NoFolder,
			NoRights,
			CreateDam,
			NoSendAs,
			NoTemplateId,
			Execution,
			QuotaExceeded,
			TooManyRecips,
			FolderOverQuota
		}
	}
}
