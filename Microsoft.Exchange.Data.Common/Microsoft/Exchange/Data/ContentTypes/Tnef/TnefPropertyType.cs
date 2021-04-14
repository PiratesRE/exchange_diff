using System;

namespace Microsoft.Exchange.Data.ContentTypes.Tnef
{
	public enum TnefPropertyType : short
	{
		Unspecified,
		Null,
		I2,
		Long,
		R4,
		Double,
		Currency,
		AppTime,
		Error = 10,
		Boolean,
		Object = 13,
		I8 = 20,
		String8 = 30,
		Unicode,
		SysTime = 64,
		ClassId = 72,
		Binary = 258,
		MultiValued = 4096
	}
}
