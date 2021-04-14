using System;
using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public sealed class FilterValueCollection : ICollection, IEnumerable
	{
		internal FilterValueCollection(DataListView owner)
		{
			this.owner = owner;
		}

		public string this[int columnIndex]
		{
			get
			{
				if (!this.owner.IsHandleCreated)
				{
					return "";
				}
				NativeMethods.HDTEXTFILTER hdtextfilter = default(NativeMethods.HDTEXTFILTER);
				hdtextfilter.pszText = new string(new char[256 * Marshal.SystemDefaultCharSize]);
				hdtextfilter.cchTextMax = 256;
				IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(hdtextfilter));
				string pszText;
				try
				{
					Marshal.StructureToPtr(hdtextfilter, intPtr, false);
					InternalNativeMethods.HDITEM hditem = default(InternalNativeMethods.HDITEM);
					hditem.mask = 256U;
					hditem.pvFilter = intPtr;
					InternalUnsafeNativeMethods.SendMessage(this.owner.HeaderHandle, NativeMethods.HDM_GETITEM, (IntPtr)columnIndex, ref hditem);
					pszText = ((NativeMethods.HDTEXTFILTER)Marshal.PtrToStructure(intPtr, typeof(NativeMethods.HDTEXTFILTER))).pszText;
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
				return pszText;
			}
			set
			{
				NativeMethods.HDTEXTFILTER hdtextfilter = default(NativeMethods.HDTEXTFILTER);
				hdtextfilter.pszText = value;
				hdtextfilter.cchTextMax = value.Length + 1;
				IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(hdtextfilter));
				try
				{
					Marshal.StructureToPtr(hdtextfilter, intPtr, false);
					InternalNativeMethods.HDITEM hditem = default(InternalNativeMethods.HDITEM);
					hditem.mask = 256U;
					hditem.type = 0U;
					hditem.pvFilter = intPtr;
					InternalUnsafeNativeMethods.SendMessage(this.owner.HeaderHandle, NativeMethods.HDM_SETITEM, (IntPtr)columnIndex, ref hditem);
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
		}

		public void CopyTo(Array array, int index)
		{
			for (int i = 0; i < this.Count; i++)
			{
				array.SetValue(this[i], index + i);
			}
		}

		public int Count
		{
			get
			{
				return this.owner.Columns.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return ((ICollection)this.owner.Columns).SyncRoot;
			}
		}

		public IEnumerator GetEnumerator()
		{
			string[] array = new string[this.Count];
			this.CopyTo(array, 0);
			return array.GetEnumerator();
		}

		private DataListView owner;
	}
}
