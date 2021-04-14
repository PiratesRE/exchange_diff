using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.UnJournalAgent
{
	internal static class AgentStrings
	{
		static AgentStrings()
		{
			AgentStrings.stringIDs.Add(4023179196U, "PermanentErrorOther");
			AgentStrings.stringIDs.Add(2314169302U, "NoRecipientsResolvedMsg");
			AgentStrings.stringIDs.Add(3020404937U, "UnexpectedJournalMessageFormatMsg");
		}

		public static LocalizedString DefectiveJournalNoRecipients(string recipientList)
		{
			return new LocalizedString("DefectiveJournalNoRecipients", AgentStrings.ResourceManager, new object[]
			{
				recipientList
			});
		}

		public static LocalizedString InvalidEnvelopeJournalMessageMissingReport(string messageid)
		{
			return new LocalizedString("InvalidEnvelopeJournalMessageMissingReport", AgentStrings.ResourceManager, new object[]
			{
				messageid
			});
		}

		public static LocalizedString PermanentErrorOther
		{
			get
			{
				return new LocalizedString("PermanentErrorOther", AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidEnvelopeJournalMessageMissingEmbedded(string messageid)
		{
			return new LocalizedString("InvalidEnvelopeJournalMessageMissingEmbedded", AgentStrings.ResourceManager, new object[]
			{
				messageid
			});
		}

		public static LocalizedString InvalidEnvelopeJournalMessageAttachment(string messageid)
		{
			return new LocalizedString("InvalidEnvelopeJournalMessageAttachment", AgentStrings.ResourceManager, new object[]
			{
				messageid
			});
		}

		public static LocalizedString InvalidEnvelopeJournalVersionForExtraction(string version)
		{
			return new LocalizedString("InvalidEnvelopeJournalVersionForExtraction", AgentStrings.ResourceManager, new object[]
			{
				version
			});
		}

		public static LocalizedString NoRecipientsResolvedMsg
		{
			get
			{
				return new LocalizedString("NoRecipientsResolvedMsg", AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnProvisionedRecipientsMsg(string recipientList)
		{
			return new LocalizedString("UnProvisionedRecipientsMsg", AgentStrings.ResourceManager, new object[]
			{
				recipientList
			});
		}

		public static LocalizedString UnexpectedJournalMessageFormatMsg
		{
			get
			{
				return new LocalizedString("UnexpectedJournalMessageFormatMsg", AgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidEnvelopeJournalMessageMissingSender(string messageid)
		{
			return new LocalizedString("InvalidEnvelopeJournalMessageMissingSender", AgentStrings.ResourceManager, new object[]
			{
				messageid
			});
		}

		public static LocalizedString InvalidEnvelopeJournalMessagesInvalidFormat(string messageid, string error)
		{
			return new LocalizedString("InvalidEnvelopeJournalMessagesInvalidFormat", AgentStrings.ResourceManager, new object[]
			{
				messageid,
				error
			});
		}

		public static LocalizedString DefectiveJournalWithRecipients(string recipientList)
		{
			return new LocalizedString("DefectiveJournalWithRecipients", AgentStrings.ResourceManager, new object[]
			{
				recipientList
			});
		}

		public static LocalizedString InvalidEnvelopeJournalMessageMissingRequiredMessageId(string messageid)
		{
			return new LocalizedString("InvalidEnvelopeJournalMessageMissingRequiredMessageId", AgentStrings.ResourceManager, new object[]
			{
				messageid
			});
		}

		public static LocalizedString GetLocalizedString(AgentStrings.IDs key)
		{
			return new LocalizedString(AgentStrings.stringIDs[(uint)key], AgentStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(3);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.MessagingPolicies.UnJournalAgent.AgentStrings", typeof(AgentStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			PermanentErrorOther = 4023179196U,
			NoRecipientsResolvedMsg = 2314169302U,
			UnexpectedJournalMessageFormatMsg = 3020404937U
		}

		private enum ParamIDs
		{
			DefectiveJournalNoRecipients,
			InvalidEnvelopeJournalMessageMissingReport,
			InvalidEnvelopeJournalMessageMissingEmbedded,
			InvalidEnvelopeJournalMessageAttachment,
			InvalidEnvelopeJournalVersionForExtraction,
			UnProvisionedRecipientsMsg,
			InvalidEnvelopeJournalMessageMissingSender,
			InvalidEnvelopeJournalMessagesInvalidFormat,
			DefectiveJournalWithRecipients,
			InvalidEnvelopeJournalMessageMissingRequiredMessageId
		}
	}
}
