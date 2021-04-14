using System;
using System.Collections.Generic;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class RouteMessageOutboundConnectorAction : TransportRuleAction, IEquatable<RouteMessageOutboundConnectorAction>
	{
		public override int GetHashCode()
		{
			if (!string.IsNullOrEmpty(this.ConnectorName))
			{
				return this.ConnectorName.GetHashCode();
			}
			return 0;
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as RouteMessageOutboundConnectorAction)));
		}

		public bool Equals(RouteMessageOutboundConnectorAction other)
		{
			if (string.IsNullOrEmpty(this.ConnectorName))
			{
				return string.IsNullOrEmpty(other.ConnectorName);
			}
			return this.ConnectorName.Equals(other.ConnectorName);
		}

		[LocDescription(RulesTasksStrings.IDs.ConnectorNameDescription)]
		[ActionParameterName("RouteMessageOutboundConnector")]
		[LocDisplayName(RulesTasksStrings.IDs.ConnectorNameDisplayName)]
		public string ConnectorName { get; set; }

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionRouteMessageOutboundConnector(this.ConnectorName);
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "RouteMessageOutboundConnector")
			{
				return null;
			}
			RouteMessageOutboundConnectorAction routeMessageOutboundConnectorAction = new RouteMessageOutboundConnectorAction();
			try
			{
				routeMessageOutboundConnectorAction.ConnectorName = TransportRuleAction.GetStringValue(action.Arguments[0]);
			}
			catch (ArgumentOutOfRangeException)
			{
				return null;
			}
			return routeMessageOutboundConnectorAction;
		}

		internal override void Reset()
		{
			this.ConnectorName = string.Empty;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.ConnectorName == string.Empty)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			int index;
			if (!Utils.CheckIsUnicodeStringWellFormed(this.ConnectorName, out index))
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.CommentsHaveInvalidChars((int)this.ConnectorName[index]), base.Name));
				return;
			}
			base.ValidateRead(errors);
		}

		internal override Action ToInternalAction()
		{
			return TransportRuleParser.Instance.CreateAction("RouteMessageOutboundConnector", new ShortList<Argument>
			{
				new Value(this.ConnectorName)
			}, Utils.GetActionName(this));
		}

		internal override string GetActionParameters()
		{
			return Utils.QuoteCmdletParameter(this.ConnectorName);
		}

		internal override void SuppressPiiData()
		{
			this.ConnectorName = SuppressingPiiProperty.TryRedactValue<string>(RuleSchema.RouteMessageOutboundConnector, this.ConnectorName);
		}
	}
}
