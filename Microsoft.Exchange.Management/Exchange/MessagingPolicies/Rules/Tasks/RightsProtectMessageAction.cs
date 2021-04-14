using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.RightsManagement;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public sealed class RightsProtectMessageAction : TransportRuleAction, IEquatable<RightsProtectMessageAction>
	{
		public override int GetHashCode()
		{
			if (this.Template != null)
			{
				return this.Template.GetHashCode();
			}
			return 0;
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as RightsProtectMessageAction)));
		}

		public bool Equals(RightsProtectMessageAction other)
		{
			if (this.Template == null)
			{
				return null == other.Template;
			}
			return this.Template.Equals(other.Template);
		}

		[LocDisplayName(RulesTasksStrings.IDs.RmsTemplateDisplayName)]
		[ActionParameterName("ApplyRightsProtectionTemplate")]
		[LocDescription(RulesTasksStrings.IDs.RmsTemplateDescription)]
		public RmsTemplateIdentity Template
		{
			get
			{
				return this.template;
			}
			set
			{
				this.template = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionRightsProtectMessage(this.template.TemplateName);
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "RightsProtectMessage")
			{
				return null;
			}
			if (action.Arguments == null || action.Arguments.Count < 3)
			{
				return null;
			}
			TransportRuleAction result;
			try
			{
				RightsProtectMessageAction rightsProtectMessageAction = new RightsProtectMessageAction();
				Guid templateId = new Guid(TransportRuleAction.GetStringValue(action.Arguments[1]));
				string stringValue = TransportRuleAction.GetStringValue(action.Arguments[2]);
				rightsProtectMessageAction.Template = new RmsTemplateIdentity(templateId, stringValue);
				result = rightsProtectMessageAction;
			}
			catch (FormatException)
			{
				result = null;
			}
			catch (OverflowException)
			{
				result = null;
			}
			return result;
		}

		internal override void Reset()
		{
			this.template = new RmsTemplateIdentity();
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.template.TemplateId == Guid.Empty)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			base.ValidateRead(errors);
		}

		internal override Action ToInternalAction()
		{
			if (this.template.TemplateId == Guid.Empty)
			{
				throw new ArgumentException(RulesTasksStrings.InvalidRmsTemplate, "TemplateId");
			}
			ShortList<Argument> arguments = new ShortList<Argument>
			{
				new Value("X-MS-Exchange-Organization-RightsProtectMessage"),
				new Value(this.template.TemplateId.ToString("D", CultureInfo.InvariantCulture)),
				new Value(this.template.TemplateName)
			};
			return TransportRuleParser.Instance.CreateAction("RightsProtectMessage", arguments, Utils.GetActionName(this));
		}

		internal override string GetActionParameters()
		{
			return this.Template.TemplateId.ToString();
		}

		internal override void SuppressPiiData()
		{
			this.template = SuppressingPiiProperty.TryRedactValue<RmsTemplateIdentity>(RuleSchema.ApplyRightsProtectionTemplate, this.template);
		}

		private RmsTemplateIdentity template;
	}
}
