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
	[Cmdlet("Set", "FrontendTransportService", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetFrontendTransportService : SetSystemConfigurationObjectTask<FrontendTransportServerIdParameter, FrontendTransportServerPresentationObject, FrontendTransportServer>
	{
		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan AgentLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["AgentLogMaxAge"];
			}
			set
			{
				base.Fields["AgentLogMaxAge"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> AgentLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["AgentLogMaxDirectorySize"];
			}
			set
			{
				base.Fields["AgentLogMaxDirectorySize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> AgentLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["AgentLogMaxFileSize"];
			}
			set
			{
				base.Fields["AgentLogMaxFileSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath AgentLogPath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["AgentLogPath"];
			}
			set
			{
				base.Fields["AgentLogPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AgentLogEnabled
		{
			get
			{
				return (bool)base.Fields["AgentLogEnabled"];
			}
			set
			{
				base.Fields["AgentLogEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan DnsLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["DnsLogMaxAge"];
			}
			set
			{
				base.Fields["DnsLogMaxAge"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> DnsLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["DnsLogMaxDirectorySize"];
			}
			set
			{
				base.Fields["DnsLogMaxDirectorySize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> DnsLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["DnsLogMaxFileSize"];
			}
			set
			{
				base.Fields["DnsLogMaxFileSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath DnsLogPath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["DnsLogPath"];
			}
			set
			{
				base.Fields["DnsLogPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DnsLogEnabled
		{
			get
			{
				return (bool)base.Fields["DnsLogEnabled"];
			}
			set
			{
				base.Fields["DnsLogEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ResourceLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["ResourceLogMaxAge"];
			}
			set
			{
				base.Fields["ResourceLogMaxAge"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ResourceLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["ResourceLogMaxDirectorySize"];
			}
			set
			{
				base.Fields["ResourceLogMaxDirectorySize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> ResourceLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["ResourceLogMaxFileSize"];
			}
			set
			{
				base.Fields["ResourceLogMaxFileSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath ResourceLogPath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["ResourceLogPath"];
			}
			set
			{
				base.Fields["ResourceLogPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ResourceLogEnabled
		{
			get
			{
				return (bool)base.Fields["ResourceLogEnabled"];
			}
			set
			{
				base.Fields["ResourceLogEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan AttributionLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["AttributionLogMaxAge"];
			}
			set
			{
				base.Fields["AttributionLogMaxAge"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> AttributionLogMaxDirectorySize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["AttributionLogMaxDirectorySize"];
			}
			set
			{
				base.Fields["AttributionLogMaxDirectorySize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> AttributionLogMaxFileSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)base.Fields["AttributionLogMaxFileSize"];
			}
			set
			{
				base.Fields["AttributionLogMaxFileSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath AttributionLogPath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["AttributionLogPath"];
			}
			set
			{
				base.Fields["AttributionLogPath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AttributionLogEnabled
		{
			get
			{
				return (bool)base.Fields["AttributionLogEnabled"];
			}
			set
			{
				base.Fields["AttributionLogEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxReceiveTlsRatePerMinute
		{
			get
			{
				return (int)base.Fields["MaxReceiveTlsRatePerMinute"];
			}
			set
			{
				base.Fields["MaxReceiveTlsRatePerMinute"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetFrontendTransportServer(this.Identity.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (base.Fields.IsModified("AgentLogMaxAge"))
			{
				this.DataObject.AgentLogMaxAge = this.AgentLogMaxAge;
			}
			if (base.Fields.IsModified("AgentLogMaxDirectorySize"))
			{
				this.DataObject.AgentLogMaxDirectorySize = this.AgentLogMaxDirectorySize;
			}
			if (base.Fields.IsModified("AgentLogMaxFileSize"))
			{
				this.DataObject.AgentLogMaxFileSize = this.AgentLogMaxFileSize;
			}
			if (base.Fields.IsModified("AgentLogPath"))
			{
				this.DataObject.AgentLogPath = this.AgentLogPath;
			}
			if (base.Fields.IsModified("AgentLogEnabled"))
			{
				this.DataObject.AgentLogEnabled = this.AgentLogEnabled;
			}
			if (base.Fields.IsModified("DnsLogMaxAge"))
			{
				this.DataObject.DnsLogMaxAge = this.DnsLogMaxAge;
			}
			if (base.Fields.IsModified("DnsLogMaxDirectorySize"))
			{
				this.DataObject.DnsLogMaxDirectorySize = this.DnsLogMaxDirectorySize;
			}
			if (base.Fields.IsModified("DnsLogMaxFileSize"))
			{
				this.DataObject.DnsLogMaxFileSize = this.DnsLogMaxFileSize;
			}
			if (base.Fields.IsModified("DnsLogPath"))
			{
				this.DataObject.DnsLogPath = this.DnsLogPath;
			}
			if (base.Fields.IsModified("DnsLogEnabled"))
			{
				this.DataObject.DnsLogEnabled = this.DnsLogEnabled;
			}
			if (base.Fields.IsModified("ResourceLogMaxAge"))
			{
				this.DataObject.ResourceLogMaxAge = this.ResourceLogMaxAge;
			}
			if (base.Fields.IsModified("ResourceLogMaxDirectorySize"))
			{
				this.DataObject.ResourceLogMaxDirectorySize = this.ResourceLogMaxDirectorySize;
			}
			if (base.Fields.IsModified("ResourceLogMaxFileSize"))
			{
				this.DataObject.ResourceLogMaxFileSize = this.ResourceLogMaxFileSize;
			}
			if (base.Fields.IsModified("ResourceLogPath"))
			{
				this.DataObject.ResourceLogPath = this.ResourceLogPath;
			}
			if (base.Fields.IsModified("ResourceLogEnabled"))
			{
				this.DataObject.ResourceLogEnabled = this.ResourceLogEnabled;
			}
			if (base.Fields.IsModified("AttributionLogMaxAge"))
			{
				this.DataObject.AttributionLogMaxAge = this.AttributionLogMaxAge;
			}
			if (base.Fields.IsModified("AttributionLogMaxDirectorySize"))
			{
				this.DataObject.AttributionLogMaxDirectorySize = this.AttributionLogMaxDirectorySize;
			}
			if (base.Fields.IsModified("AttributionLogMaxFileSize"))
			{
				this.DataObject.AttributionLogMaxFileSize = this.AttributionLogMaxFileSize;
			}
			if (base.Fields.IsModified("AttributionLogPath"))
			{
				this.DataObject.AttributionLogPath = this.AttributionLogPath;
			}
			if (base.Fields.IsModified("AttributionLogEnabled"))
			{
				this.DataObject.AttributionLogEnabled = this.AttributionLogEnabled;
			}
			if (base.Fields.IsModified("MaxReceiveTlsRatePerMinute"))
			{
				this.DataObject.MaxReceiveTlsRatePerMinute = this.MaxReceiveTlsRatePerMinute;
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
				this.Identity = FrontendTransportServerIdParameter.CreateIdentity(this.Identity);
			}
			base.InternalValidate();
			if (!this.DataObject.IsFrontendTransportServer)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorTaskRunningLocationFrontendOnly), ErrorCategory.InvalidOperation, null);
			}
			if (base.HasErrors)
			{
				return;
			}
			TaskLogger.LogExit();
		}

		private const string AgentLogMaxAgeKey = "AgentLogMaxAge";

		private const string AgentLogMaxDirectorySizeKey = "AgentLogMaxDirectorySize";

		private const string AgentLogMaxFileSizeKey = "AgentLogMaxFileSize";

		private const string AgentLogPathKey = "AgentLogPath";

		private const string AgentLogEnabledKey = "AgentLogEnabled";

		private const string DnsLogMaxAgeKey = "DnsLogMaxAge";

		private const string DnsLogMaxDirectorySizeKey = "DnsLogMaxDirectorySize";

		private const string DnsLogMaxFileSizeKey = "DnsLogMaxFileSize";

		private const string DnsLogPathKey = "DnsLogPath";

		private const string DnsLogEnabledKey = "DnsLogEnabled";

		private const string ResourceLogMaxAgeKey = "ResourceLogMaxAge";

		private const string ResourceLogMaxDirectorySizeKey = "ResourceLogMaxDirectorySize";

		private const string ResourceLogMaxFileSizeKey = "ResourceLogMaxFileSize";

		private const string ResourceLogPathKey = "ResourceLogPath";

		private const string ResourceLogEnabledKey = "ResourceLogEnabled";

		private const string AttributionLogMaxAgeKey = "AttributionLogMaxAge";

		private const string AttributionLogMaxDirectorySizeKey = "AttributionLogMaxDirectorySize";

		private const string AttributionLogMaxFileSizeKey = "AttributionLogMaxFileSize";

		private const string AttributionLogPathKey = "AttributionLogPath";

		private const string AttributionLogEnabledKey = "AttributionLogEnabled";

		private const string MaxReceiveTlsRatePerMinuteKey = "MaxReceiveTlsRatePerMinute";
	}
}
