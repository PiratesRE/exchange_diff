using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	internal sealed class AADGroupPresentationObjectSchema : AADDirectoryObjectPresentationObjectSchema
	{
		public static readonly ProviderPropertyDefinition AllowAccessTo = new SimplePropertyDefinition("AllowAccessTo", typeof(AADDirectoryObjectPresentationObject[]), null);

		public static readonly ProviderPropertyDefinition Description = new SimplePropertyDefinition("Description", typeof(string), null);

		public static readonly ProviderPropertyDefinition DisplayName = new SimplePropertyDefinition("DisplayName", typeof(string), null);

		public static readonly ProviderPropertyDefinition ExchangeResources = new SimplePropertyDefinition("ExchangeResources", typeof(string[]), null);

		public static readonly ProviderPropertyDefinition GroupType = new SimplePropertyDefinition("GroupType", typeof(string), null);

		public static readonly ProviderPropertyDefinition IsPublic = new SimplePropertyDefinition("IsPublic", typeof(bool?), null);

		public static readonly ProviderPropertyDefinition Mail = new SimplePropertyDefinition("Mail", typeof(string), null);

		public static readonly ProviderPropertyDefinition MailEnabled = new SimplePropertyDefinition("MailEnabled", typeof(bool?), null);

		public static readonly ProviderPropertyDefinition PendingMembers = new SimplePropertyDefinition("PendingMembers", typeof(AADDirectoryObjectPresentationObject[]), null);

		public static readonly ProviderPropertyDefinition ProxyAddresses = new SimplePropertyDefinition("ProxyAddresses", typeof(string[]), null);

		public static readonly ProviderPropertyDefinition SecurityEnabled = new SimplePropertyDefinition("SecurityEnabled", typeof(bool?), null);

		public static readonly ProviderPropertyDefinition SharePointResources = new SimplePropertyDefinition("SharePointResources", typeof(string[]), null);
	}
}
