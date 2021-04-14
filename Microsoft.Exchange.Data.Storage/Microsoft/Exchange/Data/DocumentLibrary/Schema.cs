using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Schema
	{
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

		protected internal Schema()
		{
			this.allProperties = new Dictionary<PropertyDefinition, DocumentLibraryPropertyDefinition>();
			BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
			FieldInfo[] fields = base.GetType().GetFields(bindingAttr);
			foreach (FieldInfo fieldInfo in fields)
			{
				object value = fieldInfo.GetValue(null);
				DocumentLibraryPropertyDefinition documentLibraryPropertyDefinition = value as DocumentLibraryPropertyDefinition;
				if (documentLibraryPropertyDefinition != null)
				{
					if (documentLibraryPropertyDefinition.PropertyId != DocumentLibraryPropertyId.None)
					{
						this.idToPropertyMap.Add(documentLibraryPropertyDefinition.PropertyId, documentLibraryPropertyDefinition);
					}
					this.allProperties.Add(documentLibraryPropertyDefinition, documentLibraryPropertyDefinition);
				}
			}
		}

		internal Dictionary<PropertyDefinition, DocumentLibraryPropertyDefinition> AllProperties
		{
			get
			{
				return this.allProperties;
			}
		}

		internal Dictionary<DocumentLibraryPropertyId, DocumentLibraryPropertyDefinition> IdToPropertyMap
		{
			get
			{
				return this.idToPropertyMap;
			}
		}

		private static Schema instance;

		private Dictionary<PropertyDefinition, DocumentLibraryPropertyDefinition> allProperties = new Dictionary<PropertyDefinition, DocumentLibraryPropertyDefinition>();

		private Dictionary<DocumentLibraryPropertyId, DocumentLibraryPropertyDefinition> idToPropertyMap = new Dictionary<DocumentLibraryPropertyId, DocumentLibraryPropertyDefinition>();
	}
}
