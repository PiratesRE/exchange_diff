using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class StreamReader : Reader
	{
		internal StreamReader(Stream stream)
		{
			Util.ThrowOnNullArgument(stream, "stream");
			this.reader = new BinaryReader(stream, Encoding.Unicode);
		}

		public override long Length
		{
			get
			{
				return this.reader.BaseStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.reader.BaseStream.Position;
			}
			set
			{
				this.reader.BaseStream.Position = value;
			}
		}

		protected override byte InternalReadByte()
		{
			return this.reader.ReadByte();
		}

		protected override double InternalReadDouble()
		{
			return this.reader.ReadDouble();
		}

		protected override short InternalReadInt16()
		{
			return this.reader.ReadInt16();
		}

		protected override ushort InternalReadUInt16()
		{
			return this.reader.ReadUInt16();
		}

		protected override int InternalReadInt32()
		{
			return this.reader.ReadInt32();
		}

		protected override uint InternalReadUInt32()
		{
			return this.reader.ReadUInt32();
		}

		protected override long InternalReadInt64()
		{
			return this.reader.ReadInt64();
		}

		protected override ulong InternalReadUInt64()
		{
			return this.reader.ReadUInt64();
		}

		protected override float InternalReadSingle()
		{
			return this.reader.ReadSingle();
		}

		protected override ArraySegment<byte> InternalReadArraySegment(uint count)
		{
			return new ArraySegment<byte>(this.reader.ReadBytes((int)count));
		}

		protected override ArraySegment<byte> InternalReadArraySegmentForString(int maxCount)
		{
			int count = this.reader.Read(base.StringBuffer, 0, maxCount);
			return new ArraySegment<byte>(base.StringBuffer, 0, count);
		}

		protected override bool NeedsStagingAreaForFixedLengthStrings
		{
			get
			{
				return true;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<StreamReader>(this);
		}

		protected override void InternalDispose()
		{
			if (this.reader != null)
			{
				((IDisposable)this.reader).Dispose();
			}
			base.InternalDispose();
		}

		private readonly BinaryReader reader;
	}
}
