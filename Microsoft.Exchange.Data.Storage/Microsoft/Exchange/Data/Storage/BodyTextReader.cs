using System;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BodyTextReader : TextReader, Body.IBodyStream, IDisposeTrackable, IDisposable
	{
		internal BodyTextReader(ICoreItem coreItem, BodyReadConfiguration configuration, Stream inputStream)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				Stream stream = null;
				TextReader textReader = null;
				bool flag = false;
				this.disposeTracker = this.GetDisposeTracker();
				try
				{
					if (inputStream == null)
					{
						stream = BodyReadStream.OpenBodyStream(coreItem);
						inputStream = stream;
					}
					if (inputStream == null)
					{
						inputStream = Body.GetEmptyStream();
					}
					textReader = BodyReadDelegates.CreateReader(coreItem, configuration, inputStream, out this.conversionCallbacks);
					flag = true;
				}
				finally
				{
					if (!flag && stream != null)
					{
						stream.Dispose();
					}
				}
				this.reader = textReader;
				this.bodyStream = stream;
				this.isDisposed = false;
				disposeGuard.Success();
			}
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

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<BodyTextReader>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public override int Peek()
		{
			this.CheckDisposed();
			return ConvertUtils.CallCtsWithReturnValue<int>(ExTraceGlobals.CcBodyTracer, "BodyTextReader::Peek", ServerStrings.ConversionBodyConversionFailed, () => this.reader.Peek());
		}

		public override int Read()
		{
			this.CheckDisposed();
			return ConvertUtils.CallCtsWithReturnValue<int>(ExTraceGlobals.CcBodyTracer, "BodyTextReader::Read", ServerStrings.ConversionBodyConversionFailed, () => this.reader.Read());
		}

		public override int Read(char[] buffer, int index, int count)
		{
			this.CheckDisposed();
			return ConvertUtils.CallCtsWithReturnValue<int>(ExTraceGlobals.CcBodyTracer, "BodyTextReader::Read", ServerStrings.ConversionBodyConversionFailed, () => this.reader.Read(buffer, index, count));
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this.CloseReader();
				}
				this.isDisposed = true;
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private void CloseReader()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			if (this.reader != null)
			{
				try
				{
					this.reader.Dispose();
					this.reader = null;
				}
				catch (ExchangeDataException arg)
				{
					ExTraceGlobals.CcBodyTracer.TraceError<ExchangeDataException>((long)this.GetHashCode(), "BodyTextReader.CloseReader() exception {0}", arg);
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

		private readonly DisposeTracker disposeTracker;

		private Stream bodyStream;

		private TextReader reader;

		private ConversionCallbackBase conversionCallbacks;

		private bool isDisposed;
	}
}
