using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.DirectoryCache;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
	internal class DirectoryCacheService : IDirectoryCacheService
	{
		public IAsyncResult BeginGetObject(DirectoryCacheRequest cacheRequest, AsyncCallback callback, object asyncState)
		{
			this.stopwatch = Stopwatch.StartNew();
			return this.Invoke(delegate
			{
				ArgumentValidator.ThrowIfNull("cacheRequest", cacheRequest);
				if (cacheRequest.ObjectType == ObjectType.ADRawEntry && cacheRequest.Properties == null)
				{
					throw new ArgumentException("properties cannot be null when request object type is ADRawEntry");
				}
				MSExchangeDirectoryCacheServiceCounters.NumberOfCacheReadRequests.Increment();
				GetObjectContext getObjectContext = new GetObjectContext();
				getObjectContext.ResultState = ADCacheResultState.NotFound;
				LazyAsyncResult lazyAsyncResult = new LazyAsyncResult(getObjectContext, asyncState, callback);
				ExTraceGlobals.WCFServiceEndpointTracer.TraceDebug<DirectoryCacheRequest>((long)this.GetHashCode(), "Begin GetObject. Request {0}", cacheRequest);
				MSExchangeDirectoryCacheServiceCounters.CacheHitRatioBase.Increment();
				ObjectTuple objectTuple = null;
				foreach (Tuple<string, KeyType> tuple in cacheRequest.Keys)
				{
					CacheEntry cacheEntry = null;
					foreach (KeyType keyType in DirectoryCacheService.AllSupportedKeyTypes)
					{
						if ((keyType & tuple.Item2) != KeyType.None)
						{
							string composedKey = this.GetComposedKey(cacheRequest.ForestOrPartitionFqdn, tuple.Item1, keyType);
							string text = (string)CacheManager.Instance.KeyTable.Get(composedKey, null);
							if (text != null)
							{
								cacheEntry = (CacheEntry)CacheManager.Instance.ADObjectCache.Get(text, null);
								if (cacheEntry != null && cacheEntry[keyType].Contains(composedKey.ToLower()))
								{
									break;
								}
								if (cacheEntry != null)
								{
									if (ExTraceGlobals.WCFServiceEndpointTracer.IsTraceEnabled(TraceType.DebugTrace))
									{
										string keyValues = string.Empty;
										cacheEntry[keyType].ForEach(delegate(string keyVal)
										{
											keyValues += keyVal;
											keyValues += ",";
										});
										ExTraceGlobals.WCFServiceEndpointTracer.TraceDebug<string, string>((long)this.GetHashCode(), "the key {0} does not match the cacheEntry's one {1}", composedKey, keyValues);
									}
									cacheEntry = null;
								}
							}
						}
					}
					if (cacheEntry != null)
					{
						if (cacheEntry.Invalid)
						{
							break;
						}
						foreach (ObjectTuple objectTuple2 in cacheEntry.SimpleADObjectList)
						{
							if (objectTuple2.ObjType == cacheRequest.ObjectType)
							{
								objectTuple = objectTuple2;
								break;
							}
						}
						if (objectTuple == null)
						{
							break;
						}
						if (cacheRequest.Properties != null)
						{
							foreach (string key in cacheRequest.Properties)
							{
								if (!objectTuple.ADObject.Properties.ContainsKey(key))
								{
									objectTuple = null;
									getObjectContext.ResultState = ADCacheResultState.PropertiesMissing;
									ADProviderPerf.UpdateDirectoryADRawCachePropertiesMismatchRate(true);
									break;
								}
							}
						}
						if (objectTuple == null)
						{
							break;
						}
						getObjectContext.Object = objectTuple.ADObject;
						getObjectContext.ResultState = ADCacheResultState.Succeed;
						if (cacheRequest.Properties != null)
						{
							ADProviderPerf.UpdateDirectoryADRawCachePropertiesMismatchRate(false);
							break;
						}
						break;
					}
				}
				ExTraceGlobals.WCFServiceEndpointTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Begin GetObject. Request {0}. Was Cache {1}", cacheRequest.RequestId, (getObjectContext.Object != null) ? "HIT" : "MISS");
				ADProviderPerf.UpdateDirectoryCacheHitRatio(null != getObjectContext.Object, cacheRequest.ObjectType);
				this.stopwatch.Stop();
				getObjectContext.BeginOperationLatency = this.stopwatch.ElapsedMilliseconds;
				this.stopwatch.Restart();
				callback(lazyAsyncResult);
				return lazyAsyncResult;
			});
		}

		public GetObjectContext EndGetObject(IAsyncResult result)
		{
			ArgumentValidator.ThrowIfNull("result", result);
			ArgumentValidator.ThrowIfTypeInvalid<LazyAsyncResult>("result", result);
			LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)result;
			if (lazyAsyncResult.EndCalled)
			{
				throw new InvalidOperationException("End was already called. Invalid Begin/End");
			}
			lazyAsyncResult.EndCalled = true;
			GetObjectContext getObjectContext = (GetObjectContext)lazyAsyncResult.AsyncObject;
			this.stopwatch.Stop();
			getObjectContext.EndOperationLatency = this.stopwatch.ElapsedMilliseconds;
			return getObjectContext;
		}

		public IAsyncResult BeginPutObject(AddDirectoryCacheRequest cacheRequest, AsyncCallback callback, object asyncState)
		{
			this.stopwatch = Stopwatch.StartNew();
			return this.Invoke(delegate
			{
				ArgumentValidator.ThrowIfNull("cacheRequest", cacheRequest);
				CacheResponseContext cacheResponseContext = new CacheResponseContext();
				LazyAsyncResult lazyAsyncResult = new LazyAsyncResult(cacheResponseContext, asyncState, callback);
				ExTraceGlobals.WCFServiceEndpointTracer.TraceDebug<AddDirectoryCacheRequest>((long)this.GetHashCode(), "Begin Put. Request {0}", cacheRequest);
				MSExchangeDirectoryCacheServiceCounters.NumberOfCacheInsertionRequests.Increment();
				Tuple<string, KeyType> tuple = cacheRequest.Keys.Find((Tuple<string, KeyType> x) => KeyType.None != (x.Item2 & KeyType.Guid));
				if (tuple == null || CacheManager.Instance.ADObjectCache.Get(tuple.Item1.ToLower(), null) == null)
				{
					Tuple<string, KeyType> tuple2 = cacheRequest.Keys.Find((Tuple<string, KeyType> x) => KeyType.None != (x.Item2 & KeyType.DistinguishedName));
					if (tuple == null)
					{
						tuple = tuple2;
					}
					else if (tuple2 != null && CacheManager.Instance.ADObjectCache.Get(tuple2.Item1.ToLower(), null) != null)
					{
						tuple = tuple2;
					}
				}
				if (tuple == null)
				{
					return lazyAsyncResult;
				}
				string text = tuple.Item1.ToLower();
				cacheRequest.Object.Initialize(true);
				CacheItemPolicy cacheItemPolicy = this.GetCacheItemPolicy(cacheRequest);
				CacheEntry cacheEntry = (CacheEntry)CacheManager.Instance.ADObjectCache.Get(text, null);
				if (cacheEntry == null || cacheEntry.Invalid)
				{
					cacheEntry = new CacheEntry(new List<ObjectTuple>(1));
					cacheEntry.SimpleADObjectList.Add(new ObjectTuple(cacheRequest.ObjectType, cacheRequest.Object));
					CacheManager.Instance.ADObjectCache.Set(text, cacheEntry, cacheItemPolicy, null);
				}
				else
				{
					ObjectTuple objectTuple = null;
					foreach (ObjectTuple objectTuple2 in cacheEntry.SimpleADObjectList)
					{
						if (objectTuple2.ObjType == cacheRequest.ObjectType)
						{
							objectTuple = objectTuple2;
							objectTuple2.ADObject = cacheRequest.Object;
							break;
						}
					}
					if (objectTuple == null)
					{
						cacheEntry.SimpleADObjectList = new List<ObjectTuple>(cacheEntry.SimpleADObjectList)
						{
							new ObjectTuple(cacheRequest.ObjectType, cacheRequest.Object)
						};
					}
				}
				foreach (Tuple<string, KeyType> tuple3 in cacheRequest.Keys)
				{
					foreach (KeyType keyType in DirectoryCacheService.AllSupportedKeyTypes)
					{
						if ((keyType & tuple3.Item2) != KeyType.None)
						{
							cacheEntry.ClearKey(keyType);
						}
					}
				}
				foreach (Tuple<string, KeyType> tuple4 in cacheRequest.Keys)
				{
					foreach (KeyType keyType2 in DirectoryCacheService.AllSupportedKeyTypes)
					{
						if ((keyType2 & tuple4.Item2) != KeyType.None)
						{
							string composedKey = this.GetComposedKey(cacheRequest.ForestOrPartitionFqdn, tuple4.Item1, keyType2);
							cacheEntry[keyType2].Add(composedKey.ToLower());
							CacheManager.Instance.KeyTable.Set(composedKey, text, cacheItemPolicy, null);
						}
					}
				}
				this.stopwatch.Stop();
				cacheResponseContext.BeginOperationLatency = this.stopwatch.ElapsedMilliseconds;
				this.stopwatch.Restart();
				callback(lazyAsyncResult);
				return lazyAsyncResult;
			});
		}

		public CacheResponseContext EndPutObject(IAsyncResult result)
		{
			ArgumentValidator.ThrowIfNull("result", result);
			ArgumentValidator.ThrowIfTypeInvalid<LazyAsyncResult>("result", result);
			LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)result;
			if (lazyAsyncResult.EndCalled)
			{
				throw new InvalidOperationException("End was already called. Invalid Begin/End");
			}
			lazyAsyncResult.EndCalled = true;
			this.stopwatch.Stop();
			CacheResponseContext cacheResponseContext = (CacheResponseContext)lazyAsyncResult.AsyncObject;
			cacheResponseContext.EndOperationLatency = this.stopwatch.ElapsedMilliseconds;
			return cacheResponseContext;
		}

		public IAsyncResult BeginRemoveObject(RemoveDirectoryCacheRequest cacheRequest, AsyncCallback callback, object asyncState)
		{
			this.stopwatch = Stopwatch.StartNew();
			return this.Invoke(delegate
			{
				ArgumentValidator.ThrowIfNull("cacheRequest", cacheRequest);
				CacheResponseContext cacheResponseContext = new CacheResponseContext();
				LazyAsyncResult lazyAsyncResult = new LazyAsyncResult(cacheResponseContext, asyncState, callback);
				MSExchangeDirectoryCacheServiceCounters.NumberOfCacheRemovalRequests.Increment();
				this.InternalRemoveObject(cacheRequest.ForestOrPartitionFqdn, cacheRequest.Key);
				this.stopwatch.Stop();
				cacheResponseContext.BeginOperationLatency = this.stopwatch.ElapsedMilliseconds;
				this.stopwatch.Restart();
				callback(lazyAsyncResult);
				return lazyAsyncResult;
			});
		}

		public CacheResponseContext EndRemoveObject(IAsyncResult result)
		{
			ArgumentValidator.ThrowIfNull("result", result);
			ArgumentValidator.ThrowIfTypeInvalid<LazyAsyncResult>("result", result);
			LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)result;
			if (lazyAsyncResult.EndCalled)
			{
				throw new InvalidOperationException("End was already called. Invalid Begin/End");
			}
			CacheResponseContext cacheResponseContext = (CacheResponseContext)lazyAsyncResult.AsyncObject;
			this.stopwatch.Stop();
			cacheResponseContext.EndOperationLatency = this.stopwatch.ElapsedMilliseconds;
			lazyAsyncResult.EndCalled = true;
			return cacheResponseContext;
		}

		public IAsyncResult BeginDiagnostic(DiagnosticsCacheRequest cacheRequest, AsyncCallback callback, object asyncState)
		{
			ArgumentValidator.ThrowIfNull("cacheRequest", cacheRequest);
			LazyAsyncResult lazyAsyncResult = new LazyAsyncResult(null, asyncState, callback);
			CacheManager.Instance.ResetAllCaches();
			callback(lazyAsyncResult);
			return lazyAsyncResult;
		}

		public void EndDiagnostic(IAsyncResult result)
		{
			ArgumentValidator.ThrowIfNull("result", result);
			ArgumentValidator.ThrowIfTypeInvalid<LazyAsyncResult>("result", result);
			LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)result;
			if (lazyAsyncResult.EndCalled)
			{
				throw new InvalidOperationException("End was already called. Invalid Begin/End");
			}
			lazyAsyncResult.EndCalled = true;
		}

		private void InternalRemoveObject(string forestOrPartitionFqdn, Tuple<string, KeyType> key)
		{
			ExTraceGlobals.WCFServiceEndpointTracer.TraceDebug<string, Tuple<string, KeyType>>((long)this.GetHashCode(), "Remove object. ForestOrPartitionFqdn '{0}'. key '{1}'.", forestOrPartitionFqdn, key);
			foreach (KeyType keyType in DirectoryCacheService.AllSupportedKeyTypes)
			{
				if ((keyType & key.Item2) != KeyType.None)
				{
					string text = (string)CacheManager.Instance.KeyTable.Remove(this.GetComposedKey(forestOrPartitionFqdn, key.Item1, keyType), null);
					if (text != null)
					{
						CacheEntry cacheEntry = (CacheEntry)CacheManager.Instance.ADObjectCache.Remove(text, null);
						if (cacheEntry != null)
						{
							cacheEntry.Invalid = true;
							cacheEntry.ClearKeys();
							return;
						}
						break;
					}
				}
			}
		}

		private CacheItemPolicy GetCacheItemPolicy(AddDirectoryCacheRequest cacheRequest)
		{
			return new CacheItemPolicy
			{
				AbsoluteExpiration = this.GetCacheItemDateTimeOffsetExpiration(cacheRequest.ObjectType, cacheRequest.SecondsTimeout),
				Priority = cacheRequest.Priority
			};
		}

		private DateTimeOffset GetCacheItemDateTimeOffsetExpiration(ObjectType objectType, int secondsTimeout)
		{
			ArgumentValidator.ThrowIfOutOfRange<int>("secondsTimeout", secondsTimeout, 1, 2147483646);
			if (2147483646 == secondsTimeout)
			{
				if (objectType <= ObjectType.OWAMiniRecipient)
				{
					if (objectType <= ObjectType.FederatedOrganizationId)
					{
						switch (objectType)
						{
						case ObjectType.ExchangeConfigurationUnit:
						case ObjectType.AcceptedDomain:
							break;
						case ObjectType.Recipient:
							goto IL_93;
						case ObjectType.ExchangeConfigurationUnit | ObjectType.Recipient:
							goto IL_A5;
						default:
							if (objectType != ObjectType.FederatedOrganizationId)
							{
								goto IL_A5;
							}
							break;
						}
						secondsTimeout = Globals.TenantInfoCacheTime;
						goto IL_AB;
					}
					if (objectType != ObjectType.MiniRecipient && objectType != ObjectType.TransportMiniRecipient && objectType != ObjectType.OWAMiniRecipient)
					{
						goto IL_A5;
					}
				}
				else if (objectType <= ObjectType.ADRawEntry)
				{
					if (objectType != ObjectType.ActiveSyncMiniRecipient && objectType != ObjectType.ADRawEntry)
					{
						goto IL_A5;
					}
				}
				else if (objectType != ObjectType.StorageMiniRecipient && objectType != ObjectType.LoadBalancingMiniRecipient)
				{
					if (objectType != ObjectType.MiniRecipientWithTokenGroups)
					{
						goto IL_A5;
					}
					secondsTimeout = Globals.RecipientTokenGroupsCacheTime;
					goto IL_AB;
				}
				IL_93:
				secondsTimeout = Globals.RecipientInfoCacheTime;
				goto IL_AB;
				IL_A5:
				throw new NotImplementedException();
			}
			IL_AB:
			return DateTimeOffset.Now.AddSeconds((double)secondsTimeout);
		}

		private IAsyncResult Invoke(Func<IAsyncResult> action)
		{
			IAsyncResult result;
			try
			{
				result = action();
			}
			catch (Exception ex)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_ADCacheServiceUnexpectedException, ex.GetType().Name, new object[]
				{
					ex.ToString()
				});
				throw;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private string GetComposedKey(string forestOrPartitionFqdn, string key, KeyType keyType)
		{
			if (ExEnvironment.IsTest)
			{
				int num = forestOrPartitionFqdn.IndexOf(".extest.microsoft.com", StringComparison.OrdinalIgnoreCase);
				int num2 = forestOrPartitionFqdn.IndexOf('.');
				if (num2 != num)
				{
					forestOrPartitionFqdn = forestOrPartitionFqdn.Substring(num2 + 1);
				}
			}
			return (forestOrPartitionFqdn + keyType + key).ToLower();
		}

		private static readonly KeyType[] AllSupportedKeyTypes = (KeyType[])Enum.GetValues(typeof(KeyType));

		private Stopwatch stopwatch;
	}
}
