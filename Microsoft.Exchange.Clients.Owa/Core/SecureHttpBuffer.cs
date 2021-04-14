using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Web;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class SecureHttpBuffer
	{
		public SecureHttpBuffer(int size, HttpResponse response)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			if (size < 0)
			{
				throw new ArgumentException("Size is not valid");
			}
			this.buffer = new char[size];
			this.size = size;
			this.response = response;
		}

		public int Size
		{
			get
			{
				return this.buffer.Length;
			}
			set
			{
				if (value != this.buffer.Length)
				{
					if (value < this.size)
					{
						throw new ArgumentException("new Value is smaller than the current list size");
					}
					if (value == 0)
					{
						Array.Clear(this.buffer, 0, this.size);
						this.buffer = new char[0];
						return;
					}
					char[] destinationArray = new char[value];
					if (this.size > 0)
					{
						Array.Copy(this.buffer, 0, destinationArray, 0, this.size);
						Array.Clear(this.buffer, 0, this.size);
					}
					this.buffer = destinationArray;
				}
			}
		}

		public void CopyAtCurrentPosition(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.AdjustSizeAtCurrentPosition(value.Length);
			value.CopyTo(0, this.buffer, this.currentPosition, value.Length);
			this.currentPosition += value.Length;
		}

		public void CopyAtCurrentPosition(SecureString secureValue)
		{
			if (secureValue == null)
			{
				throw new ArgumentNullException("secureValue");
			}
			this.AdjustSizeAtCurrentPosition(secureValue.Length);
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				intPtr = Marshal.SecureStringToBSTR(secureValue);
				Marshal.Copy(intPtr, this.buffer, this.currentPosition, secureValue.Length);
				this.currentPosition += secureValue.Length;
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.ZeroFreeBSTR(intPtr);
				}
			}
		}

		private void AdjustSizeAtCurrentPosition(int length)
		{
			int num = this.currentPosition + 1 + length;
			if (num > this.size)
			{
				if (num < 2 * this.size)
				{
					this.Size = 2 * this.size;
					return;
				}
				this.Size = num;
			}
		}

		public void FlushBuffer()
		{
			this.response.Write(this.buffer, 0, this.currentPosition + 1);
			Array.Clear(this.buffer, 0, this.currentPosition + 1);
			this.response.Flush();
		}

		private char[] buffer;

		private int size;

		private int currentPosition;

		private HttpResponse response;
	}
}
