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
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Transport.Agent.InterceptorAgent;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "InterceptorRule", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class NewInterceptorRule : NewFixedNameSystemConfigurationObjectTask<InterceptorRule>
	{
		public NewInterceptorRule()
		{
			base.Fields.ResetChangeTracking();
		}

		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNullOrEmpty]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		[Parameter(Mandatory = true)]
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

		[Parameter(Mandatory = true)]
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

		[Parameter(Mandatory = true)]
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

		[Parameter(Mandatory = true)]
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

		[ValidateNotNullOrEmpty]
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
		[ValidateNotNullOrEmpty]
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
		[ValidateNotNullOrEmpty]
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
				return (EnhancedTimeSpan)(base.Fields["TimeInterval"] ?? NewInterceptorRule.DefaultTimeInterval);
			}
			set
			{
				base.Fields["TimeInterval"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
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

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
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

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
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
				return Strings.ConfirmationMessageNewInterceptorRule(this.Name);
			}
		}

		protected override bool SkipWriteResult
		{
			get
			{
				return true;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADObjectId descendantId = base.RootOrgContainerId.GetDescendantId(InterceptorRule.InterceptorRulesContainer);
			this.DataObject.Name = this.Name;
			InterceptorRule interceptorRule = (InterceptorRule)base.PrepareDataObject();
			interceptorRule.SetId(descendantId.GetChildId(this.DataObject.Name));
			return interceptorRule;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.Fields.IsModified("ExpireTime") && DateTime.UtcNow > this.ExpireTime.ToUniversalTime())
			{
				base.WriteError(new LocalizedException(Strings.InterceptorErrorExpireTimePassed(this.ExpireTime.ToString("G"))), ErrorCategory.InvalidData, "ExpireTime");
			}
			List<InterceptorAgentCondition> conditions;
			LocalizedString localizedString;
			if (!InterceptorHelper.TryCreateConditions(this.Condition, out conditions, out localizedString))
			{
				base.WriteError(new LocalizedException(localizedString), ErrorCategory.InvalidData, this.Condition);
				return;
			}
			if (this.Event == InterceptorAgentEvent.Invalid)
			{
				base.WriteError(new LocalizedException(Strings.InterceptorErrorEventInvalid), ErrorCategory.InvalidData, this.Event);
			}
			if (this.Action == InterceptorAgentRuleBehavior.NoOp)
			{
				base.WriteError(new LocalizedException(Strings.InterceptorErrorActionInvalid), ErrorCategory.InvalidData, this.Action);
			}
			if (!InterceptorHelper.ValidateEventConditionPairs(this.Event, conditions, out localizedString))
			{
				base.WriteError(new LocalizedException(localizedString), ErrorCategory.InvalidArgument, this.Condition);
			}
			if (!InterceptorHelper.ValidateEventActionPairs(this.Event, this.Action, out localizedString))
			{
				base.WriteError(new LocalizedException(localizedString), ErrorCategory.InvalidArgument, this.Condition);
			}
			InterceptorAgentAction interceptorAgentAction;
			LocalizedString warning;
			if (!InterceptorHelper.TryCreateAction(this.Action, this.CustomResponseCode, base.Fields.IsChanged("CustomResponseCode"), this.CustomResponseString, base.Fields.IsChanged("CustomResponseText"), this.TimeInterval, this.Path, out interceptorAgentAction, out warning, out localizedString))
			{
				base.WriteError(new LocalizedException(localizedString), ErrorCategory.InvalidData, this.Action);
			}
			this.WriteWarningAndReset(warning);
			if (InterceptorAgentAction.IsArchivingBehavior(interceptorAgentAction.Action))
			{
				this.WriteWarning(InterceptorHelper.GetArchivedItemRetentionMessage(interceptorAgentAction.Action, this.Name, this.Path, 14));
			}
			this.ResolveTargets();
			if (!base.HasErrors)
			{
				this.rule = new InterceptorAgentRule(this.Name, this.Description, conditions, interceptorAgentAction, this.Event, this.Source, this.CreatedBy);
			}
		}

		protected override void InternalProcessRecord()
		{
			if (this.Action == InterceptorAgentRuleBehavior.Delay && !base.ShouldProcess(Strings.InterceptorConfirmDelayAction))
			{
				TaskLogger.LogExit();
				return;
			}
			if (this.dags.Count == 0 && this.servers.Count == 0 && this.sites.Count == 0 && !base.ShouldProcess(Strings.InterceptorConfirmEntireForestRule(this.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			this.DataObject.Version = InterceptorAgentRule.Version.ToString();
			this.DataObject.Xml = this.rule.ToXmlString();
			if (this.dags.Count > 0 || this.servers.Count > 0 || this.sites.Count > 0)
			{
				this.DataObject.Target = new MultiValuedProperty<ADObjectId>(this.servers.ConvertAll<ADObjectId>((Server s) => s.OriginalId).Concat(this.dags.ConvertAll<ADObjectId>((DatabaseAvailabilityGroup d) => d.OriginalId)).Concat(this.sites.ConvertAll<ADObjectId>((ADSite s) => s.OriginalId)));
			}
			if (base.Fields.IsChanged("ExpireTime"))
			{
				this.DataObject.ExpireTimeUtc = this.ExpireTime.ToUniversalTime();
			}
			else
			{
				this.DataObject.ExpireTimeUtc = DateTime.UtcNow + NewInterceptorRule.DefaultExpireTime;
			}
			base.InternalProcessRecord();
			this.rule.SetPropertiesFromAdObjet(this.DataObject);
			this.WriteResult(this.rule);
		}

		private void ResolveTargets()
		{
			if (base.Fields.IsChanged("Server") && this.Server != null)
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
			if (base.Fields.IsChanged("Dag") && this.Dag != null)
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

		private LocalizedString WriteWarningAndReset(LocalizedString warning)
		{
			if (warning != LocalizedString.Empty)
			{
				this.WriteWarning(warning);
			}
			return LocalizedString.Empty;
		}

		internal static readonly TimeSpan DefaultExpireTime = TimeSpan.FromDays(7.0);

		private InterceptorAgentRule rule;

		private List<Server> servers = new List<Server>();

		private List<DatabaseAvailabilityGroup> dags = new List<DatabaseAvailabilityGroup>();

		private List<ADSite> sites = new List<ADSite>();

		private static readonly EnhancedTimeSpan DefaultTimeInterval = EnhancedTimeSpan.FromMinutes(2.0);
	}
}
