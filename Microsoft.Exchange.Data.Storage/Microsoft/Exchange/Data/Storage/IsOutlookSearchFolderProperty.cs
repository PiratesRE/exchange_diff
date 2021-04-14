using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class IsOutlookSearchFolderProperty : SmartPropertyDefinition
	{
		internal IsOutlookSearchFolderProperty() : base("IsOutlookSearchFolderProperty", typeof(bool), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.ExtendedFolderFlagsInternal, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object obj = ExtendedFolderFlagsProperty.DecodeFolderFlags(propertyBag.GetValue(InternalSchema.ExtendedFolderFlagsInternal));
			ExtendedFolderFlagsProperty.ParsedFlags parsedFlags = obj as ExtendedFolderFlagsProperty.ParsedFlags;
			return parsedFlags != null && parsedFlags.ContainsKey(ExtendedFolderFlagsProperty.FlagTag.Clsid);
		}
	}
}
