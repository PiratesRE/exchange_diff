using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class FindRemoteArchiveFolder : SingleStepServiceCommand<FindFolderRequest, FindFolderResponse>
	{
		public FindRemoteArchiveFolder(CallContext callContext, FindFolderRequest request) : base(callContext, request)
		{
			ServiceCommandBase.ThrowIfNull(request, "remoteArchiveRequest", "FindRemoteArchiveFolder::FindRemoteArchiveFolder");
			this.responseShape = Global.ResponseShapeResolver.GetResponseShape<FolderResponseShape>(request.ShapeName, request.FolderShape, null);
			this.paging = request.Paging;
			this.restriction = request.Restriction;
			this.parentFolderIds = request.ParentFolderIds;
			this.folderQueryTraversal = request.Traversal;
			this.returnParentFolder = request.ReturnParentFolder;
			this.archiveService = ((IRemoteArchiveRequest)request).ArchiveService;
			ServiceCommandBase.ThrowIfNullOrEmpty<BaseFolderId>(this.parentFolderIds, "this.parentFolderIds", "FindRemoteArchiveFolder::FindRemoteArchiveFolder");
			ServiceCommandBase.ThrowIfNull(this.responseShape, "this.responseShape", "FindRemoteArchiveFolder::FindRemoteArchiveFolder");
			ServiceCommandBase.ThrowIfNull(this.archiveService, "this.archiveService", "FindRemoteArchiveFolder::FindRemoteArchiveFolder");
			this.archiveService.Timeout = EwsClientHelper.RemoteEwsClientTimeout;
			this.FindFolderFunc = new Func<FindFolderType, FindFolderResponseType>(this.archiveService.FindFolder);
		}

		internal Func<FindFolderType, FindFolderResponseType> FindFolderFunc { get; set; }

		internal override ServiceResult<FindFolderResponse> Execute()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			FindFolderRequest findFolderRequest = new FindFolderRequest();
			findFolderRequest.FolderShape = this.responseShape;
			findFolderRequest.ParentFolderIds = this.parentFolderIds;
			findFolderRequest.Traversal = this.folderQueryTraversal;
			findFolderRequest.Paging = this.paging;
			findFolderRequest.Restriction = this.restriction;
			findFolderRequest.ReturnParentFolder = this.returnParentFolder;
			FindFolderResponse findFolderResponse;
			try
			{
				FindFolderType findFolderType = EwsClientHelper.Convert<FindFolderRequest, FindFolderType>(findFolderRequest);
				Exception ex = null;
				FindFolderResponseType findFolderResponseType = null;
				bool flag = EwsClientHelper.ExecuteEwsCall(delegate
				{
					findFolderResponseType = this.FindFolderFunc(findFolderType);
				}, out ex);
				if (flag)
				{
					findFolderResponse = EwsClientHelper.Convert<FindFolderResponseType, FindFolderResponse>(findFolderResponseType);
				}
				else
				{
					ServiceError error = new ServiceError((CoreResources.IDs)4160418372U, Microsoft.Exchange.Services.Core.Types.ResponseCodeType.ErrorInternalServerError, 0, ExchangeVersion.Exchange2012);
					ExTraceGlobals.SearchTracer.TraceError<string, string>((long)this.GetHashCode(), "[FindRemoteArchiveFolder::ExecuteRemoteArchiveFindFolder] Find folder against URL {0} failed with error: {1}.", this.archiveService.Url, ex.Message);
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(RequestDetailsLogger.Current, "FailedFindRemoteArchiveFolder", ex.Message);
					findFolderResponse = new FindFolderResponse();
					findFolderResponse.AddResponse(new FindFolderResponseMessage(ServiceResultCode.Error, error, null));
				}
			}
			finally
			{
				stopwatch.Stop();
				CallContext.Current.ProtocolLog.AppendGenericInfo("FindRemoteArchiveFolderProcessingTime", stopwatch.ElapsedMilliseconds);
			}
			return new ServiceResult<FindFolderResponse>(findFolderResponse);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return base.Result.Value;
		}

		private const string ProcessingTimeLogField = "FindRemoteArchiveFolderProcessingTime";

		private const string FailureMessageLogField = "FailedFindRemoteArchiveFolder";

		private FolderResponseShape responseShape;

		private Microsoft.Exchange.Services.Core.Search.BasePagingType paging;

		private Microsoft.Exchange.Services.Core.Types.RestrictionType restriction;

		private BaseFolderId[] parentFolderIds;

		private FolderQueryTraversal folderQueryTraversal;

		private readonly bool returnParentFolder;

		private ExchangeServiceBinding archiveService;
	}
}
