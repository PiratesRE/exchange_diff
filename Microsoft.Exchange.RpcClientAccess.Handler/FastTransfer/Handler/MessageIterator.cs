using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Handler;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal class MessageIterator : ServerObject, IMessageIterator, IDisposable, WatsonHelper.IProvideWatsonReportData
	{
		internal MessageIterator(Logon logon, StoreId folderId, StoreId[] messageIds, FastTransferCopyMessagesFlag flags, FastTransferSendOption sendOptions) : base(logon)
		{
			this.folderId = folderId;
			this.messageIds = messageIds;
			this.flags = flags;
			this.sendOptions = sendOptions;
			if (Activity.Current != null)
			{
				this.watsonReportActionGuard = Activity.Current.RegisterWatsonReportDataProviderAndGetGuard(WatsonReportActionType.MessageAdaptor, this);
			}
		}

		public IEnumerator<IMessage> GetMessages()
		{
			MessageAdaptor.Options options = new MessageAdaptor.Options
			{
				IsReadOnly = true,
				IsEmbedded = false,
				SendEntryId = ((byte)(this.flags & FastTransferCopyMessagesFlag.SendEntryId) == 32),
				DownloadBodyOption = (((byte)(this.flags & FastTransferCopyMessagesFlag.BestBody) == 16) ? DownloadBodyOption.BestBodyOnly : DownloadBodyOption.RtfOnly),
				IsUpload = this.sendOptions.IsUpload()
			};
			int messagesPreread = 0;
			int messagesTillNextPreread = 25;
			this.PrereadMessages(ref messagesPreread);
			foreach (StoreId messageId in this.messageIds)
			{
				IMessage message = null;
				if (this.TryGetMessage(messageId, options, out message))
				{
					yield return message;
				}
				else
				{
					yield return null;
				}
				messagesTillNextPreread--;
				if (messagesTillNextPreread == 0)
				{
					this.PrereadMessages(ref messagesPreread);
					messagesTillNextPreread = 50;
				}
			}
			yield break;
		}

		private static string StoreIdsToString(StoreId[] ids)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (StoreId storeId in ids)
			{
				stringBuilder.Append("[");
				stringBuilder.Append(storeId.ToString());
				stringBuilder.Append("] ");
			}
			return stringBuilder.ToString();
		}

		private bool TryGetMessage(StoreId messageId, MessageAdaptor.Options options, out IMessage message)
		{
			message = null;
			bool result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CoreItem coreItem = null;
				try
				{
					coreItem = CoreItem.Bind(base.LogonObject.Session, base.LogonObject.Session.IdConverter.CreateMessageId(this.folderId, messageId), CoreObjectSchema.AllPropertiesOnStore);
				}
				catch (ObjectNotFoundException)
				{
				}
				catch (AccessDeniedException)
				{
				}
				if (coreItem == null)
				{
					result = false;
				}
				else
				{
					disposeGuard.Add<CoreItem>(coreItem);
					if (options.SkipMessagesInConflict && coreItem.PropertyBag.GetValueAsNullable<bool>(MessageItemSchema.MessageInConflict) == true)
					{
						result = false;
					}
					else
					{
						ReferenceCount<CoreItem> referenceCount = new ReferenceCount<CoreItem>(coreItem);
						try
						{
							IMessage message2 = new MessageAdaptor(referenceCount, options, base.LogonObject.LogonString8Encoding, this.sendOptions.IsUpload(), null);
							disposeGuard.Add<IMessage>(message2);
							PropertyValue propertyValue = message2.PropertyBag.GetAnnotatedProperty(PropertyTag.Subject).PropertyValue;
							this.lastMessageForTracing = (propertyValue.IsError ? null : propertyValue.GetValueAssert<string>());
							disposeGuard.Success();
							message = message2;
							result = true;
						}
						finally
						{
							referenceCount.Release();
						}
					}
				}
			}
			return result;
		}

		private void PrereadMessages(ref int messagesPreread)
		{
			PrivateLogon privateLogon = base.LogonObject as PrivateLogon;
			if (privateLogon != null && messagesPreread < this.messageIds.Length)
			{
				int num = Math.Min(this.messageIds.Length - messagesPreread, 50);
				if (num > 1)
				{
					StoreId[] array = new StoreId[num];
					for (int i = 0; i < num; i++)
					{
						array[i] = base.LogonObject.Session.IdConverter.CreateMessageId(this.folderId, this.messageIds[messagesPreread + i]);
					}
					privateLogon.MailboxSession.PrereadMessages(array);
					messagesPreread += num;
				}
			}
		}

		protected override void InternalDispose()
		{
			base.InternalDispose();
			Util.DisposeIfPresent(this.watsonReportActionGuard);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MessageIterator>(this);
		}

		string WatsonHelper.IProvideWatsonReportData.GetWatsonReportString()
		{
			base.CheckDisposed();
			return string.Format("MessageIterator: MessageIds: {0}\r\nLast Message = \"{1}\".", MessageIterator.StoreIdsToString(this.messageIds), this.lastMessageForTracing);
		}

		private const int MaximumMessagePrereadSize = 50;

		private readonly StoreId folderId;

		private readonly StoreId[] messageIds;

		private readonly FastTransferCopyMessagesFlag flags;

		private readonly FastTransferSendOption sendOptions;

		private readonly IDisposable watsonReportActionGuard;

		private string lastMessageForTracing;
	}
}
