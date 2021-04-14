using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	[Serializable]
	public sealed class JournalRuleObject : RulePresentationObjectBase
	{
		internal ObjectSchema Schema
		{
			get
			{
				return JournalRuleObject.schema;
			}
		}

		public SmtpAddress? Recipient
		{
			get
			{
				SmtpAddress? smtpAddress = this.recipient;
				if (smtpAddress != null && SuppressingPiiContext.NeedPiiSuppression)
				{
					smtpAddress = new SmtpAddress?((SmtpAddress)SuppressingPiiProperty.TryRedact(JournalRuleObjectSchema.Recipient, smtpAddress));
				}
				return smtpAddress;
			}
			set
			{
				this.recipient = value;
			}
		}

		public SmtpAddress JournalEmailAddress
		{
			get
			{
				SmtpAddress smtpAddress = this.journalEmailAddress;
				if (SuppressingPiiContext.NeedPiiSuppression)
				{
					smtpAddress = (SmtpAddress)SuppressingPiiProperty.TryRedact(JournalRuleObjectSchema.JournalEmailAddress, smtpAddress);
				}
				return smtpAddress;
			}
			set
			{
				this.journalEmailAddress = value;
			}
		}

		public JournalRuleScope Scope
		{
			get
			{
				return this.ruleScope;
			}
			set
			{
				this.ruleScope = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		public DateTime? ExpiryDate
		{
			get
			{
				return this.expiryDate;
			}
			set
			{
				this.expiryDate = value;
			}
		}

		public JournalRuleType RuleType
		{
			get
			{
				return this.ruleType;
			}
			set
			{
				this.ruleType = value;
			}
		}

		public JournalRuleObject()
		{
			this.ruleScope = JournalRuleScope.Global;
		}

		internal JournalRuleObject(string name, bool enabled, SmtpAddress? recipient, SmtpAddress journalEmailAddress, JournalRuleScope ruleScope, DateTime? expiryDate, GccType gccRuleType)
		{
			base.Name = name;
			this.enabled = enabled;
			this.recipient = recipient;
			this.ruleScope = ruleScope;
			this.journalEmailAddress = journalEmailAddress;
			this.expiryDate = expiryDate;
			this.ruleType = JournalRuleObject.ConvertGccTypeToJournalRuleType(gccRuleType);
		}

		internal static bool LookupAndCheckAllowedTypes(RecipientIdParameter recipId, IRecipientSession recipSession, OrganizationId orgId, bool isNonGccInDc, out SmtpAddress address)
		{
			Microsoft.Exchange.Data.Directory.Recipient.RecipientType? recipientType;
			address = JournalRuleObject.Lookup(recipId, recipSession, orgId, isNonGccInDc && JournalRuleObject.isJournalAddressCheckEnabled, out recipientType);
			return !isNonGccInDc || !JournalRuleObject.isJournalAddressCheckEnabled || recipientType == null || !(recipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUser) || !(recipientType != Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailContact);
		}

		internal static SmtpAddress Lookup(RecipientIdParameter recipId, IRecipientSession recipSession, OrganizationId orgId, bool getRecipientType, out Microsoft.Exchange.Data.Directory.Recipient.RecipientType? recipientType)
		{
			recipientType = null;
			bool flag = SmtpAddress.IsValidSmtpAddress(recipId.RawIdentity);
			if (!getRecipientType && flag)
			{
				return SmtpAddress.Parse(recipId.RawIdentity);
			}
			if (flag)
			{
				SmtpAddress smtpAddress = SmtpAddress.Parse(recipId.RawIdentity);
				ADRecipient adrecipientBySmtpAddress = JournalRuleObject.GetADRecipientBySmtpAddress(smtpAddress);
				if (adrecipientBySmtpAddress != null)
				{
					recipientType = new Microsoft.Exchange.Data.Directory.Recipient.RecipientType?(adrecipientBySmtpAddress.RecipientType);
				}
				return smtpAddress;
			}
			IEnumerable<ADRecipient> objects = recipId.GetObjects<ADRecipient>((null == orgId) ? null : orgId.OrganizationalUnit, recipSession ?? DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, orgId.ToADSessionSettings(), 429, "Lookup", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\JournalRule\\JournalRuleObject.cs"));
			ADRecipient adrecipient = null;
			using (IEnumerator<ADRecipient> enumerator = objects.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					MimeRecipient mimeRecipient = null;
					try
					{
						mimeRecipient = MimeRecipient.Parse(recipId.RawIdentity, AddressParserFlags.IgnoreComments | AddressParserFlags.AllowSquareBrackets);
					}
					catch (MimeException)
					{
					}
					if (mimeRecipient == null || string.IsNullOrEmpty(mimeRecipient.Email) || !SmtpAddress.IsValidSmtpAddress(mimeRecipient.Email))
					{
						throw new RecipientInvalidException(Strings.NoRecipients);
					}
					return new SmtpAddress(mimeRecipient.Email);
				}
				else
				{
					adrecipient = enumerator.Current;
					if (enumerator.MoveNext())
					{
						throw new RecipientInvalidException(Strings.MoreThanOneRecipient);
					}
				}
			}
			if (adrecipient is IADOrgPerson && adrecipient.PrimarySmtpAddress.Equals(SmtpAddress.Empty))
			{
				throw new RecipientInvalidException(Strings.NoSmtpAddress);
			}
			recipientType = new Microsoft.Exchange.Data.Directory.Recipient.RecipientType?(adrecipient.RecipientType);
			return adrecipient.PrimarySmtpAddress;
		}

		internal static JournalRuleType ConvertGccTypeToJournalRuleType(GccType gccRuleType)
		{
			switch (gccRuleType)
			{
			case GccType.Full:
				return JournalRuleType.Full;
			case GccType.Prtt:
				return JournalRuleType.Prtt;
			}
			return JournalRuleType.Journaling;
		}

		internal static GccType ConvertJournalRuleTypeToGccType(JournalRuleType journalRuleType)
		{
			switch (journalRuleType)
			{
			case JournalRuleType.Prtt:
				return GccType.Prtt;
			case JournalRuleType.Full:
				return GccType.Full;
			}
			return GccType.None;
		}

		internal static JournalRuleObject CreateCorruptJournalRuleObject(TransportRule transportRule, LocalizedString errorText)
		{
			return new JournalRuleObject(transportRule.Name, false, null, SmtpAddress.Empty, JournalRuleScope.Global, null, GccType.None)
			{
				isValid = false,
				errorText = errorText
			};
		}

		internal static SmtpAddress GetJournalingReportNdrToSmtpAddress(OrganizationId orgId, IConfigurationSession configSession)
		{
			SmtpAddress result = SmtpAddress.Empty;
			if (orgId != OrganizationId.ForestWideOrgId)
			{
				TransportConfigContainer[] array = configSession.Find<TransportConfigContainer>(orgId.ConfigurationUnit, QueryScope.SubTree, null, null, 1);
				if (array != null && array.Length == 1)
				{
					result = array[0].JournalingReportNdrTo;
				}
			}
			return result;
		}

		internal static JournalNdrValidationCheckResult ValidateJournalNdrMailboxSetting(IConfigDataProvider dataProvider, SmtpAddress journalNdrToAddress)
		{
			ADJournalRuleStorageManager adjournalRuleStorageManager = null;
			bool flag = false;
			try
			{
				adjournalRuleStorageManager = new ADJournalRuleStorageManager("JournalingVersioned", dataProvider);
			}
			catch (RuleCollectionNotInAdException)
			{
			}
			if (adjournalRuleStorageManager != null)
			{
				adjournalRuleStorageManager.LoadRuleCollection();
				RoutingAddress value = new RoutingAddress(journalNdrToAddress.ToString());
				if (value == RoutingAddress.NullReversePath)
				{
					if (adjournalRuleStorageManager.Count > 0)
					{
						return JournalNdrValidationCheckResult.JournalNdrCannotBeNullReversePath;
					}
				}
				else
				{
					foreach (Microsoft.Exchange.MessagingPolicies.Rules.Rule rule in adjournalRuleStorageManager.GetRuleCollection())
					{
						TransportRule transportRule = (TransportRule)rule;
						JournalRuleObject journalRuleObject = new JournalRuleObject();
						journalRuleObject.Deserialize(transportRule as JournalingRule);
						if (journalRuleObject.Recipient != null && journalRuleObject.Recipient != null && journalNdrToAddress == journalRuleObject.Recipient.Value)
						{
							return JournalNdrValidationCheckResult.JournalNdrExistInJournalRuleRecipient;
						}
						if (journalNdrToAddress == journalRuleObject.JournalEmailAddress)
						{
							flag = true;
						}
					}
				}
			}
			if (!flag)
			{
				return JournalNdrValidationCheckResult.JournalNdrValidationPassed;
			}
			return JournalNdrValidationCheckResult.JournalNdrExistInJournalRuleJournalEmailAddress;
		}

		private static bool IsJournalAddressCheckEnabled()
		{
			try
			{
				object value = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Exchange_Test\\v15\\BCM", "DisableJournalingToDCMailboxFilter", 0);
				if (value is int && (int)value != 0)
				{
					return false;
				}
			}
			catch (SecurityException)
			{
			}
			catch (IOException)
			{
			}
			return true;
		}

		internal JournalingRule Serialize()
		{
			Microsoft.Exchange.MessagingPolicies.Rules.Condition condition = this.CreateScopeCondition();
			Microsoft.Exchange.MessagingPolicies.Rules.Condition condition2 = this.CreateRecipientCondition();
			Microsoft.Exchange.MessagingPolicies.Rules.Condition condition3 = this.CombineUsingAnd(new Microsoft.Exchange.MessagingPolicies.Rules.Condition[]
			{
				condition,
				condition2
			});
			Microsoft.Exchange.MessagingPolicies.Rules.Action item = this.CreateAction();
			JournalingRule journalingRule = (JournalingRule)JournalingRuleParser.Instance.CreateRule(base.Name);
			journalingRule.Condition = condition3;
			journalingRule.Actions.Add(item);
			journalingRule.ExpiryDate = this.ExpiryDate;
			journalingRule.GccRuleType = JournalRuleObject.ConvertJournalRuleTypeToGccType(this.RuleType);
			journalingRule.Enabled = (this.enabled ? RuleState.Enabled : RuleState.Disabled);
			return journalingRule;
		}

		internal void Deserialize(JournalingRule rule)
		{
			base.Name = rule.Name;
			this.ReadAction(rule);
			this.enabled = (rule.Enabled == RuleState.Enabled);
			this.ExpiryDate = rule.ExpiryDate;
			this.RuleType = JournalRuleObject.ConvertGccTypeToJournalRuleType(rule.GccRuleType);
			this.Recipient = null;
			this.Scope = JournalRuleScope.Global;
			if (rule.Condition.ConditionType == ConditionType.True)
			{
				return;
			}
			if (this.TryReadScopeCondition(rule.Condition))
			{
				return;
			}
			if (this.TryReadRecipientCondition(rule.Condition))
			{
				return;
			}
			if (rule.Condition.ConditionType != ConditionType.And)
			{
				throw new JournalRuleCorruptException();
			}
			List<Microsoft.Exchange.MessagingPolicies.Rules.Condition> subConditions = ((AndCondition)rule.Condition).SubConditions;
			if (subConditions.Count != 2)
			{
				throw new JournalRuleCorruptException();
			}
			foreach (Microsoft.Exchange.MessagingPolicies.Rules.Condition condition in subConditions)
			{
				if (!this.TryReadScopeCondition(condition) && !this.TryReadRecipientCondition(condition))
				{
					throw new JournalRuleCorruptException();
				}
			}
		}

		private void ReadAction(Microsoft.Exchange.MessagingPolicies.Rules.Rule rule)
		{
			if (rule.Actions.Count != 1)
			{
				throw new JournalRuleCorruptException();
			}
			string name;
			if ((name = rule.Actions[0].Name) != null && name == "Journal")
			{
				this.ReadJournalAction(rule.Actions[0]);
				return;
			}
			throw new JournalRuleCorruptException();
		}

		private static ADRecipient GetADRecipientBySmtpAddress(SmtpAddress smtpAddress)
		{
			try
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.IgnoreInvalid, null, ADSessionSettings.FromTenantAcceptedDomain(smtpAddress.Domain), 802, "GetADRecipientBySmtpAddress", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\JournalRule\\JournalRuleObject.cs");
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.PrimarySmtpAddress, smtpAddress.ToString());
				ADRecipient[] array = tenantOrRootOrgRecipientSession.Find<ADRecipient>(null, QueryScope.SubTree, filter, null, 1);
				if (array != null && array.Length > 0)
				{
					return array[0];
				}
			}
			catch (CannotResolveTenantNameException)
			{
			}
			return null;
		}

		private void ReadJournalAction(Microsoft.Exchange.MessagingPolicies.Rules.Action action)
		{
			if (action.Arguments.Count != 1 || !action.Arguments[0].IsString)
			{
				throw new JournalRuleCorruptException();
			}
			this.ReadJournalEmailAddress(((Value)action.Arguments[0]).ParsedValue.ToString());
		}

		private void ReadJournalEmailAddress(string addressString)
		{
			SmtpAddress smtpAddress = SmtpAddress.Parse(addressString);
			if (!smtpAddress.IsValidAddress)
			{
				throw new JournalRuleCorruptException();
			}
			this.journalEmailAddress = smtpAddress;
		}

		private bool TryReadScopeCondition(Microsoft.Exchange.MessagingPolicies.Rules.Condition condition)
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return false;
			}
			if (((PredicateCondition)condition).Name != "equal")
			{
				return false;
			}
			string name = ((PredicateCondition)condition).Property.Name;
			if (name == "Microsoft.Exchange.Journaling.External")
			{
				this.Scope = JournalRuleScope.External;
				return true;
			}
			if (name == "Microsoft.Exchange.Journaling.Internal")
			{
				this.Scope = JournalRuleScope.Internal;
				return true;
			}
			return false;
		}

		private bool TryReadRecipientCondition(Microsoft.Exchange.MessagingPolicies.Rules.Condition condition)
		{
			if (condition.ConditionType != ConditionType.Or)
			{
				return false;
			}
			List<Microsoft.Exchange.MessagingPolicies.Rules.Condition> subConditions = ((OrCondition)condition).SubConditions;
			if (subConditions.Count != 2)
			{
				return false;
			}
			foreach (Microsoft.Exchange.MessagingPolicies.Rules.Condition condition2 in subConditions)
			{
				if (condition2.ConditionType != ConditionType.Or)
				{
					return false;
				}
				List<Microsoft.Exchange.MessagingPolicies.Rules.Condition> subConditions2 = ((OrCondition)condition2).SubConditions;
				using (List<Microsoft.Exchange.MessagingPolicies.Rules.Condition>.Enumerator enumerator2 = subConditions2.GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						Microsoft.Exchange.MessagingPolicies.Rules.Condition condition3 = enumerator2.Current;
						if (condition3.ConditionType != ConditionType.Predicate)
						{
							return false;
						}
						PredicateCondition predicateCondition = condition3 as PredicateCondition;
						if (!predicateCondition.Name.Equals("isMemberOf") && !predicateCondition.Name.Equals("isSameUser"))
						{
							return false;
						}
						if (!predicateCondition.Property.Name.Equals("Message.Sender") && !predicateCondition.Property.Name.Equals("Message.EnvelopeRecipients"))
						{
							return false;
						}
						this.recipient = new SmtpAddress?(new SmtpAddress(predicateCondition.Value.RawValues[0]));
						return true;
					}
				}
			}
			return false;
		}

		private Microsoft.Exchange.MessagingPolicies.Rules.Condition CreateScopeCondition()
		{
			if (this.ruleScope == JournalRuleScope.Global)
			{
				return null;
			}
			string propertyNameToCheck = (this.ruleScope == JournalRuleScope.External) ? "Microsoft.Exchange.Journaling.External" : "Microsoft.Exchange.Journaling.Internal";
			return this.CreatePropertyEqualsOneCheck(propertyNameToCheck);
		}

		private Microsoft.Exchange.MessagingPolicies.Rules.Condition CreatePropertyEqualsOneCheck(string propertyNameToCheck)
		{
			Property property = JournalingRuleParser.Instance.CreateProperty("Message.ExtendedProperties:" + propertyNameToCheck, "integer");
			ShortList<string> shortList = new ShortList<string>();
			shortList.Add("1");
			return JournalingRuleParser.Instance.CreatePredicate("equal", property, shortList, new RulesCreationContext());
		}

		private Microsoft.Exchange.MessagingPolicies.Rules.Condition CreateRecipientCondition()
		{
			if (this.recipient == null)
			{
				return null;
			}
			Property property = JournalingRuleParser.Instance.CreateProperty("Message.Sender");
			ShortList<string> shortList = new ShortList<string>();
			shortList.Add(this.recipient.ToString());
			PredicateCondition predicateCondition = JournalingRuleParser.Instance.CreatePredicate("isMemberOf", property, shortList);
			PredicateCondition predicateCondition2 = JournalingRuleParser.Instance.CreatePredicate("isSameUser", property, shortList);
			Microsoft.Exchange.MessagingPolicies.Rules.Condition condition = this.CombineUsingOr(new Microsoft.Exchange.MessagingPolicies.Rules.Condition[]
			{
				predicateCondition,
				predicateCondition2
			});
			Property property2 = JournalingRuleParser.Instance.CreateProperty("Message.EnvelopeRecipients");
			PredicateCondition predicateCondition3 = JournalingRuleParser.Instance.CreatePredicate("isMemberOf", property2, shortList);
			PredicateCondition predicateCondition4 = JournalingRuleParser.Instance.CreatePredicate("isSameUser", property2, shortList);
			Microsoft.Exchange.MessagingPolicies.Rules.Condition condition2 = this.CombineUsingOr(new Microsoft.Exchange.MessagingPolicies.Rules.Condition[]
			{
				predicateCondition3,
				predicateCondition4
			});
			return this.CombineUsingOr(new Microsoft.Exchange.MessagingPolicies.Rules.Condition[]
			{
				condition,
				condition2
			});
		}

		private Microsoft.Exchange.MessagingPolicies.Rules.Condition CombineUsingAnd(params Microsoft.Exchange.MessagingPolicies.Rules.Condition[] conditions)
		{
			int num = 0;
			Microsoft.Exchange.MessagingPolicies.Rules.Condition result = null;
			foreach (Microsoft.Exchange.MessagingPolicies.Rules.Condition condition in conditions)
			{
				if (condition != null)
				{
					num++;
					result = condition;
				}
			}
			if (num == 0)
			{
				return Microsoft.Exchange.MessagingPolicies.Rules.Condition.True;
			}
			if (num == 1)
			{
				return result;
			}
			AndCondition andCondition = new AndCondition();
			foreach (Microsoft.Exchange.MessagingPolicies.Rules.Condition item in conditions)
			{
				andCondition.SubConditions.Add(item);
			}
			return andCondition;
		}

		private Microsoft.Exchange.MessagingPolicies.Rules.Action CreateAction()
		{
			Value item = new Value(this.journalEmailAddress.ToString());
			ShortList<Argument> shortList = new ShortList<Argument>();
			shortList.Add(item);
			return JournalingRuleParser.Instance.CreateAction("Journal", shortList, null);
		}

		private OrCondition CombineUsingOr(params Microsoft.Exchange.MessagingPolicies.Rules.Condition[] conditions)
		{
			OrCondition orCondition = new OrCondition();
			foreach (Microsoft.Exchange.MessagingPolicies.Rules.Condition item in conditions)
			{
				orCondition.SubConditions.Add(item);
			}
			return orCondition;
		}

		public override ValidationError[] Validate()
		{
			if (!this.isValid)
			{
				return new ValidationError[]
				{
					new ObjectValidationError(this.errorText, base.Identity, null)
				};
			}
			List<ValidationError> list = new List<ValidationError>();
			ValidationError item;
			if ((item = RulePresentationObjectBaseSchema.Name.ValidateValue(base.Name, false)) != null)
			{
				list.Add(item);
			}
			if (!this.journalEmailAddress.IsValidAddress || this.journalEmailAddress.Equals(SmtpAddress.Empty))
			{
				list.Add(new PropertyValidationError(Strings.NotValidEmailAddress(this.journalEmailAddress.ToString()), JournalRuleObjectSchema.JournalEmailAddress, this.JournalEmailAddress));
			}
			return list.ToArray();
		}

		private const string SenderProperty = "Message.Sender";

		private const string ToProperty = "Message.EnvelopeRecipients";

		private const string ExtendedProperyBag = "Message.ExtendedProperties";

		public const string ExternalFlagProperty = "Microsoft.Exchange.Journaling.External";

		public const string InternalFlagProperty = "Microsoft.Exchange.Journaling.Internal";

		private static readonly bool isJournalAddressCheckEnabled = JournalRuleObject.IsJournalAddressCheckEnabled();

		private static JournalRuleObjectSchema schema = ObjectSchema.GetInstance<JournalRuleObjectSchema>();

		private SmtpAddress? recipient;

		private SmtpAddress journalEmailAddress;

		private LocalizedString errorText = LocalizedString.Empty;

		private bool enabled;

		private JournalRuleScope ruleScope;

		private DateTime? expiryDate;

		private JournalRuleType ruleType;
	}
}
