using System;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class CopyItemBatch : MoveCopyItemBatchCommandBase<CopyItemRequest, CopyItem>
	{
		public CopyItemBatch(CallContext callContext, CopyItemRequest request) : base(callContext, request)
		{
		}

		internal override ServiceResult<ItemType> Execute()
		{
			ExTraceGlobals.CopyItemCallTracer.TraceDebug((long)this.GetHashCode(), "CopyItemBatch.Execute called");
			if (base.CurrentStep == 0)
			{
				int num;
				if (base.TryCopyItemBatch(out num))
				{
					this.objectsChanged += num;
					base.LogCommandOptimizationToIIS(true);
				}
				else
				{
					base.FallbackCommand = new CopyItem(base.CallContext, base.Request);
					base.FallbackCommand.PreExecuteCommand();
				}
			}
			if (base.FallbackCommand != null)
			{
				base.FallbackCommand.CurrentStep = base.CurrentStep;
				return base.FallbackCommand.Execute();
			}
			return base.ServiceResults[base.CurrentStep];
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			CopyItemResponse copyItemResponse = new CopyItemResponse();
			copyItemResponse.BuildForResults<ItemType>(base.Results);
			return copyItemResponse;
		}
	}
}
