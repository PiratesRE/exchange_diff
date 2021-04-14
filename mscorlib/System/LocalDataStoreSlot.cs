using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	public sealed class LocalDataStoreSlot
	{
		internal LocalDataStoreSlot(LocalDataStoreMgr mgr, int slot, long cookie)
		{
			this.m_mgr = mgr;
			this.m_slot = slot;
			this.m_cookie = cookie;
		}

		internal LocalDataStoreMgr Manager
		{
			get
			{
				return this.m_mgr;
			}
		}

		internal int Slot
		{
			get
			{
				return this.m_slot;
			}
		}

		internal long Cookie
		{
			get
			{
				return this.m_cookie;
			}
		}

		protected override void Finalize()
		{
			try
			{
				LocalDataStoreMgr mgr = this.m_mgr;
				if (mgr != null)
				{
					int slot = this.m_slot;
					this.m_slot = -1;
					mgr.FreeDataSlot(slot, this.m_cookie);
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		private LocalDataStoreMgr m_mgr;

		private int m_slot;

		private long m_cookie;
	}
}
