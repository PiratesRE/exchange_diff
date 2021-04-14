using System;
using System.Globalization;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ClientLanguageConstraint : PropertyDefinitionConstraint
	{
		public static bool IsSupportedCulture(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			CultureInfo[] installedLanguagePackCultures = LanguagePackInfo.GetInstalledLanguagePackCultures(LanguagePackType.Client);
			CultureInfo cultureInfo = Array.Find<CultureInfo>(installedLanguagePackCultures, delegate(CultureInfo installedCulture)
			{
				for (CultureInfo cultureInfo2 = culture; cultureInfo2 != null; cultureInfo2 = cultureInfo2.Parent)
				{
					if (installedCulture.LCID == cultureInfo2.LCID)
					{
						return true;
					}
					if (cultureInfo2.Parent == cultureInfo2)
					{
						break;
					}
				}
				return false;
			});
			return cultureInfo != null;
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (value != null)
			{
				ClientSupportedLanguage value2 = ((ClientSupportedLanguage?)value).Value;
				if (!ClientLanguageConstraint.IsSupportedCulture(CultureInfo.GetCultureInfo((int)value2)))
				{
					return new PropertyConstraintViolationError(ServerStrings.ErrorNotSupportedLanguageWithInstalledLanguagePack(value.ToString()), propertyDefinition, value, this);
				}
			}
			return null;
		}
	}
}
