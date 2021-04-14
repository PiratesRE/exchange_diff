using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore
{
	[ComVisible(true)]
	public interface ISymbolWriter
	{
		void Initialize(IntPtr emitter, string filename, bool fFullBuild);

		ISymbolDocumentWriter DefineDocument(string url, Guid language, Guid languageVendor, Guid documentType);

		void SetUserEntryPoint(SymbolToken entryMethod);

		void OpenMethod(SymbolToken method);

		void CloseMethod();

		void DefineSequencePoints(ISymbolDocumentWriter document, int[] offsets, int[] lines, int[] columns, int[] endLines, int[] endColumns);

		int OpenScope(int startOffset);

		void CloseScope(int endOffset);

		void SetScopeRange(int scopeID, int startOffset, int endOffset);

		void DefineLocalVariable(string name, FieldAttributes attributes, byte[] signature, SymAddressKind addrKind, int addr1, int addr2, int addr3, int startOffset, int endOffset);

		void DefineParameter(string name, ParameterAttributes attributes, int sequence, SymAddressKind addrKind, int addr1, int addr2, int addr3);

		void DefineField(SymbolToken parent, string name, FieldAttributes attributes, byte[] signature, SymAddressKind addrKind, int addr1, int addr2, int addr3);

		void DefineGlobalVariable(string name, FieldAttributes attributes, byte[] signature, SymAddressKind addrKind, int addr1, int addr2, int addr3);

		void Close();

		void SetSymAttribute(SymbolToken parent, string name, byte[] data);

		void OpenNamespace(string name);

		void CloseNamespace();

		void UsingNamespace(string fullName);

		void SetMethodSourceRange(ISymbolDocumentWriter startDoc, int startLine, int startColumn, ISymbolDocumentWriter endDoc, int endLine, int endColumn);

		void SetUnderlyingWriter(IntPtr underlyingWriter);
	}
}
