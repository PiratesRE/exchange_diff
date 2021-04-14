using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.NaturalLanguage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ExtractedNaturalLanguageProperty<TExtraction, TExtractionSet> : SmartPropertyDefinition where TExtractionSet : ExtractionSet<TExtraction>, new()
	{
		internal ExtractedNaturalLanguageProperty(string displayName, NativeStorePropertyDefinition xmlExtractedProperty) : base(displayName, typeof(TExtraction[]), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(xmlExtractedProperty, PropertyDependencyType.NeedForRead)
		})
		{
			this.xmlExtractedProperty = xmlExtractedProperty;
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object value = propertyBag.GetValue(this.xmlExtractedProperty);
			if (!(value is string))
			{
				return null;
			}
			object extractions;
			try
			{
				XmlSerializer serializer = SerializerCache.GetSerializer(typeof(TExtractionSet));
				TExtractionSet textractionSet;
				using (StringReader stringReader = new StringReader((string)value))
				{
					textractionSet = (serializer.Deserialize(stringReader) as TExtractionSet);
				}
				extractions = textractionSet.Extractions;
			}
			catch (InvalidOperationException innerException)
			{
				throw new CorruptDataException(ServerStrings.CorruptNaturalLanguageProperty, innerException);
			}
			return extractions;
		}

		private NativeStorePropertyDefinition xmlExtractedProperty;
	}
}
