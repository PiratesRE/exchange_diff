using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class MarkAllItemsAsRead : MultiStepServiceCommand<MarkAllItemsAsReadRequest, ServiceResultNone>
	{
		public MarkAllItemsAsRead(CallContext callContext, MarkAllItemsAsReadRequest request) : base(callContext, request)
		{
			this.InitializeTracers();
		}

		internal override void PreExecuteCommand()
		{
			this.folderIds = base.Request.FolderIds;
			this.readFlag = base.Request.ReadFlag;
			this.supressReadReceipts = base.Request.SuppressReadReceipts;
			ServiceCommandBase.ThrowIfNullOrEmpty<BaseFolderId>(this.folderIds, "folderIds", "MarkAllItemsAsRead::PreExecuteCommand");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			MarkAllItemsAsReadResponse markAllItemsAsReadResponse = new MarkAllItemsAsReadResponse();
			markAllItemsAsReadResponse.BuildForNoReturnValue(base.Results);
			return markAllItemsAsReadResponse;
		}

		internal override int StepCount
		{
			get
			{
				return this.folderIds.Length;
			}
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			IdAndSession folderIdAndSession = base.IdConverter.ConvertFolderIdToIdAndSessionReadOnly(this.folderIds[base.CurrentStep]);
			this.MarkAllItemsAsReadOrUnread(folderIdAndSession);
			this.objectsChanged++;
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		private void MarkAllItemsAsReadOrUnread(IdAndSession folderIdAndSession)
		{
			this.tracer.TraceDebug<StoreId>((long)this.GetHashCode(), "MarkAllItemsAsReadOrUnread called for folderId '{0}'", folderIdAndSession.Id);
			bool suppressReadReceipts = this.supressReadReceipts;
			MailboxSession mailboxSession = folderIdAndSession.Session as MailboxSession;
			if (mailboxSession != null)
			{
				StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.JunkEmail);
				if (folderIdAndSession.Id.Equals(defaultFolderId))
				{
					suppressReadReceipts = true;
				}
			}
			else if (folderIdAndSession.Session is PublicFolderSession)
			{
				suppressReadReceipts = true;
			}
			using (Folder folder = Folder.Bind(folderIdAndSession.Session, folderIdAndSession.Id, null))
			{
				if (this.readFlag)
				{
					folder.MarkAllAsRead(suppressReadReceipts);
				}
				else
				{
					folder.MarkAllAsUnread(suppressReadReceipts);
				}
			}
		}

		protected override void LogTracesForCurrentRequest()
		{
			ServiceCommandBase.TraceLoggerFactory.Create(base.CallContext.HttpContext.Response.Headers).LogTraces(this.requestTracer);
		}

		private void InitializeTracers()
		{
			ITracer tracer;
			if (!base.IsRequestTracingEnabled)
			{
				ITracer instance = NullTracer.Instance;
				tracer = instance;
			}
			else
			{
				tracer = new InMemoryTracer(ExTraceGlobals.MarkAllItemsAsReadTracer.Category, ExTraceGlobals.MarkAllItemsAsReadTracer.TraceTag);
			}
			this.requestTracer = tracer;
			this.tracer = ExTraceGlobals.MarkAllItemsAsReadTracer.Compose(this.requestTracer);
		}

		protected BaseFolderId[] folderIds;

		private bool readFlag;

		protected bool supressReadReceipts;

		protected ITracer tracer = ExTraceGlobals.MarkAllItemsAsReadTracer;

		private ITracer requestTracer = NullTracer.Instance;
	}
}
