using System;

namespace System.Reflection
{
	[Serializable]
	internal struct CustomAttributeRecord
	{
		internal ConstArray blob;

		internal MetadataToken tkCtor;
	}
}
