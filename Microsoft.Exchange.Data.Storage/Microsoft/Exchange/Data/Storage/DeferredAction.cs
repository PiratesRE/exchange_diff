using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DeferredAction : IDisposable
	{
		private DeferredAction()
		{
		}

		internal static DeferredAction Create(MailboxSession session, StoreObjectId ruleFolderId, string providerName)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(ruleFolderId, "ruleFolderId");
			Util.ThrowOnNullArgument(providerName, "providerName");
			if (!IdConverter.IsFolderId(ruleFolderId))
			{
				throw new ArgumentException(ServerStrings.InvalidFolderId(ruleFolderId.ToBase64String()));
			}
			DeferredAction deferredAction = new DeferredAction();
			deferredAction.actions = new List<RuleAction>();
			deferredAction.ruleIds = new List<long>();
			deferredAction.message = MessageItem.Create(session, session.GetDefaultFolderId(DefaultFolderType.DeferredActionFolder));
			deferredAction.message[InternalSchema.ItemClass] = "IPC.Microsoft Exchange 4.0.Deferred Action";
			deferredAction.message[InternalSchema.RuleFolderEntryId] = ruleFolderId.ProviderLevelItemId;
			deferredAction.message[InternalSchema.RuleProvider] = providerName;
			return deferredAction;
		}

		public void ClearActions()
		{
			this.CheckDisposed("ClearActions");
			this.actions.Clear();
			this.ruleIds.Clear();
		}

		public void AddAction(long ruleId, RuleAction action)
		{
			this.CheckDisposed("AddAction");
			Util.ThrowOnNullArgument(action, "action");
			this.ruleIds.Add(ruleId);
			this.actions.Add(action);
		}

		public void SerializeActionsAndSave()
		{
			this.CheckDisposed("SerializeActionsAndSave");
			byte[] buffer = null;
			StoreSession session = this.message.Session;
			bool flag = false;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				buffer = this.message.Session.Mailbox.MapiStore.MapActionsToMDBActions(this.actions.ToArray());
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ErrorSavingRules, ex, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("DeferredAction.SerializeActionsAndSave.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.ErrorSavingRules, ex2, session, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("DeferredAction.SerializeActionsAndSave.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			using (Stream stream = this.Message.OpenPropertyStream(InternalSchema.ClientActions, PropertyOpenMode.Create))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(stream))
				{
					binaryWriter.Write(buffer);
					binaryWriter.Flush();
				}
			}
			byte[] array = new byte[this.ruleIds.Count * 8];
			for (int i = 0; i < this.ruleIds.Count; i++)
			{
				Array.Copy(BitConverter.GetBytes(this.ruleIds[i]), 0, array, 8 * i, 8);
			}
			this.Message[InternalSchema.RuleIds] = array;
			this.message.Save(SaveMode.NoConflictResolution);
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

		private const string DamMsgClass = "IPC.Microsoft Exchange 4.0.Deferred Action";

		private const int RuleIdSize = 8;

		private MessageItem message;

		private List<RuleAction> actions;

		private List<long> ruleIds;

		private bool isDisposed;
	}
}
