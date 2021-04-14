using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class CreateAttachmentFromUri : ServiceCommand<string>
	{
		public CreateAttachmentFromUri(CallContext callContext, ItemId itemId, string uri, string name, string subscriptionId) : base(callContext)
		{
			if (itemId == null)
			{
				throw new ArgumentNullException("itemId");
			}
			if (string.IsNullOrWhiteSpace(uri))
			{
				throw new ArgumentNullException("uri");
			}
			this.itemId = itemId;
			this.uri = new Uri(uri);
			this.name = name;
			this.subscriptionId = subscriptionId;
		}

		protected override string InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true);
			Guid operationId = Guid.NewGuid();
			CreateAttachmentFromUri.DownloadAndAttachFileFromUri(this.uri, this.name, this.subscriptionId, operationId, this.itemId, userContext, base.IdConverter);
			return operationId.ToString();
		}

		internal static void DownloadAndAttachFileFromUri(Uri uri, string name, string subscriptionId, Guid operationId, ItemId itemId, UserContext userContext, IdConverter idConverter)
		{
			try
			{
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(async delegate()
				{
					try
					{
						using (HttpClient client = new HttpClient())
						{
							using (HttpResponseMessage response = await client.GetAsync(uri))
							{
								HttpStatusCode statusCode = response.StatusCode;
								AttachmentResultCode resultCode;
								if (statusCode != HttpStatusCode.OK)
								{
									switch (statusCode)
									{
									case HttpStatusCode.Forbidden:
										resultCode = AttachmentResultCode.AccessDenied;
										break;
									case HttpStatusCode.NotFound:
										resultCode = AttachmentResultCode.NotFound;
										break;
									default:
										if (statusCode != HttpStatusCode.RequestTimeout)
										{
											resultCode = AttachmentResultCode.GenericFailure;
										}
										else
										{
											resultCode = AttachmentResultCode.Timeout;
										}
										break;
									}
								}
								else
								{
									resultCode = AttachmentResultCode.Success;
								}
								if (resultCode != AttachmentResultCode.Success)
								{
									CreateAttachmentHelper.SendFailureNotification(userContext, subscriptionId, operationId.ToString(), resultCode, null, null);
								}
								byte[] buffer = await response.Content.ReadAsByteArrayAsync();
								CreateAttachmentNotificationPayload result = new CreateAttachmentNotificationPayload
								{
									SubscriptionId = subscriptionId,
									Id = operationId.ToString(),
									Bytes = buffer,
									Item = null,
									ResultCode = resultCode
								};
								CreateAttachmentHelper.CreateAttachmentAndSendPendingGetNotification(userContext, itemId.Id, buffer, name, result, idConverter);
							}
						}
					}
					catch (TaskCanceledException)
					{
					}
				});
			}
			catch (GrayException ex)
			{
				CreateAttachmentHelper.SendFailureNotification(userContext, subscriptionId, operationId.ToString(), AttachmentResultCode.GenericFailure, null, ex);
				ExTraceGlobals.AttachmentHandlingTracer.TraceError<string>(0L, "CreateAttachmentFromUri.DownloadAndAttachFileFromUri Exception while trying to download and attach file async : {0}", ex.StackTrace);
			}
		}

		private readonly string name;

		private readonly string subscriptionId;

		private ItemId itemId;

		private Uri uri;
	}
}
