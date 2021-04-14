using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISyncWatermark : ICustomSerializableBuilder, ICustomSerializable, IComparable, ICloneable
	{
		bool IsNew { get; }
	}
}
