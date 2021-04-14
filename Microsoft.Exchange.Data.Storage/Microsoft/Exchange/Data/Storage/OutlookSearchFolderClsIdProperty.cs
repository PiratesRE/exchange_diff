using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class OutlookSearchFolderClsIdProperty : SmartPropertyDefinition
	{
		internal OutlookSearchFolderClsIdProperty() : base("OutlookSearchFolderClsIdProperty", typeof(Guid), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.ExtendedFolderFlagsInternal, PropertyDependencyType.AllRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object obj = ExtendedFolderFlagsProperty.DecodeFolderFlags(propertyBag.GetValue(InternalSchema.ExtendedFolderFlagsInternal));
			ExtendedFolderFlagsProperty.ParsedFlags parsedFlags = obj as ExtendedFolderFlagsProperty.ParsedFlags;
			if (parsedFlags != null)
			{
				byte[] b;
				if (parsedFlags.TryGetValue(ExtendedFolderFlagsProperty.FlagTag.Clsid, out b))
				{
					try
					{
						return new Guid(b);
					}
					catch (ArgumentException)
					{
						return new PropertyError(this, PropertyErrorCode.CorruptedData);
					}
				}
				return new PropertyError(this, PropertyErrorCode.NotFound);
			}
			return obj;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			byte[] value2 = ((Guid)value).ToByteArray();
			ExtendedFolderFlagsProperty.ParsedFlags parsedFlags = ExtendedFolderFlagsProperty.DecodeFolderFlags(propertyBag.GetValue(InternalSchema.ExtendedFolderFlagsInternal)) as ExtendedFolderFlagsProperty.ParsedFlags;
			if (parsedFlags == null)
			{
				parsedFlags = new ExtendedFolderFlagsProperty.ParsedFlags();
			}
			parsedFlags[ExtendedFolderFlagsProperty.FlagTag.Clsid] = value2;
			propertyBag.SetValueWithFixup(InternalSchema.ExtendedFolderFlagsInternal, ExtendedFolderFlagsProperty.EncodeFolderFlags(parsedFlags));
		}
	}
}
