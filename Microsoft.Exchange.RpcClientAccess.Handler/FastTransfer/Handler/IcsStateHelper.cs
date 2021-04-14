using System;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal sealed class IcsStateHelper : BaseObject
	{
		public IcsStateHelper(ReferenceCount<CoreFolder> propertyMappingReference)
		{
			this.propertyMappingReference = propertyMappingReference;
			this.propertyMappingReference.AddRef();
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.propertyBag = IcsStateHelper.CreateInMemoryPropertyBag(propertyMappingReference.ReferencedObject);
				this.icsState = new IcsState();
				disposeGuard.Success();
			}
		}

		public FastTransferIcsState CreateIcsStateFastTransferObject()
		{
			base.CheckDisposed();
			this.EnsureClientInitialStateLoaded();
			FastTransferIcsState result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IcsStateAdaptor disposable = new IcsStateAdaptor(this.icsState, this.propertyMappingReference);
				disposeGuard.Add<IcsStateAdaptor>(disposable);
				FastTransferIcsState fastTransferIcsState = new FastTransferIcsState(disposable);
				disposeGuard.Success();
				result = fastTransferIcsState;
			}
			return result;
		}

		public void BeginUploadProperty(PropertyTag property, uint dataSize)
		{
			base.CheckDisposed();
			this.icsState.CheckCanLoad(IcsStateOrigin.ClientInitial);
			PropertyTag propertyTag = FastTransferIcsState.AllIcsStateProperties.FirstOrDefault((PropertyTag icsStateProp) => icsStateProp.PropertyId == property.PropertyId);
			if (propertyTag == default(PropertyTag))
			{
				throw new RopExecutionException(string.Format("Expected an IcsState property. Found {0}.", property), (ErrorCode)2147746050U);
			}
			if (this.currentPropertyStream != null)
			{
				throw new RopExecutionException("Can't begin upload of a property before finishing upload of another one", (ErrorCode)2147500037U);
			}
			if (!this.propertyBag.GetAnnotatedProperty(propertyTag).PropertyValue.IsNotFound)
			{
				throw new RopExecutionException("Can't begin upload of a property for the second time", (ErrorCode)2147500037U);
			}
			this.currentPropertyStream = this.propertyBag.SetPropertyStream(propertyTag, (long)((ulong)dataSize));
			this.currentPropertyStream.SetLength((long)((ulong)dataSize));
		}

		public void UploadPropertyData(ArraySegment<byte> data)
		{
			base.CheckDisposed();
			this.icsState.CheckCanLoad(IcsStateOrigin.ClientInitial);
			if (this.currentPropertyStream == null || (long)data.Count > this.currentPropertyStream.Length - this.currentPropertyStream.Position)
			{
				throw new RopExecutionException("Can't upload data for a property in excess of what was declared, or without initiating upload first with BeginUploadProperty", (ErrorCode)2147500037U);
			}
			this.currentPropertyStream.Write(data.Array, data.Offset, data.Count);
		}

		public void EndUploadProperty()
		{
			base.CheckDisposed();
			this.icsState.CheckCanLoad(IcsStateOrigin.ClientInitial);
			try
			{
				if (this.currentPropertyStream == null || this.currentPropertyStream.Length != this.currentPropertyStream.Position)
				{
					throw new RopExecutionException("Property upload was not initiated with BeginUploadProperty or data for a property ended prematurely", (ErrorCode)2147500037U);
				}
				this.needToLoadIcsStateFromPropertyBag = true;
			}
			finally
			{
				Util.DisposeIfPresent(this.currentPropertyStream);
				this.currentPropertyStream = null;
			}
		}

		public IcsState IcsState
		{
			get
			{
				base.CheckDisposed();
				this.EnsureClientInitialStateLoaded();
				return this.icsState;
			}
		}

		internal static IPropertyBag CreateInMemoryPropertyBag(ICoreObject propertyMappingReference)
		{
			return new MemoryPropertyBag(new SessionAdaptor(propertyMappingReference.Session));
		}

		private void EnsureClientInitialStateLoaded()
		{
			if (this.currentPropertyStream != null)
			{
				throw new RopExecutionException("State property uploads in progress", (ErrorCode)2147500037U);
			}
			if (this.needToLoadIcsStateFromPropertyBag)
			{
				this.icsState.Load(IcsStateOrigin.ClientInitial, this.propertyBag);
				this.needToLoadIcsStateFromPropertyBag = false;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<IcsStateHelper>(this);
		}

		protected override void InternalDispose()
		{
			this.propertyMappingReference.Release();
			Util.DisposeIfPresent(this.currentPropertyStream);
			base.InternalDispose();
		}

		private readonly IcsState icsState;

		private readonly IPropertyBag propertyBag;

		private readonly ReferenceCount<CoreFolder> propertyMappingReference;

		private Stream currentPropertyStream;

		private bool needToLoadIcsStateFromPropertyBag = true;
	}
}
