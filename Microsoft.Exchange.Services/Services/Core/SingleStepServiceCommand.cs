using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class SingleStepServiceCommand<RequestType, SingleItemType> : BaseStepServiceCommand<RequestType, SingleItemType> where RequestType : BaseRequest
	{
		public SingleStepServiceCommand(CallContext callContext, RequestType request) : base(callContext, request)
		{
		}

		internal ServiceResult<SingleItemType> Result { get; set; }

		internal override int StepCount
		{
			get
			{
				return 1;
			}
		}

		internal override void SetCurrentStepResult(ServiceResult<SingleItemType> result)
		{
			this.Result = result;
			base.CheckAndThrowFaultExceptionOnRequestLevelErrors<SingleItemType>(new ServiceResult<SingleItemType>[]
			{
				result
			});
		}
	}
}
