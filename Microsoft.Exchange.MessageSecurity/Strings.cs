using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessageSecurity
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(470791283U, "NoConfigAdminRoleObjectFound");
			Strings.stringIDs.Add(1274848177U, "NoRootFound");
			Strings.stringIDs.Add(698953728U, "MoreThanOneRootFound");
		}

		public static LocalizedString NoConfigAdminRoleObjectFound
		{
			get
			{
				return new LocalizedString("NoConfigAdminRoleObjectFound", "ExEE2DC4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoDirectoryServiceObjectsFound(string containerDn)
		{
			return new LocalizedString("NoDirectoryServiceObjectsFound", "Ex4DE9DC", false, true, Strings.ResourceManager, new object[]
			{
				containerDn
			});
		}

		public static LocalizedString NoRootFound
		{
			get
			{
				return new LocalizedString("NoRootFound", "Ex1E271C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MoreThanOneRootFound
		{
			get
			{
				return new LocalizedString("MoreThanOneRootFound", "ExD91B34", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(3);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.MessageSecurity.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			NoConfigAdminRoleObjectFound = 470791283U,
			NoRootFound = 1274848177U,
			MoreThanOneRootFound = 698953728U
		}

		private enum ParamIDs
		{
			NoDirectoryServiceObjectsFound
		}
	}
}
