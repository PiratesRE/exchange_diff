using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.GlobalLocatorService;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Authorization;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal static class UserValidationHelper
	{
		private static GlsDirectorySession GlsSession
		{
			get
			{
				if (UserValidationHelper.glsSession == null)
				{
					UserValidationHelper.glsSession = new GlsDirectorySession(GlsCallerId.Exchange);
				}
				return UserValidationHelper.glsSession;
			}
		}

		internal static bool ValidateFilteringOnlyUser(string domain, string username)
		{
			if (string.IsNullOrEmpty(domain) || !VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.ValidateFilteringOnlyUser.Enabled)
			{
				return false;
			}
			if (username.EndsWith(".exchangemon.net", StringComparison.InvariantCultureIgnoreCase))
			{
				AuthZLogger.SafeAppendGenericInfo("ValidateFilteringOnlyUser", string.Format("Bypass monitoring account {0} check.", username));
				return false;
			}
			bool result;
			try
			{
				bool flag = false;
				domain = domain.ToLower();
				if (!UserValidationHelper.filteringOnlyCache.TryGetValue(domain, out flag))
				{
					CustomerType customerType = CustomerType.None;
					Guid guid;
					string text;
					string text2;
					UserValidationHelper.GlsSession.GetFfoTenantSettingsByDomain(domain, out guid, out text, out text2, out customerType);
					flag = (customerType == CustomerType.FilteringOnly);
					UserValidationHelper.filteringOnlyCache.TryInsertAbsolute(domain, flag, UserValidationHelper.DefaultAbsoluteTimeout);
					ExTraceGlobals.PublicPluginAPITracer.TraceDebug(0L, "[UserValidationHelper.ValidateFilteringOnlyUser] Domain:{0} belongs to TenantId:{1}, Region:{2}, Version: {3}, CustomerType: {4}.", new object[]
					{
						domain,
						guid,
						text,
						text2,
						customerType
					});
					AuthZLogger.SafeAppendGenericInfo("ValidateFilteringOnlyUser", string.Format("Domain:{0} belongs to TenantId:{1}, Region:{2}, Version: {3}, CustomerType: {4}.", new object[]
					{
						domain,
						guid,
						text,
						text2,
						customerType
					}));
				}
				else
				{
					AuthZLogger.SafeAppendGenericInfo("ValidateFilteringOnlyUser", string.Format("HitCache Domain: {0} is filteringOnly: {1}.", domain, flag));
				}
				result = flag;
			}
			catch (Exception ex)
			{
				ExTraceGlobals.PublicPluginAPITracer.TraceError<Exception>(0L, "[UserValidationHelper.ValidateFilteringOnlyUser] Exception:{0}", ex);
				AuthZLogger.SafeAppendGenericError("ValidateFilteringOnlyUser", ex, new Func<Exception, bool>(KnownException.IsUnhandledException));
				result = false;
			}
			return result;
		}

		private const int FilteringOnlyUserCacheSize = 10240;

		internal const string MultiTenantTestDomain = ".exchangemon.net";

		private static GlsDirectorySession glsSession;

		private static readonly ExactTimeoutCache<string, bool> filteringOnlyCache = new ExactTimeoutCache<string, bool>(null, null, null, 10240, false);

		private static readonly TimeSpan DefaultAbsoluteTimeout = TimeSpan.FromMinutes(30.0);
	}
}
