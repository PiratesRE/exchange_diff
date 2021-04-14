using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class IdConverter
	{
		internal IdConverter(StoreSession session)
		{
			this.session = session;
		}

		public static bool IsFromPublicStore(byte[] entryId)
		{
			Util.ThrowOnNullArgument(entryId, "entryId");
			StoreSession storeSession = null;
			object thisObject = null;
			bool flag = false;
			bool result;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				try
				{
					result = MapiStore.IsPublicEntryId(entryId);
				}
				catch (MapiExceptionInvalidEntryId innerException)
				{
					throw new ArgumentException("Id is invalid.", innerException);
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetParentEntryId, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to check if entry id is from public store.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetParentEntryId, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to check if entry id is from public store.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
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
			return result;
		}

		public static bool IsFromPublicStore(StoreObjectId id)
		{
			Util.ThrowOnNullArgument(id, "id");
			return IdConverter.IsFromPublicStore(id.ProviderLevelItemId);
		}

		public static void ExpandIdSet(IdSet idset, Action<byte[]> action)
		{
			foreach (GuidGlobCountSet guidGlobCountSet in idset)
			{
				foreach (GlobCountRange globCountRange in guidGlobCountSet.GlobCountSet)
				{
					for (ulong num = globCountRange.LowBound; num <= globCountRange.HighBound; num += 1UL)
					{
						byte[] array = new byte[22];
						int dstOffset = ExBitConverter.Write(guidGlobCountSet.Guid, array, 0);
						Buffer.BlockCopy(IdConverter.GlobcntIntoByteArray(num), 0, array, dstOffset, 6);
						action(array);
					}
				}
			}
		}

		public static GuidGlobCount GuidGlobCountFromEntryId(byte[] entryId)
		{
			return IdConverter.ExtractGuidGlobCountFromEntryId(entryId, 22);
		}

		public static GuidGlobCount MessageGuidGlobCountFromEntryId(byte[] entryId)
		{
			if (!IdConverter.IsMessageId(entryId))
			{
				throw new CorruptDataException(ServerStrings.MapiInvalidId, new ArgumentException("Invalid message id size.", "entryId"));
			}
			return IdConverter.ExtractGuidGlobCountFromEntryId(entryId, 46);
		}

		public static bool IsValidMessageEntryId(byte[] entryId)
		{
			if (!IdConverter.IsMessageId(entryId))
			{
				return false;
			}
			for (int i = 62; i < 68; i++)
			{
				if (entryId[i] != 0)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsMessageId(StoreObjectId storeObjectId)
		{
			Util.ThrowOnNullArgument(storeObjectId, "storeObjectId");
			return storeObjectId.IsMessageId;
		}

		public static bool IsMessageId(byte[] entryId)
		{
			return entryId != null && entryId.Length == 70;
		}

		public static bool IsFolderId(StoreObjectId storeObjectId)
		{
			Util.ThrowOnNullArgument(storeObjectId, "storeObjectId");
			return storeObjectId.IsFolderId;
		}

		public static bool IsFolderId(byte[] entryId)
		{
			return entryId != null && entryId.Length == 46;
		}

		public static byte[] GetParentEntryIdFromMessageEntryId(byte[] messageEntryId)
		{
			Util.ThrowOnNullArgument(messageEntryId, "messageEntryId");
			if (!IdConverter.IsMessageId(messageEntryId))
			{
				throw new ArgumentException(ServerStrings.ExOnlyMessagesHaveParent);
			}
			StoreSession storeSession = null;
			object thisObject = null;
			bool flag = false;
			byte[] folderEntryIdFromMessageEntryId;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				try
				{
					folderEntryIdFromMessageEntryId = MapiStore.GetFolderEntryIdFromMessageEntryId(messageEntryId);
				}
				catch (MapiExceptionInvalidEntryId innerException)
				{
					throw new ArgumentException("Id is invalid.", innerException);
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetParentEntryId, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to get the parent entry id for the message.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetParentEntryId, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to get the parent entry id for the message.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
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
			return folderEntryIdFromMessageEntryId;
		}

		public static StoreObjectId GetParentIdFromMessageId(StoreObjectId messageId)
		{
			Util.ThrowOnNullArgument(messageId, "messageId");
			return StoreObjectId.FromProviderSpecificId(IdConverter.GetParentEntryIdFromMessageEntryId(messageId.ProviderLevelItemId));
		}

		public static byte[] CreateEntryIdFromForeignServerId(byte[] serverId)
		{
			if (serverId == null)
			{
				throw new ArgumentNullException("serverId");
			}
			if (serverId.Length > 1 && serverId[0] == 0)
			{
				byte[] array = new byte[serverId.Length - 1];
				Array.Copy(serverId, 1, array, 0, serverId.Length - 1);
				return array;
			}
			throw new CorruptDataException(ServerStrings.MapiInvalidId, new ArgumentException("Invalid foreign serverId, the first byte should be 0.", "serverId"));
		}

		public StoreObjectId CreateMessageIdFromSvrEId(byte[] svrEId)
		{
			long num = 0L;
			long num2 = 0L;
			int num3 = 0;
			if (IdConverter.ParseOursServerEntryId(svrEId, out num, out num2, out num3) && num != 0L && num2 != 0L && num3 == 0)
			{
				return this.CreateMessageId(num, num2);
			}
			throw new CorruptDataException(ServerStrings.MapiInvalidId, new ArgumentException("Invalid SvrEid format, which should be 21 bytes, the first byte should be 1 and the last four bytes should be 0.", "svrEId"));
		}

		public StoreObjectId CreateMessageId(long parentFolderFid, long messageMid)
		{
			byte[] entryId = null;
			StoreSession storeSession = this.session;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				entryId = this.session.Mailbox.MapiStore.CreateEntryId(parentFolderFid, messageMid);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateEntryIdFromShortTermId, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to create a message entry id from short term ids. ParentFolderFid = {0}. MessageMid = {1}.", parentFolderFid, messageMid),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateEntryIdFromShortTermId, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to create a message entry id from short term ids. ParentFolderFid = {0}. MessageMid = {1}.", parentFolderFid, messageMid),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
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
			return StoreObjectId.FromProviderSpecificId(entryId);
		}

		public StoreObjectId CreateFolderId(long folderFid)
		{
			byte[] entryId = null;
			StoreSession storeSession = this.session;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				entryId = this.session.Mailbox.MapiStore.CreateEntryId(folderFid);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateEntryIdFromShortTermId, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to create a folder entry id from short term id. FolderFid = {0}.", folderFid),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateEntryIdFromShortTermId, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to create a folder entry id from short term id. FolderFid = {0}.", folderFid),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
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
			return StoreObjectId.FromProviderSpecificId(entryId);
		}

		public StoreObjectId CreatePublicMessageId(long parentFolderFid, long messageMid)
		{
			byte[] entryId = null;
			StoreSession storeSession = this.session;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				entryId = this.session.Mailbox.MapiStore.CreatePublicEntryId(parentFolderFid, messageMid);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateEntryIdFromShortTermId, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to create a public message entry id from short term ids. ParentFolderFid = {0}. MessageMid = {1}.", parentFolderFid, messageMid),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateEntryIdFromShortTermId, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to create a public message entry id from short term ids. ParentFolderFid = {0}. MessageMid = {1}.", parentFolderFid, messageMid),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
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
			return StoreObjectId.FromProviderSpecificId(entryId);
		}

		public StoreObjectId GetSessionSpecificId(StoreObjectId storeObjectId)
		{
			if (this.session is PublicFolderSession)
			{
				if (IdConverter.IsMessageId(storeObjectId))
				{
					return this.CreateMessageId(this.GetFidFromId(storeObjectId), this.GetMidFromMessageId(storeObjectId));
				}
				if (IdConverter.IsFolderId(storeObjectId))
				{
					return this.CreateFolderId(this.GetFidFromId(storeObjectId));
				}
			}
			return storeObjectId;
		}

		public StoreObjectId CreatePublicFolderId(long folderFid)
		{
			byte[] entryId = null;
			StoreSession storeSession = this.session;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				entryId = this.session.Mailbox.MapiStore.CreatePublicEntryId(folderFid);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateEntryIdFromShortTermId, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to create a public folder entry id from short term id. FolderFid = {0}.", folderFid),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateEntryIdFromShortTermId, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to create a public folder entry id from short term id. FolderFid = {0}.", folderFid),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
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
			return StoreObjectId.FromProviderSpecificId(entryId);
		}

		public long GetFidFromId(StoreObjectId storeObjectId)
		{
			Util.ThrowOnNullArgument(storeObjectId, "storeObjectId");
			StoreSession storeSession = this.session;
			bool flag = false;
			long fidFromEntryId;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				try
				{
					fidFromEntryId = this.session.Mailbox.MapiStore.GetFidFromEntryId(storeObjectId.ProviderLevelItemId);
				}
				catch (MapiExceptionInvalidEntryId innerException)
				{
					throw new CorruptDataException(ServerStrings.MapiInvalidId, innerException);
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiErrorParsingId, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to get the FID from the entry ID.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiErrorParsingId, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to get the FID from the entry ID.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
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
			return fidFromEntryId;
		}

		public long GetMidFromMessageId(StoreObjectId messageStoreObjectId)
		{
			Util.ThrowOnNullArgument(messageStoreObjectId, "messageStoreObjectId");
			if (!IdConverter.IsMessageId(messageStoreObjectId))
			{
				throw new ArgumentException("The argument messageStoreObjectId is not an id of a message.");
			}
			StoreSession storeSession = this.session;
			bool flag = false;
			long midFromMessageEntryId;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				try
				{
					midFromMessageEntryId = this.session.Mailbox.MapiStore.GetMidFromMessageEntryId(messageStoreObjectId.ProviderLevelItemId);
				}
				catch (MapiExceptionInvalidEntryId innerException)
				{
					throw new CorruptDataException(ServerStrings.MapiInvalidId, innerException);
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiErrorParsingId, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to get the MID from the entry ID.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiErrorParsingId, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to get the MID from the entry ID.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
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
			return midFromMessageEntryId;
		}

		public KeyValuePair<ushort, Guid> GetMdbIdMapping()
		{
			StoreSession storeSession = this.session;
			bool flag = false;
			KeyValuePair<ushort, Guid> result;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				ushort key;
				Guid value;
				this.session.Mailbox.MapiStore.GetMdbIdMapping(out key, out value);
				result = new KeyValuePair<ushort, Guid>(key, value);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiErrorParsingId, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to get Mdb Id mapping", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiErrorParsingId, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to get Mdb Id mapping", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
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
			return result;
		}

		public byte[] GetLongTermIdFromId(long id)
		{
			StoreSession storeSession = this.session;
			bool flag = false;
			byte[] result;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				result = this.session.Mailbox.MapiStore.GlobalIdFromId(id);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotGetLongTermIdFromId(id), ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("StoreSession::GetLongTermIdFromId failed.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotGetLongTermIdFromId(id), ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("StoreSession::GetLongTermIdFromId failed.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
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
			return result;
		}

		public long GetIdFromLongTermId(byte[] longTermId)
		{
			Util.ThrowOnNullArgument(longTermId, "The longTermId cannot be null.");
			StoreSession storeSession = this.session;
			bool flag = false;
			long result;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				try
				{
					result = this.session.Mailbox.MapiStore.IdFromGlobalId(longTermId);
				}
				catch (MapiExceptionInvalidEntryId innerException)
				{
					throw new CorruptDataException(ServerStrings.MapiInvalidId, innerException);
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotGetIdFromLongTermId(Convert.ToBase64String(longTermId)), ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("StoreSession::GetIdFromLongTermId failed.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.CannotGetIdFromLongTermId(Convert.ToBase64String(longTermId)), ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("StoreSession::GetIdFromLongTermId failed.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
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
			return result;
		}

		public string GetLegacyPopUid(StoreObjectId storeId)
		{
			Util.ThrowOnNullArgument(storeId, "storeId");
			byte[] longTermIdFromId = this.GetLongTermIdFromId(storeId);
			if (longTermIdFromId.Length == 22)
			{
				Array.Resize<byte>(ref longTermIdFromId, 24);
				longTermIdFromId[longTermIdFromId.Length - 1] = 0;
				longTermIdFromId[longTermIdFromId.Length - 2] = 0;
			}
			int num = 4 * longTermIdFromId.Length / 3;
			if (num % 4 != 0)
			{
				num += 4 - num % 4;
			}
			char[] array = new char[num];
			Convert.ToBase64CharArray(longTermIdFromId, 0, longTermIdFromId.Length, array, 0);
			Array.Reverse(array);
			return new string(array);
		}

		public byte[] GetLongTermIdFromId(StoreObjectId folderOrMessageId)
		{
			if (IdConverter.IsFolderId(folderOrMessageId))
			{
				return this.GetLongTermIdFromId(this.GetFidFromId(folderOrMessageId));
			}
			if (IdConverter.IsMessageId(folderOrMessageId))
			{
				return this.GetLongTermIdFromId(this.GetMidFromMessageId(folderOrMessageId));
			}
			throw new ArgumentException("Not a valid folder or message ID", "folderOrMessageId");
		}

		internal byte[][] GetSourceKeys(ICollection<StoreObjectId> objectIds, Predicate<StoreObjectId> isIdValid)
		{
			byte[][] array = new byte[objectIds.Count][];
			int num = 0;
			foreach (StoreObjectId storeObjectId in objectIds)
			{
				if (!isIdValid(storeObjectId))
				{
					throw new ArgumentException(ServerStrings.ExInvalidItemId);
				}
				array[num++] = this.GetLongTermIdFromId(storeObjectId);
			}
			return array;
		}

		private static bool MapiIsFolderId(byte[] entryId)
		{
			StoreSession storeSession = null;
			object thisObject = null;
			bool flag = false;
			bool result;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				try
				{
					result = MapiStore.IsFolderEntryId(entryId);
				}
				catch (MapiExceptionInvalidEntryId)
				{
					result = false;
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiErrorParsingId, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to determine if the id is a folder id.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiErrorParsingId, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to determine if the id is a folder id.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
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
			return result;
		}

		private static bool MapiIsMessageId(byte[] entryId)
		{
			StoreSession storeSession = null;
			object thisObject = null;
			bool flag = false;
			bool result;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				try
				{
					result = MapiStore.IsMessageEntryId(entryId);
				}
				catch (MapiExceptionInvalidEntryId)
				{
					result = false;
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiErrorParsingId, ex, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to determine if the id is a message id.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiErrorParsingId, ex2, storeSession, thisObject, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to determine if the id is a message id.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
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
			return result;
		}

		private static byte[] GlobcntIntoByteArray(ulong globCnt)
		{
			return new byte[]
			{
				(byte)(globCnt >> 40),
				(byte)(globCnt >> 32),
				(byte)(globCnt >> 24),
				(byte)(globCnt >> 16),
				(byte)(globCnt >> 8),
				(byte)globCnt
			};
		}

		private static bool ParseOursServerEntryId(byte[] entryId, out long fid, out long mid, out int instanceNum)
		{
			if (entryId == null || entryId.Length != 21 || entryId[0] != 1)
			{
				fid = 0L;
				mid = 0L;
				instanceNum = 0;
				return false;
			}
			fid = ParseSerialize.ParseInt64(entryId, 1);
			mid = ParseSerialize.ParseInt64(entryId, 9);
			instanceNum = ParseSerialize.ParseInt32(entryId, 17);
			return true;
		}

		private static GuidGlobCount ExtractGuidGlobCountFromEntryId(byte[] entryId, int begIndex)
		{
			if (entryId.Length - 24 < begIndex)
			{
				string message = string.Format("Invalid message id size. need {0} bytes but had {1}", begIndex + 24, entryId.Length);
				throw new CorruptDataException(ServerStrings.MapiInvalidId, new ArgumentException(message, "entryId"));
			}
			byte[] array = new byte[16];
			Buffer.BlockCopy(entryId, begIndex, array, 0, array.Length);
			byte[] array2 = new byte[8];
			Buffer.BlockCopy(entryId, begIndex + array.Length, array2, 0, array2.Length);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(array2, 0, 6);
			}
			return new GuidGlobCount(new Guid(array), BitConverter.ToUInt64(array2, 0));
		}

		private const int MessageEntryIdSize = 70;

		private const int FolderEntryIdSize = 46;

		private readonly StoreSession session;
	}
}
