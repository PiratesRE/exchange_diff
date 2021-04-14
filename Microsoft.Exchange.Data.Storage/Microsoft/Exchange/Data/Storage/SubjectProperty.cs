using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class SubjectProperty : SmartPropertyDefinition
	{
		internal SubjectProperty() : base("SubjectProperty", typeof(string), PropertyFlags.None, Array<PropertyDefinitionConstraint>.Empty, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.MapiSubject, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.NormalizedSubjectInternal, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.SubjectPrefixInternal, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.ItemClass, PropertyDependencyType.NeedToReadForWrite),
			new PropertyDependency(InternalSchema.ReplyForwardStatus, PropertyDependencyType.NeedToReadForWrite)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.SubjectPrefixInternal);
			string valueOrDefault2 = propertyBag.GetValueOrDefault<string>(InternalSchema.NormalizedSubjectInternal);
			if (valueOrDefault == null && valueOrDefault2 == null)
			{
				return propertyBag.GetValue(InternalSchema.MapiSubject);
			}
			return valueOrDefault + valueOrDefault2;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			string text = (string)value;
			string text2 = propertyBag.GetValue(InternalSchema.SubjectPrefixInternal) as string;
			string text3 = propertyBag.GetValue(InternalSchema.NormalizedSubjectInternal) as string;
			if (text2 != null && text3 != null && text == text2 + text3)
			{
				return;
			}
			string propertyValue;
			string text4;
			SubjectProperty.ComputeSubjectPrefix(text, out propertyValue, out text4);
			propertyBag.SetValueWithFixup(InternalSchema.SubjectPrefixInternal, propertyValue);
			propertyBag.SetValueWithFixup(InternalSchema.NormalizedSubjectInternal, text4);
			propertyBag.SetValueWithFixup(InternalSchema.MapiSubject, text);
			if (text4 != text3)
			{
				MessageItem messageItem = propertyBag.Context.StoreObject as MessageItem;
				if (messageItem != null)
				{
					string itemClass = propertyBag.GetValue(InternalSchema.ItemClass) as string;
					if (!ObjectClass.IsPost(itemClass))
					{
						messageItem.ConversationTopic = text4;
						if (!string.IsNullOrEmpty(text3))
						{
							messageItem.ConversationIndex = ConversationIndex.CreateNew().ToByteArray();
							if (messageItem.MessageResponseType == MessageResponseType.None)
							{
								SubjectProperty.ClearReplyForwardProperties(messageItem);
								return;
							}
						}
						else if (messageItem.GetValueOrDefault<byte[]>(InternalSchema.ConversationIndex) == null)
						{
							messageItem.ConversationIndex = ConversationIndex.CreateNew().ToByteArray();
						}
					}
				}
			}
		}

		protected override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			propertyBag.Delete(InternalSchema.SubjectPrefixInternal);
			propertyBag.Delete(InternalSchema.NormalizedSubjectInternal);
			propertyBag.Delete(InternalSchema.MapiSubject);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			TextFilter textFilter = filter as TextFilter;
			if (comparisonFilter != null)
			{
				return new ComparisonFilter(comparisonFilter.ComparisonOperator, InternalSchema.MapiSubject, (string)comparisonFilter.PropertyValue);
			}
			if (textFilter != null)
			{
				return new TextFilter(InternalSchema.MapiSubject, textFilter.Text, textFilter.MatchOptions, textFilter.MatchFlags);
			}
			if (filter is ExistsFilter)
			{
				return new ExistsFilter(InternalSchema.MapiSubject);
			}
			return base.SmartFilterToNativeFilter(filter);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			SinglePropertyFilter singlePropertyFilter = filter as SinglePropertyFilter;
			if (singlePropertyFilter != null && singlePropertyFilter.Property.Equals(InternalSchema.MapiSubject))
			{
				ComparisonFilter comparisonFilter = filter as ComparisonFilter;
				TextFilter textFilter = filter as TextFilter;
				if (comparisonFilter != null)
				{
					return new ComparisonFilter(comparisonFilter.ComparisonOperator, this, (string)comparisonFilter.PropertyValue);
				}
				if (textFilter != null)
				{
					return new TextFilter(this, textFilter.Text, textFilter.MatchOptions, textFilter.MatchFlags);
				}
				if (filter is ExistsFilter)
				{
					return new ExistsFilter(this);
				}
			}
			return null;
		}

		internal override void RegisterFilterTranslation()
		{
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(ComparisonFilter));
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(TextFilter));
			FilterRestrictionConverter.RegisterFilterTranslation(this, typeof(ExistsFilter));
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.All;
			}
		}

		protected override NativeStorePropertyDefinition GetSortProperty()
		{
			return InternalSchema.NormalizedSubjectInternal;
		}

		internal static void ComputeSubjectPrefix(string subject, out string prefix, out string normalized)
		{
			int num = Math.Min(subject.Length, 4);
			int num2 = -1;
			for (int i = 0; i < num; i++)
			{
				if (subject[i] == ':' && i + 1 < subject.Length && subject[i + 1] == ' ')
				{
					num2 = i + 1;
					break;
				}
				if (!char.IsLetter(subject[i]))
				{
					break;
				}
			}
			if (num2 > 0)
			{
				prefix = subject.Substring(0, num2 + 1);
				normalized = subject.Substring(num2 + 1);
				return;
			}
			prefix = string.Empty;
			normalized = subject;
		}

		internal static string ExtractPrefixUsingNormalizedSubject(string mapiSubject, string normalizedSubject)
		{
			if (mapiSubject.EndsWith(normalizedSubject, StringComparison.Ordinal))
			{
				return mapiSubject.Substring(0, mapiSubject.Length - normalizedSubject.Length);
			}
			return null;
		}

		internal static void ModifySubjectProperty(Item item, NativeStorePropertyDefinition property, string value)
		{
			SubjectProperty.ModifySubjectProperty(item.PropertyBag, property, value);
		}

		internal static void ModifySubjectProperty(PropertyBag propertyBag, NativeStorePropertyDefinition property, string value)
		{
			SubjectProperty.ModifySubjectProperty((PropertyBag.BasicPropertyStore)propertyBag, property, value);
		}

		internal static void ModifySubjectProperty(PropertyBag.BasicPropertyStore item, NativeStorePropertyDefinition property, string value)
		{
			string text = item.GetValue(InternalSchema.SubjectPrefixInternal) as string;
			string text2 = item.GetValue(InternalSchema.NormalizedSubjectInternal) as string;
			string text3 = item.GetValue(InternalSchema.MapiSubject) as string;
			if (property == InternalSchema.NormalizedSubjectInternal)
			{
				text2 = value;
				if (text3 != null)
				{
					string text4 = SubjectProperty.ExtractPrefixUsingNormalizedSubject(text3, text2);
					if (text4 != null)
					{
						text = text4;
					}
				}
				if (text == null)
				{
					text = string.Empty;
				}
			}
			else if (property == InternalSchema.SubjectPrefixInternal)
			{
				text = value;
				if (text3 != null && text3.StartsWith(text, StringComparison.Ordinal))
				{
					text2 = text3.Substring(text.Length);
				}
				if (text2 == null)
				{
					text2 = string.Empty;
				}
			}
			else
			{
				if (property != InternalSchema.MapiSubject)
				{
					throw new ArgumentException("Not a supported subject property", "property");
				}
				if (!string.IsNullOrEmpty(text) && value.StartsWith(text, StringComparison.Ordinal))
				{
					text2 = value.Substring(text.Length);
				}
				else if (!string.IsNullOrEmpty(text2))
				{
					string text5 = SubjectProperty.ExtractPrefixUsingNormalizedSubject(value, text2);
					if (text5 != null)
					{
						text = text5;
					}
					else
					{
						SubjectProperty.ComputeSubjectPrefix(value, out text, out text2);
					}
				}
				else
				{
					SubjectProperty.ComputeSubjectPrefix(value, out text, out text2);
				}
			}
			text3 = text + text2;
			item.SetValueWithFixup(InternalSchema.SubjectPrefixInternal, text);
			item.SetValueWithFixup(InternalSchema.NormalizedSubjectInternal, text2);
			item.SetValueWithFixup(InternalSchema.MapiSubject, text3);
			string itemClass = item.GetValue(InternalSchema.ItemClass) as string;
			if (!ObjectClass.IsPost(itemClass))
			{
				item.SetValueWithFixup(InternalSchema.ConversationTopic, text2);
			}
		}

		internal static void TruncateSubject(Item item, int limit)
		{
			SubjectProperty.TruncateSubject(item.PropertyBag, limit);
		}

		internal static bool TruncateSubject(PropertyBag propertyBag, int limit)
		{
			bool result = false;
			string text = propertyBag.TryGetProperty(InternalSchema.Subject) as string;
			if (text != null && SubjectProperty.TruncateSubject(ref text, limit))
			{
				SubjectProperty.ModifySubjectProperty((PropertyBag.BasicPropertyStore)propertyBag, InternalSchema.MapiSubject, text);
				result = true;
			}
			return result;
		}

		internal static bool TruncateSubject(ref string subject, int limit)
		{
			if (subject.Length > limit)
			{
				int num = limit - "...".Length;
				bool flag = true;
				if (num < 0)
				{
					num = limit;
					flag = false;
				}
				if (char.IsHighSurrogate(subject[num - 1]))
				{
					num--;
				}
				subject = subject.Substring(0, num);
				if (flag)
				{
					subject += "...";
				}
				return true;
			}
			return false;
		}

		private static void ClearReplyForwardProperties(MessageItem message)
		{
			message.MessageResponseType = MessageResponseType.None;
			message.ParentMessageId = null;
			message[InternalSchema.InReplyTo] = string.Empty;
			message[InternalSchema.InternetReferences] = string.Empty;
			message[InternalSchema.ReplyForwardStatus] = string.Empty;
		}

		private const string TruncationString = "...";

		private const int MaxPrefixLength = 4;
	}
}
