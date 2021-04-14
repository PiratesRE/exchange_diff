using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Management.Search
{
	internal class FailedDocumentSchema : SimpleProviderObjectSchema
	{
		public static SimpleProviderPropertyDefinition DocID = new SimpleProviderPropertyDefinition("DocID", ExchangeObjectVersion.Exchange2007, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition Database = new SimpleProviderPropertyDefinition("Database", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition MailboxGuid = new SimpleProviderPropertyDefinition("MailboxGuid", ExchangeObjectVersion.Exchange2007, typeof(Guid), PropertyDefinitionFlags.None, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition Mailbox = new SimpleProviderPropertyDefinition("Mailbox", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition SmtpAddress = new SimpleProviderPropertyDefinition("SmtpAddress", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition EntryID = new SimpleProviderPropertyDefinition("EntryID", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition Subject = new SimpleProviderPropertyDefinition("Subject", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition ErrorCode = new SimpleProviderPropertyDefinition("ErrorCode", ExchangeObjectVersion.Exchange2007, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition Description = new SimpleProviderPropertyDefinition("Description", ExchangeObjectVersion.Exchange2007, typeof(LocalizedString), PropertyDefinitionFlags.None, LocalizedString.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition IsPartialIndexed = new SimpleProviderPropertyDefinition("IsPartialIndexed", ExchangeObjectVersion.Exchange2007, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition FailedTime = new SimpleProviderPropertyDefinition("FailedTime", ExchangeObjectVersion.Exchange2007, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition AdditionalInfo = new SimpleProviderPropertyDefinition("AdditionalInfo", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition FailureMode = new SimpleProviderPropertyDefinition("FailureMode", ExchangeObjectVersion.Exchange2007, typeof(FailureMode), PropertyDefinitionFlags.None, Microsoft.Exchange.Search.Core.Abstraction.FailureMode.Permanent, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition AttemptCount = new SimpleProviderPropertyDefinition("AttemptCount", ExchangeObjectVersion.Exchange2007, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
