using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Management.Dsn
{
	[Cmdlet("Set", "SystemMessage", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetSystemMessage : SetSystemConfigurationObjectTask<SystemMessageIdParameter, SystemMessage>
	{
		internal static QuotaMessageType ConvertToInternal(QuotaMessageType quotaType)
		{
			switch (quotaType)
			{
			case QuotaMessageType.WarningMailboxUnlimitedSize:
				return QuotaMessageType.WarningMailboxUnlimitedSize;
			case QuotaMessageType.WarningPublicFolderUnlimitedSize:
				return QuotaMessageType.WarningPublicFolderUnlimitedSize;
			case QuotaMessageType.WarningMailbox:
				return QuotaMessageType.WarningMailbox;
			case QuotaMessageType.WarningPublicFolder:
				return QuotaMessageType.WarningPublicFolder;
			case QuotaMessageType.ProhibitSendMailbox:
				return QuotaMessageType.ProhibitSendMailbox;
			case QuotaMessageType.ProhibitPostPublicFolder:
				return QuotaMessageType.ProhibitPostPublicFolder;
			case QuotaMessageType.ProhibitSendReceiveMailBox:
				return QuotaMessageType.ProhibitSendReceiveMailBox;
			case QuotaMessageType.WarningMailboxMessagesPerFolderCount:
				return QuotaMessageType.WarningMailboxMessagesPerFolderCount;
			case QuotaMessageType.ProhibitReceiveMailboxMessagesPerFolderCount:
				return QuotaMessageType.ProhibitReceiveMailboxMessagesPerFolderCount;
			case QuotaMessageType.WarningFolderHierarchyChildrenCount:
				return QuotaMessageType.WarningFolderHierarchyChildrenCount;
			case QuotaMessageType.ProhibitReceiveFolderHierarchyChildrenCountCount:
				return QuotaMessageType.ProhibitReceiveFolderHierarchyChildrenCountCount;
			case QuotaMessageType.WarningMailboxMessagesPerFolderUnlimitedCount:
				return QuotaMessageType.WarningMailboxMessagesPerFolderUnlimitedCount;
			case QuotaMessageType.WarningFolderHierarchyChildrenUnlimitedCount:
				return QuotaMessageType.WarningFolderHierarchyChildrenUnlimitedCount;
			case QuotaMessageType.WarningFolderHierarchyDepth:
				return QuotaMessageType.WarningFolderHierarchyDepth;
			case QuotaMessageType.ProhibitReceiveFolderHierarchyDepth:
				return QuotaMessageType.ProhibitReceiveFolderHierarchyDepth;
			case QuotaMessageType.WarningFolderHierarchyDepthUnlimited:
				return QuotaMessageType.WarningFolderHierarchyDepthUnlimited;
			case QuotaMessageType.WarningFoldersCount:
				return QuotaMessageType.WarningFoldersCount;
			case QuotaMessageType.ProhibitReceiveFoldersCount:
				return QuotaMessageType.ProhibitReceiveFoldersCount;
			case QuotaMessageType.WarningFoldersCountUnlimited:
				return QuotaMessageType.WarningFoldersCountUnlimited;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetSystemMessage(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Original
		{
			get
			{
				return (SwitchParameter)(base.Fields["Original"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Original"] = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				ADObjectId orgContainerId = (base.DataSession as IConfigurationSession).GetOrgContainerId();
				return SystemMessage.GetDsnCustomizationContainer(orgContainerId);
			}
		}

		protected override void InternalProcessRecord()
		{
			if (this.Original)
			{
				SystemMessage dataObject = this.DataObject;
				if (dataObject.DsnCode != null)
				{
					LocalizedString localizedString;
					if (DsnDefaultMessages.TryGetResourceRecipientExplanation(dataObject.DsnCode.Value, out localizedString))
					{
						if (!ClientCultures.IsCultureSupportedForDsn(dataObject.Language))
						{
							this.WriteWarning(Strings.CustomizedDsnLanguageNotAvailable(dataObject.Language.Name, base.DataSession.Source));
						}
						dataObject.Text = localizedString.ToString(dataObject.Language);
					}
					else
					{
						base.WriteError(new CodeNotADefaultException(dataObject.DsnCode), ErrorCategory.InvalidOperation, this.DataObject);
					}
				}
				else if (dataObject.QuotaMessageType != null)
				{
					QuotaLocalizedTexts quotaLocalizedTexts = QuotaLocalizedTexts.GetQuotaLocalizedTexts(SetSystemMessage.ConvertToInternal(dataObject.QuotaMessageType.Value), string.Empty, string.Empty, true);
					dataObject.Text = quotaLocalizedTexts.Details.ToString(dataObject.Language);
				}
			}
			base.InternalProcessRecord();
		}
	}
}
