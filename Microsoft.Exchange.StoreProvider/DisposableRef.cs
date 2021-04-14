using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DisposableRef : DisposeTrackableBase
	{
		internal DisposableRef(IForceReportDisposeTrackable childObject)
		{
			this.childObject = childObject;
			this.nextRef = this;
			this.prevRef = this;
		}

		internal IForceReportDisposeTrackable Child
		{
			get
			{
				return this.childObject;
			}
		}

		internal DisposableRef NextRef(DisposableRef item)
		{
			if (item.nextRef == this)
			{
				return null;
			}
			return this.nextRef;
		}

		internal DisposableRef AddToList(IForceReportDisposeTrackable iObject)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(33))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(13106, 33, (long)this.GetHashCode(), "DisposableRef.AddToList: iObject={0}", TraceUtils.MakeHash(iObject));
			}
			DisposableRef disposableRef = new DisposableRef(iObject);
			disposableRef.nextRef = this.nextRef;
			disposableRef.prevRef = this;
			this.nextRef.prevRef = disposableRef;
			this.nextRef = disposableRef;
			if (ComponentTrace<MapiNetTags>.CheckEnabled(33))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(11058, 33, (long)this.GetHashCode(), "DisposableRef.AddToList results: childRef={0}", TraceUtils.MakeHash(disposableRef));
			}
			return disposableRef;
		}

		internal static void RemoveFromList(DisposableRef childRef)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(33))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(15154, 33, 0L, "DisposableRef.RemoveFromList params: childRef={0}", TraceUtils.MakeHash(childRef));
			}
			if (childRef != null)
			{
				childRef.prevRef.nextRef = childRef.nextRef;
				childRef.nextRef.prevRef = childRef.prevRef;
				childRef.prevRef = childRef;
				childRef.nextRef = childRef;
				childRef.childObject = null;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.childObject != null)
			{
				this.childObject.Dispose();
				this.childObject = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DisposableRef>(this);
		}

		private IForceReportDisposeTrackable childObject;

		private DisposableRef prevRef;

		private DisposableRef nextRef;
	}
}
