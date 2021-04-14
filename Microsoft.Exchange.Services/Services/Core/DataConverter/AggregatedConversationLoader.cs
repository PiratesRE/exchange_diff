using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class AggregatedConversationLoader
	{
		private AggregatedConversationLoader()
		{
		}

		private static AggregatedConversationLoader Instance
		{
			get
			{
				if (AggregatedConversationLoader.instance == null)
				{
					AggregatedConversationLoader.instance = new AggregatedConversationLoader();
				}
				return AggregatedConversationLoader.instance;
			}
		}

		internal static void LoadProperties(ConversationType conversation, IStorePropertyBag aggregatedProperties, MailboxSession session, PropertyUriEnum[] properties)
		{
			foreach (PropertyUriEnum key in properties)
			{
				AggregatedConversationLoader.AggregatedConversationConverter converter = AggregatedConversationLoader.AggregatedConversationConverters[key].Converter;
				object propertyValue = converter.ConvertProperty(aggregatedProperties, session);
				AggregatedConversationLoader.AggregatedConversationConverters[key].Setter(conversation, propertyValue);
			}
		}

		internal static HashSet<PropertyDefinition> GetAggregatedConversationDependencies(PropertyUriEnum[] properties)
		{
			HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>();
			foreach (PropertyUriEnum key in properties)
			{
				AggregatedConversationLoader.AggregatedConversationConverter converter = AggregatedConversationLoader.AggregatedConversationConverters[key].Converter;
				foreach (ApplicationAggregatedProperty item in converter.Dependencies)
				{
					hashSet.Add(item);
				}
			}
			return hashSet;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static AggregatedConversationLoader()
		{
			Dictionary<PropertyUriEnum, AggregatedConversationLoader.AggregatedConversationConversion> dictionary = new Dictionary<PropertyUriEnum, AggregatedConversationLoader.AggregatedConversationConversion>();
			dictionary.Add(PropertyUriEnum.ConversationId, new AggregatedConversationLoader.AggregatedConversationConversion(new AggregatedConversationLoader.ConversationIdConverter(), delegate(ConversationType c, object p)
			{
				c.ConversationId = (ItemId)p;
			}));
			dictionary.Add(PropertyUriEnum.Topic, new AggregatedConversationLoader.AggregatedConversationConversion(new AggregatedConversationLoader.StringValueConverter(AggregatedConversationSchema.Topic), delegate(ConversationType c, object p)
			{
				c.ConversationTopic = (string)p;
			}));
			dictionary.Add(PropertyUriEnum.ConversationPreview, new AggregatedConversationLoader.AggregatedConversationConversion(new AggregatedConversationLoader.StringValueConverter(AggregatedConversationSchema.Preview), delegate(ConversationType c, object p)
			{
				c.Preview = (string)p;
			}));
			dictionary.Add(PropertyUriEnum.ConversationHasAttachments, new AggregatedConversationLoader.AggregatedConversationConversion(new AggregatedConversationLoader.BoolConverter(AggregatedConversationSchema.HasAttachments), delegate(ConversationType c, object p)
			{
				c.HasAttachments = new bool?((bool)p);
				c.HasAttachmentsSpecified = true;
			}));
			dictionary.Add(PropertyUriEnum.ConversationHasIrm, new AggregatedConversationLoader.AggregatedConversationConversion(new AggregatedConversationLoader.BoolConverter(AggregatedConversationSchema.HasIrm), delegate(ConversationType c, object p)
			{
				c.HasIrm = new bool?((bool)p);
				c.HasIrmSpecified = true;
			}));
			dictionary.Add(PropertyUriEnum.ConversationUniqueSenders, new AggregatedConversationLoader.AggregatedConversationConversion(new AggregatedConversationLoader.ConversationSendersConverter(), delegate(ConversationType c, object p)
			{
				c.UniqueSenders = (string[])p;
			}));
			dictionary.Add(PropertyUriEnum.ConversationLastDeliveryTime, new AggregatedConversationLoader.AggregatedConversationConversion(new AggregatedConversationLoader.DateTimeConverter(AggregatedConversationSchema.LastDeliveryTime), delegate(ConversationType c, object p)
			{
				c.LastDeliveryTime = (string)p;
			}));
			dictionary.Add(PropertyUriEnum.ConversationLastModifiedTime, new AggregatedConversationLoader.AggregatedConversationConversion(new AggregatedConversationLoader.DateTimeConverter(AggregatedConversationSchema.LastDeliveryTime), delegate(ConversationType c, object p)
			{
				c.LastModifiedTime = (string)p;
			}));
			dictionary.Add(PropertyUriEnum.ConversationGlobalMessageCount, new AggregatedConversationLoader.AggregatedConversationConversion(new AggregatedConversationLoader.IntValueConverter(AggregatedConversationSchema.ItemCount), delegate(ConversationType c, object p)
			{
				c.MessageCount = (c.GlobalMessageCount = new int?((int)p));
				c.MessageCountSpecified = (c.GlobalMessageCountSpecified = true);
			}));
			dictionary.Add(PropertyUriEnum.ConversationSize, new AggregatedConversationLoader.AggregatedConversationConversion(new AggregatedConversationLoader.IntValueConverter(AggregatedConversationSchema.Size), delegate(ConversationType c, object p)
			{
				c.Size = new int?((int)p);
			}));
			dictionary.Add(PropertyUriEnum.ConversationGlobalItemIds, new AggregatedConversationLoader.AggregatedConversationConversion(new AggregatedConversationLoader.ConversationItemIdsConverter(), delegate(ConversationType c, object p)
			{
				c.ItemIds = (c.GlobalItemIds = (BaseItemId[])p);
			}));
			dictionary.Add(PropertyUriEnum.ConversationUnreadCount, new AggregatedConversationLoader.AggregatedConversationConversion(new AggregatedConversationLoader.ZeroConverter(), delegate(ConversationType c, object p)
			{
				c.UnreadCount = new int?((int)p);
				c.UnreadCountSpecified = true;
			}));
			dictionary.Add(PropertyUriEnum.ConversationGlobalUnreadCount, new AggregatedConversationLoader.AggregatedConversationConversion(new AggregatedConversationLoader.ZeroConverter(), delegate(ConversationType c, object p)
			{
				c.GlobalUnreadCount = new int?((int)p);
				c.GlobalUnreadCountSpecified = true;
			}));
			dictionary.Add(PropertyUriEnum.ConversationInstanceKey, new AggregatedConversationLoader.AggregatedConversationConversion(new AggregatedConversationLoader.ByteArrayConverter(AggregatedConversationSchema.InstanceKey), delegate(ConversationType c, object p)
			{
				c.InstanceKey = (byte[])p;
			}));
			dictionary.Add(PropertyUriEnum.ConversationItemClasses, new AggregatedConversationLoader.AggregatedConversationConversion(new AggregatedConversationLoader.ConversationItemClassesConverter(), delegate(ConversationType c, object p)
			{
				c.ItemClasses = (string[])p;
			}));
			dictionary.Add(PropertyUriEnum.ConversationImportance, new AggregatedConversationLoader.AggregatedConversationConversion(new AggregatedConversationLoader.ConversationImportanceConverter(), delegate(ConversationType c, object p)
			{
				c.Importance = (ImportanceType)p;
			}));
			AggregatedConversationLoader.AggregatedConversationConverters = dictionary;
		}

		private static AggregatedConversationLoader instance;

		private static Dictionary<PropertyUriEnum, AggregatedConversationLoader.AggregatedConversationConversion> AggregatedConversationConverters;

		private class AggregatedConversationConversion
		{
			internal AggregatedConversationLoader.AggregatedConversationConverter Converter { get; private set; }

			internal AggregatedConversationPropertySetter Setter { get; private set; }

			internal AggregatedConversationConversion(AggregatedConversationLoader.AggregatedConversationConverter converter, AggregatedConversationPropertySetter setter)
			{
				this.Converter = converter;
				this.Setter = setter;
			}
		}

		private abstract class AggregatedConversationConverter
		{
			internal ApplicationAggregatedProperty[] Dependencies { get; set; }

			public AggregatedConversationConverter(ApplicationAggregatedProperty[] dependencies)
			{
				this.Dependencies = dependencies;
			}

			public abstract object ConvertProperty(IStorePropertyBag aggregatedConversation, MailboxSession session);
		}

		private class ConversationIdConverter : AggregatedConversationLoader.AggregatedConversationConverter
		{
			public ConversationIdConverter() : base(new ApplicationAggregatedProperty[]
			{
				AggregatedConversationSchema.Id
			})
			{
			}

			public override object ConvertProperty(IStorePropertyBag aggregatedConversation, MailboxSession session)
			{
				ConversationId valueOrDefault = aggregatedConversation.GetValueOrDefault<ConversationId>(AggregatedConversationSchema.Id, null);
				string id = IdConverter.ConversationIdToEwsId(session.MailboxGuid, valueOrDefault);
				return new ItemId(id, null);
			}
		}

		private class ConversationItemIdsConverter : AggregatedConversationLoader.AggregatedConversationConverter
		{
			public ConversationItemIdsConverter() : base(new ApplicationAggregatedProperty[]
			{
				AggregatedConversationSchema.ItemIds
			})
			{
			}

			public override object ConvertProperty(IStorePropertyBag aggregatedConversation, MailboxSession session)
			{
				MailboxId mailboxId = new MailboxId(session);
				StoreObjectId[] valueOrDefault = aggregatedConversation.GetValueOrDefault<StoreObjectId[]>(AggregatedConversationSchema.ItemIds, null);
				BaseItemId[] array = new BaseItemId[valueOrDefault.Length];
				for (int i = 0; i < valueOrDefault.Length; i++)
				{
					ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(valueOrDefault[i], mailboxId, null);
					array[i] = new ItemId(concatenatedId.Id, concatenatedId.ChangeKey);
				}
				return array;
			}
		}

		private class ConversationItemClassesConverter : AggregatedConversationLoader.AggregatedConversationConverter
		{
			public ConversationItemClassesConverter() : base(new ApplicationAggregatedProperty[]
			{
				AggregatedConversationSchema.ItemClasses
			})
			{
			}

			public override object ConvertProperty(IStorePropertyBag aggregatedConversation, MailboxSession session)
			{
				return aggregatedConversation.GetValueOrDefault<string[]>(AggregatedConversationSchema.ItemClasses, null);
			}
		}

		private class ConversationImportanceConverter : AggregatedConversationLoader.AggregatedConversationConverter
		{
			public ConversationImportanceConverter() : base(new ApplicationAggregatedProperty[0])
			{
			}

			public override object ConvertProperty(IStorePropertyBag aggregatedConversation, MailboxSession session)
			{
				return (ImportanceType)aggregatedConversation.GetValueOrDefault<Importance>(AggregatedConversationSchema.Importance, Importance.Normal);
			}
		}

		private class ConversationSendersConverter : AggregatedConversationLoader.AggregatedConversationConverter
		{
			public ConversationSendersConverter() : base(new ApplicationAggregatedProperty[]
			{
				AggregatedConversationSchema.DirectParticipants
			})
			{
			}

			public override object ConvertProperty(IStorePropertyBag aggregatedConversation, MailboxSession session)
			{
				Participant[] valueOrDefault = aggregatedConversation.GetValueOrDefault<Participant[]>(AggregatedConversationSchema.DirectParticipants, null);
				string[] array = null;
				if (valueOrDefault != null && valueOrDefault.Length > 0)
				{
					array = new string[valueOrDefault.Length];
					for (int i = 0; i < valueOrDefault.Length; i++)
					{
						string text = string.IsNullOrEmpty(valueOrDefault[i].DisplayName) ? valueOrDefault[i].ToString(AddressFormat.Rfc822Smtp) : valueOrDefault[i].DisplayName;
						array[i] = text;
					}
				}
				return array;
			}
		}

		private class DateTimeConverter : AggregatedConversationLoader.AggregatedConversationConverter
		{
			public DateTimeConverter(ApplicationAggregatedProperty property) : base(new ApplicationAggregatedProperty[]
			{
				property
			})
			{
			}

			public override object ConvertProperty(IStorePropertyBag aggregatedConversation, MailboxSession session)
			{
				ExDateTime valueOrDefault = aggregatedConversation.GetValueOrDefault<ExDateTime>(base.Dependencies[0], ExDateTime.Now);
				return ExDateTimeConverter.ToOffsetXsdDateTime(valueOrDefault, valueOrDefault.TimeZone);
			}
		}

		private class StringValueConverter : AggregatedConversationLoader.AggregatedConversationConverter
		{
			public StringValueConverter(ApplicationAggregatedProperty property) : base(new ApplicationAggregatedProperty[]
			{
				property
			})
			{
			}

			public override object ConvertProperty(IStorePropertyBag aggregatedConversation, MailboxSession session)
			{
				return aggregatedConversation.GetValueOrDefault<string>(base.Dependencies[0], string.Empty);
			}
		}

		private class BoolConverter : AggregatedConversationLoader.AggregatedConversationConverter
		{
			public BoolConverter(ApplicationAggregatedProperty property) : base(new ApplicationAggregatedProperty[]
			{
				property
			})
			{
			}

			public override object ConvertProperty(IStorePropertyBag aggregatedConversation, MailboxSession session)
			{
				return aggregatedConversation.GetValueOrDefault<bool>(base.Dependencies[0], false);
			}
		}

		private class IntValueConverter : AggregatedConversationLoader.AggregatedConversationConverter
		{
			public IntValueConverter(ApplicationAggregatedProperty property) : base(new ApplicationAggregatedProperty[]
			{
				property
			})
			{
			}

			public override object ConvertProperty(IStorePropertyBag aggregatedConversation, MailboxSession session)
			{
				return aggregatedConversation.GetValueOrDefault<int>(base.Dependencies[0], 0);
			}
		}

		private class ByteArrayConverter : AggregatedConversationLoader.AggregatedConversationConverter
		{
			public ByteArrayConverter(ApplicationAggregatedProperty property) : base(new ApplicationAggregatedProperty[]
			{
				property
			})
			{
			}

			public override object ConvertProperty(IStorePropertyBag aggregatedConversation, MailboxSession session)
			{
				return aggregatedConversation.GetValueOrDefault<byte[]>(base.Dependencies[0], null);
			}
		}

		private class ZeroConverter : AggregatedConversationLoader.AggregatedConversationConverter
		{
			public ZeroConverter() : base(new ApplicationAggregatedProperty[0])
			{
			}

			public override object ConvertProperty(IStorePropertyBag aggregatedConversation, MailboxSession session)
			{
				return 0;
			}
		}
	}
}
