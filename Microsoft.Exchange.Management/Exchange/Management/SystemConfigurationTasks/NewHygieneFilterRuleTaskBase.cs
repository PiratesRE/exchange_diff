using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class NewHygieneFilterRuleTaskBase : NewMultitenancyFixedNameSystemConfigurationObjectTask<TransportRule>
	{
		protected NewHygieneFilterRuleTaskBase(string ruleCollectionName)
		{
			this.ruleCollectionName = ruleCollectionName;
			this.Priority = 0;
			this.Enabled = true;
			base.Fields.ResetChangeTracking();
		}

		[Parameter(Mandatory = true, Position = 0)]
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

		[Parameter(Mandatory = false)]
		public int Priority
		{
			get
			{
				return (int)base.Fields["Priority"];
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException(Strings.NegativePriority);
				}
				base.Fields["Priority"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return (bool)base.Fields["Enabled"];
			}
			set
			{
				base.Fields["Enabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Comments
		{
			get
			{
				return (string)base.Fields["Comments"];
			}
			set
			{
				base.Fields["Comments"] = value;
			}
		}

		protected TransportRulePredicate[] Conditions
		{
			get
			{
				return (TransportRulePredicate[])base.Fields["Conditions"];
			}
			set
			{
				base.Fields["Conditions"] = value;
			}
		}

		protected TransportRulePredicate[] Exceptions
		{
			get
			{
				return (TransportRulePredicate[])base.Fields["Exceptions"];
			}
			set
			{
				base.Fields["Exceptions"] = value;
			}
		}

		protected override void InternalValidate()
		{
			Exception exception;
			string target;
			if (!Utils.ValidateParametersForRole(base.Fields, out exception, out target))
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, target);
			}
			if (!Utils.ValidateRecipientIdParameters(base.Fields, base.TenantGlobalCatalogSession, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>), out exception, out target))
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, target);
			}
			try
			{
				List<Type> list;
				List<Type> list2;
				TransportRulePredicate[] array;
				TransportRulePredicate[] array2;
				Utils.BuildConditionsAndExceptionsFromParameters(base.Fields, base.TenantGlobalCatalogSession, base.DataSession, false, out list, out list2, out array, out array2);
				if (array.Length > 0)
				{
					this.Conditions = array;
				}
				if (array2.Length > 0)
				{
					this.Exceptions = array2;
				}
			}
			catch (Exception exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, this.Name);
			}
			if (this.Conditions == null)
			{
				base.WriteError(new ArgumentException(Strings.ErrorCannotCreateRuleWithoutCondition), ErrorCategory.InvalidArgument, this.Name);
			}
		}

		protected readonly string ruleCollectionName;
	}
}
