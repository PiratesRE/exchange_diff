using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Guid("02b51929-c1c4-4a7e-8940-0312b5c18500")]
	[ComImport]
	internal interface IKeyValuePair<K, V>
	{
		K Key { get; }

		V Value { get; }
	}
}
