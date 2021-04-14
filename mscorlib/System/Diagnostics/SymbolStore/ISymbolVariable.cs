using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore
{
	[ComVisible(true)]
	public interface ISymbolVariable
	{
		string Name { get; }

		object Attributes { get; }

		byte[] GetSignature();

		SymAddressKind AddressKind { get; }

		int AddressField1 { get; }

		int AddressField2 { get; }

		int AddressField3 { get; }

		int StartOffset { get; }

		int EndOffset { get; }
	}
}
