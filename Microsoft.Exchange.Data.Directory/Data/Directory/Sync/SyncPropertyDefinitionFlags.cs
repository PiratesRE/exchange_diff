using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[Flags]
	public enum SyncPropertyDefinitionFlags
	{
		Ignore = 1048576,
		ForwardSync = 2097152,
		BackSync = 4194304,
		TwoWay = 6291456,
		Immutable = 8388608,
		Shadow = 16777216,
		Cloud = 33554432,
		AlwaysReturned = 67108864,
		NotInMsoDirectory = 134217728,
		FilteringOnly = 268435456,
		TaskPopulated = 256,
		MultiValued = 2,
		Calculated = 4,
		ReadOnly = 1,
		PersistDefaultValue = 32
	}
}
