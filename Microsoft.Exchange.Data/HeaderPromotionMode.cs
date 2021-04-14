using System;

namespace Microsoft.Exchange.Data
{
	public enum HeaderPromotionMode
	{
		[LocDescription(DataStrings.IDs.HeaderPromotionModeNoCreate)]
		NoCreate,
		[LocDescription(DataStrings.IDs.HeaderPromotionModeMayCreate)]
		MayCreate,
		[LocDescription(DataStrings.IDs.HeaderPromotionModeMustCreate)]
		MustCreate
	}
}
