using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore
{
	[ComVisible(true)]
	public interface ISymbolDocumentWriter
	{
		void SetSource(byte[] source);

		void SetCheckSum(Guid algorithmId, byte[] checkSum);
	}
}
