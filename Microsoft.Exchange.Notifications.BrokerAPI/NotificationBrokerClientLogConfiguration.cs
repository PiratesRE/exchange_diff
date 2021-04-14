using System;
using System.IO;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class NotificationBrokerClientLogConfiguration : ILogConfiguration
	{
		public NotificationBrokerClientLogConfiguration()
		{
			this.IsLoggingEnabled = AppConfigLoader.GetConfigBoolValue("IsClientLoggingEnabled", true);
			this.LogPath = AppConfigLoader.GetConfigStringValue("ClientLogPath", NotificationBrokerClientLogConfiguration.DefaultLogPath);
			this.MaxLogAge = AppConfigLoader.GetConfigTimeSpanValue("ClientMaxLogAge", TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromDays(30.0));
			this.MaxLogDirectorySizeInBytes = (long)ByteQuantifiedSize.FromGB(1UL).ToBytes();
			this.MaxLogFileSizeInBytes = (long)ByteQuantifiedSize.FromMB(10UL).ToBytes();
		}

		public static string DefaultLogPath
		{
			get
			{
				if (NotificationBrokerClientLogConfiguration.defaultLogPath == null)
				{
					NotificationBrokerClientLogConfiguration.defaultLogPath = Path.Combine(ExchangeSetupContext.LoggingPath, "NotificationBroker\\Client");
				}
				return NotificationBrokerClientLogConfiguration.defaultLogPath;
			}
		}

		public bool IsLoggingEnabled { get; private set; }

		public bool IsActivityEventHandler
		{
			get
			{
				return true;
			}
		}

		public string LogPath { get; private set; }

		public TimeSpan MaxLogAge { get; private set; }

		public long MaxLogDirectorySizeInBytes { get; private set; }

		public long MaxLogFileSizeInBytes { get; private set; }

		public string LogComponent
		{
			get
			{
				return "NotificationBrokerClientLog";
			}
		}

		public string LogPrefix
		{
			get
			{
				return NotificationBrokerClientLogConfiguration.LogPrefixValue;
			}
		}

		public string LogType
		{
			get
			{
				return "Notification Broker Client Log";
			}
		}

		private const string LogTypeValue = "Notification Broker Client Log";

		private const string LogComponentValue = "NotificationBrokerClientLog";

		private const string DefaultRelativeFilePath = "NotificationBroker\\Client";

		public static readonly string LogPrefixValue = "NotificationBrokerClient";

		private static string defaultLogPath;
	}
}
