using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Serializable]
	public sealed class UMCallAnsweringRule : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return UMCallAnsweringRule.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		[Parameter]
		public MultiValuedProperty<CallerIdItem> CallerIds
		{
			get
			{
				return (MultiValuedProperty<CallerIdItem>)this[UMCallAnsweringRuleSchema.CallerIds];
			}
			set
			{
				this[UMCallAnsweringRuleSchema.CallerIds] = value;
			}
		}

		[Parameter]
		public bool CallersCanInterruptGreeting
		{
			get
			{
				return (bool)this[UMCallAnsweringRuleSchema.CallersCanInterruptGreeting];
			}
			set
			{
				this[UMCallAnsweringRuleSchema.CallersCanInterruptGreeting] = value;
			}
		}

		[Parameter]
		public bool CheckAutomaticReplies
		{
			get
			{
				return (bool)this[UMCallAnsweringRuleSchema.CheckAutomaticReplies];
			}
			set
			{
				this[UMCallAnsweringRuleSchema.CheckAutomaticReplies] = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return (bool)this[UMCallAnsweringRuleSchema.Enabled];
			}
			internal set
			{
				this[UMCallAnsweringRuleSchema.Enabled] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ExtensionsDialed
		{
			get
			{
				return (MultiValuedProperty<string>)this[UMCallAnsweringRuleSchema.ExtensionsDialed];
			}
			set
			{
				this[UMCallAnsweringRuleSchema.ExtensionsDialed] = value;
			}
		}

		public bool InError { get; internal set; }

		[Parameter]
		public MultiValuedProperty<KeyMapping> KeyMappings
		{
			get
			{
				return (MultiValuedProperty<KeyMapping>)this[UMCallAnsweringRuleSchema.KeyMappings];
			}
			set
			{
				this[UMCallAnsweringRuleSchema.KeyMappings] = value;
			}
		}

		[Parameter]
		[ValidateNotNullOrEmpty]
		public string Name
		{
			get
			{
				return (string)this[UMCallAnsweringRuleSchema.Name];
			}
			set
			{
				this[UMCallAnsweringRuleSchema.Name] = value;
			}
		}

		[Parameter]
		public int Priority
		{
			get
			{
				return (int)this[UMCallAnsweringRuleSchema.Priority];
			}
			set
			{
				this[UMCallAnsweringRuleSchema.Priority] = value;
			}
		}

		public RuleDescription Description { get; internal set; }

		[Parameter]
		public int ScheduleStatus
		{
			get
			{
				return (int)this[UMCallAnsweringRuleSchema.ScheduleStatus];
			}
			set
			{
				this[UMCallAnsweringRuleSchema.ScheduleStatus] = value;
			}
		}

		[Parameter]
		public TimeOfDay TimeOfDay
		{
			get
			{
				return (TimeOfDay)this[UMCallAnsweringRuleSchema.TimeOfDay];
			}
			set
			{
				this[UMCallAnsweringRuleSchema.TimeOfDay] = value;
			}
		}

		public UMCallAnsweringRule() : base(new SimpleProviderPropertyBag())
		{
		}

		public UMCallAnsweringRule(UMCallAnsweringRuleId identity) : base(new SimpleProviderPropertyBag())
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this.propertyBag.SetField(SimpleProviderObjectSchema.Identity, identity);
		}

		public override string ToString()
		{
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			if (!string.IsNullOrEmpty(this.Name))
			{
				return this.Name;
			}
			return base.ToString();
		}

		private static UMCallAnsweringRuleSchema schema = ObjectSchema.GetInstance<UMCallAnsweringRuleSchema>();
	}
}
