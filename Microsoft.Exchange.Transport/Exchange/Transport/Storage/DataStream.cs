using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Transport.Storage
{
	internal abstract class DataStream : Stream
	{
		internal DataStream(DataColumn column, DataRow row, int sequence)
		{
			if (!column.MultiValued && sequence != 1)
			{
				throw new ArgumentException("Column is not multi-valued and sequence != 1 was suplied", "column");
			}
			this.Column = column;
			this.row = row;
			this.sequence = sequence;
		}

		internal DataStream(DataStream rhs) : this(rhs.Column, rhs.DataRow, rhs.Sequence)
		{
			this.length = rhs.Length;
		}

		internal static int JetChunkSize
		{
			get
			{
				if (DataStream.jetChunkSize == 0)
				{
					DataSource.InitGlobal();
					int num = 0;
					string text = null;
					Api.JetGetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, (JET_param)163, ref num, out text, 0);
					DataStream.jetChunkSize = num;
				}
				return DataStream.jetChunkSize;
			}
		}

		internal static int TransportChunkSize
		{
			get
			{
				if (DataStream.transportChunkSize == 0)
				{
					DataSource.InitGlobal();
					int num = DataStream.JetChunkSize;
					DataStream.transportChunkSize = Math.Max(1, 65536 / num) * num;
				}
				return DataStream.transportChunkSize;
			}
		}

		internal static int BufferedStreamSize
		{
			get
			{
				if (DataStream.bufferedStreamSize == 0)
				{
					DataSource.InitGlobal();
					TransportAppConfig.JetDatabaseConfig jetDatabase = Components.TransportAppConfig.JetDatabase;
					DataStream.bufferedStreamSize = (int)((double)jetDatabase.BufferedStreamSize);
				}
				return DataStream.bufferedStreamSize;
			}
		}

		internal DataRow DataRow
		{
			get
			{
				return this.row;
			}
		}

		internal int Sequence
		{
			get
			{
				return this.sequence;
			}
		}

		public override long Length
		{
			get
			{
				return this.length;
			}
		}

		public override long Position
		{
			get
			{
				return this.position;
			}
			set
			{
				if (value < 0L || value > this.length)
				{
					throw new DataStream.PositionException(this, value);
				}
				this.position = value;
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
			case SeekOrigin.Begin:
				this.Position = offset;
				break;
			case SeekOrigin.Current:
				this.Position += offset;
				break;
			case SeekOrigin.End:
				this.Position = this.Length + offset;
				break;
			}
			return this.position;
		}

		protected void SetReadPosition(long newPosition)
		{
			this.position = newPosition;
			this.length = Math.Max(this.length, newPosition);
		}

		protected int InternalRead(byte[] buffer, int offset, int count, DataTableCursor cursor)
		{
			JET_RETINFO retinfo = new JET_RETINFO
			{
				ibLongValue = (int)this.position,
				itagSequence = this.Sequence
			};
			int num = 0;
			try
			{
				Api.JetRetrieveColumn(cursor.Session, cursor.TableId, this.Column.ColumnId, buffer, count, offset, out num, RetrieveColumnGrbit.RetrieveNull, retinfo);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, cursor.Connection.Source))
				{
					throw;
				}
			}
			this.DataRow.PerfCounters.StreamReads.Increment();
			if (num == 0)
			{
				return 0;
			}
			int num2 = Math.Min(count, num);
			this.DataRow.PerfCounters.StreamBytesRead.IncrementBy((long)num2);
			this.SetReadPosition(this.Position + (long)num2);
			return num2;
		}

		protected override void Dispose(bool disposing)
		{
			this.row = null;
			base.Dispose(disposing);
		}

		public readonly DataColumn Column;

		private static int jetChunkSize;

		private static int transportChunkSize;

		private static int bufferedStreamSize;

		protected DataRow row;

		protected int sequence;

		protected long length;

		protected long position;

		internal class PositionException : InvalidOperationException
		{
			public PositionException(object stream, long seekValue)
			{
				this.stream = stream;
				this.seekValue = seekValue;
			}

			protected PositionException(SerializationInfo info, StreamingContext context) : base(info, context)
			{
				if (info != null)
				{
					this.seekValue = (long)info.GetValue("Position", typeof(long));
				}
			}

			public object Stream
			{
				get
				{
					return this.stream;
				}
			}

			public long Position
			{
				get
				{
					return this.seekValue;
				}
			}

			[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
			public override void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				base.GetObjectData(info, context);
				info.AddValue("Position", this.Position, typeof(long));
			}

			private object stream;

			private long seekValue;
		}
	}
}
