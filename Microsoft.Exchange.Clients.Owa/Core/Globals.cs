using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public static class Globals
	{
		internal static OWAVDirType OwaVDirType { get; private set; }

		internal static void Initialize(OWAVDirType owaVDirType)
		{
			Globals.OwaVDirType = owaVDirType;
			Globals.applicationStopwatch.Start();
			if (owaVDirType == OWAVDirType.OWA)
			{
				Globals.owaSettings = new OwaSettingsLoader();
			}
			else if (owaVDirType == OWAVDirType.Calendar)
			{
				Globals.owaSettings = new CalendarVDirSettingsLoader();
			}
			Globals.owaSettings.Load();
		}

		internal static void UnloadOwaSettings()
		{
			if (Globals.owaSettings != null)
			{
				Globals.owaSettings.Unload();
			}
		}

		public static bool IsPushNotificationsEnabled
		{
			get
			{
				return Globals.owaSettings.IsPushNotificationsEnabled;
			}
		}

		public static bool IsPullNotificationsEnabled
		{
			get
			{
				return Globals.owaSettings.IsPullNotificationsEnabled;
			}
		}

		public static bool IsFolderContentNotificationsEnabled
		{
			get
			{
				return Globals.owaSettings.IsFolderContentNotificationsEnabled;
			}
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

		public static CultureInfo ServerCulture
		{
			get
			{
				if (!Globals.IsInitialized && Globals.owaSettings.ServerCulture == null)
				{
					return Culture.GetCultureInfoInstance(1033);
				}
				return Globals.owaSettings.ServerCulture;
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
				return OwaRegistryKeys.OwaBasicVersion;
			}
		}

		public static ServerVersion LocalHostVersion
		{
			get
			{
				return Globals.owaSettings.LocalHostVersion;
			}
		}

		public static Dictionary<ServerVersion, ServerVersion> LocalVersionFolders
		{
			get
			{
				return Globals.owaSettings.LocalVersionFolders;
			}
		}

		public static string ScriptDirectory
		{
			get
			{
				return Globals.ApplicationVersion + "/scripts/";
			}
		}

		public static bool ArePerfCountersEnabled
		{
			get
			{
				return Globals.owaSettings.ArePerfCountersEnabled;
			}
		}

		public static int MaxSearchStringLength
		{
			get
			{
				return Globals.owaSettings.MaxSearchStringLength;
			}
		}

		public static int AutoSaveInterval
		{
			get
			{
				return Globals.owaSettings.AutoSaveInterval;
			}
		}

		public static bool ChangeExpiredPasswordEnabled
		{
			get
			{
				return Globals.owaSettings.ChangeExpiredPasswordEnabled;
			}
		}

		public static int ConnectionCacheSize
		{
			get
			{
				return Globals.owaSettings.ConnectionCacheSize;
			}
		}

		public static bool ShowDebugInformation
		{
			get
			{
				return Globals.owaSettings.ShowDebugInformation;
			}
		}

		public static bool EnableEmailReports
		{
			get
			{
				return Globals.owaSettings.EnableEmailReports;
			}
		}

		public static bool ListenAdNotifications
		{
			get
			{
				return Globals.owaSettings.ListenAdNotifications;
			}
		}

		public static bool RenderBreadcrumbsInAboutPage
		{
			get
			{
				return Globals.owaSettings.RenderBreadcrumbsInAboutPage;
			}
		}

		public static bool CollectPerRequestPerformanceStats
		{
			get
			{
				return Globals.owaSettings.CollectPerRequestPerformanceStats;
			}
		}

		public static bool CollectSearchStrings
		{
			get
			{
				return Globals.owaSettings.CollectSearchStrings;
			}
		}

		public static bool DisablePrefixSearch
		{
			get
			{
				return Globals.owaSettings.DisablePrefixSearch;
			}
		}

		public static bool FilterETag
		{
			get
			{
				return Globals.owaSettings.FilterETag;
			}
		}

		public static string ContentDeliveryNetworkEndpoint
		{
			get
			{
				return Globals.owaSettings.ContentDeliveryNetworkEndpoint;
			}
		}

		public static string ErrorReportAddress
		{
			get
			{
				return Globals.owaSettings.ErrorReportAddress;
			}
		}

		public static int MaximumTemporaryFilteredViewPerUser
		{
			get
			{
				return Globals.owaSettings.MaximumTemporaryFilteredViewPerUser;
			}
		}

		public static int MaximumFilteredViewInFavoritesPerUser
		{
			get
			{
				return Globals.owaSettings.MaximumFilteredViewInFavoritesPerUser;
			}
		}

		public static bool SendWatsonReports
		{
			get
			{
				return Globals.owaSettings.SendWatsonReports;
			}
		}

		public static bool SendClientWatsonReports
		{
			get
			{
				return Globals.owaSettings.SendClientWatsonReports;
			}
		}

		public static bool DisableBreadcrumbs
		{
			get
			{
				return Globals.owaSettings.DisableBreadcrumbs;
			}
		}

		public static bool IsPreCheckinApp
		{
			get
			{
				return Globals.owaSettings.IsPreCheckinApp;
			}
		}

		public static int ServicePointConnectionLimit
		{
			get
			{
				return Globals.owaSettings.ServicePointConnectionLimit;
			}
		}

		public static bool ProxyToLocalHost
		{
			get
			{
				return Globals.owaSettings.ProxyToLocalHost;
			}
		}

		public static int MaxBreadcrumbs
		{
			get
			{
				return Globals.owaSettings.MaxBreadcrumbs;
			}
		}

		public static bool StoreTransientExceptionEventLogEnabled
		{
			get
			{
				return Globals.owaSettings.StoreTransientExceptionEventLogEnabled;
			}
		}

		public static int StoreTransientExceptionEventLogThreshold
		{
			get
			{
				return Globals.owaSettings.StoreTransientExceptionEventLogThreshold;
			}
		}

		public static int StoreTransientExceptionEventLogFrequencyInSeconds
		{
			get
			{
				return Globals.owaSettings.StoreTransientExceptionEventLogFrequencyInSeconds;
			}
		}

		public static int MaxPendingRequestLifeInSeconds
		{
			get
			{
				return Globals.owaSettings.MaxPendingRequestLifeInSeconds;
			}
		}

		public static int MaxItemsInConversationExpansion
		{
			get
			{
				return Globals.owaSettings.MaxItemsInConversationExpansion;
			}
		}

		public static int MaxItemsInConversationReadingPane
		{
			get
			{
				return Globals.owaSettings.MaxItemsInConversationReadingPane;
			}
		}

		public static long MaxBytesInConversationReadingPane
		{
			get
			{
				return Globals.owaSettings.MaxBytesInConversationReadingPane;
			}
		}

		public static bool HideDeletedItems
		{
			get
			{
				return Globals.owaSettings.HideDeletedItems;
			}
		}

		public static string OCSServerName
		{
			get
			{
				return Globals.owaSettings.OCSServerName;
			}
		}

		public static int ActivityBasedPresenceDuration
		{
			get
			{
				return Globals.owaSettings.ActivityBasedPresenceDuration;
			}
		}

		public static int MailTipsMaxClientCacheSize
		{
			get
			{
				return Globals.owaSettings.MailTipsMaxClientCacheSize;
			}
		}

		public static int MailTipsMaxMailboxSourcedRecipientSize
		{
			get
			{
				return Globals.owaSettings.MailTipsMaxMailboxSourcedRecipientSize;
			}
		}

		public static int MailTipsClientCacheEntryExpiryInHours
		{
			get
			{
				return Globals.owaSettings.MailTipsClientCacheEntryExpiryInHours;
			}
		}

		internal static PhishingLevel MinimumSuspiciousPhishingLevel
		{
			get
			{
				return Globals.owaSettings.MinimumSuspiciousPhishingLevel;
			}
		}

		public static long ApplicationTime
		{
			get
			{
				return Globals.applicationStopwatch.ElapsedMilliseconds;
			}
		}

		public static bool CanaryProtectionRequired
		{
			get
			{
				return Globals.OwaVDirType == OWAVDirType.OWA;
			}
		}

		internal static int UserContextLockTimeout
		{
			get
			{
				return Globals.owaSettings.UserContextLockTimeout;
			}
		}

		public const int AutoCompleteCacheVersion = 3;

		public const int PasswordExpirationNotificationDays = 14;

		public const string DocumentLibraryNamespace = "DocumentLibrary";

		public const string SendByEmail = "SendByEmail";

		public const int MaxSubjectLength = 255;

		public const int MaxInviteMessageLength = 300;

		private const int FailoverServerLcid = 1033;

		private const double MinUserTimeoutMaxPendingLifeRatio = 1.25;

		private const int DefaultMaximumTemporaryFilteredViewPerUser = 60;

		private const int DefaultMaximumFilteredViewInFavoritesPerUser = 25;

		internal static readonly string HtmlDirectionCharacterString = new string('‎', 1);

		public static readonly string CopyrightMessage = "Copyright (c) 2006 Microsoft Corporation.  All rights reserved.";

		public static readonly string SupportedBrowserHelpUrl = "http://go.microsoft.com/fwlink/?LinkID=129362";

		public static readonly string VirtualRootName = "owa";

		public static readonly string RealmParameter = "realm";

		private static Stopwatch applicationStopwatch = new Stopwatch();

		private static bool isInitialized;

		private static Exception initializationError;

		private static OwaSettingsLoaderBase owaSettings;
	}
}
