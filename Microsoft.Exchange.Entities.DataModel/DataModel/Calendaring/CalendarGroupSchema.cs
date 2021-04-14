using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public sealed class CalendarGroupSchema : StorageEntitySchema
	{
		public CalendarGroupSchema()
		{
			base.RegisterPropertyDefinition(CalendarGroupSchema.StaticClassIdProperty);
			base.RegisterPropertyDefinition(CalendarGroupSchema.StaticNameProperty);
		}

		public TypedPropertyDefinition<Guid> ClassIdProperty
		{
			get
			{
				return CalendarGroupSchema.StaticClassIdProperty;
			}
		}

		public TypedPropertyDefinition<string> NameProperty
		{
			get
			{
				return CalendarGroupSchema.StaticNameProperty;
			}
		}

		private static readonly TypedPropertyDefinition<Guid> StaticClassIdProperty = new TypedPropertyDefinition<Guid>("CalendarGroup.ClassId", default(Guid), true);

		private static readonly TypedPropertyDefinition<string> StaticNameProperty = new TypedPropertyDefinition<string>("CalendarGroup.Name", null, true);
	}
}
