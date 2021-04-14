using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class SubscribeCommandBase : SingleStepServiceCommand<SubscribeRequest, SubscriptionBase>
	{
		public SubscribeCommandBase(CallContext callContext, SubscribeRequest request) : base(callContext, request)
		{
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			SubscribeResponse subscribeResponse = new SubscribeResponse();
			SubscribeResponseMessage message = new SubscribeResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
			subscribeResponse.AddResponse(message);
			return subscribeResponse;
		}

		internal override ServiceResult<SubscriptionBase> Execute()
		{
			this.ValidateOperation();
			IdAndSession[] folderIds = null;
			if (!base.Request.SubscriptionRequest.SubscribeToAllFolders)
			{
				folderIds = this.GetFolderIdsToMonitor();
			}
			if (base.CallContext.EffectiveCaller.ObjectGuid == Guid.Empty)
			{
				ExTraceGlobals.SubscribeCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[SubscribeCommandBase::ExecuteSingleStep] Effective caller has an object guid of Guid.Empty.  This likely means that the callers is from another forest.  Caller SID: {0}", base.CallContext.EffectiveCaller.ClientSecurityContext.UserSid.ToString());
				throw new ServiceAccessDeniedException(CoreResources.IDs.MessageInsufficientPermissionsToSubscribe);
			}
			SubscriptionBase subscriptionBase;
			try
			{
				subscriptionBase = this.CreateSubscriptionInstance(folderIds);
			}
			catch (AccessDeniedException innerException)
			{
				if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1))
				{
					throw new ServiceAccessDeniedException(CoreResources.IDs.MessageInsufficientPermissionsToSubscribe, innerException);
				}
				throw;
			}
			Subscriptions.Singleton.Add(subscriptionBase);
			if (subscriptionBase is StreamingSubscription && SubscribeRequest.HasPreferServerAffinityHeader(base.CallContext))
			{
				base.CallContext.HttpContext.Response.AppendHeader("X-FromBackend-ServerAffinity", bool.TrueString);
			}
			return new ServiceResult<SubscriptionBase>(subscriptionBase);
		}

		protected virtual void ValidateOperation()
		{
			if (base.Request.SubscriptionRequest.SubscribeToAllFolders && base.Request.SubscriptionRequest.FolderIds != null && base.Request.SubscriptionRequest.FolderIds.Length != 0)
			{
				throw new InvalidSubscriptionRequestException((CoreResources.IDs)2956059769U);
			}
			if (!base.Request.SubscriptionRequest.SubscribeToAllFolders && (base.Request.SubscriptionRequest.FolderIds == null || base.Request.SubscriptionRequest.FolderIds.Length == 0))
			{
				throw new InvalidSubscriptionRequestException((CoreResources.IDs)2362895530U);
			}
			IEwsBudget budget = CallContext.Current.Budget;
			if (!budget.ThrottlingPolicy.EwsMaxSubscriptions.IsUnlimited)
			{
				int subscriptionCountForUser = Subscriptions.Singleton.GetSubscriptionCountForUser(budget.Owner);
				if ((long)subscriptionCountForUser >= (long)((ulong)budget.ThrottlingPolicy.EwsMaxSubscriptions.Value))
				{
					Subscriptions.Singleton.RemoveExpiredSubscriptions();
					subscriptionCountForUser = Subscriptions.Singleton.GetSubscriptionCountForUser(budget.Owner);
					if ((long)subscriptionCountForUser >= (long)((ulong)budget.ThrottlingPolicy.EwsMaxSubscriptions.Value))
					{
						ExceededMaxSubscriptionLimitException.Throw();
					}
				}
			}
		}

		private IdAndSession[] GetFolderIdsToMonitor()
		{
			IdAndSession[] array = new IdAndSession[base.Request.SubscriptionRequest.FolderIds.Length];
			if (array.Length != 0)
			{
				array[0] = base.IdConverter.ConvertFolderIdToIdAndSessionReadOnly(base.Request.SubscriptionRequest.FolderIds[0]);
				if (array[0].Session is PublicFolderSession)
				{
					if (base.Request.SubscriptionRequest.FolderIds.Length > 1)
					{
						throw new InvalidSubscriptionRequestException();
					}
				}
				else
				{
					int i = 1;
					while (i < base.Request.SubscriptionRequest.FolderIds.Length)
					{
						array[i] = base.IdConverter.ConvertFolderIdToIdAndSessionReadOnly(base.Request.SubscriptionRequest.FolderIds[i]);
						if (array[i].Session != array[0].Session)
						{
							if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1))
							{
								throw new InvalidSubscriptionRequestException();
							}
							throw new EmailAddressMismatchException();
						}
						else
						{
							i++;
						}
					}
				}
			}
			return array;
		}

		protected abstract SubscriptionBase CreateSubscriptionInstance(IdAndSession[] folderIds);
	}
}
