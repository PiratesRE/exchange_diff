using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FolderChangeAdaptor : BaseObject, IFolderChange, IDisposable
	{
		internal FolderChangeAdaptor(IPropertyBag folderPropertyBag)
		{
			this.folderPropertyBag = folderPropertyBag;
		}

		public IPropertyBag FolderPropertyBag
		{
			get
			{
				base.CheckDisposed();
				return this.folderPropertyBag;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FolderChangeAdaptor>(this);
		}

		private readonly IPropertyBag folderPropertyBag;
	}
}
