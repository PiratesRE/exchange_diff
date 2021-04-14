using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.FailFast.EventLog;
using Microsoft.Exchange.Diagnostics.Components.FailFast;

namespace Microsoft.Exchange.Configuration.FailFast
{
	internal class PrimaryFailFastUserCache : FailFastUserCache
	{
		private PrimaryFailFastUserCache() : base(FailFastUserCache.FailFastEnabled ? 20 : 1, FailFastUserCache.FailFastEnabled ? 5000 : 1)
		{
			if (FailFastUserCache.FailFastEnabled)
			{
				this.primaryObjectBehavior = new CrossAppDomainPrimaryObjectBehavior(FailFastUserCache.PipeNameOfThisProcess, BehaviorDirection.In, new CrossAppDomainPrimaryObjectBehavior.OnMessageReceived(this.OnBlockedUserInfoReceived));
			}
		}

		internal static PrimaryFailFastUserCache Singleton
		{
			get
			{
				if (PrimaryFailFastUserCache.singleton == null)
				{
					lock (PrimaryFailFastUserCache.syncRoot)
					{
						if (PrimaryFailFastUserCache.singleton == null)
						{
							Logger.TraceDebug(ExTraceGlobals.FailFastCacheTracer, "Create PrimaryFailFastUserCache instance. FailFastEnabled={0}", new object[]
							{
								FailFastUserCache.FailFastEnabled
							});
							PrimaryFailFastUserCache.singleton = new PrimaryFailFastUserCache();
						}
					}
				}
				return PrimaryFailFastUserCache.singleton;
			}
		}

		internal override bool IsUserInCache(string userToken, string userTenant, out string cacheKey, out FailFastUserCacheValue cacheValue, out BlockedReason blockedReason)
		{
			cacheKey = null;
			cacheValue = null;
			blockedReason = BlockedReason.None;
			if (!FailFastUserCache.FailFastEnabled)
			{
				return false;
			}
			Logger.EnterFunction(ExTraceGlobals.FailFastCacheTracer, "PrimaryFailFastUserCache.IsUserBlocked.");
			if (base.TryGetValue(userToken, out cacheValue) && cacheValue.IsValid)
			{
				blockedReason = BlockedReason.BySelf;
				cacheKey = userToken;
				Logger.TraceDebug(ExTraceGlobals.FailFastCacheTracer, "User key {0} exist. CachedValue: {1}", new object[]
				{
					userToken,
					cacheValue.ToString()
				});
				return true;
			}
			if (!string.IsNullOrEmpty(userTenant))
			{
				string text = "Tenant:C8E2A9F6-0E7A-4bcc-95A0-9CE1BCA7EE68:" + userTenant;
				if (base.TryGetValue(text, out cacheValue) && cacheValue.IsValid)
				{
					cacheKey = text;
					blockedReason = BlockedReason.ByTenant;
					Logger.TraceDebug(ExTraceGlobals.FailFastCacheTracer, "Tenant Key {0} exist. CachedValue: {1}", new object[]
					{
						text,
						cacheValue.ToString()
					});
					return true;
				}
			}
			if (base.TryGetValue("AllUsers:D3511BCA-379C-4a38-97E5-0FDA0C231C33", out cacheValue) && cacheValue.IsValid)
			{
				blockedReason = BlockedReason.ByServer;
				cacheKey = "AllUsers:D3511BCA-379C-4a38-97E5-0FDA0C231C33";
				Logger.TraceDebug(ExTraceGlobals.FailFastCacheTracer, "All User Key {0} exist. CacheValue: {1}", new object[]
				{
					"AllUsers:D3511BCA-379C-4a38-97E5-0FDA0C231C33",
					cacheValue.ToString()
				});
				return true;
			}
			Logger.ExitFunction(ExTraceGlobals.FailFastCacheTracer, "PrimaryFailFastUserCache.IsUserBlocked.");
			return false;
		}

		protected override void InsertValueToCache(string key, BlockedType blockedType, TimeSpan blockedTime)
		{
			if (!FailFastUserCache.FailFastEnabled)
			{
				return;
			}
			Logger.EnterFunction(ExTraceGlobals.FailFastCacheTracer, "PrimaryFailFastUserCache.InsertValueToCache");
			blockedTime = PrimaryFailFastUserCache.GetValidBlockedTime(blockedTime);
			Logger.TraceDebug(ExTraceGlobals.FailFastCacheTracer, "Try to insert value to the cache. Key: {0}. BlockedType: {1}. BlockedTime: {2}.", new object[]
			{
				key,
				blockedType,
				blockedTime
			});
			FailFastUserCacheValue failFastUserCacheValue;
			if (base.TryGetValue(key, out failFastUserCacheValue) && failFastUserCacheValue.IsValid && failFastUserCacheValue.BlockedType > blockedType)
			{
				Logger.TraceDebug(ExTraceGlobals.FailFastCacheTracer, "Adding blocked type {0} that has smaller scope than existing one {1}.", new object[]
				{
					blockedType,
					failFastUserCacheValue.BlockedType
				});
				return;
			}
			if (failFastUserCacheValue == null || !failFastUserCacheValue.IsValid)
			{
				failFastUserCacheValue = new FailFastUserCacheValue(blockedType, blockedTime);
			}
			else
			{
				failFastUserCacheValue.BlockedTime = blockedTime;
				failFastUserCacheValue.BlockedType = blockedType;
				failFastUserCacheValue.AddedTime = DateTime.UtcNow;
				failFastUserCacheValue.HitCount++;
			}
			Logger.LogEvent(TaskEventLogConstants.Tuple_LogUserAddedToFailFastUserCached, "UserAddedToFailFastCache:" + key, new object[]
			{
				key,
				failFastUserCacheValue.ToString()
			});
			Logger.TraceInformation(ExTraceGlobals.FailFastCacheTracer, "Insert value to the cache. Key: {0}. CacheValue: {1}.", new object[]
			{
				key,
				failFastUserCacheValue.ToString()
			});
			base.InsertAbsolute(key, failFastUserCacheValue, blockedTime, new RemoveItemDelegate<string, FailFastUserCacheValue>(this.OnCacheValueBeRemoved));
			FailFastModule.RemotePowershellPerfCounter.FailFastUserCacheSize.RawValue = (long)base.Count;
			Logger.ExitFunction(ExTraceGlobals.FailFastCacheTracer, "PrimaryFailFastUserCache.InsertValueToCache");
		}

