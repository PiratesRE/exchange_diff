using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Flags]
	internal enum ADPropertyDefinitionFlags
	{
		None = 0,
		ReadOnly = 1,
		MultiValued = 2,
		Calculated = 4,
		FilterOnly = 8,
		Mandatory = 16,
		PersistDefaultValue = 32,
		WriteOnce = 64,
		Binary = 128,
		TaskPopulated = 256,
		DoNotProvisionalClone = 512,
		ValidateInFirstOrganization = 1024,
		DoNotValidate = 2048,
		BackLink = 4096,
		Ranged = 8192,
		ValidateInSharedConfig = 16384,
		ForestSpecific = 32768,
		NonADProperty = 65536
	}
}
