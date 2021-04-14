using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class FastQueryView : DisposeTrackableBase
	{
		public FastQueryView(ResponseFactory factory, Folder folder, SortBy[] sortBys, PropertyDefinition[] propDefs)
		{
			this.factory = factory;
			this.view = folder.ItemQuery(ItemQueryType.RetrieveFromIndex, null, sortBys, propDefs);
		}

		public QueryResult TableView
		{
			get
			{
				return this.view;
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (this.view != null)
			{
				try
				{
					this.view.Dispose();
				}
				catch (LocalizedException ex)
				{
					ProtocolBaseServices.SessionTracer.TraceDebug<string>(this.factory.Session.SessionId, "Exception caught while disposing fastQueryView. {0}", ex.ToString());
				}
				this.view = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<FastQueryView>(this);
		}

		private QueryResult view;

		private ResponseFactory factory;
	}
}
