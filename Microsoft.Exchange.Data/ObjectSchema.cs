using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	internal abstract class ObjectSchema
	{
		internal static TSchema GetInstance<TSchema>() where TSchema : ObjectSchema, new()
		{
			ObjectSchema.SchemaConstructorDelegate schemaConstructor = () => Activator.CreateInstance<TSchema>();
			ObjectSchema instanceImpl = ObjectSchema.GetInstanceImpl(typeof(TSchema), schemaConstructor);
			return (TSchema)((object)instanceImpl);
		}

		internal static ObjectSchema GetInstance(Type schemaType)
		{
			if (null == schemaType)
			{
				throw new ArgumentNullException("schemaType");
			}
			if (!typeof(ObjectSchema).GetTypeInfo().IsAssignableFrom(schemaType.GetTypeInfo()))
			{
				throw new ArgumentException(string.Format("Invalid ObjectSchema Input Type: {0}", schemaType), "schemaType");
			}
			if (!ReflectionHelper.HasParameterlessConstructor(schemaType))
			{
				throw new ArgumentException(string.Format("Input type does not have a parameterless constructor: {0}", schemaType), "schemaType");
			}
			ObjectSchema.SchemaConstructorDelegate schemaConstructor = () => (ObjectSchema)Activator.CreateInstance(schemaType);
			return ObjectSchema.GetInstanceImpl(schemaType, schemaConstructor);
		}

		private static ObjectSchema GetInstanceImpl(Type schemaType, ObjectSchema.SchemaConstructorDelegate schemaConstructor)
		{
			ObjectSchema result;
			if (!ObjectSchema.Instances.TryGetValue(schemaType, out result))
			{
				lock (ObjectSchema.InstancesLock)
				{
					if (!ObjectSchema.Instances.TryGetValue(schemaType, out result))
					{
						ObjectSchema objectSchema = schemaConstructor();
						ObjectSchema.Instances = new Dictionary<Type, ObjectSchema>(ObjectSchema.Instances)
						{
							{
								schemaType,
								objectSchema
							}
						};
						result = objectSchema;
					}
				}
			}
			return result;
		}

		protected ObjectSchema()
		{
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>();
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			List<FieldInfo> list2 = ReflectionHelper.AggregateTypeHierarchy<FieldInfo>(base.GetType(), new AggregateType<FieldInfo>(ReflectionHelper.AggregateStaticFields));
			IEnumerable<FieldInfo> declaredFields = base.GetType().GetTypeInfo().DeclaredFields;
			foreach (FieldInfo fieldInfo in list2)
			{
				object value = fieldInfo.GetValue(null);
				if (typeof(ProviderPropertyDefinition).GetTypeInfo().IsAssignableFrom(fieldInfo.FieldType.GetTypeInfo()) && value == null)
				{
					throw new InvalidOperationException(string.Format("Property definition '{0}' is not initialized. This can be caused by loop dependency between initialization of one or more static fields.", fieldInfo.Name));
				}
				ProviderPropertyDefinition providerPropertyDefinition = value as ProviderPropertyDefinition;
				if (providerPropertyDefinition != null)
				{
					bool flag = false;
					foreach (FieldInfo fieldInfo2 in declaredFields)
					{
						if (fieldInfo2.Name.Equals(fieldInfo.Name) && !this.IsSameFieldHandle(fieldInfo2, fieldInfo))
						{
							flag = true;
							break;
						}
					}
					if (!this.containsCalculatedProperties && providerPropertyDefinition.IsCalculated)
					{
						this.containsCalculatedProperties = true;
					}
					if (!flag)
					{
						if (!providerPropertyDefinition.IsFilterOnly)
						{
							hashSet.TryAdd(providerPropertyDefinition);
							if (!providerPropertyDefinition.IsCalculated)
							{
								continue;
							}
							using (ReadOnlyCollection<ProviderPropertyDefinition>.Enumerator enumerator3 = providerPropertyDefinition.SupportingProperties.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									ProviderPropertyDefinition item = enumerator3.Current;
									hashSet.TryAdd(item);
								}
								continue;
							}
						}
						if (!providerPropertyDefinition.IsCalculated)
						{
							list.Add(providerPropertyDefinition);
						}
					}
				}
			}
			this.AllProperties = new ReadOnlyCollection<PropertyDefinition>(hashSet.ToArray());
			this.AllFilterOnlyProperties = new ReadOnlyCollection<PropertyDefinition>(list.ToArray());
			this.InitializePropertyCollections();
		}

		protected void InitializePropertyCollections()
		{
			if (this.AllProperties == null)
			{
				throw new InvalidOperationException("Dev Bug: AllProperties should never be null");
			}
			if (this.AllFilterOnlyProperties == null)
			{
				throw new InvalidOperationException("Dev Bug: AllFilterOnlyProperties should never be null");
			}
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			List<PropertyDefinition> list2 = new List<PropertyDefinition>();
			foreach (PropertyDefinition propertyDefinition in this.AllProperties)
			{
				ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)propertyDefinition;
				if (providerPropertyDefinition.IsFilterable)
				{
					list.Add(providerPropertyDefinition);
				}
				if (providerPropertyDefinition.IsMandatory && !providerPropertyDefinition.IsCalculated)
				{
					list2.Add(providerPropertyDefinition);
				}
			}
			foreach (PropertyDefinition propertyDefinition2 in this.AllFilterOnlyProperties)
			{
				ProviderPropertyDefinition providerPropertyDefinition2 = (ProviderPropertyDefinition)propertyDefinition2;
				if (providerPropertyDefinition2.IsFilterable)
				{
					list.Add(providerPropertyDefinition2);
				}
			}
			this.allFilterableProperties = new ReadOnlyCollection<PropertyDefinition>(list.ToArray());
			this.allMandatoryProperties = new ReadOnlyCollection<PropertyDefinition>(list2.ToArray());
		}

		public ReadOnlyCollection<PropertyDefinition> AllProperties
		{
			get
			{
				return this.allProperties;
			}
			protected set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.allProperties = value;
			}
		}

		public ReadOnlyCollection<PropertyDefinition> AllFilterOnlyProperties
		{
			get
			{
				return this.allFilterOnlyProperties;
			}
			protected set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.allFilterOnlyProperties = value;
			}
		}

		public virtual ReadOnlyCollection<PropertyDefinition> AllFilterableProperties
		{
			get
			{
				return this.allFilterableProperties;
			}
		}

		public ReadOnlyCollection<PropertyDefinition> AllMandatoryProperties
		{
			get
			{
				return this.allMandatoryProperties;
			}
		}

		public bool ContainsCalculatedProperties
		{
			get
			{
				return this.containsCalculatedProperties;
			}
		}

		public PropertyDefinition this[string propertyName]
		{
			get
			{
				if (this.namesToProperties == null)
				{
					Dictionary<string, PropertyDefinition> dictionary = new Dictionary<string, PropertyDefinition>(this.AllProperties.Count, StringComparer.OrdinalIgnoreCase);
					foreach (PropertyDefinition propertyDefinition in this.AllProperties)
					{
						dictionary[propertyDefinition.Name] = propertyDefinition;
					}
					Interlocked.CompareExchange<Dictionary<string, PropertyDefinition>>(ref this.namesToProperties, dictionary, null);
				}
				PropertyDefinition result = null;
				this.namesToProperties.TryGetValue(propertyName, out result);
				return result;
			}
		}

		private bool IsSameFieldHandle(FieldInfo left, FieldInfo right)
		{
			return left.FieldHandle.Value == right.FieldHandle.Value;
		}

		private ReadOnlyCollection<PropertyDefinition> allProperties;

		private ReadOnlyCollection<PropertyDefinition> allFilterableProperties;

		private ReadOnlyCollection<PropertyDefinition> allMandatoryProperties;

		private ReadOnlyCollection<PropertyDefinition> allFilterOnlyProperties;

		private Dictionary<string, PropertyDefinition> namesToProperties;

		private static Dictionary<Type, ObjectSchema> Instances = new Dictionary<Type, ObjectSchema>();

		private static readonly object InstancesLock = new object();

		private readonly bool containsCalculatedProperties;

		private delegate ObjectSchema SchemaConstructorDelegate();
	}
}
