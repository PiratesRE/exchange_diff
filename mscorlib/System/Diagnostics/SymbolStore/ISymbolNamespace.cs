using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore
{
	[ComVisible(true)]
	public interface ISymbolNamespace
	{
		string Name { get; }

		ISymbolNamespace[] GetNamespaces();

		ISymbolVariable[] GetVariables();
	}
}
