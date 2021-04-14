using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Office.CompliancePolicy.Exchange.Dar.LocStrings
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(2339760653U, "TaskTypeUnknown");
			Strings.stringIDs.Add(952580535U, "TaskCannotBeRestored");
			Strings.stringIDs.Add(4014022951U, "TaskIsDisabled");
			Strings.stringIDs.Add(883701452U, "TaskNotFound");
			Strings.stringIDs.Add(386282707U, "TaskAlreadyExists");
			Strings.stringIDs.Add(648305924U, "TenantMustBeSpecified");
		}

		public static string TaskTypeUnknown
		{
			get
			{
				return Strings.ResourceManager.GetString("TaskTypeUnknown");
			}
		}

		public static string TaskCannotBeRestored
		{
			get
			{
				return Strings.ResourceManager.GetString("TaskCannotBeRestored");
			}
		}

		public static string TaskIsDisabled
		{
			get
			{
				return Strings.ResourceManager.GetString("TaskIsDisabled");
			}
		}

		public static string TaskNotFound
		{
			get
			{
				return Strings.ResourceManager.GetString("TaskNotFound");
			}
		}

		public static string TaskAlreadyExists
		{
			get
			{
				return Strings.ResourceManager.GetString("TaskAlreadyExists");
			}
		}

		public static string TenantMustBeSpecified
		{
			get
			{
				return Strings.ResourceManager.GetString("TenantMustBeSpecified");
			}
		}

		public static string ErrorDuringDarCall(string correlationId)
		{
			return string.Format(Strings.ResourceManager.GetString("ErrorDuringDarCall"), correlationId);
		}

		public static string GetLocalizedString(Strings.IDs key)
		{
			return Strings.ResourceManager.GetString(Strings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(6);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Office.CompliancePolicy.Exchange.Dar.LocStrings.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			TaskTypeUnknown = 2339760653U,
			TaskCannotBeRestored = 952580535U,
			TaskIsDisabled = 4014022951U,
			TaskNotFound = 883701452U,
			TaskAlreadyExists = 386282707U,
			TenantMustBeSpecified = 648305924U
		}

		private enum ParamIDs
		{
			ErrorDuringDarCall
		}
	}
}
