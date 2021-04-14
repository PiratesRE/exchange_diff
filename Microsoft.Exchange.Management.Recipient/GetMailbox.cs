using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[OutputType(new Type[]
	{
		typeof(Mailbox)
	})]
	[Cmdlet("Get", "Mailbox", DefaultParameterSetName = "Identity")]
	public sealed class GetMailbox : GetMailboxOrSyncMailbox
	{
		[Parameter(Mandatory = false)]
		public new long UsnForReconciliationSearch
		{
			get
			{
				return base.UsnForReconciliationSearch;
			}
			set
			{
				base.UsnForReconciliationSearch = value;
			}
		}

		protected override bool IsRetryableTask
		{
			get
			{
				return true;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			IConfigurable configurable = base.ConvertDataObjectToPresentationObject(dataObject);
			Mailbox mailbox = configurable as Mailbox;
			if (mailbox == null)
			{
				return null;
			}
			TenantPublicFolderConfiguration value = TenantPublicFolderConfigurationCache.Instance.GetValue(mailbox.OrganizationId);
			bool flag = mailbox.RecipientTypeDetails == Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.PublicFolderMailbox;
			if (!flag)
			{
				if (mailbox.DefaultPublicFolderMailboxValue == null || mailbox.DefaultPublicFolderMailboxValue.IsDeleted)
				{
					PublicFolderRecipient publicFolderRecipient = value.GetPublicFolderRecipient(mailbox.ExchangeGuid, null);
					if (publicFolderRecipient != null)
					{
						if (base.NeedSuppressingPiiData)
						{
							string text;
							string text2;
							mailbox.DefaultPublicFolderMailbox = SuppressingPiiData.Redact(publicFolderRecipient.ObjectId, out text, out text2);
						}
						else
						{
							mailbox.DefaultPublicFolderMailbox = publicFolderRecipient.ObjectId;
						}
					}
				}
				else
				{
					mailbox.DefaultPublicFolderMailbox = mailbox.DefaultPublicFolderMailboxValue;
				}
			}
			mailbox.IsRootPublicFolderMailbox = (flag && value.GetHierarchyMailboxInformation().HierarchyMailboxGuid == mailbox.ExchangeGuid);
			if (this.UsnForReconciliationSearch >= 0L)
			{
				mailbox.ReconciliationId = mailbox.NetID;
			}
			mailbox.ResetChangeTracking();
			return mailbox;
		}

		private bool DoesIdentityContainWildCard()
		{
			string text = (this.Identity == null) ? null : this.Identity.ToString();
			return !string.IsNullOrEmpty(text) && (text.StartsWith("*") || text.EndsWith("*"));
		}
	}
}
