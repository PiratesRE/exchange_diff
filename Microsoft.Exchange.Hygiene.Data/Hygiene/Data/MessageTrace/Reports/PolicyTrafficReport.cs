using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports
{
	internal class PolicyTrafficReport : Schema
	{
		public string Organization
		{
			get
			{
				return (string)this[PolicyTrafficReport.OrganizationDefinition];
			}
			set
			{
				this[PolicyTrafficReport.OrganizationDefinition] = value;
			}
		}

		public string Domain
		{
			get
			{
				return (string)this[PolicyTrafficReport.DomainDefinition];
			}
			set
			{
				this[PolicyTrafficReport.DomainDefinition] = value;
			}
		}

		public int DateKey
		{
			get
			{
				return (int)this[PolicyTrafficReport.DateKeyDefinition];
			}
			set
			{
				this[PolicyTrafficReport.DateKeyDefinition] = value;
			}
		}

		public int HourKey
		{
			get
			{
				return (int)this[PolicyTrafficReport.HourKeyDefinition];
			}
			set
			{
				this[PolicyTrafficReport.HourKeyDefinition] = value;
			}
		}

		public string Direction
		{
			get
			{
				return (string)this[PolicyTrafficReport.DirectionDefinition];
			}
			set
			{
				this[PolicyTrafficReport.DirectionDefinition] = value;
			}
		}

		public string EventType
		{
			get
			{
				return (string)this[PolicyTrafficReport.EventTypeDefinition];
			}
			set
			{
				this[PolicyTrafficReport.EventTypeDefinition] = value;
			}
		}

		public string Action
		{
			get
			{
				return (string)this[PolicyTrafficReport.ActionDefinition];
			}
			set
			{
				this[PolicyTrafficReport.ActionDefinition] = value;
			}
		}

		public int MessageCount
		{
			get
			{
				return (int)this[PolicyTrafficReport.MessageCountDefinition];
			}
			set
			{
				this[PolicyTrafficReport.MessageCountDefinition] = value;
			}
		}

		public string PolicyName
		{
			get
			{
				return (string)this[PolicyTrafficReport.PolicyNameDefinition];
			}
			set
			{
				this[PolicyTrafficReport.PolicyNameDefinition] = value;
			}
		}

		public string RuleName
		{
			get
			{
				return (string)this[PolicyTrafficReport.RuleNameDefinition];
			}
			set
			{
				this[PolicyTrafficReport.RuleNameDefinition] = value;
			}
		}

		public string DataSource
		{
			get
			{
				return this[PolicyTrafficReport.DataSourceDefinition] as string;
			}
			set
			{
				this[PolicyTrafficReport.DataSourceDefinition] = value;
			}
		}

		internal static readonly HygienePropertyDefinition OrganizationDefinition = new HygienePropertyDefinition("OrganizationalUnitRootId", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DomainDefinition = new HygienePropertyDefinition("DomainName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DateKeyDefinition = new HygienePropertyDefinition("DateKey", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition HourKeyDefinition = new HygienePropertyDefinition("HourKey", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ActionDefinition = new HygienePropertyDefinition("Action", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EventTypeDefinition = new HygienePropertyDefinition("EventType", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DirectionDefinition = new HygienePropertyDefinition("Direction", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MessageCountDefinition = new HygienePropertyDefinition("MessageCount", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition PolicyNameDefinition = new HygienePropertyDefinition("PolicyName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition RuleNameDefinition = new HygienePropertyDefinition("RuleName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DataSourceDefinition = new HygienePropertyDefinition("DataSource", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
