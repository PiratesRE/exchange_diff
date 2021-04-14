using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class BaseStepServiceCommand<RequestType, SingleItemType> : ServiceCommandBase where RequestType : BaseRequest
	{
		public BaseStepServiceCommand(CallContext callContext, RequestType request) : base(callContext)
		{
			this.Request = request;
		}

		internal override ResourceKey[] GetResources()
		{
			RequestType request = this.Request;
			return request.GetResources(base.CallContext, base.CurrentStep);
		}

		internal RequestType Request { get; private set; }

		internal void CheckAndThrowFaultExceptionOnRequestLevelErrors<TResult>(params ServiceResult<TResult>[] results)
		{
			if (base.CallContext.IsOwa)
			{
				return;
			}
			ServiceErrors.CheckAndThrowFaultExceptionOnRequestLevelErrors<TResult>(results);
		}

		internal override void InternalExecuteStep(out bool isBatchStopResponse)
		{
			try
			{
				ServiceResult<SingleItemType> serviceResult = ExceptionHandler<SingleItemType>.Execute((int step) => this.Execute(), base.CurrentStep);
				this.SetCurrentStepResult(serviceResult);
				isBatchStopResponse = serviceResult.IsStopBatchProcessingError;
			}
			finally
			{
				base.LogRequestTraces();
			}
		}

		internal override void InternalCancelStep(LocalizedException exception, out bool isBatchStopResponse)
		{
			if (exception != null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(base.CallContext.ProtocolLog, exception, "BaseStepSvcCmd_InternalCancel");
			}
			ServiceResult<SingleItemType> serviceResult = ExceptionHandler<SingleItemType>.GetServiceResult<SingleItemType>(exception, null);
			this.SetCurrentStepResult(serviceResult);
			isBatchStopResponse = serviceResult.IsStopBatchProcessingError;
		}

		internal abstract void SetCurrentStepResult(ServiceResult<SingleItemType> result);

		internal abstract ServiceResult<SingleItemType> Execute();
	}
}
