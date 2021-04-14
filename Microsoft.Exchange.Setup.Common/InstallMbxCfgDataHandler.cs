using System;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InstallMbxCfgDataHandler : InstallRoleBaseDataHandler
	{
		public InstallMbxCfgDataHandler(ISetupContext context, MonadConnection connection) : base(context, "MailboxRole", "Install-MailboxRole", connection)
		{
			this.mailboxRoleConfigurationInfo = (MailboxRoleConfigurationInfo)base.InstallableUnitConfigurationInfo;
			if (context.ParsedArguments.ContainsKey("mdbname"))
			{
				this.MdbName = (string)context.ParsedArguments["mdbname"];
			}
			if (context.ParsedArguments.ContainsKey("dbfilepath"))
			{
				this.DbFilePath = ((EdbFilePath)context.ParsedArguments["dbfilepath"]).PathName;
			}
			if (context.ParsedArguments.ContainsKey("logfolderpath"))
			{
				this.LogFolderPath = ((NonRootLocalLongFullPath)context.ParsedArguments["logfolderpath"]).PathName;
			}
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			base.Parameters.AddWithValue("MdbName", this.MdbName);
			base.Parameters.AddWithValue("DbFilePath", this.DbFilePath);
			base.Parameters.AddWithValue("LogFolderPath", this.LogFolderPath);
			SetupLogger.TraceExit();
		}

		public string MdbName
		{
			get
			{
				return this.mailboxRoleConfigurationInfo.MdbName;
			}
			set
			{
				this.mailboxRoleConfigurationInfo.MdbName = value;
			}
		}

		public string DbFilePath
		{
			get
			{
				return this.mailboxRoleConfigurationInfo.DbFilePath;
			}
			set
			{
				this.mailboxRoleConfigurationInfo.DbFilePath = value;
			}
		}

		public string LogFolderPath
		{
			get
			{
				return this.mailboxRoleConfigurationInfo.LogFolderPath;
			}
			set
			{
				this.mailboxRoleConfigurationInfo.LogFolderPath = value;
			}
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
			base.UpdatePreCheckTaskDataHandler();
			PrerequisiteAnalysisTaskDataHandler.GetInstance(base.SetupContext, base.Connection);
		}

		private MailboxRoleConfigurationInfo mailboxRoleConfigurationInfo;
	}
}
