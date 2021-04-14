using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.DirSync;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class ADValueConvertor
	{
		internal static string ConvertValueToString(object originalValue, IFormatProvider formatProvider)
		{
			string result;
			Exception ex;
			if (ADValueConvertor.TryConvertValueToString(originalValue, formatProvider, out result, out ex))
			{
				return result;
			}
			if (typeof(NotImplementedException) == ex.GetType())
			{
				throw ex;
			}
			throw new FormatException(DataStrings.ErrorCannotConvertToString(ex.Message), ex);
		}

		internal static bool TryConvertValueToString(object originalValue, IFormatProvider formatProvider, out string result, out Exception error)
		{
			error = null;
			result = null;
			if (originalValue == null)
			{
				error = new ArgumentNullException("originalValue");
				return false;
			}
			Type type = originalValue.GetType();
			if (type.Equals(typeof(ADObjectId)))
			{
				result = ((ADObjectId)originalValue).ToGuidOrDNString();
			}
			else if (type.Equals(typeof(ADObjectIdWithString)))
			{
				result = ((ADObjectIdWithString)originalValue).ToDNStringSyntax(false);
			}
			else if (type.Equals(typeof(DateTime)))
			{
				DateTime dateTime = (DateTime)originalValue;
				IFormatProvider formatProvider2 = formatProvider ?? DateTimeFormatProvider.Generalized;
				string format = (string)formatProvider2.GetFormat(typeof(DateTimeFormatProvider.DateTimeFormat));
				result = dateTime.ToUniversalTime().ToString(format, DateTimeFormatInfo.InvariantInfo);
			}
			else if (type.Equals(typeof(RecipientTypeDetails)))
			{
				result = ((long)originalValue).ToString();
			}
			else if (type == typeof(EmailAddressPolicyPriority))
			{
				result = ((EmailAddressPolicyPriority)originalValue).ToString();
			}
			else if (type == typeof(HolidaySchedule))
			{
				result = ((HolidaySchedule)originalValue).ToADString();
			}
			else if (type == typeof(AddressSpace))
			{
				result = ((AddressSpace)originalValue).ADToString();
			}
			else if (type == typeof(ServerCostPair))
			{
				result = ((ServerCostPair)originalValue).ToADString();
			}
			else if (originalValue is ApprovedApplication)
			{
				result = ((ApprovedApplication)originalValue).ToString();
			}
			else if (type == typeof(CmdletRoleEntry) || type == typeof(ScriptRoleEntry) || type == typeof(ApplicationPermissionRoleEntry) || type == typeof(WebServiceRoleEntry) || type == typeof(UnknownRoleEntry))
			{
				result = ((RoleEntry)originalValue).ToADString();
			}
			else if (type == typeof(OfflineAddressBookMapiProperty))
			{
				result = ((OfflineAddressBookMapiProperty)originalValue).ToSerializationString();
			}
			else if (originalValue is TextMessagingStateBase)
			{
				result = ((TextMessagingStateBase)originalValue).ToADString();
			}
			else if (originalValue is SyncLink)
			{
				result = ((SyncLink)originalValue).ToADString();
			}
			else if (originalValue is PropertyReference)
			{
				result = ((PropertyReference)originalValue).ToADString();
			}
			else if (originalValue is DirectoryObjectClass)
			{
				result = Enum.GetName(typeof(DirectoryObjectClass), originalValue);
			}
			else if (type.Equals(typeof(RemoteRecipientType)))
			{
				result = ((long)originalValue).ToString();
			}
			else if (originalValue is LinkedPartnerGroupInformation)
			{
				result = ((LinkedPartnerGroupInformation)originalValue).ToString();
			}
			else if (originalValue is SmtpReceiveDomainCapabilities)
			{
				result = ((SmtpReceiveDomainCapabilities)originalValue).ToADString();
			}
			else if (originalValue is SmtpX509Identifier)
			{
				result = ((SmtpX509Identifier)originalValue).ToString();
			}
			else if (originalValue is SmtpX509IdentifierEx)
			{
				result = ((SmtpX509IdentifierEx)originalValue).ToString();
			}
			else if (originalValue is TlsCertificate)
			{
				result = ((TlsCertificate)originalValue).ToString();
			}
			else if (originalValue is SharedConfigurationInfo)
			{
				result = ((SharedConfigurationInfo)originalValue).ToString();
			}
			else if (originalValue is ReconciliationCookie)
			{
				result = ((ReconciliationCookie)originalValue).ToString();
			}
			else if (originalValue is FullSyncObjectRequest)
			{
				result = originalValue.ToString();
			}
			else if (originalValue is PublicFolderInformation)
			{
				result = ((PublicFolderInformation)originalValue).Serialize();
			}
			else
			{
				if (!(originalValue is MailboxProvisioningConstraint))
				{
					return ValueConvertor.TryConvertValueToString(originalValue, formatProvider, out result, out error);
				}
				result = ((MailboxProvisioningConstraint)originalValue).Value;
			}
			return null == error;
		}

		internal static object ConvertValueFromString(string originalValue, Type resultType, IFormatProvider formatProvider)
		{
			object result;
			Exception ex;
			if (ADValueConvertor.TryConvertValueFromString(originalValue, resultType, formatProvider, out result, out ex))
			{
				return result;
			}
			if (typeof(NotImplementedException) == ex.GetType())
			{
				throw ex;
			}
			throw new FormatException(DataStrings.ErrorCannotConvertFromString(originalValue, resultType.ToString(), ex.Message), ex);
		}

		internal static bool TryConvertValueFromString(string originalValue, Type resultType, IFormatProvider formatProvider, out object result, out Exception error)
		{
			error = null;
			result = null;
			if (originalValue == null)
			{
				error = new ArgumentNullException("originalValue");
				return false;
			}
			if (null == resultType)
			{
				error = new ArgumentNullException("resultType");
				return false;
			}
			try
			{
				if (resultType.Equals(typeof(ADObjectId)))
				{
					result = ADObjectId.ParseExtendedDN(originalValue);
				}
				else if (resultType.Equals(typeof(ADObjectIdWithString)))
				{
					result = ADObjectIdWithString.ParseDNStringSyntax(originalValue, Guid.Empty, null);
				}
				else if (resultType.Equals(typeof(DateTime?)))
				{
					IFormatProvider formatProvider2 = formatProvider ?? DateTimeFormatProvider.Generalized;
					string format = (string)formatProvider2.GetFormat(typeof(DateTimeFormatProvider.DateTimeFormat));
					DateTime value = DateTime.ParseExact(originalValue, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
					result = new DateTime?(value);
				}
				else if (resultType == typeof(EmailAddressPolicyPriority))
				{
					result = EmailAddressPolicyPriority.Parse(originalValue);
				}
				else if (resultType == typeof(HolidaySchedule))
				{
					result = HolidaySchedule.ParseADString(originalValue);
				}
				else if (resultType == typeof(AddressSpace))
				{
					result = AddressSpace.ADParse(originalValue);
				}
				else if (resultType == typeof(ServerCostPair))
				{
					result = ServerCostPair.ParseFromAD(originalValue);
				}
				else if (resultType == typeof(ApprovedApplication))
				{
					result = ApprovedApplication.Parse(originalValue);
				}
				else if (resultType == typeof(RecipientDisplayType?))
				{
					result = (RecipientDisplayType?)Enum.Parse(typeof(RecipientDisplayType), originalValue);
				}
				else if (resultType == typeof(AudioCodecEnum?))
				{
					result = (AudioCodecEnum?)Enum.Parse(typeof(AudioCodecEnum), originalValue, false);
				}
				else if (resultType == typeof(ExchangeResourceType?))
				{
					result = (ExchangeResourceType?)Enum.Parse(typeof(ExchangeResourceType), originalValue);
				}
				else if (resultType == typeof(WellKnownRecipientType?))
				{
					result = (WellKnownRecipientType?)Enum.Parse(typeof(WellKnownRecipientType), originalValue);
				}
				else if (resultType == typeof(OfflineAddressBookMapiProperty))
				{
					result = OfflineAddressBookMapiProperty.ParseSerializationString(originalValue);
				}
				else if (resultType == typeof(TextMessagingStateBase))
				{
					result = TextMessagingStateBase.ParseFromADString(originalValue);
				}
				else if (resultType == typeof(SyncLink))
				{
					result = SyncLink.ParseFromADString(originalValue);
				}
				else if (resultType == typeof(PropertyReference))
				{
					result = PropertyReference.ParseFromADString(originalValue);
				}
				else if (resultType == typeof(DirectoryObjectClass))
				{
					result = (DirectoryObjectClass)Enum.Parse(typeof(DirectoryObjectClass), originalValue, true);
				}
				else if (resultType == typeof(EwsApplicationAccessPolicy?))
				{
					result = (EwsApplicationAccessPolicy?)Enum.Parse(typeof(EwsApplicationAccessPolicy), originalValue);
				}
				else if (resultType == typeof(uint))
				{
					result = (uint)int.Parse(originalValue);
				}
				else if (resultType == typeof(uint?))
				{
					result = new uint?((uint)int.Parse(originalValue));
				}
				else if (resultType == typeof(LinkedPartnerGroupInformation))
				{
					result = LinkedPartnerGroupInformation.Parse(originalValue);
				}
				else if (resultType == typeof(SmtpReceiveDomainCapabilities))
				{
					result = SmtpReceiveDomainCapabilities.FromADString(originalValue);
				}
				else if (resultType == typeof(SmtpX509Identifier))
				{
					result = SmtpX509Identifier.Parse(originalValue);
				}
				else if (resultType == typeof(SmtpX509IdentifierEx))
				{
					result = SmtpX509IdentifierEx.Parse(originalValue);
				}
				else if (resultType == typeof(TlsCertificate))
				{
					result = TlsCertificate.Parse(originalValue);
				}
				else if (resultType == typeof(SharedConfigurationInfo))
				{
					result = SharedConfigurationInfo.Parse(originalValue);
				}
				else if (resultType == typeof(ReconciliationCookie))
				{
					result = ReconciliationCookie.Parse(originalValue);
				}
				else if (resultType == typeof(CountryInfo))
				{
					result = CountryInfo.Parse(originalValue);
				}
				else if (resultType == typeof(FullSyncObjectRequest))
				{
					result = FullSyncObjectRequest.Parse(originalValue);
				}
				else if (resultType == typeof(PublicFolderInformation))
				{
					result = PublicFolderInformation.Deserialize(originalValue);
				}
				else if (resultType == typeof(LegacyRecipientDisplayType?))
				{
					result = (LegacyRecipientDisplayType?)Enum.Parse(typeof(LegacyRecipientDisplayType), originalValue);
				}
				else
				{
					if (!(resultType == typeof(MailboxProvisioningConstraint)))
					{
						return ValueConvertor.TryConvertValueFromString(originalValue, resultType, formatProvider, out result, out error);
					}
					result = new MailboxProvisioningConstraint(originalValue);
				}
			}
			catch (FormatException ex)
			{
				error = ex;
				return false;
			}
			catch (ArgumentException ex2)
			{
				error = ex2;
				return false;
			}
			catch (InvalidCountryOrRegionException ex3)
			{
				error = ex3;
				return false;
			}
			return null == error;
		}

		internal static byte[] ConvertValueToBinary(object originalValue, IFormatProvider formatProvider)
		{
			byte[] result;
			Exception ex;
			if (ADValueConvertor.TryConvertValueToBinary(originalValue, formatProvider, out result, out ex))
			{
				return result;
			}
			if (typeof(NotImplementedException) == ex.GetType())
			{
				throw ex;
			}
			throw new FormatException(DataStrings.ErrorCannotConvertToBinary(ex.Message), ex);
		}

		internal static bool TryConvertValueToBinary(object originalValue, IFormatProvider formatProvider, out byte[] result, out Exception error)
		{
			error = null;
			result = null;
			if (originalValue == null)
			{
				error = new ArgumentNullException("originalValue");
				return false;
			}
			CrossTenantObjectId crossTenantObjectId = originalValue as CrossTenantObjectId;
			if (crossTenantObjectId != null)
			{
				result = crossTenantObjectId.GetBytes();
				return true;
			}
			return ValueConvertor.TryConvertValueToBinary(originalValue, formatProvider, out result, out error);
		}

		internal static object ConvertValueFromBinary(byte[] originalValue, Type resultType, IFormatProvider formatProvider)
		{
			object result;
			Exception ex;
			if (ADValueConvertor.TryConvertValueFromBinary(originalValue, resultType, formatProvider, out result, out ex))
			{
				return result;
			}
			if (typeof(NotImplementedException) == ex.GetType())
			{
				throw ex;
			}
			throw new FormatException(DataStrings.ErrorCannotConvertFromBinary(resultType.ToString(), ex.Message), ex);
		}

		internal static bool TryConvertValueFromBinary(byte[] originalValue, Type resultType, IFormatProvider formatProvider, out object result, out Exception error)
		{
			error = null;
			result = null;
			if (originalValue == null)
			{
				error = new ArgumentNullException("originalValue");
				return false;
			}
			if (null == resultType)
			{
				error = new ArgumentNullException("resultType");
				return false;
			}
			try
			{
				if (resultType.Equals(typeof(ReplicationCursor)))
				{
					result = ReplicationCursor.Parse(originalValue);
				}
				else if (resultType.Equals(typeof(ReplicationNeighbor)))
				{
					result = ReplicationNeighbor.Parse(originalValue);
				}
				else if (resultType.Equals(typeof(LinkMetadata)))
				{
					result = LinkMetadata.Parse(originalValue);
				}
				else if (resultType.Equals(typeof(AttributeMetadata)))
				{
					result = AttributeMetadata.Parse(originalValue);
				}
				else
				{
					if (!resultType.Equals(typeof(CrossTenantObjectId)))
					{
						return ValueConvertor.TryConvertValueFromBinary(originalValue, resultType, formatProvider, out result, out error);
					}
					result = CrossTenantObjectId.Parse(originalValue);
				}
			}
			catch (FormatException ex)
			{
				error = ex;
				return false;
			}
			return null == error;
		}

		internal static void ConvertAndAddValueToDirectoryAttribute(object value, ADPropertyDefinition property, DirectoryAttribute attribute, bool softLinkEnabled)
		{
			if (softLinkEnabled && property.IsSoftLinkAttribute)
			{
				byte[] value2 = ADObjectIdResolutionHelper.ResolveSoftLink((ADObjectId)value).ToSoftLinkValue();
				attribute.Add(value2);
				return;
			}
			if (property.IsBinary)
			{
				byte[] array = ADValueConvertor.ConvertValueToBinary(value, property.FormatProvider);
				attribute.Add(array);
				ExTraceGlobals.ADSaveDetailsTracer.TraceDebug<string, int>(0L, "ADValueConvertor::ConvertAndAddValueToDirectoryAttribute - adding binary {0}: {1} bytes", attribute.Name, array.Length);
				return;
			}
			string text = ADValueConvertor.ConvertValueToString(value, property.FormatProvider);
			DirectoryAttributeModification directoryAttributeModification = attribute as DirectoryAttributeModification;
			if (string.IsNullOrEmpty(text) && directoryAttributeModification != null && directoryAttributeModification.Operation == DirectoryAttributeOperation.Replace && !property.Flags.HasFlag(ADPropertyDefinitionFlags.Mandatory))
			{
				directoryAttributeModification.Operation = DirectoryAttributeOperation.Delete;
				ExTraceGlobals.ADSaveDetailsTracer.TraceDebug<string>(0L, "ADValueConvertor::ConvertAndAddValueToDirectoryAttribute - deleting attribute {0}", attribute.Name);
				return;
			}
			attribute.Add(text);
			ExTraceGlobals.ADSaveDetailsTracer.TraceDebug<string, string, string>(0L, "ADValueConvertor::ConvertAndAddValueToDirectoryAttribute - adding string {0}: '{1}'. DirectoryAttributeOperation {2}", attribute.Name, text, (directoryAttributeModification != null) ? directoryAttributeModification.Operation.ToString() : "not an attribute modification");
		}

		internal static object ConvertValueFromAD(ADPropertyDefinition property, object originalValue)
		{
			return ADValueConvertor.ConvertValueFromAD(null, property, originalValue, null);
		}

		internal static object ConvertValueFromAD(ADObjectId objectId, ADPropertyDefinition property, object originalValue, OrganizationId executingUserOrgId)
		{
			Type type = property.Type;
			IFormatProvider formatProvider = property.FormatProvider;
			if (typeof(string) == originalValue.GetType())
			{
				if (property == ADObjectSchema.ObjectClass)
				{
					return Globals.StringPool.Intern((string)originalValue);
				}
				if (type.Equals(typeof(ADObjectId)))
				{
					ADObjectId adobjectId = ADObjectId.ParseExtendedDN((string)originalValue, ADObjectId.ResourcePartitionGuid, executingUserOrgId);
					if (property.IsSoftLinkAttribute)
					{
						return ADObjectIdResolutionHelper.ResolveDNIfNecessary(adobjectId);
					}
					return adobjectId;
				}
				else
				{
					if (type.Equals(typeof(ADObjectIdWithString)))
					{
						return ADObjectIdWithString.ParseDNStringSyntax((string)originalValue, ADObjectId.ResourcePartitionGuid, executingUserOrgId);
					}
					if (type == typeof(ProxyAddress))
					{
						return ProxyAddress.ParseFromAD((string)originalValue);
					}
					return ADValueConvertor.ConvertValueFromString((string)originalValue, type, formatProvider);
				}
			}
			else
			{
				if (type.Equals(typeof(ADObjectId)))
				{
					ADObjectId obj = ADObjectId.FromSoftLinkValue((byte[])originalValue, objectId, executingUserOrgId);
					return ADObjectIdResolutionHelper.ResolveDNIfNecessary(obj);
				}
				return ADValueConvertor.ConvertValueFromBinary((byte[])originalValue, type, formatProvider);
			}
		}

		internal static object GetValueFromDirectoryAttribute(ADObjectId objectId, DirectoryAttribute attributeValues, ADPropertyDefinition property, bool createReadOnlyCollections, List<ADPropertyDefinition> rangedProperties, OrganizationId executingUserOrgId, List<ValidationError> errors, IntRange valueRange, bool softLinkEnabled)
		{
			bool isBinary;
			if (softLinkEnabled && property.IsSoftLinkAttribute)
			{
				isBinary = property.SoftLinkShadowProperty.IsBinary;
			}
			else
			{
				isBinary = property.IsBinary;
			}
			object[] values = attributeValues.GetValues(isBinary ? typeof(byte[]) : typeof(string));
			return ADValueConvertor.GetValueFromDirectoryAttributeValues(objectId, property, values, createReadOnlyCollections, rangedProperties, executingUserOrgId, errors, valueRange);
		}

		public static object GetValueFromDirectoryAttributeValues(ADPropertyDefinition property, object[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			if (values.Length < 1)
			{
				throw new ArgumentException("'values' must contain at least one value.");
			}
			List<ADPropertyDefinition> rangedProperties = new List<ADPropertyDefinition>();
			List<ValidationError> list = new List<ValidationError>();
			object valueFromDirectoryAttributeValues = ADValueConvertor.GetValueFromDirectoryAttributeValues(null, property, values, true, rangedProperties, null, list, null);
			if (list.Count > 0)
			{
				throw new DataValidationException(list[0]);
			}
			return valueFromDirectoryAttributeValues;
		}

		private static object GetValueFromDirectoryAttributeValues(ADObjectId objectId, ADPropertyDefinition property, object[] values, bool createReadOnlyCollections, List<ADPropertyDefinition> rangedProperties, OrganizationId executingUserOrgId, List<ValidationError> errors, IntRange valueRange)
		{
			bool noConversionRequired = property.Type == (property.IsBinary ? typeof(byte[]) : typeof(string));
			if (property.IsMultivalued)
			{
				if (values.Length == 0)
				{
					rangedProperties.Add(property);
					return null;
				}
				ArrayList arrayList = new ArrayList(values.Length);
				ArrayList invalidValues = null;
				ADValueConvertor.ConvertMultipleValues(objectId, property, noConversionRequired, values, errors, arrayList, executingUserOrgId, out invalidValues);
				MultiValuedPropertyBase multiValuedPropertyBase = ADValueConvertor.CreateGenericMultiValuedProperty(property, createReadOnlyCollections || property.IsReadOnly, arrayList, invalidValues, null);
				ValidationError validationError = property.ValidateCollection(multiValuedPropertyBase, true);
				if (validationError != null)
				{
					errors.Add(validationError);
				}
				multiValuedPropertyBase.IsCompletelyRead = true;
				if (valueRange != null && !valueRange.Equals(RangedPropertyHelper.AllValuesRange))
				{
					multiValuedPropertyBase.ValueRange = valueRange;
				}
				return multiValuedPropertyBase;
			}
			else
			{
				ValidationError validationError2;
				object obj = ADValueConvertor.ConvertFromADAndValidateSingleValue(objectId, values[0], property, noConversionRequired, executingUserOrgId, out validationError2);
				if (validationError2 != null)
				{
					errors.Add(validationError2);
					return null;
				}
				ExTraceGlobals.ADReadDetailsTracer.TraceDebug<string, object>(0L, "ADValueConvertor::GetValueFromDirectoryAttributeValues - adding valid sval {0}: {1}", property.LdapDisplayName, obj);
				return obj;
			}
		}

		internal static void ConvertMultipleValues(ADObjectId objectId, ADPropertyDefinition property, bool noConversionRequired, object[] values, List<ValidationError> errors, ArrayList convertedValues, out ArrayList invalidValues)
		{
			ADValueConvertor.ConvertMultipleValues(objectId, property, noConversionRequired, values, errors, convertedValues, null, out invalidValues);
		}

		internal static void ConvertMultipleValues(ADObjectId objectId, ADPropertyDefinition property, bool noConversionRequired, object[] values, List<ValidationError> errors, ArrayList convertedValues, OrganizationId executingUserOrgId, out ArrayList invalidValues)
		{
			invalidValues = null;
			for (int i = 0; i < values.Length; i++)
			{
				ValidationError validationError;
				object obj = ADValueConvertor.ConvertFromADAndValidateSingleValue(objectId, values[i], property, noConversionRequired, executingUserOrgId, out validationError);
				if (validationError != null)
				{
					errors.Add(validationError);
					if (invalidValues == null)
					{
						invalidValues = new ArrayList();
					}
					invalidValues.Add(values[i]);
				}
				else
				{
					ExTraceGlobals.ADReadDetailsTracer.TraceDebug<string, object>(0L, "ADValueConvertor::ConvertMultipleValues - adding valid mval {0}: {1}", property.LdapDisplayName, obj);
					convertedValues.Add(obj);
				}
			}
		}

		internal static MultiValuedPropertyBase GetValueFromMultipleDirectoryAttributes(ADObjectId objectId, List<DirectoryAttribute> attributeValuesList, ADPropertyDefinition property, bool createReadOnlyCollections, List<ValidationError> errors)
		{
			ArrayList arrayList = new ArrayList();
			ArrayList invalidValues = null;
			bool noConversionRequired = property.Type == (property.IsBinary ? typeof(byte[]) : typeof(string));
			foreach (DirectoryAttribute directoryAttribute in attributeValuesList)
			{
				object[] values = directoryAttribute.GetValues(property.IsBinary ? typeof(byte[]) : typeof(string));
				ADValueConvertor.ConvertMultipleValues(objectId, property, noConversionRequired, values, errors, arrayList, out invalidValues);
			}
			return ADValueConvertor.CreateGenericMultiValuedProperty(property, createReadOnlyCollections, arrayList, invalidValues, null);
		}

		internal static object ConvertFromADAndValidateSingleValue(object value, ADPropertyDefinition property, bool noConversionRequired, out ValidationError error)
		{
			return ADValueConvertor.ConvertFromADAndValidateSingleValue(null, value, property, noConversionRequired, null, out error);
		}

		internal static object ConvertFromADAndValidateSingleValue(ADObjectId objectId, object value, ADPropertyDefinition property, bool noConversionRequired, OrganizationId executingUserOrgId, out ValidationError error)
		{
			object obj = value;
			error = null;
			if (!noConversionRequired)
			{
				Exception ex = null;
				try
				{
					obj = ADValueConvertor.ConvertValueFromAD(objectId, property, value, executingUserOrgId);
				}
				catch (ArgumentOutOfRangeException ex2)
				{
					ex = ex2;
				}
				catch (ArgumentNullException ex3)
				{
					ex = ex3;
				}
				catch (ArgumentException ex4)
				{
					ex = ex4;
				}
				catch (FormatException ex5)
				{
					ex = ex5;
				}
				catch (OverflowException ex6)
				{
					ex = ex6;
				}
				catch (InvalidOperationException ex7)
				{
					ex = ex7;
				}
				if (ex != null)
				{
					ExTraceGlobals.ADReadDetailsTracer.TraceWarning<string, Type, string>(0L, "ADValueConvertor::ConvertFromADAndValidateSingleValue - failed when converting {0} with {1}, {2}", property.LdapDisplayName, ex.GetType(), ex.Message);
					error = new PropertyConversionError(DirectoryStrings.ConversionFailed(property.Name, property.Type.Name, ex.Message), property, value, ex);
					return null;
				}
			}
			error = property.ValidateValue(obj, true);
			if (error != null)
			{
				ExTraceGlobals.ADReadDetailsTracer.TraceWarning<string, LocalizedString>(0L, "ADValueConvertor::ConvertFromADAndValidateSingleValue - failed when Validating {0}: {1}", property.LdapDisplayName, error.Description);
				return null;
			}
			return obj;
		}

		internal static void ConvertToAndAppendFilterString(ADPropertyDefinition property, object originalValue, StringBuilder sb, bool softLinkEnabled, byte softLinkPrefix = 0)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property", DirectoryStrings.ExArgumentNullException("property"));
			}
			if (originalValue == null)
			{
				throw new ArgumentNullException("originalValue", DirectoryStrings.ExceptionFilterWithNullValue(property.Name));
			}
			if (sb == null)
			{
				throw new ArgumentNullException("sb", DirectoryStrings.ExArgumentNullException("sb"));
			}
			if (property.Type.Equals(typeof(string)) && originalValue is string)
			{
				ADValueConvertor.EscapeAndAppendString((string)originalValue, sb);
				return;
			}
			if (property.Type.Equals(typeof(Guid)) && originalValue is Guid)
			{
				sb.Append(ADValueConvertor.EscapeBinaryValue(((Guid)originalValue).ToByteArray()));
				return;
			}
			if (property.Type.Equals(typeof(byte[])) && originalValue is byte[])
			{
				sb.Append(ADValueConvertor.EscapeBinaryValue((byte[])originalValue));
				return;
			}
			if (property.Type.Equals(typeof(EmailAddressPolicyPriority)) && originalValue is EmailAddressPolicyPriority)
			{
				sb.Append(((EmailAddressPolicyPriority)originalValue).ToString());
				return;
			}
			if (property.Type.Equals(typeof(RecipientTypeDetails)) && originalValue is RecipientTypeDetails)
			{
				sb.Append(((long)originalValue).ToString());
				return;
			}
			if (property.Type.Equals(typeof(RemoteRecipientType)) && originalValue is RemoteRecipientType)
			{
				sb.Append(((long)originalValue).ToString());
				return;
			}
			if (property.Type.Equals(typeof(ExchangeObjectVersion)) && originalValue is ExchangeObjectVersion)
			{
				sb.Append(((ExchangeObjectVersion)originalValue).ToInt64().ToString());
				return;
			}
			if ((property.Type.Equals(typeof(bool)) && originalValue is bool) || (property.Type.Equals(typeof(bool?)) && originalValue is bool?))
			{
				sb.Append(ADValueConvertor.ConvertBooleanToString((bool)originalValue));
				return;
			}
			CrossTenantObjectId crossTenantObjectId = originalValue as CrossTenantObjectId;
			if (property.Type.Equals(typeof(CrossTenantObjectId)) && crossTenantObjectId != null)
			{
				sb.Append(ADValueConvertor.EscapeBinaryValue(crossTenantObjectId.GetBytes()));
				return;
			}
			if (originalValue is ADObjectId)
			{
				if (softLinkEnabled && property.IsSoftLinkAttribute && Guid.Empty != ((ADObjectId)originalValue).ObjectGuid)
				{
					sb.Append(ADValueConvertor.EscapeBinaryValue(((ADObjectId)originalValue).ToSoftLinkLdapQueryValue(softLinkPrefix)) + "*");
					return;
				}
				ADValueConvertor.EscapeAndAppendString(((ADObjectId)originalValue).ToGuidOrDNString(), sb);
				return;
			}
			else
			{
				if (originalValue is ProxyAddressBase)
				{
					ADValueConvertor.EscapeAndAppendString(((ProxyAddressBase)originalValue).ToString(), sb);
					return;
				}
				if (originalValue is NetworkAddress)
				{
					ADValueConvertor.EscapeAndAppendString(((NetworkAddress)originalValue).ToString(), sb);
					return;
				}
				if (originalValue is DateTime)
				{
					IFormatProvider formatProvider = property.FormatProvider ?? DateTimeFormatProvider.Generalized;
					string format = (string)formatProvider.GetFormat(typeof(DateTimeFormatProvider.DateTimeFormat));
					sb.Append(((DateTime)originalValue).ToUniversalTime().ToString(format, DateTimeFormatInfo.InvariantInfo));
					return;
				}
				if (property.Type.Equals(typeof(DirectoryObjectClass)) && originalValue is DirectoryObjectClass)
				{
					ADValueConvertor.EscapeAndAppendString(Enum.GetName(typeof(DirectoryObjectClass), originalValue), sb);
					return;
				}
				if (property.Type.IsEnum || (null != Nullable.GetUnderlyingType(property.Type) && Nullable.GetUnderlyingType(property.Type).IsEnum))
				{
					if (property.Type.IsEnum && property.Type.GetEnumUnderlyingType() == typeof(byte))
					{
						sb.Append(((byte)originalValue).ToString());
						return;
					}
					sb.Append(((int)originalValue).ToString());
					return;
				}
				else
				{
					if (originalValue is ApprovedApplication)
					{
						ADValueConvertor.EscapeAndAppendString(((ApprovedApplication)originalValue).ToString(), sb);
						return;
					}
					if (originalValue is string)
					{
						ADValueConvertor.EscapeAndAppendString((string)originalValue, sb);
						return;
					}
					if (originalValue is Enum)
					{
						sb.Append((int)originalValue);
						return;
					}
					sb.Append(originalValue.ToString());
					return;
				}
			}
		}

		private static string ConvertBooleanToString(bool value)
		{
			if (!value)
			{
				return "FALSE";
			}
			return "TRUE";
		}

		internal static void EscapeAndAppendString(string originalValue, StringBuilder sb)
		{
			ADValueConvertor.EscapeAndAppendString(originalValue, sb, false);
		}

		internal static void EscapeAndAppendString(string originalValue, StringBuilder sb, bool doNotEscapeWildcard)
		{
			if (sb == null)
			{
				throw new ArgumentNullException("sb", DirectoryStrings.ExArgumentNullException("sb"));
			}
			int i = 0;
			while (i < originalValue.Length)
			{
				char c = originalValue[i];
				if (c <= ' ')
				{
					if (c != '\0')
					{
						if (c != ' ')
						{
							goto IL_E0;
						}
						sb.Append((i == 0) ? "\\20" : " ");
					}
					else
					{
						sb.Append("\\00");
					}
				}
				else
				{
					switch (c)
					{
					case '(':
						sb.Append("\\28");
						break;
					case ')':
						sb.Append("\\29");
						break;
					case '*':
						if (doNotEscapeWildcard)
						{
							sb.Append(originalValue[i]);
						}
						else
						{
							sb.Append("\\2a");
						}
						break;
					default:
						if (c != '/')
						{
							if (c != '\\')
							{
								goto IL_E0;
							}
							sb.Append("\\5c");
						}
						else
						{
							sb.Append("\\2f");
						}
						break;
					}
				}
				IL_EE:
				i++;
				continue;
				IL_E0:
				sb.Append(originalValue[i]);
				goto IL_EE;
			}
		}

		internal static string EscapeBinaryValue(byte[] value)
		{
			return HexConverter.ByteArrayToEscapedHexString(value, 92, 0, value.Length);
		}

		internal static MultiValuedPropertyBase CreateGenericMultiValuedProperty(ProviderPropertyDefinition propertyDefinition, bool createAsReadOnly, ICollection values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage)
		{
			MultiValuedPropertyBase result;
			if (!ADValueConvertor.TryCreateGenericMultiValuedProperty(propertyDefinition, createAsReadOnly, values, invalidValues, readOnlyErrorMessage, out result))
			{
				throw new NotImplementedException(DataStrings.ErrorMvpNotImplemented(propertyDefinition.Type.ToString(), propertyDefinition.Name));
			}
			return result;
		}

		internal static bool TryCreateGenericMultiValuedProperty(ProviderPropertyDefinition propertyDefinition, bool createAsReadOnly, ICollection values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage, out MultiValuedPropertyBase mvp)
		{
			mvp = null;
			if (propertyDefinition.Type == typeof(ADObjectId))
			{
				mvp = new ADMultiValuedProperty<ADObjectId>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(CrossTenantObjectId))
			{
				mvp = new ADMultiValuedProperty<CrossTenantObjectId>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(ADObjectIdWithString))
			{
				mvp = new ADMultiValuedProperty<ADObjectIdWithString>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(Page))
			{
				mvp = new ADMultiValuedProperty<Page>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(ApprovedApplication))
			{
				mvp = new ApprovedApplicationCollection(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(OfflineAddressBookMapiProperty))
			{
				mvp = new ADMultiValuedProperty<OfflineAddressBookMapiProperty>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(AuthenticationMethod))
			{
				mvp = new ADMultiValuedProperty<AuthenticationMethod>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(X509Identifier))
			{
				mvp = new ADMultiValuedProperty<X509Identifier>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(OfflineAddressBookVersion))
			{
				mvp = new ADMultiValuedProperty<OfflineAddressBookVersion>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(TextMessagingStateBase))
			{
				mvp = new ADMultiValuedProperty<TextMessagingStateBase>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(int?))
			{
				mvp = new ADMultiValuedProperty<int>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(UMHuntGroup))
			{
				mvp = new ADMultiValuedProperty<UMHuntGroup>(true, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(ReplicationCursor))
			{
				mvp = new ADMultiValuedProperty<ReplicationCursor>(true, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(ReplicationNeighbor))
			{
				mvp = new ADMultiValuedProperty<ReplicationNeighbor>(true, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(ADDirSyncLink))
			{
				mvp = new ADMultiValuedProperty<ADDirSyncLink>(true, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(SyncLink))
			{
				mvp = new ADMultiValuedProperty<SyncLink>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(PropertyReference))
			{
				mvp = new ADMultiValuedProperty<PropertyReference>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(Capability))
			{
				mvp = new ADMultiValuedProperty<Capability>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			else if (propertyDefinition.Type == typeof(CountryInfo))
			{
				mvp = new ADMultiValuedProperty<CountryInfo>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(AssignedPlanValue))
			{
				mvp = new ADMultiValuedProperty<AssignedPlanValue>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(ProvisionedPlanValue))
			{
				mvp = new ADMultiValuedProperty<ProvisionedPlanValue>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(ValidationErrorValue))
			{
				mvp = new ADMultiValuedProperty<ValidationErrorValue>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(CompanyVerifiedDomainValue))
			{
				mvp = new ADMultiValuedProperty<CompanyVerifiedDomainValue>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(RightsManagementTenantKeyValue))
			{
				mvp = new ADMultiValuedProperty<RightsManagementTenantKeyValue>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(ServiceInfoValue))
			{
				mvp = new ADMultiValuedProperty<ServiceInfoValue>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(MailboxAuditOperations))
			{
				mvp = new ADMultiValuedProperty<MailboxAuditOperations>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			else if (propertyDefinition.Type == typeof(LinkMetadata))
			{
				mvp = new ADMultiValuedProperty<LinkMetadata>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			else if (propertyDefinition.Type == typeof(AttributeMetadata))
			{
				mvp = new ADMultiValuedProperty<AttributeMetadata>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			else if (propertyDefinition.Type == typeof(ReconciliationCookie))
			{
				mvp = new ADMultiValuedProperty<ReconciliationCookie>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			else if (propertyDefinition.Type == typeof(ValidationError))
			{
				mvp = new ADMultiValuedProperty<ValidationError>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			else if (propertyDefinition.Type == typeof(HybridFeature))
			{
				mvp = new ADMultiValuedProperty<HybridFeature>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			else if (propertyDefinition.Type == typeof(FullSyncObjectRequest))
			{
				mvp = new ADMultiValuedProperty<FullSyncObjectRequest>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			else if (propertyDefinition.Type == typeof(TransitionCount))
			{
				mvp = new ADMultiValuedProperty<TransitionCount>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			else if (propertyDefinition.Type == typeof(RelocationConstraint))
			{
				mvp = new ADMultiValuedProperty<RelocationConstraint>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			else if (propertyDefinition.Type == typeof(MailboxProvisioningConstraint))
			{
				mvp = new ADMultiValuedProperty<MailboxProvisioningConstraint>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			else if (propertyDefinition.Type == typeof(ScopeStorage))
			{
				mvp = new ADMultiValuedProperty<ScopeStorage>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			else if (propertyDefinition.Type == typeof(MservRecord))
			{
				mvp = new ADMultiValuedProperty<MservRecord>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			else if (propertyDefinition.Type == typeof(ProxyAddress))
			{
				mvp = new ProxyAddressCollection(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
				if (propertyDefinition is MServPropertyDefinition)
				{
					((ProxyAddressCollection)mvp).CopyChangesOnly = true;
				}
			}
			return mvp != null || ValueConvertor.TryCreateGenericMultiValuedProperty(propertyDefinition, createAsReadOnly, values, invalidValues, readOnlyErrorMessage, out mvp);
		}

		public static object SerializeData(ProviderPropertyDefinition propertyDefinition, object input)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			if (input == null)
			{
				return null;
			}
			MultiValuedPropertyBase multiValuedPropertyBase = input as MultiValuedPropertyBase;
			if (multiValuedPropertyBase != null)
			{
				return multiValuedPropertyBase;
			}
			if (input.GetType().Equals(typeof(ADObjectId)))
			{
				return input;
			}
			if (input.GetType().Equals(typeof(ADObjectIdWithString)))
			{
				return ((ADObjectIdWithString)input).ToDNStringSyntax(true);
			}
			IFormatProvider formatProvider = null;
			if (propertyDefinition is ADPropertyDefinition)
			{
				formatProvider = (propertyDefinition as ADPropertyDefinition).FormatProvider;
			}
			if (propertyDefinition.IsBinary)
			{
				return ADValueConvertor.ConvertValueToBinary(input, formatProvider);
			}
			return ADValueConvertor.ConvertValueToString(input, formatProvider);
		}

		public static object DeserializeData(ProviderPropertyDefinition propertyDefinition, object input)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			if (input == null)
			{
				return null;
			}
			MultiValuedPropertyBase multiValuedPropertyBase = input as MultiValuedPropertyBase;
			if (multiValuedPropertyBase != null)
			{
				return multiValuedPropertyBase;
			}
			if (input.GetType().Equals(typeof(ADObjectId)))
			{
				return input;
			}
			IFormatProvider formatProvider = null;
			if (propertyDefinition is ADPropertyDefinition)
			{
				formatProvider = (propertyDefinition as ADPropertyDefinition).FormatProvider;
			}
			if (propertyDefinition.IsBinary)
			{
				return ADValueConvertor.ConvertValueFromBinary((byte[])input, propertyDefinition.Type, formatProvider);
			}
			return ADValueConvertor.ConvertValueFromString((string)input, propertyDefinition.Type, formatProvider);
		}
	}
}
