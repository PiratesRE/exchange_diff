using System;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class SingleRowRefreshRequest : PartialRefreshRequest
	{
		public SingleRowRefreshRequest(object refreshCategory, object identity) : base(refreshCategory)
		{
			this.identity = identity;
		}

		public override void DoRefresh(IRefreshable refreshableDataSource, IProgress progress)
		{
			ISupportFastRefresh supportFastRefresh = refreshableDataSource as ISupportFastRefresh;
			if (supportFastRefresh == null)
			{
				throw new InvalidOperationException();
			}
			supportFastRefresh.Refresh(progress, this.identity);
		}

		public override bool Equals(object right)
		{
			if (right == null)
			{
				return false;
			}
			if (object.ReferenceEquals(this, right))
			{
				return true;
			}
			SingleRowRefreshRequest singleRowRefreshRequest = right as SingleRowRefreshRequest;
			return singleRowRefreshRequest != null && base.RefreshCategory == singleRowRefreshRequest.RefreshCategory && this.identity == singleRowRefreshRequest.identity;
		}

		public override int GetHashCode()
		{
			return this.identity.GetHashCode() ^ base.RefreshCategory.GetHashCode();
		}

		protected object ObjectIdentity
		{
			get
			{
				return this.identity;
			}
		}

		private readonly object identity;
	}
}
