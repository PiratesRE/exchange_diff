using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class CommonLogRowData : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		public string Machine
		{
			get
			{
				return (string)this[CommonLogRowData.MachineProperty];
			}
			set
			{
				this[CommonLogRowData.MachineProperty] = value;
			}
		}

		public DateTime LogTime
		{
			get
			{
				return (DateTime)this[CommonLogRowData.LogTimeProperty];
			}
			set
			{
				this[CommonLogRowData.LogTimeProperty] = value;
			}
		}

		public DateTime ItemTime
		{
			get
			{
				return (DateTime)this[CommonLogRowData.ItemTimeProperty];
			}
			set
			{
				this[CommonLogRowData.ItemTimeProperty] = value;
			}
		}

		public Guid TenantId
		{
			get
			{
				return (Guid)this[CommonLogRowData.TenantIdProperty];
			}
			set
			{
				this[CommonLogRowData.TenantIdProperty] = value;
			}
		}

		public string Agent
		{
			get
			{
				return (string)this[CommonLogRowData.AgentProperty];
			}
			set
			{
				this[CommonLogRowData.AgentProperty] = value;
			}
		}

		public string Source
		{
			get
			{
				return (string)this[CommonLogRowData.SourceProperty];
			}
			set
			{
				this[CommonLogRowData.SourceProperty] = value;
			}
		}

		public Guid Scope
		{
			get
			{
				return (Guid)this[CommonLogRowData.ScopeProperty];
			}
			set
			{
				this[CommonLogRowData.ScopeProperty] = value;
			}
		}

		public Guid ItemId
		{
			get
			{
				return (Guid)this[CommonLogRowData.ItemIdProperty];
			}
			set
			{
				this[CommonLogRowData.ItemIdProperty] = value;
			}
		}

		public string EventType
		{
			get
			{
				return (string)this[CommonLogRowData.EventTypeProperty];
			}
			set
			{
				this[CommonLogRowData.EventTypeProperty] = value;
			}
		}

		public int LineId
		{
			get
			{
				return (int)this[CommonLogRowData.LineIdProperty];
			}
			set
			{
				this[CommonLogRowData.LineIdProperty] = value;
			}
		}

		public string CustomData
		{
			get
			{
				return (string)this[CommonLogRowData.CustomDataProperty];
			}
			set
			{
				this[CommonLogRowData.CustomDataProperty] = value;
			}
		}

		public string PIICustomData
		{
			get
			{
				return (string)this[CommonLogRowData.PIICustomDataProperty];
			}
			set
			{
				this[CommonLogRowData.PIICustomDataProperty] = value;
			}
		}

		public Guid ObjectId
		{
			get
			{
				return (Guid)this[CommonLogRowData.ObjectIdProperty];
			}
			set
			{
				this[CommonLogRowData.ObjectIdProperty] = value;
			}
		}

		public override IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			return CommonLogRowData.propertydefinitions;
		}

		private static readonly HygienePropertyDefinition MachineProperty = new HygienePropertyDefinition("Machine", typeof(string));

		internal static readonly HygienePropertyDefinition LogTimeProperty = new HygienePropertyDefinition("LogTime", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);

		private static readonly HygienePropertyDefinition ItemTimeProperty = new HygienePropertyDefinition("ItemTime", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition TenantIdProperty = new HygienePropertyDefinition("TenantId", typeof(Guid));

		internal static readonly HygienePropertyDefinition AgentProperty = new HygienePropertyDefinition("Agent", typeof(string));

		internal static readonly HygienePropertyDefinition SourceProperty = new HygienePropertyDefinition("Source", typeof(string));

		private static readonly HygienePropertyDefinition ScopeProperty = new HygienePropertyDefinition("Scope", typeof(Guid));

		private static readonly HygienePropertyDefinition ItemIdProperty = new HygienePropertyDefinition("ItemId", typeof(Guid));

		internal static readonly HygienePropertyDefinition EventTypeProperty = new HygienePropertyDefinition("EventType", typeof(string));

		internal static readonly HygienePropertyDefinition LineIdProperty = new HygienePropertyDefinition("LineId", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		private static readonly HygienePropertyDefinition CustomDataProperty = new HygienePropertyDefinition("CustomData", typeof(string));

		private static readonly HygienePropertyDefinition PIICustomDataProperty = new HygienePropertyDefinition("PIICustomData", typeof(string));

		private static readonly HygienePropertyDefinition ObjectIdProperty = new HygienePropertyDefinition("ObjectId", typeof(Guid));

		private static readonly PropertyDefinition[] propertydefinitions = new PropertyDefinition[]
		{
			CommonLogRowData.MachineProperty,
			CommonLogRowData.LogTimeProperty,
			CommonLogRowData.ItemTimeProperty,
			CommonLogRowData.TenantIdProperty,
			CommonLogRowData.AgentProperty,
			CommonLogRowData.SourceProperty,
			CommonLogRowData.ScopeProperty,
			CommonLogRowData.ItemIdProperty,
			CommonLogRowData.EventTypeProperty,
			CommonLogRowData.LineIdProperty,
			CommonLogRowData.CustomDataProperty,
			CommonLogRowData.PIICustomDataProperty,
			CommonLogRowData.ObjectIdProperty
		};
	}
}
