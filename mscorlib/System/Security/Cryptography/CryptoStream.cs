using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class CryptoStream : Stream, IDisposable
	{
		public CryptoStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode) : this(stream, transform, mode, false)
		{
		}

		public CryptoStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode, bool leaveOpen)
		{
			this._stream = stream;
			this._transformMode = mode;
			this._Transform = transform;
			this._leaveOpen = leaveOpen;
			CryptoStreamMode transformMode = this._transformMode;
			if (transformMode != CryptoStreamMode.Read)
			{
				if (transformMode != CryptoStreamMode.Write)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_InvalidValue"));
				}
				if (!this._stream.CanWrite)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_StreamNotWritable"), "stream");
				}
				this._canWrite = true;
			}
			else
			{
				if (!this._stream.CanRead)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_StreamNotReadable"), "stream");
				}
				this._canRead = true;
			}
			this.InitializeBuffer();
		}

		public override bool CanRead
		{
			get
			{
				return this._canRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this._canWrite;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_UnseekableStream"));
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_UnseekableStream"));
			}
			set
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_UnseekableStream"));
			}
		}

		public bool HasFlushedFinalBlock
		{
			get
			{
				return this._finalBlockTransformed;
			}
		}

		public void FlushFinalBlock()
		{
			if (this._finalBlockTransformed)
			{
				throw new NotSupportedException(Environment.GetResourceString("Cryptography_CryptoStream_FlushFinalBlockTwice"));
			}
			byte[] array = this._Transform.TransformFinalBlock(this._InputBuffer, 0, this._InputBufferIndex);
			this._finalBlockTransformed = true;
			if (this._canWrite && this._OutputBufferIndex > 0)
			{
				this._stream.Write(this._OutputBuffer, 0, this._OutputBufferIndex);
				this._OutputBufferIndex = 0;
			}
			if (this._canWrite)
			{
				this._stream.Write(array, 0, array.Length);
			}
			CryptoStream cryptoStream = this._stream as CryptoStream;
			if (cryptoStream != null)
			{
				if (!cryptoStream.HasFlushedFinalBlock)
				{
					cryptoStream.FlushFinalBlock();
				}
			}
			else
			{
				this._stream.Flush();
			}
			if (this._InputBuffer != null)
			{
				Array.Clear(this._InputBuffer, 0, this._InputBuffer.Length);
			}
			if (this._OutputBuffer != null)
			{
				Array.Clear(this._OutputBuffer, 0, this._OutputBuffer.Length);
			}
		}

		public override void Flush()
		{
		}

		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			if (base.GetType() != typeof(CryptoStream))
			{
				return base.FlushAsync(cancellationToken);
			}
			if (!cancellationToken.IsCancellationRequested)
			{
				return Task.CompletedTask;
			}
			return Task.FromCancellation(cancellationToken);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException(Environment.GetResourceString("NotSupported_UnseekableStream"));
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException(Environment.GetResourceString("NotSupported_UnseekableStream"));
		}

		public override int Read([In] [Out] byte[] buffer, int offset, int count)
		{
			if (!this.CanRead)
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_UnreadableStream"));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			int i = count;
			int num = offset;
			if (this._OutputBufferIndex != 0)
			{
				if (this._OutputBufferIndex > count)
				{
					Buffer.InternalBlockCopy(this._OutputBuffer, 0, buffer, offset, count);
					Buffer.InternalBlockCopy(this._OutputBuffer, count, this._OutputBuffer, 0, this._OutputBufferIndex - count);
					this._OutputBufferIndex -= count;
					return count;
				}
				Buffer.InternalBlockCopy(this._OutputBuffer, 0, buffer, offset, this._OutputBufferIndex);
				i -= this._OutputBufferIndex;
				num += this._OutputBufferIndex;
				this._OutputBufferIndex = 0;
			}
			if (this._finalBlockTransformed)
			{
				return count - i;
			}
			if (i > this._OutputBlockSize && this._Transform.CanTransformMultipleBlocks)
			{
				int num2 = i / this._OutputBlockSize;
				int num3 = num2 * this._InputBlockSize;
				byte[] array = new byte[num3];
				Buffer.InternalBlockCopy(this._InputBuffer, 0, array, 0, this._InputBufferIndex);
				int num4 = this._InputBufferIndex;
				num4 += this._stream.Read(array, this._InputBufferIndex, num3 - this._InputBufferIndex);
				this._InputBufferIndex = 0;
				if (num4 <= this._InputBlockSize)
				{
					this._InputBuffer = array;
					this._InputBufferIndex = num4;
				}
				else
				{
					int num5 = num4 / this._InputBlockSize * this._InputBlockSize;
					int num6 = num4 - num5;
					if (num6 != 0)
					{
						this._InputBufferIndex = num6;
						Buffer.InternalBlockCopy(array, num5, this._InputBuffer, 0, num6);
					}
					byte[] array2 = new byte[num5 / this._InputBlockSize * this._OutputBlockSize];
					int num7 = this._Transform.TransformBlock(array, 0, num5, array2, 0);
					Buffer.InternalBlockCopy(array2, 0, buffer, num, num7);
					Array.Clear(array, 0, array.Length);
					Array.Clear(array2, 0, array2.Length);
					i -= num7;
					num += num7;
				}
			}
			while (i > 0)
			{
				while (this._InputBufferIndex < this._InputBlockSize)
				{
					int num4 = this._stream.Read(this._InputBuffer, this._InputBufferIndex, this._InputBlockSize - this._InputBufferIndex);
					if (num4 != 0)
					{
						this._InputBufferIndex += num4;
					}
					else
					{
						byte[] array3 = this._Transform.TransformFinalBlock(this._InputBuffer, 0, this._InputBufferIndex);
						this._OutputBuffer = array3;
						this._OutputBufferIndex = array3.Length;
						this._finalBlockTransformed = true;
						if (i < this._OutputBufferIndex)
						{
							Buffer.InternalBlockCopy(this._OutputBuffer, 0, buffer, num, i);
							this._OutputBufferIndex -= i;
							Buffer.InternalBlockCopy(this._OutputBuffer, i, this._OutputBuffer, 0, this._OutputBufferIndex);
							return count;
						}
						Buffer.InternalBlockCopy(this._OutputBuffer, 0, buffer, num, this._OutputBufferIndex);
						i -= this._OutputBufferIndex;
						this._OutputBufferIndex = 0;
						return count - i;
					}
				}
				int num7 = this._Transform.TransformBlock(this._InputBuffer, 0, this._InputBlockSize, this._OutputBuffer, 0);
				this._InputBufferIndex = 0;
				if (i < num7)
				{
					Buffer.InternalBlockCopy(this._OutputBuffer, 0, buffer, num, i);
					this._OutputBufferIndex = num7 - i;
					Buffer.InternalBlockCopy(this._OutputBuffer, i, this._OutputBuffer, 0, this._OutputBufferIndex);
					return count;
				}
				Buffer.InternalBlockCopy(this._OutputBuffer, 0, buffer, num, num7);
				num += num7;
				i -= num7;
			}
			return count;
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (!this.CanRead)
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_UnreadableStream"));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			if (base.GetType() != typeof(CryptoStream))
			{
				return base.ReadAsync(buffer, offset, count, cancellationToken);
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCancellation<int>(cancellationToken);
			}
			return this.ReadAsyncInternal(buffer, offset, count, cancellationToken);
		}

		private async Task<int> ReadAsyncInternal(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			await default(CryptoStream.HopToThreadPoolAwaitable);
			SemaphoreSlim sem = base.EnsureAsyncActiveSemaphoreInitialized();
			await sem.WaitAsync().ConfigureAwait(false);
			int result;
			try
			{
				int bytesToDeliver = count;
				int currentOutputIndex = offset;
				if (this._OutputBufferIndex != 0)
				{
					if (this._OutputBufferIndex > count)
					{
						Buffer.InternalBlockCopy(this._OutputBuffer, 0, buffer, offset, count);
						Buffer.InternalBlockCopy(this._OutputBuffer, count, this._OutputBuffer, 0, this._OutputBufferIndex - count);
						this._OutputBufferIndex -= count;
						return count;
					}
					Buffer.InternalBlockCopy(this._OutputBuffer, 0, buffer, offset, this._OutputBufferIndex);
					bytesToDeliver -= this._OutputBufferIndex;
					currentOutputIndex += this._OutputBufferIndex;
					this._OutputBufferIndex = 0;
				}
				if (this._finalBlockTransformed)
				{
					result = count - bytesToDeliver;
				}
				else
				{
					if (bytesToDeliver > this._OutputBlockSize && this._Transform.CanTransformMultipleBlocks)
					{
						int num = bytesToDeliver / this._OutputBlockSize * this._InputBlockSize;
						byte[] tempInputBuffer = new byte[num];
						Buffer.InternalBlockCopy(this._InputBuffer, 0, tempInputBuffer, 0, this._InputBufferIndex);
						int inputBufferIndex = this._InputBufferIndex;
						int num2 = inputBufferIndex + await this._stream.ReadAsync(tempInputBuffer, this._InputBufferIndex, num - this._InputBufferIndex, cancellationToken).ConfigureAwait(false);
						this._InputBufferIndex = 0;
						if (num2 <= this._InputBlockSize)
						{
							this._InputBuffer = tempInputBuffer;
							this._InputBufferIndex = num2;
						}
						else
						{
							int num3 = num2 / this._InputBlockSize * this._InputBlockSize;
							int num4 = num2 - num3;
							if (num4 != 0)
							{
								this._InputBufferIndex = num4;
								Buffer.InternalBlockCopy(tempInputBuffer, num3, this._InputBuffer, 0, num4);
							}
							byte[] array = new byte[num3 / this._InputBlockSize * this._OutputBlockSize];
							int num5 = this._Transform.TransformBlock(tempInputBuffer, 0, num3, array, 0);
							Buffer.InternalBlockCopy(array, 0, buffer, currentOutputIndex, num5);
							Array.Clear(tempInputBuffer, 0, tempInputBuffer.Length);
							Array.Clear(array, 0, array.Length);
							bytesToDeliver -= num5;
							currentOutputIndex += num5;
							tempInputBuffer = null;
						}
					}
					while (bytesToDeliver > 0)
					{
						while (this._InputBufferIndex < this._InputBlockSize)
						{
							int num2 = await this._stream.ReadAsync(this._InputBuffer, this._InputBufferIndex, this._InputBlockSize - this._InputBufferIndex, cancellationToken).ConfigureAwait(false);
							if (num2 != 0)
							{
								this._InputBufferIndex += num2;
							}
							else
							{
								byte[] array2 = this._Transform.TransformFinalBlock(this._InputBuffer, 0, this._InputBufferIndex);
								this._OutputBuffer = array2;
								this._OutputBufferIndex = array2.Length;
								this._finalBlockTransformed = true;
								if (bytesToDeliver < this._OutputBufferIndex)
								{
									Buffer.InternalBlockCopy(this._OutputBuffer, 0, buffer, currentOutputIndex, bytesToDeliver);
									this._OutputBufferIndex -= bytesToDeliver;
									Buffer.InternalBlockCopy(this._OutputBuffer, bytesToDeliver, this._OutputBuffer, 0, this._OutputBufferIndex);
									return count;
								}
								Buffer.InternalBlockCopy(this._OutputBuffer, 0, buffer, currentOutputIndex, this._OutputBufferIndex);
								bytesToDeliver -= this._OutputBufferIndex;
								this._OutputBufferIndex = 0;
								return count - bytesToDeliver;
							}
						}
						int num5 = this._Transform.TransformBlock(this._InputBuffer, 0, this._InputBlockSize, this._OutputBuffer, 0);
						this._InputBufferIndex = 0;
						if (bytesToDeliver < num5)
						{
							Buffer.InternalBlockCopy(this._OutputBuffer, 0, buffer, currentOutputIndex, bytesToDeliver);
							this._OutputBufferIndex = num5 - bytesToDeliver;
							Buffer.InternalBlockCopy(this._OutputBuffer, bytesToDeliver, this._OutputBuffer, 0, this._OutputBufferIndex);
							return count;
						}
						Buffer.InternalBlockCopy(this._OutputBuffer, 0, buffer, currentOutputIndex, num5);
						currentOutputIndex += num5;
						bytesToDeliver -= num5;
					}
					result = count;
				}
			}
			finally
			{
				sem.Release();
			}
			return result;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_UnwritableStream"));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			int i = count;
			int num = offset;
			if (this._InputBufferIndex > 0)
			{
				if (count < this._InputBlockSize - this._InputBufferIndex)
				{
					Buffer.InternalBlockCopy(buffer, offset, this._InputBuffer, this._InputBufferIndex, count);
					this._InputBufferIndex += count;
					return;
				}
				Buffer.InternalBlockCopy(buffer, offset, this._InputBuffer, this._InputBufferIndex, this._InputBlockSize - this._InputBufferIndex);
				num += this._InputBlockSize - this._InputBufferIndex;
				i -= this._InputBlockSize - this._InputBufferIndex;
				this._InputBufferIndex = this._InputBlockSize;
			}
			if (this._OutputBufferIndex > 0)
			{
				this._stream.Write(this._OutputBuffer, 0, this._OutputBufferIndex);
				this._OutputBufferIndex = 0;
			}
			if (this._InputBufferIndex == this._InputBlockSize)
			{
				int count2 = this._Transform.TransformBlock(this._InputBuffer, 0, this._InputBlockSize, this._OutputBuffer, 0);
				this._stream.Write(this._OutputBuffer, 0, count2);
				this._InputBufferIndex = 0;
			}
			while (i > 0)
			{
				if (i < this._InputBlockSize)
				{
					Buffer.InternalBlockCopy(buffer, num, this._InputBuffer, 0, i);
					this._InputBufferIndex += i;
					return;
				}
				if (this._Transform.CanTransformMultipleBlocks)
				{
					int num2 = i / this._InputBlockSize;
					int num3 = num2 * this._InputBlockSize;
					byte[] array = new byte[num2 * this._OutputBlockSize];
					int count2 = this._Transform.TransformBlock(buffer, num, num3, array, 0);
					this._stream.Write(array, 0, count2);
					num += num3;
					i -= num3;
				}
				else
				{
					int count2 = this._Transform.TransformBlock(buffer, num, this._InputBlockSize, this._OutputBuffer, 0);
					this._stream.Write(this._OutputBuffer, 0, count2);
					num += this._InputBlockSize;
					i -= this._InputBlockSize;
				}
			}
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_UnwritableStream"));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			if (base.GetType() != typeof(CryptoStream))
			{
				return base.WriteAsync(buffer, offset, count, cancellationToken);
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCancellation(cancellationToken);
			}
			return this.WriteAsyncInternal(buffer, offset, count, cancellationToken);
		}

		private async Task WriteAsyncInternal(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			await default(CryptoStream.HopToThreadPoolAwaitable);
			SemaphoreSlim sem = base.EnsureAsyncActiveSemaphoreInitialized();
			await sem.WaitAsync().ConfigureAwait(false);
			try
			{
				int bytesToWrite = count;
				int currentInputIndex = offset;
				if (this._InputBufferIndex > 0)
				{
					if (count < this._InputBlockSize - this._InputBufferIndex)
					{
						Buffer.InternalBlockCopy(buffer, offset, this._InputBuffer, this._InputBufferIndex, count);
						this._InputBufferIndex += count;
						return;
					}
					Buffer.InternalBlockCopy(buffer, offset, this._InputBuffer, this._InputBufferIndex, this._InputBlockSize - this._InputBufferIndex);
					currentInputIndex += this._InputBlockSize - this._InputBufferIndex;
					bytesToWrite -= this._InputBlockSize - this._InputBufferIndex;
					this._InputBufferIndex = this._InputBlockSize;
				}
				if (this._OutputBufferIndex > 0)
				{
					await this._stream.WriteAsync(this._OutputBuffer, 0, this._OutputBufferIndex, cancellationToken).ConfigureAwait(false);
					this._OutputBufferIndex = 0;
				}
				if (this._InputBufferIndex == this._InputBlockSize)
				{
					int count2 = this._Transform.TransformBlock(this._InputBuffer, 0, this._InputBlockSize, this._OutputBuffer, 0);
					await this._stream.WriteAsync(this._OutputBuffer, 0, count2, cancellationToken).ConfigureAwait(false);
					this._InputBufferIndex = 0;
				}
				while (bytesToWrite > 0)
				{
					if (bytesToWrite < this._InputBlockSize)
					{
						Buffer.InternalBlockCopy(buffer, currentInputIndex, this._InputBuffer, 0, bytesToWrite);
						this._InputBufferIndex += bytesToWrite;
						break;
					}
					if (this._Transform.CanTransformMultipleBlocks)
					{
						int num = bytesToWrite / this._InputBlockSize;
						int numWholeBlocksInBytes = num * this._InputBlockSize;
						byte[] array = new byte[num * this._OutputBlockSize];
						int count2 = this._Transform.TransformBlock(buffer, currentInputIndex, numWholeBlocksInBytes, array, 0);
						await this._stream.WriteAsync(array, 0, count2, cancellationToken).ConfigureAwait(false);
						currentInputIndex += numWholeBlocksInBytes;
						bytesToWrite -= numWholeBlocksInBytes;
					}
					else
					{
						int count2 = this._Transform.TransformBlock(buffer, currentInputIndex, this._InputBlockSize, this._OutputBuffer, 0);
						await this._stream.WriteAsync(this._OutputBuffer, 0, count2, cancellationToken).ConfigureAwait(false);
						currentInputIndex += this._InputBlockSize;
						bytesToWrite -= this._InputBlockSize;
					}
				}
			}
			finally
			{
				sem.Release();
			}
		}

		public void Clear()
		{
			this.Close();
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (!this._finalBlockTransformed)
					{
						this.FlushFinalBlock();
					}
					if (!this._leaveOpen)
					{
						this._stream.Close();
					}
				}
			}
			finally
			{
				try
				{
					this._finalBlockTransformed = true;
					if (this._InputBuffer != null)
					{
						Array.Clear(this._InputBuffer, 0, this._InputBuffer.Length);
					}
					if (this._OutputBuffer != null)
					{
						Array.Clear(this._OutputBuffer, 0, this._OutputBuffer.Length);
					}
					this._InputBuffer = null;
					this._OutputBuffer = null;
					this._canRead = false;
					this._canWrite = false;
				}
				finally
				{
					base.Dispose(disposing);
				}
			}
		}

		private void InitializeBuffer()
		{
			if (this._Transform != null)
			{
				this._InputBlockSize = this._Transform.InputBlockSize;
				this._InputBuffer = new byte[this._InputBlockSize];
				this._OutputBlockSize = this._Transform.OutputBlockSize;
				this._OutputBuffer = new byte[this._OutputBlockSize];
			}
		}

		private Stream _stream;

		private ICryptoTransform _Transform;

		private byte[] _InputBuffer;

		private int _InputBufferIndex;

		private int _InputBlockSize;

		private byte[] _OutputBuffer;

		private int _OutputBufferIndex;

		private int _OutputBlockSize;

		private CryptoStreamMode _transformMode;

		private bool _canRead;

		private bool _canWrite;

		private bool _finalBlockTransformed;

		private bool _leaveOpen;

		private struct HopToThreadPoolAwaitable : INotifyCompletion
		{
			public CryptoStream.HopToThreadPoolAwaitable GetAwaiter()
			{
				return this;
			}

			public bool IsCompleted
			{
				get
				{
					return false;
				}
			}

			public void OnCompleted(Action continuation)
			{
				Task.Run(continuation);
			}

			public void GetResult()
			{
			}
		}
	}
}
