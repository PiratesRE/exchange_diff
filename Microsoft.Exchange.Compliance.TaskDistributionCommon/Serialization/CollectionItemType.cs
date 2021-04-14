using System;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization
{
	public enum CollectionItemType : byte
	{
		NotDefined,
		Short,
		Int,
		Long,
		Double,
		Guid,
		String,
		Blob
	}
}
