using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore
{
	[ComVisible(true)]
	public interface ISymbolReader
	{
		ISymbolDocument GetDocument(string url, Guid language, Guid languageVendor, Guid documentType);

		ISymbolDocument[] GetDocuments();

		SymbolToken UserEntryPoint { get; }

		ISymbolMethod GetMethod(SymbolToken method);

		ISymbolMethod GetMethod(SymbolToken method, int version);

		ISymbolVariable[] GetVariables(SymbolToken parent);

		ISymbolVariable[] GetGlobalVariables();

		ISymbolMethod GetMethodFromDocumentPosition(ISymbolDocument document, int line, int column);

		byte[] GetSymAttribute(SymbolToken parent, string name);

		ISymbolNamespace[] GetNamespaces();
	}
}
