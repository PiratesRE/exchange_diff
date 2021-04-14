using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Entity
{
	internal abstract class EntityProperty : PropertyBase
	{
		public EntityProperty(PropertyType type = PropertyType.ReadWrite, bool syncForOccurenceItem = false)
		{
			this.Type = type;
			this.SyncForOccurenceItem = syncForOccurenceItem;
		}

		public EntityProperty(PropertyDefinition edmProperty, PropertyType type = PropertyType.ReadWrite, bool syncForOccurenceItem = false) : this(new EntityPropertyDefinition(edmProperty), type, false)
		{
		}

		public EntityProperty(EntityPropertyDefinition propertyDefinition, PropertyType type = PropertyType.ReadWrite, bool syncForOccurenceItem = false)
		{
			this.EntityPropertyDefinition = propertyDefinition;
			this.Type = type;
			this.SyncForOccurenceItem = syncForOccurenceItem;
		}

		private protected IItem Item { protected get; private set; }

		public PropertyType Type { get; private set; }

		public bool SyncForOccurenceItem { get; set; }

		internal EntityPropertyDefinition EntityPropertyDefinition { get; private set; }

		protected Event CalendaringEvent
		{
			get
			{
				if (this.calendaringEvent == null)
				{
					this.calendaringEvent = (this.Item as Event);
					if (this.calendaringEvent == null)
					{
						throw new UnexpectedTypeException("Event", this.Item);
					}
				}
				return this.calendaringEvent;
			}
		}

		public virtual void Bind(IItem item)
		{
			this.Item = item;
			if (!this.SyncForOccurenceItem && this.CalendaringEvent != null && this.CalendaringEvent.Type == EventType.Occurrence)
			{
				base.State = PropertyState.SetToDefault;
				return;
			}
			if (this.EntityPropertyDefinition == null)
			{
				base.State = PropertyState.Modified;
				return;
			}
			if (this.Item.IsPropertySet(this.EntityPropertyDefinition.EdmDefinition))
			{
				base.State = PropertyState.Modified;
				return;
			}
			base.State = PropertyState.SetToDefault;
		}

		public override void Unbind()
		{
			try
			{
				this.calendaringEvent = null;
				this.Item = null;
				base.State = PropertyState.Uninitialized;
			}
			finally
			{
				base.Unbind();
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"(Base: ",
				base.ToString(),
				", propertyDefinition: ",
				this.EntityPropertyDefinition,
				", type: ",
				this.Type,
				", item: ",
				this.Item,
				", state: ",
				base.State,
				")"
			});
		}

		private Event calendaringEvent;
	}
}
