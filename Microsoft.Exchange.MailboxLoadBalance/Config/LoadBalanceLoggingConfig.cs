using System;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Config
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceLoggingConfig : ObjectLogConfiguration
	{
		public LoadBalanceLoggingConfig(string logName)
		{
			AnchorUtil.ThrowOnNullOrEmptyArgument(logName, "logName");
			this.LogName = logName;
		}

		public string LogName { get; private set; }

		public override bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		public override string LoggingFolder
		{
			get
			{
				string text = LoadBalanceADSettings.Instance.Value.LogFilePath;
				if (string.IsNullOrWhiteSpace(text))
				{
					text = Path.Combine(ExchangeSetupContext.InstallPath ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), string.Format("logging\\{0}Logs", "MailboxLoadBalance"));
				}
				return Path.Combine(text, this.LogName);
			}
		}

		public override string LogComponentName
		{
			get
			{
				return "MLB";
			}
		}

		public override string FilenamePrefix
		{
			get
			{
				return this.LogName;
			}
		}

		public override long MaxLogDirSize
		{
			get
			{
				return LoadBalanceADSettings.Instance.Value.LogMaxDirectorySize;
			}
		}

		public override long MaxLogFileSize
		{
			get
			{
				return LoadBalanceADSettings.Instance.Value.LogMaxFileSize;
			}
		}

		internal const string DefaultLogComponentName = "MLB";
	}
}
