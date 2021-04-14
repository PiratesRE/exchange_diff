using System;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Handler;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal class MessageIteratorClient : ServerObject, IMessageIteratorClient, IDisposable, WatsonHelper.IProvideWatsonReportData
	{
		internal MessageIteratorClient(CoreFolder folder)
		{
			this.session = folder.Session;
			if (!this.session.IsMoveUser)
			{
				throw new ArgumentException("MessageIteratorClient", "IsMoveUser");
			}
			this.folderId = folder.Id.ObjectId;
			this.logonString8Encoding = Encoding.ASCII;
			this.useNullLogon = true;
		}

		internal MessageIteratorClient(Folder folder, Logon logon) : base(logon)
		{
			this.session = folder.Session;
			this.folderId = folder.CoreFolder.Id.ObjectId;
			this.logonString8Encoding = base.LogonObject.LogonString8Encoding;
			this.useNullLogon = false;
		}

		public IMessage UploadMessage(bool isAssociatedMessage)
		{
			IMessage result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CoreItem coreItem = CoreItem.Create(this.session, this.folderId, isAssociatedMessage ? CreateMessageType.Associated : CreateMessageType.Normal);
				disposeGuard.Add<CoreItem>(coreItem);
				ReferenceCount<CoreItem> referenceCount = new ReferenceCount<CoreItem>(coreItem);
				try
				{
					MessageAdaptor messageAdaptor = new MessageAdaptor(referenceCount, new MessageAdaptor.Options
					{
						IsReadOnly = false,
						IsEmbedded = false,
						DownloadBodyOption = DownloadBodyOption.AllBodyProperties,
						IsUpload = false
					}, this.logonString8Encoding, true, this.useNullLogon ? null : base.LogonObject);
					disposeGuard.Success();
					result = messageAdaptor;
				}
				finally
				{
					referenceCount.Release();
				}
			}
			return result;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MessageIteratorClient>(this);
		}

		string WatsonHelper.IProvideWatsonReportData.GetWatsonReportString()
		{
			base.CheckDisposed();
			return string.Format("MessageIteratorClient: Last Message = \"{0}\".", "<No message has been uploaded yet>");
		}

		private const string LastMessage = "<No message has been uploaded yet>";

		private readonly StoreSession session;

		private readonly StoreObjectId folderId;

		private readonly Encoding logonString8Encoding;

		private readonly bool useNullLogon;
	}
}
