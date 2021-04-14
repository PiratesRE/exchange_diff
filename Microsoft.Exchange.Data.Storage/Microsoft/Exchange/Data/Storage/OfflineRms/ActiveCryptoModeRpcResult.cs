using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.RightsManagementServices.Core;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class ActiveCryptoModeRpcResult
	{
		internal ActiveCryptoModeRpcResult(ActiveCryptoModeResult originalResult)
		{
			if (originalResult == null)
			{
				throw new ArgumentNullException("originalResult");
			}
			this.ActiveCryptoMode = originalResult.ActiveCryptoMode;
			if (originalResult.Error != null)
			{
				this.isPermanentFailure = originalResult.Error.IsPermanentFailure;
				this.wellKnownErrorCode = originalResult.Error.WellKnownErrorCode;
				this.errorResults = ErrorResult.GetErrorResultListFromException(originalResult.Error);
			}
		}

		public int ActiveCryptoMode { get; private set; }

		public bool IsPermanentFailure
		{
			get
			{
				return this.isPermanentFailure;
			}
		}

		public WellKnownErrorCode WellKnownErrorCode
		{
			get
			{
				return this.wellKnownErrorCode;
			}
		}

		public List<ErrorResult> ErrorResults
		{
			get
			{
				return this.errorResults;
			}
		}

		public string GetSerializedString()
		{
			return ErrorResult.GetSerializedString(this.errorResults);
		}

		private readonly List<ErrorResult> errorResults = new List<ErrorResult>();

		private readonly bool isPermanentFailure;

		private readonly WellKnownErrorCode wellKnownErrorCode;
	}
}
