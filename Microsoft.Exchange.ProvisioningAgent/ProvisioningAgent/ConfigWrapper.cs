using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProvisioningAgent;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal sealed class ConfigWrapper : ILifetimeTrackable
	{
		internal ConfigWrapper(IConfigurationPolicy configurationPolicy, LogMessageDelegate logMessage) : this(configurationPolicy, DateTime.UtcNow, logMessage)
		{
		}

		internal ConfigWrapper(IConfigurationPolicy configurationPolicy, DateTime createTime, LogMessageDelegate logMessage)
		{
			this.configurationPolicy = configurationPolicy;
			this.CreateTime = createTime;
			logMessage(Strings.EnteredInitializeConfig);
			if (ExTraceGlobals.AdminAuditLogTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.AdminAuditLogTracer.TraceDebug<ConfigWrapper>((long)this.GetHashCode(), "{0} Initializing adminAuditLogConfig.", this);
			}
			this.AdminAuditLogConfig = configurationPolicy.GetAdminAuditLogConfig();
			if (this.IsAuditConfigFromUCCPolicyEnabled())
			{
				AuditPolicyCacheEntry auditPolicyCacheEntry;
				AuditPolicyUtility.RetrieveAuditPolicy(configurationPolicy.OrganizationId, out auditPolicyCacheEntry);
				this.isAuditPolicyAvailable = (auditPolicyCacheEntry != null && auditPolicyCacheEntry.IsExist());
			}
			else
			{
				this.isAuditPolicyAvailable = false;
			}
			if (this.isAuditPolicyAvailable)
			{
				this.ParameterWildcardMatcher = new WildcardMatcher(ConfigWrapper.defaultAdminAuditLogParameters);
				this.CmdletWildcardMatcher = new WildcardMatcher(ConfigWrapper.defaultAdminAuditLogCmdlets);
				this.ExcludedCmdletWildcardMatcher = new WildcardMatcher(ConfigWrapper.defaultAdminAuditLogExcludedCmdlets);
			}
			else if (this.AdminAuditLogConfig != null)
			{
				this.ParameterWildcardMatcher = new WildcardMatcher(this.AdminAuditLogConfig.AdminAuditLogParameters);
				this.CmdletWildcardMatcher = new WildcardMatcher(this.AdminAuditLogConfig.AdminAuditLogCmdlets);
				this.ExcludedCmdletWildcardMatcher = new WildcardMatcher(this.AdminAuditLogConfig.AdminAuditLogExcludedCmdlets);
			}
			logMessage(Strings.ExitedInitializeConfig);
		}

		public IAdminAuditLogConfig AdminAuditLogConfig { get; private set; }

		public WildcardMatcher ParameterWildcardMatcher { get; private set; }

		public WildcardMatcher CmdletWildcardMatcher { get; private set; }

		public WildcardMatcher ExcludedCmdletWildcardMatcher { get; private set; }

		public bool LoggingEnabled
		{
			get
			{
				return this.isAuditPolicyAvailable || (this.AdminAuditLogConfig != null && this.AdminAuditLogConfig.AdminAuditLogEnabled && (this.ArbitrationMailboxStatus != ArbitrationMailboxStatus.R4 || this.AdminAuditLogConfig.IsValidAuditLogMailboxAddress));
			}
		}

		public bool LoggingCapable
		{
			get
			{
				return this.ArbitrationMailboxStatus != ArbitrationMailboxStatus.R4 || (this.AdminAuditLogConfig != null && this.AdminAuditLogConfig.IsValidAuditLogMailboxAddress);
			}
		}

		public ArbitrationMailboxStatus ArbitrationMailboxStatus
		{
			get
			{
				if (this.status == null)
				{
					this.status = new ArbitrationMailboxStatus?(this.configurationPolicy.CheckArbitrationMailboxStatus(out this.initialError));
				}
				return this.status.Value;
			}
		}

		public Exception Error
		{
			get
			{
				return this.initialError;
			}
		}

		public IAuditLog MailboxLogger
		{
			get
			{
				EnumValidator.ThrowIfInvalid<ArbitrationMailboxStatus>(this.ArbitrationMailboxStatus, ConfigWrapper.validStatusValues);
				if (this.auditLog == null)
				{
					this.auditLog = this.configurationPolicy.CreateLogger(this.ArbitrationMailboxStatus);
				}
				return this.auditLog;
			}
		}

		public DateTime CreateTime { get; private set; }

		public DateTime LastAccessTime { get; set; }

		public bool ShouldLogBasedOnCmdletName(string cmdletName)
		{
			if (this.configurationPolicy.RunningOnDataCenter)
			{
				if (Array.FindIndex<string>(ConfigWrapper.MRSInternalCmdlets, (string x) => string.Equals(x, cmdletName, StringComparison.OrdinalIgnoreCase)) != -1)
				{
					if (ExTraceGlobals.AdminAuditLogTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.AdminAuditLogTracer.TraceDebug<ConfigWrapper, string>((long)this.GetHashCode(), "{0} Cmdlet name '{1}' is one of mrs internal hidden cmdlets. Skip logging", this, cmdletName);
					}
					return false;
				}
			}
			if (this.ExcludedCmdletWildcardMatcher != null && this.ExcludedCmdletWildcardMatcher.IsMatch(cmdletName))
			{
				if (ExTraceGlobals.AdminAuditLogTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.AdminAuditLogTracer.TraceDebug<ConfigWrapper, string, WildcardMatcher>((long)this.GetHashCode(), "{0} Cmdlet name '{1}' matches the exclusion setting '{2}'. Skip logging", this, cmdletName, this.ExcludedCmdletWildcardMatcher);
				}
				return false;
			}
			if ((this.AdminAuditLogConfig == null || !this.AdminAuditLogConfig.TestCmdletLoggingEnabled) && ConfigWrapper.TestCmdletWildcardMatcher.IsMatch(cmdletName))
			{
				if (ExTraceGlobals.AdminAuditLogTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.AdminAuditLogTracer.TraceDebug<ConfigWrapper, string>((long)this.GetHashCode(), "{0} Cmdlet name '{1}' is a test cmdlet and TestCmdletLoggingEnabled is false. Skip logging", this, cmdletName);
				}
				return false;
			}
			if (this.CmdletWildcardMatcher == null || !this.CmdletWildcardMatcher.IsMatch(cmdletName))
			{
				if (ExTraceGlobals.AdminAuditLogTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.AdminAuditLogTracer.TraceDebug<ConfigWrapper, string, WildcardMatcher>((long)this.GetHashCode(), "{0} Cmdlet name '{1}' does not match settings '{2}'. Skip logging.", this, cmdletName, this.CmdletWildcardMatcher);
				}
				return false;
			}
			if (ExTraceGlobals.AdminAuditLogTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.AdminAuditLogTracer.TraceDebug<ConfigWrapper, string>((long)this.GetHashCode(), "{0} Cmdlet name '{1}' matches settings.", this, cmdletName);
			}
			return true;
		}

		public bool CmdletAlwaysLogged(string cmdletName)
		{
			bool flag = string.Equals(cmdletName, "Set-AdminAuditLogConfig", StringComparison.OrdinalIgnoreCase);
			return flag && this.LoggingCapable;
		}

		internal void ResetLogMailboxStatus()
		{
			this.status = null;
			this.initialError = null;
		}

		private bool IsAuditConfigFromUCCPolicyEnabled()
		{
			Exception ex;
			this.configurationPolicy.CheckArbitrationMailboxStatus(out ex);
			return AuditFeatureManager.IsAuditConfigFromUCCPolicyEnabled(null, this.configurationPolicy.ExchangePrincipal);
		}

		private static ArbitrationMailboxStatus[] validStatusValues = new ArbitrationMailboxStatus[]
		{
			ArbitrationMailboxStatus.R4,
			ArbitrationMailboxStatus.R5,
			ArbitrationMailboxStatus.E15,
			ArbitrationMailboxStatus.FFO
		};

		private static readonly MultiValuedProperty<string> defaultAdminAuditLogCmdlets = new MultiValuedProperty<string>("*");

		private static readonly MultiValuedProperty<string> defaultAdminAuditLogParameters = new MultiValuedProperty<string>("*");

		private static readonly MultiValuedProperty<string> defaultAdminAuditLogExcludedCmdlets = new MultiValuedProperty<string>();

		private IConfigurationPolicy configurationPolicy;

		private static readonly WildcardMatcher TestCmdletWildcardMatcher = new WildcardMatcher("Test-*");

		private static readonly string[] MRSInternalCmdlets = new string[]
		{
			"Update-MovedMailbox"
		};

		private IAuditLog auditLog;

		private ArbitrationMailboxStatus? status;

		private Exception initialError;

		private readonly bool isAuditPolicyAvailable;
	}
}
