using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal abstract class BaseLeafFilterConverter : BaseSingleFilterConverter
	{
		internal override bool IsLeafFilter
		{
			get
			{
				return true;
			}
		}

		internal abstract QueryFilter ConvertToQueryFilter(SearchExpressionType searchExpression);

		internal abstract SearchExpressionType ConvertToSearchExpression(QueryFilter queryFilter);

		protected static bool TryGetOperandAsConstantValue(object operandElement, out string constantValue)
		{
			ConstantValueType constantValueType = operandElement as ConstantValueType;
			if (constantValueType != null)
			{
				constantValue = constantValueType.Value;
				return true;
			}
			constantValue = string.Empty;
			return false;
		}

		protected static object GetConvertedValueForPropertyDefinition(PropertyDefinition propDef, string valueAsString)
		{
			object result;
			try
			{
				result = BaseConverter.GetConverterForPropertyDefinition(propDef).ConvertToObject(valueAsString);
			}
			catch (UnsupportedTypeForConversionException innerException)
			{
				throw new InvalidValueForPropertyException(SearchSchemaMap.GetPropertyPath(propDef), innerException);
			}
			catch (FormatException innerException2)
			{
				throw new InvalidValueForPropertyException(SearchSchemaMap.GetPropertyPath(propDef), innerException2);
			}
			catch (OverflowException innerException3)
			{
				throw new InvalidValueForPropertyException(SearchSchemaMap.GetPropertyPath(propDef), innerException3);
			}
			catch (InvalidCastException innerException4)
			{
				throw new InvalidValueForPropertyException(SearchSchemaMap.GetPropertyPath(propDef), innerException4);
			}
			catch (ArgumentNullException innerException5)
			{
				throw new InvalidValueForPropertyException(SearchSchemaMap.GetPropertyPath(propDef), innerException5);
			}
			return result;
		}

		protected static string GetStringForPropertyValue(object value)
		{
			if (value == null)
			{
				return null;
			}
			return BaseConverter.GetConverterForType(value.GetType()).ConvertToString(value);
		}

		protected static PropertyDefinition GetAndValidatePropertyDefinitionForQuery(PropertyPath propertyPath)
		{
			PropertyDefinition propertyDefinition;
			if (!SearchSchemaMap.TryGetPropertyDefinition(propertyPath, out propertyDefinition))
			{
				throw new UnsupportedPathForQueryException(propertyPath);
			}
			StorePropertyDefinition storePropertyDefinition = (StorePropertyDefinition)propertyDefinition;
			if ((storePropertyDefinition.Capabilities & StorePropertyCapabilities.CanQuery) != StorePropertyCapabilities.CanQuery)
			{
				throw new UnsupportedPathForQueryException(propertyPath);
			}
			return propertyDefinition;
		}

		protected string ConvertSmtpToExAddress(PropertyDefinition propertyDefinition, string smtpAddressIn)
		{
			if (propertyDefinition != MessageItemSchema.SenderEmailAddress && propertyDefinition != ItemSchema.SentRepresentingEmailAddress)
			{
				if (propertyDefinition != CalendarItemBaseSchema.OrganizerEmailAddress)
				{
					return smtpAddressIn;
				}
			}
			try
			{
				Participant participant = new Participant(string.Empty, smtpAddressIn, "SMTP");
				Participant participant2 = MailboxHelper.TryConvertTo(participant, "EX");
				if (participant2 != null)
				{
					return participant2.EmailAddress;
				}
			}
			catch (ArgumentException ex)
			{
				ExTraceGlobals.ExceptionTracer.TraceError<string, string, string>((long)this.GetHashCode(), "[BaseLeafFilterConverter::ConvertSmtpToExAddress] Exception encountered trying to convert from address: '{0}'.  Exception: {1},  {2}", smtpAddressIn, ex.GetType().FullName, ex.Message);
				return smtpAddressIn;
			}
			catch (InvalidParticipantException ex2)
			{
				ExTraceGlobals.ExceptionTracer.TraceError<string, string, string>((long)this.GetHashCode(), "[BaseLeafFilterConverter::ConvertSmtpToExAddress] Exception encountered trying to convert from address: '{0}'.  Exception: {1},  {2}", smtpAddressIn, ex2.GetType().FullName, ex2.Message);
				return smtpAddressIn;
			}
			catch (NotSupportedException ex3)
			{
				ExTraceGlobals.ExceptionTracer.TraceError<string, string, string>((long)this.GetHashCode(), "[BaseLeafFilterConverter::ConvertSmtpToExAddress] Exception encountered trying to convert from address: '{0}'.  Exception: {1},  {2}", smtpAddressIn, ex3.GetType().FullName, ex3.Message);
				return smtpAddressIn;
			}
			return smtpAddressIn;
		}
	}
}
