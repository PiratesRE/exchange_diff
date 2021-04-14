using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[Guid("036d2c08-df29-41af-8aa2-d774be62ba6f")]
	[ComImport]
	internal interface IBindableIterable
	{
		IBindableIterator First();
	}
}
