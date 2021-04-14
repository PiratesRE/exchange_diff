using System;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class CreateReferenceAttachmentFromLocalFile : ServiceCommand<string>
	{
		public CreateReferenceAttachmentFromLocalFile(CallContext callContext, CreateReferenceAttachmentFromLocalFileRequest requestObject, bool isHtml5) : base(callContext)
		{
			if (requestObject == null)
			{
				throw new ArgumentNullException("requestObject");
			}
			if (requestObject.ParentItemId == null)
			{
				throw new OwaInvalidRequestException("RequestObject.ParentItemId cannot be null");
			}
			this.isHtml5 = isHtml5;
			this.itemId = requestObject.ParentItemId;
			this.fileName = requestObject.FileName;
			this.fileContent = Convert.FromBase64String(requestObject.FileContentToUpload);
			this.subscriptionId = requestObject.SubscriptionId;
			this.channelId = requestObject.ChannelId;
			this.cancellationId = requestObject.CancellationId;
			UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true);
			if (userContext.IsGroupUserContext && this.channelId == null)
			{
				throw new OwaInvalidRequestException("RequestObject.ChannelId cannot be null for a group request");
			}
		}

		protected override string InternalExecute()
		{
			if (!this.isHtml5)
			{
				CreateAttachmentHelper.UpdateContentType(base.CallContext);
			}
			UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true);
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			CancellationToken token = cancellationTokenSource.Token;
			if (this.cancellationId != null)
			{
				bool flag = userContext.CancelAttachmentManager.OnCreateAttachment(this.cancellationId, cancellationTokenSource);
				if (flag)
				{
					CreateAttachmentHelper.SendFailureNotification(userContext, this.subscriptionId, null, AttachmentResultCode.Cancelled, this.channelId, null);
					return null;
				}
			}
			Guid operationId = Guid.NewGuid();
			CreateReferenceAttachmentFromLocalFile.UploadAndAttachReferenceAttachment(operationId, userContext, base.CallContext, this.itemId, this.fileName, this.fileContent, base.IdConverter, this.subscriptionId, this.channelId, token, this.cancellationId);
			return operationId.ToString();
		}

		internal static void UploadAndAttachReferenceAttachment(Guid operationId, UserContext userContext, CallContext callContext, ItemId itemId, string fileName, byte[] fileContent, IdConverter idConverter, string subscriptionId, string channelId, CancellationToken cancellationToken, string cancellationId)
		{
			AttachmentResultCode errorCode = AttachmentResultCode.GenericFailure;
			AttachmentIdType attachmentId = null;
			Exception exception = null;
			try
			{
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(async delegate()
				{
					try
					{
						AttachmentDataProvider attachmentDataProvider = userContext.AttachmentDataProviderManager.GetDefaultUploadDataProvider(callContext);
						if (attachmentDataProvider == null)
						{
							throw new InvalidOperationException("The user has no default data provider");
						}
						UploadItemAsyncResult uploadResult = await attachmentDataProvider.UploadItemAsync(fileContent, fileName, cancellationToken, callContext).ConfigureAwait(false);
						CreateAttachmentNotificationPayload notificationPayload = new CreateAttachmentNotificationPayload
						{
							SubscriptionId = subscriptionId,
							Id = operationId.ToString(),
							Item = uploadResult.Item,
							ResultCode = uploadResult.ResultCode
						};
						if (uploadResult.ResultCode == AttachmentResultCode.Success)
						{
							notificationPayload.Response = CreateReferenceAttachmentFromAttachmentDataProvider.AttachReferenceAttachment(attachmentDataProvider, userContext, uploadResult.Item.Location, string.Empty, itemId.Id, idConverter, null, uploadResult.Item.ProviderEndpointUrl);
							attachmentId = CreateAttachmentHelper.GetAttachmentIdFromCreateAttachmentResponse(notificationPayload.Response);
						}
						if (!userContext.IsDisposed)
						{
							try
							{
								userContext.LockAndReconnectMailboxSession();
								CreateAttachmentHelper.SendPendingGetNotification(userContext, notificationPayload, channelId);
							}
							finally
							{
								userContext.UnlockAndDisconnectMailboxSession();
							}
						}
					}
					catch (OperationCanceledException exception)
					{
						errorCode = AttachmentResultCode.Cancelled;
						exception = exception;
						if (cancellationId != null)
						{
							userContext.CancelAttachmentManager.CreateAttachmentCancelled(cancellationId);
						}
					}
				});
			}
			catch (GrayException ex)
			{
				ExTraceGlobals.AttachmentHandlingTracer.TraceError<string>(0L, "CreateReferenceAttachmentFromLocalFile.UploadAndAttachReferenceAttachment Exception while trying to upload and attach file async : {0}", ex.StackTrace);
				exception = ex;
			}
			finally
			{
				if (cancellationId != null)
				{
					userContext.CancelAttachmentManager.CreateAttachmentCompleted(cancellationId, attachmentId);
				}
				if (exception != null)
				{
					CreateAttachmentHelper.SendFailureNotification(userContext, subscriptionId, operationId.ToString(), errorCode, channelId, exception);
				}
			}
		}

		private readonly ItemId itemId;

		private readonly string fileName;

		private readonly byte[] fileContent;

		private readonly string subscriptionId;

		private readonly string channelId;

		private readonly string cancellationId;

		private readonly bool isHtml5;
	}
}
