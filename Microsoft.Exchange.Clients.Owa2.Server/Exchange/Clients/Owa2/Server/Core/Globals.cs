using System;
using System.ServiceModel;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public static class Globals
	{
		public static void TestHook_SetFlag(bool enable)
		{
			Globals.Owa2ServerUnitTestsHook = enable;
		}

		public static bool IsInitialized
		{
			get
			{
				return Globals.isInitialized;
			}
			set
			{
				Globals.isInitialized = value;
			}
		}

		public static Exception InitializationError
		{
			get
			{
				return Globals.initializationError;
			}
			set
			{
				Globals.initializationError = value;
			}
		}

		public static string ApplicationVersion
		{
			get
			{
				if (Globals.application == null)
				{
					return string.Empty;
				}
				return Globals.application.ApplicationVersion;
			}
		}

		public static long ApplicationTime
		{
			get
			{
				if (Globals.application == null)
				{
					return 0L;
				}
				return Globals.application.ApplicationTime;
			}
		}

		public static bool ArePerfCountersEnabled
		{
			get
			{
				return Globals.application != null && Globals.application.ArePerfCountersEnabled;
			}
		}

		public static int ActivityBasedPresenceDuration
		{
			get
			{
				if (Globals.application == null)
				{
					return 0;
				}
				return Globals.application.ActivityBasedPresenceDuration;
			}
		}

		public static bool SendWatsonReports
		{
			get
			{
				return Globals.application != null && Globals.application.SendWatsonReports;
			}
		}

		public static int MaxBreadcrumbs
		{
			get
			{
				if (Globals.application == null)
				{
					return 0;
				}
				return Globals.application.MaxBreadcrumbs;
			}
		}

		public static bool LogVerboseNotifications
		{
			get
			{
				return Globals.application != null && Globals.application.LogVerboseNotifications;
			}
		}

		public static bool DisableBreadcrumbs
		{
			get
			{
				return Globals.application == null || Globals.application.DisableBreadcrumbs;
			}
		}

		public static bool CheckForForgottenAttachmentsEnabled
		{
			get
			{
				return Globals.application == null || Globals.application.CheckForForgottenAttachmentsEnabled;
			}
		}

		public static bool ControlTasksQueueDisabled
		{
			get
			{
				return Globals.application == null || Globals.application.ControlTasksQueueDisabled;
			}
		}

		public static string[] BlockedQueryStringValues
		{
			get
			{
				return Globals.blockedQueryStringValues;
			}
		}

		public static HttpClientCredentialType ServiceAuthenticationType
		{
			get
			{
				if (Globals.application == null)
				{
					return HttpClientCredentialType.None;
				}
				return Globals.application.ServiceAuthenticationType;
			}
		}

		public static TroubleshootingContext TroubleshootingContext
		{
			get
			{
				if (Globals.application == null)
				{
					return null;
				}
				return Globals.application.TroubleshootingContext;
			}
		}

		public static bool LogErrorDetails
		{
			get
			{
				return Globals.application != null && Globals.application.LogErrorDetails;
			}
		}

		public static bool LogErrorTraces
		{
			get
			{
				return Globals.application != null && Globals.application.LogErrorTraces;
			}
		}

		public static string ContentDeliveryNetworkEndpoint
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(Globals.testContentDeliveryNetworkEndpoint))
				{
					return Globals.testContentDeliveryNetworkEndpoint;
				}
				if (Globals.application == null)
				{
					return string.Empty;
				}
				return Globals.application.ContentDeliveryNetworkEndpoint;
			}
		}

		public static bool IsAnonymousCalendarApp { get; private set; }

		public static bool IsFirstReleaseFlightingEnabled { get; private set; }

		public static bool IsPreCheckinApp { get; set; }

		public static bool OwaIsNoRecycleEnabled { get; private set; }

		public static double OwaVersionReadingInterval { get; private set; }

		public static void TestHook_SetContentDeliveryNetworkEndpoint(string cdn)
		{
			Globals.testContentDeliveryNetworkEndpoint = cdn;
		}

		internal static void Initialize()
		{
			Globals.application = BaseApplication.CreateInstance();
			Globals.IsAnonymousCalendarApp = (Globals.application is OwaAnonymousApplication);
			Globals.IsPreCheckinApp = Globals.application.IsPreCheckinApp;
			Globals.IsFirstReleaseFlightingEnabled = Globals.application.IsFirstReleaseFlightingEnabled;
			Globals.OwaIsNoRecycleEnabled = Globals.application.OwaIsNoRecycleEnabled;
			Globals.OwaVersionReadingInterval = Globals.application.OwaVersionReadingInterval;
			Globals.application.Initialize();
			if (Globals.application.BlockedQueryStringValues != null)
			{
				Globals.blockedQueryStringValues = Globals.application.BlockedQueryStringValues.Split(new string[]
				{
					";"
				}, StringSplitOptions.RemoveEmptyEntries);
			}
		}

		internal static void Dispose()
		{
			if (Globals.application != null)
			{
				Globals.application.Dispose();
			}
		}

		internal static void UpdateErrorTracingConfiguration()
		{
			if (Globals.application != null)
			{
				Globals.application.UpdateErrorTracingConfiguration();
			}
		}

		internal static string FormatURIForCDN(string relativeUri)
		{
			if (string.IsNullOrEmpty(Globals.ContentDeliveryNetworkEndpoint))
			{
				return relativeUri;
			}
			if (new Uri(relativeUri, UriKind.RelativeOrAbsolute).IsAbsoluteUri)
			{
				return relativeUri;
			}
			StringBuilder stringBuilder = new StringBuilder(Globals.ContentDeliveryNetworkEndpoint);
			if (!Globals.ContentDeliveryNetworkEndpoint.EndsWith("/"))
			{
				stringBuilder.Append("/");
			}
			stringBuilder.Append("owa/");
			stringBuilder.Append(relativeUri);
			return stringBuilder.ToString();
		}

		public static bool Owa2ServerUnitTestsHook;

		private static BaseApplication application;

		private static bool isInitialized;

		private static Exception initializationError;

		private static string testContentDeliveryNetworkEndpoint = null;

		private static string[] blockedQueryStringValues;
	}
}
