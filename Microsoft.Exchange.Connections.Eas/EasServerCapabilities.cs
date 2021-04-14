using System;
using System.Collections.Generic;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class EasServerCapabilities : ServerCapabilities
	{
		internal EasServerCapabilities()
		{
		}

		internal EasServerCapabilities(IEnumerable<string> capabilities) : base(capabilities)
		{
		}

		internal bool SupportsEasAutodiscover
		{
			get
			{
				return base.Supports("Autodiscover");
			}
		}

		internal bool SupportsEasFolderCreate
		{
			get
			{
				return base.Supports("FolderCreate");
			}
		}

		internal bool SupportsEasFolderDelete
		{
			get
			{
				return base.Supports("FolderDelete");
			}
		}

		internal bool SupportsEasFolderSync
		{
			get
			{
				return base.Supports("FolderSync");
			}
		}

		internal bool SupportsEasFolderUpdate
		{
			get
			{
				return base.Supports("FolderUpdate");
			}
		}

		internal bool SupportsEasGetAttachment
		{
			get
			{
				return base.Supports("GetAttachment");
			}
		}

		internal bool SupportsEasGetItemEstimate
		{
			get
			{
				return base.Supports("GetItemEstimate");
			}
		}

		internal bool SupportsEasItemOperations
		{
			get
			{
				return base.Supports("ItemOperations");
			}
		}

		internal bool SupportsEasMeetingResponse
		{
			get
			{
				return base.Supports("MeetingResponse");
			}
		}

		internal bool SupportsEasMoveItems
		{
			get
			{
				return base.Supports("MoveItems");
			}
		}

		internal bool SupportsEasPing
		{
			get
			{
				return base.Supports("Ping");
			}
		}

		internal bool SupportsEasProvision
		{
			get
			{
				return base.Supports("Provision");
			}
		}

		internal bool SupportsEasResolveRecipients
		{
			get
			{
				return base.Supports("ResolveRecipients");
			}
		}

		internal bool SupportsEasSearch
		{
			get
			{
				return base.Supports("Search");
			}
		}

		internal bool SupportsEasSendMail
		{
			get
			{
				return base.Supports("SendMail");
			}
		}

		internal bool SupportsEasSettings
		{
			get
			{
				return base.Supports("Settings");
			}
		}

		internal bool SupportsEasSmartForward
		{
			get
			{
				return base.Supports("SmartForward");
			}
		}

		internal bool SupportsEasSmartReply
		{
			get
			{
				return base.Supports("SmartReply");
			}
		}

		internal bool SupportsEasSync
		{
			get
			{
				return base.Supports("Sync");
			}
		}

		internal bool SupportsEasValidateCert
		{
			get
			{
				return base.Supports("ValidateCert");
			}
		}

		internal bool SupportsEasVersion140
		{
			get
			{
				return base.Supports("14.0");
			}
		}

		internal const string EasAutodiscoverCapability = "Autodiscover";

		internal const string EasFolderCreateCapability = "FolderCreate";

		internal const string EasFolderDeleteCapability = "FolderDelete";

		internal const string EasFolderSyncCapability = "FolderSync";

		internal const string EasFolderUpdateCapability = "FolderUpdate";

		internal const string EasGetAttachmentCapability = "GetAttachment";

		internal const string EasGetItemEstimateCapability = "GetItemEstimate";

		internal const string EasItemOperationsCapability = "ItemOperations";

		internal const string EasMeetingResponseCapability = "MeetingResponse";

		internal const string EasMoveItemsCapability = "MoveItems";

		internal const string EasPingCapability = "Ping";

		internal const string EasProvisionCapability = "Provision";

		internal const string EasResolveRecipientsCapability = "ResolveRecipients";

		internal const string EasSearchCapability = "Search";

		internal const string EasSendMailCapability = "SendMail";

		internal const string EasSettingsCapability = "Settings";

		internal const string EasSmartForwardCapability = "SmartForward";

		internal const string EasSmartReplyCapability = "SmartReply";

		internal const string EasSyncCapability = "Sync";

		internal const string EasValidateCertCapability = "ValidateCert";

		internal const string EasVersion140Capability = "14.0";
	}
}
