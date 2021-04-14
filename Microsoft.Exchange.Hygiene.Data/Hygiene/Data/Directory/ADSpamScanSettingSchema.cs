using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class ADSpamScanSettingSchema : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition ConfigurationIdProp = new HygienePropertyDefinition("configId", typeof(ADObjectId));

		public static readonly HygienePropertyDefinition ActionTypeIdProp = new HygienePropertyDefinition("actionTypeId", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ParameterProp = new HygienePropertyDefinition("parameter", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CsfmImageProp = new HygienePropertyDefinition("csfmImage", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CsfmEmptyProp = new HygienePropertyDefinition("csfmEmpty", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CsfmScriptProp = new HygienePropertyDefinition("csfmScript", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CsfmIframeProp = new HygienePropertyDefinition("csfmIframe", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CsfmObjectProp = new HygienePropertyDefinition("csfmObject", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CsfmEmbedProp = new HygienePropertyDefinition("csfmEmbed", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CsfmWebBugsProp = new HygienePropertyDefinition("csfmWebBugs", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CsfmFormProp = new HygienePropertyDefinition("csfmForm", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CsfmWordListProp = new HygienePropertyDefinition("csfmWordList", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CsfmUrlNumericIPProp = new HygienePropertyDefinition("csfmUrlNumericIP", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CsfmUrlRedirectProp = new HygienePropertyDefinition("csfmUrlRedirect", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CsfmWebsiteProp = new HygienePropertyDefinition("csfmWebsite", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CsfmSpfFailProp = new HygienePropertyDefinition("csfmSpfFail", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CsfmSpfFromFailProp = new HygienePropertyDefinition("csfmSpfFromFail", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CsfmNdrBackScatterProp = new HygienePropertyDefinition("csfmNdrBackscatter", typeof(byte), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition FlagsProp = new HygienePropertyDefinition("flags", typeof(SpamScanFlags), SpamScanFlags.AllowUserOptOut, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CsfmTestBccAddressProp = new HygienePropertyDefinition("csfmTestBccAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
