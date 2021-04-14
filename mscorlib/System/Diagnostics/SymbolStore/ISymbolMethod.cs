using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore
{
	[ComVisible(true)]
	public interface ISymbolMethod
	{
		SymbolToken Token { get; }

		int SequencePointCount { get; }

		void GetSequencePoints(int[] offsets, ISymbolDocument[] documents, int[] lines, int[] columns, int[] endLines, int[] endColumns);

		ISymbolScope RootScope { get; }

		ISymbolScope GetScope(int offset);

		int GetOffset(ISymbolDocument document, int line, int column);

		int[] GetRanges(ISymbolDocument document, int line, int column);

		ISymbolVariable[] GetParameters();

		ISymbolNamespace GetNamespace();

		bool GetSourceStartEnd(ISymbolDocument[] docs, int[] lines, int[] columns);
	}
}
