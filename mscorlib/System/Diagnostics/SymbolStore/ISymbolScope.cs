using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore
{
	[ComVisible(true)]
	public interface ISymbolScope
	{
		ISymbolMethod Method { get; }

		ISymbolScope Parent { get; }

		ISymbolScope[] GetChildren();

		int StartOffset { get; }

		int EndOffset { get; }

		ISymbolVariable[] GetLocals();

		ISymbolNamespace[] GetNamespaces();
	}
}
