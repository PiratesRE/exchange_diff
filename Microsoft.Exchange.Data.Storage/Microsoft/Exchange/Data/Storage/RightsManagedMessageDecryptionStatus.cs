using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct RightsManagedMessageDecryptionStatus
	{
		public RightsManagedMessageDecryptionStatus(RightsManagementFailureCode failureCode, Exception exception)
		{
			EnumValidator.ThrowIfInvalid<RightsManagementFailureCode>(failureCode, "failureCode");
			this.failureCode = failureCode;
			this.exception = exception;
		}

		public RightsManagementFailureCode FailureCode
		{
			get
			{
				return this.failureCode;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public bool Failed
		{
			get
			{
				return this.failureCode != RightsManagementFailureCode.Success;
			}
		}

		public static RightsManagedMessageDecryptionStatus CreateFromException(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			RightsManagementPermanentException ex = exception as RightsManagementPermanentException;
			if (ex != null)
			{
				return new RightsManagedMessageDecryptionStatus(ex.FailureCode, ex);
			}
			return new RightsManagedMessageDecryptionStatus(RightsManagementFailureCode.UnknownFailure, exception);
		}

		public static readonly RightsManagedMessageDecryptionStatus Success = new RightsManagedMessageDecryptionStatus(RightsManagementFailureCode.Success, null);

		public static readonly RightsManagedMessageDecryptionStatus FeatureDisabled = new RightsManagedMessageDecryptionStatus(RightsManagementFailureCode.FeatureDisabled, null);

		public static readonly RightsManagedMessageDecryptionStatus NotSupported = new RightsManagedMessageDecryptionStatus(RightsManagementFailureCode.NotSupported, null);

		private RightsManagementFailureCode failureCode;

		private Exception exception;
	}
}
