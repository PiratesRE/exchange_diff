using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class BaseConverter
	{
		static BaseConverter()
		{
			BaseConverter.propDefTypeOverrideMap.Add(CalendarItemBaseSchema.FreeBusyStatus, typeof(BusyType));
			BaseConverter.propDefTypeOverrideMap.Add(CalendarItemBaseSchema.IntendedFreeBusyStatus, typeof(BusyType));
			BaseConverter.propDefTypeOverrideMap.Add(CalendarItemBaseSchema.ResponseType, typeof(ResponseType));
			BaseConverter.propDefTypeOverrideMap.Add(ContactSchema.FileAsId, typeof(FileAsMapping));
			BaseConverter.propDefTypeOverrideMap.Add(ContactSchema.PostalAddressId, typeof(ContactConverter.PostalAddressIndexType));
			BaseConverter.propDefTypeOverrideMap.Add(TaskSchema.DelegationState, typeof(TaskDelegateState));
			BaseConverter.propDefTypeOverrideMap.Add(ItemSchema.FlagStatus, typeof(FlagStatus));
			BaseConverter.propDefTypeOverrideMap.Add(ConversationItemSchema.ConversationImportance, typeof(Importance));
			BaseConverter.propDefTypeOverrideMap.Add(ConversationItemSchema.ConversationGlobalImportance, typeof(Importance));
			BaseConverter.propDefTypeOverrideMap.Add(ConversationItemSchema.ConversationFlagStatus, typeof(FlagStatus));
			BaseConverter.propDefTypeOverrideMap.Add(ConversationItemSchema.ConversationGlobalFlagStatus, typeof(FlagStatus));
			BaseConverter.typeToConverterMap = new Dictionary<Type, BaseConverter>();
			BaseConverter.AddConversionEntry(typeof(byte[]), new Base64StringConverter());
			BaseConverter.typeToConverterMap.Add(typeof(bool), new BooleanConverter());
			BaseConverter.AddConversionEntry(typeof(ExDateTime), new ExDateTimeConverter());
			BaseConverter.AddConversionEntry(typeof(double), new DoubleConverter());
			BaseConverter.AddConversionEntry(typeof(float), new FloatConverter());
			BaseConverter.AddConversionEntry(typeof(Guid), new GuidConverter());
			BaseConverter.AddConversionEntry(typeof(int), new IntConverter());
			BaseConverter.AddConversionEntry(typeof(long), new LongConverter());
			BaseConverter.AddConversionEntry(typeof(short), new ShortConverter());
			BaseConverter.AddConversionEntry(typeof(string), new StringConverter());
			BaseConverter.AddConversionEntry(typeof(BusyType), new BusyTypeConverter());
			BaseConverter.AddConversionEntry(typeof(CalendarItemType), new CalendarItemTypeConverter());
			BaseConverter.AddConversionEntry(typeof(FileAsMapping), new ContactConverter.FileAsMapping());
			BaseConverter.AddConversionEntry(typeof(FlagStatus), new FlagStatusConverter());
			BaseConverter.AddConversionEntry(typeof(Importance), new ImportanceConverter());
			BaseConverter.AddConversionEntry(typeof(ResponseType), new ResponseTypeConverter());
			BaseConverter.AddConversionEntry(typeof(Sensitivity), new SensitivityConverter());
			BaseConverter.AddConversionEntry(typeof(TaskDelegateState), new TaskDelegateStateConverter());
			BaseConverter.AddConversionEntry(typeof(AppointmentStateFlags), new IntConverter());
			BaseConverter.AddConversionEntry(typeof(ContactConverter.PostalAddressIndexType), new ContactConverter.PostalAddressIndex());
			BaseConverter.AddConversionEntry(typeof(EmailAddress), new EmailAddressValueConverter());
			BaseConverter.AddConversionEntry(typeof(PersonType), new PersonaTypeConverter());
			BaseConverter.AddConversionEntry(typeof(PhoneNumber), new PhoneNumberConverter());
			BaseConverter.AddConversionEntry(typeof(Participant), new ParticipantConverter());
			BaseConverter.AddConversionEntry<StoreObjectId>(new StoreObjectIdConverter());
			BaseConverter.AddConversionEntry<Attribution>(new AttributionConverter());
			BaseConverter.AddConversionEntry<AttributedValue<string>>(new StringAttributedValueConverter());
			BaseConverter.AddConversionEntry<AttributedValue<string[]>>(new StringArrayAttributedValueConverter());
			BaseConverter.AddConversionEntry<AttributedValue<ExDateTime>>(new ExDateTimeAttributedValueConverter());
			BaseConverter.AddConversionEntry<AttributedValue<EmailAddress>>(new EmailAddressAttributedValueConverter());
			BaseConverter.AddConversionEntry<AttributedValue<Participant>>(new EmailAddressAttributedValueConverter());
			BaseConverter.AddConversionEntry<AttributedValue<Microsoft.Exchange.Data.Storage.PostalAddress>>(new PostalAddressAttributedValueConverter());
			BaseConverter.AddConversionEntry<AttributedValue<PhoneNumber>>(new PhoneNumberAttributedValueConverter());
			BaseConverter.AddConversionEntry<AttributedValue<ContactExtendedPropertyData>>(new ExtendedPropertyAttributedValueConverter());
		}

		private static void AddConversionEntry(Type type, BaseConverter converter)
		{
			BaseConverter.typeToConverterMap.Add(type, converter);
			BaseConverter.typeToConverterMap.Add(type.MakeArrayType(), converter);
		}

		private static void AddConversionEntry<T>(BaseConverter converter)
		{
			BaseConverter.typeToConverterMap.Add(typeof(T), converter);
			BaseConverter.typeToConverterMap.Add(typeof(IEnumerable<T>), converter);
		}

		public abstract object ConvertToObject(string propertyString);

		public abstract string ConvertToString(object propertyValue);

		protected virtual object ConvertToServiceObjectValue(object propertyValue)
		{
			return propertyValue;
		}

		public virtual object ConvertToServiceObjectValue(object propertyValue, IdConverterWithCommandSettings idConverterWithCommandSettings)
		{
			return this.ConvertToServiceObjectValue(propertyValue);
		}

		private static Type GetTypeOverride(PropertyDefinition propertyDefinition)
		{
			Type result;
			if (BaseConverter.propDefTypeOverrideMap.TryGetValue(propertyDefinition, out result))
			{
				return result;
			}
			return propertyDefinition.Type;
		}

		public static bool TryGetConverterForType(Type type, out BaseConverter converter)
		{
			return BaseConverter.typeToConverterMap.TryGetValue(type, out converter);
		}

		public static BaseConverter GetConverterForType(Type type)
		{
			BaseConverter result;
			if (!BaseConverter.TryGetConverterForType(type, out result))
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<Type>(0L, "BaseConverter.GetConverterForType throwing UnsupportedTypeForConversionException for type: {0}", type);
				throw new UnsupportedTypeForConversionException();
			}
			return result;
		}

		public static BaseConverter GetConverterForPropertyDefinition(PropertyDefinition propertyDefinition)
		{
			BaseConverter result;
			if (!BaseConverter.TryGetConverterForPropertyDefinition(propertyDefinition, out result))
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<PropertyDefinition>(0L, "BaseConverter.GetConverterForPropertyDefinition throwing UnsupportedTypeForConversionException for property: {0}", propertyDefinition);
				throw new UnsupportedTypeForConversionException();
			}
			return result;
		}

		public static bool TryGetConverterForPropertyDefinition(PropertyDefinition propertyDefinition, out BaseConverter converter)
		{
			Type typeOverride = BaseConverter.GetTypeOverride(propertyDefinition);
			return BaseConverter.typeToConverterMap.TryGetValue(typeOverride, out converter);
		}

		private static Dictionary<PropertyDefinition, Type> propDefTypeOverrideMap = new Dictionary<PropertyDefinition, Type>();

		private static Dictionary<Type, BaseConverter> typeToConverterMap;
	}
}
