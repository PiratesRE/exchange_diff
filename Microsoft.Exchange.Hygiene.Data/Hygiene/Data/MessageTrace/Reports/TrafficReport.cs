using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports
{
	internal class TrafficReport : Schema
	{
		public string Organization
		{
			get
			{
				return (string)this[TrafficReport.OrganizationDefinition];
			}
			set
			{
				this[TrafficReport.OrganizationDefinition] = value;
			}
		}

		public string Domain
		{
			get
			{
				return (string)this[TrafficReport.DomainDefinition];
			}
			set
			{
				this[TrafficReport.DomainDefinition] = value;
			}
		}

		public int DateKey
		{
			get
			{
				return (int)this[TrafficReport.DateKeyDefinition];
			}
			set
			{
				this[TrafficReport.DateKeyDefinition] = value;
			}
		}

		public int HourKey
		{
			get
			{
				return (int)this[TrafficReport.HourKeyDefinition];
			}
			set
			{
				this[TrafficReport.HourKeyDefinition] = value;
			}
		}

		public string Direction
		{
			get
			{
				return (string)this[TrafficReport.DirectionDefinition];
			}
			set
			{
				this[TrafficReport.DirectionDefinition] = value;
			}
		}

		public string EventType
		{
			get
			{
				return (string)this[TrafficReport.EventTypeDefinition];
			}
			set
			{
				this[TrafficReport.EventTypeDefinition] = value;
			}
		}

		public string Action
		{
			get
			{
				return (string)this[TrafficReport.ActionDefinition];
			}
			set
			{
				this[TrafficReport.ActionDefinition] = value;
			}
		}

		public int MessageCount
		{
			get
			{
				return (int)this[TrafficReport.MessageCountDefinition];
			}
			set
			{
				this[TrafficReport.MessageCountDefinition] = value;
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
	}
}
