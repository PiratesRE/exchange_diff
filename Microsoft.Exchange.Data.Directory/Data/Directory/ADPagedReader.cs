using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ADPagedReader<TResult> : ADGenericPagedReader<TResult> where TResult : IConfigurable, new()
	{
		internal ADPagedReader()
		{
		}

		internal ADPagedReader(IDirectorySession session, ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties, bool skipCheckVirtualIndex) : base(session, rootId, scope, filter, sortBy, pageSize, properties, skipCheckVirtualIndex)
		{
			this.pageResultRequestControl = new PageResultRequestControl(base.PageSize);
			base.DirectoryControls.Add(this.pageResultRequestControl);
		}

		protected override SearchResultEntryCollection GetNextResultCollection()
		{
			this.pageResultRequestControl.Cookie = base.Cookie;
			this.pageResultRequestControl.PageSize = base.PageSize;
			if (base.PagesReturned > 0)
			{
				ADProviderPerf.UpdateProcessCounter(Counter.ProcessRatePaged, UpdateType.Add, 1U);
				ADProviderPerf.UpdateDCCounter(base.PreferredServerName, Counter.DCRatePaged, UpdateType.Add, 1U);
			}
			DirectoryControl directoryControl;
			SearchResultEntryCollection nextResultCollection = base.GetNextResultCollection(typeof(PageResultResponseControl), out directoryControl);
			base.Cookie = ((directoryControl == null) ? null : ((PageResultResponseControl)directoryControl).Cookie);
			if (base.Cookie == null || base.Cookie.Length == 0 || nextResultCollection == null)
			{
				base.RetrievedAllData = new bool?(true);
			}
			else
			{
				base.RetrievedAllData = new bool?(false);
			}
			return nextResultCollection;
		}

		private PageResultRequestControl pageResultRequestControl;
	}
}
