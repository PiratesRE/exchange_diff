using System;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Exchange.Data.Internal
{
	internal static class ScratchPad
	{
		public static void Begin()
		{
			if (ScratchPad.pad == null)
			{
				ScratchPad.pad = new ScratchPad.ScratchPadContainer();
				return;
			}
			ScratchPad.pad.AddRef();
		}

		public static void End()
		{
			if (ScratchPad.pad != null && ScratchPad.pad.Release())
			{
				ScratchPad.pad = null;
			}
		}

		public static byte[] GetByteBuffer(int size)
		{
			if (ScratchPad.pad == null)
			{
				return new byte[size];
			}
			return ScratchPad.pad.GetByteBuffer(size);
		}

		[Conditional("DEBUG")]
		public static void ReleaseByteBuffer()
		{
			if (ScratchPad.pad != null)
			{
				ScratchPad.pad.ReleaseByteBuffer();
			}
		}

		public static char[] GetCharBuffer(int size)
		{
			if (ScratchPad.pad == null)
			{
				return new char[size];
			}
			return ScratchPad.pad.GetCharBuffer(size);
		}

		[Conditional("DEBUG")]
		public static void ReleaseCharBuffer()
		{
			if (ScratchPad.pad != null)
			{
				ScratchPad.pad.ReleaseCharBuffer();
			}
		}

		public static StringBuilder GetStringBuilder()
		{
			return ScratchPad.GetStringBuilder(16);
		}

		public static StringBuilder GetStringBuilder(int initialCapacity)
		{
			if (ScratchPad.pad == null)
			{
				return new StringBuilder(initialCapacity);
			}
			return ScratchPad.pad.GetStringBuilder(initialCapacity);
		}

		public static void ReleaseStringBuilder()
		{
			if (ScratchPad.pad != null)
			{
				ScratchPad.pad.ReleaseStringBuilder();
			}
		}

		[ThreadStatic]
		private static ScratchPad.ScratchPadContainer pad;

		private class ScratchPadContainer
		{
			public ScratchPadContainer()
			{
				this.refCount = 1;
			}

			public void AddRef()
			{
				this.refCount++;
			}

			public bool Release()
			{
				this.refCount--;
				return this.refCount == 0;
			}

			public byte[] GetByteBuffer(int size)
			{
				if (this.byteBuffer == null || this.byteBuffer.Length < size)
				{
					this.byteBuffer = new byte[size];
				}
				return this.byteBuffer;
			}

			public void ReleaseByteBuffer()
			{
			}

			public char[] GetCharBuffer(int size)
			{
				if (this.charBuffer == null || this.charBuffer.Length < size)
				{
					this.charBuffer = new char[size];
				}
				return this.charBuffer;
			}

			public void ReleaseCharBuffer()
			{
			}

			public StringBuilder GetStringBuilder(int initialCapacity)
			{
				if (initialCapacity <= 512)
				{
					if (this.stringBuilder == null)
					{
						this.stringBuilder = new StringBuilder(512);
					}
					else
					{
						this.stringBuilder.Length = 0;
					}
					return this.stringBuilder;
				}
				return new StringBuilder(initialCapacity);
			}

			public void ReleaseStringBuilder()
			{
				if (this.stringBuilder != null && (this.stringBuilder.Capacity > 512 || this.stringBuilder.Length * 2 >= this.stringBuilder.Capacity + 1))
				{
					this.stringBuilder = null;
				}
			}

			public const int ScratchStringBuilderCapacity = 512;

			private int refCount;

			private byte[] byteBuffer;

			private char[] charBuffer;

			private StringBuilder stringBuilder;
		}
	}
}
