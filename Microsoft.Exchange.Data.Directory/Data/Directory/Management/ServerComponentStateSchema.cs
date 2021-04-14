using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class ServerComponentStateSchema : SimpleProviderObjectSchema
	{
		public static readonly ADPropertyDefinition ComponentStates = ServerSchema.ComponentStates;
	}
}
