using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.RightsManagement;
using Microsoft.Exchange.Management.RightsManagement;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Management.OutlookProtectionRules
{
	[Cmdlet("New", "OutlookProtectionRule", SupportsShouldProcess = true)]
	public sealed class NewOutlookProtectionRule : NewMultitenancySystemConfigurationObjectTask<TransportRule>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewOutlookProtectionRule(base.Name);
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true)]
		[ValidateNotNullOrEmpty]
		public RmsTemplateIdParameter ApplyRightsProtectionTemplate
		{
			get
			{
				return (RmsTemplateIdParameter)base.Fields["ApplyRightsProtectionTemplate"];
			}
			set
			{
				base.Fields["ApplyRightsProtectionTemplate"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return !this.IsParameterSpecified("Enabled") || (bool)base.Fields["Enabled"];
			}
			set
			{
				base.Fields["Enabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return this.force;
			}
			set
			{
				this.force = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public string[] FromDepartment
		{
			get
			{
				return (string[])base.Fields["FromDepartment"];
			}
			set
			{
				base.Fields["FromDepartment"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateRange(0, 2147483647)]
		public int Priority
		{
			get
			{
				if (this.IsParameterSpecified("Priority"))
				{
					return (int)base.Fields["Priority"];
				}
				return 0;
			}
			set
			{
				base.Fields["Priority"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public RecipientIdParameter[] SentTo
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["SentTo"];
			}
			set
			{
				base.Fields["SentTo"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ToUserScope SentToScope
		{
			get
			{
				if (this.IsParameterSpecified("SentToScope"))
				{
					return (ToUserScope)base.Fields["SentToScope"];
				}
				return ToUserScope.All;
			}
			set
			{
				base.Fields["SentToScope"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool UserCanOverride
		{
			get
			{
				return !this.IsParameterSpecified("UserCanOverride") || (bool)base.Fields["UserCanOverride"];
			}
			set
			{
				base.Fields["UserCanOverride"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigDataProvider configDataProvider = base.CreateSession();
			this.rmsTemplateDataProvider = new RmsTemplateDataProvider((IConfigurationSession)configDataProvider, RmsTemplateType.Distributed, true);
			this.priorityHelper = new PriorityHelper(configDataProvider);
			return configDataProvider;
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			TransportRule transportRule = (TransportRule)base.PrepareDataObject();
			transportRule.SetId(Utils.GetRuleId(base.DataSession, base.Name));
			transportRule.OrganizationId = this.ResolveCurrentOrganization();
			transportRule.Priority = this.GetSequenceNumberForPriority(this.Priority);
			transportRule.Xml = new OutlookProtectionRulePresentationObject(transportRule)
			{
				ApplyRightsProtectionTemplate = this.ResolveTemplate(),
				Enabled = this.Enabled,
				FromDepartment = this.FromDepartment,
				SentTo = this.ResolveSentToRecipients(),
				SentToScope = this.SentToScope,
				UserCanOverride = this.UserCanOverride
			}.Serialize();
			TaskLogger.LogExit();
			return transportRule;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				if (this.RuleNameAlreadyInUse())
				{
					base.WriteError(new OutlookProtectionRuleNameIsNotUniqueException(base.Name), (ErrorCategory)1000, this.DataObject);
				}
				if (!this.IsPriorityValid(this.Priority))
				{
					base.WriteError(new OutlookProtectionRuleInvalidPriorityException(), (ErrorCategory)1000, this.DataObject);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (Utils.IsEmptyCondition(this.DataObject) && !this.Force && !base.ShouldContinue(Strings.ConfirmationMessageOutlookProtectionRuleWithEmptyCondition(base.Name)))
			{
				return;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable result)
		{
			TaskLogger.LogEnter();
			TransportRule transportRule = (TransportRule)result;
			base.WriteResult(new OutlookProtectionRulePresentationObject(transportRule)
			{
				Priority = this.Priority
			});
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(ParserException).IsInstanceOfType(exception) || RmsUtil.IsKnownException(exception);
		}

		private bool IsParameterSpecified(string parameterName)
		{
			return base.Fields.IsModified(parameterName);
		}

		private int GetSequenceNumberForPriority(int priority)
		{
			return this.priorityHelper.GetSequenceNumberToInsertPriority(priority);
		}

		private int GetPriorityFromSequenceNumber(int sequenceNumber)
		{
			return this.priorityHelper.GetPriorityFromSequenceNumber(sequenceNumber);
		}

		private bool IsPriorityValid(int priority)
		{
			return this.priorityHelper.IsPriorityValidForInsertion(priority);
		}

		private bool RuleNameAlreadyInUse()
		{
			return base.DataSession.Read<TransportRule>(Utils.GetRuleId(base.DataSession, base.Name)) != null;
		}

		private RmsTemplateIdentity ResolveTemplate()
		{
			string name = (this.ApplyRightsProtectionTemplate != null) ? this.ApplyRightsProtectionTemplate.ToString() : string.Empty;
			RmsTemplatePresentation rmsTemplatePresentation = (RmsTemplatePresentation)base.GetDataObject<RmsTemplatePresentation>(this.ApplyRightsProtectionTemplate, this.rmsTemplateDataProvider, null, new LocalizedString?(Strings.OutlookProtectionRuleRmsTemplateNotFound(name)), new LocalizedString?(Strings.OutlookProtectionRuleRmsTemplateNotUnique(name)));
			return (RmsTemplateIdentity)rmsTemplatePresentation.Identity;
		}

		private SmtpAddress[] ResolveSentToRecipients()
		{
			LocalizedException exception;
			IEnumerable<SmtpAddress> enumerable = Utils.ResolveRecipientIdParameters(base.TenantGlobalCatalogSession, this.SentTo, out exception);
			if (enumerable == null)
			{
				base.WriteError(exception, (ErrorCategory)1000, this.DataObject);
			}
			return enumerable.ToArray<SmtpAddress>();
		}

		private RmsTemplateDataProvider rmsTemplateDataProvider;

		private PriorityHelper priorityHelper;

		private SwitchParameter force;

		private static class DefaultParameterValues
		{
			public const bool Enabled = true;

			public const ToUserScope SentToScope = ToUserScope.All;

			public const bool UserCanOverride = true;

			public const int Priority = 0;
		}
	}
}
