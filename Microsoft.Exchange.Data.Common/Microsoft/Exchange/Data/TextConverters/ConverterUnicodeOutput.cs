using System;
using System.IO;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal class ConverterUnicodeOutput : ConverterOutput, IRestartable, IReusable
	{
		public ConverterUnicodeOutput(object destination, bool push, bool restartable)
		{
			if (push)
			{
				this.pushSink = (destination as TextWriter);
			}
			else
			{
				this.pullSink = (destination as ConverterReader);
				this.pullSink.SetSource(this);
			}
			this.canRestart = restartable;
			this.restartable = restartable;
		}

		public override bool CanAcceptMore
		{
			get
			{
				return this.canRestart || this.pullSink == null || this.cache.Length == 0;
			}
		}

		bool IRestartable.CanRestart()
		{
			return this.canRestart;
		}

		void IRestartable.Restart()
		{
			this.Reinitialize();
			this.canRestart = false;
		}

		void IRestartable.DisableRestart()
		{
			this.canRestart = false;
			this.FlushCached();
		}

		void IReusable.Initialize(object newSourceOrDestination)
		{
			if (this.pushSink != null && newSourceOrDestination != null)
			{
				TextWriter textWriter = newSourceOrDestination as TextWriter;
				if (textWriter == null)
				{
					throw new InvalidOperationException("cannot reinitialize this converter - new output should be a TextWriter object");
				}
				this.pushSink = textWriter;
			}
			this.Reinitialize();
		}

		public override void Write(char[] buffer, int offset, int count, IFallback fallback)
		{
			byte unsafeAsciiMask = 0;
			byte[] unsafeAsciiMap = (fallback == null) ? null : fallback.GetUnsafeAsciiMap(out unsafeAsciiMask);
			bool hasUnsafeUnicode = fallback != null && fallback.HasUnsafeUnicode();
			if (this.cache.Length == 0)
			{
				if (!this.canRestart)
				{
					if (this.pullSink != null)
					{
						char[] array;
						int num;
						int num2;
						this.pullSink.GetOutputBuffer(out array, out num, out num2);
						if (num2 != 0)
						{
							if (fallback != null)
							{
								int num3 = num;
								while (count != 0 && num2 != 0)
								{
									char c = buffer[offset];
									if (ConverterUnicodeOutput.IsUnsafeCharacter(c, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, this.isFirstChar, fallback))
									{
										int num4 = num;
										if (!fallback.FallBackChar(c, array, ref num, num + num2))
										{
											break;
										}
										num2 -= num - num4;
									}
									else
									{
										array[num++] = c;
										num2--;
									}
									this.isFirstChar = false;
									count--;
									offset++;
								}
								this.pullSink.ReportOutput(num - num3);
							}
							else
							{
								int num5 = Math.Min(num2, count);
								Buffer.BlockCopy(buffer, offset * 2, array, num * 2, num5 * 2);
								this.isFirstChar = false;
								count -= num5;
								offset += num5;
								this.pullSink.ReportOutput(num5);
								num += num5;
								num2 -= num5;
							}
						}
						while (count != 0)
						{
							if (fallback != null)
							{
								char[] array2;
								int num6;
								int num7;
								this.cache.GetBuffer(16, out array2, out num6, out num7);
								int num8 = num6;
								while (count != 0 && num7 != 0)
								{
									char c2 = buffer[offset];
									if (ConverterUnicodeOutput.IsUnsafeCharacter(c2, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, this.isFirstChar, fallback))
									{
										int num9 = num6;
										if (!fallback.FallBackChar(c2, array2, ref num6, num6 + num7))
										{
											break;
										}
										num7 -= num6 - num9;
									}
									else
									{
										array2[num6++] = c2;
										num7--;
									}
									this.isFirstChar = false;
									count--;
									offset++;
								}
								this.cache.Commit(num6 - num8);
							}
							else
							{
								int size = Math.Min(count, 256);
								char[] array2;
								int num6;
								int num7;
								this.cache.GetBuffer(size, out array2, out num6, out num7);
								int num10 = Math.Min(num7, count);
								Buffer.BlockCopy(buffer, offset * 2, array2, num6 * 2, num10 * 2);
								this.isFirstChar = false;
								this.cache.Commit(num10);
								offset += num10;
								count -= num10;
							}
						}
						while (num2 != 0)
						{
							if (this.cache.Length == 0)
							{
								return;
							}
							char[] src;
							int num11;
							int val;
							this.cache.GetData(out src, out num11, out val);
							int num12 = Math.Min(val, num2);
							Buffer.BlockCopy(src, num11 * 2, array, num * 2, num12 * 2);
							this.cache.ReportRead(num12);
							this.pullSink.ReportOutput(num12);
							num += num12;
							num2 -= num12;
						}
					}
					else
					{
						if (fallback != null)
						{
							char[] array3;
							int num13;
							int num14;
							this.cache.GetBuffer(1024, out array3, out num13, out num14);
							int num15 = num13;
							int num16 = num14;
							while (count != 0)
							{
								while (count != 0 && num14 != 0)
								{
									char c3 = buffer[offset];
									if (ConverterUnicodeOutput.IsUnsafeCharacter(c3, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, this.isFirstChar, fallback))
									{
										int num17 = num13;
										if (!fallback.FallBackChar(c3, array3, ref num13, num13 + num14))
										{
											break;
										}
										num14 -= num13 - num17;
									}
									else
									{
										array3[num13++] = c3;
										num14--;
									}
									this.isFirstChar = false;
									count--;
									offset++;
								}
								if (num13 - num15 != 0)
								{
									this.pushSink.Write(array3, num15, num13 - num15);
									num13 = num15;
									num14 = num16;
								}
							}
							return;
						}
						if (count != 0)
						{
							this.pushSink.Write(buffer, offset, count);
							this.isFirstChar = false;
						}
					}
					return;
				}
			}
			while (count != 0)
			{
				if (fallback != null)
				{
					char[] array4;
					int num18;
					int num19;
					this.cache.GetBuffer(16, out array4, out num18, out num19);
					int num20 = num18;
					while (count != 0 && num19 != 0)
					{
						char c4 = buffer[offset];
						if (ConverterUnicodeOutput.IsUnsafeCharacter(c4, unsafeAsciiMap, unsafeAsciiMask, hasUnsafeUnicode, this.isFirstChar, fallback))
						{
							int num21 = num18;
							if (!fallback.FallBackChar(c4, array4, ref num18, num18 + num19))
							{
								break;
							}
							num19 -= num18 - num21;
						}
						else
						{
							array4[num18++] = c4;
							num19--;
						}
						this.isFirstChar = false;
						count--;
						offset++;
					}
					this.cache.Commit(num18 - num20);
				}
				else
				{
					int size2 = Math.Min(count, 256);
					char[] array4;
					int num18;
					int num19;
					this.cache.GetBuffer(size2, out array4, out num18, out num19);
					int num22 = Math.Min(num19, count);
					Buffer.BlockCopy(buffer, offset * 2, array4, num18 * 2, num22 * 2);
					this.isFirstChar = false;
					this.cache.Commit(num22);
					offset += num22;
					count -= num22;
				}
			}
		}

		public override void Flush()
		{
			if (this.endOfFile)
			{
				return;
			}
			this.canRestart = false;
			this.FlushCached();
			if (this.pullSink == null)
			{
				this.pushSink.Flush();
			}
			else if (this.cache.Length == 0)
			{
				this.pullSink.ReportEndOfFile();
			}
			this.endOfFile = true;
		}

		public bool GetOutputChunk(out char[] chunkBuffer, out int chunkOffset, out int chunkLength)
		{
			if (this.cache.Length == 0 || this.canRestart)
			{
				chunkBuffer = null;
				chunkOffset = 0;
				chunkLength = 0;
				return false;
			}
			this.cache.GetData(out chunkBuffer, out chunkOffset, out chunkLength);
			return true;
		}

		public void ReportOutput(int readCount)
		{
			this.cache.ReportRead(readCount);
			if (this.cache.Length == 0 && this.endOfFile)
			{
				this.pullSink.ReportEndOfFile();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.cache != null && this.cache is IDisposable)
			{
				((IDisposable)this.cache).Dispose();
			}
			this.cache = null;
			this.pushSink = null;
			this.pullSink = null;
			base.Dispose(disposing);
		}

		private static bool IsUnsafeCharacter(char ch, byte[] unsafeAsciiMap, byte unsafeAsciiMask, bool hasUnsafeUnicode, bool isFirstChar, IFallback fallback)
		{
			return unsafeAsciiMap != null && (((int)ch < unsafeAsciiMap.Length && (unsafeAsciiMap[(int)ch] & unsafeAsciiMask) != 0) || (hasUnsafeUnicode && ch >= '\u007f' && fallback.IsUnsafeUnicode(ch, isFirstChar)));
		}

		private void Reinitialize()
		{
			this.endOfFile = false;
			this.cache.Reset();
			this.canRestart = this.restartable;
			this.isFirstChar = true;
		}

		private bool FlushCached()
		{
			if (this.canRestart || this.cache.Length == 0)
			{
				return false;
			}
			if (this.pullSink == null)
			{
				while (this.cache.Length != 0)
				{
					char[] buffer;
					int num;
					int num2;
					this.cache.GetData(out buffer, out num, out num2);
					this.pushSink.Write(buffer, num, num2);
					this.cache.ReportRead(num2);
				}
			}
			else
			{
				char[] buffer;
				int num;
				int count;
				this.pullSink.GetOutputBuffer(out buffer, out num, out count);
				int num2 = this.cache.Read(buffer, num, count);
				this.pullSink.ReportOutput(num2);
				if (this.cache.Length == 0 && this.endOfFile)
				{
					this.pullSink.ReportEndOfFile();
				}
			}
			return true;
		}

		private const int FallbackExpansionMax = 16;

		private TextWriter pushSink;

		private ConverterReader pullSink;

		private bool endOfFile;

		private bool restartable;

		private bool canRestart;

		private bool isFirstChar = true;

		private TextCache cache = new TextCache();
	}
}
