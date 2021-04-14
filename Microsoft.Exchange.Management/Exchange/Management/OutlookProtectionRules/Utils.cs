using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.OutlookProtectionRules
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class Utils
	{
		public static ADObjectId GetRuleCollectionId(IConfigDataProvider session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			return RuleIdParameter.GetRuleCollectionId(session, "OutlookProtectionRules");
		}

		public static ADObjectId GetRuleId(IConfigDataProvider session, string name)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			ADObjectId ruleCollectionId = Utils.GetRuleCollectionId(session);
			return ruleCollectionId.GetChildId(name);
		}

		public static bool IsEmptyCondition(TransportRule rule)
		{
			if (rule == null)
			{
				throw new ArgumentNullException("rule");
			}
			OutlookProtectionRulePresentationObject outlookProtectionRulePresentationObject = new OutlookProtectionRulePresentationObject(rule);
			return outlookProtectionRulePresentationObject.IsEmptyCondition();
		}

		public static IEnumerable<SmtpAddress> ResolveRecipientIdParameters(IRecipientSession recipientSession, IEnumerable<RecipientIdParameter> recipientIdParameters, out LocalizedException resolutionError)
		{
			resolutionError = null;
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			if (recipientIdParameters == null)
			{
				return Utils.EmptySmtpAddressArray;
			}
			LinkedList<SmtpAddress> linkedList = new LinkedList<SmtpAddress>();
			foreach (RecipientIdParameter recipientIdParameter in recipientIdParameters)
			{
				SmtpAddress smtpAddress = Utils.ResolveRecipientIdParameter(recipientSession, recipientIdParameter, out resolutionError);
				if (resolutionError != null || SmtpAddress.Empty.Equals(smtpAddress))
				{
					return null;
				}
				linkedList.AddLast(smtpAddress);
			}
			return linkedList;
		}

		public static bool IsChildOfOutlookProtectionRuleContainer(RuleIdParameter ruleId)
		{
			return ruleId == null || ruleId.InternalADObjectId == null || ruleId.InternalADObjectId.Parent == null || string.Equals(ruleId.InternalADObjectId.Parent.Name, "OutlookProtectionRules", StringComparison.OrdinalIgnoreCase);
		}

		private static SmtpAddress ResolveRecipientIdParameter(IRecipientSession recipientSession, RecipientIdParameter recipientIdParameter, out LocalizedException resolutionError)
		{
			resolutionError = null;
			if (SmtpAddress.IsValidSmtpAddress(recipientIdParameter.RawIdentity))
			{
				return SmtpAddress.Parse(recipientIdParameter.RawIdentity);
			}
			IEnumerable<ADRecipient> objects = recipientIdParameter.GetObjects<ADRecipient>(null, recipientSession);
			ADRecipient adrecipient = null;
			using (IEnumerator<ADRecipient> enumerator = objects.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					resolutionError = new NoRecipientsForRecipientIdException(recipientIdParameter.ToString());
					return SmtpAddress.Empty;
				}
				adrecipient = enumerator.Current;
				if (enumerator.MoveNext())
				{
					resolutionError = new MoreThanOneRecipientForRecipientIdException(recipientIdParameter.ToString());
					return SmtpAddress.Empty;
				}
			}
			if (SmtpAddress.Empty.Equals(adrecipient.PrimarySmtpAddress))
			{
				resolutionError = new NoSmtpAddressForRecipientIdException(recipientIdParameter.ToString());
				return SmtpAddress.Empty;
			}
			return adrecipient.PrimarySmtpAddress;
		}

		private static readonly SmtpAddress[] EmptySmtpAddressArray = new SmtpAddress[0];
	}
}