		protected override void Dispose(bool isDisposing)
		{
			Logger.EnterFunction(ExTraceGlobals.FailFastCacheTracer, "PrimaryFailFastUserCache.Dispose");
			if (this.primaryObjectBehavior != null)
			{
				this.primaryObjectBehavior.Dispose();
				this.primaryObjectBehavior = null;
			}
			base.Dispose(isDisposing);
			Logger.ExitFunction(ExTraceGlobals.FailFastCacheTracer, "PrimaryFailFastUserCache.Dispose");
		}

		private static TimeSpan GetValidBlockedTime(TimeSpan blockedTime)
		{
			Logger.TraceDebug(ExTraceGlobals.FailFastCacheTracer, "Blocked time before validation is: {0}.", new object[]
			{
				blockedTime
			});
			if (blockedTime <= TimeSpan.Zero)
			{
				return PrimaryFailFastUserCache.defaultBlockedTime;
			}
			if (blockedTime >= PrimaryFailFastUserCache.maxBlockedTime)
			{
				return PrimaryFailFastUserCache.maxBlockedTime;
			}
			return blockedTime;
		}

		private void OnCacheValueBeRemoved(string key, FailFastUserCacheValue cacheValue, RemoveReason reason)
		{
			Logger.TraceInformation(ExTraceGlobals.FailFastCacheTracer, "Remove value from the cache. Key: {0}. CacheValue: {1}. Reason: {2}", new object[]
			{
				key,
				cacheValue.ToString(),
				reason
			});
			FailFastModule.RemotePowershellPerfCounter.FailFastUserCacheSize.RawValue = (long)base.Count;
		}

		private void OnBlockedUserInfoReceived(byte[] message, int receivedSize)
		{
			Logger.EnterFunction(ExTraceGlobals.FailFastCacheTracer, "PrimaryFailFastUserCache.OnBlockedUserInfoReceived");
			string @string = FailFastUserCache.Encoding.GetString(message, 0, receivedSize);
			Logger.TraceDebug(ExTraceGlobals.FailFastCacheTracer, "Blocked user info \"{0}\" received.", new object[]
			{
				@string
			});
			if (string.IsNullOrWhiteSpace(@string))
			{
				Logger.TraceDebug(ExTraceGlobals.FailFastCacheTracer, "Null/Empty/Whitespace info is ignored.", new object[0]);
				return;
			}
			BlockedType blockedType = BlockedType.None;
			long ticks = 0L;
			bool flag = true;
			string[] array = @string.Split(PrimaryFailFastUserCache.blockedInfoSeparatorArray, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length != 3)
			{
				flag = false;
			}
			if (flag && !Enum.TryParse<BlockedType>(array[1], out blockedType))
			{
				flag = false;
			}
			if (flag && !long.TryParse(array[2], out ticks))
			{
				flag = false;
			}
			if (flag)
			{
				string key = array[0];
				this.InsertValueToCache(key, blockedType, new TimeSpan(ticks));
				Logger.ExitFunction(ExTraceGlobals.FailFastCacheTracer, "PrimaryFailFastUserCache.OnBlockedUserInfoReceived");
				return;
			}
			string text = string.Format("The received value {0} is invalid.", @string);
			Logger.LogEvent(TaskEventLogConstants.Tuple_LogIncorrectUserInfoReceivedOnServerStream, null, new object[]
			{
				@string,
				text
			});
			Logger.TraceError(ExTraceGlobals.FailFastCacheTracer, text, new object[0]);
			throw new InvalidOperationException(text);
		}

		private const string UserAddedToFailFastCacheEventKeyPrefix = "UserAddedToFailFastCache:";

		private static readonly object syncRoot = new object();

		private static readonly char[] blockedInfoSeparatorArray = new char[]
		{
			';'
		};

		private static readonly TimeSpan defaultBlockedTime = TimeSpan.FromSeconds(60.0);

		private static readonly TimeSpan maxBlockedTime = TimeSpan.FromSeconds(60.0);

		private static PrimaryFailFastUserCache singleton = null;

		private CrossAppDomainPrimaryObjectBehavior primaryObjectBehavior;
	}
}
