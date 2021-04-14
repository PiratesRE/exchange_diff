using System;

namespace Microsoft.Exchange.Common
{
	public enum ExtendedTypeCode : byte
	{
		Invalid,
		Boolean,
		Int16,
		Int32,
		Int64,
		Single,
		Double,
		DateTime,
		Guid,
		String,
		Binary,
		MVFlag = 16,
		MVInt16 = 18,
		MVInt32,
		MVInt64,
		MVSingle,
		MVDouble,
		MVDateTime,
		MVGuid,
		MVString,
		MVBinary
	}
}
