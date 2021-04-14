using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.UnifiedContent.Exchange;
using Microsoft.Filtering;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class ScanResultStorageProvider : IExtendedMapiFilteringContext, IMapiFilteringContext
	{
		protected Item StoreItem { get; set; }

		protected ScanResultStorageProvider(Item storeItem)
		{
			if (storeItem == null)
			{
				throw new ArgumentNullException("storeItem");
			}
			this.StoreItem = storeItem;
		}

		public abstract IEnumerable<DiscoveredDataClassification> GetDlpDetectedClassificationObjects();

		public abstract void SetDlpDetectedClassificationObjects(IEnumerable<DiscoveredDataClassification> dlpDetectedClassificationObjects);

		public abstract void ResetDlpDetectedClassificationObjects();

		public abstract void SetDlpDetectedClassifications(string dcIds);

		public abstract void ResetDlpDetectedClassifications();

		public abstract void SetHasDlpDetectedClassifications();

		public abstract void ResetHasDlpDetectedClassifications(bool alsoInAttachments = false);

		public abstract bool NeedsClassificationScan();

		public abstract bool NeedsClassificationScan(Attachment attachment);

		public abstract void SetFipsRecoveryOptions(RecoveryOptions options);

		public abstract RecoveryOptions GetFipsRecoveryOptions();

		public virtual bool NeedsClassificationForBodyOrAnyAttachments()
		{
			if (this.NeedsClassificationScan())
			{
				return true;
			}
			AttachmentCollection attachmentCollection = this.StoreItem.AttachmentCollection;
			if (attachmentCollection != null && attachmentCollection.Count > 0)
			{
				foreach (AttachmentHandle handle in attachmentCollection)
				{
					using (Attachment attachment = this.StoreItem.AttachmentCollection.Open(handle))
					{
						if (attachment != null && !ScanResultStorageProvider.IsExcludedFromDlp(attachment) && this.NeedsClassificationScan(attachment))
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		public virtual void ResetAllClassifications()
		{
			this.ResetHasDlpDetectedClassifications(true);
			this.ResetDlpDetectedClassifications();
			this.ResetDlpDetectedClassificationObjects();
		}

		public virtual void RefreshBodyClassifications()
		{
			this.ResetHasDlpDetectedClassifications(false);
			List<string> list = new List<string>();
			list.Add(ScanResultStorageProvider.MessageBodyName);
			IEnumerable<DiscoveredDataClassification> dlpDetectedClassificationObjects = FipsResultParser.DeleteClassifications(this.GetDlpDetectedClassificationObjects(), list, false);
			this.SetDlpDetectedClassificationObjects(dlpDetectedClassificationObjects);
		}

		public virtual void RefreshAttachmentClassifications()
		{
			List<string> list = new List<string>();
			AttachmentCollection attachmentCollection = this.StoreItem.AttachmentCollection;
			if (attachmentCollection != null && attachmentCollection.Count > 0)
			{
				foreach (AttachmentHandle handle in attachmentCollection)
				{
					using (Attachment attachment = this.StoreItem.AttachmentCollection.Open(handle))
					{
						if (!ScanResultStorageProvider.IsExcludedFromDlp(attachment))
						{
							list.Add(string.Format(ScanResultStorageProvider.UniqueIdFormat, attachment.FileName, attachment.Id.ToBase64String()));
						}
					}
				}
			}
			list.Add(ScanResultStorageProvider.MessageBodyName);
			IEnumerable<DiscoveredDataClassification> dlpDetectedClassificationObjects = FipsResultParser.DeleteClassifications(this.GetDlpDetectedClassificationObjects(), list, true);
			this.SetDlpDetectedClassificationObjects(dlpDetectedClassificationObjects);
		}

		public static bool IsExcludedFromDlp(Attachment attachment)
		{
			return attachment == null || string.IsNullOrEmpty(attachment.FileName);
		}

		public const char UniqueIdFormatSeparatorChar = ':';

		public const string UniqueIdFormatSeparator = ":";

		public static readonly string UniqueIdFormat = string.Join<string>(":", new string[]
		{
			"{0}",
			"{1}"
		});

		public static readonly string MessageBodyName = "Message Body";
	}
}
