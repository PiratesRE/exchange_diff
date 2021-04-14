using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BodyTextWriter : TextWriter, Body.IBodyStream, IDisposeTrackable, IDisposable
	{
		internal BodyTextWriter(ICoreItem coreItem, BodyWriteConfiguration configuration, Stream outputStream)
		{
			this.coreItem = coreItem;
			this.disposeTracker = this.GetDisposeTracker();
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				Stream stream = null;
				if (outputStream == null)
				{
					stream = new StreamWrapper(BodyWriteStream.OpenBodyStream(coreItem, configuration), true);
					disposeGuard.Add<Stream>(stream);
					outputStream = stream;
				}
				TextWriter disposable = BodyWriteDelegates.CreateWriter(coreItem, configuration, outputStream, out this.conversionCallback);
				disposeGuard.Add<TextWriter>(disposable);
				disposeGuard.Success();
				this.writer = disposable;
				this.bodyStream = stream;
			}
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<BodyTextWriter>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public override Encoding Encoding
		{
			get
			{
				this.CheckDisposed();
				return ConvertUtils.CallCtsWithReturnValue<Encoding>(ExTraceGlobals.CcBodyTracer, "BodyTextWriter::Encoding::get", ServerStrings.ConversionBodyConversionFailed, () => this.writer.Encoding);
			}
		}

		public override void Write(char value)
		{
			this.CheckDisposed();
			ConvertUtils.CallCts(ExTraceGlobals.CcBodyTracer, "BodyTextWriter::Write(char)", ServerStrings.ConversionBodyConversionFailed, delegate
			{
				this.writer.Write(value);
			});
		}

		public override void Write(char[] buffer)
		{
			this.CheckDisposed();
			ConvertUtils.CallCts(ExTraceGlobals.CcBodyTracer, "BodyTextWriter::Write(char[])", ServerStrings.ConversionBodyConversionFailed, delegate
			{
				this.writer.Write(buffer);
			});
		}

		public override void Write(char[] buffer, int index, int count)
		{
			this.CheckDisposed();
			ConvertUtils.CallCts(ExTraceGlobals.CcBodyTracer, "BodyTextWriter::Write(char[]/int/int)", ServerStrings.ConversionBodyConversionFailed, delegate
			{
				this.writer.Write(buffer, index, count);
			});
		}

		public override void Write(string value)
		{
			this.CheckDisposed();
			ConvertUtils.CallCts(ExTraceGlobals.CcBodyTracer, "BodyTextWriter::Write(string)", ServerStrings.ConversionBodyConversionFailed, delegate
			{
				this.writer.Write(value);
			});
		}

		public override void WriteLine(string value)
		{
			this.CheckDisposed();
			ConvertUtils.CallCts(ExTraceGlobals.CcBodyTracer, "BodyTextWriter::WriteLine", ServerStrings.ConversionBodyConversionFailed, delegate
			{
				this.writer.WriteLine(value);
			});
		}

		public override void Flush()
		{
			this.CheckDisposed();
			ConvertUtils.CallCts(ExTraceGlobals.CcBodyTracer, "BodyTextWriter::Flush", ServerStrings.ConversionBodyConversionFailed, delegate
			{
				this.writer.Flush();
			});
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this.CloseWriter();
				}
				this.isDisposed = true;
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private void CloseWriter()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			ConvertUtils.CallCts(ExTraceGlobals.CcBodyTracer, "BodyTextWriter::Close", ServerStrings.ConversionBodyConversionFailed, delegate
			{
				if (this.writer != null)
				{
					try
					{
						this.writer.Dispose();
						this.writer = null;
					}
					catch (ExchangeDataException ex)
					{
						ExTraceGlobals.CcBodyTracer.TraceError<ExchangeDataException>((long)this.GetHashCode(), "BodyTextWriter.CloseWriter() exception {0}", ex);
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
			GC.SuppressFinalize(this);
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

		private readonly DisposeTracker disposeTracker;

		private Stream bodyStream;

		private TextWriter writer;

		private ConversionCallbackBase conversionCallback;

		private ICoreItem coreItem;

		private bool isDisposed;
	}
}
