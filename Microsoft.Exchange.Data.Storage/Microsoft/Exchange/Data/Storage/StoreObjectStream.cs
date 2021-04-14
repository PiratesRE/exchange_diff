using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class StoreObjectStream : Stream, IDisposeTrackable, IDisposable
	{
		internal StoreObjectStream(StoreObjectPropertyBag storePropertyBag, NativeStorePropertyDefinition property, PropertyOpenMode streamOpenMode)
		{
			Util.ThrowOnNullArgument(storePropertyBag, "storePropertyBag");
			Util.ThrowOnNullArgument(property, "property");
			this.storePropertyBag = storePropertyBag;
			this.property = property;
			this.openMode = streamOpenMode;
			this.CreateStream();
			this.disposeTracker = this.GetDisposeTracker();
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<StoreObjectStream>(this);
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
				return this.ActiveStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				this.CheckDisposed();
				return this.ActiveStream.Position;
			}
			set
			{
				this.CheckDisposed();
				Util.ThrowOnArgumentOutOfRangeOnLessThan(value, 0L, "Position");
				this.ActiveStream.Position = value;
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed();
			Util.ThrowOnNullArgument(buffer, "buffer");
			Util.ThrowOnArgumentOutOfRangeOnLessThan(offset, 0, "offset");
			Util.ThrowOnArgumentInvalidOnGreaterThan(offset, buffer.Length, "offset");
			Util.ThrowOnArgumentOutOfRangeOnLessThan(count, 0, "count");
			Util.ThrowOnArgumentInvalidOnGreaterThan(offset + count, buffer.Length, "offset + count exceeds buffer size");
			this.CheckNotReadOnly();
			if (count == 0)
			{
				return;
			}
			ExtendedRuleConditionConstraint.ValidateStreamIfApplicable(this.ActiveStream.Position + (long)count, this.property, this.storePropertyBag);
			this.dataChanged = true;
			if (this.cache != null && this.NeedToSwitchToMapiStream(this.cache.Position + (long)count))
			{
				this.OpenMapiStream(true);
			}
			this.ActiveStream.Write(buffer, offset, count);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed();
			Util.ThrowOnNullArgument(buffer, "buffer");
			Util.ThrowOnArgumentOutOfRangeOnLessThan(offset, 0, "offset");
			Util.ThrowOnArgumentInvalidOnGreaterThan(offset, buffer.Length, "offset");
			Util.ThrowOnArgumentOutOfRangeOnLessThan(count, 0, "count");
			Util.ThrowOnArgumentInvalidOnGreaterThan(offset + count, buffer.Length, "offset + count exceeds buffer size");
			return this.ActiveStream.Read(buffer, offset, count);
		}

		public override void SetLength(long length)
		{
			this.CheckDisposed();
			this.CheckNotReadOnly();
			Util.ThrowOnArgumentOutOfRangeOnLessThan(length, 0L, "length");
			ExtendedRuleConditionConstraint.ValidateStreamIfApplicable(length, this.property, this.storePropertyBag);
			if (this.NeedToSwitchToMapiStream(length))
			{
				this.OpenMapiStream(false);
			}
			this.dataChanged = true;
			this.ActiveStream.SetLength(length);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.CheckDisposed();
			return this.ActiveStream.Seek(offset, origin);
		}

		public override void Flush()
		{
			this.CheckDisposed();
			if (this.mapiStream != null)
			{
				this.mapiStream.Flush();
			}
			else if (this.dataChanged)
			{
				this.FlushCacheIntoPropertyBag();
			}
			this.dataChanged = false;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
					}
					try
					{
						if (this.cache != null && this.dataChanged)
						{
							this.FlushCacheIntoPropertyBag();
						}
						if (this.ActiveStream != null)
						{
							this.ActiveStream.Dispose();
						}
					}
					finally
					{
						this.OnStreamClose();
						this.cache = null;
						if (this.mapiStream != null)
						{
							this.mapiStream.Dispose();
							this.mapiStream = null;
						}
						this.dataChanged = false;
						this.isClosed = true;
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private static int GetByteCount(byte[] content)
		{
			int num = 0;
			while (num < content.Length - 1 && (content[num] != 0 || content[num + 1] != 0))
			{
				num += 2;
			}
			return num;
		}

		private static Encoding GetUnicodeEncoding()
		{
			return new UnicodeEncoding(false, false);
		}

		private static PooledMemoryStream CreateExpandableMemoryStream(byte[] bytes)
		{
			PooledMemoryStream pooledMemoryStream;
			if (bytes != null)
			{
				pooledMemoryStream = new PooledMemoryStream(bytes.Length);
				pooledMemoryStream.Write(bytes, 0, bytes.Length);
				pooledMemoryStream.Seek(0L, SeekOrigin.Begin);
			}
			else
			{
				pooledMemoryStream = new PooledMemoryStream(16384);
			}
			return pooledMemoryStream;
		}

		private void CreateStream()
		{
			if ((this.property.PropertyFlags & PropertyFlags.Streamable) == PropertyFlags.None)
			{
				ExTraceGlobals.PropertyBagTracer.TraceDebug<NativeStorePropertyDefinition>(0L, "StoreObjectStream::CreateStream property {0} is not marked as streamable.", this.property);
			}
			if (this.property.MapiPropertyType == PropType.Object)
			{
				this.OpenMapiStream(false);
				return;
			}
			if (this.property.MapiPropertyType != PropType.String && this.property.MapiPropertyType != PropType.Binary)
			{
				throw new InvalidOperationException(ServerStrings.ExPropertyNotStreamable(this.property.ToString()));
			}
			switch (this.openMode)
			{
			case PropertyOpenMode.ReadOnly:
			case PropertyOpenMode.Modify:
			{
				if (!((IDirectPropertyBag)this.storePropertyBag.MemoryPropertyBag).IsLoaded(this.property))
				{
					this.OpenMapiStream(false);
					return;
				}
				object obj = this.storePropertyBag.MemoryPropertyBag.TryGetProperty(this.property);
				PropertyError propertyError = obj as PropertyError;
				if (propertyError == null)
				{
					this.CreateCache(obj);
					return;
				}
				if (PropertyError.IsPropertyValueTooBig(obj))
				{
					this.OpenMapiStream(false);
					return;
				}
				if (propertyError.PropertyErrorCode != PropertyErrorCode.NotFound)
				{
					throw PropertyError.ToException(new PropertyError[]
					{
						propertyError
					});
				}
				if (Array.IndexOf<StorePropertyDefinition>(StoreObjectStream.bodyProperties, this.property) != -1)
				{
					this.OpenMapiStream(false);
					return;
				}
				if (this.openMode != PropertyOpenMode.Modify)
				{
					throw new ObjectNotFoundException(ServerStrings.ExUnableToGetStreamProperty(this.property.Name));
				}
				break;
			}
			case PropertyOpenMode.Create:
				break;
			default:
				return;
			}
			this.CreateCache(null);
			this.dataChanged = true;
		}

		private void OpenMapiStream(bool flushCacheToPosition)
		{
			PropertyOpenMode propertyOpenMode = this.openMode;
			if (propertyOpenMode == PropertyOpenMode.Modify && this.cache != null)
			{
				propertyOpenMode = PropertyOpenMode.Create;
			}
			Stream stream = null;
			try
			{
				ExTraceGlobals.PropertyBagTracer.TraceDebug(0L, "StoreObejctStream::OpenMapiStream. Open MapiStream. property = {0}, localOpenMode = {1}, openMode = {2}, flushCacheToPosition = {3}.", new object[]
				{
					this.property,
					propertyOpenMode,
					this.openMode,
					flushCacheToPosition
				});
				stream = this.storePropertyBag.MapiPropertyBag.OpenPropertyStream(this.property, propertyOpenMode);
				if (this.cache != null && (propertyOpenMode == PropertyOpenMode.Create || propertyOpenMode == PropertyOpenMode.Modify))
				{
					long position = this.cache.Position;
					if (this.cache.Length > 0L)
					{
						this.cache.Position = 0L;
						Util.StreamHandler.CopyStreamData(this.cache, stream, new int?((int)this.cache.Length));
					}
					if (position != this.cache.Length)
					{
						stream.Position = position;
					}
					this.cache.Dispose();
					this.cache = null;
				}
				if (this.openMode != PropertyOpenMode.ReadOnly)
				{
					this.SetPropertyRequiresStreaming();
				}
				this.mapiStream = stream;
				stream = null;
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
				}
			}
		}

		private bool NeedToSwitchToMapiStream(long streamLocation)
		{
			return this.cache != null && this.openMode != PropertyOpenMode.ReadOnly && streamLocation > (long)this.maximumCacheSize;
		}

		private void CreateCache(object value)
		{
			if (value == null)
			{
				this.cache = StoreObjectStream.CreateExpandableMemoryStream(null);
			}
			else if (this.property.MapiPropertyType == PropType.Binary)
			{
				this.cache = StoreObjectStream.CreateExpandableMemoryStream((byte[])value);
			}
			else if (this.property.MapiPropertyType == PropType.String)
			{
				byte[] bytes = StoreObjectStream.GetUnicodeEncoding().GetBytes((string)value);
				PooledMemoryStream pooledMemoryStream = StoreObjectStream.CreateExpandableMemoryStream(bytes);
				pooledMemoryStream.Position = 0L;
				this.cache = pooledMemoryStream;
			}
			this.maximumCacheSize = ((32768 > (int)this.cache.Length) ? 32768 : ((int)this.cache.Length));
		}

		private void FlushCacheIntoPropertyBag()
		{
			if (this.storePropertyBag == null)
			{
				return;
			}
			this.storePropertyBag.Load(null);
			byte[] array = this.cache.ToArray();
			if (this.property.MapiPropertyType == PropType.Binary)
			{
				this.storePropertyBag.MemoryPropertyBag[this.property] = array;
				return;
			}
			if (this.property.MapiPropertyType == PropType.String)
			{
				this.storePropertyBag.MemoryPropertyBag[this.property] = StoreObjectStream.GetUnicodeEncoding().GetString(array, 0, StoreObjectStream.GetByteCount(array));
			}
		}

		private void SetPropertyRequiresStreaming()
		{
			this.storePropertyBag.MemoryPropertyBag.MarkPropertyAsRequireStreamed(this.property);
		}

		private void CheckNotReadOnly()
		{
			if (this.openMode == PropertyOpenMode.ReadOnly)
			{
				throw new NotSupportedException();
			}
			if (this.storePropertyBag == null)
			{
				throw new InvalidOperationException();
			}
		}

		private void CheckDisposed()
		{
			if (this.isClosed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private Stream ActiveStream
		{
			get
			{
				return this.mapiStream ?? this.cache;
			}
		}

		internal void DetachPropertyBag()
		{
			this.storePropertyBag = null;
		}

		private void OnStreamClose()
		{
			if (this.storePropertyBag != null)
			{
				this.storePropertyBag.OnStreamClose(this);
				this.storePropertyBag = null;
			}
		}

		private const int StandardMaximumCacheSize = 32768;

		private const int InitialCacheSize = 16384;

		private static readonly StorePropertyDefinition[] bodyProperties = new StorePropertyDefinition[]
		{
			InternalSchema.TextBody,
			InternalSchema.RtfBody,
			InternalSchema.HtmlBody
		};

		private readonly DisposeTracker disposeTracker;

		private bool isClosed;

		private StoreObjectPropertyBag storePropertyBag;

		private PropertyOpenMode openMode;

		private NativeStorePropertyDefinition property;

		private PooledMemoryStream cache;

		private Stream mapiStream;

		private int maximumCacheSize;

		private bool dataChanged;
	}
}
