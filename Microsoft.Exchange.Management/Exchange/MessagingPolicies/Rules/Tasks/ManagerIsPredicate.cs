using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class ManagerIsPredicate : BifurcationInfoPredicate, IEquatable<ManagerIsPredicate>
	{
		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<SmtpAddress>(this.Addresses) + this.EvaluatedUser.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as ManagerIsPredicate)));
		}

		public bool Equals(ManagerIsPredicate other)
		{
			if (this.Addresses == null)
			{
				return other.Addresses == null && this.EvaluatedUser.Equals(other.EvaluatedUser);
			}
			return this.Addresses.SequenceEqual(other.Addresses) && this.EvaluatedUser.Equals(other.EvaluatedUser);
		}

		[ConditionParameterName("ManagerAddresses")]
		[LocDescription(RulesTasksStrings.IDs.ToAddressesDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.ToAddressesDisplayName)]
		[ExceptionParameterName("ExceptIfManagerAddresses")]
		public SmtpAddress[] Addresses
		{
			get
			{
				return this.addresses;
			}
			set
			{
				this.addresses = value;
			}
		}

		[LocDisplayName(RulesTasksStrings.IDs.EvaluatedUserDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.EvaluatedUserDescription)]
		[ConditionParameterName("ManagerForEvaluatedUser")]
		[ExceptionParameterName("ExceptIfManagerForEvaluatedUser")]
		public EvaluatedUser EvaluatedUser
		{
			get
			{
				return this.evaluatedUser;
			}
			set
			{
				this.evaluatedUser = value;
			}
		}

		internal override string Description
		{
			get
			{
				string evaluatesUser = LocalizedDescriptionAttribute.FromEnum(typeof(EvaluatedUser), this.EvaluatedUser);
				string text = RuleDescription.BuildDescriptionStringFromStringArray(Utils.BuildSmtpAddressStringList(this.Addresses, false), RulesTasksStrings.RuleDescriptionOrDelimiter, base.MaxDescriptionListLength);
				return RulesTasksStrings.RuleDescriptionManagerIs(evaluatesUser, text);
			}
		}

		internal static TransportRulePredicate CreatePredicateFromBifInfo(RuleBifurcationInfo bifInfo)
		{
			if (bifInfo.ADAttributesForTextMatch.Count > 0 || bifInfo.ADAttributes.Count > 0 || bifInfo.Managers.Count == 0 || bifInfo.Recipients.Count > 0 || bifInfo.Lists.Count > 0 || bifInfo.FromRecipients.Count > 0 || bifInfo.FromLists.Count > 0 || bifInfo.Partners.Count > 0 || bifInfo.RecipientAddressContainsWords.Count > 0 || bifInfo.RecipientDomainIs.Count > 0 || bifInfo.RecipientMatchesPatterns.Count > 0 || bifInfo.RecipientAttributeContains.Count > 0 || bifInfo.RecipientAttributeMatches.Count > 0 || bifInfo.SenderInRecipientList.Count > 0 || bifInfo.RecipientInSenderList.Count > 0)
			{
				return null;
			}
			if (bifInfo.InternalRecipients || bifInfo.ExternalRecipients || bifInfo.ExternalPartnerRecipients || bifInfo.ExternalNonPartnerRecipients)
			{
				return null;
			}
			if (!string.IsNullOrEmpty(bifInfo.ManagementRelationship))
			{
				return null;
			}
			List<SmtpAddress> list = new List<SmtpAddress>(bifInfo.Managers.Count);
			for (int i = 0; i < bifInfo.Managers.Count; i++)
			{
				SmtpAddress item = new SmtpAddress(bifInfo.Managers[i]);
				if (item.IsValidAddress)
				{
					list.Add(item);
				}
			}
			ManagerIsPredicate managerIsPredicate = new ManagerIsPredicate();
			if (list.Count > 0)
			{
				managerIsPredicate.Addresses = list.ToArray();
				if (bifInfo.IsSenderEvaluation)
				{
					managerIsPredicate.EvaluatedUser = EvaluatedUser.Sender;
				}
				else
				{
					managerIsPredicate.EvaluatedUser = EvaluatedUser.Recipient;
				}
				return managerIsPredicate;
			}
			return null;
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			return null;
		}

		internal override void Reset()
		{
			this.addresses = null;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.Addresses == null || this.Addresses.Length == 0)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			foreach (SmtpAddress smtpAddress in this.Addresses)
			{
				if (!smtpAddress.IsValidAddress)
				{
					errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.InvalidRecipient(smtpAddress.ToString()), base.Name));
					return;
				}
			}
			base.ValidateRead(errors);
		}

		internal override RuleBifurcationInfo ToRuleBifurcationInfo(out RuleBifurcationInfo additionalBifurcationInfo)
		{
			additionalBifurcationInfo = null;
			RuleBifurcationInfo ruleBifurcationInfo = new RuleBifurcationInfo();
			foreach (SmtpAddress smtpAddress in this.Addresses)
			{
				string text = smtpAddress.ToString();
				if (string.IsNullOrEmpty(text))
				{
					throw new ArgumentException(RulesTasksStrings.InvalidRecipient(text), "Address");
				}
				ruleBifurcationInfo.Managers.Add(text);
			}
			ruleBifurcationInfo.IsSenderEvaluation = (this.EvaluatedUser == EvaluatedUser.Sender);
			return ruleBifurcationInfo;
		}

		internal override string ToCmdletParameter(bool isException = false)
		{
			return string.Format("-{0} {1} -{2} {3}", new object[]
			{
				isException ? "ExceptIfManagerAddresses" : "ManagerAddresses",
				string.Join(", ", Utils.BuildSmtpAddressStringList(this.Addresses, true)),
				isException ? "ExceptIfManagerForEvaluatedUser" : "ManagerForEvaluatedUser",
				Enum.GetName(typeof(EvaluatedUser), this.EvaluatedUser)
			});
		}

		internal override void SuppressPiiData()
		{
			string[] array;
			string[] array2;
			this.Addresses = SuppressingPiiData.Redact(this.Addresses, out array, out array2);
		}

		private SmtpAddress[] addresses;

		private EvaluatedUser evaluatedUser;
	}
}
