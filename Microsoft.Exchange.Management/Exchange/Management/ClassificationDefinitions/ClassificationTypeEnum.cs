using System;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Flags]
	public enum ClassificationTypeEnum : short
	{
		Entity = 1,
		Affinity = 2,
		Fingerprint = 3
	}
}
