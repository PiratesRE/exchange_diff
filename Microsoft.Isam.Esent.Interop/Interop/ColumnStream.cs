using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.Isam.Esent.Interop
{
	public class ColumnStream : Stream
	{
		public ColumnStream(JET_SESID sesid, JET_TABLEID tableid, JET_COLUMNID columnid)
		{
			this.sesid = sesid;
			this.tableid = tableid;
			this.columnid = columnid;
			this.Itag = 1;
		}

		public int Itag { get; set; }

		public override bool CanRead
		{
			[DebuggerStepThrough]
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			[DebuggerStepThrough]
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			[DebuggerStepThrough]
			get
			{
				return true;
			}
		}

		public override long Position
		{
			[DebuggerStepThrough]
			get
			{
				return (long)this.ibLongValue;
			}
			set
			{
				if (value < 0L || value > 2147483647L)
				{
					throw new ArgumentOutOfRangeException("value", value, "A long-value offset has to be between 0 and 0x7fffffff bytes");
				}
				this.ibLongValue = checked((int)value);
			}
		}

		public override long Length
		{
			get
			{
				JET_RETINFO retinfo = new JET_RETINFO
				{
					itagSequence = this.Itag,
					ibLongValue = 0
				};
				int num;
				Api.JetRetrieveColumn(this.sesid, this.tableid, this.columnid, null, 0, out num, ColumnStream.RetrieveGrbit, retinfo);
				return (long)num;
			}
		}

		private static RetrieveColumnGrbit RetrieveGrbit
		{
			[DebuggerStepThrough]
			get
			{
				return RetrieveColumnGrbit.RetrieveCopy;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "ColumnStream(0x{0:x}:{1})", new object[]
			{
				this.columnid.Value,
				this.Itag
			});
		}

		public override void Flush()
		{
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			ColumnStream.CheckBufferArguments(buffer, offset, count);
			checked
			{
				int num = (int)this.Length;
				int num2 = this.ibLongValue + count;
				JET_SETINFO setinfo;
				if (this.ibLongValue > num)
				{
					setinfo = new JET_SETINFO
					{
						itagSequence = this.Itag
					};
					Api.JetSetColumn(this.sesid, this.tableid, this.columnid, null, this.ibLongValue, SetColumnGrbit.SizeLV, setinfo);
					num = this.ibLongValue;
				}
				SetColumnGrbit grbit;
				if (this.ibLongValue == num)
				{
					grbit = SetColumnGrbit.AppendLV;
				}
				else if (num2 >= num)
				{
					grbit = (SetColumnGrbit.OverwriteLV | SetColumnGrbit.SizeLV);
				}
				else
				{
					grbit = SetColumnGrbit.OverwriteLV;
				}
				setinfo = new JET_SETINFO
				{
					itagSequence = this.Itag,
					ibLongValue = this.ibLongValue
				};
				Api.JetSetColumn(this.sesid, this.tableid, this.columnid, buffer, count, offset, grbit, setinfo);
				this.ibLongValue += count;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			ColumnStream.CheckBufferArguments(buffer, offset, count);
			if ((long)this.ibLongValue >= this.Length)
			{
				return 0;
			}
			JET_RETINFO retinfo = new JET_RETINFO
			{
				itagSequence = this.Itag,
				ibLongValue = this.ibLongValue
			};
			int val;
			Api.JetRetrieveColumn(this.sesid, this.tableid, this.columnid, buffer, count, offset, out val, ColumnStream.RetrieveGrbit, retinfo);
			int num = Math.Min(val, count);
			checked
			{
				this.ibLongValue += num;
				return num;
			}
		}

		public override void SetLength(long value)
		{
			if (value > 2147483647L || value < 0L)
			{
				throw new ArgumentOutOfRangeException("value", value, "A LongValueStream cannot be longer than 0x7FFFFFF or less than 0 bytes");
			}
			if (value < this.Length && value > 0L)
			{
				byte[] array = new byte[value];
				JET_RETINFO retinfo = new JET_RETINFO
				{
					itagSequence = this.Itag,
					ibLongValue = 0
				};
				int num;
				Api.JetRetrieveColumn(this.sesid, this.tableid, this.columnid, array, array.Length, out num, ColumnStream.RetrieveGrbit, retinfo);
				JET_SETINFO setinfo = new JET_SETINFO
				{
					itagSequence = this.Itag
				};
				Api.JetSetColumn(this.sesid, this.tableid, this.columnid, array, array.Length, SetColumnGrbit.None, setinfo);
			}
			else
			{
				JET_SETINFO setinfo2 = new JET_SETINFO
				{
					itagSequence = this.Itag
				};
				SetColumnGrbit grbit = (0L == value) ? SetColumnGrbit.ZeroLength : SetColumnGrbit.SizeLV;
				Api.JetSetColumn(this.sesid, this.tableid, this.columnid, null, checked((int)value), grbit, setinfo2);
			}
			if ((long)this.ibLongValue > value)
			{
				this.ibLongValue = checked((int)value);
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			checked
			{
				long num;
				switch (origin)
				{
				case SeekOrigin.Begin:
					num = offset;
					break;
				case SeekOrigin.Current:
					num = unchecked((long)this.ibLongValue) + offset;
					break;
				case SeekOrigin.End:
					num = this.Length + offset;
					break;
				default:
					throw new ArgumentOutOfRangeException("origin", origin, "Unknown origin");
				}
				if (num < 0L || num > 2147483647L)
				{
					throw new ArgumentOutOfRangeException("offset", offset, "invalid offset/origin combination");
				}
				this.ibLongValue = (int)num;
			}
			return (long)this.ibLongValue;
		}

		private static void CheckBufferArguments(ICollection<byte> buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", offset, "cannot be negative");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", count, "cannot be negative");
			}
			if (checked(buffer.Count - offset) < count)
			{
				throw new ArgumentOutOfRangeException("count", count, "cannot be larger than the size of the buffer");
			}
		}

		private const int MaxLongValueSize = 2147483647;

		private readonly JET_SESID sesid;

		private readonly JET_TABLEID tableid;

		private readonly JET_COLUMNID columnid;

		private int ibLongValue;
	}
}
