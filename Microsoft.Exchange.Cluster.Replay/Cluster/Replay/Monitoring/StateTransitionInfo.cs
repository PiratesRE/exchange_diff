using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay.Monitoring
{
	internal class StateTransitionInfo
	{
		public TransientErrorInfo StateInfo { get; private set; }

		public StateTransitionInfo() : this(true)
		{
		}

		public static StateTransitionInfo ConstructFromPersisted(TransientErrorInfoPersisted errorInfo)
		{
			StateTransitionInfo stateTransitionInfo;
			if (errorInfo == null)
			{
				stateTransitionInfo = new StateTransitionInfo();
			}
			else
			{
				stateTransitionInfo = new StateTransitionInfo(false);
				stateTransitionInfo.StateInfo = TransientErrorInfo.ConstructFromPersisted(errorInfo);
			}
			return stateTransitionInfo;
		}

		private StateTransitionInfo(bool fCreateTransientErrorInfo)
		{
			if (fCreateTransientErrorInfo)
			{
				this.StateInfo = new TransientErrorInfo();
			}
		}

		public TransientErrorInfoPersisted ConvertToSerializable()
		{
			return new TransientErrorInfoPersisted
			{
				CurrentErrorState = StateTransitionInfo.ConvertErrorTypeToSerializable(this.StateInfo.CurrentErrorState),
				LastSuccessTransitionUtc = DateTimeHelper.ToPersistedString(this.LastTransitionIntoStateUtc),
				LastFailureTransitionUtc = DateTimeHelper.ToPersistedString(this.LastTransitionOutOfStateUtc)
			};
		}

		public bool IsSuccess
		{
			get
			{
				return this.StateInfo.CurrentErrorState == TransientErrorInfo.ErrorType.Success;
			}
		}

		public TimeSpan SuccessDuration
		{
			get
			{
				return this.StateInfo.SuccessDuration;
			}
		}

		public TimeSpan FailedDuration
		{
			get
			{
				return this.StateInfo.FailedDuration;
			}
		}

		public void ReportSuccess()
		{
			this.StateInfo.ReportSuccess();
		}

		public void ReportFailure()
		{
			this.StateInfo.ReportFailure();
		}

		public DateTime LastTransitionIntoStateUtc
		{
			get
			{
				return this.StateInfo.LastSuccessTransitionUtc;
			}
		}

		public DateTime LastTransitionOutOfStateUtc
		{
			get
			{
				return this.StateInfo.LastFailureTransitionUtc;
			}
		}

		internal static ErrorTypePersisted ConvertErrorTypeToSerializable(TransientErrorInfo.ErrorType errorType)
		{
			switch (errorType)
			{
			case TransientErrorInfo.ErrorType.Unknown:
				return ErrorTypePersisted.Unknown;
			case TransientErrorInfo.ErrorType.Success:
				return ErrorTypePersisted.Success;
			case TransientErrorInfo.ErrorType.Failure:
				return ErrorTypePersisted.Failure;
			default:
				DiagCore.RetailAssert(false, "Missing case for enumeration: {0}", new object[]
				{
					errorType
				});
				return ErrorTypePersisted.Unknown;
			}
		}

		internal static TransientErrorInfo.ErrorType ConvertErrorTypeFromSerializable(ErrorTypePersisted errorType)
		{
			switch (errorType)
			{
			case ErrorTypePersisted.Unknown:
				return TransientErrorInfo.ErrorType.Unknown;
			case ErrorTypePersisted.Success:
				return TransientErrorInfo.ErrorType.Success;
			case ErrorTypePersisted.Failure:
				return TransientErrorInfo.ErrorType.Failure;
			default:
				DiagCore.RetailAssert(false, "Missing case for enumeration: {0}", new object[]
				{
					errorType
				});
				return TransientErrorInfo.ErrorType.Unknown;
			}
		}
	}
}
