using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetRemoteArchiveFolder : SingleStepServiceCommand<GetFolderRequest, GetFolderResponse>
	{
		public GetRemoteArchiveFolder(CallContext callContext, GetFolderRequest request) : base(callContext, request)
		{
			ServiceCommandBase.ThrowIfNull(request, "remoteArchiveRequest", "GetRemoteArchiveFolder::GetRemoteArchiveFolder");
			this.folderIds = request.Ids;
			this.responseShape = Global.ResponseShapeResolver.GetResponseShape<FolderResponseShape>(request.ShapeName, request.FolderShape, null);
			this.archiveService = ((IRemoteArchiveRequest)request).ArchiveService;
			ServiceCommandBase.ThrowIfNullOrEmpty<BaseFolderId>(this.folderIds, "this.folderIds", "GetRemoteArchiveFolder::GetRemoteArchiveFolder");
			ServiceCommandBase.ThrowIfNull(this.responseShape, "this.responseShape", "GetRemoteArchiveFolder::GetRemoteArchiveFolder");
			ServiceCommandBase.ThrowIfNull(this.archiveService, "this.archiveService", "GetRemoteArchiveFolder::GetRemoteArchiveFolder");
			this.archiveService.Timeout = EwsClientHelper.RemoteEwsClientTimeout;
			this.GetFolderFunc = new Func<GetFolderType, GetFolderResponseType>(this.archiveService.GetFolder);
		}

		internal Func<GetFolderType, GetFolderResponseType> GetFolderFunc { get; set; }

		internal override ServiceResult<GetFolderResponse> Execute()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			GetFolderRequest getFolderRequest = new GetFolderRequest();
			getFolderRequest.Ids = this.folderIds;
			getFolderRequest.FolderShape = this.responseShape;
			GetFolderResponse getFolderResponse;
			try
			{
				GetFolderType getFolderType = EwsClientHelper.Convert<GetFolderRequest, GetFolderType>(getFolderRequest);
				Exception ex = null;
				GetFolderResponseType getFolderResponseType = null;
				bool flag = EwsClientHelper.ExecuteEwsCall(delegate
				{
					getFolderResponseType = this.GetFolderFunc(getFolderType);
				}, out ex);
				if (flag)
				{
					getFolderResponse = EwsClientHelper.Convert<GetFolderResponseType, GetFolderResponse>(getFolderResponseType);
				}
				else
				{
					ServiceError error = new ServiceError(CoreResources.IDs.ErrorGetRemoteArchiveFolderFailed, Microsoft.Exchange.Services.Core.Types.ResponseCodeType.ErrorInternalServerError, 0, ExchangeVersion.Exchange2012);
					ExTraceGlobals.SearchTracer.TraceError<string, string>((long)this.GetHashCode(), "[GetRemoteArchiveFolder::ExecuteRemoteArchiveGetFolder] Get folder against URL {0} failed with error: {1}.", this.archiveService.Url, ex.Message);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(RequestDetailsLogger.Current, "FailedGetRemoteArchiveFolder", ex.Message);
					getFolderResponse = new GetFolderResponse();
					getFolderResponse.AddResponse(getFolderResponse.CreateResponseMessage<Microsoft.Exchange.Services.Core.Types.BaseFolderType>(ServiceResultCode.Error, error, null));
				}
			}
			finally
			{
				stopwatch.Stop();
				RequestDetailsLogger.Current.AppendGenericInfo("GetRemoteArchiveFolderProcessingTime", stopwatch.ElapsedMilliseconds);
			}
			return new ServiceResult<GetFolderResponse>(getFolderResponse);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return base.Result.Value;
		}

		private const string ProcessingTimeLogField = "GetRemoteArchiveFolderProcessingTime";

		private const string FailureMessageLogField = "FailedGetRemoteArchiveFolder";

		private BaseFolderId[] folderIds;

		private FolderResponseShape responseShape;

		private ExchangeServiceBinding archiveService;
	}
}
