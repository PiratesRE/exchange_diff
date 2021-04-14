using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Common
{
	internal class Schema
	{
		protected internal Schema()
		{
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>();
			HashSet<PropertyDefinition> hashSet2 = new HashSet<PropertyDefinition>();
			FieldInfo[] fields = base.GetType().GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			foreach (FieldInfo fieldInfo in fields)
			{
				object value = fieldInfo.GetValue(null);
				PropertyDefinition propertyDefinition = value as PropertyDefinition;
				if (propertyDefinition != null)
				{
					hashSet2.Add(propertyDefinition);
					if (fieldInfo.IsPublic)
					{
						hashSet.Add(propertyDefinition);
					}
				}
			}
			this.allProperties = hashSet;
			this.allPropertiesInternal = hashSet2;
		}

		public static Schema Instance
		{
			get
			{
				if (Schema.instance == null)
				{
					Schema.instance = new Schema();
				}
				return Schema.instance;
			}
		}

		public ICollection<PropertyDefinition> AllProperties
		{
			get
			{
				return this.allProperties;
			}
		}

		internal ICollection<PropertyDefinition> InternalAllProperties
		{
			get
			{
				return this.allPropertiesInternal;
			}
		}

		private const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		private static Schema instance;

		private HashSet<PropertyDefinition> allProperties;

		private HashSet<PropertyDefinition> allPropertiesInternal;
	}
}
