using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ConversationPropertyAggregationStrategy
	{
		public static PropertyAggregationStrategy CreatePriorityPropertyAggregation(StorePropertyDefinition sourceProperty)
		{
			return new PropertyAggregationStrategy.SingleValuePropertyAggregation(ItemSelectionStrategy.CreateSingleSourceProperty(sourceProperty));
		}

		public static PropertyAggregationStrategy CreateAnyTruePropertyAggregation(StorePropertyDefinition sourceProperty)
		{
			return new ConversationPropertyAggregationStrategy.AnyTruePropertyAggregation(new StorePropertyDefinition[]
			{
				sourceProperty
			});
		}

		public static PropertyAggregationStrategy CreateZeroValuePropertyAggregation()
		{
			return new ConversationPropertyAggregationStrategy.ZeroValueAggregation(Array<StorePropertyDefinition>.Empty);
		}

		public static readonly PropertyAggregationStrategy CreationTimeProperty = new PropertyAggregationStrategy.CreationTimeAggregation();

		public static readonly PropertyAggregationStrategy ConversationIdProperty = new ConversationPropertyAggregationStrategy.ConversationIdAggregation();

		public static readonly PropertyAggregationStrategy ConversationTopicProperty = ConversationPropertyAggregationStrategy.CreatePriorityPropertyAggregation(ItemSchema.NormalizedSubject);

		public static readonly PropertyAggregationStrategy InstanceKeyProperty = ConversationPropertyAggregationStrategy.CreatePriorityPropertyAggregation(ItemSchema.InstanceKey);

		public static readonly PropertyAggregationStrategy PreviewProperty = ConversationPropertyAggregationStrategy.CreatePriorityPropertyAggregation(ItemSchema.Preview);

		public static readonly PropertyAggregationStrategy ItemCountProperty = new ConversationPropertyAggregationStrategy.ItemCountAggregation();

		public static readonly PropertyAggregationStrategy SizeProperty = new ConversationPropertyAggregationStrategy.ConversationSizeAggregation();

		public static readonly PropertyAggregationStrategy LastDeliveryTimeProperty = new ConversationPropertyAggregationStrategy.LastDeliveryTimeAggregation();

		public static readonly PropertyAggregationStrategy ImportanceProperty = new ConversationPropertyAggregationStrategy.ImportancePropertyAggregation();

		public static readonly PropertyAggregationStrategy TotalItemLikesProperty = ConversationPropertyAggregationStrategy.CreateZeroValuePropertyAggregation();

		public static readonly PropertyAggregationStrategy ConversationLikesProperty = ConversationPropertyAggregationStrategy.CreateZeroValuePropertyAggregation();

		public static readonly PropertyAggregationStrategy DirectParticipantsProperty = new ConversationPropertyAggregationStrategy.DirectParticipantsAggregation();

		public static readonly PropertyAggregationStrategy HasAttachmentsProperty = ConversationPropertyAggregationStrategy.CreateAnyTruePropertyAggregation(ItemSchema.HasAttachment);

		public static readonly PropertyAggregationStrategy HasIrmProperty = new ConversationPropertyAggregationStrategy.HasIrmAggregation();

		public static readonly PropertyAggregationStrategy DraftItemIdsProperty = new ConversationPropertyAggregationStrategy.DraftItemIdsAggregation();

		public static readonly PropertyAggregationStrategy IconIndexProperty = new ConversationPropertyAggregationStrategy.IconIndexAggregation();

		public static readonly PropertyAggregationStrategy UnreadCountProperty = new ConversationPropertyAggregationStrategy.UnreadCountAggregation();

		public static readonly PropertyAggregationStrategy FlagStatusProperty = new ConversationPropertyAggregationStrategy.FlagStatusAggregation();

		public static readonly PropertyAggregationStrategy RichContentProperty = new ConversationPropertyAggregationStrategy.RichContentAggregation();

		private sealed class ZeroValueAggregation : PropertyAggregationStrategy
		{
			public ZeroValueAggregation(StorePropertyDefinition[] dependencies) : base(dependencies)
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				value = 0;
				return true;
			}
		}

		private sealed class ItemCountAggregation : PropertyAggregationStrategy
		{
			public ItemCountAggregation() : base(new StorePropertyDefinition[0])
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				value = context.Sources.Count;
				return true;
			}
		}

		private sealed class DirectParticipantsAggregation : PropertyAggregationStrategy
		{
			public DirectParticipantsAggregation() : base(new StorePropertyDefinition[]
			{
				ItemSchema.From,
				ItemSchema.Sender
			})
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				Dictionary<string, Participant> dictionary = new Dictionary<string, Participant>();
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					Participant valueOrDefault = storePropertyBag.GetValueOrDefault<Participant>(ItemSchema.From, null);
					if (valueOrDefault == null)
					{
						valueOrDefault = storePropertyBag.GetValueOrDefault<Participant>(ItemSchema.Sender, null);
					}
					if (!(valueOrDefault == null))
					{
						string text = string.IsNullOrEmpty(valueOrDefault.DisplayName) ? valueOrDefault.ToString(AddressFormat.Rfc822Smtp) : valueOrDefault.DisplayName;
						if (!string.IsNullOrEmpty(text) && !dictionary.ContainsKey(text))
						{
							dictionary.Add(text, valueOrDefault);
						}
					}
				}
				if (dictionary.Count == 0)
				{
					value = null;
					return false;
				}
				value = dictionary.Values.ToArray<Participant>();
				return true;
			}
		}

		private class AnyTruePropertyAggregation : PropertyAggregationStrategy
		{
			public AnyTruePropertyAggregation(params StorePropertyDefinition[] dependencies) : base(dependencies)
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				bool flag = false;
				foreach (IStorePropertyBag source in context.Sources)
				{
					if (this.GetBoolValue(source))
					{
						flag = true;
						break;
					}
				}
				value = flag;
				return true;
			}

			public virtual bool GetBoolValue(IStorePropertyBag source)
			{
				return source.GetValueOrDefault<bool>(base.Dependencies[0].Property, false);
			}
		}

		private sealed class HasIrmAggregation : ConversationPropertyAggregationStrategy.AnyTruePropertyAggregation
		{
			public HasIrmAggregation() : base(new StorePropertyDefinition[]
			{
				InternalSchema.Flags
			})
			{
			}

			public override bool GetBoolValue(IStorePropertyBag source)
			{
				MessageFlags valueOrDefault = source.GetValueOrDefault<MessageFlags>(base.Dependencies[0].Property, MessageFlags.None);
				return (MessageFlags.IsRestricted & valueOrDefault) == MessageFlags.IsRestricted;
			}
		}

		private sealed class ConversationIdAggregation : PropertyAggregationStrategy
		{
			public ConversationIdAggregation() : base(new StorePropertyDefinition[]
			{
				InternalSchema.ConversationId,
				InternalSchema.ItemClass
			})
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				ConversationId conversationId = null;
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					ConversationId valueOrDefault = storePropertyBag.GetValueOrDefault<ConversationId>(InternalSchema.ConversationId, null);
					if (conversationId != null && valueOrDefault != null && !valueOrDefault.Equals(conversationId))
					{
						throw new ArgumentException("sources", "Property bag collection should have same conversationId");
					}
					if (conversationId == null)
					{
						conversationId = valueOrDefault;
					}
				}
				value = conversationId;
				return true;
			}
		}

		private sealed class ConversationSizeAggregation : PropertyAggregationStrategy
		{
			public ConversationSizeAggregation() : base(new StorePropertyDefinition[]
			{
				InternalSchema.Size
			})
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				int num = 0;
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					int valueOrDefault = storePropertyBag.GetValueOrDefault<int>(InternalSchema.Size, 0);
					num += valueOrDefault;
				}
				value = num;
				return true;
			}
		}

		internal sealed class LastDeliveryTimeAggregation : PropertyAggregationStrategy
		{
			public LastDeliveryTimeAggregation() : base(new StorePropertyDefinition[]
			{
				InternalSchema.ReceivedTime
			})
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				ExDateTime exDateTime = ExDateTime.MinValue;
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					ExDateTime valueOrDefault = storePropertyBag.GetValueOrDefault<ExDateTime>(InternalSchema.ReceivedTime, ExDateTime.MinValue);
					if (valueOrDefault > exDateTime)
					{
						exDateTime = valueOrDefault;
					}
				}
				value = exDateTime;
				return true;
			}
		}

		private sealed class ImportancePropertyAggregation : PropertyAggregationStrategy
		{
			public ImportancePropertyAggregation() : base(new StorePropertyDefinition[]
			{
				InternalSchema.Importance
			})
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				Importance importance = Importance.Low;
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					Importance valueOrDefault = storePropertyBag.GetValueOrDefault<Importance>(InternalSchema.Importance, Importance.Normal);
					if (valueOrDefault > importance)
					{
						importance = valueOrDefault;
						if (importance == Importance.High)
						{
							break;
						}
					}
				}
				value = importance;
				return true;
			}
		}

		private sealed class DraftItemIdsAggregation : PropertyAggregationStrategy
		{
			public DraftItemIdsAggregation() : base(new StorePropertyDefinition[]
			{
				InternalSchema.IsDraft
			})
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				List<StoreObjectId> list = new List<StoreObjectId>(context.Sources.Count);
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					if (storePropertyBag.GetValueOrDefault<bool>(InternalSchema.IsDraft, false))
					{
						byte[] valueOrDefault = storePropertyBag.GetValueOrDefault<byte[]>(InternalSchema.EntryId, null);
						if (valueOrDefault != null)
						{
							StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(valueOrDefault, StoreObjectType.Unknown);
							if (storeObjectId != null)
							{
								list.Add(storeObjectId);
							}
						}
					}
				}
				value = list.ToArray();
				return true;
			}
		}

		private sealed class IconIndexAggregation : PropertyAggregationStrategy
		{
			public IconIndexAggregation() : base(new StorePropertyDefinition[]
			{
				InternalSchema.IconIndex
			})
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				IconIndex iconIndex = IconIndex.Default;
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					IconIndex valueOrDefault = storePropertyBag.GetValueOrDefault<IconIndex>(InternalSchema.IconIndex, IconIndex.Default);
					if (valueOrDefault != IconIndex.Default && valueOrDefault != iconIndex)
					{
						iconIndex = valueOrDefault;
					}
				}
				value = iconIndex;
				return true;
			}
		}

		private sealed class UnreadCountAggregation : PropertyAggregationStrategy
		{
			public UnreadCountAggregation() : base(new StorePropertyDefinition[]
			{
				InternalSchema.IsRead
			})
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				int num = 0;
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					if (!storePropertyBag.GetValueOrDefault<bool>(InternalSchema.IsRead, true))
					{
						num++;
					}
				}
				value = num;
				return true;
			}
		}

		private sealed class FlagStatusAggregation : PropertyAggregationStrategy
		{
			public FlagStatusAggregation() : base(new StorePropertyDefinition[]
			{
				InternalSchema.FlagStatus
			})
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				FlagStatus flagStatus = FlagStatus.NotFlagged;
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					FlagStatus valueOrDefault = storePropertyBag.GetValueOrDefault<FlagStatus>(InternalSchema.FlagStatus, FlagStatus.NotFlagged);
					if (valueOrDefault > flagStatus)
					{
						flagStatus = valueOrDefault;
						if (flagStatus == FlagStatus.Flagged)
						{
							break;
						}
					}
				}
				value = flagStatus;
				return true;
			}
		}

		private sealed class RichContentAggregation : PropertyAggregationStrategy
		{
			public RichContentAggregation() : base(new StorePropertyDefinition[]
			{
				InternalSchema.RichContent
			})
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				List<short> list = new List<short>(context.Sources.Count);
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					short valueOrDefault = storePropertyBag.GetValueOrDefault<short>(InternalSchema.RichContent, 0);
					list.Add(valueOrDefault);
				}
				value = list.ToArray();
				return true;
			}
		}
	}
}
