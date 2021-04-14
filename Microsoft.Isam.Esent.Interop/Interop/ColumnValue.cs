using System;
using System.Collections.Generic;

namespace Microsoft.Isam.Esent.Interop
{
	public abstract class ColumnValue
	{
		protected ColumnValue()
		{
			this.ItagSequence = 1;
		}

		public JET_COLUMNID Columnid { get; set; }

		public abstract object ValueAsObject { get; }

		public SetColumnGrbit SetGrbit { get; set; }

		public RetrieveColumnGrbit RetrieveGrbit { get; set; }

		public int ItagSequence { get; set; }

		public JET_wrn Error { get; internal set; }

		public abstract int Length { get; }

		protected abstract int Size { get; }

		public abstract override string ToString();

		internal unsafe static void RetrieveColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues)
		{
			if (columnValues.Length > 1024)
			{
				throw new ArgumentOutOfRangeException("columnValues", columnValues.Length, "Too many column values");
			}
			byte[] array = null;
			NATIVE_RETRIEVECOLUMN* ptr = stackalloc NATIVE_RETRIEVECOLUMN[checked(unchecked((UIntPtr)columnValues.Length) * (UIntPtr)sizeof(NATIVE_RETRIEVECOLUMN))];
			try
			{
				array = Caches.ColumnCache.Allocate();
				try
				{
					fixed (byte* ptr2 = array)
					{
						byte* ptr3 = ptr2;
						int num = columnValues.Length;
						for (int i = 0; i < columnValues.Length; i++)
						{
							if (columnValues[i].Size != 0)
							{
								columnValues[i].MakeNativeRetrieveColumn(ref ptr[i]);
								ptr[i].pvData = new IntPtr((void*)ptr3);
								ptr[i].cbData = checked((uint)columnValues[i].Size);
								ptr3 += ptr[i].cbData;
								num--;
							}
						}
						if (num > 0)
						{
							int num4;
							checked
							{
								int num2 = (int)(unchecked((long)(ptr3 - ptr2)));
								int num3 = array.Length - num2;
								num4 = num3 / num;
							}
							for (int j = 0; j < columnValues.Length; j++)
							{
								if (columnValues[j].Size == 0)
								{
									columnValues[j].MakeNativeRetrieveColumn(ref ptr[j]);
									ptr[j].pvData = new IntPtr((void*)ptr3);
									ptr[j].cbData = checked((uint)num4);
									ptr3 += ptr[j].cbData;
								}
							}
						}
						Api.Check(Api.Impl.JetRetrieveColumns(sesid, tableid, ptr, columnValues.Length));
						for (int k = 0; k < columnValues.Length; k++)
						{
							columnValues[k].Error = (JET_wrn)ptr[k].err;
							columnValues[k].ItagSequence = (int)ptr[k].itagSequence;
						}
						for (int l = 0; l < columnValues.Length; l++)
						{
							if (ptr[l].err != 1006)
							{
								byte* ptr4 = (byte*)((void*)ptr[l].pvData);
								int startIndex = checked((int)(unchecked((long)(ptr4 - ptr2))));
								columnValues[l].GetValueFromBytes(array, startIndex, checked((int)unchecked(ptr[l]).cbActual), ptr[l].err);
							}
						}
					}
				}
				finally
				{
					byte* ptr2 = null;
				}
				ColumnValue.RetrieveTruncatedBuffers(sesid, tableid, columnValues, ptr);
			}
			finally
			{
				if (array != null)
				{
					Caches.ColumnCache.Free(ref array);
				}
			}
		}

		internal unsafe abstract int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i);

		internal unsafe int SetColumns(JET_SESID sesid, JET_TABLEID tableid, ColumnValue[] columnValues, NATIVE_SETCOLUMN* nativeColumns, int i, void* buffer, int bufferSize, bool hasValue)
		{
			this.MakeNativeSetColumn(ref nativeColumns[i]);
			if (hasValue)
			{
				nativeColumns[i].cbData = checked((uint)bufferSize);
				nativeColumns[i].pvData = new IntPtr(buffer);
				if (bufferSize == 0)
				{
					nativeColumns[i].grbit |= 32U;
				}
			}
			int result = (i == columnValues.Length - 1) ? Api.Impl.JetSetColumns(sesid, tableid, nativeColumns, columnValues.Length) : columnValues[i + 1].SetColumns(sesid, tableid, columnValues, nativeColumns, i + 1);
			this.Error = (JET_wrn)nativeColumns[i].err;
			return result;
		}

		protected abstract void GetValueFromBytes(byte[] value, int startIndex, int count, int err);

		private unsafe static void RetrieveTruncatedBuffers(JET_SESID sesid, JET_TABLEID tableid, IList<ColumnValue> columnValues, NATIVE_RETRIEVECOLUMN* nativeRetrievecolumns)
		{
			for (int i = 0; i < columnValues.Count; i++)
			{
				if (nativeRetrievecolumns[i].err == 1006)
				{
					byte[] array = new byte[nativeRetrievecolumns[i].cbActual];
					JET_RETINFO retinfo = new JET_RETINFO
					{
						itagSequence = columnValues[i].ItagSequence
					};
					int count;
					int num;
					fixed (byte* ptr = array)
					{
						num = Api.Impl.JetRetrieveColumn(sesid, tableid, columnValues[i].Columnid, new IntPtr((void*)ptr), array.Length, out count, columnValues[i].RetrieveGrbit, retinfo);
					}
					Api.Check(num);
					columnValues[i].Error = (JET_wrn)num;
					columnValues[i].GetValueFromBytes(array, 0, count, num);
				}
			}
		}

		private void MakeNativeSetColumn(ref NATIVE_SETCOLUMN setcolumn)
		{
			setcolumn.columnid = this.Columnid.Value;
			setcolumn.grbit = (uint)this.SetGrbit;
			setcolumn.itagSequence = checked((uint)this.ItagSequence);
		}

		private void MakeNativeRetrieveColumn(ref NATIVE_RETRIEVECOLUMN retrievecolumn)
		{
			retrievecolumn.columnid = this.Columnid.Value;
			retrievecolumn.grbit = (uint)this.RetrieveGrbit;
			retrievecolumn.itagSequence = checked((uint)this.ItagSequence);
		}
	}
}
