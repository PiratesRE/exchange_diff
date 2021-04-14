using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BodyWriteStream : StreamBase, Body.IBodyStream
	{
		internal BodyWriteStream(ICoreItem coreItem, BodyWriteConfiguration configuration, Stream outputStream) : base(StreamBase.Capabilities.Writable)
		{
			this.coreItem = coreItem;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				Stream stream = null;
				if (outputStream == null)
				{
					stream = BodyWriteStream.OpenBodyStream(coreItem, configuration);
					disposeGuard.Add<Stream>(stream);
					outputStream = stream;
				}
				Stream disposable = BodyWriteDelegates.CreateStream(coreItem, configuration, outputStream, out this.conversionCallback);
				disposeGuard.Add<Stream>(disposable);
				disposeGuard.Success();
				this.writeStream = disposable;
				this.bodyStream = stream;
			}
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<BodyWriteStream>(this);
		}

		internal static Stream OpenBodyStream(ICoreItem coreItem, BodyWriteConfiguration configuration)
		{
			StorePropertyDefinition bodyProperty = Body.GetBodyProperty(configuration.TargetFormat);
			return coreItem.Body.InternalOpenBodyStream(bodyProperty, PropertyOpenMode.Create);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed();
			ConvertUtils.CallCts(ExTraceGlobals.CcBodyTracer, "BodyWriteStream::Write", ServerStrings.ConversionBodyConversionFailed, delegate
			{
				this.writeStream.Write(buffer, offset, count);
			});
		}

		public override void Flush()
		{
			this.CheckDisposed();
			ConvertUtils.CallCts(ExTraceGlobals.CcBodyTracer, "BodyWriteStream::Flush", ServerStrings.ConversionBodyConversionFailed, delegate
			{
				this.writeStream.Flush();
			});
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this.CloseStream();
					this.isDisposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private void CloseStream()
		{
			ConvertUtils.CallCts(ExTraceGlobals.CcBodyTracer, "BodyWriteStream::Close", ServerStrings.ConversionBodyConversionFailed, delegate
			{
				if (this.writeStream != null)
				{
					try
					{
						this.writeStream.Dispose();
						this.writeStream = null;
					}
					catch (ExchangeDataException ex)
					{
						ExTraceGlobals.CcBodyTracer.TraceError<ExchangeDataException>((long)this.GetHashCode(), "BodyWriteStream.CloseStream() exception {0}", ex);
						this.coreItem.Body.SetBodyStreamingException(ex);
					}
				}
			});
			if (this.bodyStream != null)
			{
				this.bodyStream.Dispose();
				this.bodyStream = null;
			}
			if (this.conversionCallback != null)
			{
				this.conversionCallback.SaveChanges();
			}
			this.conversionCallback = null;
			this.isDisposed = true;
		}

		public bool IsDisposed()
		{
			return this.isDisposed;
		}

		private void CheckDisposed()
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private Stream bodyStream;

		private Stream writeStream;

		private ConversionCallbackBase conversionCallback;

		private ICoreItem coreItem;

		private bool isDisposed;
	}
}
