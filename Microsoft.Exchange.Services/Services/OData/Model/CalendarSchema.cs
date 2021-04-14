using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CalendarSchema : EntitySchema
	{
		public new static CalendarSchema SchemaInstance
		{
			get
			{
				return CalendarSchema.CalendarSchemaInstance.Member;
			}
		}

		public override EdmEntityType EdmEntityType
		{
			get
			{
				return Calendar.EdmEntityType;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DeclaredProperties
		{
			get
			{
				return CalendarSchema.DeclaredCalendarProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> AllProperties
		{
			get
			{
				return CalendarSchema.AllCalendarProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DefaultProperties
		{
			get
			{
				return CalendarSchema.DefaultCalendarProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> MandatoryCreationProperties
		{
			get
			{
				return CalendarSchema.MandatoryCalendarCreationProperties;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static CalendarSchema()
		{
			PropertyDefinition propertyDefinition = new PropertyDefinition("Name", typeof(string));
			propertyDefinition.EdmType = EdmCoreModel.Instance.GetString(true);
			propertyDefinition.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate);
			PropertyDefinition propertyDefinition2 = propertyDefinition;
			DataEntityPropertyProvider<Calendar> dataEntityPropertyProvider = new DataEntityPropertyProvider<Calendar>("Name");
			dataEntityPropertyProvider.Getter = delegate(Entity e, PropertyDefinition ep, Calendar d)
			{
				e[ep] = d.Name;
			};
			dataEntityPropertyProvider.Setter = delegate(Entity e, PropertyDefinition ep, Calendar d)
			{
				d.Name = (string)e[ep];
			};
			propertyDefinition2.DataEntityPropertyProvider = dataEntityPropertyProvider;
			CalendarSchema.Name = propertyDefinition;
			CalendarSchema.ChangeKey = ItemSchema.ChangeKey;
			CalendarSchema.Events = new PropertyDefinition("Events", typeof(IEnumerable<Event>))
			{
				NavigationTargetEntity = Event.EdmEntityType,
				Flags = PropertyDefinitionFlags.Navigation
			};
			CalendarSchema.DeclaredCalendarProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
			{
				CalendarSchema.Name,
				CalendarSchema.ChangeKey,
				CalendarSchema.Events
			});
			CalendarSchema.AllCalendarProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(EntitySchema.AllEntityProperties.Union(CalendarSchema.DeclaredCalendarProperties)));
			CalendarSchema.DefaultCalendarProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(EntitySchema.DefaultEntityProperties)
			{
				CalendarSchema.Name,
				CalendarSchema.ChangeKey
			});
			CalendarSchema.MandatoryCalendarCreationProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
			{
				CalendarSchema.Name
			});
			CalendarSchema.CalendarSchemaInstance = new LazyMember<CalendarSchema>(() => new CalendarSchema());
		}

		public static readonly PropertyDefinition Name;

		public static readonly PropertyDefinition ChangeKey;

		public static readonly PropertyDefinition Events;

		public static readonly ReadOnlyCollection<PropertyDefinition> DeclaredCalendarProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> AllCalendarProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> DefaultCalendarProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> MandatoryCalendarCreationProperties;

		private static readonly LazyMember<CalendarSchema> CalendarSchemaInstance;
	}
}
