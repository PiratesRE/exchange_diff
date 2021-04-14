using System;
using System.IO;
using System.Web.Hosting;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcHttpModules
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class RpcHttpLogConfiguration : ILogConfiguration
	{
		public RpcHttpLogConfiguration()
		{
			this.IsLoggingEnabled = true;
			this.LogPath = RpcHttpLogConfiguration.DefaultLogPath;
			this.MaxLogAge = TimeSpan.FromDays(30.0);
			this.MaxLogDirectorySizeInBytes = 1073741824L;
			this.MaxLogFileSizeInBytes = 10485760L;
		}

		public static string DefaultLogPath
		{
			get
			{
				string path = string.Format("{0}\\W3SVC{1}", "Logging\\RpcHttp", HostingEnvironment.ApplicationHost.GetSiteID());
				return RpcHttpLogConfiguration.defaultLogPath = Path.Combine(ExchangeSetupContext.InstallPath, path);
			}
		}

		public bool IsLoggingEnabled { get; private set; }

		public bool IsActivityEventHandler
		{
			get
			{
				return false;
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
				return "RpcHttpLog";
			}
		}

		public string LogPrefix
		{
			get
			{
				return RpcHttpLogConfiguration.LogPrefixValue;
			}
		}

		public string LogType
		{
			get
			{
				return "RpcHttp Log";
			}
		}

		private const string LogTypeValue = "RpcHttp Log";

		private const string LogComponentValue = "RpcHttpLog";

		private const string DefaultRelativeFilePath = "Logging\\RpcHttp";

		public static readonly string LogPrefixValue = "RpcHttp";

		private static string defaultLogPath;
	}
}
