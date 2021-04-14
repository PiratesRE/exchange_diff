using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.Management.CompliancePolicy.LocStrings
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(978984437U, "ResolvedServer");
			Strings.stringIDs.Add(883701452U, "TaskNotFound");
			Strings.stringIDs.Add(1421563760U, "ResolvedOrg");
		}

		public static string ResolvedServer
		{
			get
			{
				return Strings.ResourceManager.GetString("ResolvedServer");
			}
		}

		public static string TaskNotFound
		{
			get
			{
				return Strings.ResourceManager.GetString("TaskNotFound");
			}
		}

		public static string ResolvedOrg
		{
			get
			{
				return Strings.ResourceManager.GetString("ResolvedOrg");
			}
		}

		public static string GetLocalizedString(Strings.IDs key)
		{
			return Strings.ResourceManager.GetString(Strings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(3);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.Management.CompliancePolicy.LocStrings.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ResolvedServer = 978984437U,
			TaskNotFound = 883701452U,
			ResolvedOrg = 1421563760U
		}
	}
}
