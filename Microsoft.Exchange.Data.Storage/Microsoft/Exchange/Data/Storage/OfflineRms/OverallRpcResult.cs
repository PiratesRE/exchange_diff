using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.RightsManagementServices.Core;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class OverallRpcResult
	{
		internal OverallRpcResult(Exception e)
		{
			if (e == null)
			{
				this.status = OverallRpcStatus.Success;
				return;
			}
			RightsManagementServerException ex = e as RightsManagementServerException;
			this.status = ((ex != null && !ex.IsPermanentFailure) ? OverallRpcStatus.TransientFailure : OverallRpcStatus.PermanentFailure);
			this.errorResults = ErrorResult.GetErrorResultListFromException(e);
			if (ex != null)
			{
				this.wellKnownErrorCode = ex.WellKnownErrorCode;
			}
		}

		public List<ErrorResult> ErrorResults
		{
			get
			{
				return this.errorResults;
			}
		}

		public OverallRpcStatus Status
		{
			get
			{
				return this.status;
			}
		}

		public WellKnownErrorCode WellKnownErrorCode
		{
			get
			{
				return this.wellKnownErrorCode;
			}
		}

		private readonly List<ErrorResult> errorResults = new List<ErrorResult>();

		private readonly OverallRpcStatus status;

		private readonly WellKnownErrorCode wellKnownErrorCode;
	}
}
