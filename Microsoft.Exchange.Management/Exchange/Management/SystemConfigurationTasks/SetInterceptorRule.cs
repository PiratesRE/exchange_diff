using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Transport.Agent.InterceptorAgent;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "InterceptorRule", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetInterceptorRule : SetSystemConfigurationObjectTask<InterceptorRuleIdParameter, InterceptorAgentRule, InterceptorRule>
	{
		public SetInterceptorRule()
		{
			base.Fields.ResetChangeTracking();
		}

		[Parameter(Mandatory = false)]
		public InterceptorAgentRuleBehavior Action
		{
			get
			{
				return (InterceptorAgentRuleBehavior)base.Fields["Action"];
			}
			set
			{
				base.Fields["Action"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public InterceptorAgentEvent Event
		{
			get
			{
				return (InterceptorAgentEvent)base.Fields["Event"];
			}
			set
			{
				base.Fields["Event"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string Condition
		{
			get
			{
				return (string)base.Fields["Condition"];
			}
			set
			{
				base.Fields["Condition"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string Description
		{
			get
			{
				return (string)base.Fields["Description"];
			}
			set
			{
				base.Fields["Description"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public MultiValuedProperty<ServerIdParameter> Server
		{
			get
			{
				return (MultiValuedProperty<ServerIdParameter>)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public MultiValuedProperty<DatabaseAvailabilityGroupIdParameter> Dag
		{
			get
			{
				return (MultiValuedProperty<DatabaseAvailabilityGroupIdParameter>)base.Fields["Dag"];
			}
			set
			{
				base.Fields["Dag"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public MultiValuedProperty<AdSiteIdParameter> Site
		{
			get
			{
				return (MultiValuedProperty<AdSiteIdParameter>)base.Fields["Site"];
			}
			set
			{
				base.Fields["Site"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public EnhancedTimeSpan TimeInterval
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["TimeInterval"];
			}
			set
			{
				base.Fields["TimeInterval"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public string CustomResponseString
		{
			get
			{
				return (string)base.Fields["CustomResponseText"];
			}
			set
			{
				base.Fields["CustomResponseText"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string CustomResponseCode
		{
			get
			{
				return (string)base.Fields["CustomResponseCode"];
			}
			set
			{
				base.Fields["CustomResponseCode"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public DateTime ExpireTime
		{
			get
			{
				return (DateTime)base.Fields["ExpireTime"];
			}
			set
			{
				base.Fields["ExpireTime"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public string Path
		{
			get
			{
				return (string)base.Fields["Path"];
			}
			set
			{
				base.Fields["Path"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public SourceType Source
		{
			get
			{
				if (base.Fields.Contains("Source"))
				{
					return (SourceType)base.Fields["Source"];
				}
				return SourceType.User;
			}
			set
			{
				base.Fields["Source"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string CreatedBy
		{
			get
			{
				if (base.Fields.Contains("CreatedBy"))
				{
					return (string)base.Fields["CreatedBy"];
				}
				return InterceptorAgentRule.DefaultUser;
			}
			set
			{
				base.Fields["CreatedBy"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetInterceptorRule(this.Identity.ToString());
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return base.RootOrgContainerId.GetDescendantId(InterceptorRule.InterceptorRulesContainer);
			}
		}

		protected override void InternalValidate()
		{
			bool flag = base.Fields.IsModified("Event");
			bool flag2 = base.Fields.IsModified("Action");
			bool flag3 = base.Fields.IsModified("Condition");
			bool flag4 = base.Fields.IsModified("CustomResponseCode");
			bool flag5 = base.Fields.IsModified("CustomResponseText");
			bool flag6 = base.Fields.IsModified("Description");
			bool flag7 = base.Fields.IsModified("TimeInterval");
			bool flag8 = base.Fields.IsModified("Path");
			this.xmlNeedsUpdating = (flag2 || flag || flag3 || flag4 || flag5 || flag7 || flag6 || flag8 || base.Fields.IsModified("Identity"));
			if (base.Fields.IsModified("ExpireTime") && DateTime.UtcNow > this.ExpireTime.ToUniversalTime())
			{
				base.WriteError(new LocalizedException(Strings.InterceptorErrorExpireTimePassed(this.ExpireTime.ToString("G"))), ErrorCategory.InvalidData, "ExpireTime");
			}
			this.DataObject = (InterceptorRule)this.ResolveDataObject();
			if (this.xmlNeedsUpdating)
			{
				try
				{
					this.rule = InterceptorAgentRule.CreateRuleFromXml(this.DataObject.Xml);
					this.rule.SetPropertiesFromAdObjet(this.DataObject);
				}
				catch (FormatException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidData, null);
					TaskLogger.LogExit();
					return;
				}
				catch (InvalidOperationException exception2)
				{
					base.WriteError(exception2, ErrorCategory.InvalidData, null);
					TaskLogger.LogExit();
					return;
				}
			}
			if (this.rule.RuleVersion > InterceptorAgentRule.Version)
			{
				base.WriteError(new LocalizedException(Strings.InterceptorErrorModifyingNewerVersion(this.rule.RuleVersion.ToString())), ErrorCategory.InvalidOperation, null);
			}
			if (flag2 || flag)
			{
				InterceptorAgentRuleBehavior action = flag2 ? this.Action : this.rule.Action.Action;
				InterceptorAgentEvent interceptorAgentEvent = flag ? this.Event : this.rule.Events;
				LocalizedString localizedString;
				if (!InterceptorHelper.ValidateEventActionPairs(interceptorAgentEvent, action, out localizedString))
				{
					base.WriteError(new LocalizedException(localizedString), ErrorCategory.InvalidArgument, this.Condition);
				}
				this.rule.Events = interceptorAgentEvent;
				string customResponseCode;
				if (flag2 && !flag4 && InterceptorHelper.TryGetStatusCodeForModifiedRejectAction(this.Action, this.rule.Action.Action, this.rule.Action.Response.StatusCode, out customResponseCode))
				{
					this.CustomResponseCode = customResponseCode;
					flag4 = base.Fields.IsModified("CustomResponseCode");
				}
			}
			if (flag3)
			{
				LocalizedString localizedString;
				List<InterceptorAgentCondition> conditions;
				if (!InterceptorHelper.TryCreateConditions(this.Condition, out conditions, out localizedString))
				{
					base.WriteError(new LocalizedException(localizedString), ErrorCategory.InvalidData, this.Condition);
					return;
				}
				InterceptorAgentEvent evt = flag ? this.Event : this.rule.Events;
				if (!InterceptorHelper.ValidateEventConditionPairs(evt, conditions, out localizedString))
				{
					base.WriteError(new LocalizedException(localizedString), ErrorCategory.InvalidArgument, this.Condition);
				}
				this.rule.Conditions = conditions;
			}
			if (flag)
			{
				LocalizedString localizedString;
				if (!flag3 && !InterceptorHelper.ValidateEventConditionPairs(this.Event, this.rule.Conditions, out localizedString))
				{
					base.WriteError(new LocalizedException(localizedString), ErrorCategory.InvalidArgument, this.Condition);
				}
				this.rule.Events = this.Event;
			}
			if (flag6)
			{
				this.rule.Description = this.Description;
			}
			this.SetAction(flag4, flag5, flag2, flag8, flag7);
			this.ResolveTargets();
		}

		protected override void InternalProcessRecord()
		{
			this.rule.Name = this.Identity.ToString();
			if (base.Fields.IsModified("Action") && this.Action == InterceptorAgentRuleBehavior.Delay && !base.ShouldContinue(Strings.InterceptorConfirmDelayAction))
			{
				TaskLogger.LogExit();
				return;
			}
			bool flag = false;
			if ((base.Fields.IsModified("Dag") || base.Fields.IsModified("Site") || base.Fields.IsModified("Server")) && this.dags.Count == 0 && this.servers.Count == 0 && this.sites.Count == 0)
			{
				if (!base.ShouldContinue(Strings.InterceptorConfirmEntireForestRule(this.Identity.ToString())))
				{
					TaskLogger.LogExit();
					return;
				}
				flag = true;
			}
			if (InterceptorAgentRule.Version > this.rule.RuleVersion && !base.ShouldContinue(Strings.InterceptorConfirmModifyingOlderVersion(this.rule.RuleVersion.ToString(), InterceptorAgentRule.Version.ToString())))
			{
				TaskLogger.LogExit();
				return;
			}
			this.DataObject.Version = InterceptorAgentRule.Version.ToString();
			if (this.xmlNeedsUpdating)
			{
				this.DataObject.Xml = this.rule.ToXmlString();
			}
			if (base.Fields.IsModified("ExpireTime"))
			{
				this.DataObject.ExpireTimeUtc = this.ExpireTime.ToUniversalTime();
			}
			if (this.dags.Count > 0 || this.servers.Count > 0 || this.sites.Count > 0)
			{
				this.DataObject.Target = new MultiValuedProperty<ADObjectId>(this.servers.ConvertAll<ADObjectId>((Server s) => s.OriginalId).Concat(this.dags.ConvertAll<ADObjectId>((DatabaseAvailabilityGroup d) => d.OriginalId)).Concat(this.sites.ConvertAll<ADObjectId>((ADSite s) => s.OriginalId)));
			}
			else if (flag)
			{
				this.DataObject.Target = new MultiValuedProperty<ADObjectId>();
			}
			base.InternalProcessRecord();
		}

		private void ResolveTargets()
		{
			if (base.Fields.IsModified("Server") && this.Server != null)
			{
				foreach (ServerIdParameter serverIdParameter in this.Server)
				{
					Server item = (Server)base.GetDataObject<Server>(serverIdParameter, base.RootOrgGlobalConfigSession, null, new LocalizedString?(Strings.ErrorServerNotFound(serverIdParameter.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(serverIdParameter.ToString())));
					if (!this.servers.Contains(item))
					{
						this.servers.Add(item);
					}
				}
			}
			if (base.Fields.IsModified("Dag") && this.Dag != null)
			{
				foreach (DatabaseAvailabilityGroupIdParameter databaseAvailabilityGroupIdParameter in this.Dag)
				{
					DatabaseAvailabilityGroup item2 = (DatabaseAvailabilityGroup)base.GetDataObject<DatabaseAvailabilityGroup>(databaseAvailabilityGroupIdParameter, base.RootOrgGlobalConfigSession, null, new LocalizedString?(Strings.ErrorDagNotFound(databaseAvailabilityGroupIdParameter.ToString())), new LocalizedString?(Strings.ErrorDagNotUnique(databaseAvailabilityGroupIdParameter.ToString())));
					if (!this.dags.Contains(item2))
					{
						this.dags.Add(item2);
					}
				}
			}
			if (base.Fields.IsChanged("Site") && this.Site != null)
			{
				foreach (AdSiteIdParameter adSiteIdParameter in this.Site)
				{
					ADSite item3 = (ADSite)base.GetDataObject<ADSite>(adSiteIdParameter, base.RootOrgGlobalConfigSession, null, new LocalizedString?(Strings.ErrorSiteNotFound(adSiteIdParameter.ToString())), new LocalizedString?(Strings.ErrorSiteNotUnique(adSiteIdParameter.ToString())));
					if (!this.sites.Contains(item3))
					{
						this.sites.Add(item3);
					}
				}
			}
		}

		private void SetAction(bool responseCodeModified, bool responseTextModified, bool actionModified, bool pathModified, bool timeIntervalModified)
		{
			if (responseCodeModified || responseTextModified || actionModified || timeIntervalModified || pathModified)
			{
				string customResponseCode = this.CustomResponseCode;
				bool flag = false;
				if (!responseCodeModified && !SmtpResponse.Empty.Equals(this.rule.Action.Response))
				{
					customResponseCode = this.rule.Action.Response.StatusCode;
					flag = true;
				}
				string customResponseText = this.CustomResponseString;
				bool flag2 = false;
				if (!responseTextModified && !SmtpResponse.Empty.Equals(this.rule.Action.Response))
				{
					customResponseText = this.rule.Action.Response.StatusText[0];
					flag2 = true;
				}
				TimeSpan timeInterval = timeIntervalModified ? this.TimeInterval : this.rule.Action.Delay;
				InterceptorAgentAction interceptorAgentAction;
				LocalizedString warning;
				LocalizedString localizedString;
				if (!InterceptorHelper.TryCreateAction(actionModified ? this.Action : this.rule.Action.Action, customResponseCode, responseCodeModified || flag, customResponseText, responseTextModified || flag2, timeInterval, pathModified ? this.Path : this.rule.Action.Path, out interceptorAgentAction, out warning, out localizedString))
				{
					base.WriteError(new LocalizedException(localizedString), ErrorCategory.InvalidData, actionModified ? this.Action : this.rule.Action.Action);
				}
				this.WriteWarningAndReset(warning);
				if (actionModified && InterceptorAgentAction.IsArchivingBehavior(interceptorAgentAction.Action))
				{
					this.WriteWarning(InterceptorHelper.GetArchivedItemRetentionMessage(interceptorAgentAction.Action, this.Identity.ToString(), this.Path, 14));
				}
				this.rule.Action = interceptorAgentAction;
			}
		}

		private LocalizedString WriteWarningAndReset(LocalizedString warning)
		{
			if (warning != LocalizedString.Empty)
			{
				this.WriteWarning(warning);
			}
			return LocalizedString.Empty;
		}

		private bool xmlNeedsUpdating;

		private List<Server> servers = new List<Server>();

		private List<DatabaseAvailabilityGroup> dags = new List<DatabaseAvailabilityGroup>();

		private List<ADSite> sites = new List<ADSite>();

		private InterceptorAgentRule rule;
	}
}
