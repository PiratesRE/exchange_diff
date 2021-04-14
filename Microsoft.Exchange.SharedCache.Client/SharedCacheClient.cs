using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.SharedCache;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.SharedCache;

namespace Microsoft.Exchange.SharedCache.Client
{
	public class SharedCacheClient
	{
		public SharedCacheClient(Guid cacheGuid, string clientName, int timeoutMilliseconds) : this(cacheGuid, clientName, timeoutMilliseconds, false, null)
		{
		}

		public SharedCacheClient(Guid cacheGuid, string clientName, int timeoutMilliseconds, bool throwRpcExceptions) : this(cacheGuid, clientName, timeoutMilliseconds, throwRpcExceptions, null)
		{
		}

		public SharedCacheClient(Guid cacheGuid, string clientName, int timeoutMilliseconds, IConcurrencyGuard concurrencyGuard) : this(cacheGuid, clientName, timeoutMilliseconds, false, concurrencyGuard)
		{
		}

		public SharedCacheClient(Guid cacheGuid, string clientName, int timeoutMilliseconds, bool throwRpcExceptions, IConcurrencyGuard concurrencyGuard) : this(cacheGuid, clientName, throwRpcExceptions, concurrencyGuard, new SharedCacheRpcClientImpl("localhost", timeoutMilliseconds))
		{
		}

		internal SharedCacheClient(Guid cacheGuid, string clientName, bool throwRpcExceptions, IConcurrencyGuard concurrencyGuard, ISharedCacheRpcClient rpcClient)
		{
			this.cacheGuid = cacheGuid;
			this.clientName = clientName;
			this.rpcCacheClient = rpcClient;
			this.throwRpcExceptions = throwRpcExceptions;
			this.guard = concurrencyGuard;
		}

		public bool ThrowRpcExceptions
		{
			get
			{
				return this.throwRpcExceptions;
			}
		}

		public bool TryGet<T>(string key, out T value) where T : ISharedCacheEntry, new()
		{
			return this.TryGet<T>(key, Guid.NewGuid(), out value);
		}

		public bool TryGet<T>(string key, Guid requestCorrelationId, out T value) where T : ISharedCacheEntry, new()
		{
			string text;
			return this.TryGet<T>(key, requestCorrelationId, out value, out text);
		}

		public bool TryGet<T>(string key, out T value, out string diagnosticsInformation) where T : ISharedCacheEntry, new()
		{
			return this.TryGet<T>(key, Guid.NewGuid(), out value, out diagnosticsInformation);
		}

		public bool TryGet<T>(string key, Guid requestCorrelationId, out T value, out string diagnosticsInformation) where T : ISharedCacheEntry, new()
		{
			value = default(T);
			byte[] serializedBytes;
			bool flag = this.TryGet(key, requestCorrelationId, out serializedBytes, out diagnosticsInformation);
			if (flag)
			{
				value = SerializationHelper.Deserialize<T>(serializedBytes);
			}
			return flag;
		}

		public bool TryGet(string key, out byte[] value)
		{
			return this.TryGet(key, Guid.NewGuid(), out value);
		}

		public bool TryGet(string key, Guid requestCorrelationId, out byte[] value)
		{
			string text;
			return this.TryGet(key, requestCorrelationId, out value, out text);
		}

		public bool TryGet(string key, out byte[] value, out string diagnosticsInformation)
		{
			return this.TryGet(key, Guid.NewGuid(), out value, out diagnosticsInformation);
		}

		public bool TryGet(string key, Guid requestCorrelationId, out byte[] value, out string diagnosticsInformation)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("key", key);
			string transactionId = RpcHelper.CreateTransactionString(this.clientName, "Get", requestCorrelationId, key);
			SharedCacheClient.RpcAction actionToExecute = () => this.rpcCacheClient.Get(this.cacheGuid, key);
			CacheResponse response = this.Execute(transactionId, actionToExecute);
			RpcHelper.SetCommonOutParameters(response, out value, out diagnosticsInformation);
			return RpcHelper.ValidateValuedBasedResponse(response);
		}

		public bool TryInsert(string key, ISharedCacheEntry value, DateTime valueTimestamp)
		{
			return this.TryInsert(key, value, valueTimestamp, Guid.NewGuid());
		}

		public bool TryInsert(string key, ISharedCacheEntry value, DateTime valueTimestamp, Guid requestCorrelationId)
		{
			string text;
			return this.TryInsert(key, value, valueTimestamp, requestCorrelationId, out text);
		}

		public bool TryInsert(string key, ISharedCacheEntry value, DateTime valueTimestamp, out string diagnosticsInformation)
		{
			return this.TryInsert(key, value, valueTimestamp, Guid.NewGuid(), out diagnosticsInformation);
		}

		public bool TryInsert(string key, ISharedCacheEntry value, DateTime valueTimestamp, Guid requestCorrelationId, out string diagnosticsInformation)
		{
			ArgumentValidator.ThrowIfNull("value", value);
			byte[] value2 = SerializationHelper.Serialize(value);
			return this.TryInsert(key, value2, valueTimestamp, requestCorrelationId, out diagnosticsInformation);
		}

		public bool TryInsert(string key, byte[] value, DateTime valueTimestamp)
		{
			return this.TryInsert(key, value, valueTimestamp, Guid.NewGuid());
		}

		public bool TryInsert(string key, byte[] value, DateTime valueTimestamp, Guid requestCorrelationId)
		{
			string text;
			return this.TryInsert(key, value, valueTimestamp, requestCorrelationId, out text);
		}

		public bool TryInsert(string key, byte[] value, DateTime valueTimestamp, out string diagnosticsInformation)
		{
			return this.TryInsert(key, value, valueTimestamp, Guid.NewGuid(), out diagnosticsInformation);
		}

