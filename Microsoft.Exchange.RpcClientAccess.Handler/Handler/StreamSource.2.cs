using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal class StreamSource<T> : StreamSource where T : IDisposable
	{
		public StreamSource(ReferenceCount<T> coreReference, Func<T, ICorePropertyBag> getPropertyBagFunc)
		{
			this.coreReference = coreReference;
			this.coreReference.AddRef();
			this.getPropertyBagFunc = getPropertyBagFunc;
		}

		public override ICorePropertyBag PropertyBag
		{
			get
			{
				return this.getPropertyBagFunc(this.coreReference.ReferencedObject);
			}
		}

		public override void OnAccess()
		{
			this.PropertyBag.Load(null);
		}

		public override StreamSource Duplicate()
		{
			return new StreamSource<T>(this.coreReference, this.getPropertyBagFunc);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<StreamSource<T>>(this);
		}

		protected override void InternalDispose()
		{
			this.coreReference.Release();
			base.InternalDispose();
		}

		private readonly ReferenceCount<T> coreReference;

		private readonly Func<T, ICorePropertyBag> getPropertyBagFunc;
	}
}
