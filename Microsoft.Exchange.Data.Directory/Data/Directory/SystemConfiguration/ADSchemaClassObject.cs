using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public sealed class ADSchemaClassObject : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADSchemaClassObject.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADSchemaClassObject.mostDerivedClass;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[ADSchemaObjectSchema.DisplayName];
			}
		}

		public Guid SchemaIDGuid
		{
			get
			{
				return (Guid)this[ADSchemaObjectSchema.SchemaIDGuid];
			}
		}

		public MultiValuedProperty<string> MayContain
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADSchemaObjectSchema.MayContain];
			}
		}

		public MultiValuedProperty<string> SystemMayContain
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADSchemaObjectSchema.SystemMayContain];
			}
		}

		public MultiValuedProperty<string> MustContain
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADSchemaObjectSchema.MustContain];
			}
		}

		public MultiValuedProperty<string> SystemMustContain
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADSchemaObjectSchema.SystemMustContain];
			}
		}

		public ADObjectId DefaultObjectCategory
		{
			get
			{
				return (ADObjectId)this[ADSchemaObjectSchema.DefaultObjectCategory];
			}
		}

		private static ADSchemaObjectSchema schema = ObjectSchema.GetInstance<ADSchemaObjectSchema>();

		private static string mostDerivedClass = "classSchema";
	}
}
