using System;
using System.Linq;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class MultiStepServiceCommand<RequestType, SingleItemType> : BaseStepServiceCommand<RequestType, SingleItemType> where RequestType : BaseRequest
	{
		public MultiStepServiceCommand(CallContext callContext, RequestType request) : base(callContext, request)
		{
		}

		private bool HaveLoggedErrorResponse { get; set; }

		internal ServiceResult<SingleItemType>[] Results { get; private set; }

		internal override bool InternalPreExecute()
		{
			bool success = false;
			ServiceResult<SingleItemType>[] results = ExceptionHandler<SingleItemType>.Execute(delegate()
			{
				FaultInjection.GenerateFault((FaultInjection.LIDs)3716558141U);
				this.PreExecuteCommand();
				if (this.StepCount > 0)
				{
					this.Results = new ServiceResult<SingleItemType>[this.StepCount];
				}
				success = true;
				return null;
			});
			if (!success)
			{
				this.Results = results;
				base.CheckAndThrowFaultExceptionOnRequestLevelErrors<SingleItemType>(results);
			}
			return success;
		}

		internal virtual void PreExecuteCommand()
		{
		}

		internal override void InternalPostExecute()
		{
			bool success = false;
			ServiceResult<SingleItemType>[] results = ExceptionHandler<SingleItemType>.Execute(delegate()
			{
				this.PostExecuteCommand();
				success = true;
				return null;
			});
			if (!success)
			{
				this.Results = results;
			}
			base.CheckAndThrowFaultExceptionOnRequestLevelErrors<SingleItemType>(this.Results);
		}

		internal virtual void PostExecuteCommand()
		{
		}

		internal override void SetCurrentStepResult(ServiceResult<SingleItemType> result)
		{
			this.Results[base.CurrentStep] = result;
		}

		internal void LogServiceResultErrorAsAppropriate(ServiceResultCode resultCode, ServiceError serviceError)
		{
			if (!this.HaveLoggedErrorResponse && resultCode != ServiceResultCode.Success && serviceError != null)
			{
				base.CallContext.ProtocolLog.Set(ServiceCommonMetadata.ErrorCode, serviceError.MessageKey);
				this.HaveLoggedErrorResponse = true;
			}
		}

		internal bool LogItemId()
		{
			return base.CurrentStep < 5 && base.CallContext != null && !string.IsNullOrEmpty(base.CallContext.UserAgent) && Global.WellKnownClientsForBackgroundSync.Any((string x) => base.CallContext.UserAgent.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0);
		}

		internal void LogServiceResultErrorAsAppropriate(ServiceResult<XmlNode> result)
		{
			this.LogServiceResultErrorAsAppropriate(result.Code, result.Error);
		}

		private const int ItemIdLoggingBatchLimit = 5;
	}
}
