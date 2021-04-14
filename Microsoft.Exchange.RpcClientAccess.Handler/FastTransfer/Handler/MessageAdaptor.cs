using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Handler;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MessageAdaptor : BaseObject, IMessage, IDisposable, WatsonHelper.IProvideWatsonReportData
	{
		internal MessageAdaptor(ReferenceCount<CoreItem> referenceCoreItem, MessageAdaptor.Options options, Encoding string8Encoding, bool wantUnicode, Logon logon = null)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.options = options;
				this.string8Encoding = string8Encoding;
				this.wantUnicode = wantUnicode;
				this.logon = logon;
				if (!this.options.IsEmbedded && Activity.Current != null)
				{
					this.watsonReportActionGuard = Activity.Current.RegisterWatsonReportDataProviderAndGetGuard(WatsonReportActionType.MessageAdaptor, this);
				}
				this.referenceCoreItem = referenceCoreItem;
				this.referenceCoreItem.AddRef();
				this.bestBodyCoreObjectProperties = new BestBodyCoreObjectProperties(this.referenceCoreItem.ReferencedObject, this.referenceCoreItem.ReferencedObject.PropertyBag, this.string8Encoding, new Func<BodyReadConfiguration, System.IO.Stream>(this.GetBodyConversionStreamCallback));
				disposeGuard.Success();
			}
		}

		internal MessageAdaptor(BestBodyCoreObjectProperties bestBodyCoreObjectProperties, ReferenceCount<CoreItem> referenceCoreItem, MessageAdaptor.Options options, Encoding string8Encoding, bool wantUnicode, Logon logon = null)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.options = options;
				this.string8Encoding = string8Encoding;
				this.wantUnicode = wantUnicode;
				this.logon = logon;
				if (!this.options.IsEmbedded && Activity.Current != null)
				{
					this.watsonReportActionGuard = Activity.Current.RegisterWatsonReportDataProviderAndGetGuard(WatsonReportActionType.MessageAdaptor, this);
				}
				this.referenceCoreItem = referenceCoreItem;
				this.referenceCoreItem.AddRef();
				this.bestBodyCoreObjectProperties = bestBodyCoreObjectProperties;
				disposeGuard.Success();
			}
		}

		public IPropertyBag PropertyBag
		{
			get
			{
				base.CheckDisposed();
				if (this.propertyBag == null)
				{
					this.propertyBag = new CoreItemPropertyBag(new CorePropertyBagAdaptor(this.bestBodyCoreObjectProperties, this.referenceCoreItem.ReferencedObject.PropertyBag, this.referenceCoreItem.ReferencedObject, ClientSideProperties.MessageInstance, PropertyConverter.Message, this.options.DownloadBodyOption, this.string8Encoding, this.wantUnicode, this.options.IsUpload, this.options.IsFastTransferCopyProperties), this.options.SendEntryId);
				}
				return this.propertyBag;
			}
		}

		public ReferenceCount<CoreItem> ReferenceCoreItem
		{
			get
			{
				base.CheckDisposed();
				return this.referenceCoreItem;
			}
		}

		public bool IsAssociated
		{
			get
			{
				base.CheckDisposed();
				return MessageAdaptor.IsAssociatedMessage(this);
			}
		}

		public static bool IsAssociatedMessage(IMessage message)
		{
			AnnotatedPropertyValue annotatedProperty = message.PropertyBag.GetAnnotatedProperty(PropertyTag.MessageFlags);
			return !annotatedProperty.PropertyValue.IsError && ((int)annotatedProperty.PropertyValue.Value & 64) == 64;
		}

		public IEnumerable<IRecipient> GetRecipients()
		{
			base.CheckDisposed();
			if (!this.options.IsReadOnly)
			{
				throw new InvalidOperationException("Cannot iterate through recipients unless it is a readonly message.");
			}
			IEnumerable<CoreRecipient> recipients = this.referenceCoreItem.ReferencedObject.Recipients;
			if (!this.options.IsEmbedded)
			{
				recipients = recipients.ToArray<CoreRecipient>();
			}
			foreach (CoreRecipient recipient in recipients)
			{
				yield return new RecipientAdaptor(recipient, this.referenceCoreItem.ReferencedObject, this.string8Encoding, this.wantUnicode);
			}
			yield break;
		}

		public IRecipient CreateRecipient()
		{
			base.CheckDisposed();
			if (this.options.IsReadOnly)
			{
				throw new InvalidOperationException("Cannot CreateRecipient on readonly messages.");
			}
			int count = this.referenceCoreItem.ReferencedObject.Recipients.Count;
			CoreRecipient coreRecipient = this.referenceCoreItem.ReferencedObject.Recipients.CreateOrReplace(count);
			return new RecipientAdaptor(coreRecipient, this.referenceCoreItem.ReferencedObject, this.string8Encoding, this.wantUnicode);
		}

		public void RemoveRecipient(int rowId)
		{
			this.referenceCoreItem.ReferencedObject.Recipients.Remove(rowId);
		}

		public IEnumerable<IAttachmentHandle> GetAttachments()
		{
			base.CheckDisposed();
			if (!this.options.IsReadOnly)
			{
				throw new InvalidOperationException("Cannot iterate through attachments unless it is a readonly message.");
			}
			CoreAttachmentCollection coreAttachmentCollection = this.referenceCoreItem.ReferencedObject.AttachmentCollection;
			foreach (AttachmentHandle attachmentHandle in coreAttachmentCollection)
			{
				yield return new MessageAdaptor.AttachmentHandleAdaptor(coreAttachmentCollection, attachmentHandle, this.string8Encoding, this.wantUnicode, this.options.IsUpload);
			}
			yield break;
		}

		public IAttachment CreateAttachment()
		{
			base.CheckDisposed();
			if (this.options.IsReadOnly)
			{
				throw new InvalidOperationException("Cannot CreateAttachment on readonly messages.");
			}
			IAttachment result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CoreAttachment coreAttachment = this.referenceCoreItem.ReferencedObject.AttachmentCollection.Create(AttachmentType.Stream);
				coreAttachment.PropertyBag.Load(CoreObjectSchema.AllPropertiesOnStore);
				this.referenceCoreItem.ReferencedObject.PropertyBag[CoreItemSchema.MapiHasAttachment] = true;
				disposeGuard.Add<CoreAttachment>(coreAttachment);
				ReferenceCount<CoreAttachment> referenceCount = new ReferenceCount<CoreAttachment>(coreAttachment);
				try
				{
					AttachmentAdaptor attachmentAdaptor = new AttachmentAdaptor(referenceCount, false, this.string8Encoding, this.wantUnicode, false);
					disposeGuard.Success();
					result = attachmentAdaptor;
				}
				finally
				{
					referenceCount.Release();
				}
			}
			return result;
		}

		public void Save()
		{
			base.CheckDisposed();
			this.referenceCoreItem.ReferencedObject.SaveFlags |= (PropertyBagSaveFlags.IgnoreMapiComputedErrors | PropertyBagSaveFlags.IgnoreAccessDeniedErrors);
			CoreItem referencedObject = this.referenceCoreItem.ReferencedObject;
			this.bestBodyCoreObjectProperties.ResetBody();
			bool flag = false;
			if (this.logon != null && !this.options.IsEmbedded)
			{
				try
				{
					flag = TeamMailboxExecutionHelper.SaveChangesToLinkedDocumentLibraryIfNecessary(referencedObject, this.logon);
				}
				catch (StoragePermanentException e)
				{
					TeamMailboxExecutionHelper.LogServerFailures(referencedObject, this.logon, e);
					throw;
				}
				catch (StorageTransientException e2)
				{
					TeamMailboxExecutionHelper.LogServerFailures(referencedObject, this.logon, e2);
					throw;
				}
			}
			if (flag)
			{
				((MailboxSession)referencedObject.Session).TryToSyncSiteMailboxNow();
				return;
			}
			IContentIndexingSession contentIndexingSession = referencedObject.Session.ContentIndexingSession;
			if (contentIndexingSession != null)
			{
				contentIndexingSession.EnableWordBreak = true;
			}
			try
			{
				referencedObject.Body.ResetBodyFormat();
				ConflictResolutionResult conflictResolutionResult = referencedObject.Save(SaveMode.ResolveConflicts);
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					Feature.Stubbed(65889, "Handle message conflicts in FX Upload");
					throw new RopExecutionException("Failed to save message due to conflicts.", (ErrorCode)2147746057U);
				}
			}
			finally
			{
				if (contentIndexingSession != null)
				{
					contentIndexingSession.EnableWordBreak = false;
				}
			}
		}

		public void SetLongTermId(StoreLongTermId longTermId)
		{
			CoreItem coreItem = (this.referenceCoreItem == null) ? null : this.referenceCoreItem.ReferencedObject;
			if (coreItem == null || coreItem.Session == null || !coreItem.Session.IsMoveUser)
			{
				throw new RopExecutionException("Should not be called in MoMT scenarios.", (ErrorCode)2147746050U);
			}
			coreItem.PropertyBag.SetProperty(MessageItemSchema.LTID, longTermId.ToBytes());
			coreItem.Flush(SaveMode.NoConflictResolution);
		}

		string WatsonHelper.IProvideWatsonReportData.GetWatsonReportString()
		{
			base.CheckDisposed();
			return string.Format("Subject: {0}\r\nReceive time: {1}", this.referenceCoreItem.ReferencedObject.PropertyBag.GetValueOrDefault<string>(CoreItemSchema.Subject, string.Empty), this.referenceCoreItem.ReferencedObject.PropertyBag.GetValueOrDefault<object>(CoreItemSchema.ReceivedTime));
		}

		protected override void InternalDispose()
		{
			this.bestBodyCoreObjectProperties.ResetBody();
			this.referenceCoreItem.Release();
			base.InternalDispose();
			Util.DisposeIfPresent(this.watsonReportActionGuard);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MessageAdaptor>(this);
		}

		private System.IO.Stream GetBodyConversionStreamCallback(BodyReadConfiguration readConfiguration)
		{
			return this.referenceCoreItem.ReferencedObject.Body.OpenReadStream(readConfiguration);
		}

		private const int AssociatedFlag = 64;

		private readonly IDisposable watsonReportActionGuard;

		private readonly MessageAdaptor.Options options;

		private readonly Encoding string8Encoding;

		private readonly bool wantUnicode;

		private readonly Logon logon;

		private readonly ReferenceCount<CoreItem> referenceCoreItem;

		private readonly BestBodyCoreObjectProperties bestBodyCoreObjectProperties;

		private IPropertyBag propertyBag;

		internal struct Options
		{
			public bool SkipMessagesInConflict
			{
				get
				{
					return this.SendEntryId;
				}
			}

			public bool IsReadOnly;

			public bool IsEmbedded;

			public bool SendEntryId;

			public DownloadBodyOption DownloadBodyOption;

			public bool IsUpload;

			public bool IsFastTransferCopyProperties;
		}

		private sealed class AttachmentHandleAdaptor : IAttachmentHandle
		{
			public AttachmentHandleAdaptor(CoreAttachmentCollection attachmentCollection, AttachmentHandle attachmentHandle, Encoding string8Encoding, bool wantUnicode, bool isUpload)
			{
				this.attachmentCollection = attachmentCollection;
				this.attachmentHandle = attachmentHandle;
				this.string8Encoding = string8Encoding;
				this.wantUnicode = wantUnicode;
				this.isUpload = isUpload;
			}

			public IAttachment GetAttachment()
			{
				IAttachment result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					CoreAttachment coreAttachment = this.attachmentCollection.Open(this.attachmentHandle, CoreObjectSchema.AllPropertiesOnStore);
					disposeGuard.Add<CoreAttachment>(coreAttachment);
					ReferenceCount<CoreAttachment> referenceCount = new ReferenceCount<CoreAttachment>(coreAttachment);
					try
					{
						AttachmentAdaptor attachmentAdaptor = new AttachmentAdaptor(referenceCount, true, this.string8Encoding, this.wantUnicode, this.isUpload);
						disposeGuard.Success();
						result = attachmentAdaptor;
					}
					finally
					{
						referenceCount.Release();
					}
				}
				return result;
			}

			private readonly CoreAttachmentCollection attachmentCollection;

			private readonly AttachmentHandle attachmentHandle;

			private readonly Encoding string8Encoding;

			private readonly bool wantUnicode;

			private readonly bool isUpload;
		}
	}
}
