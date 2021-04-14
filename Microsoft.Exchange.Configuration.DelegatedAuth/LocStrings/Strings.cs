using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.DelegatedAuthentication.LocStrings
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(121065743U, "CannotResolveWindowsIdentityException");
			Strings.stringIDs.Add(2246886311U, "DelegatedAccessDeniedException");
			Strings.stringIDs.Add(1563788146U, "SecurityTokenExpired");
		}

		public static LocalizedString CannotResolveWindowsIdentityException
		{
			get
			{
				return new LocalizedString("CannotResolveWindowsIdentityException", "ExCD01B5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotResolveSidToADAccountException(string userId)
		{
			return new LocalizedString("CannotResolveSidToADAccountException", "ExA87173", false, true, Strings.ResourceManager, new object[]
			{
				userId
			});
		}

		public static LocalizedString CannotResolveUserTenantException(string userId)
		{
			return new LocalizedString("CannotResolveUserTenantException", "Ex26165F", false, true, Strings.ResourceManager, new object[]
			{
				userId
			});
		}

		public static LocalizedString DelegatedAccessDeniedException
		{
			get
			{
				return new LocalizedString("DelegatedAccessDeniedException", "Ex007002", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelegatedServerErrorException(Exception ex)
		{
			return new LocalizedString("DelegatedServerErrorException", "Ex622435", false, true, Strings.ResourceManager, new object[]
			{
				ex
			});
		}

		public static LocalizedString SecurityTokenExpired
		{
			get
			{
				return new LocalizedString("SecurityTokenExpired", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotResolveCurrentKeyException(bool currentKey)
		{
			return new LocalizedString("CannotResolveCurrentKeyException", "Ex78D2C1", false, true, Strings.ResourceManager, new object[]
			{
				currentKey
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(3);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Configuration.DelegatedAuthentication.LocStrings.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			CannotResolveWindowsIdentityException = 121065743U,
			DelegatedAccessDeniedException = 2246886311U,
			SecurityTokenExpired = 1563788146U
		}

		private enum ParamIDs
		{
			CannotResolveSidToADAccountException,
			CannotResolveUserTenantException,
			DelegatedServerErrorException,
			CannotResolveCurrentKeyException
		}
	}
}
