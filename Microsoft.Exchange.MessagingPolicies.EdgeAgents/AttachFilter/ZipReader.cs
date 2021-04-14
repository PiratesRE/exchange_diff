using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.AttachFilter
{
	internal sealed class ZipReader : IDisposable, IEnumerable<string>, IEnumerable
	{
		public ZipReader(Stream input, int nestedLevel)
		{
			this.input = input;
			this.nestedLevel = nestedLevel;
		}

		public void Dispose()
		{
			if (this.input != null)
			{
				this.input.Close();
			}
		}

		IEnumerator<string> IEnumerable<string>.GetEnumerator()
		{
			return new ZipReader.ZipEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new ZipReader.ZipEnumerator(this);
		}

		internal static bool Skip(BinaryReader reader, long bytes)
		{
			if (bytes + reader.BaseStream.Position <= 0L)
			{
				return false;
			}
			if (reader.BaseStream.CanSeek)
			{
				reader.BaseStream.Seek(bytes, SeekOrigin.Current);
			}
			else
			{
				long num;
				do
				{
					num = bytes;
					bytes = num - 1L;
				}
				while (num > 0L && reader.BaseStream.ReadByte() != -1);
				if (bytes > 0L)
				{
					return false;
				}
			}
			return true;
		}

		internal static byte[] SafeReadHeader(BinaryReader reader, int length)
		{
			if (length <= 0)
			{
				throw new ExchangeDataException("bad zip file");
			}
			byte[] array = reader.ReadBytes(length);
			if (array == null || array.Length < length)
			{
				throw new ExchangeDataException("bad zip file");
			}
			return array;
		}

		private Stream input;

		private int nestedLevel;

		private sealed class ZipEnumerator : IEnumerator<string>, IDisposable, IEnumerator
		{
			public ZipEnumerator(ZipReader reader)
			{
				this.buffer = new BinaryReader(reader.input, ZipReader.ZipEnumerator.encoding);
				this.trail = new Stack<ZipReader.ZipEnumerator.BookMark>();
				this.bytesTally = 0L;
				this.nestedLevel = reader.nestedLevel;
				this.fileName = string.Empty;
			}

			public object Current
			{
				get
				{
					return this.fileName;
				}
			}

			string IEnumerator<string>.Current
			{
				get
				{
					return this.fileName;
				}
			}

			public bool MoveNext()
			{
				if (this.trail != null && this.trail.Count > this.nestedLevel)
				{
					throw new ExchangeDataException("bad zip file");
				}
				this.fileName = string.Empty;
				bool result;
				try
				{
					uint num = 0U;
					try
					{
						num = this.buffer.ReadUInt32();
					}
					catch (EndOfStreamException)
					{
						return false;
					}
					this.bytesTally += 4L;
					if (num != 67324752U)
					{
						if (this.trail.Count > 0)
						{
							ZipReader.ZipEnumerator.BookMark bookMark = this.trail.Pop();
							long num2 = bookMark.Length + bookMark.Offset - this.bytesTally;
							if (!ZipReader.Skip(this.buffer, num2))
							{
								result = false;
							}
							else
							{
								this.bytesTally += num2;
								result = true;
							}
						}
						else
						{
							if (num != 33639248U && (num & 65535U) != 23U)
							{
								throw new ExchangeDataException("bad zip file");
							}
							result = false;
						}
					}
					else
					{
						this.buffer.ReadUInt16();
						ushort num3 = this.buffer.ReadUInt16();
						this.bytesTally += 4L;
						this.buffer.ReadUInt16();
						this.buffer.ReadUInt32();
						this.bytesTally += 6L;
						this.buffer.ReadUInt32();
						long num4 = (long)((ulong)this.buffer.ReadUInt32());
						this.bytesTally += 8L;
						this.buffer.ReadUInt32();
						int num5 = (int)this.buffer.ReadUInt16();
						int num6 = (int)this.buffer.ReadUInt16();
						this.bytesTally += 8L;
						byte[] bytes = this.buffer.ReadBytes(num5);
						this.bytesTally += (long)num5;
						this.fileName = ZipReader.ZipEnumerator.encoding.GetString(bytes);
						if (num6 > 0)
						{
							if (!ZipReader.Skip(this.buffer, (long)num6))
							{
								return false;
							}
							this.bytesTally += (long)num6;
						}
						if (num4 > 0L)
						{
							if (this.fileName.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
							{
								this.trail.Push(new ZipReader.ZipEnumerator.BookMark(this.bytesTally, num4));
								return true;
							}
							if (!ZipReader.Skip(this.buffer, num4))
							{
								return false;
							}
							this.bytesTally += num4;
						}
						if ((num3 & 8) != 0)
						{
							if (num4 == 0L)
							{
								uint num7 = this.buffer.ReadUInt32();
								this.bytesTally += 4L;
								while (num7 != 134695760U)
								{
									uint num8 = (uint)this.buffer.ReadByte();
									num7 = (num7 >> 8 | num8 << 24);
									this.bytesTally += 1L;
								}
							}
							this.buffer.ReadUInt32();
							num4 = (long)((ulong)this.buffer.ReadUInt32());
							this.buffer.ReadUInt32();
							this.bytesTally += 12L;
						}
						result = true;
					}
				}
				catch (IOException)
				{
					throw new ExchangeDataException("bad zip file");
				}
				catch (ArgumentOutOfRangeException)
				{
					throw new ExchangeDataException("bad zip file");
				}
				return result;
			}

			public void Dispose()
			{
				if (this.buffer != null)
				{
					this.buffer.Close();
				}
			}

			public void Reset()
			{
				throw new NotImplementedException();
			}

			private const int DataDescriptorSignature = 134695760;

			private static Encoding encoding = new UTF8Encoding();

			private BinaryReader buffer;

			private Stack<ZipReader.ZipEnumerator.BookMark> trail;

			private long bytesTally;

			private string fileName;

			private int nestedLevel;

			private struct BookMark
			{
				public BookMark(long length, long offset)
				{
					this.Length = length;
					this.Offset = offset;
				}

				internal long Length;

				internal long Offset;
			}
		}
	}
}
