using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class ExportRemoteArchiveItems : SingleStepServiceCommand<ExportItemsRequest, ExportItemsResponse>
	{
		public ExportRemoteArchiveItems(CallContext callContext, ExportItemsRequest request) : base(callContext, request)
		{
			ServiceCommandBase.ThrowIfNull(request, "remoteArchiveRequest", "ExportRemoteArchiveItems::ExportRemoteArchiveItems");
			this.ids = request.Ids;
			this.archiveService = ((IRemoteArchiveRequest)request).ArchiveService;
			ServiceCommandBase.ThrowIfNull(this.ids, "this.ids", "ExportRemoteArchiveItems::ExportRemoteArchiveItems");
			ServiceCommandBase.ThrowIfNull(this.archiveService, "this.archiveService", "ExportRemoteArchiveItems::ExportRemoteArchiveItems");
			this.archiveService.Timeout = EwsClientHelper.RemoteEwsClientTimeout;
			this.ExportItemsFunc = new Func<ExportItemsType, ExportItemsResponseType>(this.archiveService.ExportItems);
		}

		internal Func<ExportItemsType, ExportItemsResponseType> ExportItemsFunc { get; set; }

		internal override ServiceResult<ExportItemsResponse> Execute()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			ExportItemsRequest exportItemsRequest = new ExportItemsRequest();
			exportItemsRequest.Ids = this.ids;
			ExportItemsResponse exportItemsResponse;
			try
			{
				ExportItemsType exportItemsType = EwsClientHelper.Convert<ExportItemsRequest, ExportItemsType>(exportItemsRequest);
				Exception ex = null;
				ExportItemsResponseType exportItemsResponseType = null;
				bool flag = EwsClientHelper.ExecuteEwsCall(delegate
				{
					exportItemsResponseType = this.ExportItemsFunc(exportItemsType);
				}, out ex);
				if (flag)
				{
					exportItemsResponse = EwsClientHelper.Convert<ExportItemsResponseType, ExportItemsResponse>(exportItemsResponseType);
				}
				else
				{
					ServiceError error = new ServiceError(CoreResources.IDs.ErrorExportRemoteArchiveItemsFailed, Microsoft.Exchange.Services.Core.Types.ResponseCodeType.ErrorInternalServerError, 0, ExchangeVersion.Exchange2012);
					ExTraceGlobals.SearchTracer.TraceError<string, string>((long)this.GetHashCode(), "[ExportRemoteArchiveItems::ExecuteRemoteArchiveExportItems] Export items against URL {0} failed with error: {1}.", this.archiveService.Url, ex.Message);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(RequestDetailsLogger.Current, "FailedExportRemoteArchiveItems", ex.Message);
					exportItemsResponse = new ExportItemsResponse();
					exportItemsResponse.AddResponse(exportItemsResponse.CreateResponseMessage<ExportItemsResponseMessage>(ServiceResultCode.Error, error, null));
				}
			}
			finally
			{
				stopwatch.Stop();
				RequestDetailsLogger.Current.AppendGenericInfo("ExportRemoteArchiveItemsProcessingTime", stopwatch.ElapsedMilliseconds);
			}
			return new ServiceResult<ExportItemsResponse>(exportItemsResponse);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return base.Result.Value;
		}

		private const string ProcessingTimeLogField = "ExportRemoteArchiveItemsProcessingTime";

		private const string FailureMessageLogField = "FailedExportRemoteArchiveItems";

		private XmlNodeArray ids;

		private ExchangeServiceBinding archiveService;
	}
}
