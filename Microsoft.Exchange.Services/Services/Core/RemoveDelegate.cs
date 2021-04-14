using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class RemoveDelegate : DelegateCommandBase<RemoveDelegateRequest>
	{
		public RemoveDelegate(CallContext callContext, RemoveDelegateRequest request) : base(callContext, request)
		{
		}

		internal override void PreExecuteCommand()
		{
			base.PreExecuteCommand();
			this.delegateUserCollectionHandler = new DelegateUserCollectionHandler(base.GetMailboxIdentityMailboxSession(), base.CallContext.ADRecipientSessionContext);
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
			return new RemoveDelegateResponseMessage(serviceResult.Code, serviceResult.Error, serviceResult.Value);
		}

		internal override void PostExecuteCommand()
		{
			if (this.saveDelegateUsers)
			{
				DelegateUserCollectionSaveResult delegateUserCollectionSaveResult = this.delegateUserCollectionHandler.SaveDelegate(false);
				if (delegateUserCollectionSaveResult.Problems.Count > 0)
				{
					if (ExTraceGlobals.RemoveDelegateCallTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.RemoveDelegateCallTracer.TraceError<string>((long)this.GetHashCode(), "Failed to Save delegates due to the following problems: {0}", DelegateUtilities.BuildErrorStringFromProblems(delegateUserCollectionSaveResult.Problems));
					}
					throw new DelegateSaveFailedException((CoreResources.IDs)3763931121U);
				}
			}
		}

		internal override ServiceResult<DelegateUserType> Execute()
		{
			UserId user = base.Request.UserIds[base.CurrentStep];
			this.delegateUserCollectionHandler.RemoveDelegate(user);
			this.saveDelegateUsers = true;
			return new ServiceResult<DelegateUserType>(null);
		}

		internal override int StepCount
		{
			get
			{
				return base.Request.UserIds.Length;
			}
		}

		private bool saveDelegateUsers;

		private DelegateUserCollectionHandler delegateUserCollectionHandler;
	}
}
