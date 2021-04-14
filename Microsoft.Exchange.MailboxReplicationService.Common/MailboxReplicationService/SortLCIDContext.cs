using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class SortLCIDContext : DisposeTrackableBase
	{
		public SortLCIDContext(MapiStore mapiStore, int lcidValue)
		{
			this.mapiStore = mapiStore;
			this.SetNewLcid(lcidValue);
		}

		public SortLCIDContext(StoreSession storeSession, int lcidValue)
		{
			this.storeSession = storeSession;
			this.SetNewLcid(lcidValue);
		}

		private void SetNewLcid(int lcidValue)
		{
			if (lcidValue == 0)
			{
				this.originalLCIDValue = -1;
				return;
			}
			this.originalLCIDValue = this.ReadLcid();
			if (this.originalLCIDValue == lcidValue)
			{
				this.originalLCIDValue = -1;
				return;
			}
			this.SetLcid(lcidValue);
		}

		private int ReadLcid()
		{
			if (this.mapiStore != null)
			{
				PropValue prop = this.mapiStore.GetProp(PropTag.SortLocaleId);
				if (!prop.IsNull() && !prop.IsError())
				{
					return prop.GetInt();
				}
				return 0;
			}
			else
			{
				this.storeSession.Mailbox.Load(new PropertyDefinition[]
				{
					SortLCIDContext.SortLciPropertyDefinition
				});
				object obj = this.storeSession.Mailbox[SortLCIDContext.SortLciPropertyDefinition];
				if (obj == null || obj is PropertyError)
				{
					return 0;
				}
				return (int)obj;
			}
		}

		private void SetLcid(int lcidValue)
		{
			if (this.mapiStore != null)
			{
				this.mapiStore.SetProps(new PropValue[]
				{
					new PropValue(PropTag.SortLocaleId, lcidValue)
				});
				return;
			}
			this.storeSession.Mailbox[SortLCIDContext.SortLciPropertyDefinition] = lcidValue;
			this.storeSession.Mailbox.Save();
			this.storeSession.Mailbox.Load();
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.originalLCIDValue != -1)
			{
				CommonUtils.CatchKnownExceptions(delegate
				{
					this.SetLcid(this.originalLCIDValue);
				}, null);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SortLCIDContext>(this);
		}

		private static readonly PropertyDefinition SortLciPropertyDefinition = PropertyTagPropertyDefinition.CreateCustom(PropTag.SortLocaleId.ToString(), 1728380931U);

		private MapiStore mapiStore;

		private StoreSession storeSession;

		private int originalLCIDValue;
	}
}
