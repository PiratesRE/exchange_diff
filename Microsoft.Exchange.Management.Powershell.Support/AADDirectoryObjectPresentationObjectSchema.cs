using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	internal class AADDirectoryObjectPresentationObjectSchema : ObjectSchema
	{
		public static readonly ProviderPropertyDefinition Members = new SimplePropertyDefinition("Members", typeof(AADDirectoryObjectPresentationObject[]), null);

		public static readonly ProviderPropertyDefinition ObjectId = new SimplePropertyDefinition("ObjectId", typeof(string), null);

		public static readonly ProviderPropertyDefinition ObjectType = new SimplePropertyDefinition("ObjectType", typeof(string), null);

		public static readonly ProviderPropertyDefinition Owners = new SimplePropertyDefinition("Owners", typeof(AADDirectoryObjectPresentationObject[]), null);
	}
}
