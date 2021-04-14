using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ByteQueue
	{
		public ByteQueue(int size)
		{
			this.buffer = new byte[size];
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public void Enqueue(byte[] inputBuffer)
		{
			this.EnqueueInternal(inputBuffer.Length, inputBuffer, 0);
		}

		public void Enqueue(int inputCount, byte[] inputBuffer, int inputOffset)
		{
			this.EnqueueInternal(inputCount, inputBuffer, inputOffset);
		}

		public byte[] Dequeue(int count)
		{
			int num = Math.Min(count, this.count);
			byte[] array = new byte[num];
			this.DequeueInternal(array.Length, array, 0);
			return array;
		}

		public int Dequeue(byte[] outputBuffer)
		{
			return this.DequeueInternal(outputBuffer.Length, outputBuffer, 0);
		}

		public int Dequeue(int outputBufferSize, byte[] outputBuffer, int outputOffset)
		{
			return this.DequeueInternal(outputBufferSize, outputBuffer, outputOffset);
		}

		private void EnqueueInternal(int inputCount, byte[] inputBuffer, int inputOffset)
		{
			int num = this.offset + this.count;
			while (inputCount > 0)
			{
				if (num >= this.buffer.Length)
				{
					num -= this.buffer.Length;
				}
				this.buffer[num] = inputBuffer[inputOffset];
				num++;
				this.count++;
				inputOffset++;
				inputCount--;
			}
		}

		private int DequeueInternal(int outputBufferSize, byte[] outputBuffer, int outputOffset)
		{
			int num = 0;
			while (outputBufferSize > 0 && this.count > 0)
			{
				if (this.offset >= this.buffer.Length)
				{
					this.offset -= this.buffer.Length;
				}
				outputBuffer[outputOffset] = this.buffer[this.offset];
				this.offset++;
				this.count--;
				outputOffset++;
				outputBufferSize--;
				num++;
			}
			return num;
		}

		private byte[] buffer;

		private int offset;

		private int count;
	}
}
