using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class MRSRequestSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition MailboxMoveStatus = SharedPropertyDefinitions.MailboxMoveStatus;

		public static readonly ADPropertyDefinition MailboxMoveFlags = SharedPropertyDefinitions.MailboxMoveFlags;

		public static readonly ADPropertyDefinition MailboxMoveBatchName = SharedPropertyDefinitions.MailboxMoveBatchName;

		public static readonly ADPropertyDefinition MailboxMoveRemoteHostName = SharedPropertyDefinitions.MailboxMoveRemoteHostName;

		public static readonly ADPropertyDefinition MailboxMoveSourceMDB = SharedPropertyDefinitions.MailboxMoveSourceMDB;

		public static readonly ADPropertyDefinition MailboxMoveTargetMDB = SharedPropertyDefinitions.MailboxMoveTargetMDB;

		public static readonly ADPropertyDefinition MailboxMoveFilePath = new ADPropertyDefinition("MailboxMoveFilePath", ExchangeObjectVersion.Exchange2003, typeof(string), "msExchMailboxMoveFilePath", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailboxMoveRequestGuid = new ADPropertyDefinition("MailboxMoveRequestGuid", ExchangeObjectVersion.Exchange2003, typeof(Guid), "msExchMailboxMoveRequestGuid", ADPropertyDefinitionFlags.Binary | ADPropertyDefinitionFlags.DoNotProvisionalClone, System.Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailboxMoveStorageMDB = new ADPropertyDefinition("MailboxMoveStorageMDB", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, "msExchMailboxMoveStorageMDBLink", null, "msExchMailboxMoveStorageMDBLinkSL", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, SimpleProviderPropertyDefinition.None, null, null, null, null, null);

		public static readonly ADPropertyDefinition MailboxMoveSourceUser = new ADPropertyDefinition("MailboxMoveSourceUser", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchMailboxMoveSourceUserLink", ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailboxMoveTargetUser = new ADPropertyDefinition("MailboxMoveTargetUser", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "msExchMailboxMoveTargetUserLink", ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MRSRequestType = new ADPropertyDefinition("MRSRequestType", ExchangeObjectVersion.Exchange2003, typeof(MRSRequestType), "msExchMRSRequestType", ADPropertyDefinitionFlags.None, Microsoft.Exchange.Data.Directory.SystemConfiguration.MRSRequestType.Move, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(MRSRequestType))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DisplayName = SharedPropertyDefinitions.MandatoryDisplayName;
	}
}
