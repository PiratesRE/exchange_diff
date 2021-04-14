using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	internal sealed class AADUserPresentationObjectSchema : AADDirectoryObjectPresentationObjectSchema
	{
		public static readonly ProviderPropertyDefinition DisplayName = new SimplePropertyDefinition("DisplayName", typeof(string), null);

		public static readonly ProviderPropertyDefinition MailNickname = new SimplePropertyDefinition("MailNickname", typeof(string), null);
	}
}
