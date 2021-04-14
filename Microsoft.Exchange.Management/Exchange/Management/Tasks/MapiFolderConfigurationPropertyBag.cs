using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mapi;

namespace Microsoft.Exchange.Management.Tasks
{
	[Serializable]
	internal class MapiFolderConfigurationPropertyBag : PropertyBag
	{
		public MapiFolderConfigurationPropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
		}

		public MapiFolderConfigurationPropertyBag() : this(false, 16)
		{
		}

		internal override ProviderPropertyDefinition ObjectVersionPropertyDefinition
		{
			get
			{
				return MapiFolderConfigurationSchema.ExchangeVersion;
			}
		}

		internal override ProviderPropertyDefinition ObjectStatePropertyDefinition
		{
			get
			{
				return MapiFolderConfigurationSchema.ObjectState;
			}
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return MapiFolderConfigurationSchema.Identity;
			}
		}

		internal override object SerializeData(ProviderPropertyDefinition propertyDefinition, object input)
		{
			if (typeof(MapiObjectId) == propertyDefinition.Type)
			{
				return input;
			}
			if (typeof(ELCFolderIdParameter) == propertyDefinition.Type)
			{
				return input;
			}
			if (typeof(ByteQuantifiedSize) == propertyDefinition.Type)
			{
				return input;
			}
			return base.SerializeData(propertyDefinition, input);
		}

		internal override object DeserializeData(ProviderPropertyDefinition propertyDefinition, object input)
		{
			if (typeof(MapiObjectId) == propertyDefinition.Type)
			{
				return input;
			}
			if (typeof(ELCFolderIdParameter) == propertyDefinition.Type)
			{
				return input;
			}
			if (typeof(ByteQuantifiedSize) == propertyDefinition.Type)
			{
				return input;
			}
			return base.DeserializeData(propertyDefinition, input);
		}
	}
}
