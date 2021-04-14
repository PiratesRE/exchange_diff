using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class AddDelegate : DelegateCommandBase<AddDelegateRequest>
	{
		public AddDelegate(CallContext callContext, AddDelegateRequest request) : base(callContext, request)
		{
		}

		internal override int StepCount
		{
			get
			{
				return base.Request.DelegateUsers.Length;
			}
		}

		internal override void PreExecuteCommand()
		{
			base.PreExecuteCommand();
			this.delegateUserCollectionHandler = new DelegateUserCollectionHandler(base.GetMailboxIdentityMailboxSession(), base.CallContext.ADRecipientSessionContext);
		}

		internal override ServiceResult<DelegateUserType> Execute()
		{
			DelegateUserType delegateUserType = base.Request.DelegateUsers[base.CurrentStep];
			DelegateUser delegateUser = this.delegateUserCollectionHandler.AddDelegate(delegateUserType);
			delegateUserType = DelegateUtilities.BuildDelegateUserResponse(delegateUser, false);
			this.saveDelegateUsers = true;
			return new ServiceResult<DelegateUserType>(delegateUserType);
		}

		internal override void PostExecuteCommand()
		{
			if (base.Request.DeliverMeetingRequests != DeliverMeetingRequestsType.None)
			{
				this.delegateUserCollectionHandler.SetDelegateOptions(base.Request.DeliverMeetingRequests);
			}
			if (this.saveDelegateUsers)
			{
				DelegateUserCollectionSaveResult delegateUserCollectionSaveResult = this.delegateUserCollectionHandler.SaveDelegate(false);
				if (delegateUserCollectionSaveResult.Problems.Count > 0)
				{
					if (ExTraceGlobals.AddDelegateCallTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.AddDelegateCallTracer.TraceError<string>((long)this.GetHashCode(), "Failed to Save delegates due to the following problems: {0}", DelegateUtilities.BuildErrorStringFromProblems(delegateUserCollectionSaveResult.Problems));
					}
					throw new DelegateSaveFailedException(CoreResources.IDs.ErrorAddDelegatesFailed);
				}
			}
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			ServiceResult<DelegateUserResponseMessageType[]> serviceResult;
			if (base.PreExecuteSucceeded)
			{
				DelegateUserResponseMessageType[] value = DelegateUtilities.BuildDelegateResponseFromResults(base.Results);
				serviceResult = new ServiceResult<DelegateUserResponseMessageType[]>(value);
			}
			else
			{
				serviceResult = new ServiceResult<DelegateUserResponseMessageType[]>(base.Results[0].Code, null, base.Results[0].Error);
			}
			return new AddDelegateResponseMessage(serviceResult.Code, serviceResult.Error, serviceResult.Value);
		}

		private bool saveDelegateUsers;

		private DelegateUserCollectionHandler delegateUserCollectionHandler;
	}
}
