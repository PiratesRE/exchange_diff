using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "MapiPropertyTypeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum MapiPropertyType
	{
		ApplicationTime = 7,
		ApplicationTimeArray = 4103,
		Binary = 258,
		BinaryArray = 4354,
		Boolean = 11,
		CLSID = 72,
		CLSIDArray = 4168,
		Currency = 6,
		CurrencyArray = 4102,
		Double = 5,
		DoubleArray = 4101,
		Error = 10,
		Float = 4,
		FloatArray = 4100,
		Integer = 3,
		IntegerArray = 4099,
		Long = 20,
		LongArray = 4116,
		Null = 1,
		Object = 13,
		ObjectArray = 4109,
		Short = 2,
		ShortArray = 4098,
		SystemTime = 64,
		SystemTimeArray = 4160,
		String = 31,
		StringArray = 4127
	}
}
