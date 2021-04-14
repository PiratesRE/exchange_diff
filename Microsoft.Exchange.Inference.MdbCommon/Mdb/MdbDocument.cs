using System;
using System.Threading;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Inference.Mdb
{
	internal sealed class MdbDocument : Document, IDisposableDocument, IDocument, IPropertyBag, IReadOnlyPropertyBag, IDisposeTrackable, IDisposable
	{
		internal MdbDocument(MdbCompositeItemIdentity identity, PropertyDefinition[] propertiesToPreload, MailboxSession session, MdbPropertyMap propertyMap, DocumentOperation operation) : this(identity, propertiesToPreload, null, session, propertyMap, operation)
		{
		}

		internal MdbDocument(MdbCompositeItemIdentity identity, PropertyDefinition[] propertiesToPreload, Item item, MdbPropertyMap propertyMap, DocumentOperation operation) : this(identity, propertiesToPreload, item, null, propertyMap, operation)
		{
		}

		internal MdbDocument(MdbCompositeItemIdentity identity, PropertyDefinition[] propertiesToPreload, Item item, MailboxSession session, MdbPropertyMap propertyMap, DocumentOperation operation) : this(identity, operation, new MdbDocumentAdapter(identity, propertiesToPreload, item, session, propertyMap, true))
		{
		}

		internal MdbDocument(MdbCompositeItemIdentity identity, DocumentOperation operation, MdbDocumentAdapter documentAdapter) : base(identity, operation, documentAdapter)
		{
			Util.ThrowOnNullArgument(documentAdapter, "documentAdapter");
			this.disposeTracker = this.GetDisposeTracker();
		}

		public bool IsDisposed
		{
			get
			{
				return Interlocked.CompareExchange(ref this.isDisposedFlag, 0, 0) != 0;
			}
		}

		public bool IsDisposing
		{
			get
			{
				return Interlocked.CompareExchange(ref this.isDisposingFlag, 0, 0) != 0;
			}
		}

		internal MdbDocumentAdapter MdbDocumentAdapter
		{
			get
			{
				this.CheckDisposed();
				return base.DocumentAdapter as MdbDocumentAdapter;
			}
			private set
			{
				base.DocumentAdapter = value;
			}
		}

		public override bool TryGetProperty(PropertyDefinition property, out object value)
		{
			return base.TryGetProperty(property, out value);
		}

		public override void SetProperty(PropertyDefinition property, object value)
		{
			base.SetProperty(property, value);
		}

		public override string ToString()
		{
			this.CheckDisposed();
			object obj = null;
			this.TryGetProperty(DocumentSchema.MailboxId, out obj);
			return string.Format("OP:{0}, DID:{1}, MID:{2}", base.Operation, base.Identity, (obj == null) ? string.Empty : ((string)obj));
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MdbDocument>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.MdbDocumentAdapter != null)
			{
				this.MdbDocumentAdapter.Dispose();
				this.MdbDocumentAdapter = null;
			}
		}

		private void CheckDisposed()
		{
			if (this.IsDisposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private void Dispose(bool calledFromDispose)
		{
			if (Interlocked.Exchange(ref this.isDisposingFlag, 1) == 0)
			{
				try
				{
					if (!this.IsDisposed)
					{
						if (calledFromDispose && this.disposeTracker != null)
						{
							this.disposeTracker.Dispose();
							this.disposeTracker = null;
						}
						this.InternalDispose(calledFromDispose);
						Interlocked.Exchange(ref this.isDisposedFlag, 1);
					}
				}
				finally
				{
					Interlocked.Exchange(ref this.isDisposingFlag, 0);
				}
			}
		}

		private int isDisposedFlag;

		private int isDisposingFlag;

		private DisposeTracker disposeTracker;
	}
}
