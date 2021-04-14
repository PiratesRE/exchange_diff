using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.Net
{
	internal static class MonitoringWebClientStrings
	{
		static MonitoringWebClientStrings()
		{
			MonitoringWebClientStrings.stringIDs.Add(3908151889U, "ScenarioCanceled");
			MonitoringWebClientStrings.stringIDs.Add(644136196U, "UnexpectedResponseCodeReceived");
			MonitoringWebClientStrings.stringIDs.Add(2108302982U, "MissingLiveIdAuthCookies");
			MonitoringWebClientStrings.stringIDs.Add(1647397280U, "HealthCheckRequestFailed");
			MonitoringWebClientStrings.stringIDs.Add(3546529694U, "BrickAuthenticationMissingOkOrLanguageSelection");
			MonitoringWebClientStrings.stringIDs.Add(3494523549U, "EcpErrorPage");
			MonitoringWebClientStrings.stringIDs.Add(3040841166U, "MissingFormAction");
			MonitoringWebClientStrings.stringIDs.Add(1698098118U, "MissingFbaAuthCookies");
			MonitoringWebClientStrings.stringIDs.Add(3677395520U, "OwaErrorPage");
			MonitoringWebClientStrings.stringIDs.Add(1362173822U, "NoRedirectToEcpAfterLanguageSelection");
			MonitoringWebClientStrings.stringIDs.Add(1527735966U, "MissingUserContextCookie");
			MonitoringWebClientStrings.stringIDs.Add(3209757262U, "PassiveDatabase");
			MonitoringWebClientStrings.stringIDs.Add(3315439992U, "CafeErrorPage");
		}

		public static string ScenarioCanceled
		{
			get
			{
				return MonitoringWebClientStrings.ResourceManager.GetString("ScenarioCanceled");
			}
		}

		public static string UnexpectedResponseCodeReceived
		{
			get
			{
				return MonitoringWebClientStrings.ResourceManager.GetString("UnexpectedResponseCodeReceived");
			}
		}

		public static string ScenarioExceptionInnerException(string message)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("ScenarioExceptionInnerException"), message);
		}

		public static string MissingLiveIdAuthCookies
		{
			get
			{
				return MonitoringWebClientStrings.ResourceManager.GetString("MissingLiveIdAuthCookies");
			}
		}

		public static string NoResponseFromServerThroughPublicName(string serverName, int testCount, Uri publicName)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("NoResponseFromServerThroughPublicName"), serverName, testCount, publicName);
		}

		public static string HealthCheckRequestFailed
		{
			get
			{
				return MonitoringWebClientStrings.ResourceManager.GetString("HealthCheckRequestFailed");
			}
		}

		public static string MissingOwaFbaRedirectPage(string missingKeyword)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("MissingOwaFbaRedirectPage"), missingKeyword);
		}

		public static string MissingStaticFile(string missingKeywords)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("MissingStaticFile"), missingKeywords);
		}

		public static string ScenarioTimedOut(double maxTime, double totalTime)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("ScenarioTimedOut"), maxTime, totalTime);
		}

		public static string MissingPreloadPage(string marker)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("MissingPreloadPage"), marker);
		}

		public static string MissingUserNameFieldFromAdfsResponse(string marker)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("MissingUserNameFieldFromAdfsResponse"), marker);
		}

		public static string MissingOwaFbaPage(string missingKeyword)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("MissingOwaFbaPage"), missingKeyword);
		}

		public static string ActualStatusCode(string statusCodes)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("ActualStatusCode"), statusCodes);
		}

		public static string MissingEcpStartPage(string missingKeywords)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("MissingEcpStartPage"), missingKeywords);
		}

		public static string MissingJavascriptEmptyBody(string variableName)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("MissingJavascriptEmptyBody"), variableName);
		}

		public static string BrickAuthenticationMissingOkOrLanguageSelection
		{
			get
			{
				return MonitoringWebClientStrings.ResourceManager.GetString("BrickAuthenticationMissingOkOrLanguageSelection");
			}
		}

		public static string EcpErrorPage
		{
			get
			{
				return MonitoringWebClientStrings.ResourceManager.GetString("EcpErrorPage");
			}
		}

		public static string MissingFormAction
		{
			get
			{
				return MonitoringWebClientStrings.ResourceManager.GetString("MissingFormAction");
			}
		}

		public static string MissingJsonVariable(string variableName)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("MissingJsonVariable"), variableName);
		}

		public static string MissingOwaStartPage(string missingKeywords)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("MissingOwaStartPage"), missingKeywords);
		}

		public static string MissingFbaAuthCookies
		{
			get
			{
				return MonitoringWebClientStrings.ResourceManager.GetString("MissingFbaAuthCookies");
			}
		}

		public static string BadPreloadPath(string variableName)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("BadPreloadPath"), variableName);
		}

		public static string MissingEcpHelpDeskStartPage(string missingKeyword)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("MissingEcpHelpDeskStartPage"), missingKeyword);
		}

		public static string LogonError(string errorMessage)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("LogonError"), errorMessage);
		}

		public static string MissingLiveIdCompactToken(string redirectionLocation)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("MissingLiveIdCompactToken"), redirectionLocation);
		}

		public static string OwaErrorPage
		{
			get
			{
				return MonitoringWebClientStrings.ResourceManager.GetString("OwaErrorPage");
			}
		}

		public static string ExpectedStatusCode(string statusCodes)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("ExpectedStatusCode"), statusCodes);
		}

		public static string NoRedirectToEcpAfterLanguageSelection
		{
			get
			{
				return MonitoringWebClientStrings.ResourceManager.GetString("NoRedirectToEcpAfterLanguageSelection");
			}
		}

		public static string MissingJavascriptVariable(string variableName)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("MissingJavascriptVariable"), variableName);
		}

		public static string NameResolutionFailure(string hostName)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("NameResolutionFailure"), hostName);
		}

		public static string MissingUserContextCookie
		{
			get
			{
				return MonitoringWebClientStrings.ResourceManager.GetString("MissingUserContextCookie");
			}
		}

		public static string PassiveDatabase
		{
			get
			{
				return MonitoringWebClientStrings.ResourceManager.GetString("PassiveDatabase");
			}
		}

		public static string MissingPasswordFieldFromAdfsResponse(string marker)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("MissingPasswordFieldFromAdfsResponse"), marker);
		}

		public static string CafeErrorPage
		{
			get
			{
				return MonitoringWebClientStrings.ResourceManager.GetString("CafeErrorPage");
			}
		}

		public static string ScenarioExceptionMessageHeader(string exceptionType, string baseMessage, string failureSource, string failureReason, string component, string hint)
		{
			return string.Format(MonitoringWebClientStrings.ResourceManager.GetString("ScenarioExceptionMessageHeader"), new object[]
			{
				exceptionType,
				baseMessage,
				failureSource,
				failureReason,
				component,
				hint
			});
		}

		public static string GetLocalizedString(MonitoringWebClientStrings.IDs key)
		{
			return MonitoringWebClientStrings.ResourceManager.GetString(MonitoringWebClientStrings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(13);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.Net.MonitoringWebClientStrings", typeof(MonitoringWebClientStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ScenarioCanceled = 3908151889U,
			UnexpectedResponseCodeReceived = 644136196U,
			MissingLiveIdAuthCookies = 2108302982U,
			HealthCheckRequestFailed = 1647397280U,
			BrickAuthenticationMissingOkOrLanguageSelection = 3546529694U,
			EcpErrorPage = 3494523549U,
			MissingFormAction = 3040841166U,
			MissingFbaAuthCookies = 1698098118U,
			OwaErrorPage = 3677395520U,
			NoRedirectToEcpAfterLanguageSelection = 1362173822U,
			MissingUserContextCookie = 1527735966U,
			PassiveDatabase = 3209757262U,
			CafeErrorPage = 3315439992U
		}

		private enum ParamIDs
		{
			ScenarioExceptionInnerException,
			NoResponseFromServerThroughPublicName,
			MissingOwaFbaRedirectPage,
			MissingStaticFile,
			ScenarioTimedOut,
			MissingPreloadPage,
			MissingUserNameFieldFromAdfsResponse,
			MissingOwaFbaPage,
			ActualStatusCode,
			MissingEcpStartPage,
			MissingJavascriptEmptyBody,
			MissingJsonVariable,
			MissingOwaStartPage,
			BadPreloadPath,
			MissingEcpHelpDeskStartPage,
			LogonError,
			MissingLiveIdCompactToken,
			ExpectedStatusCode,
			MissingJavascriptVariable,
			NameResolutionFailure,
			MissingPasswordFieldFromAdfsResponse,
			ScenarioExceptionMessageHeader
		}
	}
}
