using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class CancelAttachmentManager
	{
		public CancelAttachmentManager(UserContext userContext)
		{
			this.lockObject = new object();
			this.userContext = userContext;
		}

		private Dictionary<string, CancelAttachmentManager.CancellationItem> CancellationItems
		{
			get
			{
				if (this.cancellationItems == null)
				{
					this.cancellationItems = new Dictionary<string, CancelAttachmentManager.CancellationItem>();
				}
				return this.cancellationItems;
			}
		}

		public bool OnCreateAttachment(string cancellationId, CancellationTokenSource tokenSource)
		{
			bool result = false;
			lock (this.lockObject)
			{
				if (this.CancellationItems.ContainsKey(cancellationId) && this.CancellationItems[cancellationId].IsCancellationRequested)
				{
					OwaServerTraceLogger.AppendToLog(new TraceLogEvent("CancelAttachmentManager.OnCreateAttachment", this.userContext, "OnCreateAttachment", string.Format("Attachment was cancelled before CreateAttachment, CancellationId: {0}.", cancellationId)));
					result = true;
				}
				else
				{
					CancelAttachmentManager.CancellationItem cancellationItem = new CancelAttachmentManager.CancellationItem();
					cancellationItem.CancellationId = cancellationId;
					cancellationItem.CancellationTokenSource = tokenSource;
					cancellationItem.AttachmentCompletedEvent = new ManualResetEvent(false);
					this.CancellationItems[cancellationId] = cancellationItem;
				}
			}
			return result;
		}

		public void CreateAttachmentCancelled(string cancellationId)
		{
			OwaServerTraceLogger.AppendToLog(new TraceLogEvent("CancelAttachmentManager.CreateAttachmentCancelled", this.userContext, "CreateAttachmentCancelled", string.Format("CreateAttachment cancelled for CancellationId: {0}.", cancellationId)));
			if (cancellationId == null)
			{
				return;
			}
			this.CancellationItems[cancellationId].AttachmentCompletedEvent.Set();
		}

		public void CreateAttachmentCompleted(string cancellationId, AttachmentIdType attachmentId)
		{
			OwaServerTraceLogger.AppendToLog(new TraceLogEvent("CancelAttachmentManager.CreateAttachmentCompleted", this.userContext, "CreateAttachmentCompleted", string.Format("CreateAttachment completed for AttachmentId: {0}, CancellationId: {1}.", (attachmentId == null) ? "Null" : attachmentId.Id, string.IsNullOrEmpty(cancellationId) ? "Null" : cancellationId)));
			if (cancellationId == null)
			{
				return;
			}
			this.CancellationItems[cancellationId].AttachmentId = attachmentId;
			this.CancellationItems[cancellationId].AttachmentCompletedEvent.Set();
		}

		public bool CancelAttachment(string cancellationId, int timeout)
		{
			CancelAttachmentManager.CancellationItem cancellationItem;
			lock (this.lockObject)
			{
				if (!this.CancellationItems.ContainsKey(cancellationId))
				{
					OwaServerTraceLogger.AppendToLog(new TraceLogEvent("CancelAttachmentManager.CancelAttachment", this.userContext, "CancelAttachment", string.Format("CreateAttachment has not started, marking item as cancelled for CancellationId: {0}", cancellationId)));
					cancellationItem = new CancelAttachmentManager.CancellationItem();
					cancellationItem.CancellationId = cancellationId;
					cancellationItem.IsCancellationRequested = true;
					this.cancellationItems.Add(cancellationId, cancellationItem);
					return true;
				}
				cancellationItem = this.CancellationItems[cancellationId];
				cancellationItem.IsCancellationRequested = true;
				if (cancellationItem.CancellationTokenSource != null)
				{
					cancellationItem.CancellationTokenSource.Cancel();
				}
			}
			if (cancellationItem.AttachmentId == null && cancellationItem.AttachmentCompletedEvent != null && !cancellationItem.AttachmentCompletedEvent.WaitOne(timeout))
			{
				OwaServerTraceLogger.AppendToLog(new TraceLogEvent("CancelAttachmentManager.CancelAttachment", this.userContext, "CancelAttachment", string.Format("Cancel attachment timed out for CancellationId: {0}", cancellationId)));
				return false;
			}
			if (cancellationItem.AttachmentId == null)
			{
				OwaServerTraceLogger.AppendToLog(new TraceLogEvent("CancelAttachmentManager.CancelAttachment", this.userContext, "CancelAttachment", string.Format("AttachmentId not found. Returning success for cancellationId: {0}", cancellationId)));
				return true;
			}
			bool flag2 = AttachmentUtilities.DeleteAttachment(cancellationItem.AttachmentId);
			OwaServerTraceLogger.AppendToLog(new TraceLogEvent("CancelAttachmentManager.CancelAttachment", this.userContext, "CancelAttachment", string.Format("DeleteAttachmentSucceeded = {0} for CancellationId: {1}, AttachmentId: {2}", flag2, cancellationId, cancellationItem.AttachmentId.Id)));
			return flag2;
		}

		private Dictionary<string, CancelAttachmentManager.CancellationItem> cancellationItems;

		private object lockObject;

		private UserContext userContext;

		private class CancellationItem
		{
			internal string CancellationId;

			internal bool IsCancellationRequested;

			internal AttachmentIdType AttachmentId;

			internal ManualResetEvent AttachmentCompletedEvent;

			internal CancellationTokenSource CancellationTokenSource;
		}
	}
}
