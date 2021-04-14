using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class MsiTaskBase : Task
	{
		protected MsiTaskBase()
		{
			base.Fields["PropertyValues"] = string.Empty;
			base.Fields["LogMode"] = InstallLogMode.Error;
			base.Fields["LogFile"] = Path.Combine(ConfigurationContext.Setup.SetupLoggingPath, "msi.log");
			base.Fields["Activity"] = string.Empty;
			base.Fields["Canceled"] = false;
			base.Fields.ResetChangeTracking();
			this.uiHandlerObject = new MsiUIHandler();
			this.uiHandlerObject.OnProgress = new MsiUIHandler.ProgressHandler(this.UpdateProgress);
			this.uiHandlerObject.IsCanceled = new MsiUIHandler.IsCanceledHandler(this.IsCanceled);
			this.uiHandlerObject.OnMsiError = new MsiUIHandler.MsiErrorHandler(this.OnMsiError);
		}

		[Parameter(Mandatory = false)]
		public string PropertyValues
		{
			get
			{
				return (string)base.Fields["PropertyValues"];
			}
			set
			{
				base.Fields["PropertyValues"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LogFile
		{
			get
			{
				return (string)base.Fields["LogFile"];
			}
			set
			{
				base.Fields["LogFile"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public InstallLogMode LogMode
		{
			get
			{
				return (InstallLogMode)base.Fields["LogMode"];
			}
			set
			{
				base.Fields["LogMode"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] Features
		{
			get
			{
				return (string[])base.Fields["Features"];
			}
			set
			{
				base.Fields["Features"] = value;
			}
		}

		internal bool Canceled
		{
			get
			{
				return (bool)base.Fields["Canceled"];
			}
			set
			{
				base.Fields["Canceled"] = value;
			}
		}

		internal LocalizedString Activity
		{
			get
			{
				return (LocalizedString)base.Fields["Activity"];
			}
			set
			{
				base.Fields["Activity"] = value;
			}
		}

		internal MsiUIHandler UIHandler
		{
			get
			{
				return this.uiHandlerObject;
			}
		}

		internal string LastMsiError
		{
			get
			{
				return this.lastMsiError;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (!this.PropertyValues.Contains(" REBOOT="))
			{
				this.PropertyValues += " REBOOT=ReallySuppress";
			}
			TaskLogger.LogExit();
		}

		internal void SetLogging()
		{
			TaskLogger.LogEnter();
			if (this.LogMode != InstallLogMode.None)
			{
				MsiNativeMethods.EnableLog(this.LogMode, this.LogFile, InstallLogAttributes.Append);
			}
			TaskLogger.LogExit();
		}

		internal void UpdateProgress(int progress)
		{
			TaskLogger.LogEnter();
			ExProgressRecord exProgressRecord = new ExProgressRecord(0, this.Activity, Strings.MsiProgressStatus);
			exProgressRecord.RecordType = ProgressRecordType.Processing;
			exProgressRecord.PercentComplete = progress;
			if (!base.Stopping)
			{
				try
				{
					base.WriteProgress(exProgressRecord);
				}
				catch (PipelineStoppedException)
				{
					this.Canceled = true;
				}
			}
			TaskLogger.LogExit();
		}

		internal void OnMsiError(string errorMsg)
		{
			TaskLogger.LogEnter();
			this.lastMsiError = errorMsg;
			TaskLogger.LogExit();
		}

		internal bool IsCanceled()
		{
			return base.Stopping || this.Canceled;
		}

		private MsiUIHandler uiHandlerObject;

		private string lastMsiError;
	}
}
