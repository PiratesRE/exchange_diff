using System;
using System.IO;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MsiConfigurationInfo
	{
		public static SetupContext SetupContext
		{
			get
			{
				return MsiConfigurationInfo.setupContext;
			}
			set
			{
				MsiConfigurationInfo.setupContext = value;
			}
		}

		public abstract string Name { get; }

		public abstract Guid ProductCode { get; }

		protected abstract string LogFileName { get; }

		public string LogFilePath
		{
			get
			{
				return Path.Combine(MsiConfigurationInfo.setupLogDirectory, this.LogFileName);
			}
		}

		private static SetupContext setupContext;

		private static readonly string setupLogDirectory = ConfigurationContext.Setup.SetupLoggingPath;
	}
}
