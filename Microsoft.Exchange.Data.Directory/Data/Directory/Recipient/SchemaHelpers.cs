using System;
using System.Resources;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal static class SchemaHelpers
	{
		internal static object RoleGroupDescriptionGetter(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[ADGroupSchema.LocalizationFlags];
			int num2 = (int)propertyBag[ADGroupSchema.RoleGroupTypeId];
			if ((num & 1) == 0 && num2 != 0 && RoleGroup.RoleGroupStringIds.ContainsKey(num2))
			{
				string text = null;
				try
				{
					text = SchemaHelpers.resourceManager.GetString(RoleGroup.RoleGroupStringIds[num2]);
				}
				catch (MissingManifestResourceException)
				{
				}
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.Description];
			if (multiValuedProperty.Count <= 0)
			{
				return string.Empty;
			}
			return multiValuedProperty[0];
		}

		internal static void RoleGroupDescriptionSetter(object value, IPropertyBag propertyBag)
		{
			MultiValuedProperty<string> value2 = (value == null) ? null : new MultiValuedProperty<string>(value);
			propertyBag[ADRecipientSchema.Description] = value2;
			propertyBag[ADGroupSchema.LocalizationFlags] = ((int)propertyBag[ADGroupSchema.LocalizationFlags] | 1);
		}

		private static ExchangeResourceManager resourceManager = ExchangeResourceManager.GetResourceManager(typeof(CoreStrings).FullName, typeof(CoreStrings).Assembly);
	}
}
