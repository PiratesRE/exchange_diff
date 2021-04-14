using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CalendarGroupSchema : EntitySchema
	{
		public new static CalendarGroupSchema SchemaInstance
		{
			get
			{
				return CalendarGroupSchema.CalendarGroupSchemaInstance.Member;
			}
		}

		public override EdmEntityType EdmEntityType
		{
			get
			{
				return CalendarGroup.EdmEntityType;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DeclaredProperties
		{
			get
			{
				return CalendarGroupSchema.DeclaredCalendarGroupProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> AllProperties
		{
			get
			{
				return CalendarGroupSchema.AllCalendarGroupProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DefaultProperties
		{
			get
			{
				return CalendarGroupSchema.DefaultCalendarGroupProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> MandatoryCreationProperties
		{
			get
			{
				return CalendarGroupSchema.MandatoryCalendarGroupCreationProperties;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static CalendarGroupSchema()
		{
			PropertyDefinition propertyDefinition = new PropertyDefinition("Name", typeof(string));
			propertyDefinition.EdmType = EdmCoreModel.Instance.GetString(true);
			propertyDefinition.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate);
			PropertyDefinition propertyDefinition2 = propertyDefinition;
			DataEntityPropertyProvider<CalendarGroup> dataEntityPropertyProvider = new DataEntityPropertyProvider<CalendarGroup>("Name");
			dataEntityPropertyProvider.Getter = delegate(Entity e, PropertyDefinition ep, CalendarGroup d)
			{
				e[ep] = d.Name;
			};
			dataEntityPropertyProvider.Setter = delegate(Entity e, PropertyDefinition ep, CalendarGroup d)
			{
				d.Name = (string)e[ep];
			};
			propertyDefinition2.DataEntityPropertyProvider = dataEntityPropertyProvider;
			CalendarGroupSchema.Name = propertyDefinition;
			PropertyDefinition propertyDefinition3 = new PropertyDefinition("ClassId", typeof(Guid));
			propertyDefinition3.EdmType = EdmCoreModel.Instance.GetGuid(true);
			propertyDefinition3.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate);
			PropertyDefinition propertyDefinition4 = propertyDefinition3;
			DataEntityPropertyProvider<CalendarGroup> dataEntityPropertyProvider2 = new DataEntityPropertyProvider<CalendarGroup>("ClassId");
			dataEntityPropertyProvider2.Getter = delegate(Entity e, PropertyDefinition ep, CalendarGroup d)
			{
				e[ep] = d.ClassId;
			};
			dataEntityPropertyProvider2.Setter = delegate(Entity e, PropertyDefinition ep, CalendarGroup d)
			{
				d.ClassId = (Guid)e[ep];
			};
			propertyDefinition4.DataEntityPropertyProvider = dataEntityPropertyProvider2;
			CalendarGroupSchema.ClassId = propertyDefinition3;
			CalendarGroupSchema.ChangeKey = ItemSchema.ChangeKey;
			CalendarGroupSchema.Calendars = new PropertyDefinition("Calendars", typeof(IEnumerable<Calendar>))
			{
				NavigationTargetEntity = Calendar.EdmEntityType,
				Flags = PropertyDefinitionFlags.Navigation
			};
			CalendarGroupSchema.DeclaredCalendarGroupProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
			{
				CalendarGroupSchema.Name,
				CalendarGroupSchema.ChangeKey,
				CalendarGroupSchema.ClassId,
				CalendarGroupSchema.Calendars
			});
			CalendarGroupSchema.AllCalendarGroupProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(EntitySchema.AllEntityProperties.Union(CalendarGroupSchema.DeclaredCalendarGroupProperties)));
			CalendarGroupSchema.DefaultCalendarGroupProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(EntitySchema.DefaultEntityProperties)
			{
				CalendarGroupSchema.Name,
				CalendarGroupSchema.ChangeKey,
				CalendarGroupSchema.ClassId
			});
			CalendarGroupSchema.MandatoryCalendarGroupCreationProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
			{
				CalendarSchema.Name
			});
			CalendarGroupSchema.CalendarGroupSchemaInstance = new LazyMember<CalendarGroupSchema>(() => new CalendarGroupSchema());
		}

		public static readonly PropertyDefinition Name;

		public static readonly PropertyDefinition ClassId;

		public static readonly PropertyDefinition ChangeKey;

		public static readonly PropertyDefinition Calendars;

		public static readonly ReadOnlyCollection<PropertyDefinition> DeclaredCalendarGroupProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> AllCalendarGroupProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> DefaultCalendarGroupProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> MandatoryCalendarGroupCreationProperties;

		private static readonly LazyMember<CalendarGroupSchema> CalendarGroupSchemaInstance;
	}
}
