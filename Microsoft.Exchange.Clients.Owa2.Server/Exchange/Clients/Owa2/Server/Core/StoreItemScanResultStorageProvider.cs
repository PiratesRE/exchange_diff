using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Filtering;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class StoreItemScanResultStorageProvider : ScanResultStorageProvider
	{
		public StoreItemScanResultStorageProvider(Item storeItem) : base(storeItem)
		{
		}

		public override IEnumerable<DiscoveredDataClassification> GetDlpDetectedClassificationObjects()
		{
			string dlpDetectedClassificationObjectsAsString = this.GetDlpDetectedClassificationObjectsAsString();
			return DiscoveredDataClassification.DeserializeFromXml(dlpDetectedClassificationObjectsAsString);
		}

		private string GetDlpDetectedClassificationObjectsAsString()
		{
			try
			{
				using (Stream stream = base.StoreItem.OpenPropertyStream(ItemSchema.DlpDetectedClassificationObjects, PropertyOpenMode.ReadOnly))
				{
					UnicodeEncoding encoding = new UnicodeEncoding();
					using (StreamReader streamReader = new StreamReader(stream, encoding))
					{
						return streamReader.ReadToEnd();
					}
				}
			}
			catch (ObjectNotFoundException)
			{
			}
			return null;
		}

		public override void SetDlpDetectedClassificationObjects(IEnumerable<DiscoveredDataClassification> dlpDetectedClassificationObjects)
		{
			string text = DiscoveredDataClassification.SerializeToXml(dlpDetectedClassificationObjects);
			if (string.IsNullOrEmpty(text))
			{
				base.StoreItem.SetOrDeleteProperty(ItemSchema.DlpDetectedClassificationObjects, null);
				return;
			}
			UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
			base.StoreItem[ItemSchema.DlpDetectedClassificationObjects] = unicodeEncoding.GetBytes(text);
		}

		public override void ResetDlpDetectedClassificationObjects()
		{
			base.StoreItem.SetOrDeleteProperty(ItemSchema.DlpDetectedClassificationObjects, null);
		}

		public override void SetDlpDetectedClassifications(string dcIds)
		{
			if (dcIds == null)
			{
				dcIds = string.Empty;
			}
			base.StoreItem[ItemSchema.DlpDetectedClassifications] = dcIds;
		}

		public override void ResetDlpDetectedClassifications()
		{
			base.StoreItem.Delete(ItemSchema.DlpDetectedClassifications);
		}

		public override void SetHasDlpDetectedClassifications()
		{
			base.StoreItem[ItemSchema.HasDlpDetectedClassifications] = string.Empty;
			AttachmentCollection attachmentCollection = base.StoreItem.AttachmentCollection;
			if (attachmentCollection != null)
			{
				foreach (AttachmentHandle handle in attachmentCollection)
				{
					using (Attachment attachment = base.StoreItem.AttachmentCollection.Open(handle))
					{
						if (attachment != null && !ScanResultStorageProvider.IsExcludedFromDlp(attachment))
						{
							attachment[AttachmentSchema.HasDlpDetectedClassifications] = string.Empty;
							attachment.Save();
						}
					}
				}
			}
		}

		public override void ResetHasDlpDetectedClassifications(bool alsoInAttachments = false)
		{
			base.StoreItem.Delete(ItemSchema.HasDlpDetectedClassifications);
			if (alsoInAttachments)
			{
				AttachmentCollection attachmentCollection = base.StoreItem.AttachmentCollection;
				if (attachmentCollection != null)
				{
					foreach (AttachmentHandle handle in attachmentCollection)
					{
						using (Attachment attachment = base.StoreItem.AttachmentCollection.Open(handle))
						{
							StoreItemScanResultStorageProvider.ResetHasDlpDetectedClassifications(attachment);
						}
					}
				}
			}
		}

		private static void ResetHasDlpDetectedClassifications(Attachment attachment)
		{
			if (attachment != null && !ScanResultStorageProvider.IsExcludedFromDlp(attachment))
			{
				attachment.Delete(AttachmentSchema.HasDlpDetectedClassifications);
				attachment.Save();
			}
		}

		public override bool NeedsClassificationScan()
		{
			string valueOrDefault = base.StoreItem.PropertyBag.GetValueOrDefault<string>(ItemSchema.HasDlpDetectedClassifications, null);
			return valueOrDefault == null;
		}

		public override bool NeedsClassificationScan(Attachment attachment)
		{
			if (attachment == null)
			{
				return false;
			}
			string valueOrDefault = attachment.PropertyBag.GetValueOrDefault<string>(AttachmentSchema.HasDlpDetectedClassifications, null);
			return valueOrDefault == null;
		}

		public override void SetFipsRecoveryOptions(RecoveryOptions options)
		{
			base.StoreItem[ItemSchema.RecoveryOptions] = (int)options;
		}

		public override RecoveryOptions GetFipsRecoveryOptions()
		{
			return (RecoveryOptions)base.StoreItem.PropertyBag.GetValueOrDefault<int>(ItemSchema.RecoveryOptions, 0);
		}
	}
}
