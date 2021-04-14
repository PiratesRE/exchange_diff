using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MapiAttach : MapiProp
	{
		internal MapiAttach(IExMapiAttach iAttach, IAttach externalAttach, MapiStore mapiStore) : base(iAttach, externalAttach, mapiStore, MapiAttach.IAttachGuids)
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(10))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(14514, 10, (long)this.GetHashCode(), "MapiAttach.MapiAttach: this={0}", TraceUtils.MakeHash(this));
			}
		}

		protected override void MapiInternalDispose()
		{
			if (ComponentTrace<MapiNetTags>.CheckEnabled(10))
			{
				ComponentTrace<MapiNetTags>.Trace<string>(9394, 10, (long)this.GetHashCode(), "MapiAttach.InternalDispose: this={0}", TraceUtils.MakeHash(this));
			}
			base.MapiInternalDispose();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiAttach>(this);
		}

		public MapiMessage OpenEmbeddedMessage(OpenPropertyFlags flags)
		{
			base.CheckDisposed();
			base.BlockExternalObjectCheck();
			base.LockStore();
			MapiMessage result;
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(10))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(13490, 10, (long)this.GetHashCode(), "MapiAttach.OpenEmbeddedMessage params: flags={0}", flags.ToString());
				}
				IExInterface iExInterface = null;
				MapiMessage mapiMessage;
				try
				{
					iExInterface = base.InternalOpenProperty(PropTag.AttachDataObj, InterfaceIds.IMessageGuid, 0, flags);
					mapiMessage = new MapiMessage(iExInterface.ToInterface<IExMapiMessage>(), null, base.MapiStore);
					iExInterface = null;
				}
				finally
				{
					iExInterface.DisposeIfValid();
				}
				if ((flags & OpenPropertyFlags.Create) != OpenPropertyFlags.None)
				{
					base.SetProps((ICollection<PropValue>)MapiAttach.embeddedAttachMethodProps);
				}
				if (ComponentTrace<MapiNetTags>.CheckEnabled(10))
				{
					ComponentTrace<MapiNetTags>.Trace<string>(11442, 10, (long)this.GetHashCode(), "MapiAttach.OpenEmbeddedMessage results: embeddedMessage={0}", TraceUtils.MakeHash(mapiMessage));
				}
				result = mapiMessage;
			}
			finally
			{
				base.UnlockStore();
			}
			return result;
		}

		private static readonly Guid[] IAttachGuids = new Guid[]
		{
			InterfaceIds.IAttachGuid
		};

		private static readonly PropValue[] embeddedAttachMethodProps = new PropValue[]
		{
			new PropValue(PropTag.AttachMethod, AttachMethods.EmbeddedMessage)
		};
	}
}
