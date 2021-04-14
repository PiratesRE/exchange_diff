using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RuleUtil
	{
		internal static void FaultInjection(FaultInjectionLid lid)
		{
			ExTraceGlobals.FaultInjectionTracer.TraceTest((uint)lid);
		}

		internal static void CheckValueType(object value, PropTag tag)
		{
			if (value == null)
			{
				return;
			}
			PropType propType = tag.ValueType();
			Type[] array = RuleUtil.singleValuePropTypes;
			if (tag.IsMultiValued() || tag.IsMultiInstance())
			{
				array = RuleUtil.multiValuePropTypes;
			}
			uint num = (uint)(propType & (PropType)(-12289));
			if (num < (uint)array.Length)
			{
				Type type = array[(int)((UIntPtr)num)];
				if (type != null)
				{
					if (value.GetType() == type)
					{
						return;
					}
					if (value.GetType() == typeof(ExDateTime) && type == typeof(DateTime))
					{
						return;
					}
					throw new InvalidRuleException(string.Format("Value of tag {0} is of unexpected type {1}", tag, value.GetType()));
				}
			}
			throw new InvalidRuleException(string.Format("Can't determine type for tag {0}", tag));
		}

		internal unsafe static int CompareBytes(long x, long y)
		{
			long* px = &x;
			long* py = &y;
			return RuleUtil.CompareBytes((byte*)px, (byte*)py, 8);
		}

		internal unsafe static int CompareBytes(Guid x, Guid y)
		{
			Guid* px = &x;
			Guid* py = &y;
			return RuleUtil.CompareBytes((byte*)px, (byte*)py, sizeof(Guid));
		}

		internal unsafe static int CompareBytes(byte[] x, byte[] y)
		{
			int len = Math.Min(x.Length, y.Length);
			fixed (byte* ptr = x)
			{
				fixed (byte* ptr2 = y)
				{
					int num = RuleUtil.CompareBytes(ptr, ptr2, len);
					int result;
					if (num != 0)
					{
						result = num;
					}
					else
					{
						result = x.Length.CompareTo(y.Length);
					}
					return result;
				}
			}
		}

		internal static bool Contains(byte[] content, byte[] pattern)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			if (pattern == null)
			{
				throw new ArgumentNullException("pattern");
			}
			int num = content.Length - pattern.Length + 1;
			for (int i = 0; i < num; i++)
			{
				int num2 = 0;
				while (num2 < pattern.Length && content[i + num2] == pattern[num2])
				{
					num2++;
				}
				if (num2 == pattern.Length)
				{
					return true;
				}
			}
			return false;
		}

		internal static object ConvertEnumValue(PropTag tag, object enumValue)
		{
			ulong num = Convert.ToUInt64(enumValue);
			PropType propType = tag.ValueType();
			PropType propType2 = propType;
			switch (propType2)
			{
			case PropType.Short:
				return (short)num;
			case PropType.Int:
				return (int)num;
			default:
				if (propType2 == PropType.Boolean)
				{
					return num != 0UL;
				}
				if (propType2 != PropType.Long)
				{
					throw new InvalidRuleException(string.Format("Enum value can't be used on tag {0}", tag));
				}
				return (int)num;
			}
		}

		internal unsafe static bool EqualsByteArray(byte[] leftArray, byte[] rightArray)
		{
			if (leftArray == null)
			{
				return rightArray == null;
			}
			if (rightArray == null || rightArray.Length != leftArray.Length)
			{
				return false;
			}
			fixed (byte* ptr = leftArray)
			{
				fixed (byte* ptr2 = rightArray)
				{
					long* ptr3 = (long*)ptr;
					long* ptr4 = (long*)ptr2;
					int i = 0;
					while (i < leftArray.Length / 16)
					{
						bool result;
						if (*ptr3 != *ptr4)
						{
							result = false;
						}
						else
						{
							if (ptr3[1] == ptr4[1])
							{
								ptr3 += 2;
								ptr4 += 2;
								i++;
								continue;
							}
							result = false;
						}
						return result;
					}
					byte* ptr5 = (byte*)ptr3;
					byte* ptr6 = (byte*)ptr4;
					for (int j = 0; j < leftArray.Length % 16; j++)
					{
						if (ptr5[j] != ptr6[j])
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		internal static bool EqualsEntryId(byte[] x, byte[] y)
		{
			if (RuleUtil.EqualsByteArray(x, y))
			{
				return true;
			}
			Participant participant = RuleUtil.GetParticipant(x);
			Participant participant2 = RuleUtil.GetParticipant(y);
			return !(participant == null) && !(participant2 == null) && participant.AreAddressesEqual(participant2);
		}

		internal static bool EqualsStoreId(StoreId idX, StoreId idY)
		{
			if (idX == null)
			{
				return idY == null;
			}
			return idY != null && RuleUtil.GetObjectId(idX).Equals(RuleUtil.GetObjectId(idY));
		}

		internal static StoreId GetObjectId(StoreId id)
		{
			VersionedId versionedId = id as VersionedId;
			if (versionedId != null && versionedId.ObjectId != null)
			{
				return versionedId.ObjectId;
			}
			return id;
		}

		internal static bool IsMemberOf(IRuleEvaluationContext context, byte[] recipientEntryId, byte[] groupEntryId)
		{
			ProxyAddress legacyProxyAddress = RuleUtil.GetLegacyProxyAddress(context, recipientEntryId);
			if (legacyProxyAddress == null)
			{
				context.TraceDebug("IsMemberOf: unable to get legacy DN for recipient.");
				return false;
			}
			ProxyAddress legacyProxyAddress2 = RuleUtil.GetLegacyProxyAddress(context, groupEntryId);
			if (legacyProxyAddress2 == null)
			{
				context.TraceDebug("IsMemberOf: unable to get address for group, this rule is in error.");
				throw new InvalidRuleException(ServerStrings.FolderRuleErrorInvalidGroup(BitConverter.ToString(groupEntryId)));
			}
			context.TraceDebug<string, string>("IsMemberOf: Recipient=\"{0}\" Group=\"{1}\"", legacyProxyAddress.AddressString, legacyProxyAddress2.AddressString);
			Result<ADRawEntry> result = context.RecipientCache.FindAndCacheRecipient(legacyProxyAddress);
			if (result.Data == null)
			{
				context.TraceDebug("Recipient doesn't exist in AD");
				context.RecordError(ServerStrings.FolderRuleErrorGroupDoesNotResolve(legacyProxyAddress.ToString()));
				return false;
			}
			bool result2;
			try
			{
				IsMemberOfResolver<string> isMemberOfResolver = context.IsMemberOfResolver;
				bool flag = isMemberOfResolver.IsMemberOf(context.RecipientCache.ADSession, result.Data.Id, legacyProxyAddress2.AddressString);
				context.TraceDebug<bool>("IsMemberOf: {0}", flag);
				result2 = flag;
			}
			catch (AddressBookTransientException exception)
			{
				context.RecordError(exception, ServerStrings.FolderRuleStageEvaluation);
				result2 = false;
			}
			return result2;
		}

		internal static PropTag GetMultiValuePropTag(PropTag tag)
		{
			return PropTagHelper.PropTagFromIdAndType(tag.Id(), tag.ValueType() | PropType.MultiValueFlag);
		}

		internal static ProxyAddress GetOriginalSender(MessageItem message)
		{
			string text = message.TryGetProperty(ItemSchema.SentRepresentingEmailAddress) as string;
			string text2 = message.TryGetProperty(ItemSchema.SentRepresentingType) as string;
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			if (text2 == null)
			{
				text2 = string.Empty;
			}
			return ProxyAddress.Parse(text2, text);
		}

		internal static Participant GetParticipant(byte[] entryId)
		{
			ParticipantEntryId participantEntryId = ParticipantEntryId.TryFromEntryId(entryId);
			if (participantEntryId is UnrecognizedParticipantEntryId)
			{
				return null;
			}
			Participant.Builder builder = new Participant.Builder();
			builder.SetPropertiesFrom(participantEntryId);
			return builder.ToParticipant();
		}

		internal static bool GetPropTypeSize(PropType type, out int size)
		{
			int num = -1;
			PropType propType = type & (PropType)(-12289);
			if (propType <= PropType.String)
			{
				switch (propType)
				{
				case PropType.Short:
					num = 2;
					goto IL_97;
				case PropType.Int:
				case PropType.Error:
					num = 4;
					goto IL_97;
				case PropType.Float:
					num = 4;
					goto IL_97;
				case PropType.Double:
				case PropType.AppTime:
					num = 8;
					goto IL_97;
				case PropType.Currency:
					break;
				case (PropType)8:
				case (PropType)9:
					goto IL_8C;
				case PropType.Boolean:
					num = 1;
					goto IL_97;
				default:
					if (propType != PropType.Long)
					{
						switch (propType)
						{
						case PropType.AnsiString:
						case PropType.String:
							goto IL_97;
						default:
							goto IL_8C;
						}
					}
					break;
				}
				num = 8;
				goto IL_97;
			}
			if (propType == PropType.SysTime)
			{
				num = 8;
				goto IL_97;
			}
			if (propType == PropType.Guid)
			{
				num = 16;
				goto IL_97;
			}
			if (propType == PropType.Binary)
			{
				goto IL_97;
			}
			IL_8C:
			throw new InvalidRuleException("Can't get size for property type");
			IL_97:
			size = num;
			return size != -1;
		}

		internal static ProxyAddress GetProxyAddressFromSearchKey(object searchKey)
		{
			byte[] array = searchKey as byte[];
			if (array == null || array.Length == 0 || array[0] == 0)
			{
				return null;
			}
			int num = array.Length;
			if (array[num - 1] == 0)
			{
				num--;
			}
			string @string = RuleUtil.asciiEncoding.GetString(array, 0, num);
			ProxyAddress proxyAddress = ProxyAddress.Parse(@string);
			if (proxyAddress is InvalidProxyAddress && string.Equals(proxyAddress.PrefixString, ProxyAddressPrefix.Smtp.PrimaryPrefix, StringComparison.OrdinalIgnoreCase))
			{
				string text = StringUtil.Unwrap(proxyAddress.ValueString);
				if (!object.ReferenceEquals(text, proxyAddress.ValueString))
				{
					proxyAddress = ProxyAddress.Parse(proxyAddress.PrefixString, text);
				}
			}
			return proxyAddress;
		}

		internal static PropTag GetSingleValuePropTag(PropTag tag)
		{
			return PropTagHelper.PropTagFromIdAndType(tag.Id(), tag.ValueType() & (PropType)(-12289));
		}

		internal static int GetSize(object value)
		{
			if (value == null)
			{
				return 0;
			}
			byte[] array = value as byte[];
			if (array != null)
			{
				return array.Length;
			}
			string text = value as string;
			if (text != null)
			{
				return text.Length * 2;
			}
			return -1;
		}

		internal static bool IsBinaryProp(PropTag tag)
		{
			PropType propType = tag.ValueType();
			return (propType & (PropType)(-12289)) == PropType.Binary;
		}

		internal static bool IsBooleanTag(PropTag tag)
		{
			PropType propType = tag.ValueType();
			return (propType & (PropType)(-12289)) == PropType.Boolean;
		}

		internal static bool IsDlEntry(ADRawEntry entry)
		{
			Microsoft.Exchange.Data.Directory.Recipient.RecipientType recipientType = (Microsoft.Exchange.Data.Directory.Recipient.RecipientType)entry[ADRecipientSchema.RecipientType];
			return recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.Group || recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalDistributionGroup || recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalSecurityGroup || recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailNonUniversalGroup || recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.DynamicDistributionGroup;
		}

		internal static bool IsEntryIdProp(PropTag tag)
		{
			return tag == PropTag.SenderEntryId || tag == PropTag.SentRepresentingEntryId || tag == PropTag.OriginalSenderEntryId || tag == PropTag.OriginalSentRepresentingEntryId || tag == PropTag.ReceivedByEntryId || tag == PropTag.RcvdRepresentingEntryId || tag == PropTag.ReportEntryId || tag == PropTag.ReadReceiptEntryId || tag == (PropTag)1073283330U || tag == (PropTag)1073414402U || tag == (PropTag)1716650242U || tag == (PropTag)1717436674U || tag == (PropTag)1717895426U || tag == PropTag.OriginalAuthorEntryId;
		}

		internal static bool IsLongID(PropTag tag)
		{
			return tag == PropTag.Fid || tag == PropTag.Mid || tag == (PropTag)1065877524U || tag == (PropTag)1065353236U || tag == (PropTag)1732837396U || tag == (PropTag)1733099540U || tag == (PropTag)241500180U || tag == (PropTag)251527188U || tag == PropTag.SourceFid;
		}

		internal static bool IsMultiValueTag(PropTag tag)
		{
			return tag.IsMultiValued();
		}

		internal static bool IsNullOrEmpty(Array array)
		{
			return array == null || array.Length == 0;
		}

		internal static bool IsPrefix(byte[] content, byte[] pattern)
		{
			if (content == null || pattern == null)
			{
				return false;
			}
			if (pattern.Length > content.Length)
			{
				return false;
			}
			for (int i = 0; i < pattern.Length; i++)
			{
				if (content[i] != pattern[i])
				{
					return false;
				}
			}
			return true;
		}

		internal static bool IsSameType(PropTag tagX, PropTag tagY)
		{
			PropType propType = tagX.ValueType() & (PropType)(-12289);
			PropType propType2 = tagY.ValueType() & (PropType)(-12289);
			return propType == propType2;
		}

		internal static bool IsSameUser(IRuleEvaluationContext context, IADRecipientCache recipientCache, ProxyAddress addressToResolve, ProxyAddress addressToCompare)
		{
			if (addressToResolve == null || addressToResolve is InvalidProxyAddress)
			{
				return false;
			}
			if (addressToCompare == null || addressToCompare is InvalidProxyAddress)
			{
				return false;
			}
			if (addressToResolve.Equals(addressToCompare))
			{
				return true;
			}
			ADRawEntry data = recipientCache.FindAndCacheRecipient(addressToResolve).Data;
			if (data == null)
			{
				context.TraceDebug<string>("Message recipient {0} did not resolve, IsSameUser returning false.", addressToResolve.ToString());
				return false;
			}
			string text = (string)data[ADRecipientSchema.LegacyExchangeDN];
			if (text != null && addressToCompare.ValueString.Equals(text, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)data[ADRecipientSchema.EmailAddresses];
			if (proxyAddressCollection == null)
			{
				return false;
			}
			foreach (ProxyAddress proxyAddress in proxyAddressCollection)
			{
				if (addressToCompare.ValueString.Equals(proxyAddress.ValueString, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsTextProp(PropTag tag)
		{
			PropType propType = tag.ValueType() & (PropType)(-12289);
			return propType == PropType.String || propType == PropType.AnsiString;
		}

		internal static PropTag NormalizeTag(PropTag tag)
		{
			PropType propType = tag.ValueType() & (PropType)(-12289);
			if (propType == PropType.AnsiString)
			{
				propType = ((tag.ValueType() & PropType.MultiValueFlag) | PropType.String);
				tag = PropTagHelper.PropTagFromIdAndType(tag.Id(), propType);
			}
			else
			{
				propType = (tag.ValueType() & (PropType)(-8193));
				tag = PropTagHelper.PropTagFromIdAndType(tag.Id(), propType);
			}
			return tag;
		}

		internal static object ReadStreamedProperty(IStorePropertyBag propertyBag, StorePropertyDefinition propertyDefinition)
		{
			if (propertyDefinition.Type == typeof(string))
			{
				using (StreamReader streamReader = new StreamReader(propertyBag.OpenPropertyStream(propertyDefinition, PropertyOpenMode.ReadOnly), Encoding.Unicode))
				{
					return streamReader.ReadToEnd();
				}
			}
			if (propertyDefinition.Type == typeof(byte[]))
			{
				using (Stream stream = propertyBag.OpenPropertyStream(propertyDefinition, PropertyOpenMode.ReadOnly))
				{
					byte[] array = new byte[stream.Length];
					stream.Read(array, 0, array.Length);
					return array;
				}
			}
			throw new InvalidRuleException(string.Format("Property {0} is not streamable", propertyDefinition));
		}

		internal static object RunMapiCode(LocalizedString context, ActionWithReturnValue action)
		{
			try
			{
				return action();
			}
			catch (MapiRetryableException e)
			{
				RuleUtil.TranslateThrow(e, context);
			}
			catch (MapiPermanentException e2)
			{
				RuleUtil.TranslateThrow(e2, context);
			}
			return null;
		}

		internal static void RunMapiCode(LocalizedString context, ActionParams action, params object[] arguments)
		{
			try
			{
				action(arguments);
			}
			catch (MapiRetryableException e)
			{
				RuleUtil.TranslateThrow(e, context);
			}
			catch (MapiPermanentException e2)
			{
				RuleUtil.TranslateThrow(e2, context);
			}
		}

		public static bool TryRunStoreCode(Action action, out Exception exception)
		{
			exception = null;
			try
			{
				action();
				return true;
			}
			catch (ExchangeDataException ex)
			{
				exception = ex;
			}
			catch (DataValidationException ex2)
			{
				exception = ex2;
			}
			catch (InvalidRuleException ex3)
			{
				exception = ex3;
			}
			catch (MapiPermanentException ex4)
			{
				exception = ex4;
			}
			catch (MapiRetryableException ex5)
			{
				exception = ex5;
			}
			catch (StoragePermanentException ex6)
			{
				exception = ex6;
			}
			catch (StorageTransientException ex7)
			{
				exception = ex7;
			}
			return false;
		}

		internal static byte[] SearchKeyFromAddress(string addressType, string address)
		{
			byte[] bytes = RuleUtil.asciiEncoding.GetBytes(addressType + ":" + address + "\0");
			for (int i = 0; i < bytes.Length; i++)
			{
				if (97 <= bytes[i] && bytes[i] <= 122)
				{
					byte[] array = bytes;
					int num = i;
					array[num] &= 223;
				}
			}
			return bytes;
		}

		public static byte[] SearchKeyFromParticipant(Participant participant)
		{
			if (null == participant)
			{
				return null;
			}
			return RuleUtil.SearchKeyFromAddress(participant.RoutingType, participant.EmailAddress);
		}

		internal static bool SetRecipients(IRuleEvaluationContext context, MessageItem message, IList<ProxyAddress> senderAddresses, AdrEntry[] recipients, bool promoteToEnvelope)
		{
			message.Recipients.Clear();
			int num = 0;
			foreach (AdrEntry entry in recipients)
			{
				Participant participant = RuleUtil.ParticipantFromAddressEntry(entry);
				if (participant != null)
				{
					if (promoteToEnvelope && RuleUtil.IsRecipientSameAsSender(senderAddresses, participant.EmailAddress))
					{
						context.TraceDebug<string>("Skipping recipient {0} because that was the original sender.", participant.EmailAddress);
					}
					else
					{
						context.TraceDebug<Participant>("Adding recipient {0}.", participant);
						Recipient recipient = message.Recipients.Add(participant);
						recipient[ItemSchema.Responsibility] = promoteToEnvelope;
						num++;
					}
				}
			}
			return num > 0;
		}

		internal static bool SetRecipients(IRuleEvaluationContext context, MessageItem message, IList<ProxyAddress> senderAddresses, RecipientCollection recipients, bool promoteToEnvelope)
		{
			int num = 0;
			message.Recipients.Clear();
			foreach (Recipient recipient in recipients)
			{
				if (promoteToEnvelope && RuleUtil.IsRecipientSameAsSender(senderAddresses, recipient.Participant.EmailAddress))
				{
					context.TraceDebug<string>("Skipping recipient {0} because that was the original sender.", recipient.Participant.EmailAddress);
				}
				else
				{
					context.TraceDebug<string>("Adding recipient {0}.", recipient.Participant.ToString());
					Recipient recipient2 = message.Recipients.Add(recipient.Participant, recipient.RecipientItemType);
					recipient2[ItemSchema.Responsibility] = promoteToEnvelope;
					num++;
				}
			}
			return num > 0;
		}

		internal static Participant ParticipantFromAddressEntry(AdrEntry entry)
		{
			string displayName = string.Empty;
			string emailAddress = string.Empty;
			string routingType = string.Empty;
			foreach (PropValue propValue in entry.Values)
			{
				PropTag propTag = propValue.PropTag;
				if (propTag != PropTag.DisplayName)
				{
					if (propTag != PropTag.AddrType)
					{
						if (propTag == PropTag.EmailAddress)
						{
							emailAddress = (string)propValue.Value;
						}
					}
					else
					{
						routingType = (string)propValue.Value;
					}
				}
				else
				{
					displayName = (string)propValue.Value;
				}
			}
			return new Participant(displayName, emailAddress, routingType);
		}

		internal static void DisableRule(Rule rule, MapiFolder mapiFolder)
		{
			rule.StateFlags &= ~RuleStateFlags.Enabled;
			RuleUtil.RunMapiCode(ServerStrings.ModifyRuleInStore, new ActionParams(RuleUtil.ModifyRule), new object[]
			{
				rule,
				mapiFolder
			});
		}

		internal static void MarkRuleInError(Rule rule, MapiFolder mapiFolder)
		{
			rule.StateFlags |= RuleStateFlags.Error;
			RuleUtil.RunMapiCode(ServerStrings.ModifyRuleInStore, new ActionParams(RuleUtil.ModifyRule), new object[]
			{
				rule,
				mapiFolder
			});
		}

		internal static bool IsRecipientSameAsSender(IList<ProxyAddress> senderAddresses, string recipientAddress)
		{
			if (senderAddresses == null)
			{
				return false;
			}
			foreach (ProxyAddress proxyAddress in senderAddresses)
			{
				if (string.Equals(proxyAddress.AddressString, recipientAddress, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private static void ModifyRule(params object[] arguments)
		{
			MapiFolder mapiFolder = (MapiFolder)arguments[1];
			mapiFolder.ModifyRules(new Rule[]
			{
				(Rule)arguments[0]
			});
		}

		private unsafe static int CompareBytes(byte* px, byte* py, int len)
		{
			for (int i = 0; i < len; i++)
			{
				if (px[i] < py[i])
				{
					return -1;
				}
				if (px[i] > py[i])
				{
					return 1;
				}
			}
			return 0;
		}

		private static ProxyAddress GetLegacyProxyAddress(IRuleEvaluationContext context, byte[] entryId)
		{
			if (entryId.Length == 0)
			{
				context.TraceDebug("GetLegacyProxyAddress: entry ID is zero-length");
				return null;
			}
			ParticipantEntryId participantEntryId = ParticipantEntryId.TryFromEntryId(entryId);
			context.TraceDebug<string>("GetLegacyProxyAddress: entry ID is {0}", participantEntryId.GetType().Name);
			Participant.Builder builder = new Participant.Builder();
			builder.SetPropertiesFrom(participantEntryId);
			Participant participant = builder.ToParticipant();
			string routingType = participant.RoutingType;
			string emailAddress = participant.EmailAddress;
			if (routingType != "EX" || string.IsNullOrEmpty(emailAddress))
			{
				context.TraceDebug<string, string>("GetLegacyProxyAddress: returning null, address is {0}:{1}", routingType ?? "(null)", emailAddress ?? "(null)");
				return null;
			}
			ProxyAddress proxyAddress = ProxyAddress.Parse(routingType, emailAddress);
			if (proxyAddress is InvalidProxyAddress)
			{
				context.TraceDebug<string>("GetLegacyProxyAddress: legacyDN {0} is not valid", emailAddress);
				return null;
			}
			context.TraceDebug<string>("GetLegacyProxyAddress: returning EX:{0}", emailAddress);
			return proxyAddress;
		}

		private static Type[] GetMultiValuePropTypes()
		{
			return new Type[]
			{
				null,
				null,
				typeof(short[]),
				typeof(int[]),
				typeof(float[]),
				typeof(double[]),
				typeof(long[]),
				typeof(double[]),
				null,
				null,
				null,
				typeof(bool[]),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				typeof(long[]),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				typeof(string[]),
				typeof(string[]),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				typeof(DateTime[]),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				typeof(Guid[]),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				typeof(byte[][])
			};
		}

		private static Type[] GetSingleValuePropTypes()
		{
			return new Type[]
			{
				null,
				null,
				typeof(short),
				typeof(int),
				typeof(float),
				typeof(double),
				typeof(long),
				typeof(double),
				null,
				null,
				null,
				typeof(bool),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				typeof(long),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				typeof(string),
				typeof(string),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				typeof(DateTime),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				typeof(Guid),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				typeof(byte[])
			};
		}

		private static void TranslateThrow(LocalizedException e, LocalizedString context)
		{
			throw StorageGlobals.TranslateMapiException(context, e, null, null, string.Empty, new object[0]);
		}

		private const PropTag TagActiveUserEntryId = (PropTag)1716650242U;

		private const PropTag TagCDOExpansionMID = (PropTag)251527188U;

		private const PropTag TagCreatorEntryId = (PropTag)1073283330U;

		private const PropTag TagDID = (PropTag)1065353236U;

		private const PropTag TagFID = PropTag.Fid;

		private const PropTag TagInstID = (PropTag)1733099540U;

		private const PropTag TagLastModifierEntryId = (PropTag)1073414402U;

		private const PropTag TagMID = PropTag.Mid;

		private const PropTag TagOriginalAuthorEntryId = PropTag.OriginalAuthorEntryId;

		private const PropTag TagOriginatorEntryId = (PropTag)1717436674U;

		private const PropTag TagParentFID = (PropTag)1732837396U;

		private const PropTag TagReportDestinationEntryId = (PropTag)1717895426U;

		private const PropTag TagSearchedFID = (PropTag)241500180U;

		private const PropTag TagSenderEntryId = PropTag.SenderEntryId;

		private const PropTag TagSourceFID = PropTag.SourceFid;

		private const PropTag TagVID = (PropTag)1065877524U;

		private static readonly Type[] multiValuePropTypes = RuleUtil.GetMultiValuePropTypes();

		private static readonly Type[] singleValuePropTypes = RuleUtil.GetSingleValuePropTypes();

		private static ASCIIEncoding asciiEncoding = new ASCIIEncoding();
	}
}
