using System;

namespace Microsoft.Isam.Esent.Interop
{
	public interface IDeepCloneable<T>
	{
		T DeepClone();
	}
}
