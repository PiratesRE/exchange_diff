using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class IdSetUtilities
	{
		internal static IdSet IdSetFromBytes(Context context, byte[] idsetBytes)
		{
			IdSet result;
			try
			{
				result = IdSetUtilities.ThrowableIdSetFromBytes(context, idsetBytes);
			}
			catch (StoreException exception)
			{
				context.OnExceptionCatch(exception);
				result = new IdSet();
			}
			return result;
		}

		internal static IdSet ThrowableIdSetFromBytes(Context context, byte[] idsetBytes)
		{
			IdSet result;
			using (Reader reader = Reader.CreateBufferReader(idsetBytes))
			{
				result = IdSetUtilities.ThrowableIdSetFromReader(context, reader);
			}
			return result;
		}

		internal static IdSet ThrowableIdSetFromStream(Context context, Stream readStream)
		{
			readStream.Position = 0L;
			IdSet result;
			using (Reader reader = Reader.CreateStreamReader(readStream))
			{
				result = IdSetUtilities.ThrowableIdSetFromReader(context, reader);
			}
			return result;
		}

		internal static IdSet ThrowableIdSetFromReader(Context context, Reader rcaReader)
		{
			IdSet result;
			try
			{
				IdSet idSet = IdSet.ParseWithReplGuids(rcaReader);
				if (rcaReader.Length != rcaReader.Position)
				{
					throw new StoreException((LID)30272U, ErrorCodeValue.CorruptData, "Serialized IDSet is corrupted");
				}
				result = idSet;
			}
			catch (BufferParseException ex)
			{
				context.OnExceptionCatch(ex);
				throw new StoreException((LID)30100U, ErrorCodeValue.CorruptData, "Serialized IDSet is corrupted", ex);
			}
			return result;
		}

		internal static byte[] BytesFromIdSet(IdSet idSet)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(100))
			{
				using (Writer writer = new StreamWriter(memoryStream))
				{
					idSet.SerializeWithReplGuids(writer);
				}
				result = memoryStream.ToArray();
			}
			return result;
		}
	}
}
