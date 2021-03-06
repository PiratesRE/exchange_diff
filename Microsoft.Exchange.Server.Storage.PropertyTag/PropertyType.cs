using System;

namespace Microsoft.Exchange.Server.Storage.PropTags
{
	public enum PropertyType : ushort
	{
		Unspecified,
		Null,
		Int16,
		Int32,
		Real32,
		Real64,
		Currency,
		AppTime,
		Error = 10,
		Boolean,
		Object = 13,
		Int64 = 20,
		String8 = 30,
		Unicode,
		SysTime = 64,
		Guid = 72,
		SvrEid = 251,
		SRestriction = 253,
		Actions,
		Binary = 258,
		Invalid = 4095,
		MVFlag,
		MVNull,
		MVInt16,
		MVInt32,
		MVReal32,
		MVInt64 = 4116,
		MVCurrency = 4102,
		MVReal64 = 4101,
		MVGuid = 4168,
		MVString8 = 4126,
		MVUnicode,
		MVBinary = 4354,
		MVAppTime = 4103,
		MVSysTime = 4160,
		MVInvalid = 8191,
		MVInstance,
		MVINull = 12289,
		MVIInt16,
		MVIInt32,
		MVIReal32,
		MVIInt64 = 12308,
		MVICurrency = 12294,
		MVIReal64 = 12293,
		MVIGuid = 12360,
		MVIString8 = 12318,
		MVIUnicode,
		MVIBinary = 12546,
		MVIAppTime = 12295,
		MVISysTime = 12352,
		MVIInvalid = 16383
	}
}
