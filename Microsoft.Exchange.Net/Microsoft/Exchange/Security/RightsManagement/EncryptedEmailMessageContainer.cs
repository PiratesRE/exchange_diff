using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.RightsManagement.StructuredStorage;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal abstract class EncryptedEmailMessageContainer<T> : IDisposeTrackable, IDisposable where T : EncryptedEmailMessage, new()
	{
		public EncryptedEmailMessageContainer()
		{
			this.disposeTracker = this.GetDisposeTracker();
		}

		public EncryptedEmailMessageContainer(T emailMessage)
		{
			if (emailMessage == null)
			{
				throw new ArgumentNullException("emailMessage");
			}
			this.emailMessage = emailMessage;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public T EmailMessage
		{
			get
			{
				return this.emailMessage;
			}
		}

		protected virtual string EncryptedEmailMessageStreamName
		{
			get
			{
				return "EncryptedContents";
			}
		}

		internal static T ReadEncryptedEmailMessage(IStorage rootStorage, string contentStreamName, EncryptedEmailMessageBinding messageBinding, CreateStreamCallbackDelegate createBodyStreamCallback, CreateStreamCallbackDelegate createAttachmentStreamCallback)
		{
			T result = Activator.CreateInstance<T>();
			IStream stream = null;
			IStorage storage = null;
			try
			{
				stream = DrmEmailUtils.EnsureStream(rootStorage, contentStreamName);
				try
				{
					storage = messageBinding.ConvertToEncryptedStorage(stream, false);
				}
				catch (COMException innerException)
				{
					throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("InvalidDRMContentEncryptedStorage"), innerException);
				}
				result.Load(storage, createBodyStreamCallback, createAttachmentStreamCallback);
			}
			finally
			{
				if (storage != null)
				{
					Marshal.ReleaseComObject(storage);
					storage = null;
				}
				if (stream != null)
				{
					Marshal.ReleaseComObject(stream);
					stream = null;
				}
			}
			return result;
		}

		internal static void WriteEncryptedEmailMessage(IStorage rootStorage, string contentStreamName, EncryptedEmailMessageBinding messageBinding, EncryptedEmailMessage emailMessage)
		{
			IStream stream = null;
			IStorage storage = null;
			try
			{
				stream = rootStorage.CreateStream(contentStreamName, 4114, 0, 0);
				storage = messageBinding.ConvertToEncryptedStorage(stream, true);
				emailMessage.Save(storage, messageBinding);
				storage.Commit(STGC.STGC_DEFAULT);
				stream.Commit(STGC.STGC_DEFAULT);
			}
			finally
			{
				if (storage != null)
				{
					Marshal.ReleaseComObject(storage);
					storage = null;
				}
				if (stream != null)
				{
					Marshal.ReleaseComObject(stream);
					stream = null;
				}
			}
		}

		public void Load(Stream stream, CreateStreamCallbackDelegate createStreamCallback)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (createStreamCallback == null)
			{
				throw new ArgumentNullException("createStreamCallback");
			}
			if (this.emailMessage != null || this.rootStorage != null)
			{
				throw new InvalidOperationException("The object is already loaded.");
			}
			bool flag = false;
			Stream stream2 = null;
			IStorage storage = null;
			try
			{
				EncryptedEmailMessageContainer<T>.Tracer.TraceDebug((long)this.GetHashCode(), "Loading the encrypted message stream");
				stream2 = createStreamCallback(null);
				EncryptedEmailMessageContainer<T>.Tracer.TraceDebug((long)this.GetHashCode(), "Decompressing the message stream");
				DrmEmailCompression.DecompressStream(stream, stream2, true);
				try
				{
					storage = DrmEmailUtils.OpenStorageOverStream(stream2);
				}
				catch (COMException innerException)
				{
					throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("DecompressNotStorage"), innerException);
				}
				IStream o = DrmEmailUtils.EnsureStream(storage, this.EncryptedEmailMessageStreamName);
				Marshal.ReleaseComObject(o);
				EncryptedEmailMessageContainer<T>.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Loaded the encrypted message content stream with name:{0}", this.EncryptedEmailMessageStreamName);
				this.ReadBinding(storage);
				flag = true;
				EncryptedEmailMessageContainer<T>.Tracer.TraceDebug((long)this.GetHashCode(), "Loaded the binding information from the message stream.");
				this.rootStorage = storage;
				this.temporaryStream = stream2;
			}
			finally
			{
				if (!flag)
				{
					if (storage != null)
					{
						Marshal.ReleaseComObject(storage);
					}
					if (stream2 != null)
					{
						stream2.Close();
					}
				}
			}
		}

		public void Save(Stream stream, EncryptedEmailMessageBinding messageBinding)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (messageBinding == null)
			{
				throw new ArgumentNullException("messageBinding");
			}
			if (this.emailMessage == null)
			{
				throw new InvalidOperationException("Object must be loaded to perform this operation.");
			}
			IStorage storage = null;
			try
			{
				EncryptedEmailMessageContainer<T>.Tracer.TraceDebug((long)this.GetHashCode(), "Saving the encrypted message stream");
				storage = DrmEmailUtils.CreateStorageOverStream(stream);
				EncryptedEmailMessageContainer<T>.Tracer.TraceDebug((long)this.GetHashCode(), "Writing the email message");
				EncryptedEmailMessageContainer<T>.WriteEncryptedEmailMessage(storage, this.EncryptedEmailMessageStreamName, messageBinding, this.emailMessage);
				EncryptedEmailMessageContainer<T>.Tracer.TraceDebug((long)this.GetHashCode(), "Writing the email message binding");
				this.WriteBinding(storage);
				EncryptedEmailMessageContainer<T>.Tracer.TraceDebug((long)this.GetHashCode(), "Writing the email message container succeeded");
				storage.Commit(STGC.STGC_DEFAULT);
			}
			finally
			{
				if (storage != null)
				{
					Marshal.ReleaseComObject(storage);
					storage = null;
				}
			}
			DrmEmailCompression.CompressStream(stream, true);
			EncryptedEmailMessageContainer<T>.Tracer.TraceDebug((long)this.GetHashCode(), "Save completed and compressed stream");
		}

		public void Bind(EncryptedEmailMessageBinding messageBinding, CreateStreamCallbackDelegate createBodyStreamCallback, CreateStreamCallbackDelegate createAttachmentStreamCallback)
		{
			if (messageBinding == null)
			{
				throw new ArgumentNullException("messageBinding");
			}
			if (createBodyStreamCallback == null)
			{
				throw new ArgumentNullException("createBodyStreamCallback");
			}
			if (createAttachmentStreamCallback == null)
			{
				throw new ArgumentNullException("createAttachmentStreamCallback");
			}
			if (this.emailMessage != null)
			{
				throw new InvalidOperationException("The object is already bound.");
			}
			EncryptedEmailMessageContainer<T>.Tracer.TraceDebug((long)this.GetHashCode(), "Binding encrypted message storage");
			this.emailMessage = EncryptedEmailMessageContainer<T>.ReadEncryptedEmailMessage(this.rootStorage, this.EncryptedEmailMessageStreamName, messageBinding, createBodyStreamCallback, createAttachmentStreamCallback);
			EncryptedEmailMessageContainer<T>.Tracer.TraceDebug((long)this.GetHashCode(), "Binding encrypted message storage succeeded");
			Marshal.ReleaseComObject(this.rootStorage);
			this.rootStorage = null;
			this.temporaryStream.Close();
			this.temporaryStream = null;
			this.OnBound();
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<EncryptedEmailMessageContainer<T>>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected abstract void ReadBinding(IStorage bindingRootStorage);

		protected abstract void WriteBinding(IStorage bindingRootStorage);

		protected virtual void OnBound()
		{
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				EncryptedEmailMessageContainer<T>.Tracer.TraceDebug((long)this.GetHashCode(), "Disposing the encrypted email message container");
				if (this.rootStorage != null)
				{
					Marshal.ReleaseComObject(this.rootStorage);
					this.rootStorage = null;
				}
				if (this.temporaryStream != null)
				{
					this.temporaryStream.Close();
					this.temporaryStream = null;
				}
			}
			this.disposed = true;
		}

		private static readonly Trace Tracer = ExTraceGlobals.RightsManagementTracer;

		private DisposeTracker disposeTracker;

		private bool disposed;

		private T emailMessage;

		private IStorage rootStorage;

		private Stream temporaryStream;
	}
}
