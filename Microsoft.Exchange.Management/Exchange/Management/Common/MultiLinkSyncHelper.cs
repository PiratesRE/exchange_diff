using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Common
{
	internal class MultiLinkSyncHelper
	{
		public static Dictionary<object, ArrayList> GetIncompatibleParametersDictionaryForCommonMultiLink()
		{
			Dictionary<object, ArrayList> dictionary = new Dictionary<object, ArrayList>();
			dictionary[ADRecipientSchema.AcceptMessagesOnlyFromSendersOrMembers] = new ArrayList(new object[]
			{
				ADRecipientSchema.AcceptMessagesOnlyFrom,
				ADRecipientSchema.AcceptMessagesOnlyFromDLMembers,
				"RawAcceptMessagesOnlyFrom"
			});
			dictionary[ADRecipientSchema.AcceptMessagesOnlyFrom] = new ArrayList(new object[]
			{
				"RawAcceptMessagesOnlyFrom"
			});
			dictionary[ADRecipientSchema.RejectMessagesFromSendersOrMembers] = new ArrayList(new object[]
			{
				ADRecipientSchema.RejectMessagesFrom,
				ADRecipientSchema.RejectMessagesFromDLMembers,
				"RawRejectMessagesFrom"
			});
			dictionary[ADRecipientSchema.RejectMessagesFrom] = new ArrayList(new object[]
			{
				"RawRejectMessagesFrom"
			});
			dictionary[ADRecipientSchema.BypassModerationFromSendersOrMembers] = new ArrayList(new object[]
			{
				MailEnabledRecipientSchema.BypassModerationFrom,
				MailEnabledRecipientSchema.BypassModerationFromDLMembers,
				"RawBypassModerationFrom"
			});
			dictionary[ADRecipientSchema.BypassModerationFrom] = new ArrayList(new object[]
			{
				"RawBypassModerationFrom"
			});
			dictionary[ADRecipientSchema.GrantSendOnBehalfTo] = new ArrayList(new object[]
			{
				"RawGrantSendOnBehalfTo"
			});
			dictionary[ADRecipientSchema.ModeratedBy] = new ArrayList(new object[]
			{
				"RawModeratedBy"
			});
			dictionary[ADRecipientSchema.ForwardingAddress] = new ArrayList(new object[]
			{
				"RawForwardingAddress"
			});
			return dictionary;
		}

		public static void ValidateIncompatibleParameters(PropertyBag parameters, Dictionary<object, ArrayList> incompatibleParametersDictionary, Task.ErrorLoggerDelegate writeError)
		{
			foreach (object obj in incompatibleParametersDictionary.Keys)
			{
				if (parameters.IsModified(obj))
				{
					foreach (object obj2 in incompatibleParametersDictionary[obj])
					{
						if (parameters.IsModified(obj2))
						{
							writeError(new RecipientTaskException(Strings.ErrorConflictingRestrictionParameters(MultiLinkSyncHelper.GetPropertyName(obj), MultiLinkSyncHelper.GetPropertyName(obj2))), ExchangeErrorCategory.Client, null);
						}
					}
				}
			}
		}

		private static string GetPropertyName(object property)
		{
			if (property is ADPropertyDefinition)
			{
				return (property as ADPropertyDefinition).Name;
			}
			return property.ToString();
		}

		public const string RawAcceptMessagesOnlyFrom = "RawAcceptMessagesOnlyFrom";

		public const string RawBypassModerationFrom = "RawBypassModerationFrom";

		public const string RawRejectMessagesFrom = "RawRejectMessagesFrom";

		public const string RawGrantSendOnBehalfTo = "RawGrantSendOnBehalfTo";

		public const string RawModeratedBy = "RawModeratedBy";

		public const string RawForwardingAddress = "RawForwardingAddress";

		public const string RawMembers = "RawMembers";

		public const string RawCoManagedBy = "RawCoManagedBy";

		public const string Members = "Members";

		public const string AddedMembers = "AddedMembers";

		public const string RemovedMembers = "RemovedMembers";

		public const string RawSiteMailboxOwners = "RawSiteMailboxOwners";

		public const string RawSiteMailboxUsers = "RawSiteMailboxUsers";
	}
}
