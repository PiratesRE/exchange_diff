using System;
using System.Collections;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
	internal class StoreSubcategoryEnumeration : IEnumerator
	{
		public StoreSubcategoryEnumeration(IEnumSTORE_CATEGORY_SUBCATEGORY pI)
		{
			this._enum = pI;
		}

		public IEnumerator GetEnumerator()
		{
			return this;
		}

		private STORE_CATEGORY_SUBCATEGORY GetCurrent()
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

		public STORE_CATEGORY_SUBCATEGORY Current
		{
			get
			{
				return this.GetCurrent();
			}
		}

		[SecuritySafeCritical]
		public bool MoveNext()
		{
			STORE_CATEGORY_SUBCATEGORY[] array = new STORE_CATEGORY_SUBCATEGORY[1];
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

		private IEnumSTORE_CATEGORY_SUBCATEGORY _enum;

		private bool _fValid;

		private STORE_CATEGORY_SUBCATEGORY _current;
	}
}
