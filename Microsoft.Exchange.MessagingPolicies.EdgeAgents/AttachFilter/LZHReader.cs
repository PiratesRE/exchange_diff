using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Microsoft.Exchange.MessagingPolicies.AttachFilter
{
	internal sealed class LZHReader : IDisposable, IEnumerable<string>, IEnumerable
	{
		public LZHReader(Stream input)
		{
			this.input = input;
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
			return new LZHReader.LZHEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new LZHReader.LZHEnumerator(this);
		}

		private Stream input;

		private sealed class LZHEnumerator : IEnumerator<string>, IDisposable, IEnumerator
		{
			public LZHEnumerator(LZHReader reader)
			{
				this.bytesTally = 0;
				this.fileName = string.Empty;
				Encoding encoding = LZHReader.LZHEnumerator.encoding;
				if (encoding == null)
				{
					bool flag = false;
					try
					{
						encoding = Encoding.GetEncoding(932);
					}
					catch (NotSupportedException)
					{
						flag = true;
					}
					catch (ArgumentException)
					{
						flag = true;
					}
					if (flag)
					{
						encoding = new ASCIIEncoding();
					}
					Interlocked.Exchange<Encoding>(ref LZHReader.LZHEnumerator.encoding, encoding);
				}
				this.buffer = new BinaryReader(reader.input, LZHReader.LZHEnumerator.encoding);
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
				this.fileName = string.Empty;
				bool result;
				try
				{
					byte[] array = this.buffer.ReadBytes(21);
					this.bytesTally += array.Length;
					if (array.Length < 21)
					{
						result = false;
					}
					else
					{
						int num = (int)array[10] << 24 | (int)array[9] << 16 | (int)array[8] << 8 | (int)array[7];
						switch (array[20])
						{
						case 0:
						case 1:
						{
							int num2 = (int)this.buffer.ReadByte();
							this.bytesTally++;
							byte[] bytes = ZipReader.SafeReadHeader(this.buffer, num2);
							this.bytesTally += num2;
							this.fileName = LZHReader.LZHEnumerator.encoding.GetString(bytes);
							if (array[20] == 0)
							{
								this.SkipLevel0Data();
							}
							else
							{
								this.SkipLevel0Data();
								this.buffer.ReadByte();
								this.SkipOtherExtensionHeaders();
							}
							break;
						}
						case 2:
							this.GetLevel2FileNameEntry();
							this.SkipOtherExtensionHeaders();
							break;
						}
						if (!ZipReader.Skip(this.buffer, (long)num))
						{
							result = false;
						}
						else
						{
							this.bytesTally += num;
							result = true;
						}
					}
				}
				catch (IOException)
				{
					result = false;
				}
				catch (ArgumentOutOfRangeException)
				{
					result = false;
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

			private void SkipLevel0Data()
			{
				ZipReader.Skip(this.buffer, 2L);
				this.bytesTally += 2;
			}

			private void GetLevel2FileNameEntry()
			{
				ZipReader.Skip(this.buffer, 3L);
				this.bytesTally += 3;
				byte[] array = ZipReader.SafeReadHeader(this.buffer, 2);
				int num = (int)array[1] << 8 | (int)array[0];
				this.bytesTally += 2;
				if (num > 0)
				{
					byte b = this.buffer.ReadByte();
					this.bytesTally++;
					while (b != 1)
					{
						ZipReader.Skip(this.buffer, (long)(num - 3));
						this.bytesTally += num - 3;
						array = ZipReader.SafeReadHeader(this.buffer, 2);
						num = ((int)array[1] << 8 | (int)array[0]);
						this.bytesTally += 2;
						if (num <= 0)
						{
							break;
						}
						b = this.buffer.ReadByte();
						this.bytesTally++;
					}
					if (b == 1)
					{
						byte[] bytes = ZipReader.SafeReadHeader(this.buffer, num - 3);
						this.bytesTally += num - 3;
						this.fileName = LZHReader.LZHEnumerator.encoding.GetString(bytes);
					}
				}
			}

			private void SkipOtherExtensionHeaders()
			{
				for (;;)
				{
					byte[] array = ZipReader.SafeReadHeader(this.buffer, 2);
					int num = (int)array[1] << 8 | (int)array[0];
					this.bytesTally += 2;
					if (num > 0 && !ZipReader.Skip(this.buffer, (long)(num - 2)))
					{
						break;
					}
					if (num <= 0)
					{
						return;
					}
				}
			}

			private static Encoding encoding;

			private BinaryReader buffer;

			private int bytesTally;

			private string fileName;

			private struct BookMark
			{
				public BookMark(int length, int offset)
				{
					this.Length = length;
					this.Offset = offset;
				}

				internal int Length;

				internal int Offset;
			}
		}
	}
}
