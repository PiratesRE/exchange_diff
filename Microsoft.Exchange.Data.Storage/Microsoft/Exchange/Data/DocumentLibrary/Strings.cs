using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(926468432U, "ExCorruptData");
			Strings.stringIDs.Add(2758509155U, "ExProxyConnectionFailure");
			Strings.stringIDs.Add(2298744173U, "ExUnknownError");
			Strings.stringIDs.Add(2403235179U, "ExCorruptRegionalSetting");
			Strings.stringIDs.Add(1158351855U, "ExConnectionFailure");
		}

		public static string ExCorruptData
		{
			get
			{
				return Strings.ResourceManager.GetString("ExCorruptData");
			}
		}

		public static string ExCannotConnectToMachine(string machineName)
		{
			return string.Format(Strings.ResourceManager.GetString("ExCannotConnectToMachine"), machineName);
		}

		public static string ExProxyConnectionFailure
		{
			get
			{
				return Strings.ResourceManager.GetString("ExProxyConnectionFailure");
			}
		}

		public static string ExObjectMovedOrDeleted(string fileName)
		{
			return string.Format(Strings.ResourceManager.GetString("ExObjectMovedOrDeleted"), fileName);
		}

		public static string ExFilterNotSupported(Type type)
		{
			return string.Format(Strings.ResourceManager.GetString("ExFilterNotSupported"), type);
		}

		public static string ExDocumentStreamAccessDenied(string fileName)
		{
			return string.Format(Strings.ResourceManager.GetString("ExDocumentStreamAccessDenied"), fileName);
		}

		public static string ExDocumentModified(string fileName)
		{
			return string.Format(Strings.ResourceManager.GetString("ExDocumentModified"), fileName);
		}

		public static string ExAccessDeniedForGetViewUnder(string directoryName)
		{
			return string.Format(Strings.ResourceManager.GetString("ExAccessDeniedForGetViewUnder"), directoryName);
		}

		public static string ExAccessDenied(object targetObject)
		{
			return string.Format(Strings.ResourceManager.GetString("ExAccessDenied"), targetObject);
		}

		public static string ExUnknownError
		{
			get
			{
				return Strings.ResourceManager.GetString("ExUnknownError");
			}
		}

		public static string ExCorruptRegionalSetting
		{
			get
			{
				return Strings.ResourceManager.GetString("ExCorruptRegionalSetting");
			}
		}

		public static string ExConnectionFailure
		{
			get
			{
				return Strings.ResourceManager.GetString("ExConnectionFailure");
			}
		}

		public static string ExObjectNotFound(string fileName)
		{
			return string.Format(Strings.ResourceManager.GetString("ExObjectNotFound"), fileName);
		}

		public static string ExPathTooLong(string fileName)
		{
			return string.Format(Strings.ResourceManager.GetString("ExPathTooLong"), fileName);
		}

		public static string GetLocalizedString(Strings.IDs key)
		{
			return Strings.ResourceManager.GetString(Strings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(5);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.Data.DocumentLibrary.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ExCorruptData = 926468432U,
			ExProxyConnectionFailure = 2758509155U,
			ExUnknownError = 2298744173U,
			ExCorruptRegionalSetting = 2403235179U,
			ExConnectionFailure = 1158351855U
		}

		private enum ParamIDs
		{
			ExCannotConnectToMachine,
			ExObjectMovedOrDeleted,
			ExFilterNotSupported,
			ExDocumentStreamAccessDenied,
			ExDocumentModified,
			ExAccessDeniedForGetViewUnder,
			ExAccessDenied,
			ExObjectNotFound,
			ExPathTooLong
		}
	}
}
