using System;
using System.Threading;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SynchronizeWacAttachment : ServiceCommand<IAsyncResult>
	{
		public SynchronizeWacAttachment(CallContext callContext, string attachmentId, AsyncCallback asyncCallback, object asyncState) : base(callContext)
		{
			OwsLogRegistry.Register("SynchronizeWacAttachment", typeof(SynchronizeWacAttachmentMetadata), new Type[0]);
			this.attachmentId = attachmentId;
			this.asyncResult = new ServiceAsyncResult<SynchronizeWacAttachmentResponse>();
			this.asyncResult.AsyncCallback = asyncCallback;
			this.asyncResult.AsyncState = asyncState;
		}

		protected override IAsyncResult InternalExecute()
		{
			this.timer = new Timer(new TimerCallback(this.Retry), null, TimeSpan.Zero, this.retryInterval);
			return this.asyncResult;
		}

		private void Retry(object unused)
		{
			CallContext.SetCurrent(base.CallContext);
			if (this.ReadyToSend())
			{
				this.Complete(SynchronizeWacAttachmentResult.Success);
				return;
			}
			this.checks++;
			if (this.checks > 10)
			{
				this.Complete(SynchronizeWacAttachmentResult.StillEditing);
			}
		}

		private void Complete(SynchronizeWacAttachmentResult result)
		{
			if (this.timer != null)
			{
				this.timer.Dispose();
			}
			this.asyncResult.Data = new SynchronizeWacAttachmentResponse(result);
			this.asyncResult.Complete(this);
		}

		private bool ReadyToSend()
		{
			bool result;
			lock (this.asyncResult)
			{
				if (this.asyncResult.IsCompleted)
				{
					result = true;
				}
				else
				{
					base.CallContext.ProtocolLog.Set(SynchronizeWacAttachmentMetadata.Count, this.checks);
					string primarySmtpAddress = base.CallContext.MailboxIdentityPrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
					string cacheKey = CachedAttachmentInfo.GetCacheKey(primarySmtpAddress, this.attachmentId);
					CachedAttachmentInfo fromCache = CachedAttachmentInfo.GetFromCache(cacheKey);
					if (fromCache == null)
					{
						base.CallContext.ProtocolLog.Set(SynchronizeWacAttachmentMetadata.Result, SynchronizeWacAttachment.AttachmentState.NoCachedInfo);
						result = true;
					}
					else
					{
						base.CallContext.ProtocolLog.Set(SynchronizeWacAttachmentMetadata.SessionId, fromCache.SessionId);
						if (fromCache.CobaltStore == null)
						{
							if (fromCache.LockCount == 0)
							{
								base.CallContext.ProtocolLog.Set(SynchronizeWacAttachmentMetadata.Result, SynchronizeWacAttachment.AttachmentState.NoLocks);
								result = true;
							}
							else
							{
								base.CallContext.ProtocolLog.Set(SynchronizeWacAttachmentMetadata.Result, SynchronizeWacAttachment.AttachmentState.HasLocks);
								result = false;
							}
						}
						else if (fromCache.CobaltStore.EditorCount == 0)
						{
							base.CallContext.ProtocolLog.Set(SynchronizeWacAttachmentMetadata.Result, SynchronizeWacAttachment.AttachmentState.NoEditors);
							result = true;
						}
						else
						{
							base.CallContext.ProtocolLog.Set(SynchronizeWacAttachmentMetadata.Result, SynchronizeWacAttachment.AttachmentState.HasEditors);
							result = false;
						}
					}
				}
			}
			return result;
		}

		private const string ActionName = "SynchronizeWacAttachment";

		private const int maxChecks = 10;

		private readonly TimeSpan retryInterval = TimeSpan.FromSeconds(1.0);

		private readonly string attachmentId;

		private readonly ServiceAsyncResult<SynchronizeWacAttachmentResponse> asyncResult;

		private Timer timer;

		private int checks;

		public enum AttachmentState
		{
			NoEditors,
			NoCachedInfo,
			NoLocks,
			HasLocks,
			HasEditors
		}
	}
}
