using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class ADMiniDomainSchema
	{
		public static readonly ADPropertyDefinition TenantIdProp = ADObjectSchema.OrganizationalUnitRoot;

		public static readonly ADPropertyDefinition DomainIdProp = ADObjectSchema.Id;

		public static readonly HygienePropertyDefinition ConfigurationIdProp = new HygienePropertyDefinition("configurationId", typeof(ADObjectId));

		public static readonly HygienePropertyDefinition ParentDomainIdProp = new HygienePropertyDefinition("parentDomainId", typeof(ADObjectId));

		public static readonly HygienePropertyDefinition DomainNameProp = new HygienePropertyDefinition("DomainName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition UsingMicrosoftMxProp = new HygienePropertyDefinition("usingMicrosoftMxRecords", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition IsCatchAllProp = new HygienePropertyDefinition("isCatchAll", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition IsInitialDomainProp = new HygienePropertyDefinition("isInitial", typeof(bool), false, ExchangeObjectVersion.Exchange2007, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition IsDefaultDomainProp = new HygienePropertyDefinition("isDefaultOutbound", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition EdgeBlockModeProp = new HygienePropertyDefinition("edgeBlockMode", typeof(EdgeBlockMode), EdgeBlockMode.None, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition HygieneConfigurationLink = new HygienePropertyDefinition("hygieneConfigurationLink", typeof(ADObjectId));

		public static readonly HygienePropertyDefinition MailServerProp = new HygienePropertyDefinition("MailServer", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition LiveTypeProp = new HygienePropertyDefinition("liveType", typeof(string));

		public static readonly HygienePropertyDefinition LiveNetIdProp = new HygienePropertyDefinition("liveNetId", typeof(string));

		public static readonly HygienePropertyDefinition DomainFlagsProperty = new HygienePropertyDefinition("Flags", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ObjectStateProp = DalHelper.ObjectStateProp;
	}
}
