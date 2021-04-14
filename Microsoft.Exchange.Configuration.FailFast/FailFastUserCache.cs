using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics.Components.FailFast;

namespace Microsoft.Exchange.Configuration.FailFast
{
	internal abstract class FailFastUserCache : TimeoutCache<string, FailFastUserCacheValue>
	{
		static FailFastUserCache()
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				FailFastUserCache.pipeNameOfThisProcess = "M.E.C.FailFast.FailFastUserCache.NamedPipe." + currentProcess.Id;
			}
		}

		protected FailFastUserCache(int numberOfBuckets, int maxSizeForBuckets) : base(numberOfBuckets, maxSizeForBuckets, false)
		{
			AppDomain.CurrentDomain.DomainUnload += this.CurrentDomainDomainUnload;
		}

		internal static bool IsPrimaryUserCache
		{
			get
			{
				return FailFastUserCache.isPrimaryUserCache != null && FailFastUserCache.isPrimaryUserCache.Value;
			}
			set
			{
				Logger.TraceDebug(ExTraceGlobals.FailFastCacheTracer, "Set IsPrimaryUserCache to be {0}.", new object[]
				{
					value
				});
				FailFastUserCache.isPrimaryUserCache = new bool?(value);
			}
		}

		internal static FailFastUserCache Instance
		{
			get
			{
				if (!FailFastUserCache.IsPrimaryUserCache)
				{
					return PassiveFailFastUserCache.Singleton;
				}
				return PrimaryFailFastUserCache.Singleton;
			}
		}

		internal static bool FailFastEnabled
		{
			get
			{
				return FailFastUserCache.failFastEnabled;
			}
			set
			{
				Logger.TraceDebug(ExTraceGlobals.FailFastCacheTracer, "Set FailFastEnabled to be {0}.", new object[]
				{
					value
				});
				FailFastUserCache.failFastEnabled = value;
			}
		}

		protected static string PipeNameOfThisProcess
		{
			get
			{
				return FailFastUserCache.pipeNameOfThisProcess;
			}
		}

		protected static Encoding Encoding
		{
			get
			{
				return FailFastUserCache.encoding;
			}
		}

		internal void AddUserToCache(string userToken, BlockedType blockedType, TimeSpan blockedTime)
		{
			if (!FailFastUserCache.FailFastEnabled)
			{
				return;
			}
			Logger.EnterFunction(ExTraceGlobals.FailFastCacheTracer, "FailFastUserCache.BlockUser");
			this.InsertValueToCache(userToken, blockedType, blockedTime);
			Logger.ExitFunction(ExTraceGlobals.FailFastCacheTracer, "FailFastUserCache.BlockUser");
		}

		internal void AddTenantToCache(string tenant, BlockedType blockedType, TimeSpan blockedTime)
		{
			if (!FailFastUserCache.FailFastEnabled)
			{
				return;
			}
			Logger.EnterFunction(ExTraceGlobals.FailFastCacheTracer, "FailFastUserCache.BlockTenant");
			this.InsertValueToCache("Tenant:C8E2A9F6-0E7A-4bcc-95A0-9CE1BCA7EE68:" + tenant, blockedType, blockedTime);
			Logger.ExitFunction(ExTraceGlobals.FailFastCacheTracer, "FailFastUserCache.BlockTenant");
		}

		internal void AddAllUsersToCache(BlockedType blockedType, TimeSpan blockedTime)
		{
			if (!FailFastUserCache.FailFastEnabled)
			{
				return;
			}
			Logger.EnterFunction(ExTraceGlobals.FailFastCacheTracer, "FailFastUserCache.BlockAllUsers");
			this.InsertValueToCache("AllUsers:D3511BCA-379C-4a38-97E5-0FDA0C231C33", blockedType, blockedTime);
			Logger.ExitFunction(ExTraceGlobals.FailFastCacheTracer, "FailFastUserCache.BlockAllUsers");
		}

		internal abstract bool IsUserInCache(string userToken, string userTenant, out string cacheKey, out FailFastUserCacheValue cacheValue, out BlockedReason blockedReason);

		protected abstract void InsertValueToCache(string key, BlockedType blockedType, TimeSpan blockedTime);

		private void CurrentDomainDomainUnload(object sender, EventArgs e)
		{
			base.Dispose();
		}

		protected const char BlockedInfoSeparator = ';';

		protected const string TenantCacheKeyPrefix = "Tenant:C8E2A9F6-0E7A-4bcc-95A0-9CE1BCA7EE68:";

		protected const string AllUsersKey = "AllUsers:D3511BCA-379C-4a38-97E5-0FDA0C231C33";

		private static readonly string pipeNameOfThisProcess;

		private static readonly Encoding encoding = Encoding.UTF8;

		private static bool? isPrimaryUserCache;

		private static bool failFastEnabled = false;
	}
}
