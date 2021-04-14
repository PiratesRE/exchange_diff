using System;
using System.IO;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class RelayStorage : DataStorage
	{
		internal RelayStorage(IRelayable relayable)
		{
			this.relayable = relayable;
		}

		public override Stream OpenReadStream(long start, long end)
		{
			base.ThrowIfDisposed();
			if (this.temporaryStorage == null)
			{
				this.Relay();
			}
			return this.temporaryStorage.OpenReadStream(start, end);
		}

		internal override void SetReadOnly(bool makeReadOnly)
		{
			base.ThrowIfDisposed();
			base.SetReadOnly(makeReadOnly);
			if (this.temporaryStorage != null)
			{
				this.temporaryStorage.SetReadOnly(makeReadOnly);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !base.IsDisposed && this.relayable != null)
			{
				this.Invalidate();
				this.relayable = null;
			}
			base.Dispose(disposing);
		}

		internal void Invalidate()
		{
			base.ThrowIfDisposed();
			if (this.temporaryStorage != null)
			{
				this.temporaryStorage.Release();
				this.temporaryStorage = null;
			}
		}

		internal void PermanentlyRelay()
		{
			base.ThrowIfDisposed();
			if (this.temporaryStorage == null)
			{
				this.Relay();
			}
			this.relayable = null;
		}

		private void Relay()
		{
			this.temporaryStorage = new TemporaryDataStorage();
			using (Stream stream = this.temporaryStorage.OpenWriteStream(true))
			{
				this.relayable.WriteTo(stream);
			}
			this.temporaryStorage.SetReadOnly(base.IsReadOnly);
		}

		private TemporaryDataStorage temporaryStorage;

		private IRelayable relayable;
	}
}
