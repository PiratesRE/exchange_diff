using System;
using System.Security;
using System.Web;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class SecureHttpBuffer : DisposeTrackableBase
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
			this.buffer = new SecureArray<char>(new char[size]);
			this.response = response;
			this.currentPosition = 0;
		}

		public int Size
		{
			get
			{
				base.CheckDisposed();
				return this.buffer.ArrayValue.Length;
			}
		}

		public void CopyAtCurrentPosition(string value)
		{
			base.CheckDisposed();
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.AdjustSizeAtCurrentPosition(value.Length);
			value.CopyTo(0, this.buffer.ArrayValue, this.currentPosition, value.Length);
			this.currentPosition += value.Length;
		}

		public void CopyAtCurrentPosition(SecureString secureValue)
		{
			base.CheckDisposed();
			if (secureValue == null)
			{
				throw new ArgumentNullException("secureValue");
			}
			using (SecureArray<char> secureArray = secureValue.ConvertToSecureCharArray())
			{
				this.CopyAtCurrentPosition(secureArray);
			}
		}

		public void CopyAtCurrentPosition(SecureArray<char> secureArray)
		{
			base.CheckDisposed();
			if (secureArray == null)
			{
				throw new ArgumentNullException("secureArray");
			}
			this.AdjustSizeAtCurrentPosition(secureArray.ArrayValue.Length);
			secureArray.ArrayValue.CopyTo(this.buffer.ArrayValue, this.currentPosition);
			this.currentPosition += secureArray.ArrayValue.Length;
		}

		public void Flush()
		{
			base.CheckDisposed();
			if (this.currentPosition > 0)
			{
				this.response.Write(this.buffer.ArrayValue, 0, this.currentPosition);
				Array.Clear(this.buffer.ArrayValue, 0, this.buffer.ArrayValue.Length);
				this.currentPosition = 0;
				this.response.Flush();
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.buffer != null)
			{
				this.Flush();
				this.buffer.Dispose();
				this.buffer = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SecureHttpBuffer>(this);
		}

		private void AdjustSizeAtCurrentPosition(int length)
		{
			int num = this.currentPosition + 1 + length;
			if (num > this.Size)
			{
				this.Resize(Math.Max(this.Size * 2, num));
			}
		}

		private void Resize(int newSize)
		{
			using (SecureArray<char> secureArray = this.buffer)
			{
				this.buffer = new SecureArray<char>(newSize);
				secureArray.ArrayValue.CopyTo(this.buffer.ArrayValue, 0);
			}
		}

		private SecureArray<char> buffer;

		private int currentPosition;

		private HttpResponse response;
	}
}