		public bool TryInsert(string key, byte[] value, DateTime valueTimestamp, Guid requestCorrelationId, out string diagnosticsInformation)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("key", key);
			ArgumentValidator.ThrowIfNull("value", value);
			string transactionId = RpcHelper.CreateTransactionString(this.clientName, "Insert", requestCorrelationId, key);
			SharedCacheClient.RpcAction actionToExecute = () => this.rpcCacheClient.Insert(this.cacheGuid, key, value, valueTimestamp.ToBinary());
			CacheResponse cacheResponse = this.Execute(transactionId, actionToExecute);
			byte[] array;
			RpcHelper.SetCommonOutParameters(cacheResponse, out array, out diagnosticsInformation);
			return cacheResponse.ResponseCode == ResponseCode.OK;
		}

		public bool TryRemove<T>(string key, Guid requestCorrelationId, out T value) where T : ISharedCacheEntry, new()
		{
			value = default(T);
			byte[] serializedBytes;
			bool flag = this.TryRemove(key, requestCorrelationId, out serializedBytes);
			if (flag)
			{
				value = SerializationHelper.Deserialize<T>(serializedBytes);
			}
			return flag;
		}

		public bool TryRemove(string key)
		{
			byte[] array;
			return this.TryRemove(key, out array);
		}

		public bool TryRemove(string key, out byte[] value)
		{
			return this.TryRemove(key, Guid.NewGuid(), out value);
		}

		public bool TryRemove(string key, out byte[] value, out string diagnosticsInformation)
		{
			return this.TryRemove(key, Guid.NewGuid(), out value, out diagnosticsInformation);
		}

		public bool TryRemove(string key, Guid requestCorrelationId)
		{
			byte[] array;
			string text;
			return this.TryRemove(key, requestCorrelationId, out array, out text);
		}

		public bool TryRemove(string key, Guid requestCorrelationId, out byte[] value)
		{
			string text;
			return this.TryRemove(key, requestCorrelationId, out value, out text);
		}

		public bool TryRemove(string key, Guid requestCorrelationId, out string diagnosticsInformation)
		{
			byte[] array;
			return this.TryRemove(key, requestCorrelationId, out array, out diagnosticsInformation);
		}

		public bool TryRemove(string key, Guid requestCorrelationId, out byte[] value, out string diagnosticsInformation)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("key", key);
			string transactionId = RpcHelper.CreateTransactionString(this.clientName, "Delete", requestCorrelationId, key);
			SharedCacheClient.RpcAction actionToExecute = () => this.rpcCacheClient.Delete(this.cacheGuid, key);
			CacheResponse response = this.Execute(transactionId, actionToExecute);
			RpcHelper.SetCommonOutParameters(response, out value, out diagnosticsInformation);
			return RpcHelper.ValidateValuedBasedResponse(response);
		}

		private CacheResponse Execute(string transactionId, SharedCacheClient.RpcAction actionToExecute)
		{
			CacheResponse cacheResponse;
			try
			{
				ExTraceGlobals.ClientTracer.TraceDebug((long)this.GetHashCode(), "[SharedCacheClient:Execute] >> " + transactionId);
				if (this.guard != null)
				{
					this.guard.Increment(null);
				}
				cacheResponse = actionToExecute();
				if (cacheResponse == null)
				{
					if (this.ThrowRpcExceptions)
					{
						throw new CacheClientException("Cache action completed but response is null.");
					}
					cacheResponse = CacheResponse.Create(ResponseCode.RpcError);
					cacheResponse.AppendDiagInfo("Error", "Null response");
				}
				else
				{
					ExTraceGlobals.ClientTracer.TraceDebug((long)this.GetHashCode(), "[SharedCacheClient:Execute] << " + transactionId + " " + cacheResponse.ToString());
					switch (cacheResponse.ResponseCode)
					{
					case ResponseCode.InternalServerError:
						if (this.ThrowRpcExceptions)
						{
							throw new CacheClientException("Internal Server Error: " + cacheResponse.ToString());
						}
						break;
					case ResponseCode.CacheGuidNotFound:
						throw new ArgumentException("Cache GUID " + this.cacheGuid + " was not found registered with the shared cache service.");
					}
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.ClientTracer.TraceError((long)this.GetHashCode(), "[SharedCacheClient:Execute] << " + transactionId + " " + ex.ToString());
				RpcException ex2 = ex as RpcException;
				MaxConcurrencyReachedException ex3 = ex as MaxConcurrencyReachedException;
				if (this.ThrowRpcExceptions)
				{
					throw new CacheClientException("Unhandled Exception", ex);
				}
				if (ex3 != null)
				{
					cacheResponse = CacheResponse.Create(ResponseCode.TooManyOutstandingRequests);
					cacheResponse.AppendDiagInfo("Too many outstanding requests", ex3.Message);
				}
				else if (ex2 != null && ex2.ErrorCode == 1753)
				{
					cacheResponse = CacheResponse.Create(ResponseCode.RpcError);
					cacheResponse.AppendDiagInfo("Service Unavailable", "(0x" + ex2.ErrorCode.ToString("x") + ")");
				}
				else
				{
					cacheResponse = CacheResponse.Create(ResponseCode.RpcError);
					cacheResponse.AppendDiagInfo("Exception", ex.ToString());
				}
			}
			finally
			{
				if (this.guard != null)
				{
					this.guard.Decrement(null);
				}
			}
			return cacheResponse;
		}

		private const int RpcErrorNoEndpointAvailable = 1753;

		private readonly Guid cacheGuid;

		private readonly string clientName;

		private readonly ISharedCacheRpcClient rpcCacheClient;

		private readonly bool throwRpcExceptions;

		private IConcurrencyGuard guard;

		private delegate CacheResponse RpcAction();
	}
}
