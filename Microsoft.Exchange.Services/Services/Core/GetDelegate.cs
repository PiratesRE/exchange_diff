using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetDelegate : DelegateCommandBase<GetDelegateRequest>
	{
		public GetDelegate(CallContext callContext, GetDelegateRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			DelegateUserResponseMessageType[] value = null;
			if (base.Results != null)
			{
				value = DelegateUtilities.BuildDelegateResponseFromResults(base.Results);
			}
			DeliverMeetingRequestsType deliverMeetingRequest = (this.xsoDelegateUsers != null) ? DelegateUtilities.ConvertToDeliverMeetingRequestType(this.xsoDelegateUsers.DelegateRuleType) : DeliverMeetingRequestsType.None;
			ServiceResult<DelegateUserResponseMessageType[]> serviceResult;
			if (base.PreExecuteSucceeded)
			{
				serviceResult = new ServiceResult<DelegateUserResponseMessageType[]>(value);
			}
			else
			{
				serviceResult = new ServiceResult<DelegateUserResponseMessageType[]>(base.Results[0].Code, null, base.Results[0].Error);
			}
			return new GetDelegateResponseMessage(serviceResult.Code, serviceResult.Error, serviceResult.Value, deliverMeetingRequest);
		}

		internal override void PreExecuteCommand()
		{
			base.PreExecuteCommand();
			this.stepCount = ((base.Request.UserIds != null) ? base.Request.UserIds.Length : this.xsoDelegateUsers.Count);
		}

		internal override int StepCount
		{
			get
			{
				return this.stepCount;
			}
		}

		internal override ServiceResult<DelegateUserType> Execute()
		{
			DelegateUser delegateUser;
			if (base.Request.UserIds != null)
			{
				UserId user = base.Request.UserIds[base.CurrentStep];
				delegateUser = DelegateUtilities.GetDelegateUser(user, this.xsoDelegateUsers, base.CallContext.ADRecipientSessionContext);
			}
			else
			{
				delegateUser = this.xsoDelegateUsers[base.CurrentStep];
			}
			DelegateUserType value = this.BuildDelegateResponse(delegateUser);
			return new ServiceResult<DelegateUserType>(value);
		}

		private DelegateUserType BuildDelegateResponse(DelegateUser delegateUser)
		{
			if ((delegateUser.Problems & DelegateProblems.NoADUser) == DelegateProblems.NoADUser)
			{
				throw new DelegateExceptionInvalidDelegateUser(CoreResources.IDs.ErrorDelegateNoUser);
			}
			if ((delegateUser.Problems & DelegateProblems.NoADPublicDelegate) == DelegateProblems.NoADPublicDelegate || (delegateUser.Problems & DelegateProblems.NoDelegateInfo) == DelegateProblems.NoDelegateInfo)
			{
				throw new DelegateExceptionInvalidDelegateUser((CoreResources.IDs)3438146603U);
			}
			return DelegateUtilities.BuildDelegateUserResponse(delegateUser, base.Request.IncludePermissions);
		}

		private int stepCount;
	}
}
