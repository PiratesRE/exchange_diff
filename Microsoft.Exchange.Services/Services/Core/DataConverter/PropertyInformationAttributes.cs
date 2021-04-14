using System;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	[Flags]
	internal enum PropertyInformationAttributes
	{
		None = 0,
		ImplementsSetCommand = 2,
		ImplementsToXmlCommand = 4,
		ImplementsAppendUpdateCommand = 8,
		ImplementsDeleteUpdateCommand = 16,
		ImplementsSetUpdateCommand = 32,
		ImplementsToXmlForPropertyBagCommand = 64,
		ImplementsToServiceObjectCommand = 128,
		ImplementsToServiceObjectForPropertyBagCommand = 256,
		ImplementsReadOnlyCommands = 452
	}
}
