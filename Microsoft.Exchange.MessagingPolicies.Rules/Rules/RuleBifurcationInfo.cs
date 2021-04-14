using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class RuleBifurcationInfo
	{
		public List<string> FromRecipients
		{
			get
			{
				return this.fromRecipients;
			}
		}

		public List<string> FromLists
		{
			get
			{
				return this.fromLists;
			}
		}

		public List<string> Recipients
		{
			get
			{
				return this.recipients;
			}
		}

		public List<string> Lists
		{
			get
			{
				return this.lists;
			}
		}

		public List<string> Managers
		{
			get
			{
				return this.managers;
			}
		}

		public List<string> ADAttributes
		{
			get
			{
				return this.adAttributes;
			}
		}

		public List<string> ADAttributesForTextMatch
		{
			get
			{
				return this.adAttributesForTextMatch;
			}
		}

		public List<string> RecipientAddressContainsWords
		{
			get
			{
				return this.recipientAddressContainsWords;
			}
		}

		public List<string> RecipientDomainIs
		{
			get
			{
				return this.recipientDomainIs;
			}
		}

		public List<string> RecipientMatchesPatterns
		{
			get
			{
				return this.recipientMatchesPatterns;
			}
		}

		public List<string> RecipientMatchesRegexPatterns
		{
			get
			{
				return this.recipientMatchesRegexPatterns;
			}
		}

		public List<string> RecipientAttributeContains
		{
			get
			{
				return this.recipientAttributeContains;
			}
		}

		public List<string> RecipientAttributeMatches
		{
			get
			{
				return this.recipientAttributeMatches;
			}
		}

		public List<string> RecipientAttributeMatchesRegex
		{
			get
			{
				return this.recipientAttributeMatchesRegex;
			}
		}

		public List<string> RecipientInSenderList
		{
			get
			{
				return this.recipientInSenderList;
			}
		}

		public List<string> SenderInRecipientList
		{
			get
			{
				return this.senderInRecipientList;
			}
		}

		public List<string> Patterns
		{
			get
			{
				return this.patterns;
			}
		}

		public List<string> Partners
		{
			get
			{
				return this.partners;
			}
		}

		public string ManagementRelationship
		{
			get
			{
				return this.managementRelationship;
			}
			set
			{
				this.managementRelationship = value;
			}
		}

		public string ADAttributeValue
		{
			get
			{
				return this.adAttributeValue;
			}
			set
			{
				this.adAttributeValue = value;
			}
		}

		public bool ExternalRecipients
		{
			get
			{
				return this.externalRecipients;
			}
			set
			{
				this.externalRecipients = value;
			}
		}

		public bool InternalRecipients
		{
			get
			{
				return this.internalRecipients;
			}
			set
			{
				this.internalRecipients = value;
			}
		}

		public bool ExternalPartnerRecipients
		{
			get
			{
				return this.externalPartnerRecipients;
			}
			set
			{
				this.externalPartnerRecipients = value;
			}
		}

		public bool ExternalNonPartnerRecipients
		{
			get
			{
				return this.externalNonPartnerRecipients;
			}
			set
			{
				this.externalNonPartnerRecipients = value;
			}
		}

		public bool IsSenderEvaluation
		{
			get
			{
				return this.isSenderEvaluation;
			}
			set
			{
				this.isSenderEvaluation = value;
			}
		}

		public bool CheckADAttributeEquality
		{
			get
			{
				return this.checkADAttributeEquality;
			}
			set
			{
				this.checkADAttributeEquality = value;
			}
		}

		public bool Exception
		{
			get
			{
				return this.exception;
			}
			set
			{
				this.exception = value;
			}
		}

		public Version MinimumVersion
		{
			get
			{
				if (this.RecipientDomainIs.Any<string>())
				{
					return RuleBifurcationInfo.RecipientDomainIsVersion;
				}
				if (this.recipientMatchesRegexPatterns.Any<string>() || this.recipientAttributeMatchesRegex.Any<string>())
				{
					return Rule.BaseVersion15;
				}
				return Rule.BaseVersion;
			}
		}

		public int GetEstimatedSize()
		{
			int num = 18;
			this.AddStringListPropertySize(this.fromRecipients, ref num);
			this.AddStringListPropertySize(this.fromLists, ref num);
			this.AddStringListPropertySize(this.recipients, ref num);
			this.AddStringListPropertySize(this.managers, ref num);
			this.AddStringListPropertySize(this.adAttributes, ref num);
			this.AddStringListPropertySize(this.adAttributesForTextMatch, ref num);
			this.AddStringListPropertySize(this.lists, ref num);
			this.AddStringListPropertySize(this.recipientAddressContainsWords, ref num);
			this.AddStringListPropertySize(this.recipientDomainIs, ref num);
			this.AddStringListPropertySize(this.recipientMatchesPatterns, ref num);
			this.AddStringListPropertySize(this.recipientMatchesRegexPatterns, ref num);
			this.AddStringListPropertySize(this.recipientAttributeContains, ref num);
			this.AddStringListPropertySize(this.recipientAttributeMatches, ref num);
			this.AddStringListPropertySize(this.recipientAttributeMatchesRegex, ref num);
			this.AddStringListPropertySize(this.senderInRecipientList, ref num);
			this.AddStringListPropertySize(this.recipientInSenderList, ref num);
			this.AddStringListPropertySize(this.patterns, ref num);
			this.AddStringListPropertySize(this.partners, ref num);
			num += 7;
			if (!string.IsNullOrEmpty(this.managementRelationship))
			{
				num += this.managementRelationship.Length * 2;
				num += 18;
			}
			if (!string.IsNullOrEmpty(this.adAttributeValue))
			{
				num += this.adAttributeValue.Length * 2;
				num += 18;
			}
			return num;
		}

		public void AddStringListPropertySize(List<string> property, ref int size)
		{
			if (property != null)
			{
				size += 18;
				foreach (string text in property)
				{
					size += text.Length * 2;
					size += 18;
				}
			}
		}

		public static readonly Version RecipientDomainIsVersion = new Version("15.00.0005.02");

		private readonly List<string> fromRecipients = new List<string>();

		private readonly List<string> fromLists = new List<string>();

		private readonly List<string> recipients = new List<string>();

		private readonly List<string> managers = new List<string>();

		private readonly List<string> adAttributes = new List<string>();

		private readonly List<string> adAttributesForTextMatch = new List<string>();

		private readonly List<string> lists = new List<string>();

		private readonly List<string> recipientAddressContainsWords = new List<string>();

		private readonly List<string> recipientDomainIs = new List<string>();

		private readonly List<string> recipientMatchesPatterns = new List<string>();

		private readonly List<string> recipientMatchesRegexPatterns = new List<string>();

		private readonly List<string> recipientAttributeContains = new List<string>();

		private readonly List<string> recipientAttributeMatches = new List<string>();

		private readonly List<string> recipientAttributeMatchesRegex = new List<string>();

		private readonly List<string> senderInRecipientList = new List<string>();

		private readonly List<string> recipientInSenderList = new List<string>();

		private readonly List<string> patterns = new List<string>();

		private readonly List<string> partners = new List<string>();

		private bool externalRecipients;

		private bool internalRecipients;

		private bool externalPartnerRecipients;

		private bool externalNonPartnerRecipients;

		private bool exception;

		private bool isSenderEvaluation;

		private bool checkADAttributeEquality;

		private string managementRelationship;

		private string adAttributeValue;
	}
}
