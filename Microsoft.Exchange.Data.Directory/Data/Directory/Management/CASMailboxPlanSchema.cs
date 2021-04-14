﻿using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class CASMailboxPlanSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ADUserSchema>();
		}

		public static readonly ADPropertyDefinition ActiveSyncDebugLogging = ADUserSchema.ActiveSyncDebugLogging;

		public static readonly ADPropertyDefinition ActiveSyncEnabled = ADUserSchema.ActiveSyncEnabled;

		public static readonly ADPropertyDefinition ActiveSyncMailboxPolicy = ADUserSchema.ActiveSyncMailboxPolicy;

		public static readonly ADPropertyDefinition DisplayName = ADRecipientSchema.DisplayName;

		public static readonly ADPropertyDefinition ECPEnabled = ADRecipientSchema.ECPEnabled;

		public static readonly ADPropertyDefinition ImapEnabled = ADRecipientSchema.ImapEnabled;

		public static readonly ADPropertyDefinition ImapUseProtocolDefaults = ADRecipientSchema.ImapUseProtocolDefaults;

		public static readonly ADPropertyDefinition ImapMessagesRetrievalMimeFormat = ADRecipientSchema.ImapMessagesRetrievalMimeFormat;

		public static readonly ADPropertyDefinition ImapEnableExactRFC822Size = ADRecipientSchema.ImapEnableExactRFC822Size;

		public static readonly ADPropertyDefinition ImapProtocolLoggingEnabled = ADRecipientSchema.ImapProtocolLoggingEnabled;

		public static readonly ADPropertyDefinition ImapSuppressReadReceipt = ADRecipientSchema.ImapSuppressReadReceipt;

		public static readonly ADPropertyDefinition ImapForceICalForCalendarRetrievalOption = ADRecipientSchema.ImapForceICalForCalendarRetrievalOption;

		public static readonly ADPropertyDefinition MAPIEnabled = ADRecipientSchema.MAPIEnabled;

		public static readonly ADPropertyDefinition MapiHttpEnabled = ADRecipientSchema.MapiHttpEnabled;

		public static readonly ADPropertyDefinition MAPIBlockOutlookNonCachedMode = ADRecipientSchema.MAPIBlockOutlookNonCachedMode;

		public static readonly ADPropertyDefinition MAPIBlockOutlookVersions = ADRecipientSchema.MAPIBlockOutlookVersions;

		public static readonly ADPropertyDefinition MAPIBlockOutlookRpcHttp = ADRecipientSchema.MAPIBlockOutlookRpcHttp;

		public static readonly ADPropertyDefinition MAPIBlockOutlookExternalConnectivity = ADRecipientSchema.MAPIBlockOutlookExternalConnectivity;

		public static readonly ADPropertyDefinition OwaMailboxPolicy = ADUserSchema.OwaMailboxPolicy;

		public static readonly ADPropertyDefinition OWAEnabled = ADRecipientSchema.OWAEnabled;

		public static readonly ADPropertyDefinition OWAforDevicesEnabled = ADUserSchema.OWAforDevicesEnabled;

		public static readonly ADPropertyDefinition PopEnabled = ADRecipientSchema.PopEnabled;

		public static readonly ADPropertyDefinition PopUseProtocolDefaults = ADRecipientSchema.PopUseProtocolDefaults;

		public static readonly ADPropertyDefinition PopMessagesRetrievalMimeFormat = ADRecipientSchema.PopMessagesRetrievalMimeFormat;

		public static readonly ADPropertyDefinition PopEnableExactRFC822Size = ADRecipientSchema.PopEnableExactRFC822Size;

		public static readonly ADPropertyDefinition PopProtocolLoggingEnabled = ADRecipientSchema.PopProtocolLoggingEnabled;

		public static readonly ADPropertyDefinition PopSuppressReadReceipt = ADRecipientSchema.PopSuppressReadReceipt;

		public static readonly ADPropertyDefinition PopForceICalForCalendarRetrievalOption = ADRecipientSchema.PopForceICalForCalendarRetrievalOption;

		public static readonly ADPropertyDefinition RemotePowerShellEnabled = ADRecipientSchema.RemotePowerShellEnabled;

		public static readonly ADPropertyDefinition EwsEnabled = ADRecipientSchema.EwsEnabled;

		public static readonly ADPropertyDefinition EwsAllowOutlook = ADRecipientSchema.EwsAllowOutlook;

		public static readonly ADPropertyDefinition EwsAllowMacOutlook = ADRecipientSchema.EwsAllowMacOutlook;

		public static readonly ADPropertyDefinition EwsAllowEntourage = ADRecipientSchema.EwsAllowEntourage;

		public static readonly ADPropertyDefinition EwsApplicationAccessPolicy = ADRecipientSchema.EwsApplicationAccessPolicy;

		public static readonly ADPropertyDefinition EwsExceptions = ADRecipientSchema.EwsExceptions;
	}
}
