using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class DomainSettingsSchema : ADObjectSchema
	{
		public static readonly PropertyDefinition HygieneConfigurationLinkProp = new HygienePropertyDefinition("HygieneConfigurationLink", typeof(ADObjectId));

		public static readonly HygienePropertyDefinition EdgeBlockModeProp = new HygienePropertyDefinition("edgeBlockMode", typeof(EdgeBlockMode), EdgeBlockMode.None, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly PropertyDefinition MailServerProp = new HygienePropertyDefinition("MailServer", typeof(string));
	}
}
