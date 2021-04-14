using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Nspi
{
	public class NspiState
	{
		public NspiState()
		{
		}

		public NspiState(int sortType, int containerId, int currentRecord, int delta, int position, int totalRecords, int codePage, int templateLocale, int sortLocale)
		{
			this.sortType = sortType;
			this.containerId = containerId;
			this.currentRecord = currentRecord;
			this.delta = delta;
			this.position = position;
			this.totalRecords = totalRecords;
			this.codePage = codePage;
			this.templateLocale = templateLocale;
			this.sortLocale = sortLocale;
		}

		public NspiState(IntPtr src)
		{
			this.MarshalFromNative(src);
		}

		public SortIndex SortIndex
		{
			get
			{
				return (SortIndex)this.sortType;
			}
			set
			{
				this.sortType = (int)value;
			}
		}

		public int SortType
		{
			get
			{
				return this.sortType;
			}
			set
			{
				this.sortType = value;
			}
		}

		public int ContainerId
		{
			get
			{
				return this.containerId;
			}
			set
			{
				this.containerId = value;
			}
		}

		public int CurrentRecord
		{
			get
			{
				return this.currentRecord;
			}
			set
			{
				this.currentRecord = value;
			}
		}

		public int Delta
		{
			get
			{
				return this.delta;
			}
			set
			{
				this.delta = value;
			}
		}

		public int Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.position = value;
			}
		}

		public int TotalRecords
		{
			get
			{
				return this.totalRecords;
			}
			set
			{
				this.totalRecords = value;
			}
		}

		public int CodePage
		{
			get
			{
				return this.codePage;
			}
			set
			{
				this.codePage = value;
			}
		}

		public int TemplateLocale
		{
			get
			{
				return this.templateLocale;
			}
			set
			{
				this.templateLocale = value;
			}
		}

		public int SortLocale
		{
			get
			{
				return this.sortLocale;
			}
			set
			{
				this.sortLocale = value;
			}
		}

		internal NspiState Clone()
		{
			return (NspiState)base.MemberwiseClone();
		}

		internal int GetBytesToMarshal()
		{
			return 36;
		}

		internal void MarshalToNative(IntPtr dst)
		{
			Marshal.WriteInt32(dst, 0, this.sortType);
			Marshal.WriteInt32(dst, 4, this.containerId);
			Marshal.WriteInt32(dst, 8, this.currentRecord);
			Marshal.WriteInt32(dst, 12, this.delta);
			Marshal.WriteInt32(dst, 16, this.position);
			Marshal.WriteInt32(dst, 20, this.totalRecords);
			Marshal.WriteInt32(dst, 24, this.codePage);
			Marshal.WriteInt32(dst, 28, this.templateLocale);
			Marshal.WriteInt32(dst, 32, this.sortLocale);
		}

		internal void MarshalFromNative(IntPtr src)
		{
			this.sortType = Marshal.ReadInt32(src, 0);
			this.containerId = Marshal.ReadInt32(src, 4);
			this.currentRecord = Marshal.ReadInt32(src, 8);
			this.delta = Marshal.ReadInt32(src, 12);
			this.position = Marshal.ReadInt32(src, 16);
			this.totalRecords = Marshal.ReadInt32(src, 20);
			this.codePage = Marshal.ReadInt32(src, 24);
			this.templateLocale = Marshal.ReadInt32(src, 28);
			this.sortLocale = Marshal.ReadInt32(src, 32);
		}

		internal const int SizeOf = 36;

		private int sortType;

		private int containerId;

		private int currentRecord;

		private int delta;

		private int position;

		private int totalRecords;

		private int codePage;

		private int templateLocale;

		private int sortLocale;
	}
}
