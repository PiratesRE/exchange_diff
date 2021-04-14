using System;
using System.Collections;

namespace System.Runtime.InteropServices.ComTypes
{
	[Guid("496B0ABE-CDEE-11d3-88E8-00902754C43A")]
	internal interface IEnumerable
	{
		[DispId(-4)]
		IEnumerator GetEnumerator();
	}
}
