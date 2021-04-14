using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports
{
	internal class MessageTraceDetail : Schema
	{
		public string Organization
		{
			get
			{
				return (string)this[MessageTraceDetail.OrganizationDefinition];
			}
			set
			{
				this[MessageTraceDetail.OrganizationDefinition] = value;
			}
		}

		public Guid InternalMessageId
		{
			get
			{
				return (Guid)this[MessageTraceDetail.InternalMessageIdDefinition];
			}
			set
			{
				this[MessageTraceDetail.InternalMessageIdDefinition] = value;
			}
		}

		public string ClientMessageId
		{
			get
			{
				return (string)this[MessageTraceDetail.ClientMessageIdDefinition];
			}
			set
			{
				this[MessageTraceDetail.ClientMessageIdDefinition] = value;
			}
		}

		public DateTime EventDate
		{
			get
			{
				return (DateTime)this[MessageTraceDetail.EventDateDefinition];
			}
			set
			{
				this[MessageTraceDetail.EventDateDefinition] = value;
			}
		}

		public string EventDescription
		{
			get
			{
				return (string)this[MessageTraceDetail.EventDescriptionDefinition];
			}
			set
			{
				this[MessageTraceDetail.EventDescriptionDefinition] = value;
			}
		}

		public string AgentName
		{
			get
			{
				return (string)this[MessageTraceDetail.AgentNameDefinition];
			}
			set
			{
				this[MessageTraceDetail.AgentNameDefinition] = value;
			}
		}

		public string Action
		{
			get
			{
				return (string)this[MessageTraceDetail.ActionDefinition];
			}
			set
			{
				this[MessageTraceDetail.ActionDefinition] = value;
			}
		}

		public string RuleId
		{
			get
			{
				return (string)this[MessageTraceDetail.RuleIdDefinition];
			}
			set
			{
				this[MessageTraceDetail.RuleIdDefinition] = value;
			}
		}

		public string RuleName
		{
			get
			{
				return (string)this[MessageTraceDetail.TransportRuleNameDefinition];
			}
			set
			{
				this[MessageTraceDetail.TransportRuleNameDefinition] = value;
			}
		}

		public string PolicyId
		{
			get
			{
				return (string)this[MessageTraceDetail.PolicyIdDefinition];
			}
			set
			{
				this[MessageTraceDetail.PolicyIdDefinition] = value;
			}
		}

		public string PolicyName
		{
			get
			{
				return (string)this[MessageTraceDetail.PolicyNameDefinition];
			}
			set
			{
				this[MessageTraceDetail.PolicyNameDefinition] = value;
			}
		}

		public string Data
		{
			get
			{
				return (string)this[MessageTraceDetail.DataDefinition];
			}
			set
			{
				this[MessageTraceDetail.DataDefinition] = value;
			}
		}

		internal static readonly HygienePropertyDefinition OrganizationDefinition = new HygienePropertyDefinition("OrganizationalUnitRootId", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition InternalMessageIdDefinition = new HygienePropertyDefinition("InternalMessageId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ClientMessageIdDefinition = new HygienePropertyDefinition("ClientMessageId", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EventDateDefinition = new HygienePropertyDefinition("EventDate", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EventDescriptionDefinition = new HygienePropertyDefinition("EventDescription", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition AgentNameDefinition = new HygienePropertyDefinition("AgentName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ActionDefinition = new HygienePropertyDefinition("Action", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition RuleIdDefinition = new HygienePropertyDefinition("RuleId", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition TransportRuleNameDefinition = new HygienePropertyDefinition("TransportRuleName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition PolicyIdDefinition = new HygienePropertyDefinition("PolicyId", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition PolicyNameDefinition = new HygienePropertyDefinition("PolicyName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DataDefinition = new HygienePropertyDefinition("PropertyBag", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
