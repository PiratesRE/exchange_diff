using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.Configuration.FailFast.LocStrings
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(2782861920U, "FailBecauseOfServer");
			Strings.stringIDs.Add(2406874761U, "FailBecauseOfSelf");
			Strings.stringIDs.Add(1130355941U, "ErrorRpsNotEnabled");
			Strings.stringIDs.Add(362638241U, "FailBecauseOfTenant");
		}

		public static string RequestBeingBlockedInFailFast(string failedReason)
		{
			return string.Format(Strings.ResourceManager.GetString("RequestBeingBlockedInFailFast"), failedReason);
		}

		public static string FailBecauseOfServer
		{
			get
			{
				return Strings.ResourceManager.GetString("FailBecauseOfServer");
			}
		}

		public static string FailBecauseOfSelf
		{
			get
			{
				return Strings.ResourceManager.GetString("FailBecauseOfSelf");
			}
		}

		public static string ErrorRpsNotEnabled
		{
			get
			{
				return Strings.ResourceManager.GetString("ErrorRpsNotEnabled");
			}
		}

		public static string FailBecauseOfTenant
		{
			get
			{
				return Strings.ResourceManager.GetString("FailBecauseOfTenant");
			}
		}

		public static string ErrorOperationTarpitting(int delaySeconds)
		{
			return string.Format(Strings.ResourceManager.GetString("ErrorOperationTarpitting"), delaySeconds);
		}

		public static string GetLocalizedString(Strings.IDs key)
		{
			return Strings.ResourceManager.GetString(Strings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(4);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.Configuration.FailFast.LocStrings.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			FailBecauseOfServer = 2782861920U,
			FailBecauseOfSelf = 2406874761U,
			ErrorRpsNotEnabled = 1130355941U,
			FailBecauseOfTenant = 362638241U
		}

		private enum ParamIDs
		{
			RequestBeingBlockedInFailFast,
			ErrorOperationTarpitting
		}
	}
}
