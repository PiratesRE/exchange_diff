using System;

namespace System
{
	internal sealed class LocalDataStoreHolder
	{
		public LocalDataStoreHolder(LocalDataStore store)
		{
			this.m_Store = store;
		}

		protected override void Finalize()
		{
			try
			{
				LocalDataStore store = this.m_Store;
				if (store != null)
				{
					store.Dispose();
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		public LocalDataStore Store
		{
			get
			{
				return this.m_Store;
			}
		}

		private LocalDataStore m_Store;
	}
}
