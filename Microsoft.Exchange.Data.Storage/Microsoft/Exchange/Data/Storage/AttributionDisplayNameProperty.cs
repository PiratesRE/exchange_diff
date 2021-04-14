using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class AttributionDisplayNameProperty : SmartPropertyDefinition
	{
		internal AttributionDisplayNameProperty() : base("AttributionDisplayName", typeof(string), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, AttributionDisplayNameProperty.PropertyDependencies)
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			string text = propertyBag.GetValueOrDefault<string>(InternalSchema.PartnerNetworkId, null);
			if (string.IsNullOrEmpty(text))
			{
				text = WellKnownNetworkNames.Outlook;
			}
			else if (text == WellKnownNetworkNames.QuickContacts)
			{
				string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.ParentDisplayName, null);
				if ("{06967759-274D-40B2-A3EB-D7F9E73727D7}" != valueOrDefault)
				{
					text = valueOrDefault;
				}
				else
				{
					text = ServerStrings.PeopleQuickContactsAttributionDisplayName;
				}
			}
			else if (!WellKnownNetworkNames.IsWellKnownExternalNetworkName(text))
			{
				text = WellKnownNetworkNames.Outlook;
			}
			return text;
		}

		private static readonly PropertyDependency[] PropertyDependencies = new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.ParentDisplayName, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.PartnerNetworkId, PropertyDependencyType.NeedForRead)
		};
	}
}
