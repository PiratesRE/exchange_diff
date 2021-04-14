using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Misc
{
	[StructLayout(LayoutKind.Explicit, Pack = 8)]
	internal struct PROPVARIANT : IDisposable
	{
		public object Value
		{
			get
			{
				object value = this.GetValue();
				if (value == null)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unknown or unsupported type {0}", new object[]
					{
						this.vt
					}));
				}
				return value;
			}
		}

		public void Dispose()
		{
			GraphicsInteropNativeMethods.PropVariantClear(ref this);
		}

		public object GetValue()
		{
			object result = null;
			PROPID propid = this.vt;
			if (propid <= PROPID.VT_LPWSTR)
			{
				switch (propid)
				{
				case PROPID.VT_I2:
					result = this.iVal;
					break;
				case PROPID.VT_I4:
					result = this.lVal;
					break;
				case PROPID.VT_R4:
					result = this.fltVal;
					break;
				case PROPID.VT_R8:
					result = this.dblVal;
					break;
				case PROPID.VT_CY:
				case PROPID.VT_DATE:
				case PROPID.VT_BSTR:
				case PROPID.VT_DISPATCH:
				case PROPID.VT_VARIANT:
				case PROPID.VT_DECIMAL:
				case PROPID.VT_NULL | PROPID.VT_I2 | PROPID.VT_R4 | PROPID.VT_BSTR:
					break;
				case PROPID.VT_ERROR:
					result = this.scode;
					break;
				case PROPID.VT_BOOL:
					result = (0 != this.boolVal);
					break;
				case PROPID.VT_UNKNOWN:
					result = Marshal.GetObjectForIUnknown(this.ptr);
					break;
				case PROPID.VT_I1:
					result = this.cVal;
					break;
				case PROPID.VT_UI1:
					result = this.bVal;
					break;
				case PROPID.VT_UI2:
					result = this.uiVal;
					break;
				case PROPID.VT_UI4:
					result = this.ulVal;
					break;
				case PROPID.VT_I8:
					result = this.hVal;
					break;
				case PROPID.VT_UI8:
					result = this.uhVal;
					break;
				default:
					switch (propid)
					{
					case PROPID.VT_LPSTR:
						result = Marshal.PtrToStringAnsi(this.ptr);
						break;
					case PROPID.VT_LPWSTR:
						result = Marshal.PtrToStringUni(this.ptr);
						break;
					}
					break;
				}
			}
			else if (propid != PROPID.VT_FILETIME)
			{
				if (propid == PROPID.VT_CLSID)
				{
					result = Marshal.PtrToStructure(this.ptr, typeof(Guid));
				}
			}
			else
			{
				result = PROPVARIANT.FileTimeStart.Add(new TimeSpan(this.filetime));
			}
			return result;
		}

		public override string ToString()
		{
			return this.ToString(CultureInfo.CurrentCulture);
		}

		public string ToString(IFormatProvider formatProvider)
		{
			return string.Format(formatProvider ?? CultureInfo.InvariantCulture, "Type {0} = {1}", new object[]
			{
				this.vt,
				this.GetValue()
			});
		}

		private static readonly DateTime FileTimeStart = new DateTime(1601, 1, 1);

		[FieldOffset(0)]
		public PROPID vt;

		[FieldOffset(2)]
		private readonly byte wReserved1;

		[FieldOffset(3)]
		private readonly byte wReserved2;

		[FieldOffset(4)]
		private readonly int wReserved3;

		[FieldOffset(8)]
		public sbyte cVal;

		[FieldOffset(8)]
		public byte bVal;

		[FieldOffset(8)]
		public short iVal;

		[FieldOffset(8)]
		public ushort uiVal;

		[FieldOffset(8)]
		public int lVal;

		[FieldOffset(8)]
		public uint ulVal;

		[FieldOffset(8)]
		public int intVal;

		[FieldOffset(8)]
		public uint uintVal;

		[FieldOffset(8)]
		public long hVal;

		[FieldOffset(8)]
		public ulong uhVal;

		[FieldOffset(8)]
		public float fltVal;

		[FieldOffset(8)]
		public double dblVal;

		[FieldOffset(8)]
		public short boolVal;

		[FieldOffset(8)]
		public int scode;

		[FieldOffset(8)]
		public long filetime;

		[FieldOffset(8)]
		private IntPtr ptr;

		[FieldOffset(8)]
		public int cElems;

		[FieldOffset(12)]
		private IntPtr pElems;

		[FieldOffset(8)]
		public int cbSize;

		[FieldOffset(12)]
		private IntPtr pBlobData;
	}
}
