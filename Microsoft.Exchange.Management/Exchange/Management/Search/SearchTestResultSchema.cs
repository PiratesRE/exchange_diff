using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Monitoring;

namespace Microsoft.Exchange.Management.Search
{
	internal class SearchTestResultSchema : SimpleProviderObjectSchema
	{
		public static SimpleProviderPropertyDefinition Mailbox = new SimpleProviderPropertyDefinition("Mailbox", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition MailboxGuid = new SimpleProviderPropertyDefinition("MailboxGuid", ExchangeObjectVersion.Exchange2007, typeof(Guid), PropertyDefinitionFlags.None, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition UserLegacyExchangeDN = new SimpleProviderPropertyDefinition("UserLegacyExchangeDN", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition Database = new SimpleProviderPropertyDefinition("Database", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition DatabaseGuid = new SimpleProviderPropertyDefinition("DatabaseGuid", ExchangeObjectVersion.Exchange2007, typeof(Guid), PropertyDefinitionFlags.None, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition ServerGuid = new SimpleProviderPropertyDefinition("ServerGuid", ExchangeObjectVersion.Exchange2007, typeof(Guid), PropertyDefinitionFlags.None, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition ResultFound = new SimpleProviderPropertyDefinition("ResultFound", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition SearchTimeInSeconds = new SimpleProviderPropertyDefinition("SearchTimeInSeconds", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition DetailEvents = new SimpleProviderPropertyDefinition("DetailEvents", ExchangeObjectVersion.Exchange2007, typeof(List<MonitoringEvent>), PropertyDefinitionFlags.None, new List<MonitoringEvent>(), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition Server = new SimpleProviderPropertyDefinition("Server", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition Error = new SimpleProviderPropertyDefinition("Error", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
