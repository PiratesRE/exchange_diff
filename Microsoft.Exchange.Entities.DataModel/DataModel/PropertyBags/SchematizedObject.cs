using System;

namespace Microsoft.Exchange.Entities.DataModel.PropertyBags
{
	public abstract class SchematizedObject<TSchema> : PropertyChangeTrackingObject, ISchematizedObject<TSchema>, IPropertyChangeTracker<PropertyDefinition> where TSchema : TypeSchema, new()
	{
		public static TSchema SchemaInstance { get; private set; } = Activator.CreateInstance<TSchema>();

		public TSchema Schema
		{
			get
			{
				return SchematizedObject<TSchema>.SchemaInstance;
			}
		}
	}
}
