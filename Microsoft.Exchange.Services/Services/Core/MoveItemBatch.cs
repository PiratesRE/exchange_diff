using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class MoveItemBatch : MoveCopyItemBatchCommandBase<MoveItemRequest, MoveItem>
	{
		public MoveItemBatch(CallContext callContext, MoveItemRequest request) : base(callContext, request)
		{
		}

		internal override TimeSpan? MaxExecutionTime
		{
			get
			{
				return new TimeSpan?(TimeSpan.FromMinutes(5.0));
			}
		}

		internal override ServiceResult<ItemType> Execute()
		{
			ExTraceGlobals.CopyItemCallTracer.TraceDebug((long)this.GetHashCode(), "MoveItemBatch.Execute called");
			if (base.Request.Ids != null && base.Request.Ids.Length > base.CurrentStep && base.LogItemId())
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "PreMoveId: ", string.Format("{0}:{1}", base.Request.Ids[base.CurrentStep].GetId(), base.Request.Ids[base.CurrentStep].GetChangeKey()));
			}
			if (base.CurrentStep == 0)
			{
				int num;
				if (base.TryMoveItemBatch(out num))
				{
					this.objectsChanged += num;
					base.LogCommandOptimizationToIIS(true);
				}
				else
				{
					base.FallbackCommand = new MoveItem(base.CallContext, base.Request);
					base.FallbackCommand.PreExecuteCommand();
				}
			}
			ServiceResult<ItemType> serviceResult;
			if (base.FallbackCommand != null)
			{
				base.FallbackCommand.CurrentStep = base.CurrentStep;
				serviceResult = base.FallbackCommand.Execute();
			}
			else
			{
				serviceResult = base.ServiceResults[base.CurrentStep];
			}
			if (serviceResult != null && serviceResult.Value != null && serviceResult.Value.ItemId != null && base.LogItemId())
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "PostMoveId: ", string.Format("{0}:{1}", serviceResult.Value.ItemId.Id, serviceResult.Value.ItemId.GetChangeKey()));
			}
			return serviceResult;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			MoveItemResponse moveItemResponse = new MoveItemResponse();
			moveItemResponse.BuildForResults<ItemType>(base.Results);
			return moveItemResponse;
		}
	}
}
