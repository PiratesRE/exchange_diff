using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal abstract class LocalizedInfoValidator : IClassificationRuleCollectionValidator
	{
		protected static bool ValidateResourceLangCodes(IEnumerable<string> langCodes, out IList<string> invalidLangCodes, out IList<string> nonUniqueLangCodes)
		{
			ExAssert.RetailAssert(langCodes != null, "The langcode values to be validated must be specified!");
			invalidLangCodes = new List<string>();
			nonUniqueLangCodes = new List<string>();
			HashSet<CultureInfo> hashSet = new HashSet<CultureInfo>();
			foreach (string text in langCodes)
			{
				try
				{
					CultureInfo item = new CultureInfo(text);
					if (!hashSet.Add(item))
					{
						nonUniqueLangCodes.Add(text);
					}
				}
				catch (CultureNotFoundException)
				{
					invalidLangCodes.Add(text);
				}
			}
			return invalidLangCodes.Count == 0 && 0 == nonUniqueLangCodes.Count;
		}

		protected void ValidateResourceLangCodes(IEnumerable<string> langCodes, Func<IEnumerable<string>, LocalizedString> invalidLangCodesExceptionMessageBuilder, Func<IEnumerable<string>, LocalizedString> nonUniqueLangCodesExceptionMessageBuilder)
		{
			ExAssert.RetailAssert(langCodes != null, "The langcode(s) passed to ValidateResourceLangCodes must not be null!");
			ExAssert.RetailAssert(invalidLangCodesExceptionMessageBuilder != null, "The invalid langcode(s) exception message builder passed to ValidateResourceLangCodes must not be null!");
			ExAssert.RetailAssert(nonUniqueLangCodesExceptionMessageBuilder != null, "The non-unique langcode(s) exception message builder passed to ValidateResourceLangCodes must not be null!");
			IList<string> list;
			IList<string> list2;
			if (!LocalizedInfoValidator.ValidateResourceLangCodes(langCodes, out list, out list2))
			{
				if (list.Count > 0)
				{
					LocalizedString localizedExceptionMessage = invalidLangCodesExceptionMessageBuilder(from invalidLangCode in list
					select string.Format("\"{0}\"", invalidLangCode));
					throw ClassificationDefinitionUtils.PopulateExceptionSource<LocalizedException, IList<string>>(this.CreateInvalidLangCodeException(localizedExceptionMessage), list);
				}
				if (list2.Count > 0)
				{
					LocalizedString localizedExceptionMessage2 = nonUniqueLangCodesExceptionMessageBuilder(list2);
					throw ClassificationDefinitionUtils.PopulateExceptionSource<LocalizedException, IList<string>>(this.CreateNonUniqueLangCodeException(localizedExceptionMessage2), list2);
				}
			}
		}

		private static LocalizedException CreateDefaultValidationException(LocalizedString localizedExceptionMessage)
		{
			return new ClassificationRuleCollectionLocalizationInfoValidationException(localizedExceptionMessage);
		}

		protected virtual LocalizedException CreateInvalidLangCodeException(LocalizedString localizedExceptionMessage)
		{
			return LocalizedInfoValidator.CreateDefaultValidationException(localizedExceptionMessage);
		}

		protected virtual LocalizedException CreateNonUniqueLangCodeException(LocalizedString localizedExceptionMessage)
		{
			return LocalizedInfoValidator.CreateDefaultValidationException(localizedExceptionMessage);
		}

		protected abstract void InternalValidate(XDocument rulePackXDocument);

		public void Validate(ValidationContext context, XDocument rulePackXDocument)
		{
			this.InternalValidate(rulePackXDocument);
		}
	}
}
