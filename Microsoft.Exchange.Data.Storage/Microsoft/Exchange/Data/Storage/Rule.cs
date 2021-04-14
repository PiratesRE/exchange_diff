using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Rule
	{
		private Rule(Folder folder, Rule serverRule)
		{
			this.serverRule = serverRule;
			this.folder = folder;
			this.name = string.Empty;
			this.ruleId = null;
			this.isEnabled = true;
			this.sequence = 0;
			this.provider = "RuleOrganizer";
			this.providerId = Rule.ProviderIdEnum.OL98Plus;
			this.onlyOof = false;
			this.isInError = false;
			this.isNotSupported = false;
			this.actions = new ActionList(this);
			this.conditions = new ConditionList(this);
			this.exceptions = new ConditionList(this);
			this.toParticipants = new List<Participant>();
			this.fromParticipants = new List<Participant>();
			this.containsSubjectStrings = new List<string>();
			this.containsBodyStrings = new List<string>();
			this.containsSubjectOrBodyStrings = new List<string>();
			this.containsSenderStrings = new List<string>();
			this.containsHeaderStrings = new List<string>();
			this.containsRecipientStrings = new List<string>();
			this.assignedCategoriesStrings = new List<string>();
			this.messageClassificationStrings = new List<string>();
			this.exceptToParticipants = new List<Participant>();
			this.exceptFromParticipants = new List<Participant>();
			this.exceptSubjectStrings = new List<string>();
			this.exceptBodyStrings = new List<string>();
			this.exceptSubjectOrBodyStrings = new List<string>();
			this.exceptSenderStrings = new List<string>();
			this.exceptRecipientStrings = new List<string>();
			this.exceptHeaderStrings = new List<string>();
			this.exceptCategoriesStrings = new List<string>();
			this.exceptMessageClassificationStrings = new List<string>();
			if (serverRule != null)
			{
				this.ParseServerRule();
				this.isNew = false;
				this.ClearDirty();
				return;
			}
			this.isNew = true;
			this.SetDirty();
		}

		public static Rule Create(Rules rules)
		{
			return new Rule(rules.Folder, null);
		}

		internal static Rule Create(Folder folder, Rule serverRule)
		{
			return new Rule(folder, serverRule);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Name");
				}
				this.name = value;
				this.SetDirty();
			}
		}

		public RuleId Id
		{
			get
			{
				return this.ruleId;
			}
			internal set
			{
				this.ruleId = value;
				this.SetDirty();
			}
		}

		public string Provider
		{
			get
			{
				return this.provider;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Provider");
				}
				this.provider = value;
				this.SetDirty();
			}
		}

		public bool IsEnabled
		{
			get
			{
				return this.isEnabled;
			}
			set
			{
				this.isEnabled = value;
				this.SetDirty();
			}
		}

		public bool RunOnlyWhileOof
		{
			get
			{
				return this.onlyOof;
			}
			set
			{
				this.onlyOof = value;
				this.SetDirty();
			}
		}

		public bool IsParameterInError
		{
			get
			{
				return this.isInError;
			}
			set
			{
				this.isInError = value;
				this.SetDirty();
			}
		}

		public bool IsNotSupported
		{
			get
			{
				return this.isNotSupported;
			}
		}

		public IList<ActionBase> Actions
		{
			get
			{
				return this.actions;
			}
		}

		public IList<Condition> Conditions
		{
			get
			{
				return this.conditions;
			}
		}

		public IList<Condition> Exceptions
		{
			get
			{
				return this.exceptions;
			}
		}

		public QueryFilter ConditionFilter
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int Sequence
		{
			get
			{
				return this.sequence;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("Sequence");
				}
				this.sequence = value;
				this.SetDirty();
			}
		}

		public bool IsNew
		{
			get
			{
				return this.isNew;
			}
		}

		public bool IsDirty
		{
			get
			{
				return this.isDirty;
			}
		}

		public Rule.ProviderIdEnum ProviderId
		{
			get
			{
				return this.providerId;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<Rule.ProviderIdEnum>(value);
				this.providerId = value;
			}
		}

		public void Save()
		{
			if (this.IsNotSupported)
			{
				throw new NotSupportedException();
			}
			if (this.IsNew)
			{
				this.serverRule = new Rule();
				this.serverRule.Operation = RuleOperation.Create;
			}
			else if (this.isDirty)
			{
				this.serverRule.Operation = RuleOperation.Update;
			}
			Rule.ProviderData providerData;
			providerData.Version = 1U;
			providerData.RuleSearchKey = 0U;
			providerData.TimeStamp = ExDateTime.UtcNow.ToFileTime();
			this.serverRule.ProviderData = providerData.ToByteArray();
			this.serverRule.ExecutionSequence = this.Sequence;
			this.serverRule.Name = this.Name;
			this.serverRule.Provider = this.Provider;
			FromRecipientsCondition fromRecipientsCondition = null;
			for (int i = 0; i < this.Exceptions.Count; i++)
			{
				if (this.Exceptions[i].ConditionType == ConditionType.FromRecipientsCondition)
				{
					fromRecipientsCondition = (FromRecipientsCondition)this.Exceptions[i];
					this.Exceptions.RemoveAt(i);
					break;
				}
			}
			Condition[] array = this.conditions.ToArray();
			Condition[] array2 = this.exceptions.ToArray();
			Array.Sort(array, Condition.ConditionOrderComparer);
			Array.Sort(array2, Condition.ExceptionOrderComparer);
			List<Restriction> list = new List<Restriction>();
			if (fromRecipientsCondition != null)
			{
				list.Add(Restriction.Not(fromRecipientsCondition.BuildRestriction()));
			}
			for (int j = 0; j < array.Length; j++)
			{
				Restriction restriction = array[j].BuildRestriction();
				if (restriction != null)
				{
					list.Add(restriction);
				}
			}
			for (int k = 0; k < array2.Length; k++)
			{
				Restriction restriction2 = array2[k].BuildRestriction();
				if (restriction2 != null)
				{
					list.Add(Restriction.Not(restriction2));
				}
			}
			Restriction condition;
			if (list.Count == 0)
			{
				condition = Restriction.Exist(PropTag.MessageClass);
			}
			else if (list.Count > 1)
			{
				condition = Restriction.And(list.ToArray());
			}
			else
			{
				condition = list[0];
			}
			this.serverRule.Condition = condition;
			uint num = (uint)this.serverRule.StateFlags;
			num &= 4294967279U;
			ActionBase[] array3 = this.actions.ToArray();
			Array.Sort(array3, ActionBase.ActionOrderComparer);
			List<RuleAction> list2 = new List<RuleAction>();
			for (int l = 0; l < array3.Length; l++)
			{
				RuleAction ruleAction = array3[l].BuildRuleAction();
				if (ruleAction != null)
				{
					list2.Add(ruleAction);
				}
				if (array3[l].ActionType == ActionType.DeleteAction || array3[l].ActionType == ActionType.PermanentDeleteAction || ((IList<ActionBase>)this.actions)[l].ActionType == ActionType.StopProcessingAction)
				{
					num |= 16U;
				}
			}
			this.serverRule.Actions = list2.ToArray();
			if (this.IsEnabled)
			{
				num |= 1U;
			}
			else
			{
				num &= 4294967294U;
			}
			if (this.RunOnlyWhileOof)
			{
				num |= 4U;
			}
			else
			{
				num &= 4294967291U;
			}
			if (this.isInError)
			{
				num |= 2U;
			}
			else
			{
				num &= 4294967293U;
			}
			this.serverRule.StateFlags = (RuleStateFlags)num;
		}

		public void SaveNotSupported()
		{
			if (!this.isNotSupported)
			{
				throw new InvalidOperationException("SaveNotSupported() should only be called on a not supported rule.");
			}
			if (this.isNew)
			{
				throw new InvalidOperationException("Not supported rule cannot be created.");
			}
			if (this.serverRule == null)
			{
				throw new InvalidOperationException("The not supported rule has not been loaded.");
			}
			if (this.isDirty)
			{
				this.serverRule.ExecutionSequence = this.sequence;
				this.serverRule.Name = this.name;
				if (this.isEnabled)
				{
					this.serverRule.StateFlags |= RuleStateFlags.Enabled;
				}
				else
				{
					this.serverRule.StateFlags &= ~RuleStateFlags.Enabled;
				}
				this.serverRule.Operation = RuleOperation.Update;
			}
		}

		public void MarkDelete()
		{
			if (this.isNew)
			{
				throw new InvalidOperationException("Cannot delete a new rule that does not exist in store yet.");
			}
			if (this.serverRule == null)
			{
				throw new InvalidOperationException("Cannot delete a rule that does not exist in store or has not been loaded from store yet.");
			}
			this.serverRule.Operation = RuleOperation.Delete;
			this.SetDirty();
		}

		private static void ThrowRuleParseIfNull(object value)
		{
			if (value == null)
			{
				throw new RuleParseException(ServerStrings.UnsupportedPropertyRestriction);
			}
		}

		private static void ThrowRuleParseIfNullOrEmpty(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new RuleParseException(ServerStrings.UnsupportedPropertyRestriction);
			}
		}

		private static bool IsSenderRst(Restriction res)
		{
			Restriction.CommentRestriction commentRestriction = res as Restriction.CommentRestriction;
			if (commentRestriction == null)
			{
				throw new ArgumentException();
			}
			Restriction.PropertyRestriction propertyRestriction;
			if (commentRestriction.Restriction is Restriction.OrRestriction)
			{
				Restriction.OrRestriction orRestriction = commentRestriction.Restriction as Restriction.OrRestriction;
				propertyRestriction = (orRestriction.Restrictions[0] as Restriction.PropertyRestriction);
			}
			else
			{
				propertyRestriction = (commentRestriction.Restriction as Restriction.PropertyRestriction);
			}
			PropTag propTag = propertyRestriction.PropTag;
			if (propTag == PropTag.SenderSearchKey || propTag == PropTag.SenderEntryId)
			{
				return true;
			}
			if (propTag == PropTag.SearchKey || propTag == (PropTag)65794U)
			{
				return false;
			}
			throw new ArgumentException();
		}

		private static void ParticipantFromRestriction(Restriction res, IList<Participant> participants)
		{
			if (res is Restriction.ContentRestriction)
			{
				Restriction.ContentRestriction contentRestriction = res as Restriction.ContentRestriction;
				string displayName = (string)contentRestriction.PropValue.Value;
				participants.Add(new Participant(displayName, null, null));
				return;
			}
			if (res is Restriction.CommentRestriction)
			{
				Restriction.CommentRestriction commentRestriction = res as Restriction.CommentRestriction;
				PropValue[] values = commentRestriction.Values;
				object obj = null;
				if (commentRestriction.Restriction is Restriction.PropertyRestriction)
				{
					Restriction.PropertyRestriction propertyRestriction = commentRestriction.Restriction as Restriction.PropertyRestriction;
					obj = propertyRestriction.PropValue.Value;
				}
				else
				{
					Restriction.OrRestriction orRestriction = commentRestriction.Restriction as Restriction.OrRestriction;
					for (long num = 0L; num < (long)orRestriction.Restrictions.Length; num += 1L)
					{
						Restriction.PropertyRestriction propertyRestriction = orRestriction.Restrictions[(int)(checked((IntPtr)num))] as Restriction.PropertyRestriction;
						if (propertyRestriction.PropTag == PropTag.SenderSearchKey)
						{
							obj = propertyRestriction.PropValue.Value;
							break;
						}
					}
				}
				participants.Add(Rule.CreateParticipant((string)values[2].Value, (byte[])values[1].Value, (byte[])obj));
			}
		}

		private static Participant CreateParticipant(string displayName, byte[] entryIdBytes, byte[] searchKey)
		{
			Participant.Builder builder = new Participant.Builder();
			ParticipantEntryId participantEntryId = null;
			if (entryIdBytes != null)
			{
				participantEntryId = ParticipantEntryId.TryFromEntryId(entryIdBytes);
				if (participantEntryId != null)
				{
					builder.SetPropertiesFrom(participantEntryId);
				}
			}
			if (displayName != null)
			{
				builder.DisplayName = displayName;
			}
			if (builder.DisplayName == null)
			{
				builder.DisplayName = ClientStrings.UnknownDelegateUser;
			}
			StoreParticipantEntryId storeParticipantEntryId = participantEntryId as StoreParticipantEntryId;
			if (storeParticipantEntryId != null)
			{
				if (storeParticipantEntryId.IsDL == true)
				{
					throw new RuleParseException(ServerStrings.NoMapiPDLs);
				}
				if (searchKey != null)
				{
					string text = null;
					string text2 = null;
					Rule.ParseSearchKey(searchKey, out text, out text2);
					if (text != null)
					{
						builder.RoutingType = text;
					}
					if (text2 != null)
					{
						builder.EmailAddress = text2;
					}
				}
			}
			return builder.ToParticipant();
		}

		private static string ConvertSearchKeyValue(byte[] value)
		{
			string text = new string(CTSGlobals.AsciiEncoding.GetChars(value, 0, value.Length));
			string text2 = text;
			char[] trimChars = new char[1];
			return text2.TrimEnd(trimChars);
		}

		private static void ParseSearchKey(byte[] searchKeyBytes, out string routingType, out string emailaddress)
		{
			string text = Rule.ConvertSearchKeyValue(searchKeyBytes);
			string[] array = text.Split(new char[]
			{
				':'
			}, 2);
			if (array.Length >= 2)
			{
				routingType = array[0];
				emailaddress = array[1];
				return;
			}
			routingType = null;
			emailaddress = null;
		}

		private static void CrackAddressList(IList<Participant> participants, params Restriction[] resArray)
		{
			for (int i = 0; i < resArray.Length; i++)
			{
				Rule.ParticipantFromRestriction(resArray[i], participants);
			}
		}

		private static byte[] SearchKeyFromParticipant(Participant participant)
		{
			if (participant.EmailAddress == null || participant.RoutingType == null)
			{
				throw new InvalidParticipantException(ServerStrings.InvalidParticipantForRules, ParticipantValidationStatus.AddressAndRoutingTypeMismatch);
			}
			if (participant.EmailAddress.Length == 0 || participant.RoutingType.Length == 0)
			{
				throw new InvalidParticipantException(ServerStrings.InvalidParticipantForRules, ParticipantValidationStatus.AddressAndRoutingTypeMismatch);
			}
			if (participant.RoutingType == "MAPIPDL")
			{
				ParticipantEntryId participantEntryId = ParticipantEntryId.FromParticipant(participant, ParticipantEntryIdConsumer.RecipientTableSecondary);
				return participantEntryId.ToByteArray();
			}
			string text = participant.RoutingType + ":" + participant.EmailAddress + "\0";
			text = text.ToUpperInvariant();
			return CTSGlobals.AsciiEncoding.GetBytes(text);
		}

		private static Restriction CommentSearchKey(PropTag tagProp, byte[] searchKey, byte[] entryId, string displayName, LegacyRecipientDisplayType displayType, Restriction.RelOp relOp)
		{
			PropValue[] propValues = new PropValue[]
			{
				new PropValue((PropTag)1610612739U, Rule.InboxSpecialComment.Resolved),
				new PropValue((PropTag)65794U, entryId),
				new PropValue((PropTag)65567U, displayName),
				new PropValue(PropTag.DisplayType, displayType)
			};
			Restriction restriction;
			if (relOp == Restriction.RelOp.MemberOfDL)
			{
				restriction = Restriction.Or(new Restriction[]
				{
					Restriction.EQ(PropTag.SenderSearchKey, searchKey),
					Restriction.MemberOf(PropTag.SenderEntryId, entryId)
				});
			}
			else
			{
				restriction = Restriction.EQ(tagProp, searchKey);
			}
			return Restriction.Comment(restriction, propValues);
		}

		private static Restriction ContentOrPropertyFromParticipant(Participant participant, PropTag tagDisplayName)
		{
			ParticipantEntryId participantEntryId = ParticipantEntryId.FromParticipant(participant, ParticipantEntryIdConsumer.RecipientTableSecondary);
			if (participantEntryId == null && participant.DisplayName != null)
			{
				return Restriction.Content(tagDisplayName, participant.DisplayName, ContentFlags.SubString | ContentFlags.IgnoreCase | ContentFlags.Loose);
			}
			Restriction.RelOp relOp = Restriction.RelOp.Equal;
			PropTag propTag = (tagDisplayName == PropTag.SenderName) ? PropTag.SenderSearchKey : PropTag.SearchKey;
			byte[] entryId = participantEntryId.ToByteArray();
			LegacyRecipientDisplayType displayType = (participantEntryId.IsDL == true) ? LegacyRecipientDisplayType.DistributionList : LegacyRecipientDisplayType.MailUser;
			if (propTag == PropTag.SenderSearchKey && participantEntryId.IsDL == true)
			{
				relOp = Restriction.RelOp.MemberOfDL;
				propTag = PropTag.SenderEntryId;
			}
			byte[] searchKey = Rule.SearchKeyFromParticipant(participant);
			return Rule.CommentSearchKey(propTag, searchKey, entryId, participant.DisplayName, displayType, relOp);
		}

		private NativeStorePropertyDefinition PropTagToPropertyDefinitionFromCache(PropTag propTag)
		{
			NativeStorePropertyDefinition[] array = PropertyTagCache.Cache.PropertyDefinitionsFromPropTags(NativeStorePropertyDefinition.TypeCheckingFlag.DoNotCreateInvalidType, this.folder.MapiFolder, this.folder.Session, new PropTag[]
			{
				propTag
			});
			if (array == null || array.Length == 0)
			{
				throw new RuleParseException(ServerStrings.MapiCannotGetNamedProperties);
			}
			return array[0];
		}

		private bool IsNdrRestrictionSet(Restriction.AndRestriction resAnd)
		{
			if (2 != resAnd.Restrictions.Length)
			{
				return false;
			}
			Restriction.ContentRestriction contentRestriction = resAnd.Restrictions[0] as Restriction.ContentRestriction;
			if (contentRestriction == null || PropTag.MessageClass != contentRestriction.PropTag)
			{
				return false;
			}
			string value = contentRestriction.PropValue.Value as string;
			if (!"REPORT".Equals(value, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (ContentFlags.Prefix != (ContentFlags.Prefix & contentRestriction.Flags))
			{
				return false;
			}
			contentRestriction = (resAnd.Restrictions[1] as Restriction.ContentRestriction);
			if (contentRestriction == null || PropTag.MessageClass != contentRestriction.PropTag)
			{
				return false;
			}
			value = (contentRestriction.PropValue.Value as string);
			string text = ".NDR";
			return text.Equals(value, StringComparison.OrdinalIgnoreCase) && ContentFlags.SubString == (ContentFlags.SubString & contentRestriction.Flags);
		}

		private bool IsFlaggedRestrictionSet(Restriction.AndRestriction resAnd)
		{
			if (2 != resAnd.Restrictions.Length)
			{
				return false;
			}
			Restriction.PropertyRestriction propertyRestriction = resAnd.Restrictions[0] as Restriction.PropertyRestriction;
			if (propertyRestriction == null || (PropTag)277872643U != propertyRestriction.PropTag)
			{
				return false;
			}
			if (propertyRestriction.PropValue.GetInt() != 2)
			{
				return false;
			}
			Restriction.PropertyRestriction propertyRestriction2 = resAnd.Restrictions[1] as Restriction.PropertyRestriction;
			if (propertyRestriction2 != null)
			{
				GuidIdPropertyDefinition guidIdPropertyDefinition = this.PropTagToPropertyDefinitionFromCache(propertyRestriction2.PropTag) as GuidIdPropertyDefinition;
				return guidIdPropertyDefinition != null && guidIdPropertyDefinition.Equals(ItemSchema.FlagRequest);
			}
			return false;
		}

		private void AddElementFromProp(PropTag propTag, object value, int op, bool isException)
		{
			Rule.ThrowRuleParseIfNull(value);
			if (propTag <= PropTag.TransportMessageHeaders)
			{
				if (propTag <= PropTag.Sensitivity)
				{
					if (propTag <= PropTag.Importance)
					{
						if (propTag != PropTag.AutoForwarded)
						{
							if (propTag != PropTag.Importance)
							{
								goto IL_51D;
							}
							if (isException)
							{
								this.Exceptions.Add(MarkedAsImportanceCondition.Create(this, (Importance)value));
								return;
							}
							this.Conditions.Add(MarkedAsImportanceCondition.Create(this, (Importance)value));
							return;
						}
						else
						{
							if (isException)
							{
								this.Exceptions.Add(AutomaticForwardCondition.Create(this));
								return;
							}
							this.Conditions.Add(AutomaticForwardCondition.Create(this));
							return;
						}
					}
					else if (propTag != PropTag.MessageClass)
					{
						if (propTag != PropTag.Sensitivity)
						{
							goto IL_51D;
						}
						if (isException)
						{
							this.Exceptions.Add(MarkedAsSensitivityCondition.Create(this, (Sensitivity)value));
							return;
						}
						this.Conditions.Add(MarkedAsSensitivityCondition.Create(this, (Sensitivity)value));
						return;
					}
					else
					{
						string value2 = value as string;
						Rule.ThrowRuleParseIfNullOrEmpty(value2);
						if (!"IPM.Note.Rules.OofTemplate.Microsoft".Equals(value2, StringComparison.OrdinalIgnoreCase))
						{
							return;
						}
						if (isException)
						{
							this.Exceptions.Add(MarkedAsOofCondition.Create(this));
							return;
						}
						this.Conditions.Add(MarkedAsOofCondition.Create(this));
						return;
					}
				}
				else if (propTag <= PropTag.MessageToMe)
				{
					if (propTag != PropTag.Subject)
					{
						if (propTag != PropTag.MessageToMe)
						{
							goto IL_51D;
						}
						if (isException)
						{
							if (!(bool)value)
							{
								this.Exceptions.Add(NotSentToMeCondition.Create(this));
								return;
							}
							this.Exceptions.Add(SentToMeCondition.Create(this));
							return;
						}
						else
						{
							if (!(bool)value)
							{
								this.Conditions.Add(NotSentToMeCondition.Create(this));
								return;
							}
							this.Conditions.Add(SentToMeCondition.Create(this));
							return;
						}
					}
				}
				else if (propTag != PropTag.MessageCcMe)
				{
					if (propTag != PropTag.MessageRecipMe)
					{
						if (propTag != PropTag.TransportMessageHeaders)
						{
							goto IL_51D;
						}
						string text = value as string;
						Rule.ThrowRuleParseIfNull(text);
						if (isException)
						{
							this.exceptHeaderStrings.Add(text);
							return;
						}
						this.containsHeaderStrings.Add(text);
						return;
					}
					else
					{
						if (isException)
						{
							this.Exceptions.Add(SentToOrCcMeCondition.Create(this));
							return;
						}
						this.Conditions.Add(SentToOrCcMeCondition.Create(this));
						return;
					}
				}
				else
				{
					if (isException)
					{
						this.Exceptions.Add(SentCcMeCondition.Create(this));
						return;
					}
					this.Conditions.Add(SentCcMeCondition.Create(this));
					return;
				}
			}
			else if (propTag <= PropTag.MessageSize)
			{
				if (propTag <= PropTag.SenderName)
				{
					if (propTag != PropTag.RecipientType && propTag != PropTag.SenderName)
					{
						goto IL_51D;
					}
					return;
				}
				else if (propTag != PropTag.SenderSearchKey)
				{
					if (propTag != PropTag.MessageDeliveryTime)
					{
						if (propTag != PropTag.MessageSize)
						{
							goto IL_51D;
						}
						if (op == 3 || op == 2)
						{
							if (isException)
							{
								this.exceptLowRange = new int?(((int)value + 1023) / 1024);
								return;
							}
							this.lowRange = new int?(((int)value + 1023) / 1024);
							return;
						}
						else
						{
							if (op != 1 && op != 0)
							{
								return;
							}
							if (isException)
							{
								this.exceptHighRange = new int?((int)value / 1024);
								return;
							}
							this.highRange = new int?((int)value / 1024);
							return;
						}
					}
					else
					{
						ExDateTime exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, (DateTime)value);
						exDateTime = this.Folder.Session.ExTimeZone.ConvertDateTime(exDateTime);
						if (op == 3 || op == 2)
						{
							if (isException)
							{
								this.exceptAfter = new ExDateTime?(exDateTime);
								return;
							}
							this.after = new ExDateTime?(exDateTime);
							return;
						}
						else
						{
							if (op != 1 && op != 0)
							{
								return;
							}
							if (isException)
							{
								this.exceptBefore = new ExDateTime?(exDateTime);
								return;
							}
							this.before = new ExDateTime?(exDateTime);
							return;
						}
					}
				}
				else
				{
					if (op != 1)
					{
						return;
					}
					byte[] value3 = value as byte[];
					Rule.ThrowRuleParseIfNull(value3);
					string item = Rule.ConvertSearchKeyValue(value3);
					if (isException)
					{
						this.exceptSenderStrings.Add(item);
						return;
					}
					this.containsSenderStrings.Add(item);
					return;
				}
			}
			else if (propTag <= PropTag.Body)
			{
				if (propTag != PropTag.NormalizedSubject)
				{
					if (propTag != PropTag.Body)
					{
						goto IL_51D;
					}
					string text2 = value as string;
					Rule.ThrowRuleParseIfNull(text2);
					if (isException)
					{
						this.exceptBodyStrings.Add(text2);
						return;
					}
					this.containsBodyStrings.Add(text2);
					return;
				}
			}
			else if (propTag != (PropTag)277872643U)
			{
				if (propTag == PropTag.DisplayName)
				{
					return;
				}
				if (propTag != PropTag.SearchKey)
				{
					goto IL_51D;
				}
				if (op != 1)
				{
					return;
				}
				byte[] value4 = value as byte[];
				Rule.ThrowRuleParseIfNull(value4);
				string item2 = Rule.ConvertSearchKeyValue(value4);
				if (isException)
				{
					this.exceptRecipientStrings.Add(item2);
					return;
				}
				this.containsRecipientStrings.Add(item2);
				return;
			}
			else
			{
				if ((int)value != 2)
				{
					return;
				}
				string action = RequestedAction.Any.ToString();
				if (isException)
				{
					this.Exceptions.Add(FlaggedForActionCondition.Create(this, action));
					return;
				}
				this.Conditions.Add(FlaggedForActionCondition.Create(this, action));
				return;
			}
			string text3 = value as string;
			Rule.ThrowRuleParseIfNull(text3);
			if (isException)
			{
				this.exceptSubjectStrings.Add(text3);
				return;
			}
			this.containsSubjectStrings.Add(text3);
			return;
			IL_51D:
			NativeStorePropertyDefinition nativeStorePropertyDefinition = this.PropTagToPropertyDefinitionFromCache(propTag);
			GuidNamePropertyDefinition guidNamePropertyDefinition = nativeStorePropertyDefinition as GuidNamePropertyDefinition;
			if (guidNamePropertyDefinition != null)
			{
				if (guidNamePropertyDefinition.Equals(ItemSchema.Categories))
				{
					string text4 = value as string;
					Rule.ThrowRuleParseIfNull(text4);
					if (isException)
					{
						this.exceptCategoriesStrings.Add(text4);
						return;
					}
					this.assignedCategoriesStrings.Add(text4);
					return;
				}
				else if (guidNamePropertyDefinition.Equals(MessageItemSchema.IsReadReceipt))
				{
					bool flag = (bool)value;
					if (!flag)
					{
						return;
					}
					if (isException)
					{
						this.Exceptions.Add(ReadReceiptCondition.Create(this));
						return;
					}
					this.Conditions.Add(ReadReceiptCondition.Create(this));
					return;
				}
				else if (guidNamePropertyDefinition.Equals(MessageItemSchema.IsSigned))
				{
					bool flag2 = (bool)value;
					if (!flag2)
					{
						return;
					}
					if (isException)
					{
						this.Exceptions.Add(SignedCondition.Create(this));
						return;
					}
					this.Conditions.Add(SignedCondition.Create(this));
					return;
				}
			}
			else
			{
				GuidIdPropertyDefinition guidIdPropertyDefinition = nativeStorePropertyDefinition as GuidIdPropertyDefinition;
				if (guidIdPropertyDefinition != null)
				{
					if (guidIdPropertyDefinition.Equals(ItemSchema.IsClassified))
					{
						if (isException)
						{
							this.isMessageClassificationException = true;
							return;
						}
						this.isMessageClassificationCondition = true;
						return;
					}
					else if (guidIdPropertyDefinition.Equals(ItemSchema.ClassificationGuid))
					{
						string text5 = value as string;
						Rule.ThrowRuleParseIfNull(text5);
						if (isException)
						{
							this.exceptMessageClassificationStrings.Add(text5);
							return;
						}
						this.messageClassificationStrings.Add(text5);
						return;
					}
					else if (guidIdPropertyDefinition.Equals(ItemSchema.RequestedAction))
					{
						RequestedAction requestedAction = (RequestedAction)((int)value);
						string action2 = (requestedAction == RequestedAction.Any) ? RequestedAction.Any.ToString() : LocalizedDescriptionAttribute.FromEnum(FlaggedForActionCondition.RequestedActionType, requestedAction);
						if (isException)
						{
							this.Exceptions.Add(FlaggedForActionCondition.Create(this, action2));
							return;
						}
						this.Conditions.Add(FlaggedForActionCondition.Create(this, action2));
						return;
					}
					else if (guidIdPropertyDefinition.Equals(MessageItemSchema.SharingInstanceGuid))
					{
						Guid guid = (Guid)value;
						if (isException)
						{
							this.Exceptions.Add(FromSubscriptionCondition.Create(this, new Guid[]
							{
								guid
							}));
							return;
						}
						this.Conditions.Add(FromSubscriptionCondition.Create(this, new Guid[]
						{
							guid
						}));
						return;
					}
				}
			}
			throw new RuleParseException(ServerStrings.UnsupportedPropertyRestriction);
		}

		private void AddSenderElement(bool isException, IList<Participant> participants)
		{
			foreach (Participant item in participants)
			{
				if (isException)
				{
					this.exceptFromParticipants.Add(item);
				}
				else
				{
					this.fromParticipants.Add(item);
				}
			}
		}

		private void AddRecipientElement(bool isException, IList<Participant> participants)
		{
			foreach (Participant item in participants)
			{
				if (isException)
				{
					this.exceptToParticipants.Add(item);
				}
				else
				{
					this.toParticipants.Add(item);
				}
			}
		}

		private void AddElementFromBitmaskProp(PropTag propTag, int mask, Restriction.RelBmr relation, bool isException)
		{
			if (propTag != PropTag.MessageFlags)
			{
				return;
			}
			if ((mask & 16) > 0)
			{
				if (relation == Restriction.RelBmr.NotEqualToZero)
				{
					if (isException)
					{
						this.Exceptions.Add(HasAttachmentCondition.Create(this));
						return;
					}
					this.Conditions.Add(HasAttachmentCondition.Create(this));
					return;
				}
				else
				{
					if (isException)
					{
						this.Conditions.Add(HasAttachmentCondition.Create(this));
						return;
					}
					this.Exceptions.Add(HasAttachmentCondition.Create(this));
				}
			}
		}

		private void ParseOrRestriction(Restriction.OrRestriction res, bool isException)
		{
			Restriction[] restrictions = res.Restrictions;
			int num = res.Restrictions.Length;
			if (num == 0)
			{
				return;
			}
			if (!(restrictions[0] is Restriction.ContentRestriction))
			{
				if (restrictions[0] is Restriction.CommentRestriction)
				{
					List<Participant> participants = new List<Participant>();
					Rule.CrackAddressList(participants, restrictions);
					try
					{
						if (Rule.IsSenderRst(restrictions[0]))
						{
							this.AddSenderElement(isException, participants);
						}
						else
						{
							this.AddRecipientElement(isException, participants);
						}
						return;
					}
					catch (ArgumentException)
					{
						throw new RuleParseException(ServerStrings.MalformedCommentRestriction);
					}
				}
				if (restrictions[0] is Restriction.PropertyRestriction)
				{
					List<string> list = new List<string>();
					List<Guid> list2 = new List<Guid>();
					int num2 = 0;
					int num3 = 0;
					PropTag propTag = this.PropertyDefinitionToPropTagFromCache(InternalSchema.SharingInstanceGuid);
					for (int i = 0; i < num; i++)
					{
						Restriction.PropertyRestriction propertyRestriction = restrictions[i] as Restriction.PropertyRestriction;
						PropTag propTag2 = propertyRestriction.PropTag;
						if (propTag2 == PropTag.MessageClass)
						{
							list.Add((string)propertyRestriction.PropValue.Value);
						}
						else if (propTag2 == PropTag.MessageCcMe)
						{
							num2++;
						}
						else if (propTag2 == PropTag.MessageToMe)
						{
							num3++;
						}
						else if (propTag2 == propTag)
						{
							list2.Add((Guid)propertyRestriction.PropValue.Value);
						}
					}
					if (num == list.Count)
					{
						if (isException)
						{
							this.Exceptions.Add(FormsCondition.Create(ConditionType.FormsCondition, this, list.ToArray()));
							return;
						}
						this.Conditions.Add(FormsCondition.Create(ConditionType.FormsCondition, this, list.ToArray()));
						return;
					}
					else if (num2 > 0 && num3 > 0)
					{
						if (isException)
						{
							this.Exceptions.Add(SentToOrCcMeCondition.Create(this));
							return;
						}
						this.Conditions.Add(SentToOrCcMeCondition.Create(this));
						return;
					}
					else if (num == list2.Count)
					{
						if (isException)
						{
							this.Exceptions.Add(FromSubscriptionCondition.Create(this, list2.ToArray()));
							return;
						}
						this.Conditions.Add(FromSubscriptionCondition.Create(this, list2.ToArray()));
						return;
					}
				}
				else
				{
					for (int j = 0; j < num; j++)
					{
						this.AddRestrictions(restrictions[j], isException);
					}
				}
				return;
			}
			List<string> list3 = new List<string>();
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			int k = 0;
			while (k < num)
			{
				Restriction.ContentRestriction contentRestriction = restrictions[k] as Restriction.ContentRestriction;
				if (contentRestriction == null)
				{
					throw new RuleParseException(ServerStrings.UnsupportedContentRestriction);
				}
				PropTag propTag2 = contentRestriction.PropTag;
				PropTag propTag3 = propTag2;
				if (propTag3 <= PropTag.SenderName)
				{
					if (propTag3 <= PropTag.Subject)
					{
						if (propTag3 != PropTag.MessageClass)
						{
							if (propTag3 == PropTag.Subject)
							{
								goto IL_152;
							}
						}
						else
						{
							string item = contentRestriction.PropValue.Value as string;
							if (!FormsCondition.FormTypeSet.Contains(item))
							{
								throw new RuleParseException(ServerStrings.UnsupportedFormsCondition);
							}
							list3.Add((string)contentRestriction.PropValue.Value);
						}
					}
					else if (propTag3 != PropTag.TransportMessageHeaders)
					{
						if (propTag3 == PropTag.SenderName)
						{
							num4++;
						}
					}
					else
					{
						num10++;
					}
				}
				else if (propTag3 <= PropTag.NormalizedSubject)
				{
					if (propTag3 != PropTag.SenderSearchKey)
					{
						if (propTag3 == PropTag.NormalizedSubject)
						{
							goto IL_152;
						}
					}
					else
					{
						num8++;
					}
				}
				else if (propTag3 != PropTag.Body)
				{
					if (propTag3 != PropTag.DisplayName)
					{
						if (propTag3 == PropTag.SearchKey)
						{
							num9++;
						}
					}
					else
					{
						num5++;
					}
				}
				else
				{
					num6++;
				}
				IL_188:
				k++;
				continue;
				IL_152:
				num7++;
				goto IL_188;
			}
			if (num == list3.Count)
			{
				if (1 == num)
				{
					string value = list3[0];
					if ("IPM.Note.Rules.OofTemplate.Microsoft".Equals(value, StringComparison.OrdinalIgnoreCase))
					{
						if (isException)
						{
							this.Exceptions.Add(MarkedAsOofCondition.Create(this));
							return;
						}
						this.Conditions.Add(MarkedAsOofCondition.Create(this));
						return;
					}
					else if ("IPM.Note.Microsoft.Approval.Request".Equals(value, StringComparison.OrdinalIgnoreCase))
					{
						if (isException)
						{
							this.Exceptions.Add(ApprovalRequestCondition.Create(this));
							return;
						}
						this.Conditions.Add(ApprovalRequestCondition.Create(this));
						return;
					}
				}
				else if (2 == num)
				{
					if ((list3[0].Equals("IPM.Schedule.Meeting.Request", StringComparison.OrdinalIgnoreCase) && list3[1].Equals("IPM.Schedule.Meeting.Canceled", StringComparison.OrdinalIgnoreCase)) || (list3[0].Equals("IPM.Schedule.Meeting.Canceled", StringComparison.OrdinalIgnoreCase) && list3[1].Equals("IPM.Schedule.Meeting.Request", StringComparison.OrdinalIgnoreCase)))
					{
						if (isException)
						{
							this.Exceptions.Add(MeetingMessageCondition.Create(this));
							return;
						}
						this.Conditions.Add(MeetingMessageCondition.Create(this));
						return;
					}
					else if ((list3[0].Equals("IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA", StringComparison.OrdinalIgnoreCase) && list3[1].Equals("IPM.Note.rpmsg.Microsoft.Voicemail.UM", StringComparison.OrdinalIgnoreCase)) || (list3[0].Equals("IPM.Note.rpmsg.Microsoft.Voicemail.UM", StringComparison.OrdinalIgnoreCase) && list3[1].Equals("IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA", StringComparison.OrdinalIgnoreCase)))
					{
						if (isException)
						{
							this.Exceptions.Add(PermissionControlledCondition.Create(this));
							return;
						}
						this.Conditions.Add(PermissionControlledCondition.Create(this));
						return;
					}
				}
				else if (3 == num)
				{
					if (list3.Contains("IPM.Schedule.Meeting.Resp.Pos") && list3.Contains("IPM.Schedule.Meeting.Resp.Neg") && list3.Contains("IPM.Schedule.Meeting.Resp.Tent"))
					{
						if (isException)
						{
							this.Exceptions.Add(MeetingResponseCondition.Create(this));
							return;
						}
						this.Conditions.Add(MeetingResponseCondition.Create(this));
						return;
					}
					else if (list3.Contains("IPM.Note.Secure") && list3.Contains("IPM.Note" + "." + "SMIME.SignedEncrypted") && list3.Contains("IPM.Note" + "." + "SMIME.Encrypted"))
					{
						if (isException)
						{
							this.Exceptions.Add(EncryptedCondition.Create(this));
							return;
						}
						this.Conditions.Add(EncryptedCondition.Create(this));
						return;
					}
				}
				else if (5 == num && list3.Contains("IPM.Note.Microsoft.Voicemail.UM.CA") && list3.Contains("IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA") && list3.Contains("IPM.Note.rpmsg.Microsoft.Voicemail.UM") && list3.Contains("IPM.Note.Microsoft.Voicemail.UM") && list3.Contains("IPM.Note.Microsoft.Missed.Voice"))
				{
					if (isException)
					{
						this.Exceptions.Add(VoicemailCondition.Create(this));
						return;
					}
					this.Conditions.Add(VoicemailCondition.Create(this));
					return;
				}
				if (isException)
				{
					this.Exceptions.Add(FormsCondition.Create(ConditionType.FormsCondition, this, list3.ToArray()));
					return;
				}
				this.Conditions.Add(FormsCondition.Create(ConditionType.FormsCondition, this, list3.ToArray()));
				return;
			}
			else
			{
				if (num6 == num || num7 == num || num10 == num)
				{
					for (int l = 0; l < num; l++)
					{
						Restriction.ContentRestriction contentRestriction2 = restrictions[l] as Restriction.ContentRestriction;
						if (contentRestriction2 == null)
						{
							throw new RuleParseException(ServerStrings.UnsupportedContentRestriction);
						}
						object value2 = contentRestriction2.PropValue.Value;
						if (isException)
						{
							if (num6 > 0)
							{
								this.exceptBodyStrings.Add((string)value2);
							}
							else if (num10 > 0)
							{
								this.exceptHeaderStrings.Add((string)value2);
							}
							else
							{
								this.exceptSubjectStrings.Add((string)value2);
							}
						}
						else if (num6 > 0)
						{
							this.containsBodyStrings.Add((string)value2);
						}
						else if (num10 > 0)
						{
							this.containsHeaderStrings.Add((string)value2);
						}
						else
						{
							this.containsSubjectStrings.Add((string)value2);
						}
					}
					return;
				}
				if (num7 == num6 && num7 + num6 == num)
				{
					for (int m = 0; m < num; m += 2)
					{
						Restriction.ContentRestriction contentRestriction3 = restrictions[m] as Restriction.ContentRestriction;
						if (contentRestriction3 == null)
						{
							throw new RuleParseException(ServerStrings.UnsupportedContentRestriction);
						}
						object value3 = contentRestriction3.PropValue.Value;
						if (isException)
						{
							this.exceptSubjectOrBodyStrings.Add((string)value3);
						}
						else
						{
							this.containsSubjectOrBodyStrings.Add((string)value3);
						}
					}
					return;
				}
				if (num4 == num)
				{
					List<Participant> participants2 = new List<Participant>();
					Rule.CrackAddressList(participants2, restrictions);
					this.AddSenderElement(isException, participants2);
					return;
				}
				if (num5 == num)
				{
					List<Participant> participants3 = new List<Participant>();
					Rule.CrackAddressList(participants3, restrictions);
					this.AddRecipientElement(isException, participants3);
					return;
				}
				if (num8 == num || num9 == num)
				{
					for (int n = 0; n < num; n++)
					{
						Restriction.ContentRestriction contentRestriction4 = restrictions[n] as Restriction.ContentRestriction;
						if (contentRestriction4 == null)
						{
							throw new RuleParseException(ServerStrings.UnsupportedContentRestriction);
						}
						string item2 = Rule.ConvertSearchKeyValue((byte[])contentRestriction4.PropValue.Value);
						if (isException)
						{
							if (num9 == num)
							{
								this.exceptRecipientStrings.Add(item2);
							}
							else
							{
								this.exceptSenderStrings.Add(item2);
							}
						}
						else if (num9 == num)
						{
							this.containsRecipientStrings.Add(item2);
						}
						else
						{
							this.containsSenderStrings.Add(item2);
						}
					}
					return;
				}
				throw new RuleParseException(ServerStrings.UnsupportedContentRestriction);
			}
		}

		private void ParseServerRule()
		{
			this.validatingUserInput = false;
			this.Name = this.serverRule.Name;
			this.IsEnabled = ((this.serverRule.StateFlags & RuleStateFlags.Enabled) > (RuleStateFlags)0);
			this.RunOnlyWhileOof = ((this.serverRule.StateFlags & RuleStateFlags.OnlyWhenOOF) > (RuleStateFlags)0);
			this.Sequence = this.serverRule.ExecutionSequence;
			this.isInError = ((this.serverRule.StateFlags & RuleStateFlags.Error) == RuleStateFlags.Error);
			this.provider = this.serverRule.Provider;
			try
			{
				this.ParseServerRuleConditions();
				this.ParseServerRuleActions();
				if ((this.serverRule.StateFlags & RuleStateFlags.ExitAfterExecution) > (RuleStateFlags)0)
				{
					this.Actions.Add(StopProcessingAction.Create(this));
				}
			}
			catch (RuleParseException ex)
			{
				this.isNotSupported = true;
				ExTraceGlobals.StorageTracer.TraceError<string, string>((long)this.GetHashCode(), "ParseServerRule, can't process rule {0}, {1}", this.serverRule.Name, ex.Message);
			}
			this.validatingUserInput = true;
		}

		private void ParseServerRuleConditions()
		{
			Restriction condition = this.serverRule.Condition;
			this.AddRestrictions(condition, false);
			if (this.toParticipants.Count > 0)
			{
				this.Conditions.Add(SentToRecipientsCondition.Create(this, this.toParticipants.ToArray()));
			}
			if (this.fromParticipants.Count > 0)
			{
				this.Conditions.Add(FromRecipientsCondition.Create(this, this.fromParticipants.ToArray()));
			}
			if (this.containsSubjectStrings.Count > 0)
			{
				this.Conditions.Add(ContainsSubjectStringCondition.Create(this, this.containsSubjectStrings.ToArray()));
			}
			if (this.containsBodyStrings.Count > 0)
			{
				this.Conditions.Add(ContainsBodyStringCondition.Create(this, this.containsBodyStrings.ToArray()));
			}
			if (this.containsSubjectOrBodyStrings.Count > 0)
			{
				this.Conditions.Add(ContainsSubjectOrBodyStringCondition.Create(this, this.containsSubjectOrBodyStrings.ToArray()));
			}
			if (this.containsSenderStrings.Count > 0)
			{
				this.Conditions.Add(ContainsSenderStringCondition.Create(this, this.containsSenderStrings.ToArray()));
			}
			if (this.containsHeaderStrings.Count > 0)
			{
				this.Conditions.Add(ContainsHeaderStringCondition.Create(this, this.containsHeaderStrings.ToArray()));
			}
			if (this.before != null || this.after != null)
			{
				this.Conditions.Add(WithinDateRangeCondition.Create(this, this.after, this.before));
			}
			if (this.lowRange != null || this.highRange != null)
			{
				this.Conditions.Add(WithinSizeRangeCondition.Create(this, this.lowRange, this.highRange));
			}
			if (this.containsRecipientStrings.Count > 0)
			{
				this.Conditions.Add(ContainsRecipientStringCondition.Create(this, this.containsRecipientStrings.ToArray()));
			}
			if (this.assignedCategoriesStrings.Count > 0)
			{
				this.Conditions.Add(AssignedCategoriesCondition.Create(this, this.assignedCategoriesStrings.ToArray()));
			}
			if (this.isMessageClassificationCondition)
			{
				this.Conditions.Add(MessageClassificationCondition.Create(this, this.messageClassificationStrings.ToArray()));
			}
			if (this.exceptToParticipants.Count > 0)
			{
				this.Exceptions.Add(SentToRecipientsCondition.Create(this, this.exceptToParticipants.ToArray()));
			}
			if (this.exceptFromParticipants.Count > 0)
			{
				this.Exceptions.Add(FromRecipientsCondition.Create(this, this.exceptFromParticipants.ToArray()));
			}
			if (this.exceptSubjectStrings.Count > 0)
			{
				this.Exceptions.Add(ContainsSubjectStringCondition.Create(this, this.exceptSubjectStrings.ToArray()));
			}
			if (this.exceptBodyStrings.Count > 0)
			{
				this.Exceptions.Add(ContainsBodyStringCondition.Create(this, this.exceptBodyStrings.ToArray()));
			}
			if (this.exceptSubjectOrBodyStrings.Count > 0)
			{
				this.Exceptions.Add(ContainsSubjectOrBodyStringCondition.Create(this, this.exceptSubjectOrBodyStrings.ToArray()));
			}
			if (this.exceptSenderStrings.Count > 0)
			{
				this.Exceptions.Add(ContainsSenderStringCondition.Create(this, this.exceptSenderStrings.ToArray()));
			}
			if (this.exceptHeaderStrings.Count > 0)
			{
				this.Exceptions.Add(ContainsHeaderStringCondition.Create(this, this.exceptHeaderStrings.ToArray()));
			}
			if (this.exceptBefore != null || this.exceptAfter != null)
			{
				this.Exceptions.Add(WithinDateRangeCondition.Create(this, this.exceptAfter, this.exceptBefore));
			}
			if (this.exceptLowRange != null || this.exceptHighRange != null)
			{
				this.Exceptions.Add(WithinSizeRangeCondition.Create(this, this.exceptLowRange, this.exceptHighRange));
			}
			if (this.exceptRecipientStrings.Count > 0)
			{
				this.Exceptions.Add(ContainsRecipientStringCondition.Create(this, this.exceptRecipientStrings.ToArray()));
			}
			if (this.exceptCategoriesStrings.Count > 0)
			{
				this.Exceptions.Add(AssignedCategoriesCondition.Create(this, this.exceptCategoriesStrings.ToArray()));
			}
			if (this.isMessageClassificationException)
			{
				this.Exceptions.Add(MessageClassificationCondition.Create(this, this.exceptMessageClassificationStrings.ToArray()));
			}
		}

		private bool AddRestrictions(Restriction res, bool isException)
		{
			if (res is Restriction.AndRestriction)
			{
				Restriction.AndRestriction andRestriction = res as Restriction.AndRestriction;
				if (andRestriction.Restrictions.Length == 3 && andRestriction.Restrictions[2] is Restriction.PropertyRestriction && (andRestriction.Restrictions[2] as Restriction.PropertyRestriction).PropTag == PropTag.DisplayCc)
				{
					if (isException)
					{
						this.Exceptions.Add(SentOnlyToMeCondition.Create(this));
					}
					else
					{
						this.Conditions.Add(SentOnlyToMeCondition.Create(this));
					}
				}
				else if (andRestriction.Restrictions[0] is Restriction.PropertyRestriction && (andRestriction.Restrictions[0] as Restriction.PropertyRestriction).PropTag == PropTag.MessageCcMe)
				{
					if (isException)
					{
						this.Exceptions.Add(SentCcMeCondition.Create(this));
					}
					else
					{
						this.Conditions.Add(SentCcMeCondition.Create(this));
					}
				}
				else if (this.IsNdrRestrictionSet(andRestriction))
				{
					if (isException)
					{
						this.Exceptions.Add(NdrCondition.Create(this));
					}
					else
					{
						this.Conditions.Add(NdrCondition.Create(this));
					}
				}
				else if (this.IsFlaggedRestrictionSet(andRestriction))
				{
					Restriction.PropertyRestriction propertyRestriction = andRestriction.Restrictions[1] as Restriction.PropertyRestriction;
					string @string = propertyRestriction.PropValue.GetString();
					if (isException)
					{
						this.Exceptions.Add(FlaggedForActionCondition.Create(this, @string));
					}
					else
					{
						this.Conditions.Add(FlaggedForActionCondition.Create(this, @string));
					}
				}
				else
				{
					for (int i = 0; i < andRestriction.Restrictions.Length; i++)
					{
						this.AddRestrictions(andRestriction.Restrictions[i], isException);
					}
				}
			}
			else if (res is Restriction.OrRestriction)
			{
				this.ParseOrRestriction(res as Restriction.OrRestriction, isException);
			}
			else if (res is Restriction.NotRestriction)
			{
				Restriction.NotRestriction notRestriction = res as Restriction.NotRestriction;
				this.AddRestrictions(notRestriction.Restriction, !isException);
			}
			else if (res is Restriction.ContentRestriction)
			{
				Restriction.ContentRestriction contentRestriction = res as Restriction.ContentRestriction;
				PropTag propTag = contentRestriction.PropTag;
				if (propTag == PropTag.SenderName)
				{
					List<Participant> participants = new List<Participant>();
					Rule.CrackAddressList(participants, new Restriction[]
					{
						res
					});
					this.AddSenderElement(isException, participants);
				}
				else if (propTag == PropTag.DisplayName)
				{
					List<Participant> participants2 = new List<Participant>();
					Rule.CrackAddressList(participants2, new Restriction[]
					{
						res
					});
					this.AddRecipientElement(isException, participants2);
				}
				else
				{
					if (contentRestriction.MultiValued)
					{
						propTag &= (PropTag)4096U;
					}
					this.AddElementFromProp(propTag, contentRestriction.PropValue.Value, (int)contentRestriction.Flags, isException);
				}
			}
			else if (res is Restriction.PropertyRestriction)
			{
				Restriction.PropertyRestriction propertyRestriction2 = res as Restriction.PropertyRestriction;
				PropTag propTag2 = propertyRestriction2.MultiValued ? (propertyRestriction2.PropTag | (PropTag)4096U) : propertyRestriction2.PropTag;
				this.AddElementFromProp(propTag2, propertyRestriction2.PropValue.Value, (int)propertyRestriction2.Op, isException);
			}
			else if (res is Restriction.CommentRestriction)
			{
				Restriction.CommentRestriction commentRestriction = res as Restriction.CommentRestriction;
				Rule.InboxSpecialComment inboxSpecialComment = (Rule.InboxSpecialComment)commentRestriction.Values[0].Value;
				if (inboxSpecialComment == Rule.InboxSpecialComment.Resolved)
				{
					List<Participant> participants3 = new List<Participant>();
					Rule.CrackAddressList(participants3, new Restriction[]
					{
						res
					});
					try
					{
						if (Rule.IsSenderRst(res))
						{
							this.AddSenderElement(isException, participants3);
						}
						else
						{
							this.AddRecipientElement(isException, participants3);
						}
						goto IL_52C;
					}
					catch (ArgumentException)
					{
						throw new RuleParseException(ServerStrings.MalformedCommentRestriction);
					}
				}
				this.AddRestrictions(commentRestriction.Restriction, isException);
			}
			else if (res is Restriction.BitMaskRestriction)
			{
				Restriction.BitMaskRestriction bitMaskRestriction = res as Restriction.BitMaskRestriction;
				this.AddElementFromBitmaskProp(bitMaskRestriction.Tag, bitMaskRestriction.Mask, bitMaskRestriction.Bmr, isException);
			}
			else
			{
				if (res is Restriction.SubRestriction)
				{
					Restriction restriction = (res as Restriction.SubRestriction).Restriction;
					if (restriction is Restriction.OrRestriction && (restriction as Restriction.OrRestriction).Restrictions[0] is Restriction.OrRestriction && ((restriction as Restriction.OrRestriction).Restrictions[0] as Restriction.OrRestriction).Restrictions[0] is Restriction.AndRestriction && (((restriction as Restriction.OrRestriction).Restrictions[0] as Restriction.OrRestriction).Restrictions[0] as Restriction.AndRestriction).Restrictions[0] is Restriction.ContentRestriction)
					{
						Restriction[] restrictions = (restriction as Restriction.OrRestriction).Restrictions;
						List<Participant> list = new List<Participant>();
						for (int j = 0; j < restrictions.Length; j++)
						{
							Restriction.ContentRestriction contentRestriction2 = ((restrictions[j] as Restriction.OrRestriction).Restrictions[0] as Restriction.AndRestriction).Restrictions[0] as Restriction.ContentRestriction;
							object value = contentRestriction2.PropValue.Value;
							PropTag propTag3 = contentRestriction2.PropTag;
							list.Add(new Participant((string)value, null, null));
						}
						this.AddRecipientElement(isException, list);
						goto IL_52C;
					}
					if (!(restriction is Restriction.ContentRestriction))
					{
						try
						{
							this.AddRestrictions(restriction, isException);
							goto IL_52C;
						}
						finally
						{
							this.hack = true;
						}
					}
					try
					{
						this.AddRestrictions(restriction, isException);
						goto IL_52C;
					}
					finally
					{
						this.hack = true;
					}
				}
				if (res is Restriction.ExistRestriction)
				{
					Restriction.ExistRestriction existRestriction = res as Restriction.ExistRestriction;
					if (PropTag.MessageClass != existRestriction.Tag && PropTag.Sensitivity != existRestriction.Tag && (PropTag)1071841291U != existRestriction.Tag)
					{
						throw new RuleParseException(ServerStrings.UnsupportedExistRestriction);
					}
				}
			}
			IL_52C:
			return this.hack;
		}

		private void ParseServerRuleActions()
		{
			this.AddActions(this.serverRule.Actions);
		}

		private void AddActions(RuleAction[] actions)
		{
			if (actions == null || actions.Length == 0)
			{
				return;
			}
			for (int i = 0; i < actions.Length; i++)
			{
				if (actions[i] is RuleAction.InMailboxMove)
				{
					RuleAction.InMailboxMove inMailboxMove = actions[i] as RuleAction.InMailboxMove;
					MailboxSession mailboxSession = this.Folder.Session as MailboxSession;
					if (ArrayComparer<byte>.Comparer.Equals(inMailboxMove.FolderEntryID, mailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems).ProviderLevelItemId))
					{
						this.Actions.Add(DeleteAction.Create(this));
					}
					else
					{
						StoreObjectId folderId = StoreObjectId.FromProviderSpecificId(inMailboxMove.FolderEntryID, StoreObjectType.Folder);
						this.Actions.Add(MoveToFolderAction.Create(folderId, this));
					}
				}
				else if (actions[i] is RuleAction.InMailboxCopy)
				{
					RuleAction.InMailboxCopy inMailboxCopy = actions[i] as RuleAction.InMailboxCopy;
					StoreObjectId folderId2 = StoreObjectId.FromProviderSpecificId(inMailboxCopy.FolderEntryID, StoreObjectType.Folder);
					this.Actions.Add(CopyToFolderAction.Create(folderId2, this));
				}
				else
				{
					if (actions[i] is RuleAction.ExternalMove)
					{
						throw new RuleParseException(ServerStrings.UnsupportedAction);
					}
					if (actions[i] is RuleAction.ExternalCopy)
					{
						throw new RuleParseException(ServerStrings.UnsupportedAction);
					}
					if (actions[i] is RuleAction.Reply)
					{
						RuleAction.Reply reply = actions[i] as RuleAction.Reply;
						StoreObjectId messageId = null;
						Guid replyTemplateGuid = reply.ReplyTemplateGuid;
						try
						{
							messageId = StoreObjectId.FromProviderSpecificId(reply.ReplyTemplateMessageEntryID);
							using (MessageItem messageItem = MessageItem.Bind(this.Folder.Session, messageId, new PropertyDefinition[]
							{
								ItemSchema.Subject
							}))
							{
								string subject = messageItem.Subject;
							}
						}
						catch (ObjectNotFoundException e)
						{
							throw new RuleParseException(ServerStrings.NoTemplateMessage, e);
						}
						this.Actions.Add(ServerReplyMessageAction.Create(messageId, replyTemplateGuid, this));
					}
					else
					{
						if (actions[i] is RuleAction.Defer)
						{
							throw new RuleParseException(ServerStrings.NoDeferredActions);
						}
						if (actions[i] is RuleAction.Forward)
						{
							RuleAction.Forward forward = actions[i] as RuleAction.Forward;
							List<Participant> list = new List<Participant>();
							for (int j = 0; j < forward.Recipients.Length; j++)
							{
								try
								{
									list.Add(Rule.ParticipantFromAdrEntry(forward.Recipients[j]));
								}
								catch (ArgumentException)
								{
									throw new RuleParseException(ServerStrings.MalformedAdrEntry);
								}
							}
							if ((forward.Flags & RuleAction.Forward.ActionFlags.DoNotMungeMessage) > RuleAction.Forward.ActionFlags.None && (forward.Flags & RuleAction.Forward.ActionFlags.PreserveSender) > RuleAction.Forward.ActionFlags.None)
							{
								this.Actions.Add(RedirectToRecipientsAction.Create(list, this));
							}
							else if ((forward.Flags & RuleAction.Forward.ActionFlags.ForwardAsAttachment) > RuleAction.Forward.ActionFlags.None)
							{
								this.Actions.Add(ForwardAsAttachmentToRecipientsAction.Create(list, this));
							}
							else if ((forward.Flags & RuleAction.Forward.ActionFlags.SendSmsAlert) > RuleAction.Forward.ActionFlags.None)
							{
								this.Actions.Add(SendSmsAlertToRecipientsAction.Create(list, this));
							}
							else
							{
								this.Actions.Add(ForwardToRecipientsAction.Create(list, this));
							}
						}
						else if (actions[i] is RuleAction.Delegate)
						{
							RuleAction.Delegate @delegate = actions[i] as RuleAction.Delegate;
							List<Participant> list2 = new List<Participant>();
							if (@delegate.Recipients != null)
							{
								for (int k = 0; k < @delegate.Recipients.Length; k++)
								{
									try
									{
										list2.Add(Rule.ParticipantFromAdrEntry(@delegate.Recipients[k]));
									}
									catch (ArgumentException)
									{
										throw new RuleParseException(ServerStrings.MalformedAdrEntry);
									}
								}
							}
							this.Actions.Add(RedirectToRecipientsAction.Create(list2, this));
						}
						else if (actions[i] is RuleAction.Tag)
						{
							RuleAction.Tag tag = actions[i] as RuleAction.Tag;
							if (tag.Value.PropTag == PropTag.Importance)
							{
								this.Actions.Add(MarkImportanceAction.Create((Importance)tag.Value.Value, this));
							}
							else if (tag.Value.PropTag == PropTag.Sensitivity)
							{
								this.Actions.Add(MarkSensitivityAction.Create((Sensitivity)tag.Value.Value, this));
							}
							else if (tag.Value.PropTag == (PropTag)277872643U)
							{
								this.Actions.Add(FlagMessageAction.Create((FlagStatus)tag.Value.Value, this));
							}
							else
							{
								GuidNamePropertyDefinition guidNamePropertyDefinition = this.PropTagToPropertyDefinitionFromCache(tag.Value.PropTag) as GuidNamePropertyDefinition;
								if (guidNamePropertyDefinition == null || !guidNamePropertyDefinition.Equals(Rule.NamedDefinitions[0]))
								{
									throw new RuleParseException(ServerStrings.UnsupportedAction);
								}
								this.Actions.Add(AssignCategoriesAction.Create((string[])tag.Value.Value, this));
							}
						}
						else if (actions[i] is RuleAction.Delete)
						{
							this.Actions.Add(PermanentDeleteAction.Create(this));
						}
						else
						{
							if (!(actions[i] is RuleAction.MarkAsRead))
							{
								throw new RuleParseException(ServerStrings.UnsupportedAction);
							}
							this.Actions.Add(MarkAsReadAction.Create(this));
						}
					}
				}
			}
		}

		internal static Participant ParticipantFromAdrEntry(AdrEntry adrEntry)
		{
			string displayName = string.Empty;
			byte[] entryIdBytes = null;
			byte[] searchKey = null;
			for (int i = 0; i < adrEntry.Values.Length; i++)
			{
				PropTag propTag = adrEntry.Values[i].PropTag;
				if (propTag != PropTag.EntryId)
				{
					if (propTag != PropTag.DisplayName)
					{
						if (propTag == PropTag.SearchKey)
						{
							searchKey = (byte[])adrEntry.Values[i].Value;
						}
					}
					else
					{
						displayName = (string)adrEntry.Values[i].Value;
					}
				}
				else
				{
					entryIdBytes = (byte[])adrEntry.Values[i].Value;
				}
			}
			return Rule.CreateParticipant(displayName, entryIdBytes, searchKey);
		}

		internal static AdrEntry AdrEntryFromParticipant(Participant participant)
		{
			string displayName = participant.DisplayName;
			string text = participant.RoutingType;
			string emailAddress = participant.EmailAddress;
			byte[] value = Rule.SearchKeyFromParticipant(participant);
			LegacyRecipientDisplayType legacyRecipientDisplayType = LegacyRecipientDisplayType.MailUser;
			byte[] array = null;
			ParticipantEntryId participantEntryId = ParticipantEntryId.FromParticipant(participant, ParticipantEntryIdConsumer.RecipientTableSecondary);
			if (participantEntryId != null)
			{
				legacyRecipientDisplayType = participant.GetValueOrDefault<LegacyRecipientDisplayType>(ParticipantSchema.DisplayType);
				array = participantEntryId.ToByteArray();
				if (participantEntryId is StoreParticipantEntryId && ((StoreParticipantEntryId)participantEntryId).IsDL == true)
				{
					text = "MAPIPDL";
				}
			}
			List<PropValue> list = new List<PropValue>();
			if (array != null)
			{
				list.Add(new PropValue(PropTag.EntryId, array));
			}
			list.Add(new PropValue(PropTag.DisplayName, displayName));
			list.Add(new PropValue(PropTag.DisplayType, (int)legacyRecipientDisplayType));
			if (string.IsNullOrEmpty(text) || text == "SMTP")
			{
				list.Add(new PropValue(PropTag.SmtpAddress, emailAddress));
			}
			list.Add(new PropValue(PropTag.SearchKey, value));
			list.Add(new PropValue(PropTag.RecipientType, RecipientType.To));
			if (!string.IsNullOrEmpty(text))
			{
				list.Add(new PropValue(PropTag.AddrType, text));
			}
			else
			{
				list.Add(new PropValue(PropTag.AddrType, "SMTP"));
			}
			list.Add(new PropValue(PropTag.EmailAddress, emailAddress));
			return new AdrEntry(list.ToArray());
		}

		internal static Restriction OrAddressList(IList<Participant> participants, PropTag tagDisplayName)
		{
			if (participants.Count == 1)
			{
				return Rule.ContentOrPropertyFromParticipant(participants[0], tagDisplayName);
			}
			Restriction[] array = new Restriction[participants.Count];
			bool flag = false;
			for (int i = 0; i < participants.Count; i++)
			{
				array[i] = Rule.ContentOrPropertyFromParticipant(participants[i], tagDisplayName);
				if (array[i] is Restriction.ContentRestriction)
				{
					flag = true;
				}
			}
			if (flag)
			{
				Array.Sort(array, Rule.addressListComparer);
			}
			return Restriction.Or(array);
		}

		internal static Rule CheckRuleParameter(object[] parameters)
		{
			if (parameters.Length == 0)
			{
				throw new ArgumentException("rule");
			}
			if (parameters[0] == null)
			{
				throw new ArgumentNullException("rule");
			}
			Rule rule = parameters[0] as Rule;
			if (rule == null)
			{
				throw new ArgumentException("rule");
			}
			return rule;
		}

		internal Folder Folder
		{
			get
			{
				return this.folder;
			}
		}

		internal Rule ServerRule
		{
			get
			{
				return this.serverRule;
			}
		}

		internal void SetDirty()
		{
			this.isDirty = true;
		}

		internal void ClearDirty()
		{
			this.isDirty = false;
		}

		internal bool ValidatingUserInput
		{
			get
			{
				return this.validatingUserInput;
			}
		}

		internal PropTag PropertyDefinitionToPropTagFromCache(NamedPropertyDefinition propertyDefinition)
		{
			NamedPropMap mapping = NamedPropMapCache.Default.GetMapping(this.folder.Session);
			NamedPropertyDefinition.NamedPropertyKey key = propertyDefinition.GetKey();
			NamedProp namedProp = key.NamedProp;
			ushort num;
			if (mapping == null || !mapping.TryGetPropIdFromNamedProp(namedProp, out num))
			{
				PropertyTagCache.Cache.PropTagsFromPropertyDefinitions(this.folder.MapiFolder, this.folder.Session, Rule.NamedDefinitions);
			}
			PropTag propTag = PropertyTagCache.Cache.PropTagFromPropertyDefinition(this.folder.MapiFolder, this.folder.Session, propertyDefinition);
			if (PropTag.Unresolved == propTag)
			{
				throw new RuleParseException(ServerStrings.MapiCannotGetIDFromNames);
			}
			return propTag;
		}

		internal void ThrowValidateException(Rule.ThrowExceptionDelegate thrower, string errStr)
		{
			if (this.validatingUserInput)
			{
				thrower();
				return;
			}
			throw new RuleParseException(errStr);
		}

		private const string Delimiter = ";";

		internal const string OL97Provider = "MSFT:TDX Rules";

		internal const string OL98PlusProvider = "RuleOrganizer";

		internal const string Exchange14Provider = "ExchangeMailboxRules14";

		internal const string OLKRuleMsgClass = "IPM.RuleOrganizer";

		internal const string NetFolders = "#NET FOLDERS#";

		internal const string IPMRulesOofTemplate = "IPM.Note.Rules.OofTemplate.Microsoft";

		internal const string IPMScheduleMeetingRequest = "IPM.Schedule.Meeting.Request";

		internal const string IPMScheduleMeetingCanceled = "IPM.Schedule.Meeting.Canceled";

		internal const PropTag ComCritCommentIdPropTag = (PropTag)1610612739U;

		internal const PropTag DisplayTypePropTag = PropTag.DisplayType;

		public static readonly string[] ProviderStringArray = new string[]
		{
			null,
			"MSFT:TDX Rules",
			"RuleOrganizer",
			"ExchangeMailboxRules14"
		};

		internal static readonly NamedPropertyDefinition[] NamedDefinitions = new NamedPropertyDefinition[]
		{
			InternalSchema.Categories,
			InternalSchema.IsClassified,
			InternalSchema.ClassificationGuid,
			InternalSchema.IsReadReceipt,
			InternalSchema.IsSigned,
			InternalSchema.FlagRequest
		};

		private bool hack;

		private Rule serverRule;

		private Folder folder;

		private string name;

		private RuleId ruleId;

		private bool isEnabled;

		private bool onlyOof;

		private bool isInError;

		private bool isNotSupported;

		private bool isDirty;

		private int sequence;

		private string provider;

		private Rule.ProviderIdEnum providerId;

		private ActionList actions;

		private ConditionList conditions;

		private ConditionList exceptions;

		private List<Participant> toParticipants;

		private List<Participant> fromParticipants;

		private List<string> containsSubjectStrings;

		private List<string> containsBodyStrings;

		private List<string> containsSubjectOrBodyStrings;

		private List<string> containsSenderStrings;

		private List<string> containsHeaderStrings;

		private List<string> containsRecipientStrings;

		private List<string> assignedCategoriesStrings;

		private List<string> messageClassificationStrings;

		private List<Participant> exceptToParticipants;

		private List<Participant> exceptFromParticipants;

		private List<string> exceptSubjectStrings;

		private List<string> exceptBodyStrings;

		private List<string> exceptSubjectOrBodyStrings;

		private List<string> exceptSenderStrings;

		private List<string> exceptRecipientStrings;

		private List<string> exceptHeaderStrings;

		private List<string> exceptCategoriesStrings;

		private List<string> exceptMessageClassificationStrings;

		private bool validatingUserInput = true;

		private bool isNew = true;

		private bool isMessageClassificationCondition;

		private bool isMessageClassificationException;

		private int? lowRange = null;

		private int? highRange = null;

		private int? exceptLowRange = null;

		private int? exceptHighRange = null;

		private ExDateTime? before = null;

		private ExDateTime? after = null;

		private ExDateTime? exceptBefore = null;

		private ExDateTime? exceptAfter = null;

		private static Rule.AddressListRestrictionComparer addressListComparer = new Rule.AddressListRestrictionComparer();

		internal enum NamedDefinitionIndex
		{
			Categories,
			IsClassified,
			ClassificationGuid,
			IsReadReceipt,
			IsSigned,
			FlagRequest
		}

		internal delegate void ThrowExceptionDelegate();

		private class AddressListRestrictionComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				Restriction restriction = x as Restriction;
				Restriction restriction2 = y as Restriction;
				if (restriction == null || restriction2 == null)
				{
					return 0;
				}
				if (restriction.GetType() == restriction2.GetType())
				{
					return 0;
				}
				if (restriction is Restriction.CommentRestriction)
				{
					return -1;
				}
				return 1;
			}
		}

		public enum ProviderIdEnum
		{
			Unknown,
			OL97,
			OL98Plus,
			Exchange14
		}

		internal enum InboxSpecialComment
		{
			Resolved = 1,
			TopCmt,
			Forms,
			FormProps
		}

		internal struct ProviderData
		{
			public ProviderData(ArraySegment<byte> buffer)
			{
				this.Version = BitConverter.ToUInt32(buffer.Array, buffer.Offset + Rule.ProviderData.VersionOffset);
				this.RuleSearchKey = BitConverter.ToUInt32(buffer.Array, buffer.Offset + Rule.ProviderData.RuleSearchKeyOffset);
				this.TimeStamp = BitConverter.ToInt64(buffer.Array, buffer.Offset + Rule.ProviderData.TimeStampOffset);
			}

			public byte[] ToByteArray()
			{
				byte[] array = new byte[Rule.ProviderData.Size];
				ExBitConverter.Write(this.Version, array, Rule.ProviderData.VersionOffset);
				ExBitConverter.Write(this.RuleSearchKey, array, Rule.ProviderData.RuleSearchKeyOffset);
				ExBitConverter.Write(this.TimeStamp, array, Rule.ProviderData.TimeStampOffset);
				return array;
			}

			public uint Version;

			public uint RuleSearchKey;

			public long TimeStamp;

			private static readonly int VersionOffset = (int)Marshal.OffsetOf(typeof(Rule.ProviderData), "Version");

			private static readonly int RuleSearchKeyOffset = (int)Marshal.OffsetOf(typeof(Rule.ProviderData), "RuleSearchKey");

			private static readonly int TimeStampOffset = (int)Marshal.OffsetOf(typeof(Rule.ProviderData), "TimeStamp");

			private static readonly int Size = Marshal.SizeOf(typeof(Rule.ProviderData));
		}
	}
}
