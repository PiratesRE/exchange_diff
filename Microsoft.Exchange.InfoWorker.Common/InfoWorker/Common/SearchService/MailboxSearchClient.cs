using System;
using Microsoft.Exchange.Rpc.MailboxSearch;

namespace Microsoft.Exchange.InfoWorker.Common.SearchService
{
	internal class MailboxSearchClient : MailboxSearchRpcClient
	{
		internal MailboxSearchClient(string machineName) : base(machineName)
		{
		}

		internal int LastErrorCode
		{
			get
			{
				return this.lastErrorCode;
			}
		}

		internal void Start(SearchId searchId, Guid ownerGuid)
		{
			SearchErrorInfo searchErrorInfo = null;
			base.Start(searchId, ownerGuid, out searchErrorInfo);
			this.lastErrorCode = searchErrorInfo.ErrorCode;
			if (searchErrorInfo.Failed)
			{
				throw new SearchServerException(searchErrorInfo.ErrorCode, searchErrorInfo.Message);
			}
		}

		internal void StartEx(SearchId searchId, string ownerId)
		{
			SearchErrorInfo searchErrorInfo = null;
			base.StartEx(searchId, ownerId, out searchErrorInfo);
			this.lastErrorCode = searchErrorInfo.ErrorCode;
			if (searchErrorInfo.Failed)
			{
				throw new SearchServerException(searchErrorInfo.ErrorCode, searchErrorInfo.Message);
			}
		}

		internal SearchStatus GetStatus(SearchId searchId)
		{
			SearchErrorInfo searchErrorInfo = null;
			SearchStatus status = base.GetStatus(searchId, out searchErrorInfo);
			this.lastErrorCode = searchErrorInfo.ErrorCode;
			if (searchErrorInfo.Failed)
			{
				throw new SearchServerException(searchErrorInfo.ErrorCode, searchErrorInfo.Message);
			}
			if (searchErrorInfo.ErrorCode == 262658)
			{
				return null;
			}
			return status;
		}

		internal void Abort(SearchId searchId, Guid userGuid)
		{
			SearchErrorInfo searchErrorInfo = null;
			base.Abort(searchId, userGuid, out searchErrorInfo);
			this.lastErrorCode = searchErrorInfo.ErrorCode;
			if (searchErrorInfo.Failed)
			{
				throw new SearchServerException(searchErrorInfo.ErrorCode, searchErrorInfo.Message);
			}
		}

		internal void AbortEx(SearchId searchId, string userId)
		{
			SearchErrorInfo searchErrorInfo = null;
			base.AbortEx(searchId, userId, out searchErrorInfo);
			this.lastErrorCode = searchErrorInfo.ErrorCode;
			if (searchErrorInfo.Failed)
			{
				throw new SearchServerException(searchErrorInfo.ErrorCode, searchErrorInfo.Message);
			}
		}

		internal void Remove(SearchId searchId, bool removeLogs)
		{
			SearchErrorInfo searchErrorInfo = null;
			base.Remove(searchId, removeLogs, out searchErrorInfo);
			this.lastErrorCode = searchErrorInfo.ErrorCode;
			if (searchErrorInfo.Failed)
			{
				throw new SearchServerException(searchErrorInfo.ErrorCode, searchErrorInfo.Message);
			}
		}

		internal void UpdateStatus(SearchId searchId)
		{
			SearchErrorInfo searchErrorInfo = null;
			base.UpdateStatus(searchId, out searchErrorInfo);
			this.lastErrorCode = searchErrorInfo.ErrorCode;
			if (searchErrorInfo.Failed)
			{
				throw new SearchServerException(searchErrorInfo.ErrorCode, searchErrorInfo.Message);
			}
		}

		private int lastErrorCode;
	}
}
