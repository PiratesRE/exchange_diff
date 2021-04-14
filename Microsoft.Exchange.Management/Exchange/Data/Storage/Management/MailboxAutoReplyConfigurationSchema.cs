using System;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.InfoWorker.Common.OOF;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxAutoReplyConfigurationSchema : XsoMailboxConfigurationObjectSchema
	{
		public static readonly SimplePropertyDefinition AutoReplyState = new SimplePropertyDefinition("AutoReplyState", ExchangeObjectVersion.Exchange2007, typeof(OofState), PropertyDefinitionFlags.None, OofState.Disabled, OofState.Disabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition EndTime = new SimplePropertyDefinition("EndTime", ExchangeObjectVersion.Exchange2003, typeof(DateTime), PropertyDefinitionFlags.None, DateTime.MinValue, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition ExternalAudience = new SimplePropertyDefinition("ExternalAudience", ExchangeObjectVersion.Exchange2007, typeof(ExternalAudience), PropertyDefinitionFlags.None, Microsoft.Exchange.InfoWorker.Common.OOF.ExternalAudience.None, Microsoft.Exchange.InfoWorker.Common.OOF.ExternalAudience.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition ExternalMessage = new SimplePropertyDefinition("ExternalMessage", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 128000)
		});

		public static readonly SimpleProviderPropertyDefinition Identity = XsoMailboxConfigurationObjectSchema.MailboxOwnerId;

		public static readonly SimplePropertyDefinition InternalMessage = new SimplePropertyDefinition("InternalMessage", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, string.Empty, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 128000)
		});

		public static readonly SimplePropertyDefinition StartTime = new SimplePropertyDefinition("StartTime", ExchangeObjectVersion.Exchange2003, typeof(DateTime), PropertyDefinitionFlags.None, DateTime.MinValue, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
