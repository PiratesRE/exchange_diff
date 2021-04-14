using System;

namespace Microsoft.Isam.Esent.Interop
{
	public interface IContentEquatable<T>
	{
		bool ContentEquals(T other);
	}
}
