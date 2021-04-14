using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore
{
	[ComVisible(true)]
	public interface ISymbolDocument
	{
		string URL { get; }

		Guid DocumentType { get; }

		Guid Language { get; }

		Guid LanguageVendor { get; }

		Guid CheckSumAlgorithmId { get; }

		byte[] GetCheckSum();

		int FindClosestLine(int line);

		bool HasEmbeddedSource { get; }

		int SourceLength { get; }

		byte[] GetSourceRange(int startLine, int startColumn, int endLine, int endColumn);
	}
}
