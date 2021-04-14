using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class AbstractInboundConverter
	{
		protected AbstractInboundConverter(InboundMessageWriter writer, AbstractInboundConverter.IPromotionRule defaultPromotionRule)
		{
			this.messageWriter = writer;
			this.defaultPromotionRule = defaultPromotionRule;
		}

		protected abstract bool IsLargeValue();

		protected abstract object ReadValue();

		protected abstract Stream OpenValueReadStream(out int skipTrailingNulls);

		protected abstract void PromoteAttachDataObject();

		protected abstract void PromoteBodyProperty(StorePropertyDefinition property);

		protected abstract bool CanPromoteMimeOnlyProperties();

		protected virtual void PromoteInternetCpidProperty()
		{
			this.PromoteProperty(InternalSchema.InternetCpid, false);
		}

		protected virtual void PromoteMessageClass()
		{
			this.PromoteProperty(InternalSchema.ItemClass, false);
		}

		protected AbstractInboundConverter.IPromotionRule GetPropertyPromotionRule(NativeStorePropertyDefinition property)
		{
			AbstractInboundConverter.IPromotionRule result = null;
			if (this.rules.TryGetValue(property, out result))
			{
				return result;
			}
			return this.defaultPromotionRule;
		}

		protected NativeStorePropertyDefinition CreatePropertyDefinition(TnefPropertyTag propertyTag, TnefNameId? namedProperty)
		{
			PropType tnefType = (PropType)propertyTag.TnefType;
			if (tnefType == PropType.Error || tnefType == PropType.Null || tnefType == PropType.ObjectArray || tnefType == PropType.Unspecified)
			{
				return null;
			}
			if (tnefType == PropType.Object && propertyTag != TnefPropertyTag.AttachDataObj)
			{
				return null;
			}
			if (namedProperty == null)
			{
				return PropertyTagPropertyDefinition.InternalCreateCustom(string.Empty, (PropTag)propertyTag, PropertyFlags.None, NativeStorePropertyDefinition.TypeCheckingFlag.DoNotCreateInvalidType);
			}
			Guid propertySetGuid = namedProperty.Value.PropertySetGuid;
			if (namedProperty.Value.Kind == TnefNameIdKind.Id)
			{
				return GuidIdPropertyDefinition.InternalCreateCustom(string.Empty, tnefType, propertySetGuid, namedProperty.Value.Id, PropertyFlags.None, NativeStorePropertyDefinition.TypeCheckingFlag.DoNotCreateInvalidType, new PropertyDefinitionConstraint[0]);
			}
			string name = namedProperty.Value.Name;
			if (!GuidNamePropertyDefinition.IsValidName(propertySetGuid, name))
			{
				return null;
			}
			return GuidNamePropertyDefinition.InternalCreateCustom(string.Empty, tnefType, propertySetGuid, name, PropertyFlags.None, NativeStorePropertyDefinition.TypeCheckingFlag.DoNotCreateInvalidType, new PropertyDefinitionConstraint[0]);
		}

		protected void PromoteProperty(StorePropertyDefinition property, bool allowLargeProperties)
		{
			if (allowLargeProperties && this.IsLargeValue())
			{
				this.PromoteLargeProperty(property);
				return;
			}
			object value = this.ReadValue();
			this.SetProperty(property, value);
		}

		protected void SetProperty(StorePropertyDefinition property, object value)
		{
			this.messageWriter.SetProperty(property, value);
		}

		protected void PromoteAddressProperty(StorePropertyDefinition property)
		{
			object value = this.ReadValue();
			this.messageWriter.SetAddressProperty(property, value);
		}

		protected void PromoteLargeProperty(StorePropertyDefinition property)
		{
			NativeStorePropertyDefinition nativeStorePropertyDefinition = property as NativeStorePropertyDefinition;
			if (nativeStorePropertyDefinition == null)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundTnefTracer, "AbstractInboundConverter::ReadSmallPropertyValue: non-native property");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent);
			}
			if (nativeStorePropertyDefinition.MapiPropertyType != PropType.String && nativeStorePropertyDefinition.MapiPropertyType != PropType.Binary)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundTnefTracer, "AbstractInboundConverter::ReadSmallPropertyValue: non-streamable property");
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent);
			}
			int trailingNulls = 0;
			using (Stream stream = this.OpenValueReadStream(out trailingNulls))
			{
				using (Stream stream2 = this.OpenPropertyStream(property))
				{
					Util.StreamHandler.CopyStreamData(stream, stream2, null, trailingNulls);
				}
			}
		}

		protected void PromoteTimeZoneProperty(StorePropertyDefinition property)
		{
			string text = this.ReadValue() as string;
			if (!string.IsNullOrEmpty(text) && text.EndsWith("\r\n"))
			{
				text = text.Substring(0, text.Length - 2);
			}
			this.SetProperty(property, text);
		}

		private Stream OpenPropertyStream(StorePropertyDefinition property)
		{
			return this.MessageWriter.OpenPropertyStream(property);
		}

		protected InboundConversionOptions ConversionOptions
		{
			get
			{
				return this.MessageWriter.ConversionOptions;
			}
		}

		protected bool IsSenderTrusted
		{
			get
			{
				return this.ConversionOptions.IsSenderTrusted;
			}
		}

		protected ConversionComponentType CurrentComponentType
		{
			get
			{
				return this.MessageWriter.ComponentType;
			}
		}

		protected InboundMessageWriter MessageWriter
		{
			get
			{
				return this.messageWriter;
			}
		}

		protected ICoreItem CoreItem
		{
			get
			{
				return this.MessageWriter.CoreItem;
			}
		}

		protected bool IsTopLevelMessage
		{
			get
			{
				return this.MessageWriter.IsTopLevelMessage;
			}
		}

		private static void AddPromotionRule(Dictionary<NativeStorePropertyDefinition, AbstractInboundConverter.IPromotionRule> rulesTable, AbstractInboundConverter.IPromotionRule rule, params NativeStorePropertyDefinition[] properties)
		{
			foreach (NativeStorePropertyDefinition key in properties)
			{
				rulesTable[key] = rule;
			}
		}

		private static void AddAddressRule(Dictionary<NativeStorePropertyDefinition, AbstractInboundConverter.IPromotionRule> rulesTable)
		{
			HashSet<NativeStorePropertyDefinition> allCacheProperties = ConversionAddressCache.AllCacheProperties;
			AbstractInboundConverter.AddressPromotionRule value = new AbstractInboundConverter.AddressPromotionRule();
			foreach (NativeStorePropertyDefinition key in allCacheProperties)
			{
				AbstractInboundConverter.IPromotionRule promotionRule = null;
				if (!rulesTable.TryGetValue(key, out promotionRule))
				{
					rulesTable[key] = value;
				}
			}
		}

		private static Dictionary<NativeStorePropertyDefinition, AbstractInboundConverter.IPromotionRule> CreateRulesTable()
		{
			Dictionary<NativeStorePropertyDefinition, AbstractInboundConverter.IPromotionRule> dictionary = new Dictionary<NativeStorePropertyDefinition, AbstractInboundConverter.IPromotionRule>();
			AbstractInboundConverter.AddPromotionRule(dictionary, null, new NativeStorePropertyDefinition[]
			{
				InternalSchema.AttachNum
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, null, new NativeStorePropertyDefinition[]
			{
				InternalSchema.NativeBlockStatus
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, null, new NativeStorePropertyDefinition[]
			{
				InternalSchema.NativeBodyInfo
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, null, new NativeStorePropertyDefinition[]
			{
				InternalSchema.XMsExchOrganizationAVStampMailbox
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, null, new NativeStorePropertyDefinition[]
			{
				InternalSchema.QuarantineOriginalSender
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, null, new NativeStorePropertyDefinition[]
			{
				InternalSchema.LocallyDelivered,
				InternalSchema.EntryId
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, new AbstractInboundConverter.CustomRule(delegate(AbstractInboundConverter converter, NativeStorePropertyDefinition property)
			{
				if (converter.CanPromoteMimeOnlyProperties())
				{
					converter.PromoteProperty(property, false);
				}
			}), new NativeStorePropertyDefinition[]
			{
				InternalSchema.SpamConfidenceLevel,
				InternalSchema.ContentFilterPcl,
				InternalSchema.SenderIdStatus,
				InternalSchema.PurportedSenderDomain,
				InternalSchema.IsClassified,
				InternalSchema.Classification,
				InternalSchema.ClassificationDescription,
				InternalSchema.ClassificationGuid,
				InternalSchema.ClassificationKeep,
				InternalSchema.XMsExchOrganizationAuthAs,
				InternalSchema.XMsExchOrganizationAuthDomain,
				InternalSchema.XMsExchOrganizationAuthMechanism,
				InternalSchema.XMsExchOrganizationAuthSource,
				InternalSchema.ApprovalAllowedDecisionMakers,
				InternalSchema.ApprovalRequestor
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, new AbstractInboundConverter.ComponentSpecificPromotionRule(ConversionComponentType.Message), new NativeStorePropertyDefinition[]
			{
				InternalSchema.UrlCompName
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, new AbstractInboundConverter.ComponentSpecificPromotionRule(ConversionComponentType.Recipient), new NativeStorePropertyDefinition[]
			{
				InternalSchema.ObjectType
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, new AbstractInboundConverter.ComponentSpecificPromotionRule(ConversionComponentType.FileAttachment), new NativeStorePropertyDefinition[]
			{
				InternalSchema.IsContactPhoto,
				InternalSchema.AttachCalendarHidden
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, null, new NativeStorePropertyDefinition[]
			{
				InternalSchema.SentMailSvrEId,
				InternalSchema.StoreEntryId,
				InternalSchema.StoreRecordKey,
				InternalSchema.ParentEntryId,
				InternalSchema.SourceKey,
				InternalSchema.CreatorEntryId,
				InternalSchema.LastModifierEntryId,
				InternalSchema.MdbProvider,
				InternalSchema.MappingSignature,
				InternalSchema.UrlCompNamePostfix,
				InternalSchema.MID,
				InternalSchema.Associated,
				InternalSchema.Size,
				InternalSchema.SentMailSvrEId,
				InternalSchema.SentMailEntryId,
				InternalSchema.PredictedActionsInternal,
				InternalSchema.GroupingActionsDeprecated,
				InternalSchema.PredictedActionsSummaryDeprecated,
				InternalSchema.AttachSize
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, new AbstractInboundConverter.SmartPropertyPromotionRule(InternalSchema.AutoResponseSuppress, ConversionComponentType.Message, true), new NativeStorePropertyDefinition[]
			{
				InternalSchema.AutoResponseSuppressInternal
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, new AbstractInboundConverter.SmartPropertyPromotionRule(InternalSchema.IsDeliveryReceiptRequested, ConversionComponentType.Message, true), new NativeStorePropertyDefinition[]
			{
				InternalSchema.IsDeliveryReceiptRequestedInternal
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, new AbstractInboundConverter.SmartPropertyPromotionRule(InternalSchema.IsNonDeliveryReceiptRequested, ConversionComponentType.Message, true), new NativeStorePropertyDefinition[]
			{
				InternalSchema.IsNonDeliveryReceiptRequestedInternal
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, new AbstractInboundConverter.SmartPropertyPromotionRule(InternalSchema.IsReadReceiptRequested, ConversionComponentType.Message, true), new NativeStorePropertyDefinition[]
			{
				InternalSchema.IsReadReceiptRequestedInternal
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, new AbstractInboundConverter.SmartPropertyPromotionRule(InternalSchema.IsNotReadReceiptRequested, ConversionComponentType.Message, true), new NativeStorePropertyDefinition[]
			{
				InternalSchema.IsNotReadReceiptRequestedInternal
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, new AbstractInboundConverter.CustomRule(delegate(AbstractInboundConverter converter, NativeStorePropertyDefinition property)
			{
				if (converter.IsLargeValue())
				{
					StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundTnefTracer, "AbstractInboundConverter.ParseTnefProperty - subject value too big, ignoring...");
					return;
				}
				string text = (string)converter.ReadValue();
				if (text != null)
				{
					converter.messageWriter.SetSubjectProperty(property, text);
				}
			}), new NativeStorePropertyDefinition[]
			{
				InternalSchema.MapiSubject,
				InternalSchema.SubjectPrefixInternal,
				InternalSchema.NormalizedSubjectInternal
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, new AbstractInboundConverter.CustomRule(delegate(AbstractInboundConverter converter, NativeStorePropertyDefinition property)
			{
				if (converter.CurrentComponentType == ConversionComponentType.Message && !converter.ConversionOptions.ClearCategories)
				{
					converter.PromoteProperty(property, false);
				}
			}), new NativeStorePropertyDefinition[]
			{
				InternalSchema.Categories
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, new AbstractInboundConverter.CustomRule(delegate(AbstractInboundConverter converter, NativeStorePropertyDefinition property)
			{
				converter.PromoteInternetCpidProperty();
			}), new NativeStorePropertyDefinition[]
			{
				InternalSchema.MapiInternetCpid
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, new AbstractInboundConverter.CustomRule(delegate(AbstractInboundConverter converter, NativeStorePropertyDefinition property)
			{
				if (converter.CurrentComponentType == ConversionComponentType.FileAttachment)
				{
					converter.PromoteProperty(property, true);
					return;
				}
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.InvalidTnef, null);
			}), new NativeStorePropertyDefinition[]
			{
				InternalSchema.AttachDataBin
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, new AbstractInboundConverter.CustomRule(delegate(AbstractInboundConverter converter, NativeStorePropertyDefinition property)
			{
				if (converter.CurrentComponentType == ConversionComponentType.FileAttachment)
				{
					converter.PromoteAttachDataObject();
					return;
				}
				throw new ConversionFailedException(ConversionFailureReason.CorruptContent, ServerStrings.InvalidTnef, null);
			}), new NativeStorePropertyDefinition[]
			{
				InternalSchema.AttachDataObj
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, new AbstractInboundConverter.CustomRule(delegate(AbstractInboundConverter converter, NativeStorePropertyDefinition property)
			{
				converter.PromoteMessageClass();
			}), new NativeStorePropertyDefinition[]
			{
				InternalSchema.ItemClass
			});
			AbstractInboundConverter.AddPromotionRule(dictionary, new AbstractInboundConverter.CustomRule(delegate(AbstractInboundConverter converter, NativeStorePropertyDefinition property)
			{
				converter.PromoteBodyProperty(property);
			}), new NativeStorePropertyDefinition[]
			{
				InternalSchema.TextBody,
				InternalSchema.HtmlBody,
				InternalSchema.RtfBody,
				InternalSchema.RtfInSync
			});
			AbstractInboundConverter.AddAddressRule(dictionary);
			AbstractInboundConverter.AddPromotionRule(dictionary, new AbstractInboundConverter.CustomRule(delegate(AbstractInboundConverter converter, NativeStorePropertyDefinition property)
			{
				converter.PromoteTimeZoneProperty(property);
			}), new NativeStorePropertyDefinition[]
			{
				InternalSchema.TimeZone
			});
			return dictionary;
		}

		private InboundMessageWriter messageWriter;

		private readonly AbstractInboundConverter.IPromotionRule defaultPromotionRule;

		private readonly Dictionary<NativeStorePropertyDefinition, AbstractInboundConverter.IPromotionRule> rules = AbstractInboundConverter.staticRules;

		private static readonly Dictionary<NativeStorePropertyDefinition, AbstractInboundConverter.IPromotionRule> staticRules = AbstractInboundConverter.CreateRulesTable();

		internal interface IPromotionRule
		{
			void PromoteProperty(AbstractInboundConverter converter, NativeStorePropertyDefinition definition);
		}

		protected class WriteablePropertyPromotionRule : AbstractInboundConverter.IPromotionRule
		{
			public void PromoteProperty(AbstractInboundConverter converter, NativeStorePropertyDefinition property)
			{
				if ((property.PropertyFlags & PropertyFlags.ReadOnly) != PropertyFlags.ReadOnly)
				{
					converter.PromoteProperty(property, true);
				}
			}
		}

		protected class TransmittablePropertyPromotionRule : AbstractInboundConverter.IPromotionRule
		{
			public void PromoteProperty(AbstractInboundConverter converter, NativeStorePropertyDefinition property)
			{
				if (!converter.IsTopLevelMessage)
				{
					converter.PromoteProperty(property, true);
					return;
				}
				if ((property.PropertyFlags & PropertyFlags.Transmittable) == PropertyFlags.Transmittable)
				{
					converter.PromoteProperty(property, true);
				}
			}
		}

		private class AddressPromotionRule : AbstractInboundConverter.IPromotionRule
		{
			public void PromoteProperty(AbstractInboundConverter converter, NativeStorePropertyDefinition definition)
			{
				if (converter.CurrentComponentType == ConversionComponentType.Message)
				{
					converter.PromoteAddressProperty(definition);
				}
			}
		}

		private class ComponentSpecificPromotionRule : AbstractInboundConverter.IPromotionRule
		{
			public ComponentSpecificPromotionRule(ConversionComponentType targetComponentType)
			{
				this.targetComponentType = targetComponentType;
			}

			public void PromoteProperty(AbstractInboundConverter converter, NativeStorePropertyDefinition property)
			{
				if (converter.CurrentComponentType == this.targetComponentType)
				{
					converter.PromoteProperty(property, true);
				}
			}

			private readonly ConversionComponentType targetComponentType;
		}

		private class SmartPropertyPromotionRule : AbstractInboundConverter.IPromotionRule
		{
			public SmartPropertyPromotionRule(StorePropertyDefinition substituteProperty, ConversionComponentType componentType, bool promoteForOtherComponentTypes)
			{
				this.substituteProperty = substituteProperty;
				this.expectedComponentType = componentType;
				this.promoteForOtherComponentTypes = promoteForOtherComponentTypes;
			}

			public void PromoteProperty(AbstractInboundConverter converter, NativeStorePropertyDefinition property)
			{
				if (converter.CurrentComponentType == this.expectedComponentType)
				{
					converter.PromoteProperty(this.substituteProperty, false);
					return;
				}
				StorageGlobals.ContextTraceError<NativeStorePropertyDefinition, ConversionComponentType>(ExTraceGlobals.CcInboundTnefTracer, "SmartPropertyPromotionRule.PromoteProperty {0}, wrong component type {1}", property, converter.CurrentComponentType);
				if (this.promoteForOtherComponentTypes)
				{
					converter.PromoteProperty(property, false);
				}
			}

			private readonly StorePropertyDefinition substituteProperty;

			private readonly ConversionComponentType expectedComponentType;

			private readonly bool promoteForOtherComponentTypes;
		}

		private class CustomRule : AbstractInboundConverter.IPromotionRule
		{
			public CustomRule(AbstractInboundConverter.CustomRule.CustomPromotionDelegate promotionDelegate)
			{
				this.promotionDelegate = promotionDelegate;
			}

			public void PromoteProperty(AbstractInboundConverter converter, NativeStorePropertyDefinition definition)
			{
				this.promotionDelegate(converter, definition);
			}

			private AbstractInboundConverter.CustomRule.CustomPromotionDelegate promotionDelegate;

			public delegate void CustomPromotionDelegate(AbstractInboundConverter converter, NativeStorePropertyDefinition definition);
		}
	}
}
