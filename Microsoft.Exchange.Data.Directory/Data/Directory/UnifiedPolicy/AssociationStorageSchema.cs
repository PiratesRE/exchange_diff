using System;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Data.Directory.UnifiedPolicy
{
	internal class AssociationStorageSchema : UnifiedPolicyStorageBaseSchema
	{
		public static ADPropertyDefinition Type = new ADPropertyDefinition("AssociationType", ExchangeObjectVersion.Exchange2012, typeof(AssociationType), "msExchIMAP4Settings", ADPropertyDefinitionFlags.None, AssociationType.SPSiteCollection, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition AllowOverride = new ADPropertyDefinition("AllowOverride", ExchangeObjectVersion.Exchange2012, typeof(bool), "msExchPOP3Settings", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition DefaultPolicyId = new ADPropertyDefinition("DefaultPolicyId", ExchangeObjectVersion.Exchange2012, typeof(Guid?), "msExchCanaryData1", ADPropertyDefinitionFlags.Binary, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition Scope = new ADPropertyDefinition("Scope", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchTenantCountry", ADPropertyDefinitionFlags.Mandatory, string.Empty, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 1024)
		}, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition PolicyIds = new ADPropertyDefinition("PolicyIds", ExchangeObjectVersion.Exchange2012, typeof(Guid), "msExchEwsWellKnownApplicationPolicies", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition Comments = new ADPropertyDefinition("Comments", ExchangeObjectVersion.Exchange2012, typeof(string), "msExchDefaultPublicFolderMailbox", ADPropertyDefinitionFlags.None, string.Empty, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1024)
		}, PropertyDefinitionConstraint.None, null, null);

		public static ADPropertyDefinition Description = new ADPropertyDefinition("Description", ExchangeObjectVersion.Exchange2012, typeof(string), "adminDescription", ADPropertyDefinitionFlags.None, string.Empty, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 1024)
		}, PropertyDefinitionConstraint.None, null, null);
	}
}
