using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.FastTransfer;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class IcsContentUploadContext : IcsUploadContext
	{
		public override ErrorCode Configure(MapiContext operationContext, MapiFolder folder)
		{
			folder.GhostedFolderCheck(operationContext, (LID)34617U);
			ErrorCode errorCode = base.Configure(operationContext, folder);
			if (errorCode == ErrorCode.NoError)
			{
				base.ParentObject = folder;
			}
			return errorCode;
		}

		public ErrorCode ImportMessageChange(MapiContext operationContext, Properties headerProps, ImportMessageChangeFlags flags, bool partialChange, out MapiMessage icsMessage, out ExchangeId outputMessageId)
		{
			base.CheckDisposed();
			base.TraceInitialState(operationContext);
			icsMessage = null;
			outputMessageId = ExchangeId.Zero;
			ErrorCode errorCode = this.ValidateMessageChangeParams(headerProps, flags, partialChange);
			if (errorCode != ErrorCode.NoError)
			{
				return errorCode;
			}
			byte[] array = headerProps[0].Value as byte[];
			byte[] array2 = headerProps[2].Value as byte[];
			byte[] array3 = headerProps[3].Value as byte[];
			DateTime dateTime = (DateTime)headerProps[1].Value;
			if (ExTraceGlobals.IcsUploadStateTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("ImportMessageChange: SourceKey=[");
				stringBuilder.AppendAsString(array);
				stringBuilder.Append("] ChangeKey=[");
				stringBuilder.AppendAsString(array2);
				stringBuilder.Append("] PCL=[");
				stringBuilder.AppendAsString(array3);
				stringBuilder.Append("] LastModified=[");
				stringBuilder.AppendAsString(dateTime);
				stringBuilder.Append("]");
				ExTraceGlobals.IcsUploadStateTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			MapiFolder mapiFolder = base.ParentObject as MapiFolder;
			MapiMessage mapiMessage = null;
			ExchangeId exchangeId = base.MessageIdFromSourceKey(operationContext, mapiFolder.Fid, ref array);
			PCL pcl = default(PCL);
			if (!pcl.TryLoadBinaryLXCN(array3))
			{
				return ErrorCode.CreateInvalidParameter((LID)62360U);
			}
			pcl.Add(array2);
			try
			{
				MessageConfigureFlags messageConfigureFlags = MessageConfigureFlags.RequestWrite;
				if ((byte)(flags & ImportMessageChangeFlags.Associated) != 0)
				{
					messageConfigureFlags |= MessageConfigureFlags.IsAssociated;
				}
				bool flag = false;
				if (exchangeId.IsValid)
				{
					mapiMessage = new MapiMessage();
					errorCode = mapiMessage.ConfigureMessage(operationContext, base.Logon, mapiFolder.Fid, exchangeId, messageConfigureFlags, base.Logon.Session.CodePage);
					if (errorCode != ErrorCode.NoError)
					{
						if (errorCode != ErrorCodeValue.NotFound)
						{
							return errorCode;
						}
						mapiMessage.Dispose();
						mapiMessage = null;
						IdSet midsetDeleted = mapiFolder.StoreFolder.GetMidsetDeleted(operationContext);
						if (midsetDeleted.Contains(exchangeId))
						{
							return ErrorCode.CreateSyncObjectDeleted((LID)52120U);
						}
					}
				}
				if (mapiMessage == null)
				{
					mapiMessage = new MapiMessage();
					messageConfigureFlags |= MessageConfigureFlags.CreateNewMessage;
					errorCode = mapiMessage.ConfigureMessage(operationContext, base.Logon, mapiFolder.Fid, exchangeId, messageConfigureFlags, base.Logon.Session.CodePage);
					if (errorCode != ErrorCode.NoError)
					{
						return errorCode;
					}
					flag = true;
					if (exchangeId.IsValid)
					{
						mapiMessage.SetMessageId(operationContext, exchangeId);
					}
					if (array != null)
					{
						mapiMessage.SetSourceKey(operationContext, array);
					}
				}
				bool flag2 = (byte)(flags & ImportMessageChangeFlags.FailOnConflict) != 0;
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				PCL pcl2 = pcl;
				if (!flag)
				{
					PCL pcl3 = default(PCL);
					pcl3.LoadBinaryLXCN((byte[])mapiMessage.GetOnePropValue(operationContext, PropTag.Message.PCL));
					if (pcl3.IgnoreChange(pcl))
					{
						return ErrorCode.CreateSyncIgnore((LID)45976U);
					}
					flag3 = !pcl.IgnoreChange(pcl3);
					if (flag3)
					{
						if (flag2)
						{
							return ErrorCode.CreateSyncConflict((LID)35736U);
						}
						if (!flag5)
						{
							flag4 = MapiMessage.PickConflictWinner(dateTime, array2, mapiMessage.GetLastModificationTime(operationContext), mapiMessage.GetChangeKey(operationContext));
							if (flag4)
							{
								pcl3.Merge(pcl);
								pcl2 = pcl3;
							}
						}
					}
					else
					{
						pcl3.Merge(pcl);
						pcl2 = pcl3;
					}
					if (!partialChange)
					{
						mapiMessage.Scrub(operationContext);
					}
				}
				mapiMessage.SetIcsImportState(pcl2, array2, dateTime, mapiMessage.GetAssociated(operationContext) ? base.IcsState.CnsetSeenAssociated : base.IcsState.CnsetSeen, flag3, flag4, flag5, flag2);
				icsMessage = mapiMessage;
				mapiMessage = null;
			}
			finally
			{
				if (mapiMessage != null)
				{
					mapiMessage.Dispose();
				}
			}
			return ErrorCode.NoError;
		}

		public ErrorCode ImportMessageMove(MapiContext operationContext, byte[] sourceFolderSourceKey, byte[] messageSourceKey, byte[] pcl, byte[] destinationMessageSourceKey, byte[] changeKey)
		{
			base.CheckDisposed();
			base.TraceInitialState(operationContext);
			if (ExTraceGlobals.IcsUploadStateTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("ImportMessageMove: SrcFolder=[");
				stringBuilder.AppendAsString(sourceFolderSourceKey);
				stringBuilder.Append("] SrcMsg=[");
				stringBuilder.AppendAsString(messageSourceKey);
				stringBuilder.Append("] PCL=[");
				stringBuilder.AppendAsString(pcl);
				stringBuilder.Append("] ChangeKey=[");
				stringBuilder.AppendAsString(changeKey);
				stringBuilder.Append("] DestMsgSourceKey=[");
				stringBuilder.AppendAsString(destinationMessageSourceKey);
				stringBuilder.Append("]");
				ExTraceGlobals.IcsUploadStateTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			ErrorCode errorCode = this.ValidateMessageMoveParams(sourceFolderSourceKey, messageSourceKey, pcl, destinationMessageSourceKey, changeKey);
			if (errorCode != ErrorCode.NoError)
			{
				return errorCode;
			}
			MapiFolder mapiFolder = base.ParentObject as MapiFolder;
			ExchangeId exchangeId = base.FolderIdFromSourceKey(operationContext, ref sourceFolderSourceKey);
			if (exchangeId.IsNullOrZero)
			{
				return ErrorCode.CreateNotFound((LID)58264U);
			}
			if (sourceFolderSourceKey == null)
			{
				using (MapiFolder mapiFolder2 = MapiFolder.OpenFolder(operationContext, base.Logon, exchangeId))
				{
					if (mapiFolder2 == null)
					{
						return ErrorCode.CreateNotFound((LID)41880U);
					}
				}
			}
			ExchangeId mid = base.MessageIdFromSourceKey(operationContext, exchangeId, ref messageSourceKey);
			if (mid.IsNullOrZero)
			{
				return ErrorCode.CreateSyncObjectDeleted((LID)51285U);
			}
			PCL remotePCL = default(PCL);
			if (!remotePCL.TryLoadBinaryLXCN(pcl))
			{
				return ErrorCode.CreateInvalidParameter((LID)36949U);
			}
			using (MapiMessage mapiMessage = new MapiMessage())
			{
				errorCode = mapiMessage.ConfigureMessage(operationContext, base.Logon, exchangeId, mid, MessageConfigureFlags.RequestWrite, base.Logon.Session.CodePage);
				if (errorCode != ErrorCode.NoError)
				{
					if (errorCode == ErrorCodeValue.NotFound)
					{
						errorCode = ErrorCode.CreateSyncObjectDeleted((LID)37784U);
					}
					return errorCode;
				}
				ExchangeId exchangeId2 = base.MessageIdFromSourceKey(operationContext, mapiFolder.Fid, ref destinationMessageSourceKey);
				if (exchangeId2.IsValid)
				{
					if (destinationMessageSourceKey != null)
					{
						return ErrorCode.CreateNotFound((LID)45141U);
					}
					if (TopMessage.GetDocumentIdFromId(operationContext, base.Logon.StoreMailbox, mapiFolder.Fid, exchangeId2) != null)
					{
						return ErrorCode.CreateNotFound((LID)61525U);
					}
				}
				PCL pcl2 = default(PCL);
				pcl2.LoadBinaryLXCN((byte[])mapiMessage.GetOnePropValue(operationContext, PropTag.Message.PCL));
				bool flag = remotePCL.IgnoreChange(pcl2);
				bool flag2 = pcl2.IgnoreChange(remotePCL);
				if (flag && flag2)
				{
					pcl2.Add(changeKey);
				}
				else
				{
					changeKey = null;
				}
				errorCode = mapiMessage.ImportMove(operationContext, mapiFolder, exchangeId2, destinationMessageSourceKey, pcl2, changeKey);
				if (errorCode != ErrorCode.NoError)
				{
					return errorCode.Propagate((LID)58204U);
				}
				if (flag)
				{
					ExchangeId lcnCurrent = ((TopMessage)mapiMessage.StoreMessage).GetLcnCurrent(operationContext);
					IdSet cnsetSeen = mapiFolder.StoreFolder.GetCnsetSeen(operationContext);
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(cnsetSeen.Contains(lcnCurrent), "the CN must be in a folder's CnsetSeen");
					if (mapiMessage.GetAssociated(operationContext))
					{
						base.IcsState.CnsetSeenAssociated.Insert(lcnCurrent);
					}
					else
					{
						base.IcsState.CnsetSeen.Insert(lcnCurrent);
					}
				}
				base.IcsState.IdsetGiven.Insert(exchangeId2);
				if (!flag2)
				{
					return ErrorCode.CreateSyncClientChangeNewer((LID)54168U);
				}
			}
			return ErrorCode.NoError;
		}

		public ErrorCode ImportReads(MapiContext operationContext, MessageReadState[] readStates)
		{
			base.CheckDisposed();
			base.TraceInitialState(operationContext);
			MapiFolder mapiFolder = base.ParentObject as MapiFolder;
			List<ExchangeId> list = new List<ExchangeId>(readStates.Length);
			List<ExchangeId> list2 = new List<ExchangeId>(readStates.Length);
			for (int i = 0; i < readStates.Length; i++)
			{
				List<ExchangeId> list3 = readStates[i].MarkAsRead ? list : list2;
				byte[] messageId = readStates[i].MessageId;
				ExchangeId item = base.MessageIdFromSourceKey(operationContext, mapiFolder.Fid, ref messageId);
				if (item.IsValid)
				{
					list3.Add(item);
				}
			}
			List<ExchangeId> list4 = new List<ExchangeId>(readStates.Length);
			if (list.Count > 0)
			{
				using (SetReadFlagsOperation setReadFlagsOperation = new SetReadFlagsOperation(mapiFolder, list, SetReadFlagFlags.Read, list4))
				{
					setReadFlagsOperation.DoAll(operationContext, false);
				}
			}
			if (list2.Count > 0)
			{
				using (SetReadFlagsOperation setReadFlagsOperation2 = new SetReadFlagsOperation(mapiFolder, list2, SetReadFlagFlags.ClearReadFlag, list4))
				{
					setReadFlagsOperation2.DoAll(operationContext, false);
				}
			}
			if (mapiFolder.StoreFolder.IsPerUserReadUnreadTrackingEnabled && operationContext.UserIdentity != Guid.Empty)
			{
				PerUser perUser = PerUser.LoadResident(operationContext, mapiFolder.Logon.StoreMailbox, operationContext.UserIdentity, mapiFolder.Fid);
				if (perUser != null)
				{
					using (LockManager.Lock(perUser, LockManager.LockType.PerUserShared, operationContext.Diagnostics))
					{
						base.IcsState.CnsetRead = perUser.CNSet.Clone();
						goto IL_170;
					}
				}
				base.IcsState.CnsetRead = new IdSet();
				IL_170:
				base.IcsState.CnsetRead.Insert(IcsState.PerUserIdsetIndicator);
			}
			else
			{
				if (base.IcsState.CnsetRead.Remove(IcsState.PerUserIdsetIndicator))
				{
					base.IcsState.CnsetRead = new IdSet();
				}
				foreach (ExchangeId id in list4)
				{
					base.IcsState.CnsetRead.Insert(id);
				}
				base.IcsState.CnsetRead.Insert(IcsState.NonPerUserIdsetIndicator);
			}
			return ErrorCode.NoError;
		}

		public override ErrorCode ImportDelete(MapiContext operationContext, byte[][] sourceKeys)
		{
			base.CheckDisposed();
			base.TraceInitialState(operationContext);
			MapiFolder mapiFolder = base.ParentObject as MapiFolder;
			List<ExchangeId> list = new List<ExchangeId>(sourceKeys.Length);
			foreach (byte[] array in sourceKeys)
			{
				ExchangeId item = base.MessageIdFromSourceKey(operationContext, mapiFolder.Fid, ref array);
				if (item.IsValid)
				{
					list.Add(item);
				}
			}
			if (list.Count > 0)
			{
				using (DeleteMessagesOperation deleteMessagesOperation = new DeleteMessagesOperation(mapiFolder, list, true))
				{
					deleteMessagesOperation.DoAll(operationContext, false);
				}
				for (int j = 0; j < list.Count; j++)
				{
					base.IcsState.IdsetGiven.Remove(list[j]);
				}
			}
			return ErrorCode.NoError;
		}

		private ErrorCode ValidateMessageChangeParams(Properties headerProps, ImportMessageChangeFlags flags, bool partialChange)
		{
			if (partialChange && (byte)(flags & ImportMessageChangeFlags.FailOnConflict) == 0)
			{
				return ErrorCode.CreateInvalidParameter((LID)40533U);
			}
			if (headerProps.Count != IcsContentUploadContext.changeProperties.Length)
			{
				return ErrorCode.CreateInvalidParameter((LID)47256U);
			}
			for (int i = 0; i < headerProps.Count; i++)
			{
				if (headerProps[i].Tag != IcsContentUploadContext.changeProperties[i])
				{
					return ErrorCode.CreateInvalidParameter((LID)39064U);
				}
			}
			if (!IcsUploadContext.ValidChangeKey(headerProps[2].Value as byte[]))
			{
				return ErrorCode.CreateInvalidParameter((LID)63640U);
			}
			if (!IcsUploadContext.ValidSourceKey(headerProps[0].Value as byte[]))
			{
				return ErrorCode.CreateInvalidParameter((LID)55448U);
			}
			return ErrorCode.NoError;
		}

		private ErrorCode ValidateMessageMoveParams(byte[] sourceFolderSourceKey, byte[] messageSourceKey, byte[] pcl, byte[] destinationMessageSourceKey, byte[] changeKey)
		{
			if (pcl == null)
			{
				return ErrorCode.CreateInvalidParameter((LID)53333U);
			}
			if (!IcsUploadContext.ValidChangeKey(changeKey))
			{
				return ErrorCode.CreateInvalidParameter((LID)41045U);
			}
			if (!IcsUploadContext.ValidSourceKey(sourceFolderSourceKey) || !IcsUploadContext.ValidSourceKey(messageSourceKey) || !IcsUploadContext.ValidSourceKey(destinationMessageSourceKey))
			{
				return ErrorCode.CreateInvalidParameter((LID)57429U);
			}
			return ErrorCode.NoError;
		}

		private static StorePropTag[] changeProperties = new StorePropTag[]
		{
			PropTag.Message.SourceKey,
			PropTag.Message.LastModificationTime,
			PropTag.Message.ChangeKey,
			PropTag.Message.PredecessorChangeList
		};

		private enum ChangePropertyIndex
		{
			SourceKey,
			LastModificationTime,
			ChangeKey,
			PredecessorChangeList
		}
	}
}
