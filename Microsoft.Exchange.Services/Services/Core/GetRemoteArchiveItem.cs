using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetRemoteArchiveItem : SingleStepServiceCommand<GetItemRequest, GetItemResponse>
	{
		public GetRemoteArchiveItem(CallContext callContext, GetItemRequest request) : base(callContext, request)
		{
			ServiceCommandBase.ThrowIfNull(request, "remoteArchiveRequest", "GetRemoteArchiveItem::GetRemoteArchiveItem");
			this.itemIds = request.Ids;
			this.responseShape = Global.ResponseShapeResolver.GetResponseShape<ItemResponseShape>(base.Request.ShapeName, base.Request.ItemShape, null);
			this.archiveService = ((IRemoteArchiveRequest)request).ArchiveService;
			ServiceCommandBase.ThrowIfNullOrEmpty<BaseItemId>(this.itemIds, "this.itemIds", "GetRemoteArchiveItem::GetRemoteArchiveItem");
			ServiceCommandBase.ThrowIfNull(this.responseShape, "this.responseShape", "GetRemoteArchiveItem::GetRemoteArchiveItem");
			ServiceCommandBase.ThrowIfNull(this.archiveService, "this.archiveService", "GetRemoteArchiveItem::GetRemoteArchiveItem");
			this.archiveService.Timeout = EwsClientHelper.RemoteEwsClientTimeout;
			this.GetItemFunc = new Func<GetItemType, GetItemResponseType>(this.archiveService.GetItem);
		}

		internal Func<GetItemType, GetItemResponseType> GetItemFunc { get; set; }

		internal override ServiceResult<GetItemResponse> Execute()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			GetItemRequest getItemRequest = new GetItemRequest();
			getItemRequest.Ids = this.itemIds;
			getItemRequest.ItemShape = this.responseShape;
			GetItemResponse getItemResponse;
			try
			{
				GetItemType getItemType = EwsClientHelper.Convert<GetItemRequest, GetItemType>(getItemRequest);
				Exception ex = null;
				GetItemResponseType getItemResponseType = null;
				bool flag = EwsClientHelper.ExecuteEwsCall(delegate
				{
					getItemResponseType = this.GetItemFunc(getItemType);
				}, out ex);
				if (flag)
				{
					getItemResponse = EwsClientHelper.Convert<GetItemResponseType, GetItemResponse>(getItemResponseType);
				}
				else
				{
					ServiceError error = new ServiceError((CoreResources.IDs)4106572054U, Microsoft.Exchange.Services.Core.Types.ResponseCodeType.ErrorInternalServerError, 0, ExchangeVersion.Exchange2012);
					ExTraceGlobals.SearchTracer.TraceError<string, string>((long)this.GetHashCode(), "[GetRemoteArchiveItem::ExecuteRemoteArchiveGetItem] Get item against URL {0} failed with error: {1}.", this.archiveService.Url, ex.Message);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(RequestDetailsLogger.Current, "FailedGetRemoteArchiveItem", ex.Message);
					getItemResponse = new GetItemResponse();
					getItemResponse.AddResponse(getItemResponse.CreateResponseMessage<Microsoft.Exchange.Services.Core.Types.ItemType>(ServiceResultCode.Error, error, null));
				}
			}
			finally
			{
				stopwatch.Stop();
				CallContext.Current.ProtocolLog.AppendGenericInfo("GetRemoteArchiveItemProcessingTime", stopwatch.ElapsedMilliseconds);
			}
			return new ServiceResult<GetItemResponse>(getItemResponse);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return base.Result.Value;
		}

		private const string ProcessingTimeLogField = "GetRemoteArchiveItemProcessingTime";

		private const string FailureMessageLogField = "FailedGetRemoteArchiveItem";

		private BaseItemId[] itemIds;

		private ItemResponseShape responseShape;

		private ExchangeServiceBinding archiveService;
	}
}
