using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore
{
	[ComVisible(true)]
	public interface ISymbolBinder1
	{
		ISymbolReader GetReader(IntPtr importer, string filename, string searchPath);
	}
}
