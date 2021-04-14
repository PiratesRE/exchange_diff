using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class UpdateDelegate : DelegateCommandBase<UpdateDelegateRequest>
	{
		public UpdateDelegate(CallContext callContext, UpdateDelegateRequest request) : base(callContext, request)
		{
		}

		internal override void PreExecuteCommand()
		{
			base.PreExecuteCommand();
			if (base.Request.DelegateUsers == null && base.Request.DeliverMeetingRequests == DeliverMeetingRequestsType.None)
			{
				throw new InvalidRequestException(CoreResources.IDs.MessageMissingUpdateDelegateRequestInformation);
			}
			this.totalDelegateUsers = ((base.Request.DelegateUsers != null) ? base.Request.DelegateUsers.Length : 0);
			this.delegateUserCollectionHandler = new DelegateUserCollectionHandler(base.GetMailboxIdentityMailboxSession(), base.CallContext.ADRecipientSessionContext);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			ServiceResult<DelegateUserResponseMessageType[]> serviceResult;
			if (base.PreExecuteSucceeded)
			{
				DelegateUserResponseMessageType[] value = null;
				if (this.totalDelegateUsers > 0)
				{
					value = DelegateUtilities.BuildDelegateResponseFromResults(base.Results);
				}
				serviceResult = new ServiceResult<DelegateUserResponseMessageType[]>(value);
			}
			else
			{
				serviceResult = new ServiceResult<DelegateUserResponseMessageType[]>(base.Results[0].Code, null, base.Results[0].Error);
			}
			return new UpdateDelegateResponseMessage(serviceResult.Code, serviceResult.Error, serviceResult.Value);
		}

		internal override int StepCount
		{
			get
			{
				return this.totalDelegateUsers;
			}
		}

		internal override void PostExecuteCommand()
		{
			if (base.Request.DeliverMeetingRequests != DeliverMeetingRequestsType.None)
			{
				this.delegateUserCollectionHandler.SetDelegateOptions(base.Request.DeliverMeetingRequests);
			}
			if (this.saveDelegateUsers || this.totalDelegateUsers == 0)
			{
				DelegateUserCollectionSaveResult delegateUserCollectionSaveResult = this.delegateUserCollectionHandler.SaveDelegate(false);
				if (delegateUserCollectionSaveResult.Problems.Count > 0)
				{
					if (ExTraceGlobals.UpdateDelegateCallTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.UpdateDelegateCallTracer.TraceError<string>((long)this.GetHashCode(), "Failed to Save delegates due to the following problems: {0}", DelegateUtilities.BuildErrorStringFromProblems(delegateUserCollectionSaveResult.Problems));
					}
					throw new DelegateSaveFailedException(CoreResources.IDs.ErrorUpdateDelegatesFailed);
				}
			}
		}

		internal override ServiceResult<DelegateUserType> Execute()
		{
			DelegateUserType delegateUser = base.Request.DelegateUsers[base.CurrentStep];
			DelegateUser delegateUser2 = this.delegateUserCollectionHandler.UpdateDelegate(delegateUser);
			this.saveDelegateUsers = true;
			DelegateUserType value = DelegateUtilities.BuildDelegateUserResponse(delegateUser2, false);
			return new ServiceResult<DelegateUserType>(value);
		}

		private bool saveDelegateUsers;

		private int totalDelegateUsers = -1;

		private DelegateUserCollectionHandler delegateUserCollectionHandler;
	}
}
