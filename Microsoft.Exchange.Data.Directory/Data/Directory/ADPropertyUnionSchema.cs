using System;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class ADPropertyUnionSchema : ADObjectSchema
	{
		protected ADPropertyUnionSchema()
		{
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>();
			HashSet<PropertyDefinition> hashSet2 = new HashSet<PropertyDefinition>();
			foreach (ObjectSchema objectSchema in this.ObjectSchemas)
			{
				foreach (PropertyDefinition item in objectSchema.AllProperties)
				{
					hashSet.TryAdd(item);
				}
				foreach (PropertyDefinition item2 in objectSchema.AllFilterOnlyProperties)
				{
					hashSet2.TryAdd(item2);
				}
			}
			base.AllProperties = new ReadOnlyCollection<PropertyDefinition>(hashSet.ToArray());
			base.AllFilterOnlyProperties = new ReadOnlyCollection<PropertyDefinition>(hashSet2.ToArray());
			base.InitializePropertyCollections();
			base.InitializeADObjectSchemaProperties();
		}

		public abstract ReadOnlyCollection<ADObjectSchema> ObjectSchemas { get; }
	}
}
