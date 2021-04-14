using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SharepointPropertyDefinition : DocumentLibraryPropertyDefinition
	{
		internal static SharepointPropertyDefinition PropertyDefinitionToSharepointPropertyDefinition(Schema schema, PropertyDefinition propDef)
		{
			SharepointPropertyDefinition sharepointPropertyDefinition = propDef as SharepointPropertyDefinition;
			DocumentLibraryPropertyDefinition documentLibraryPropertyDefinition = propDef as DocumentLibraryPropertyDefinition;
			if (documentLibraryPropertyDefinition == null)
			{
				throw new ArgumentException("propDefs");
			}
			if (sharepointPropertyDefinition == null && schema.IdToPropertyMap.ContainsKey(documentLibraryPropertyDefinition.PropertyId))
			{
				sharepointPropertyDefinition = (schema.IdToPropertyMap[documentLibraryPropertyDefinition.PropertyId] as SharepointPropertyDefinition);
			}
			return sharepointPropertyDefinition;
		}

		internal static SharepointPropertyDefinition[] PropertyDefinitionsToSharepointPropertyDefinitions(Schema schema, ICollection<PropertyDefinition> propDefs)
		{
			List<SharepointPropertyDefinition> list = new List<SharepointPropertyDefinition>(propDefs.Count);
			foreach (PropertyDefinition propDef in propDefs)
			{
				list.Add(SharepointPropertyDefinition.PropertyDefinitionToSharepointPropertyDefinition(schema, propDef));
			}
			return list.ToArray();
		}

		internal SharepointPropertyDefinition(string displayName, Type propType, DocumentLibraryPropertyId propertyId, string name, SharepointFieldType fieldType, SharepointPropertyDefinition.MarshalTypeToSharepoint clrToSharepoint, SharepointPropertyDefinition.MarshalTypeFromSharepoint sharepointToClr, object defaultValue) : base(displayName, propType, defaultValue, propertyId)
		{
			this.name = name;
			this.fieldType = fieldType;
			this.clrToSharepoint = clrToSharepoint;
			this.sharepointToClr = sharepointToClr;
		}

		internal string SharepointName
		{
			get
			{
				return this.name;
			}
		}

		internal string FieldTypeAsString
		{
			get
			{
				return this.fieldType.ToString();
			}
		}

		internal virtual string BuildFieldRef()
		{
			throw new NotImplementedException();
		}

		internal SharepointPropertyDefinition.MarshalTypeToSharepoint ToSharepoint
		{
			get
			{
				return this.clrToSharepoint;
			}
		}

		internal SharepointPropertyDefinition.MarshalTypeFromSharepoint FromSharepoint
		{
			get
			{
				return this.sharepointToClr;
			}
		}

		private readonly string name;

		private readonly SharepointFieldType fieldType;

		private readonly SharepointPropertyDefinition.MarshalTypeToSharepoint clrToSharepoint;

		private readonly SharepointPropertyDefinition.MarshalTypeFromSharepoint sharepointToClr;

		internal delegate string MarshalTypeToSharepoint(object obj, CultureInfo cultureInfo);

		internal delegate object MarshalTypeFromSharepoint(string obj, CultureInfo cultureInfo);
	}
}
