using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class JournalingReconciliationAccountSchema : ADConfigurationObjectSchema
	{
		private static readonly PropertyDefinitionConstraint[] ReconciliationUrlConstraints = new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute),
			new StringLengthConstraint(8, 2048)
		};

		public static readonly ADPropertyDefinition ArchiveUri = new ADPropertyDefinition("ArchiveUri", ExchangeObjectVersion.Exchange2010, typeof(Uri), "msExchJournalingReconciliationUrl", ADPropertyDefinitionFlags.None, null, JournalingReconciliationAccountSchema.ReconciliationUrlConstraints, JournalingReconciliationAccountSchema.ReconciliationUrlConstraints, null, null);

		public static readonly ADPropertyDefinition UserName = new ADPropertyDefinition("UserName", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchJournalingReconciliationUsername", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Password = new ADPropertyDefinition("Password", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchJournalingReconciliationPassword", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Mailboxes = new ADPropertyDefinition("Mailboxes", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchJournalingReconciliationMailboxes", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AuthenticationCredential = new ADPropertyDefinition("AuthenticationCredential", ExchangeObjectVersion.Exchange2010, typeof(PSCredential), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			JournalingReconciliationAccountSchema.UserName,
			JournalingReconciliationAccountSchema.Password
		}, null, (IPropertyBag propertyBag) => PSCredentialHelper.GetCredentialFromUserPass((string)propertyBag[JournalingReconciliationAccountSchema.UserName], (string)propertyBag[JournalingReconciliationAccountSchema.Password], false), delegate(object value, IPropertyBag propertyBag)
		{
			string value2 = null;
			string value3 = null;
			PSCredentialHelper.GetUserPassFromCredential((PSCredential)value, out value2, out value3, false);
			propertyBag[JournalingReconciliationAccountSchema.UserName] = value2;
			propertyBag[JournalingReconciliationAccountSchema.Password] = value3;
		}, null, null);
	}
}
