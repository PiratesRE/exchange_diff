using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "MailboxTransportService", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailboxTransportService : SetSystemConfigurationObjectTask<MailboxTransportServerIdParameter, MailboxTransportServerPresentationObject, MailboxTransportServer>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetMailboxTransportService(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MailboxSubmissionAgentLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["MailboxSubmissionAgentLogMaxAge"];
			}
			set
			{
				base.Fields["MailboxSubmissionAgentLogMaxAge"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> MailboxSubmissionAgentLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["MailboxSubmissionAgentLogMaxDirectorySize"];
			}
			set
			{
				base.Fields["MailboxSubmissionAgentLogMaxDirectorySize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> MailboxSubmissionAgentLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["MailboxSubmissionAgentLogMaxFileSize"];
			}
			set
			{
				base.Fields["MailboxSubmissionAgentLogMaxFileSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath MailboxSubmissionAgentLogPath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["MailboxSubmissionAgentLogPath"];
			}
			set
			{
				base.Fields["MailboxSubmissionAgentLogPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MailboxSubmissionAgentLogEnabled
		{
			get
			{
				return (bool)base.Fields["MailboxSubmissionAgentLogEnabled"];
			}
			set
			{
				base.Fields["MailboxSubmissionAgentLogEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MailboxDeliveryAgentLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["MailboxDeliveryAgentLogMaxAge"];
			}
			set
			{
				base.Fields["MailboxDeliveryAgentLogMaxAge"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> MailboxDeliveryAgentLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["MailboxDeliveryAgentLogMaxDirectorySize"];
			}
			set
			{
				base.Fields["MailboxDeliveryAgentLogMaxDirectorySize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> MailboxDeliveryAgentLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["MailboxDeliveryAgentLogMaxFileSize"];
			}
			set
			{
				base.Fields["MailboxDeliveryAgentLogMaxFileSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath MailboxDeliveryAgentLogPath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["MailboxDeliveryAgentLogPath"];
			}
			set
			{
				base.Fields["MailboxDeliveryAgentLogPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MailboxDeliveryAgentLogEnabled
		{
			get
			{
				return (bool)base.Fields["MailboxDeliveryAgentLogEnabled"];
			}
			set
			{
				base.Fields["MailboxDeliveryAgentLogEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MailboxDeliveryThrottlingLogEnabled
		{
			get
			{
				return (bool)base.Fields["MailboxDeliveryThrottlingLogEnabled"];
			}
			set
			{
				base.Fields["MailboxDeliveryThrottlingLogEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MailboxDeliveryThrottlingLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["MailboxDeliveryThrottlingLogMaxAge"];
			}
			set
			{
				base.Fields["MailboxDeliveryThrottlingLogMaxAge"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> MailboxDeliveryThrottlingLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["MailboxDeliveryThrottlingLogMaxDirectorySize"];
			}
			set
			{
				base.Fields["MailboxDeliveryThrottlingLogMaxDirectorySize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> MailboxDeliveryThrottlingLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["MailboxDeliveryThrottlingLogMaxFileSize"];
			}
			set
			{
				base.Fields["MailboxDeliveryThrottlingLogMaxFileSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath MailboxDeliveryThrottlingLogPath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["MailboxDeliveryThrottlingLogPath"];
			}
			set
			{
				base.Fields["MailboxDeliveryThrottlingLogPath"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (base.Fields.IsModified("MailboxSubmissionAgentLogMaxAge"))
			{
				this.DataObject.MailboxSubmissionAgentLogMaxAge = this.MailboxSubmissionAgentLogMaxAge;
			}
			if (base.Fields.IsModified("MailboxSubmissionAgentLogMaxDirectorySize"))
			{
				this.DataObject.MailboxSubmissionAgentLogMaxDirectorySize = this.MailboxSubmissionAgentLogMaxDirectorySize;
			}
			if (base.Fields.IsModified("MailboxSubmissionAgentLogMaxFileSize"))
			{
				this.DataObject.MailboxSubmissionAgentLogMaxFileSize = this.MailboxSubmissionAgentLogMaxFileSize;
			}
			if (base.Fields.IsModified("MailboxSubmissionAgentLogPath"))
			{
				this.DataObject.MailboxSubmissionAgentLogPath = this.MailboxSubmissionAgentLogPath;
			}
			if (base.Fields.IsModified("MailboxSubmissionAgentLogEnabled"))
			{
				this.DataObject.MailboxSubmissionAgentLogEnabled = this.MailboxSubmissionAgentLogEnabled;
			}
			if (base.Fields.IsModified("MailboxDeliveryAgentLogMaxAge"))
			{
				this.DataObject.MailboxDeliveryAgentLogMaxAge = this.MailboxDeliveryAgentLogMaxAge;
			}
			if (base.Fields.IsModified("MailboxDeliveryAgentLogMaxDirectorySize"))
			{
				this.DataObject.MailboxDeliveryAgentLogMaxDirectorySize = this.MailboxDeliveryAgentLogMaxDirectorySize;
			}
			if (base.Fields.IsModified("MailboxDeliveryAgentLogMaxFileSize"))
			{
				this.DataObject.MailboxDeliveryAgentLogMaxFileSize = this.MailboxDeliveryAgentLogMaxFileSize;
			}
			if (base.Fields.IsModified("MailboxDeliveryAgentLogPath"))
			{
				this.DataObject.MailboxDeliveryAgentLogPath = this.MailboxDeliveryAgentLogPath;
			}
			if (base.Fields.IsModified("MailboxDeliveryAgentLogEnabled"))
			{
				this.DataObject.MailboxDeliveryAgentLogEnabled = this.MailboxDeliveryAgentLogEnabled;
			}
			if (base.Fields.IsModified("MailboxDeliveryThrottlingLogEnabled"))
			{
				this.DataObject.MailboxDeliveryThrottlingLogEnabled = this.MailboxDeliveryThrottlingLogEnabled;
			}
			if (base.Fields.IsModified("MailboxDeliveryThrottlingLogMaxAge"))
			{
				this.DataObject.MailboxDeliveryThrottlingLogMaxAge = this.MailboxDeliveryThrottlingLogMaxAge;
			}
			if (base.Fields.IsModified("MailboxDeliveryThrottlingLogMaxDirectorySize"))
			{
				this.DataObject.MailboxDeliveryThrottlingLogMaxDirectorySize = this.MailboxDeliveryThrottlingLogMaxDirectorySize;
			}
			if (base.Fields.IsModified("MailboxDeliveryThrottlingLogMaxFileSize"))
			{
				this.DataObject.MailboxDeliveryThrottlingLogMaxFileSize = this.MailboxDeliveryThrottlingLogMaxFileSize;
			}
			if (base.Fields.IsModified("MailboxDeliveryThrottlingLogPath"))
			{
				this.DataObject.MailboxDeliveryThrottlingLogPath = this.MailboxDeliveryThrottlingLogPath;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (this.Instance.IsModified(ADObjectSchema.Name))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorServerNameModified), ErrorCategory.InvalidOperation, this.Identity);
			}
			if (this.Identity != null)
			{
				this.Identity = MailboxTransportServerIdParameter.CreateIdentity(this.Identity);
			}
			base.InternalValidate();
			if (!this.DataObject.IsMailboxServer)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorTaskRunningLocationMailboxOnly), ErrorCategory.InvalidOperation, null);
			}
			if (base.HasErrors)
			{
				return;
			}
			TaskLogger.LogExit();
		}

		private const string MailboxSubmissionAgentLogMaxAgeKey = "MailboxSubmissionAgentLogMaxAge";

		private const string MailboxSubmissionAgentLogMaxDirectorySizeKey = "MailboxSubmissionAgentLogMaxDirectorySize";

		private const string MailboxSubmissionAgentLogMaxFileSizeKey = "MailboxSubmissionAgentLogMaxFileSize";

		private const string MailboxSubmissionAgentLogPathKey = "MailboxSubmissionAgentLogPath";

		private const string MailboxSubmissionAgentLogEnabledKey = "MailboxSubmissionAgentLogEnabled";

		private const string MailboxDeliveryAgentLogMaxAgeKey = "MailboxDeliveryAgentLogMaxAge";

		private const string MailboxDeliveryAgentLogMaxDirectorySizeKey = "MailboxDeliveryAgentLogMaxDirectorySize";

		private const string MailboxDeliveryAgentLogMaxFileSizeKey = "MailboxDeliveryAgentLogMaxFileSize";

		private const string MailboxDeliveryAgentLogPathKey = "MailboxDeliveryAgentLogPath";

		private const string MailboxDeliveryAgentLogEnabledKey = "MailboxDeliveryAgentLogEnabled";

		private const string MailboxDeliveryThrottlingLogEnabledKey = "MailboxDeliveryThrottlingLogEnabled";

		private const string MailboxDeliveryThrottlingLogMaxAgeKey = "MailboxDeliveryThrottlingLogMaxAge";

		private const string MailboxDeliveryThrottlingLogMaxDirectorySizeKey = "MailboxDeliveryThrottlingLogMaxDirectorySize";

		private const string MailboxDeliveryThrottlingLogMaxFileSizeKey = "MailboxDeliveryThrottlingLogMaxFileSize";

		private const string MailboxDeliveryThrottlingLogPathKey = "MailboxDeliveryThrottlingLogPath";
	}
}
