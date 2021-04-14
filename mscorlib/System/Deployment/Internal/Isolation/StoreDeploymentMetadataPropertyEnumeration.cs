using System;
using System.Collections;
using System.Security;

namespace System.Deployment.Internal.Isolation
{
	internal class StoreDeploymentMetadataPropertyEnumeration : IEnumerator
	{
		public StoreDeploymentMetadataPropertyEnumeration(IEnumSTORE_DEPLOYMENT_METADATA_PROPERTY pI)
		{
			this._enum = pI;
		}

		private StoreOperationMetadataProperty GetCurrent()
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

		public StoreOperationMetadataProperty Current
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
			StoreOperationMetadataProperty[] array = new StoreOperationMetadataProperty[1];
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

		private IEnumSTORE_DEPLOYMENT_METADATA_PROPERTY _enum;

		private bool _fValid;

		private StoreOperationMetadataProperty _current;
	}
}
