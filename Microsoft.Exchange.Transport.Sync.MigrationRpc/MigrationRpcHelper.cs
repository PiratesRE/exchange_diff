using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.MigrationService;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class MigrationRpcHelper
	{
		internal static void InvokeRpcOperation(Action rpcOperation)
		{
			try
			{
				rpcOperation();
			}
			catch (SyncMigrationRpcTransientException ex)
			{
				bool isConnectionError = MigrationServiceRpcServer.IsRpcConnectionError(ex.ErrorCode);
				throw new MigrationServiceRpcTransientException(ex.ErrorCode, isConnectionError, ex);
			}
			catch (RpcException innerException)
			{
				throw new MigrationServiceRpcException(MigrationServiceRpcResultCode.RpcException, string.Empty, innerException);
			}
		}

		internal static T ReadValue<T>(MdbefPropertyCollection inputArgs, uint key)
		{
			object obj;
			if (!inputArgs.TryGetValue(key, out obj))
			{
				throw new MigrationCommunicationException(MigrationServiceRpcResultCode.ArgumentMismatchError, string.Format("Value for key {0} not found", key));
			}
			if (!(obj is T))
			{
				throw new MigrationCommunicationException(MigrationServiceRpcResultCode.ArgumentMismatchError, string.Format("Value for key {0} expected to be of type {1} but was of type {2}", key, typeof(T), obj.GetType()));
			}
			return (T)((object)obj);
		}

		internal static T ReadValue<T>(MdbefPropertyCollection inputArgs, uint key, T defaultValue)
		{
			T result;
			if (MigrationRpcHelper.TryReadValue<T>(inputArgs, key, out result))
			{
				return result;
			}
			return defaultValue;
		}

		internal static bool TryReadValue<T>(MdbefPropertyCollection inputArgs, uint key, out T result)
		{
			object obj;
			if (!inputArgs.TryGetValue(key, out obj))
			{
				result = default(T);
				return false;
			}
			if (!(obj is T))
			{
				throw new MigrationCommunicationException(MigrationServiceRpcResultCode.ArgumentMismatchError, string.Format("Value for key {0} expected to be of type {1} but was of type {2}", key, typeof(T), obj.GetType()));
			}
			result = (T)((object)obj);
			return true;
		}

		internal static ADObjectId ReadADObjectId(MdbefPropertyCollection inputArgs, uint key)
		{
			byte[] bytes = MigrationRpcHelper.ReadValue<byte[]>(inputArgs, key);
			return new ADObjectId(bytes);
		}

		internal static TEnum ReadEnum<TEnum>(MdbefPropertyCollection inputArgs, uint key) where TEnum : struct
		{
			object obj;
			if (!inputArgs.TryGetValue(key, out obj))
			{
				throw new MigrationCommunicationException(MigrationServiceRpcResultCode.ArgumentMismatchError, string.Format("Value for key {0} not found", key));
			}
			if (obj == null)
			{
				throw new MigrationCommunicationException(MigrationServiceRpcResultCode.ArgumentMismatchError, string.Format("Value for key {0} cannot be null since this is a Enum Type", key));
			}
			Type underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
			Type type = obj.GetType();
			if (!type.Equals(underlyingType))
			{
				throw new MigrationCommunicationException(MigrationServiceRpcResultCode.ArgumentMismatchError, string.Format("For key {0}, the underlying type for the enum result is expected to be {1} but was {2}", key, underlyingType, type));
			}
			if (!Enum.IsDefined(typeof(TEnum), obj))
			{
				throw new MigrationCommunicationException(MigrationServiceRpcResultCode.ArgumentMismatchError, string.Format("For key {0}, Value {1} is not valid for enum {2}", key, obj, typeof(TEnum)));
			}
			return (TEnum)((object)Enum.ToObject(typeof(TEnum), obj));
		}

		internal static StoreObjectId TryDeserializeStoreObjectId(byte[] subscriptonMessageIdBytes)
		{
			StoreObjectId result;
			try
			{
				StoreObjectId storeObjectId = StoreObjectId.Deserialize(subscriptonMessageIdBytes);
				result = storeObjectId;
			}
			catch (CorruptDataException)
			{
				throw new MigrationCommunicationException(MigrationServiceRpcResultCode.ArgumentMismatchError, string.Format("Could not deserialize subscriptionMessageIdBytes into a valid message", new object[0]));
			}
			return result;
		}

		internal static byte[] SerializeStoreObjectId(StoreObjectId value)
		{
			if (value == null)
			{
				throw new MigrationCommunicationException(MigrationServiceRpcResultCode.ArgumentMismatchError, "Invalid StoreObjectId");
			}
			return value.GetBytes();
		}

		internal static MdbefPropertyCollection CreateResponsePropertyCollection(MigrationServiceRpcResultCode migrationServiceRpcResultCode)
		{
			MdbefPropertyCollection mdbefPropertyCollection = new MdbefPropertyCollection();
			mdbefPropertyCollection[2936012803U] = (int)migrationServiceRpcResultCode;
			return mdbefPropertyCollection;
		}
	}
}
