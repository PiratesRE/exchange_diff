using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Sharing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WindowsLiveIdApplicationIdentitySchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition InstanceType = new SimpleProviderPropertyDefinition("InstanceType", ExchangeObjectVersion.Exchange2007, typeof(LiveIdInstanceType), PropertyDefinitionFlags.None, LiveIdInstanceType.Consumer, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Name = new SimpleProviderPropertyDefinition("Name", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Id = new SimpleProviderPropertyDefinition("Id", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Status = new SimpleProviderPropertyDefinition("Status", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition UriCollection = new SimpleProviderPropertyDefinition("URIs", ExchangeObjectVersion.Exchange2007, typeof(Uri), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition CertificateCollection = new SimpleProviderPropertyDefinition("Certificates", ExchangeObjectVersion.Exchange2007, typeof(WindowsLiveIdApplicationCertificate), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RawXml = new SimpleProviderPropertyDefinition("RawXml", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
