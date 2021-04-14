using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data
{
	internal static class ValueConvertor
	{
		public static object UnwrapPSObjectIfNeeded(object value)
		{
			if (value == null)
			{
				return null;
			}
			if (string.Equals(value.GetType().FullName, "System.Management.Automation.PSObject", StringComparison.OrdinalIgnoreCase))
			{
				return ValueConvertor.UnwrapPSObject(value);
			}
			return value;
		}

		public static object ConvertValue(object originalValue, Type resultType, IFormatProvider formatProvider)
		{
			if (null == resultType)
			{
				throw new ArgumentNullException("resultType");
			}
			if (originalValue == null)
			{
				if (resultType.GetTypeInfo().IsValueType && (!resultType.GetTypeInfo().IsGenericType || !resultType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))))
				{
					throw new InvalidOperationException(DataStrings.ErrorCannotConvertNull(resultType.ToString()));
				}
				return null;
			}
			else
			{
				Type type = originalValue.GetType();
				object result = null;
				if (ValueConvertor.IsImmediatelyConvertible(originalValue, type, resultType, formatProvider, out result))
				{
					return result;
				}
				originalValue = ValueConvertor.UnwrapPSObjectIfNeeded(originalValue);
				if (ValueConvertor.IsImmediatelyConvertible(originalValue, type, resultType, formatProvider, out result))
				{
					return result;
				}
				if (resultType.GetTypeInfo().IsGenericType && resultType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
				{
					Type[] genericTypeArguments = resultType.GenericTypeArguments;
					if (genericTypeArguments.Length == 1)
					{
						object originalValue2 = ValueConvertor.ConvertValue(originalValue, genericTypeArguments[0], formatProvider);
						if (ValueConvertor.IsImmediatelyConvertible(originalValue2, genericTypeArguments[0], resultType, formatProvider, out result))
						{
							return result;
						}
					}
				}
				if (resultType.GetTypeInfo().IsEnum && type != typeof(string))
				{
					Type underlyingType = Enum.GetUnderlyingType(resultType);
					object originalValue3 = ValueConvertor.ConvertValue(originalValue, underlyingType, formatProvider);
					if (ValueConvertor.IsImmediatelyConvertible(originalValue3, underlyingType, resultType, formatProvider, out result))
					{
						return result;
					}
				}
				if (ValueConvertor.TryParseConversion(originalValue, type, resultType, formatProvider, out result))
				{
					return result;
				}
				if (ValueConvertor.TryConstructorConversion(originalValue, type, resultType, formatProvider, out result))
				{
					return result;
				}
				if (ValueConvertor.TryCastConversion(originalValue, type, resultType, formatProvider, out result))
				{
					return result;
				}
				if (ValueConvertor.TryIConvertibleConversion(originalValue, type, resultType, formatProvider, out result))
				{
					return result;
				}
				throw new NotImplementedException(DataStrings.ErrorOperationNotSupported(type.ToString(), resultType.ToString()));
			}
		}

		private static bool IsImmediatelyConvertible(object originalValue, Type originalType, Type resultType, IFormatProvider formatProvider, out object result)
		{
			if (resultType.GetTypeInfo().IsAssignableFrom(originalType.GetTypeInfo()))
			{
				result = originalValue;
				return true;
			}
			if (resultType.GetTypeInfo().IsEnum && originalType == Enum.GetUnderlyingType(resultType))
			{
				result = originalValue;
				return true;
			}
			result = null;
			return false;
		}

		private static bool TryParseConversion(object originalValue, Type originalType, Type resultType, IFormatProvider formatProvider, out object result)
		{
			if (originalValue is string)
			{
				try
				{
					try
					{
						result = ValueConvertor.ConvertValueFromString((string)originalValue, resultType, formatProvider);
						return true;
					}
					catch (NotImplementedException)
					{
						IEnumerable<MethodInfo> source = from methodinfo in resultType.GetTypeInfo().GetDeclaredMethods("Parse")
						where methodinfo.IsPublic && methodinfo.IsStatic && methodinfo.GetParameters().Length == 1
						select methodinfo;
						if (source.Any<MethodInfo>())
						{
							result = source.First<MethodInfo>().Invoke(null, new object[]
							{
								originalValue
							});
							return true;
						}
					}
				}
				catch (FormatException ex)
				{
					throw new TypeConversionException(DataStrings.ErrorConversionFailedWithException(originalValue.ToString(), originalType.ToString(), resultType.ToString(), ex), ex);
				}
				catch (TargetInvocationException ex2)
				{
					throw new TypeConversionException(DataStrings.ErrorConversionFailedWithException(originalValue.ToString(), originalType.ToString(), resultType.ToString(), ex2.InnerException), ex2.InnerException);
				}
			}
			result = null;
			return false;
		}

		private static bool TryCastConversion(object originalValue, Type originalType, Type resultType, IFormatProvider formatProvider, out object result)
		{
			foreach (Type targetType in new Type[]
			{
				resultType,
				originalType
			})
			{
				bool[] array2 = new bool[2];
				array2[0] = true;
				foreach (bool isImplicit in array2)
				{
					MethodInfo castOperator = ValueConvertor.GetCastOperator(isImplicit, targetType, originalType, resultType);
					if (null != castOperator)
					{
						try
						{
							result = castOperator.Invoke(null, new object[]
							{
								originalValue
							});
							return true;
						}
						catch (TargetInvocationException ex)
						{
							throw new TypeConversionException(DataStrings.ErrorConversionFailedWithException(originalValue.ToString(), originalType.ToString(), resultType.ToString(), ex.InnerException), ex.InnerException);
						}
					}
				}
			}
			result = null;
			return false;
		}

		private static MethodInfo GetCastOperator(bool isImplicit, Type targetType, Type originalType, Type resultType)
		{
			string param = isImplicit ? "op_Implicit" : "op_Explicit";
			return ReflectionHelper.TraverseTypeHierarchy<MethodInfo, string>(targetType, param, delegate(Type baseType, Type matchType, string functionToMatch)
			{
				IEnumerable<MethodInfo> source = from info in matchType.GetTypeInfo().GetDeclaredMethods(functionToMatch)
				where info.IsPublic && info.IsStatic && !info.IsAbstract && info.ReturnType.Equals(resultType) && info.GetParameters().Length == 1 && info.GetParameters()[0].ParameterType.Equals(originalType)
				select info;
				return source.FirstOrDefault<MethodInfo>();
			});
		}

		private static bool TryConstructorConversion(object originalValue, Type originalType, Type resultType, IFormatProvider formatProvider, out object result)
		{
			if (resultType.IsArray && originalValue != null && originalValue.GetType() == typeof(int))
			{
				result = null;
				return false;
			}
			foreach (ConstructorInfo constructorInfo in resultType.GetTypeInfo().DeclaredConstructors)
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				if (constructorInfo.IsPublic && parameters.Length == 1 && parameters[0].ParameterType.GetTypeInfo().IsAssignableFrom(originalType.GetTypeInfo()))
				{
					try
					{
						result = constructorInfo.Invoke(new object[]
						{
							originalValue
						});
						return true;
					}
					catch (TargetInvocationException ex)
					{
						throw new TypeConversionException(DataStrings.ErrorConversionFailedWithException(originalValue.ToString(), originalType.ToString(), resultType.ToString(), ex.InnerException), ex.InnerException);
					}
				}
			}
			result = null;
			return false;
		}

		private static bool TryIConvertibleConversion(object originalValue, Type originalType, Type resultType, IFormatProvider formatProvider, out object result)
		{
			if (originalValue is IConvertible)
			{
				try
				{
					result = Convert.ChangeType(originalValue, resultType, formatProvider);
					return true;
				}
				catch (OverflowException ex)
				{
					throw new TypeConversionException(DataStrings.ErrorConversionFailedWithException(originalValue.ToString(), originalType.ToString(), resultType.ToString(), ex), ex);
				}
				catch (InvalidCastException)
				{
				}
			}
			result = null;
			return false;
		}

		internal static byte[] ConvertValueToBinary(object originalValue, IFormatProvider formatProvider)
		{
			ExTraceGlobals.ValueConvertorTracer.TraceFunction(0L, "ValueConvertor.ConvertValueToBinary(); OriginalValue = {0}; FormatProvider = {1}", new object[]
			{
				originalValue ?? "<null>",
				formatProvider ?? "<null>"
			});
			byte[] result;
			Exception ex;
			if (ValueConvertor.TryConvertValueToBinary(originalValue, formatProvider, out result, out ex))
			{
				return result;
			}
			ExTraceGlobals.ValueConvertorTracer.TraceDebug<Exception>(0L, "ValueConvertor.ConvertValueToBinary(). Conversion Failed. Error = {0}", ex);
			if (typeof(NotImplementedException) == ex.GetType())
			{
				throw ex;
			}
			throw new FormatException(DataStrings.ErrorCannotConvertToBinary(ex.Message), ex);
		}

		internal static object ConvertValueFromBinary(byte[] originalValue, Type resultType, IFormatProvider formatProvider)
		{
			ExTraceGlobals.ValueConvertorTracer.TraceFunction(0L, "ValueConvertor.ConvertValueFromBinary(); OriginalValue.Length = {0}; ResultType = {1}; FormatProvider = {2}", new object[]
			{
				(originalValue == null) ? "<null>" : originalValue.Length,
				resultType ?? "<null",
				formatProvider ?? "<null>"
			});
			object result;
			Exception ex;
			if (ValueConvertor.TryConvertValueFromBinary(originalValue, resultType, formatProvider, out result, out ex))
			{
				return result;
			}
			ExTraceGlobals.ValueConvertorTracer.TraceDebug<Exception>(0L, "ValueConvertor.ConvertValueFromBinary(). Conversion Failed. Error = {0}", ex);
			if (typeof(NotImplementedException) == ex.GetType())
			{
				throw ex;
			}
			throw new FormatException(DataStrings.ErrorCannotConvertFromBinary(resultType.ToString(), ex.Message), ex);
		}

		internal static string ConvertValueToString(object originalValue, IFormatProvider formatProvider)
		{
			ExTraceGlobals.ValueConvertorTracer.TraceFunction(0L, "ValueConvertor.ConvertValueToString(); OriginalValue = {0}; FormatProvider = {1}", new object[]
			{
				originalValue ?? "<null>",
				formatProvider ?? "<null>"
			});
			string result;
			Exception ex;
			if (ValueConvertor.TryConvertValueToString(originalValue, formatProvider, out result, out ex))
			{
				return result;
			}
			ExTraceGlobals.ValueConvertorTracer.TraceDebug<Exception>(0L, "ValueConvertor.ConvertValueToString(). Conversion Failed. Error = {0}", ex);
			if (typeof(NotImplementedException) == ex.GetType())
			{
				throw ex;
			}
			throw new FormatException(DataStrings.ErrorCannotConvertToString(ex.Message), ex);
		}

		internal static object ConvertValueFromString(string originalValue, Type resultType, IFormatProvider formatProvider)
		{
			ExTraceGlobals.ValueConvertorTracer.TraceFunction<string, object, object>(0L, "ValueConvertor.ConvertValueFromString(); OriginalValue = {0}; ResultType = {1}; FormatProvider = {2}", originalValue ?? "<null>", resultType ?? "<null", formatProvider ?? "<null>");
			object result;
			Exception ex;
			if (ValueConvertor.TryConvertValueFromString(originalValue, resultType, formatProvider, out result, out ex))
			{
				return result;
			}
			ExTraceGlobals.ValueConvertorTracer.TraceDebug<Exception>(0L, "ValueConvertor.ConvertValueFromString(). Conversion Failed. Error = {0}", ex);
			if (typeof(NotImplementedException) == ex.GetType())
			{
				throw ex;
			}
			throw new FormatException(DataStrings.ErrorCannotConvertFromString(originalValue, resultType.ToString(), ex.Message), ex);
		}

		internal static MultiValuedPropertyBase CreateGenericMultiValuedProperty(ProviderPropertyDefinition propertyDefinition, bool createAsReadOnly, ICollection values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage)
		{
			MultiValuedPropertyBase result;
			if (!ValueConvertor.TryCreateGenericMultiValuedProperty(propertyDefinition, createAsReadOnly, values, invalidValues, readOnlyErrorMessage, out result))
			{
				throw new NotImplementedException(DataStrings.ErrorMvpNotImplemented(propertyDefinition.Type.ToString(), propertyDefinition.Name));
			}
			return result;
		}

		public static object SerializeData(ProviderPropertyDefinition propertyDefinition, object input)
		{
			ExTraceGlobals.ValueConvertorTracer.TraceFunction(0L, "ValueConvertor.SerializedData(); PropertyDefinition = {0}; Input = {1}", new object[]
			{
				propertyDefinition ?? "<null>",
				input ?? "<null>"
			});
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
			if (propertyDefinition.IsBinary)
			{
				return ValueConvertor.ConvertValueToBinary(input, null);
			}
			return ValueConvertor.ConvertValueToString(input, null);
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
			if (propertyDefinition.IsBinary)
			{
				return ValueConvertor.ConvertValueFromBinary((byte[])input, propertyDefinition.Type, null);
			}
			return ValueConvertor.ConvertValueFromString((string)input, propertyDefinition.Type, null);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static object UnwrapPSObject(object value)
		{
			PSObject psobject = (PSObject)value;
			return psobject.BaseObject;
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
			Type type = originalValue.GetType();
			if (type.Equals(typeof(byte[])))
			{
				result = (byte[])originalValue;
			}
			else if (type.Equals(typeof(Guid)))
			{
				result = ((Guid)originalValue).ToByteArray();
			}
			else if (type.Equals(typeof(SecurityIdentifier)))
			{
				SecurityIdentifier securityIdentifier = (SecurityIdentifier)originalValue;
				byte[] array = new byte[securityIdentifier.BinaryLength];
				securityIdentifier.GetBinaryForm(array, 0);
				result = array;
			}
			else if (type.Equals(typeof(RawSecurityDescriptor)))
			{
				RawSecurityDescriptor rawSecurityDescriptor = (RawSecurityDescriptor)originalValue;
				byte[] array2 = new byte[rawSecurityDescriptor.BinaryLength];
				rawSecurityDescriptor.GetBinaryForm(array2, 0);
				result = array2;
			}
			else if (type.Equals(typeof(SecurityDescriptor)))
			{
				result = ((SecurityDescriptor)originalValue).BinaryForm;
			}
			else if (type.Equals(typeof(Version)))
			{
				Version version = (Version)originalValue;
				byte[] array3 = new byte[]
				{
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					(byte)(version.Major >> 8)
				};
				array3[6] = (byte)version.Major;
				array3[5] = (byte)(version.Minor >> 8);
				array3[4] = (byte)version.Minor;
				array3[3] = (byte)(version.Build >> 8);
				array3[2] = (byte)version.Build;
				array3[1] = (byte)(version.Revision >> 8);
				array3[0] = (byte)version.Revision;
				result = array3;
			}
			else if (type.Equals(typeof(Schedule)))
			{
				result = ((Schedule)originalValue).ToByteArray();
			}
			else if (type.Equals(typeof(ScheduleInterval[])))
			{
				result = ScheduleInterval.GetWeekBitmapFromIntervals((ScheduleInterval[])originalValue);
			}
			else if (type.Equals(typeof(NetID)))
			{
				result = ((NetID)originalValue).ToByteArray();
			}
			else
			{
				error = new NotImplementedException(DataStrings.ErrorToBinaryNotImplemented(type.ToString()));
			}
			return null == error;
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
				if (resultType.Equals(typeof(byte[])))
				{
					result = originalValue;
				}
				else if (resultType.Equals(typeof(Guid)) || resultType.Equals(typeof(Guid?)))
				{
					result = new Guid(originalValue);
				}
				else if (resultType.Equals(typeof(SecurityIdentifier)))
				{
					result = new SecurityIdentifier(originalValue, 0);
				}
				else if (resultType.Equals(typeof(RawSecurityDescriptor)))
				{
					result = new RawSecurityDescriptor(originalValue, 0);
				}
				else if (resultType.Equals(typeof(SecurityDescriptor)))
				{
					result = new SecurityDescriptor(originalValue);
				}
				else if (resultType.Equals(typeof(NetID)))
				{
					result = new NetID(originalValue);
				}
				else if (resultType.Equals(typeof(Version)))
				{
					if (originalValue.Length == 8)
					{
						int major = ((int)originalValue[7] << 8) + (int)originalValue[6];
						int minor = ((int)originalValue[5] << 8) + (int)originalValue[4];
						int build = ((int)originalValue[3] << 8) + (int)originalValue[2];
						int revision = ((int)originalValue[1] << 8) + (int)originalValue[0];
						result = new Version(major, minor, build, revision);
					}
					else
					{
						error = new FormatException(DataStrings.ExLengthOfVersionByteArrayError);
					}
				}
				else if (resultType.Equals(typeof(Schedule)))
				{
					result = Schedule.FromByteArray(originalValue);
				}
				else if (resultType.Equals(typeof(ScheduleInterval[])))
				{
					result = ScheduleInterval.GetIntervalsFromWeekBitmap(originalValue);
				}
			}
			catch (FormatException ex)
			{
				error = ex;
				return false;
			}
			return null == error;
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
			if (type.Equals(typeof(string)))
			{
				result = (string)originalValue;
			}
			else if (type.Equals(typeof(Fqdn)))
			{
				result = ((Fqdn)originalValue).ToString();
			}
			else if (originalValue is ProxyAddressBase)
			{
				result = ((ProxyAddressBase)originalValue).ToString();
			}
			else if (originalValue is NetworkAddress)
			{
				result = ((NetworkAddress)originalValue).ToString();
			}
			else if (type.Equals(typeof(DateTime)))
			{
				DateTime dateTime = (DateTime)originalValue;
				IFormatProvider formatProvider2 = formatProvider ?? DateTimeFormatProvider.Generalized;
				string format = (string)formatProvider2.GetFormat(typeof(DateTimeFormatProvider.DateTimeFormat));
				result = dateTime.ToUniversalTime().ToString(format, DateTimeFormatInfo.InvariantInfo);
			}
			else if (type == typeof(bool))
			{
				result = ValueConvertor.ConvertBooleanToString((bool)originalValue);
			}
			else if (type == typeof(bool?))
			{
				result = ValueConvertor.ConvertBooleanToString(((bool?)originalValue).Value);
			}
			else if (type == typeof(uint))
			{
				result = ((uint)originalValue).ToString();
			}
			else if (type == typeof(Unlimited<ByteQuantifiedSize>))
			{
				Unlimited<ByteQuantifiedSize> unlimited = (Unlimited<ByteQuantifiedSize>)originalValue;
				if (unlimited.IsUnlimited)
				{
					result = unlimited.ToString();
				}
				else
				{
					ByteQuantifiedSize.Quantifier quantifier = ByteQuantifiedSize.Quantifier.None;
					if (formatProvider != null)
					{
						quantifier = (ByteQuantifiedSize.Quantifier)formatProvider.GetFormat(typeof(ByteQuantifiedSize.Quantifier));
					}
					result = unlimited.Value.RoundUpToUnit(quantifier).ToString();
				}
			}
			else if (type == typeof(ByteQuantifiedSize) || type == typeof(ByteQuantifiedSize?))
			{
				if (originalValue == null)
				{
					result = string.Empty;
				}
				else
				{
					ByteQuantifiedSize.Quantifier quantifier2 = ByteQuantifiedSize.Quantifier.None;
					if (formatProvider != null)
					{
						quantifier2 = (ByteQuantifiedSize.Quantifier)formatProvider.GetFormat(typeof(ByteQuantifiedSize.Quantifier));
					}
					result = ((ByteQuantifiedSize)originalValue).RoundUpToUnit(quantifier2).ToString();
				}
			}
			else if (type == typeof(EnhancedTimeSpan))
			{
				result = Convert.ToInt32(((EnhancedTimeSpan)originalValue).TotalSeconds).ToString();
			}
			else if (type == typeof(Unlimited<EnhancedTimeSpan>))
			{
				result = (((Unlimited<EnhancedTimeSpan>)originalValue).IsUnlimited ? string.Empty : ((Unlimited<EnhancedTimeSpan>)originalValue).Value.TotalSeconds.ToString());
			}
			else if (type == typeof(ExchangeObjectVersion))
			{
				result = ((ExchangeObjectVersion)originalValue).ToInt64().ToString();
			}
			else if (type == typeof(NonRootLocalLongFullPath) || type == typeof(LocalLongFullPath) || type == typeof(LongPath) || type == typeof(UncFileSharePath) || type == typeof(EdbFilePath))
			{
				result = originalValue.ToString();
			}
			else if (type == typeof(NumberFormat))
			{
				result = ((NumberFormat)originalValue).ToString();
			}
			else if (type == typeof(DialGroupEntry))
			{
				result = ((DialGroupEntry)originalValue).ToString();
			}
			else if (type == typeof(IPAddress))
			{
				result = ((IPAddress)originalValue).ToString();
			}
			else if (type == typeof(FileShareWitnessServerName))
			{
				result = ((FileShareWitnessServerName)originalValue).ToString();
			}
			else if (type == typeof(HolidaySchedule))
			{
				result = ((HolidaySchedule)originalValue).ToString();
			}
			else if (type == typeof(CustomMenuKeyMapping))
			{
				result = ((CustomMenuKeyMapping)originalValue).ToString();
			}
			else if (type == typeof(EnhancedStatusCode))
			{
				result = ((EnhancedStatusCode)originalValue).ToString();
			}
			else if (type == typeof(SmtpDomain))
			{
				result = ((SmtpDomain)originalValue).ToString();
			}
			else if (type == typeof(AutoDiscoverSmtpDomain))
			{
				result = ((AutoDiscoverSmtpDomain)originalValue).ToString();
			}
			else if (type == typeof(X400Domain))
			{
				result = ((X400Domain)originalValue).ToString();
			}
			else if (type == typeof(SmtpDomainWithSubdomains))
			{
				result = ((SmtpDomainWithSubdomains)originalValue).ToString();
			}
			else if (type == typeof(SmtpReceiveDomainCapabilities))
			{
				result = ((SmtpReceiveDomainCapabilities)originalValue).ToString();
			}
			else if (type == typeof(SmtpX509Identifier))
			{
				result = ((SmtpX509Identifier)originalValue).ToString();
			}
			else if (type == typeof(SmtpX509IdentifierEx))
			{
				result = ((SmtpX509IdentifierEx)originalValue).ToString();
			}
			else if (type == typeof(ServiceProviderSettings))
			{
				result = ((ServiceProviderSettings)originalValue).ToString();
			}
			else if (type == typeof(TlsCertificate))
			{
				result = ((TlsCertificate)originalValue).ToString();
			}
			else if (type == typeof(Fqdn))
			{
				result = ((Fqdn)originalValue).ToString();
			}
			else if (type == typeof(Oid))
			{
				result = ((Oid)originalValue).Value;
			}
			else if (type == typeof(SmtpAddress))
			{
				result = ((SmtpAddress)originalValue).ToString();
			}
			else if (type == typeof(IPBinding))
			{
				result = ((IPBinding)originalValue).ToString();
			}
			else if (type == typeof(IPRange))
			{
				result = ((IPRange)originalValue).ToString();
			}
			else if (type == typeof(IntRange))
			{
				result = ((IntRange)originalValue).ToString();
			}
			else if (type == typeof(AddressSpace))
			{
				result = ((AddressSpace)originalValue).ToString();
			}
			else if (type == typeof(SmartHost))
			{
				result = ((SmartHost)originalValue).ToString();
			}
			else if (type == typeof(UMSmartHost))
			{
				result = ((UMSmartHost)originalValue).ToString();
			}
			else if (type == typeof(RoutingHost))
			{
				result = ((RoutingHost)originalValue).ToString();
			}
			else if (type == typeof(ConnectedDomain))
			{
				result = ((ConnectedDomain)originalValue).ToString();
			}
			else if (type == typeof(DNWithBinary))
			{
				result = ((DNWithBinary)originalValue).ToString();
			}
			else if (type == typeof(AsciiString))
			{
				result = ((AsciiString)originalValue).ToString();
			}
			else if (type == typeof(ServerCostPair))
			{
				result = ((ServerCostPair)originalValue).ToString();
			}
			else if (type == typeof(CultureInfo))
			{
				result = ((CultureInfo)originalValue).ToString();
			}
			else if (type == typeof(SharingPolicyDomain))
			{
				result = ((SharingPolicyDomain)originalValue).ToString();
			}
			else if (type == typeof(Uri))
			{
				result = ((Uri)originalValue).ToString();
			}
			else if (type == typeof(CmdletRoleEntry) || type == typeof(ScriptRoleEntry) || type == typeof(ApplicationPermissionRoleEntry) || type == typeof(WebServiceRoleEntry) || type == typeof(UnknownRoleEntry))
			{
				result = ((RoleEntry)originalValue).ToADString();
			}
			else if (type == typeof(OrganizationSummaryEntry))
			{
				result = ((OrganizationSummaryEntry)originalValue).ToString();
			}
			else if (type == typeof(UMNumberingPlanFormat))
			{
				result = ((UMNumberingPlanFormat)originalValue).ToString();
			}
			else if (type.Equals(typeof(ExDateTime)) || type.Equals(typeof(ExDateTime?)))
			{
				result = ((ExDateTime)originalValue).ToUtc().UniversalTime.ToString("O");
			}
			else if (type == typeof(AlternateMailbox))
			{
				result = ((AlternateMailbox)originalValue).ToString();
			}
			else if (type == typeof(LegacyThrottlingPolicySettings) || type == typeof(ThrottlingPolicyAnonymousSettings) || type == typeof(ThrottlingPolicyE4eSettings) || type == typeof(ThrottlingPolicyEasSettings) || type == typeof(ThrottlingPolicyEwsSettings) || type == typeof(ThrottlingPolicyImapSettings) || type == typeof(ThrottlingPolicyOwaSettings) || type == typeof(ThrottlingPolicyPopSettings) || type == typeof(ThrottlingPolicyRcaSettings) || type == typeof(ThrottlingPolicyGeneralSettings) || type == typeof(ThrottlingPolicyPushNotificationSettings) || type == typeof(ThrottlingPolicyPowerShellSettings))
			{
				result = originalValue.ToString();
			}
			else if (type == typeof(ProtocolConnectionSettings))
			{
				result = ((ProtocolConnectionSettings)originalValue).ToString();
			}
			else if (type == typeof(CallerIdItem))
			{
				result = ((CallerIdItem)originalValue).ToString();
			}
			else if (type == typeof(TimeOfDay))
			{
				result = ((TimeOfDay)originalValue).ToString();
			}
			else if (type == typeof(KeyMapping))
			{
				result = ((KeyMapping)originalValue).ToString();
			}
			else if (type == typeof(KeywordHit))
			{
				result = ((KeywordHit)originalValue).ToString();
			}
			else if (type == typeof(DiscoverySearchStats))
			{
				result = ((DiscoverySearchStats)originalValue).ToString();
			}
			else if (type == typeof(Version))
			{
				result = ((Version)originalValue).ToString();
			}
			else if (type == typeof(LdapPolicy))
			{
				result = ((LdapPolicy)originalValue).ToADString();
			}
			else if (type.GetTypeInfo().IsEnum && type.GetEnumUnderlyingType() == typeof(byte))
			{
				result = ((byte)originalValue).ToString();
			}
			else if (type.GetTypeInfo().IsEnum)
			{
				result = ((int)originalValue).ToString();
			}
			else if (type.GetTypeInfo().IsValueType)
			{
				result = originalValue.ToString();
			}
			else
			{
				error = new NotImplementedException(DataStrings.ErrorToStringNotImplemented(type.ToString()));
			}
			return null == error;
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
				if (resultType == typeof(string))
				{
					result = (Globals.StringPool.IsInterned(originalValue) ?? originalValue);
				}
				else if (resultType.Equals(typeof(int)) || resultType.Equals(typeof(int?)))
				{
					int num = int.Parse(originalValue);
					result = ((num == 0) ? ValueConvertor.boxedZeroInt32 : num);
				}
				else if (resultType.Equals(typeof(uint)) || resultType.Equals(typeof(uint?)))
				{
					uint num2 = uint.Parse(originalValue);
					result = ((num2 == 0U) ? ValueConvertor.boxedZeroUInt32 : num2);
				}
				else if (resultType.Equals(typeof(long)) || resultType.Equals(typeof(long?)))
				{
					long num3 = long.Parse(originalValue);
					result = ((num3 == 0L) ? ValueConvertor.boxedZeroInt64 : num3);
				}
				else if (resultType.Equals(typeof(bool)) || resultType.Equals(typeof(bool?)))
				{
					result = BoxedConstants.GetBool(bool.Parse(originalValue));
				}
				else if (resultType.Equals(typeof(DateTime)) || resultType.Equals(typeof(DateTime?)))
				{
					result = DateTime.Parse(originalValue);
				}
				else if (resultType.Equals(typeof(ProxyAddress)))
				{
					result = ProxyAddress.Parse(originalValue);
				}
				else if (resultType.Equals(typeof(ProxyAddressTemplate)))
				{
					result = ProxyAddressTemplate.Parse(originalValue);
				}
				else if (resultType == typeof(Unlimited<ByteQuantifiedSize>) || resultType == typeof(ByteQuantifiedSize) || resultType == typeof(ByteQuantifiedSize?))
				{
					ByteQuantifiedSize.Quantifier defaultUnit = ByteQuantifiedSize.Quantifier.None;
					if (formatProvider != null)
					{
						defaultUnit = (ByteQuantifiedSize.Quantifier)formatProvider.GetFormat(typeof(ByteQuantifiedSize.Quantifier));
					}
					if (resultType == typeof(Unlimited<ByteQuantifiedSize>))
					{
						result = Unlimited<ByteQuantifiedSize>.Parse(originalValue, defaultUnit);
					}
					else
					{
						ByteQuantifiedSize byteQuantifiedSize = ByteQuantifiedSize.Parse(originalValue, defaultUnit);
						if (resultType == typeof(ByteQuantifiedSize?))
						{
							result = new ByteQuantifiedSize?(byteQuantifiedSize);
						}
						else
						{
							result = byteQuantifiedSize;
						}
					}
				}
				else if (resultType == typeof(ScheduleInterval))
				{
					result = ScheduleInterval.Parse(originalValue);
				}
				else if (resultType == typeof(NonRootLocalLongFullPath))
				{
					result = NonRootLocalLongFullPath.Parse(originalValue);
				}
				else if (resultType == typeof(LocalLongFullPath))
				{
					result = LocalLongFullPath.Parse(originalValue);
				}
				else if (resultType == typeof(UncFileSharePath))
				{
					result = UncFileSharePath.Parse(originalValue);
				}
				else if (resultType == typeof(EdbFilePath))
				{
					result = EdbFilePath.Parse(originalValue);
				}
				else if (resultType == typeof(Uri))
				{
					result = new Uri(originalValue, UriKind.RelativeOrAbsolute);
				}
				else if (resultType == typeof(Hostname))
				{
					result = Hostname.Parse(originalValue);
				}
				else if (resultType == typeof(NetworkAddress))
				{
					result = NetworkAddress.Parse(originalValue);
				}
				else if (resultType == typeof(UMLanguage))
				{
					result = UMLanguage.Parse(originalValue);
				}
				else if (resultType == typeof(OrganizationSummaryEntry))
				{
					result = OrganizationSummaryEntry.Parse(originalValue);
				}
				else if (resultType == typeof(Guid))
				{
					result = new Guid(originalValue);
				}
				else if (resultType == typeof(EnhancedTimeSpan))
				{
					result = EnhancedTimeSpan.FromSeconds((double)int.Parse(originalValue));
				}
				else if (resultType == typeof(Unlimited<EnhancedTimeSpan>))
				{
					result = new Unlimited<EnhancedTimeSpan>(EnhancedTimeSpan.FromSeconds((double)int.Parse(originalValue)));
				}
				else if (resultType == typeof(EnhancedTimeSpan?))
				{
					result = new EnhancedTimeSpan?(EnhancedTimeSpan.FromSeconds((double)int.Parse(originalValue)));
				}
				else if (resultType == typeof(NumberFormat))
				{
					result = NumberFormat.Parse(originalValue);
				}
				else if (resultType == typeof(DialGroupEntry))
				{
					result = DialGroupEntry.Parse(originalValue);
				}
				else if (resultType == typeof(IPAddress))
				{
					result = IPAddress.Parse(originalValue);
				}
				else if (resultType == typeof(FileShareWitnessServerName))
				{
					result = FileShareWitnessServerName.Parse(originalValue);
				}
				else if (resultType == typeof(HolidaySchedule))
				{
					result = HolidaySchedule.Parse(originalValue);
				}
				else if (resultType == typeof(CustomMenuKeyMapping))
				{
					result = CustomMenuKeyMapping.Parse(originalValue);
				}
				else if (resultType == typeof(Unlimited<int>))
				{
					result = Unlimited<int>.Parse(originalValue);
				}
				else if (resultType == typeof(EnhancedStatusCode))
				{
					result = EnhancedStatusCode.Parse(originalValue);
				}
				else if (resultType == typeof(SmtpDomain))
				{
					result = SmtpDomain.Parse(originalValue);
				}
				else if (resultType == typeof(AutoDiscoverSmtpDomain))
				{
					result = AutoDiscoverSmtpDomain.Parse(originalValue);
				}
				else if (resultType == typeof(X400Domain))
				{
					result = X400Domain.Parse(originalValue);
				}
				else if (resultType == typeof(SmtpDomainWithSubdomains))
				{
					result = SmtpDomainWithSubdomains.Parse(originalValue);
				}
				else if (resultType == typeof(SmtpReceiveDomainCapabilities))
				{
					result = SmtpReceiveDomainCapabilities.Parse(originalValue);
				}
				else if (resultType == typeof(SmtpX509Identifier))
				{
					result = SmtpX509Identifier.Parse(originalValue);
				}
				else if (resultType == typeof(SmtpX509IdentifierEx))
				{
					result = SmtpX509IdentifierEx.Parse(originalValue);
				}
				else if (resultType == typeof(ServiceProviderSettings))
				{
					result = ServiceProviderSettings.Parse(originalValue);
				}
				else if (resultType == typeof(TlsCertificate))
				{
					result = TlsCertificate.Parse(originalValue);
				}
				else if (resultType == typeof(Fqdn))
				{
					result = Fqdn.Parse(originalValue);
				}
				else if (resultType == typeof(Oid))
				{
					result = new Oid(originalValue, originalValue);
				}
				else if (resultType == typeof(SmtpAddress))
				{
					result = SmtpAddress.Parse(originalValue);
				}
				else if (resultType == typeof(SmtpAddress?))
				{
					result = new SmtpAddress?(SmtpAddress.Parse(originalValue));
				}
				else if (resultType == typeof(IPBinding))
				{
					result = IPBinding.Parse(originalValue);
				}
				else if (resultType == typeof(IPRange))
				{
					result = IPRange.Parse(originalValue);
				}
				else if (resultType == typeof(IntRange))
				{
					result = IntRange.Parse(originalValue);
				}
				else if (resultType == typeof(AddressSpace))
				{
					result = AddressSpace.Parse(originalValue);
				}
				else if (resultType == typeof(SmartHost))
				{
					result = SmartHost.Parse(originalValue);
				}
				else if (resultType == typeof(UMSmartHost))
				{
					result = UMSmartHost.Parse(originalValue);
				}
				else if (resultType == typeof(RoutingHost))
				{
					result = RoutingHost.Parse(originalValue);
				}
				else if (resultType == typeof(ConnectedDomain))
				{
					result = ConnectedDomain.Parse(originalValue);
				}
				else if (resultType == typeof(DNWithBinary))
				{
					result = DNWithBinary.Parse(originalValue);
				}
				else if (resultType == typeof(AsciiString))
				{
					result = AsciiString.Parse(originalValue);
				}
				else if (resultType == typeof(ServerCostPair))
				{
					result = ServerCostPair.Parse(originalValue);
				}
				else if (resultType == typeof(CultureInfo))
				{
					result = CultureInfo.GetCultureInfo(originalValue);
				}
				else if (resultType == typeof(ExchangeObjectVersion))
				{
					result = new ExchangeObjectVersion(long.Parse(originalValue));
				}
				else if (resultType == typeof(RoleEntry))
				{
					result = RoleEntry.Parse(originalValue);
				}
				else if (resultType == typeof(UMNumberingPlanFormat))
				{
					result = UMNumberingPlanFormat.Parse(originalValue);
				}
				else if (resultType == typeof(SharingPolicyDomain))
				{
					result = SharingPolicyDomain.Parse(originalValue);
				}
				else if (resultType == typeof(ExDateTime) || resultType == typeof(ExDateTime?))
				{
					result = ExDateTime.ParseExact(ExTimeZone.CurrentTimeZone, originalValue, "O", null);
				}
				else if (resultType == typeof(AlternateMailbox))
				{
					result = AlternateMailbox.Parse(originalValue);
				}
				else if (resultType == typeof(LegacyThrottlingPolicySettings))
				{
					result = LegacyThrottlingPolicySettings.Parse(originalValue);
				}
				else if (resultType == typeof(ThrottlingPolicyAnonymousSettings))
				{
					result = ThrottlingPolicyAnonymousSettings.Parse(originalValue);
				}
				else if (resultType == typeof(ThrottlingPolicyE4eSettings))
				{
					result = ThrottlingPolicyE4eSettings.Parse(originalValue);
				}
				else if (resultType == typeof(ThrottlingPolicyEasSettings))
				{
					result = ThrottlingPolicyEasSettings.Parse(originalValue);
				}
				else if (resultType == typeof(ThrottlingPolicyEwsSettings))
				{
					result = ThrottlingPolicyEwsSettings.Parse(originalValue);
				}
				else if (resultType == typeof(ThrottlingPolicyImapSettings))
				{
					result = ThrottlingPolicyImapSettings.Parse(originalValue);
				}
				else if (resultType == typeof(ThrottlingPolicyOwaSettings))
				{
					result = ThrottlingPolicyOwaSettings.Parse(originalValue);
				}
				else if (resultType == typeof(ThrottlingPolicyPopSettings))
				{
					result = ThrottlingPolicyPopSettings.Parse(originalValue);
				}
				else if (resultType == typeof(ThrottlingPolicyRcaSettings))
				{
					result = ThrottlingPolicyRcaSettings.Parse(originalValue);
				}
				else if (resultType == typeof(ThrottlingPolicyGeneralSettings))
				{
					result = ThrottlingPolicyGeneralSettings.Parse(originalValue);
				}
				else if (resultType == typeof(ThrottlingPolicyPushNotificationSettings))
				{
					result = ThrottlingPolicyPushNotificationSettings.Parse(originalValue);
				}
				else if (resultType == typeof(ThrottlingPolicyPowerShellSettings))
				{
					result = ThrottlingPolicyPowerShellSettings.Parse(originalValue);
				}
				else if (resultType == typeof(ProtocolConnectionSettings))
				{
					result = ProtocolConnectionSettings.Parse(originalValue);
				}
				else if (resultType == typeof(CallerIdItem))
				{
					result = CallerIdItem.Parse(originalValue);
				}
				else if (resultType == typeof(KeyMapping))
				{
					result = KeyMapping.Parse(originalValue);
				}
				else if (resultType == typeof(TimeOfDay))
				{
					result = TimeOfDay.Parse(originalValue);
				}
				else if (resultType == typeof(KeywordHit))
				{
					result = KeywordHit.Parse(originalValue);
				}
				else if (resultType == typeof(DiscoverySearchStats))
				{
					result = DiscoverySearchStats.Parse(originalValue);
				}
				else if (resultType == typeof(LdapPolicy))
				{
					result = LdapPolicy.Parse(originalValue);
				}
				else if (resultType == typeof(Version))
				{
					result = Version.Parse(originalValue);
				}
				else
				{
					if (resultType.GetTypeInfo().IsEnum)
					{
						try
						{
							result = Enum.Parse(resultType, originalValue);
							goto IL_C0E;
						}
						catch (ArgumentException)
						{
							result = Enum.Parse(resultType, originalValue, true);
							goto IL_C0E;
						}
					}
					error = new NotImplementedException(DataStrings.ErrorOperationNotSupported(typeof(string).ToString(), resultType.ToString()));
				}
				IL_C0E:;
			}
			catch (FormatException ex)
			{
				error = ex;
				return false;
			}
			catch (ArgumentException ex2)
			{
				if (resultType.GetTypeInfo().IsEnum)
				{
					string values = string.Join(", ", Enum.GetNames(resultType));
					error = new ArgumentException(DataStrings.ErrorInvalidEnumValue(values), ex2);
				}
				else
				{
					error = ex2;
				}
				return false;
			}
			catch (OverflowException ex3)
			{
				error = ex3;
				return false;
			}
			return null == error;
		}

		private static string ConvertBooleanToString(bool value)
		{
			if (!value)
			{
				return "FALSE";
			}
			return "TRUE";
		}

		internal static bool TryCreateGenericMultiValuedProperty(ProviderPropertyDefinition propertyDefinition, bool createAsReadOnly, ICollection values, ICollection invalidValues, LocalizedString? readOnlyErrorMessage, out MultiValuedPropertyBase mvp)
		{
			mvp = null;
			if (propertyDefinition.Type == typeof(string))
			{
				mvp = new MultiValuedProperty<string>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(byte[]))
			{
				mvp = new MultiValuedProperty<byte[]>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(Guid))
			{
				mvp = new MultiValuedProperty<Guid>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(SecurityIdentifier))
			{
				mvp = new MultiValuedProperty<SecurityIdentifier>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(int))
			{
				mvp = new MultiValuedProperty<int>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(DialGroupEntry))
			{
				mvp = new MultiValuedProperty<DialGroupEntry>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(HolidaySchedule))
			{
				mvp = new MultiValuedProperty<HolidaySchedule>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(CustomMenuKeyMapping))
			{
				mvp = new MultiValuedProperty<CustomMenuKeyMapping>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(EnhancedStatusCode))
			{
				mvp = new MultiValuedProperty<EnhancedStatusCode>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(SmtpDomain))
			{
				mvp = new MultiValuedProperty<SmtpDomain>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(AutoDiscoverSmtpDomain))
			{
				mvp = new MultiValuedProperty<AutoDiscoverSmtpDomain>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(X400Domain))
			{
				mvp = new MultiValuedProperty<X400Domain>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(Fqdn))
			{
				mvp = new MultiValuedProperty<Fqdn>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(SmtpDomainWithSubdomains))
			{
				mvp = new MultiValuedProperty<SmtpDomainWithSubdomains>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(SmtpReceiveDomainCapabilities))
			{
				mvp = new MultiValuedProperty<SmtpReceiveDomainCapabilities>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(SmtpX509Identifier))
			{
				mvp = new MultiValuedProperty<SmtpX509Identifier>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(SmtpX509IdentifierEx))
			{
				mvp = new MultiValuedProperty<SmtpX509IdentifierEx>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(ServiceProviderSettings))
			{
				mvp = new MultiValuedProperty<ServiceProviderSettings>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(TlsCertificate))
			{
				mvp = new MultiValuedProperty<TlsCertificate>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(Oid))
			{
				mvp = new MultiValuedProperty<Oid>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(SmtpAddress))
			{
				mvp = new MultiValuedProperty<SmtpAddress>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(IPAddress))
			{
				mvp = new MultiValuedProperty<IPAddress>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(IPBinding))
			{
				mvp = new MultiValuedProperty<IPBinding>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(IPRange))
			{
				mvp = new MultiValuedProperty<IPRange>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(IntRange))
			{
				mvp = new MultiValuedProperty<IntRange>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(ClientAccessAuthenticationMethod))
			{
				mvp = new MultiValuedProperty<ClientAccessAuthenticationMethod>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(ClientAccessProtocol))
			{
				mvp = new MultiValuedProperty<ClientAccessProtocol>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(AddressSpace))
			{
				mvp = new MultiValuedProperty<AddressSpace>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(SmartHost))
			{
				mvp = new MultiValuedProperty<SmartHost>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(RoutingHost))
			{
				mvp = new MultiValuedProperty<RoutingHost>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(ConnectedDomain))
			{
				mvp = new MultiValuedProperty<ConnectedDomain>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(ProxyAddressTemplate))
			{
				mvp = new ProxyAddressTemplateCollection(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(ProxyAddress))
			{
				mvp = new ProxyAddressCollection(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(NetworkAddress))
			{
				mvp = new NetworkAddressCollection(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(DNWithBinary))
			{
				mvp = new MultiValuedProperty<DNWithBinary>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(ServerCostPair))
			{
				mvp = new MultiValuedProperty<ServerCostPair>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(CultureInfo))
			{
				mvp = new MultiValuedProperty<CultureInfo>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(UMLanguage))
			{
				mvp = new MultiValuedProperty<UMLanguage>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(OrganizationSummaryEntry))
			{
				mvp = new MultiValuedProperty<OrganizationSummaryEntry>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(RoleEntry))
			{
				mvp = new MultiValuedProperty<RoleEntry>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(UMNumberingPlanFormat))
			{
				mvp = new MultiValuedProperty<UMNumberingPlanFormat>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(DatabaseAvailabilityGroupNetworkSubnet))
			{
				mvp = new DagNetMultiValuedProperty<DatabaseAvailabilityGroupNetworkSubnet>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(DatabaseAvailabilityGroupNetworkInterface))
			{
				mvp = new DagNetMultiValuedProperty<DatabaseAvailabilityGroupNetworkInterface>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(AlternateMailbox))
			{
				mvp = new MultiValuedProperty<AlternateMailbox>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(SharingPolicyDomain))
			{
				mvp = new MultiValuedProperty<SharingPolicyDomain>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(Uri))
			{
				mvp = new MultiValuedProperty<Uri>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(ProtocolConnectionSettings))
			{
				mvp = new MultiValuedProperty<ProtocolConnectionSettings>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(MapiFolderPath))
			{
				mvp = new MultiValuedProperty<MapiFolderPath>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(PublicFolderAdministrativePermission))
			{
				mvp = new MultiValuedProperty<PublicFolderAdministrativePermission>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(AdminAuditLogCmdletParameter))
			{
				mvp = new MultiValuedProperty<AdminAuditLogCmdletParameter>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(AdminAuditLogModifiedProperty))
			{
				mvp = new MultiValuedProperty<AdminAuditLogModifiedProperty>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(MailboxAuditLogSourceItem))
			{
				mvp = new MultiValuedProperty<MailboxAuditLogSourceItem>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(MailboxAuditLogSourceFolder))
			{
				mvp = new MultiValuedProperty<MailboxAuditLogSourceFolder>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(WindowsLiveIdApplicationCertificate))
			{
				mvp = new MultiValuedProperty<WindowsLiveIdApplicationCertificate>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(AttributeMetadata))
			{
				mvp = new MultiValuedProperty<AttributeMetadata>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(KeywordHit))
			{
				mvp = new MultiValuedProperty<KeywordHit>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(DiscoverySearchStats))
			{
				mvp = new MultiValuedProperty<DiscoverySearchStats>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(QueueViewerSortOrderEntry))
			{
				mvp = new MultiValuedProperty<QueueViewerSortOrderEntry>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(RecipientAccessRight))
			{
				mvp = new MultiValuedProperty<RecipientAccessRight>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(LdapPolicy))
			{
				mvp = new MultiValuedProperty<LdapPolicy>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(CallerIdItem))
			{
				mvp = new MultiValuedProperty<CallerIdItem>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			if (propertyDefinition.Type == typeof(KeyMapping))
			{
				mvp = new MultiValuedProperty<KeyMapping>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			else if (propertyDefinition.Type == typeof(TransportQueueLog))
			{
				mvp = new MultiValuedProperty<TransportQueueLog>(createAsReadOnly, propertyDefinition, values, invalidValues, readOnlyErrorMessage);
			}
			return null != mvp;
		}

		private static object boxedZeroInt32 = 0;

		private static object boxedZeroUInt32 = 0U;

		private static object boxedZeroInt64 = 0L;
	}
}
