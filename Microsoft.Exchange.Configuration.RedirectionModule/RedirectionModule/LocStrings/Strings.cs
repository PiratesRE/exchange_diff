using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.RedirectionModule.LocStrings
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1271656904U, "ExchangeAuthzAccessDenied");
			Strings.stringIDs.Add(3367785769U, "RemotePowerShellNotEnabled");
		}

		public static LocalizedString ExchangeClientVersionBlocked(string serverVersion)
		{
			return new LocalizedString("ExchangeClientVersionBlocked", "ExF86F19", false, true, Strings.ResourceManager, new object[]
			{
				serverVersion
			});
		}

		public static LocalizedString ExchangeAuthzAccessDenied
		{
			get
			{
				return new LocalizedString("ExchangeAuthzAccessDenied", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemotePowerShellNotEnabled
		{
			get
			{
				return new LocalizedString("RemotePowerShellNotEnabled", "Ex6C03C5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotRedirectCurrentRequest(Exception ex)
		{
			return new LocalizedString("CannotRedirectCurrentRequest", "Ex59C850", false, true, Strings.ResourceManager, new object[]
			{
				ex
			});
		}

		public static LocalizedString AmbiguousTargetSite(string domainName, int minorPartnerId, string identities)
		{
			return new LocalizedString("AmbiguousTargetSite", "Ex3791C3", false, true, Strings.ResourceManager, new object[]
			{
				domainName,
				minorPartnerId,
				identities
			});
		}

		public static LocalizedString FailedToResolveTargetSite(string domainName, int minorPartnerId)
		{
			return new LocalizedString("FailedToResolveTargetSite", "ExE8C84F", false, true, Strings.ResourceManager, new object[]
			{
				domainName,
				minorPartnerId
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(2);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Configuration.RedirectionModule.LocStrings.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ExchangeAuthzAccessDenied = 1271656904U,
			RemotePowerShellNotEnabled = 3367785769U
		}

		private enum ParamIDs
		{
			ExchangeClientVersionBlocked,
			CannotRedirectCurrentRequest,
			AmbiguousTargetSite,
			FailedToResolveTargetSite
		}
	}
}
