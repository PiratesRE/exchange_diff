using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Filtering;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class ClientScanResultStorageProvider : ScanResultStorageProvider
	{
		public ClientScanResultStorageProvider(string clientData, Item storeItem) : base(storeItem)
		{
			this.clientScanResultStorage = ClientScanResultStorage.CreateInstance(clientData);
		}

		public override IEnumerable<DiscoveredDataClassification> GetDlpDetectedClassificationObjects()
		{
			return this.clientScanResultStorage.DlpDetectedClassificationObjects;
		}

		public override void SetDlpDetectedClassificationObjects(IEnumerable<DiscoveredDataClassification> dlpDetectedClassificationObjects)
		{
			if (dlpDetectedClassificationObjects == null)
			{
				this.ResetDlpDetectedClassificationObjects();
				return;
			}
			this.clientScanResultStorage.DlpDetectedClassificationObjects = dlpDetectedClassificationObjects.ToList<DiscoveredDataClassification>();
		}

		public override void ResetDlpDetectedClassificationObjects()
		{
			this.clientScanResultStorage.DlpDetectedClassificationObjects = new List<DiscoveredDataClassification>();
		}

		public override void SetDlpDetectedClassifications(string dcIds)
		{
			if (dcIds == null)
			{
				this.ResetDlpDetectedClassifications();
				return;
			}
			this.clientScanResultStorage.DetectedClassificationIds = dcIds;
		}

		public override void ResetDlpDetectedClassifications()
		{
			this.clientScanResultStorage.DetectedClassificationIds = string.Empty;
		}

		public override void SetHasDlpDetectedClassifications()
		{
			this.ResetClassifiedParts();
			this.clientScanResultStorage.ClassifiedParts.Add(ScanResultStorageProvider.MessageBodyName);
			AttachmentCollection attachmentCollection = base.StoreItem.AttachmentCollection;
			if (attachmentCollection != null)
			{
				foreach (AttachmentHandle handle in attachmentCollection)
				{
					using (Attachment attachment = base.StoreItem.AttachmentCollection.Open(handle))
					{
						if (attachment != null && !ScanResultStorageProvider.IsExcludedFromDlp(attachment))
						{
							this.clientScanResultStorage.ClassifiedParts.Add(string.Format(ScanResultStorageProvider.UniqueIdFormat, attachment.FileName, attachment.Id.ToBase64String()));
						}
					}
				}
			}
		}

		private void ResetClassifiedParts()
		{
			this.clientScanResultStorage.ClassifiedParts = new List<string>();
		}

		public override void ResetHasDlpDetectedClassifications(bool alsoInAttachments = false)
		{
			if (alsoInAttachments)
			{
				this.ResetClassifiedParts();
				return;
			}
			this.clientScanResultStorage.ClassifiedParts.RemoveAll((string o) => o.Equals(ScanResultStorageProvider.MessageBodyName, StringComparison.OrdinalIgnoreCase));
		}

		public override bool NeedsClassificationScan()
		{
			return !this.clientScanResultStorage.ClassifiedParts.Exists((string s) => string.Equals(s, ScanResultStorageProvider.MessageBodyName, StringComparison.OrdinalIgnoreCase));
		}

		public override bool NeedsClassificationScan(Attachment attachment)
		{
			return attachment != null && !this.clientScanResultStorage.ClassifiedParts.Exists((string s) => string.Equals(s, string.Format(ScanResultStorageProvider.UniqueIdFormat, attachment.FileName, attachment.Id.ToBase64String()), StringComparison.OrdinalIgnoreCase));
		}

		public override void SetFipsRecoveryOptions(RecoveryOptions options)
		{
			this.clientScanResultStorage.RecoveryOptions = (int)options;
		}

		public override RecoveryOptions GetFipsRecoveryOptions()
		{
			return (RecoveryOptions)this.clientScanResultStorage.RecoveryOptions;
		}

		public string GetScanResultData()
		{
			return this.clientScanResultStorage.ToClientString();
		}

		public string GetDetectedClassificationIds()
		{
			return this.clientScanResultStorage.DetectedClassificationIds;
		}

		internal ClientScanResultStorage GetClientScanResultStorageForTesting()
		{
			return this.clientScanResultStorage;
		}

		private ClientScanResultStorage clientScanResultStorage;
	}
}
