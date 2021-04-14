using System;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal static class ServerIdConverter
	{
		internal static byte[] MakeOurServerId(long folderFid, long messageMid, int instance)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(21))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(1);
					binaryWriter.Write(folderFid);
					binaryWriter.Write(messageMid);
					binaryWriter.Write(instance);
					long position = memoryStream.Position;
					memoryStream.SetLength(position);
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		internal static byte[] MakeOurServerIdFromExportServerId(StoreSession session, byte[] serverId)
		{
			long folderFid = 0L;
			long messageMid = 0L;
			if (serverId != null && serverId.Length > 0)
			{
				using (BufferReader bufferReader = new BufferReader(new ArraySegment<byte>(serverId)))
				{
					if (bufferReader.ReadByte() != 2)
					{
						throw new ArgumentException("Not an export serverId.", "serverId");
					}
					if (bufferReader.Length - bufferReader.Position < (long)StoreLongTermId.ArraySize)
					{
						throw new RopExecutionException("Invalid server id. Invalid folder id value.", (ErrorCode)2147746561U);
					}
					StoreLongTermId storeLongTermId = StoreLongTermId.Parse(bufferReader, false);
					folderFid = session.IdConverter.GetIdFromLongTermId(storeLongTermId.ToBytes());
					if (bufferReader.Position != bufferReader.Length)
					{
						if (bufferReader.Length - bufferReader.Position < (long)StoreLongTermId.ArraySize)
						{
							throw new RopExecutionException("Invalid server id. Invalid message id value.", (ErrorCode)2147746561U);
						}
						StoreLongTermId storeLongTermId2 = StoreLongTermId.Parse(bufferReader, false);
						messageMid = session.IdConverter.GetIdFromLongTermId(storeLongTermId2.ToBytes());
					}
				}
				return ServerIdConverter.MakeOurServerId(folderFid, messageMid, 0);
			}
			throw new ArgumentException("ServerId value cannot be null or empty.", "serverId");
		}

		internal static byte[] MakeExportServerIdFromOurServerId(StoreSession session, byte[] serverId)
		{
			long id;
			long num;
			int num2;
			ServerIdConverter.CrackOurServerId(serverId, out id, out num, out num2);
			int num3 = 1 + StoreLongTermId.ArraySize + ((num != 0L) ? StoreLongTermId.ArraySize : 0);
			byte[] array = new byte[num3];
			using (BufferWriter bufferWriter = new BufferWriter(array))
			{
				bufferWriter.WriteByte(2);
				bufferWriter.WriteBytes(session.IdConverter.GetLongTermIdFromId(id));
				if (num != 0L)
				{
					bufferWriter.WriteBytes(session.IdConverter.GetLongTermIdFromId(num));
				}
			}
			return array;
		}

		internal static void CrackInstanceKey(byte[] instanceKey, out long folderFid, out long messageMid, out int instance)
		{
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(instanceKey))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						folderFid = binaryReader.ReadInt64();
						messageMid = binaryReader.ReadInt64();
						instance = binaryReader.ReadInt32();
					}
				}
			}
			catch (EndOfStreamException)
			{
				throw new RopExecutionException("Invalid InstanceKey", (ErrorCode)2147746055U);
			}
		}

		internal static byte[] MakeEntryIdFromServerId(StoreSession session, byte[] serverId)
		{
			if (serverId == null || serverId.Length == 0)
			{
				return Array<byte>.Empty;
			}
			ServerIdType serverIdType = ServerIdConverter.GetServerIdType(new ArraySegment<byte>(serverId));
			switch (serverIdType)
			{
			case ServerIdType.NotOurs:
				break;
			case ServerIdType.Ours:
				try
				{
					long num = 0L;
					long num2 = 0L;
					int num3 = 0;
					IdConverter idConverter = session.IdConverter;
					ServerIdConverter.CrackOurServerId(serverId, out num, out num2, out num3);
					StoreObjectId storeObjectId;
					if (num2 != 0L)
					{
						storeObjectId = idConverter.CreateMessageId(num, num2);
					}
					else
					{
						storeObjectId = idConverter.CreateFolderId(num);
					}
					return storeObjectId.ProviderLevelItemId;
				}
				catch (ObjectNotFoundException innerException)
				{
					throw new RopExecutionException("Invalid server id.", (ErrorCode)2147942487U, innerException);
				}
				catch (CorruptDataException innerException2)
				{
					throw new RopExecutionException("Invalid server id.", (ErrorCode)2147942487U, innerException2);
				}
				break;
			case ServerIdType.Export:
				try
				{
					return ServerIdConverter.MakeEntryIdFromServerId(session, ServerIdConverter.MakeOurServerIdFromExportServerId(session, serverId));
				}
				catch (CorruptDataException innerException3)
				{
					throw new RopExecutionException("Invalid server id.", (ErrorCode)2147942487U, innerException3);
				}
				goto IL_D3;
			default:
				goto IL_D3;
			}
			byte[] result = null;
			ServerIdConverter.CrackForeignServerId(serverId, out result);
			return result;
			IL_D3:
			throw new RopExecutionException(string.Format("Invalid ServerIdType {0}", serverIdType), (ErrorCode)2147942487U);
		}

		internal static byte[] MakeEntryIdFromFid(StoreSession session, long folderFid)
		{
			IdConverter idConverter = session.IdConverter;
			byte[] providerLevelItemId;
			try
			{
				StoreObjectId storeObjectId = idConverter.CreateFolderId(folderFid);
				providerLevelItemId = storeObjectId.ProviderLevelItemId;
			}
			catch (CorruptDataException)
			{
				throw new RopExecutionException(string.Format("Unable to resolve folder id. Fid = {0}.", folderFid), (ErrorCode)2147942487U);
			}
			return providerLevelItemId;
		}

		internal static byte[] MakeServerIdFromEntryId(StoreSession session, byte[] entryId)
		{
			if (entryId == null || entryId.Length == 0)
			{
				return ServerIdConverter.MakeOurServerId(0L, 0L, 0);
			}
			byte[] result;
			try
			{
				StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(entryId);
				long messageMid = 0L;
				IdConverter idConverter = session.IdConverter;
				long fidFromId;
				if (IdConverter.IsMessageId(storeObjectId))
				{
					fidFromId = idConverter.GetFidFromId(storeObjectId);
					messageMid = idConverter.GetMidFromMessageId(storeObjectId);
				}
				else
				{
					if (!IdConverter.IsFolderId(storeObjectId))
					{
						return ServerIdConverter.MakeForeignServerId(entryId);
					}
					fidFromId = idConverter.GetFidFromId(storeObjectId);
				}
				result = ServerIdConverter.MakeOurServerId(fidFromId, messageMid, 0);
			}
			catch (CorruptDataException)
			{
				result = ServerIdConverter.MakeForeignServerId(entryId);
			}
			return result;
		}

		internal static byte[] MakeServerIdFromInstanceKey(byte[] instanceKey)
		{
			if (instanceKey == null || instanceKey.Length == 0)
			{
				return ServerIdConverter.MakeOurServerId(0L, 0L, 0);
			}
			long folderFid = 0L;
			long messageMid = 0L;
			int instance = 0;
			ServerIdConverter.CrackInstanceKey(instanceKey, out folderFid, out messageMid, out instance);
			return ServerIdConverter.MakeOurServerId(folderFid, messageMid, instance);
		}

		internal static byte[] MakeInstanceKey(long folderFid, long messageMid, int instance)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(20))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(folderFid);
					binaryWriter.Write(messageMid);
					binaryWriter.Write(instance);
					long position = memoryStream.Position;
					memoryStream.SetLength(position);
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		internal static byte[] MakeInstanceKeyFromServerId(byte[] serverId)
		{
			if (serverId == null || serverId.Length == 0)
			{
				return new byte[0];
			}
			long folderFid = 0L;
			long messageMid = 0L;
			int instance = 0;
			ServerIdConverter.CrackOurServerId(serverId, out folderFid, out messageMid, out instance);
			return ServerIdConverter.MakeInstanceKey(folderFid, messageMid, instance);
		}

		internal static byte[] MakeForeignServerId(byte[] entryId)
		{
			byte[] array = new byte[entryId.Length + 1];
			Array.Copy(entryId, 0, array, 1, entryId.Length);
			array[0] = 0;
			return array;
		}

		private static void CrackForeignServerId(byte[] serverId, out byte[] entryId)
		{
			if (serverId[0] == 1)
			{
				throw new ArgumentException("Not a foreign serverId", "serverId");
			}
			entryId = new byte[serverId.Length - 1];
			Array.Copy(serverId, 1, entryId, 0, serverId.Length - 1);
		}

		internal static void CrackOurServerId(byte[] serverId, out long folderFid, out long messageMid, out int instance)
		{
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(serverId))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						if (binaryReader.ReadByte() != 1)
						{
							throw new ArgumentException("Not our serverId", "serverId");
						}
						folderFid = binaryReader.ReadInt64();
						messageMid = binaryReader.ReadInt64();
						instance = binaryReader.ReadInt32();
					}
				}
			}
			catch (EndOfStreamException)
			{
				throw new RopExecutionException(string.Format("Invalid ServerId {0}.", new ArrayTracer<byte>(serverId)), (ErrorCode)2147942487U);
			}
		}

		internal static ServerIdType GetServerIdType(ArraySegment<byte> serverId)
		{
			if (serverId.Array != null && serverId.Array.Length > 0)
			{
				return (ServerIdType)serverId.Array[serverId.Offset];
			}
			throw new ArgumentException("ServerId value cannot be null or empty.", "serverId");
		}
	}
}
