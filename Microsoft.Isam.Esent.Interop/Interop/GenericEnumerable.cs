using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Isam.Esent.Interop
{
	internal class GenericEnumerable<T> : IEnumerable<T>, IEnumerable
	{
		public GenericEnumerable(GenericEnumerable<T>.CreateEnumerator enumeratorCreator)
		{
			this.enumeratorCreator = enumeratorCreator;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.enumeratorCreator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly GenericEnumerable<T>.CreateEnumerator enumeratorCreator;

		public delegate IEnumerator<T> CreateEnumerator();
	}
}
