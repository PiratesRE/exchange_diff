using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public sealed class MapiViewAttachment : MapiViewTableBase
	{
		public MapiViewAttachment() : base(MapiObjectType.AttachmentView)
		{
		}

		public void Configure(MapiContext context, MapiLogon mapiLogon, MapiMessage message)
		{
			if (base.IsDisposed)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "Configure called on a Dispose'd MapiViewAttachment!  Throwing ExExceptionInvalidObject!");
				throw new ExExceptionInvalidObject((LID)52216U, "Configure cannot be invoked after Dispose.");
			}
			if (base.IsValid)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "Configure called on already configured MapiViewAttachment!  Throwing ExExceptionInvalidObject!");
				throw new ExExceptionInvalidObject((LID)46072U, "Object has already been Configure'd");
			}
			if (message.IsDisposed)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "Configure called on a Dispose'd MapiMessage!  Throwing ExExceptionInvalidObject!");
				throw new ExExceptionInvalidObject((LID)62456U, "Configure cannot be invoked after Dispose.");
			}
			AttachmentViewTable storeViewTable = new AttachmentViewTable(context, mapiLogon.StoreMailbox, message.StoreMessage);
			base.Configure(context, mapiLogon, Array<StorePropTag>.Empty, storeViewTable, uint.MaxValue, null);
		}

		protected override bool CanSortOnProperty(StorePropTag propTag)
		{
			return true;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiViewAttachment>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			base.InternalDispose(calledFromDispose);
		}
	}
}
