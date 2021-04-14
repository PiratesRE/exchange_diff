using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MailboxBaseConfigurationInfo : InstallableUnitConfigurationInfo
	{
		public string MdbName
		{
			get
			{
				return this.mdbName;
			}
			set
			{
				this.mdbName = value;
			}
		}

		public string DbFilePath
		{
			get
			{
				return this.dbFilePath;
			}
			set
			{
				this.dbFilePath = value;
			}
		}

		public string LogFolderPath
		{
			get
			{
				return this.logFolderPath;
			}
			set
			{
				this.logFolderPath = value;
			}
		}

		private string mdbName;

		private string dbFilePath;

		private string logFolderPath;
	}
}
