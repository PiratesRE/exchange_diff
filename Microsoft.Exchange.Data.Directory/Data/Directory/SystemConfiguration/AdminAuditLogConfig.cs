using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class AdminAuditLogConfig : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return AdminAuditLogConfig.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return AdminAuditLogConfig.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return AdminAuditLogConfig.parentPath;
			}
		}

		internal override bool IsShareable
		{
			get
			{
				return true;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AdminAuditLogEnabled
		{
			get
			{
				return (bool)this[AdminAuditLogConfigSchema.AdminAuditLogEnabled];
			}
			set
			{
				this[AdminAuditLogConfigSchema.AdminAuditLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AuditLogLevel LogLevel
		{
			get
			{
				if (this.CaptureDetailsEnabled)
				{
					return AuditLogLevel.Verbose;
				}
				return AuditLogLevel.None;
			}
			set
			{
				if (value == AuditLogLevel.None)
				{
					this.CaptureDetailsEnabled = false;
					return;
				}
				this.CaptureDetailsEnabled = true;
			}
		}

		internal bool CaptureDetailsEnabled
		{
			get
			{
				return (bool)this[AdminAuditLogConfigSchema.CaptureDetailsEnabled];
			}
			set
			{
				this[AdminAuditLogConfigSchema.CaptureDetailsEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TestCmdletLoggingEnabled
		{
			get
			{
				return (bool)this[AdminAuditLogConfigSchema.TestCmdletLoggingEnabled];
			}
			set
			{
				this[AdminAuditLogConfigSchema.TestCmdletLoggingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> AdminAuditLogCmdlets
		{
			get
			{
				return (MultiValuedProperty<string>)this[AdminAuditLogConfigSchema.AdminAuditLogCmdlets];
			}
			set
			{
				this[AdminAuditLogConfigSchema.AdminAuditLogCmdlets] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> AdminAuditLogParameters
		{
			get
			{
				return (MultiValuedProperty<string>)this[AdminAuditLogConfigSchema.AdminAuditLogParameters];
			}
			set
			{
				this[AdminAuditLogConfigSchema.AdminAuditLogParameters] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> AdminAuditLogExcludedCmdlets
		{
			get
			{
				return (MultiValuedProperty<string>)this[AdminAuditLogConfigSchema.AdminAuditLogExcludedCmdlets];
			}
			set
			{
				this[AdminAuditLogConfigSchema.AdminAuditLogExcludedCmdlets] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan AdminAuditLogAgeLimit
		{
			get
			{
				return (EnhancedTimeSpan)this[AdminAuditLogConfigSchema.AdminAuditLogAgeLimit];
			}
			set
			{
				this[AdminAuditLogConfigSchema.AdminAuditLogAgeLimit] = value;
			}
		}

		internal SmtpAddress AdminAuditLogMailbox
		{
			get
			{
				return (SmtpAddress)this[AdminAuditLogConfigSchema.AdminAuditLogMailbox];
			}
			set
			{
				this[AdminAuditLogConfigSchema.AdminAuditLogMailbox] = value;
			}
		}

		internal static ADObjectId GetWellKnownParentLocation(ADObjectId orgContainerId)
		{
			ADObjectId relativePath = AdminAuditLogConfig.parentPath;
			return orgContainerId.GetDescendantId(relativePath);
		}

		internal static bool GetValueFromFlags(IPropertyBag propertyBag, AdminAuditLogFlags flag)
		{
			object obj = propertyBag[AdminAuditLogConfigSchema.AdminLogFlags];
			return flag == ((AdminAuditLogFlags)obj & flag);
		}

		internal static void SetFlags(IPropertyBag propertyBag, AdminAuditLogFlags flag, bool value)
		{
			AdminAuditLogFlags adminAuditLogFlags = (AdminAuditLogFlags)propertyBag[AdminAuditLogConfigSchema.AdminLogFlags];
			AdminAuditLogFlags adminAuditLogFlags2 = value ? (adminAuditLogFlags | flag) : (adminAuditLogFlags & ~flag);
			propertyBag[AdminAuditLogConfigSchema.AdminLogFlags] = adminAuditLogFlags2;
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static AdminAuditLogConfigSchema schema = ObjectSchema.GetInstance<AdminAuditLogConfigSchema>();

		private static string mostDerivedClass = "msExchAdminAuditLogConfig";

		private static ADObjectId parentPath = new ADObjectId("CN=Global Settings");
	}
}
