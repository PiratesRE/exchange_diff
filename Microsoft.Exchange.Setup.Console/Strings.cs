using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Console
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1222440996U, "Banner");
			Strings.stringIDs.Add(792205043U, "PrereqCheckBanner");
			Strings.stringIDs.Add(1695043038U, "SetupExitsBecauseOfLPPathNotFoundException");
			Strings.stringIDs.Add(1251728767U, "ConfiguringExchangeServer");
			Strings.stringIDs.Add(1331356275U, "CabUtilityWrapperError");
			Strings.stringIDs.Add(2883940283U, "AdditionalErrorDetails");
			Strings.stringIDs.Add(3712765806U, "LPVersioningInvalidValue");
			Strings.stringIDs.Add(951135015U, "SuccessSummary");
			Strings.stringIDs.Add(2390584075U, "TreatPreReqErrorsAsWarnings");
			Strings.stringIDs.Add(4061842413U, "UnableToFindLPVersioning");
			Strings.stringIDs.Add(3856638130U, "SetupExitsBecauseOfTransientException");
		}

		public static LocalizedString Banner
		{
			get
			{
				return new LocalizedString("Banner", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExsetupTerminatedWithControlBreak(string message)
		{
			return new LocalizedString("ExsetupTerminatedWithControlBreak", Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString PrereqCheckBanner
		{
			get
			{
				return new LocalizedString("PrereqCheckBanner", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetupExitsBecauseOfLPPathNotFoundException
		{
			get
			{
				return new LocalizedString("SetupExitsBecauseOfLPPathNotFoundException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConfiguringExchangeServer
		{
			get
			{
				return new LocalizedString("ConfiguringExchangeServer", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CabUtilityWrapperError
		{
			get
			{
				return new LocalizedString("CabUtilityWrapperError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdditionalErrorDetails
		{
			get
			{
				return new LocalizedString("AdditionalErrorDetails", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LPVersioningInvalidValue
		{
			get
			{
				return new LocalizedString("LPVersioningInvalidValue", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotFindFile(string file)
		{
			return new LocalizedString("CannotFindFile", Strings.ResourceManager, new object[]
			{
				file
			});
		}

		public static LocalizedString ExecutionResult(int index, string result)
		{
			return new LocalizedString("ExecutionResult", Strings.ResourceManager, new object[]
			{
				index,
				result
			});
		}

		public static LocalizedString SuccessSummary
		{
			get
			{
				return new LocalizedString("SuccessSummary", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TreatPreReqErrorsAsWarnings
		{
			get
			{
				return new LocalizedString("TreatPreReqErrorsAsWarnings", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToFindLPVersioning
		{
			get
			{
				return new LocalizedString("UnableToFindLPVersioning", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnhandledErrorMessage(string message)
		{
			return new LocalizedString("UnhandledErrorMessage", Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString SetupExitsBecauseOfTransientException
		{
			get
			{
				return new LocalizedString("SetupExitsBecauseOfTransientException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(11);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Setup.Console.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			Banner = 1222440996U,
			PrereqCheckBanner = 792205043U,
			SetupExitsBecauseOfLPPathNotFoundException = 1695043038U,
			ConfiguringExchangeServer = 1251728767U,
			CabUtilityWrapperError = 1331356275U,
			AdditionalErrorDetails = 2883940283U,
			LPVersioningInvalidValue = 3712765806U,
			SuccessSummary = 951135015U,
			TreatPreReqErrorsAsWarnings = 2390584075U,
			UnableToFindLPVersioning = 4061842413U,
			SetupExitsBecauseOfTransientException = 3856638130U
		}

		private enum ParamIDs
		{
			ExsetupTerminatedWithControlBreak,
			CannotFindFile,
			ExecutionResult,
			UnhandledErrorMessage
		}
	}
}
