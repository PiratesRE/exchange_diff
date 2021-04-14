using System;
using System.Collections;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
	internal class StoreAssemblyEnumeration : IEnumerator
	{
		public StoreAssemblyEnumeration(IEnumSTORE_ASSEMBLY pI)
		{
			this._enum = pI;
		}

		private STORE_ASSEMBLY GetCurrent()
		{
			if (!this._fValid)
			{
				throw new InvalidOperationException();
			}
			return this._current;
		}

		object IEnumerator.Current
		{
			get
			{
				return this.GetCurrent();
			}
		}

		public STORE_ASSEMBLY Current
		{
			get
			{
				return this.GetCurrent();
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this;
		}

		[SecuritySafeCritical]
		public bool MoveNext()
		{
			STORE_ASSEMBLY[] array = new STORE_ASSEMBLY[1];
			uint num = this._enum.Next(1U, array);
			if (num == 1U)
			{
				this._current = array[0];
			}
			return this._fValid = (num == 1U);
		}

		[SecuritySafeCritical]
		public void Reset()
		{
			this._fValid = false;
			this._enum.Reset();
		}

		private IEnumSTORE_ASSEMBLY _enum;

		private bool _fValid;

		private STORE_ASSEMBLY _current;
	}
}
