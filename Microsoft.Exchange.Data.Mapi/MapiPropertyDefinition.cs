using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	internal sealed class MapiPropertyDefinition : ProviderPropertyDefinition
	{
		public override bool IsMultivalued
		{
			get
			{
				return MapiPropertyDefinitionFlags.None != (MapiPropertyDefinitionFlags.MultiValued & this.propertyDefinitionFlags);
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return MapiPropertyDefinitionFlags.None != (MapiPropertyDefinitionFlags.ReadOnly & this.propertyDefinitionFlags);
			}
		}

		public override bool IsCalculated
		{
			get
			{
				return MapiPropertyDefinitionFlags.None != (MapiPropertyDefinitionFlags.Calculated & this.propertyDefinitionFlags);
			}
		}

		public override bool IsFilterOnly
		{
			get
			{
				return MapiPropertyDefinitionFlags.None != (MapiPropertyDefinitionFlags.FilterOnly & this.propertyDefinitionFlags);
			}
		}

		public override bool IsMandatory
		{
			get
			{
				return MapiPropertyDefinitionFlags.None != (MapiPropertyDefinitionFlags.Mandatory & this.propertyDefinitionFlags);
			}
		}

		public override bool PersistDefaultValue
		{
			get
			{
				return MapiPropertyDefinitionFlags.None != (MapiPropertyDefinitionFlags.PersistDefaultValue & this.propertyDefinitionFlags);
			}
		}

		public override bool IsWriteOnce
		{
			get
			{
				return MapiPropertyDefinitionFlags.None != (MapiPropertyDefinitionFlags.WriteOnce & this.propertyDefinitionFlags);
			}
		}

		public override bool IsBinary
		{
			get
			{
				return false;
			}
		}

		public PropTag PropertyTag
		{
			get
			{
				return this.propertyTag;
			}
		}

		public MapiPropertyDefinitionFlags PropertyDefinitionFlags
		{
			get
			{
				return this.propertyDefinitionFlags;
			}
		}

		public object InitialValue
		{
			get
			{
				return this.initialValue;
			}
		}

		public MapiPropValueExtractorDelegate Extractor
		{
			get
			{
				return this.propertyValueExtractor;
			}
		}

		public MapiPropValuePackerDelegate Packer
		{
			get
			{
				return this.propertyValuePacker;
			}
		}

		public MapiPropertyDefinition(string name, Type type, PropTag propertyTag, MapiPropertyDefinitionFlags propertyDefinitionFlags, object defaultValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints) : this(name, type, propertyTag, propertyDefinitionFlags, defaultValue, defaultValue, readConstraints, writeConstraints)
		{
		}

		public MapiPropertyDefinition(string name, Type type, PropTag propertyTag, MapiPropertyDefinitionFlags propertyDefinitionFlags, object defaultValue, object initialValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints) : this(name, type, propertyTag, propertyDefinitionFlags, defaultValue, initialValue, null, null, readConstraints, writeConstraints)
		{
		}

		public MapiPropertyDefinition(string name, Type type, PropTag propertyTag, MapiPropertyDefinitionFlags propertyDefinitionFlags, object defaultValue, MapiPropValueExtractorDelegate propertyValueExtractor, MapiPropValuePackerDelegate propertyValuePacker, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints) : this(name, type, propertyTag, propertyDefinitionFlags, defaultValue, defaultValue, propertyValueExtractor, propertyValuePacker, readConstraints, writeConstraints)
		{
		}

		public MapiPropertyDefinition(string name, Type type, PropTag propertyTag, MapiPropertyDefinitionFlags propertyDefinitionFlags, object defaultValue, object initialValue, MapiPropValueExtractorDelegate propertyValueExtractor, MapiPropValuePackerDelegate propertyValuePacker, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints) : this(name, type, propertyTag, propertyDefinitionFlags, defaultValue, initialValue, propertyValueExtractor, propertyValuePacker, readConstraints, writeConstraints, null, null, null, null)
		{
		}

		public MapiPropertyDefinition(string name, Type type, object defaultValue, MapiPropertyDefinitionFlags propertyDefinitionFlags, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints, ProviderPropertyDefinition[] supportingProperties, CustomFilterBuilderDelegate customFilterBuilderDelegate, GetterDelegate getterDelegate, SetterDelegate setterDelegate) : this(name, type, PropTag.Null, propertyDefinitionFlags, defaultValue, null, null, null, readConstraints, writeConstraints, supportingProperties, customFilterBuilderDelegate, getterDelegate, setterDelegate)
		{
		}

		private MapiPropertyDefinition(string name, Type type, PropTag propertyTag, MapiPropertyDefinitionFlags propertyDefinitionFlags, object defaultValue, object initialValue, MapiPropValueExtractorDelegate propertyValueExtractor, MapiPropValuePackerDelegate propertyValuePacker, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints, ProviderPropertyDefinition[] supportingProperties, CustomFilterBuilderDelegate customFilterBuilderDelegate, GetterDelegate getterDelegate, SetterDelegate setterDelegate) : base(name ?? propertyTag.ToString(), ExchangeObjectVersion.Exchange2003, type ?? MapiPropValueConvertor.TypeFromPropType(propertyTag.ValueType(), true), defaultValue, readConstraints ?? PropertyDefinitionConstraint.None, writeConstraints ?? PropertyDefinitionConstraint.None, supportingProperties ?? ProviderPropertyDefinition.None, customFilterBuilderDelegate, getterDelegate, setterDelegate)
		{
			this.propertyTag = propertyTag;
			if (((PropTag)12288U & propertyTag) != PropTag.Null)
			{
				propertyDefinitionFlags |= MapiPropertyDefinitionFlags.MultiValued;
			}
			this.propertyDefinitionFlags = propertyDefinitionFlags;
			this.initialValue = initialValue;
			this.propertyValueExtractor = (propertyValueExtractor ?? new MapiPropValueExtractorDelegate(MapiPropValueConvertor.Extract));
			this.propertyValuePacker = (propertyValuePacker ?? new MapiPropValuePackerDelegate(MapiPropValueConvertor.Pack));
		}

		private readonly PropTag propertyTag;

		private readonly MapiPropertyDefinitionFlags propertyDefinitionFlags;

		private readonly object initialValue;

		private readonly MapiPropValueExtractorDelegate propertyValueExtractor;

		private readonly MapiPropValuePackerDelegate propertyValuePacker;
	}
}
