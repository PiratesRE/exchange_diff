using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports
{
	internal class TopTrafficReport : Schema
	{
		public string Organization
		{
			get
			{
				return (string)this[TopTrafficReport.OrganizationDefinition];
			}
			set
			{
				this[TopTrafficReport.OrganizationDefinition] = value;
			}
		}

		public string Domain
		{
			get
			{
				return (string)this[TopTrafficReport.DomainDefinition];
			}
			set
			{
				this[TopTrafficReport.DomainDefinition] = value;
			}
		}

		public int DateKey
		{
			get
			{
				return (int)this[TopTrafficReport.DateKeyDefinition];
			}
			set
			{
				this[TopTrafficReport.DateKeyDefinition] = value;
			}
		}

		public int HourKey
		{
			get
			{
				return (int)this[TopTrafficReport.HourKeyDefinition];
			}
			set
			{
				this[TopTrafficReport.HourKeyDefinition] = value;
			}
		}

		public string Direction
		{
			get
			{
				return (string)this[TopTrafficReport.DirectionDefinition];
			}
			set
			{
				this[TopTrafficReport.DirectionDefinition] = value;
			}
		}

		public string EventType
		{
			get
			{
				return (string)this[TopTrafficReport.EventTypeDefinition];
			}
			set
			{
				this[TopTrafficReport.EventTypeDefinition] = value;
			}
		}

		public int MessageCount
		{
			get
			{
				return (int)this[TopTrafficReport.MessageCountDefinition];
			}
			set
			{
				this[TopTrafficReport.MessageCountDefinition] = value;
			}
		}

		public string Name
		{
			get
			{
				return (string)this[TopTrafficReport.NameDefinition];
			}
			set
			{
				this[TopTrafficReport.NameDefinition] = value;
			}
		}

		internal static readonly HygienePropertyDefinition OrganizationDefinition = new HygienePropertyDefinition("OrganizationalUnitRootId", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DomainDefinition = new HygienePropertyDefinition("DomainName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DateKeyDefinition = new HygienePropertyDefinition("DateKey", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition HourKeyDefinition = new HygienePropertyDefinition("HourKey", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition NameDefinition = new HygienePropertyDefinition("Name", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition EventTypeDefinition = new HygienePropertyDefinition("EventType", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition DirectionDefinition = new HygienePropertyDefinition("Direction", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition MessageCountDefinition = new HygienePropertyDefinition("MessageCount", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
