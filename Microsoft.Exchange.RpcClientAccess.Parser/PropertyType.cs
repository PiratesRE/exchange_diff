using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal enum PropertyType : ushort
	{
		[AlternativeName(AlternativeNaming.MAPI, "PT_UNSPECIFIED")]
		Unspecified,
		[AlternativeName(AlternativeNaming.MAPI, "PT_NULL")]
		Null,
		[AlternativeName(AlternativeNaming.MAPI, "PT_I2")]
		[AlternativeName(AlternativeNaming.MAPI, "PT_SHORT")]
		Int16,
		[AlternativeName(AlternativeNaming.MAPI, "PT_I4")]
		[AlternativeName(AlternativeNaming.MAPI, "PT_LONG")]
		Int32,
		[AlternativeName(AlternativeNaming.MAPI, "PT_FLOAT")]
		[AlternativeName(AlternativeNaming.MAPI, "PT_R4")]
		Float,
		[AlternativeName(AlternativeNaming.MAPI, "PT_R8")]
		[AlternativeName(AlternativeNaming.MAPI, "PT_DOULE")]
		Double,
		[AlternativeName(AlternativeNaming.MAPI, "PT_CURRENCY")]
		Currency,
		[AlternativeName(AlternativeNaming.MAPI, "PT_APPTIME")]
		AppTime,
		[AlternativeName(AlternativeNaming.MAPI, "PT_ERROR")]
		Error = 10,
		[AlternativeName(AlternativeNaming.MAPI, "PT_BOOLEAN")]
		Bool,
		[AlternativeName(AlternativeNaming.MAPI, "PT_OBJECT")]
		Object = 13,
		[AlternativeName(AlternativeNaming.MAPI, "PT_I8")]
		[AlternativeName(AlternativeNaming.MAPI, "PT_LONGLONG")]
		Int64 = 20,
		[AlternativeName(AlternativeNaming.MAPI, "PT_STRING8")]
		String8 = 30,
		[AlternativeName(AlternativeNaming.MAPI, "PT_UNICODE")]
		Unicode,
		[AlternativeName(AlternativeNaming.MAPI, "PT_SYSTIME")]
		SysTime = 64,
		[AlternativeName(AlternativeNaming.MAPI, "PT_CLSID")]
		Guid = 72,
		[AlternativeName(AlternativeNaming.MAPI, "PT_BINARY")]
		Binary = 258,
		[AlternativeName(AlternativeNaming.MAPI, "PT_RESTRICTION")]
		Restriction = 253,
		[AlternativeName(AlternativeNaming.MAPI, "PT_ACTIONS")]
		Actions,
		[AlternativeName(AlternativeNaming.MAPI, "PT_SVREID")]
		ServerId = 251,
		[AlternativeName(AlternativeNaming.MAPI, "PT_MV_I2")]
		[AlternativeName(AlternativeNaming.MAPI, "PT_MV_SHORT")]
		MultiValueInt16 = 4098,
		[AlternativeName(AlternativeNaming.MAPI, "PT_MV_LONG")]
		[AlternativeName(AlternativeNaming.MAPI, "PT_MV_I4")]
		MultiValueInt32,
		[AlternativeName(AlternativeNaming.MAPI, "PT_MV_FLOAT")]
		[AlternativeName(AlternativeNaming.MAPI, "PT_MV_R4")]
		MultiValueFloat,
		[AlternativeName(AlternativeNaming.MAPI, "PT_MV_DOUBLE")]
		[AlternativeName(AlternativeNaming.MAPI, "PT_MV_R8")]
		MultiValueDouble,
		[AlternativeName(AlternativeNaming.MAPI, "PT_MV_CURRENCY")]
		MultiValueCurrency,
		[AlternativeName(AlternativeNaming.MAPI, "PT_MV_APPTIME")]
		MultiValueAppTime,
		[AlternativeName(AlternativeNaming.MAPI, "PT_MV_SYSTIME")]
		MultiValueSysTime = 4160,
		[AlternativeName(AlternativeNaming.MAPI, "PT_MV_STRING8")]
		MultiValueString8 = 4126,
		[AlternativeName(AlternativeNaming.MAPI, "PT_MV_BINARY")]
		MultiValueBinary = 4354,
		[AlternativeName(AlternativeNaming.MAPI, "PT_MV_UNICODE")]
		MultiValueUnicode = 4127,
		[AlternativeName(AlternativeNaming.MAPI, "PT_MV_CLSID")]
		MultiValueGuid = 4168,
		[AlternativeName(AlternativeNaming.MAPI, "PT_MV_LONGLONG")]
		[AlternativeName(AlternativeNaming.MAPI, "PT_MV_I8")]
		MultiValueInt64 = 4116
	}
}
