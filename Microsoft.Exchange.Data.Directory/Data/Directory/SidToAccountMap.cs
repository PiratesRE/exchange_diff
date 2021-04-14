using System;
using System.Security.Principal;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Data.Directory
{
	internal class SidToAccountMap : LazyLookupExactTimeoutCache<SecurityIdentifier, string>
	{
		private SidToAccountMap() : base(1000, false, TimeSpan.FromMinutes(5.0), TimeSpan.FromHours(1.0), CacheFullBehavior.ExpireExisting)
		{
		}

		protected override string CreateOnCacheMiss(SecurityIdentifier key, ref bool shouldAdd)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			string result;
			try
			{
				NTAccount ntaccount = key.Translate(typeof(NTAccount)) as NTAccount;
				result = ntaccount.Value;
			}
			catch (IdentityNotMappedException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<SecurityIdentifier, string>((long)this.GetHashCode(), "[SidToAccountMap::CreateOnCacheMiss] Failed to map SID {0} to NTAccount.  Message: '{1}'", key, ex.Message);
				result = key.ToString();
			}
			catch (SystemException ex2)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<SecurityIdentifier, string>((long)this.GetHashCode(), "[SidToAccountMap::CreateOnCacheMiss] Failed to map SID {0} to NTAccount.  Message: '{1}'", key, ex2.Message);
				result = key.ToString();
			}
			return result;
		}

		public static readonly SidToAccountMap Singleton = new SidToAccountMap();
	}
}
