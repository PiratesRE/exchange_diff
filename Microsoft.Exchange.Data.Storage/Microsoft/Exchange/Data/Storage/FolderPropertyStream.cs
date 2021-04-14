using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FolderPropertyStream : LockableStream, IDisposeTrackable, IDisposable
	{
		internal FolderPropertyStream(MapiPropertyBag mapiPropertyBag, NativeStorePropertyDefinition property, PropertyOpenMode streamOpenMode)
		{
			Util.ThrowOnNullArgument(mapiPropertyBag, "mapiPropertyBag");
			Util.ThrowOnNullArgument(property, "property");
			EnumValidator.ThrowIfInvalid<PropertyOpenMode>(streamOpenMode, "streamOpenMode");
			this.mapiPropertyBag = mapiPropertyBag;
			this.property = property;
			this.openMode = streamOpenMode;
			this.CreateStream();
			this.disposeTracker = this.GetDisposeTracker();
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FolderPropertyStream>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public override bool CanRead
		{
			get
			{
				this.CheckDisposed();
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				this.CheckDisposed();
				return this.openMode == PropertyOpenMode.Modify || this.openMode == PropertyOpenMode.Create;
			}
		}

		public override bool CanSeek
		{
			get
			{
				this.CheckDisposed();
				return true;
			}
		}

		public override long Length
		{
			get
			{
				this.CheckDisposed();
				return this.mapiStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				this.CheckDisposed();
				return this.mapiStream.Position;
			}
			set
			{
				this.CheckDisposed();
				Util.ThrowOnArgumentOutOfRangeOnLessThan(value, 0L, "Position");
				this.mapiStream.Position = value;
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed();
			Util.ThrowOnNullArgument(buffer, "buffer");
			Util.ThrowOnArgumentOutOfRangeOnLessThan(offset, 0, "offset");
			Util.ThrowOnArgumentInvalidOnGreaterThan(offset, buffer.Length, "offset");
			Util.ThrowOnArgumentOutOfRangeOnLessThan(count, 0, "count");
			Util.ThrowOnArgumentInvalidOnGreaterThan(offset + count, buffer.Length, "count");
			if (this.openMode == PropertyOpenMode.ReadOnly)
			{
				throw new NotSupportedException();
			}
			if (count == 0)
			{
				return;
			}
			this.mapiStream.Write(buffer, offset, count);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed();
			Util.ThrowOnNullArgument(buffer, "buffer");
			Util.ThrowOnArgumentOutOfRangeOnLessThan(offset, 0, "offset");
			Util.ThrowOnArgumentInvalidOnGreaterThan(offset, buffer.Length, "offset");
			Util.ThrowOnArgumentOutOfRangeOnLessThan(count, 0, "count");
			Util.ThrowOnArgumentInvalidOnGreaterThan(offset + count, buffer.Length, "count");
			if (count == 0)
			{
				return 0;
			}
			return this.mapiStream.Read(buffer, offset, count);
		}

		public override void SetLength(long length)
		{
			this.CheckDisposed();
			if (this.openMode == PropertyOpenMode.ReadOnly)
			{
				throw new NotSupportedException();
			}
			Util.ThrowOnArgumentOutOfRangeOnLessThan(length, 0L, "length");
			this.mapiStream.SetLength(length);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.CheckDisposed();
			return this.mapiStream.Seek(offset, origin);
		}

		public override void Flush()
		{
			this.CheckDisposed();
			this.mapiStream.Flush();
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					try
					{
						if (this.disposeTracker != null)
						{
							this.disposeTracker.Dispose();
						}
						if (this.mapiStream != null)
						{
							this.mapiStream.Dispose();
							this.mapiStream = null;
						}
					}
					finally
					{
						this.mapiStream = null;
						this.isClosed = true;
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override void LockRegion(long offset, long cb, int lockType)
		{
			this.CheckDisposed();
			Util.ThrowOnArgumentOutOfRangeOnLessThan(offset, 0L, "offset");
			Util.ThrowOnArgumentOutOfRangeOnLessThan(cb, 0L, "cb");
			this.mapiStream.LockRegion(offset, cb, lockType);
		}

		public override void UnlockRegion(long offset, long cb, int lockType)
		{
			this.CheckDisposed();
			Util.ThrowOnArgumentOutOfRangeOnLessThan(offset, 0L, "offset");
			Util.ThrowOnArgumentOutOfRangeOnLessThan(cb, 0L, "cb");
			this.mapiStream.UnlockRegion(offset, cb, lockType);
		}

		private void CreateStream()
		{
			if (this.property.MapiPropertyType != PropType.String && this.property.MapiPropertyType != PropType.Binary)
			{
				throw new InvalidOperationException(ServerStrings.ExPropertyNotStreamable(this.property.ToString()));
			}
			Stream stream = null;
			try
			{
				ExTraceGlobals.PropertyBagTracer.TraceDebug<NativeStorePropertyDefinition, PropertyOpenMode>(0L, "FolderPropertyStream::CreateStream property = {0}, openMode = {1}.", this.property, this.openMode);
				if ((this.property.PropertyFlags & PropertyFlags.Streamable) == PropertyFlags.None)
				{
					ExTraceGlobals.PropertyBagTracer.TraceDebug<NativeStorePropertyDefinition>(0L, "FolderPropertyStream::CreateStream property {0} is not marked as streamable.", this.property);
				}
				stream = this.mapiPropertyBag.OpenPropertyStream(this.property, this.openMode, false);
			}
			catch (ObjectNotFoundException)
			{
				if (this.openMode != PropertyOpenMode.Modify)
				{
					throw new ObjectNotFoundException(ServerStrings.ExUnableToGetStreamProperty(this.property.Name));
				}
				stream = this.mapiPropertyBag.OpenPropertyStream(this.property, PropertyOpenMode.Create, false);
			}
			if (stream == null)
			{
				throw new InvalidOperationException("Received null from a call to OpenPropertyStream!");
			}
			this.mapiStream = (stream as MapiStreamWrapper);
			if (this.mapiStream == null)
			{
				throw new InvalidOperationException("MapiPropertyBag.OpenPropertyStream did not return a MapiStreamWrapper!");
			}
		}

		private void CheckDisposed()
		{
			if (this.isClosed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private readonly MapiPropertyBag mapiPropertyBag;

		private readonly PropertyOpenMode openMode;

		private readonly NativeStorePropertyDefinition property;

		private readonly DisposeTracker disposeTracker;

		private bool isClosed;

		private MapiStreamWrapper mapiStream;
	}
}
