using System;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BodyReadStream : StreamBase, Body.IBodyStream
	{
		private BodyReadStream(Stream bodyStream, Stream readStream, ConversionCallbackBase conversionCallbacks) : this(bodyStream, readStream, conversionCallbacks, null)
		{
		}

		private BodyReadStream(Stream bodyStream, Stream readStream, ConversionCallbackBase conversionCallbacks, long? length) : base(StreamBase.Capabilities.Readable)
		{
			this.bodyStream = bodyStream;
			this.readStream = readStream;
			this.conversionCallbacks = conversionCallbacks;
			this.length = length;
			this.position = 0L;
		}

		internal static BodyReadStream TryCreateBodyReadStream(ICoreItem coreItem, BodyReadConfiguration configuration, bool createEmtpyStreamIfNotFound)
		{
			BodyReadStream bodyReadStream = BodyReadStream.InternalTryCreateBodyStream(coreItem, configuration, createEmtpyStreamIfNotFound, null);
			if (configuration.ShouldCalculateLength)
			{
				long streamLength = BodyReadStream.GetStreamLength(bodyReadStream);
				bodyReadStream.Dispose();
				bodyReadStream = BodyReadStream.InternalTryCreateBodyStream(coreItem, configuration, createEmtpyStreamIfNotFound, new long?(streamLength));
			}
			return bodyReadStream;
		}

		private static long GetStreamLength(BodyReadStream stream)
		{
			if (stream == null)
			{
				return 0L;
			}
			long result;
			using (Stream stream2 = new BodyStreamSizeCounter(null))
			{
				result = Util.StreamHandler.CopyStreamData(stream, stream2);
			}
			return result;
		}

		private static BodyReadStream InternalTryCreateBodyStream(ICoreItem coreItem, BodyReadConfiguration configuration, bool createEmtpyStreamIfNotFound, long? length)
		{
			BodyReadStream result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				Stream stream = BodyReadStream.OpenBodyStream(coreItem);
				disposeGuard.Add<Stream>(stream);
				Stream stream2 = stream;
				if (stream2 == null)
				{
					if (!createEmtpyStreamIfNotFound)
					{
						return null;
					}
					stream2 = Body.GetEmptyStream();
				}
				ConversionCallbackBase conversionCallbackBase;
				Stream disposable = BodyReadDelegates.CreateStream(coreItem, configuration, stream2, out conversionCallbackBase);
				disposeGuard.Add<Stream>(disposable);
				BodyReadStream bodyReadStream = new BodyReadStream(stream, disposable, conversionCallbackBase, length);
				disposeGuard.Add<BodyReadStream>(bodyReadStream);
				disposeGuard.Success();
				result = bodyReadStream;
			}
			return result;
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<BodyReadStream>(this);
		}

		public ReadOnlyCollection<AttachmentLink> AttachmentLinks
		{
			get
			{
				this.CheckDisposed();
				if (this.conversionCallbacks != null)
				{
					return this.conversionCallbacks.AttachmentLinks;
				}
				return null;
			}
		}

		internal static Stream OpenBodyStream(ICoreItem coreItem)
		{
			if (coreItem.Session != null && !coreItem.AreOptionalAutoloadPropertiesLoaded)
			{
				throw new NotInBagPropertyErrorException(InternalSchema.TextBody);
			}
			if (!coreItem.Body.IsBodyDefined)
			{
				return null;
			}
			StorePropertyDefinition bodyProperty = Body.GetBodyProperty(coreItem.Body.RawFormat);
			Stream result;
			try
			{
				result = coreItem.Body.InternalOpenBodyStream(bodyProperty, PropertyOpenMode.ReadOnly);
			}
			catch (ObjectNotFoundException arg)
			{
				ExTraceGlobals.CcBodyTracer.TraceError<ObjectNotFoundException>((long)coreItem.GetHashCode(), "BodyReadStream.OpenBodyStream - ObjectNotFoundException caught {0}", arg);
				result = null;
			}
			return result;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed();
			return ConvertUtils.CallCtsWithReturnValue<int>(ExTraceGlobals.CcBodyTracer, "BodyReadStream::Read", ServerStrings.ConversionBodyConversionFailed, delegate
			{
				int num = this.readStream.Read(buffer, offset, count);
				if (num > 0)
				{
					this.position += (long)num;
				}
				return num;
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

		public override long Length
		{
			get
			{
				if (this.length != null)
				{
					return this.length.Value;
				}
				return base.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.position;
			}
		}

		private void CloseStream()
		{
			if (this.readStream != null)
			{
				try
				{
					this.readStream.Dispose();
					this.readStream = null;
				}
				catch (ExchangeDataException arg)
				{
					ExTraceGlobals.CcBodyTracer.TraceError<ExchangeDataException>((long)this.GetHashCode(), "BodyReadStream.CloseStream() exception {0}", arg);
				}
			}
			if (this.bodyStream != null)
			{
				this.bodyStream.Dispose();
				this.bodyStream = null;
			}
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

		private readonly ConversionCallbackBase conversionCallbacks;

		private Stream bodyStream;

		private Stream readStream;

		private bool isDisposed;

		private long? length;

		private long position;
	}
}
