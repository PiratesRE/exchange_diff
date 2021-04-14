using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	[DebuggerDisplay("Number = {phone} ToDisplay = {displayPhone} UriType = {uriType} NumberType = {numberType}")]
	public class PhoneNumber
	{
		public PhoneNumber() : this(null)
		{
		}

		public PhoneNumber(string phone) : this(phone, PhoneNumberKind.Unknown)
		{
		}

		public PhoneNumber(string phone, PhoneNumberKind type) : this(phone, type, UMUriType.TelExtn)
		{
		}

		public PhoneNumber(string phone, PhoneNumberKind type, UMUriType uriType) : this(phone, null, type, uriType)
		{
		}

		public PhoneNumber(string phone, string displayPhone, PhoneNumberKind type, UMUriType uriType)
		{
			this.phone = ((phone != null) ? phone : string.Empty);
			this.numberType = type;
			this.uriType = uriType;
			this.displayPhone = displayPhone;
			this.ToDial = ((!this.IsEmpty && this.uriType == UMUriType.E164) ? ("+" + this.Number) : this.Number);
		}

		public static PhoneNumber Empty
		{
			get
			{
				return PhoneNumber.emptyPhone;
			}
		}

		public string Number
		{
			get
			{
				return this.phone;
			}
		}

		public PhoneNumberKind Kind
		{
			get
			{
				return this.numberType;
			}
		}

		public UMUriType UriType
		{
			get
			{
				return this.uriType;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty(this.phone);
			}
		}

		public string ToDial { get; private set; }

		public string ToDisplay
		{
			get
			{
				if (string.IsNullOrEmpty(this.displayPhone))
				{
					this.displayPhone = PhoneNumber.PhoneNumberFormatter.FormatNumber(this);
				}
				return this.displayPhone;
			}
		}

		public static PhoneNumber CreateExtension(string number)
		{
			return new PhoneNumber(number, PhoneNumberKind.Extension);
		}

		public static PhoneNumber CreateNational(string number)
		{
			return new PhoneNumber(number, PhoneNumberKind.National);
		}

		public static PhoneNumber CreateInternational(string number)
		{
			return new PhoneNumber(number, PhoneNumberKind.International);
		}

		public static bool TryCreateE164Number(string number, string countryOrRegionCode, out PhoneNumber result)
		{
			result = null;
			bool result2 = false;
			if (!string.IsNullOrEmpty(number) && !string.IsNullOrEmpty(countryOrRegionCode) && Utils.IsNumber(number) && Utils.IsNumber(countryOrRegionCode))
			{
				string text = countryOrRegionCode + number;
				if (text.Length <= 15)
				{
					result = new PhoneNumber(text, PhoneNumberKind.Unknown, UMUriType.E164);
					result2 = true;
				}
			}
			return result2;
		}

		public static bool TryParse(string phone, bool isNullOrEmptyStringValid, out PhoneNumber parsedNumber)
		{
			return PhoneNumber.TryParse(null, phone, isNullOrEmptyStringValid, out parsedNumber);
		}

		public static bool TryParse(string phone, out PhoneNumber parsedNumber)
		{
			return PhoneNumber.TryParse(null, phone, false, out parsedNumber);
		}

		public static bool TryParse(UMDialPlan dialPlan, string phone, out PhoneNumber parsedNumber)
		{
			return PhoneNumber.TryParse(dialPlan, phone, false, out parsedNumber);
		}

		public static bool TryParse(UMDialPlan dialPlan, string phone, bool isNullOrEmptyStringValid, out PhoneNumber parsedNumber)
		{
			parsedNumber = null;
			bool flag = false;
			PhoneNumberKind type = PhoneNumberKind.Unknown;
			phone = Utils.TrimSpaces(phone);
			if (!string.IsNullOrEmpty(phone))
			{
				phone = phone.ToLowerInvariant().Trim();
				phone = Utils.RemoveSchemePrefix("TEL:", phone);
				UMUriType umuriType = Utils.DetermineNumberType(phone);
				if (umuriType == UMUriType.SipName)
				{
					phone = Utils.RemoveSIPPrefix(phone);
					parsedNumber = new PhoneNumber(phone, type, umuriType);
					flag = true;
				}
				else
				{
					string text = DtmfString.SanitizePhoneNumber(phone);
					if (!string.IsNullOrEmpty(text))
					{
						flag = true;
						string text2 = string.Equals(phone, text, StringComparison.OrdinalIgnoreCase) ? null : phone;
						string text3 = text.StartsWith("+", StringComparison.OrdinalIgnoreCase) ? text.Substring(1) : text;
						parsedNumber = new PhoneNumber(text3, text2, type, umuriType);
					}
				}
				if (flag && dialPlan != null)
				{
					parsedNumber.SetPhoneNumberKindForDialPlan(dialPlan);
				}
				return flag;
			}
			if (isNullOrEmptyStringValid)
			{
				parsedNumber = new PhoneNumber();
				return true;
			}
			return false;
		}

		public static PhoneNumber Parse(string phone)
		{
			if (string.IsNullOrEmpty(phone))
			{
				throw new ArgumentNullException("phone");
			}
			PhoneNumber result = null;
			if (!PhoneNumber.TryParse(phone, out result))
			{
				return null;
			}
			return result;
		}

		public static bool IsNullOrEmpty(PhoneNumber number)
		{
			return number == null || number.IsEmpty;
		}

		public static bool IsValidPhoneNumber(string phone)
		{
			bool result = false;
			phone = phone.ToLowerInvariant().Trim();
			UMUriType umuriType = Utils.DetermineNumberType(phone);
			if (umuriType == UMUriType.SipName)
			{
				result = true;
			}
			else
			{
				string value = DtmfString.SanitizePhoneNumber(phone);
				if (!string.IsNullOrEmpty(value))
				{
					result = true;
				}
			}
			return result;
		}

		public bool IsMatch(string matchString, UMDialPlan dialplan)
		{
			if (matchString == null)
			{
				throw new ArgumentNullException("matchString cannot be null");
			}
			if (dialplan == null)
			{
				throw new ArgumentException("dialplan cannot be null");
			}
			return this.IsMatch(matchString, this.GetOptionalPrefixes(dialplan));
		}

		public bool IsMatch(string matchString, List<string> optionalPrefixes)
		{
			if (optionalPrefixes == null)
			{
				throw new ArgumentException("optionalPrefixes cannot be null");
			}
			if (string.IsNullOrEmpty(matchString) || string.IsNullOrEmpty(matchString.Trim()))
			{
				return false;
			}
			if (this.IsEmpty || string.IsNullOrEmpty(this.ToDial))
			{
				return false;
			}
			PhoneNumber phoneNumber = null;
			if (PhoneNumber.TryParse(matchString, out phoneNumber))
			{
				matchString = phoneNumber.ToDial;
			}
			string text = this.ToDial;
			foreach (string text2 in optionalPrefixes)
			{
				if (text.StartsWith(text2, StringComparison.OrdinalIgnoreCase) && text.Length > text2.Length)
				{
					text = text.Substring(text2.Length);
					break;
				}
			}
			bool result = false;
			if (matchString.EndsWith(text, StringComparison.OrdinalIgnoreCase))
			{
				if (matchString.Length == text.Length)
				{
					result = true;
				}
				else
				{
					int length = matchString.Length - text.Length;
					string item = matchString.Substring(0, length);
					if (optionalPrefixes.Contains(item))
					{
						result = true;
					}
				}
			}
			return result;
		}

		public List<string> GetOptionalPrefixes(UMDialPlan dialPlan)
		{
			List<string> list = new List<string>(7);
			string text = dialPlan.CountryOrRegionCode ?? string.Empty;
			string text2 = dialPlan.NationalNumberPrefix ?? string.Empty;
			string text3 = dialPlan.InternationalAccessCode ?? string.Empty;
			bool flag = !string.IsNullOrEmpty(text);
			bool flag2 = !string.IsNullOrEmpty(text2);
			bool flag3 = !string.IsNullOrEmpty(text3);
			if (!this.ToDial.StartsWith("+", StringComparison.OrdinalIgnoreCase) || !flag || this.ToDial.StartsWith("+" + text, StringComparison.OrdinalIgnoreCase))
			{
				if (flag && flag2)
				{
					list.Add("+" + text + text2);
				}
				if (flag)
				{
					list.Add("+" + text);
				}
				if (flag3)
				{
					if (flag && flag2)
					{
						list.Add(text3 + text + text2);
					}
					if (flag)
					{
						list.Add(text3 + text);
					}
				}
				if (flag && flag2)
				{
					list.Add(text + text2);
				}
				if (flag)
				{
					list.Add(text);
				}
				if (flag2)
				{
					list.Add(text2);
				}
			}
			list.Add("+");
			list.Add(string.Empty);
			return list;
		}

		public PhoneNumber Extend(UMDialPlan dialPlan)
		{
			string text = null;
			PhoneNumber phoneNumber = this;
			if (dialPlan.TryMapNumberingPlan(this.Number, out text) && !PhoneNumber.TryParse(text, out phoneNumber))
			{
				phoneNumber = this;
			}
			PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber);
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, data, "PhoneNumber.Extend using extended number '_PhoneNumber'", new object[0]);
			return phoneNumber;
		}

		public bool StartsWithTrunkAccessCode(UMDialPlan dialPlan)
		{
			string text = Utils.TrimSpaces(dialPlan.OutsideLineAccessCode);
			return text != null && this.Number.StartsWith(text, StringComparison.Ordinal);
		}

		public bool IsValid(UMDialPlan dialPlan)
		{
			return PhoneNumberKind.Extension != this.Kind || this.Number.Length == dialPlan.NumberOfDigitsInExtension;
		}

		public PhoneNumber Clone()
		{
			return this.Clone(null);
		}

		public PhoneNumber Clone(UMDialPlan dialPlan)
		{
			if (this.IsEmpty)
			{
				return PhoneNumber.Empty;
			}
			PhoneNumber phoneNumber = new PhoneNumber(this.phone, this.numberType, this.uriType);
			if (dialPlan != null)
			{
				phoneNumber.SetPhoneNumberKindForDialPlan(dialPlan);
			}
			return phoneNumber;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} ({1}:{2})", new object[]
			{
				this.phone,
				this.numberType,
				this.uriType
			});
		}

		public string RenderUserPart(UMDialPlan dialPlan)
		{
			if (dialPlan.URIType != UMUriType.SipName)
			{
				return this.ToDial;
			}
			switch (this.UriType)
			{
			case UMUriType.TelExtn:
				return this.ToDial + ";phone-context=user-default";
			case UMUriType.E164:
				return this.ToDial;
			default:
				throw new InvalidOperationException();
			}
		}

		internal PhoneNumber GetPstnCallbackTelephoneNumber(ContactInfo contact, UMDialPlan dialPlan)
		{
			PhoneNumber phoneNumber = PhoneNumber.Empty;
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "GetPstnCallbackTelephoneNumber: contact:{0}, dialPlan:{1}, callerId(this):{2}", new object[]
			{
				contact,
				dialPlan,
				this
			});
			if (this.IsEmpty || this.UriType == UMUriType.E164)
			{
				phoneNumber = this;
			}
			else
			{
				PhoneNumber phoneNumber2 = null;
				if (this.UriType == UMUriType.TelExtn)
				{
					phoneNumber2 = this;
				}
				else if (this.UriType == UMUriType.SipName && !PhoneNumber.TryParse(contact.SipLine, out phoneNumber2))
				{
					PhoneNumber.TryParse(contact.BusinessPhone, out phoneNumber2);
				}
				if (phoneNumber2 != null)
				{
					phoneNumber = ((dialPlan != null) ? phoneNumber2.Extend(dialPlan) : phoneNumber2);
					PIIMessage piimessage = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber2);
					PIIMessage piimessage2 = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber);
					PIIMessage[] data = new PIIMessage[]
					{
						piimessage,
						piimessage2
					};
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data, "_PhoneNumber(1).Extend({0}) -> _PhoneNumber(2)", new object[]
					{
						dialPlan
					});
					if (string.Equals(phoneNumber.Number, phoneNumber2.Number, StringComparison.OrdinalIgnoreCase))
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "Looping through SanitizedPhoneNumbers", new object[0]);
						string text = string.Empty;
						foreach (string text2 in contact.SanitizedPhoneNumbers)
						{
							if (text2.EndsWith(phoneNumber2.Number, StringComparison.OrdinalIgnoreCase) && text2.Length > text.Length)
							{
								text = text2;
							}
							PIIMessage piimessage3 = PIIMessage.Create(PIIType._PhoneNumber, text2);
							PIIMessage piimessage4 = PIIMessage.Create(PIIType._PhoneNumber, text);
							PIIMessage[] data2 = new PIIMessage[]
							{
								piimessage3,
								piimessage4
							};
							CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data2, "Analyzing:_PhoneNumber(1) BestMatch:_PhoneNumber(2)", new object[0]);
						}
						PhoneNumber phoneNumber3;
						if (!string.IsNullOrEmpty(text) && PhoneNumber.TryParse(text, out phoneNumber3))
						{
							phoneNumber = phoneNumber3;
						}
					}
				}
			}
			PIIMessage data3 = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber);
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data3, "Returning CallbackNumber:_PhoneNumber", new object[0]);
			return phoneNumber;
		}

		private void SetPhoneNumberKindForDialPlan(UMDialPlan dialPlan)
		{
			this.numberType = PhoneNumberKind.Unknown;
			if (this.IsEmpty || this.uriType == UMUriType.SipName)
			{
				return;
			}
			string text = Utils.TrimSpaces(dialPlan.OutsideLineAccessCode);
			string text2 = Utils.TrimSpaces(dialPlan.InternationalAccessCode);
			string text3 = Utils.TrimSpaces(dialPlan.CountryOrRegionCode);
			if ((dialPlan.URIType == UMUriType.TelExtn || dialPlan.URIType == UMUriType.SipName) && this.phone.Length == dialPlan.NumberOfDigitsInExtension)
			{
				this.numberType = PhoneNumberKind.Extension;
				return;
			}
			if (text3 != null && this.UriType == UMUriType.E164)
			{
				if (this.phone.StartsWith(text3, StringComparison.Ordinal))
				{
					this.numberType = PhoneNumberKind.National;
					return;
				}
				this.numberType = PhoneNumberKind.International;
				return;
			}
			else
			{
				if (text != null && text2 != null && this.phone.StartsWith(text + text2, StringComparison.Ordinal))
				{
					this.numberType = PhoneNumberKind.International;
					return;
				}
				if (text2 != null && this.phone.StartsWith(text2, StringComparison.Ordinal))
				{
					this.numberType = PhoneNumberKind.International;
					return;
				}
				this.numberType = PhoneNumberKind.National;
				return;
			}
		}

		private const int E164Length = 15;

		private const int SIPNameLength = 32;

		private static PhoneNumber emptyPhone = new PhoneNumber();

		private PhoneNumberKind numberType;

		private string phone;

		private string displayPhone;

		private UMUriType uriType;

		private abstract class PhoneNumberFormatter
		{
			static PhoneNumberFormatter()
			{
				PhoneNumber.PhoneNumberFormatter.genericE164Formatter = new PhoneNumber.PhoneNumberFormatter.GenericE164Formatter();
			}

			internal static string FormatNumber(PhoneNumber number)
			{
				string text = null;
				if (number.UriType == UMUriType.E164)
				{
					text = PhoneNumber.PhoneNumberFormatter.genericE164Formatter.Format(number);
				}
				if (text == null)
				{
					text = PhoneNumber.PhoneNumberFormatter.defaultFormatter.Format(number);
				}
				return text;
			}

			protected abstract string Format(PhoneNumber phoneNumber);

			private static PhoneNumber.PhoneNumberFormatter.GenericE164Formatter genericE164Formatter;

			private static PhoneNumber.PhoneNumberFormatter.DefaultFormatter defaultFormatter = new PhoneNumber.PhoneNumberFormatter.DefaultFormatter();

			protected class FormatterPattern
			{
				internal FormatterPattern(string format)
				{
					if (string.IsNullOrEmpty(format))
					{
						throw new ArgumentException("Telephone pattern cannot be null or empty", "format");
					}
					int num = 0;
					int i = 0;
					while (i < format.Length)
					{
						if (format[i] == 'i')
						{
							try
							{
								do
								{
									i++;
								}
								while (format[i] != 'i');
								goto IL_85;
							}
							catch (IndexOutOfRangeException innerException)
							{
								throw new ArgumentException("Pattern " + format + " is not valid", "format", innerException);
							}
							goto IL_5D;
						}
						goto IL_5D;
						IL_85:
						i++;
						continue;
						IL_5D:
						if (format[i] == 'x' || format[i] == 's' || char.IsDigit(format[i]))
						{
							num++;
							goto IL_85;
						}
						goto IL_85;
					}
					this.Format = format;
					this.NumberOfDigits = num;
				}

				internal string Format { get; private set; }

				internal int NumberOfDigits { get; private set; }

				internal bool TryFormat(string argValue, out string result)
				{
					result = null;
					if (!string.IsNullOrEmpty(argValue))
					{
						StringBuilder stringBuilder = new StringBuilder(this.Format.Length);
						if (this.NumberOfDigits == argValue.Length)
						{
							int i = 0;
							int num = 0;
							while (i < this.Format.Length)
							{
								char c = this.Format[i];
								if (c == 's')
								{
									num++;
								}
								else if (c == 'x')
								{
									stringBuilder.Append(argValue[num]);
									num++;
								}
								else if (c == 'i')
								{
									i++;
									while (this.Format[i] != 'i')
									{
										stringBuilder.Append(this.Format[i]);
										i++;
									}
								}
								else if (char.IsDigit(c))
								{
									if (c != argValue[num])
									{
										return false;
									}
									stringBuilder.Append(argValue[num]);
									num++;
								}
								else
								{
									stringBuilder.Append(this.Format[i]);
								}
								i++;
							}
							result = stringBuilder.ToString();
							return true;
						}
					}
					return false;
				}
			}

			protected class GenericE164Formatter : PhoneNumber.PhoneNumberFormatter
			{
				internal GenericE164Formatter()
				{
					this.countryPatterns = new Dictionary<string, PhoneNumber.PhoneNumberFormatter.PatternCollection>(50);
					this.countryPatterns.Add("1", new PhoneNumber.PhoneNumberFormatter.PatternCollection(new PhoneNumber.PhoneNumberFormatter.FormatterPattern[]
					{
						new PhoneNumber.PhoneNumberFormatter.FormatterPattern("s(xxx) xxx-xxxx")
					}, new PhoneNumber.PhoneNumberFormatter.FormatterPattern[]
					{
						new PhoneNumber.PhoneNumberFormatter.FormatterPattern("+x (xxx) xxx-xxxx")
					}));
				}

				protected override string Format(PhoneNumber phoneNumber)
				{
					string countryCode = this.GetCountryCode(phoneNumber);
					if (countryCode != null)
					{
						PhoneNumber.PhoneNumberFormatter.PatternCollection patternCollection = null;
						string result = null;
						if (this.countryPatterns.TryGetValue(countryCode, out patternCollection))
						{
							PhoneNumber.PhoneNumberFormatter.FormatterPattern[] array;
							if (phoneNumber.Kind == PhoneNumberKind.National)
							{
								array = patternCollection.National;
							}
							else
							{
								array = patternCollection.International;
							}
							if (array != null)
							{
								foreach (PhoneNumber.PhoneNumberFormatter.FormatterPattern formatterPattern in array)
								{
									if (formatterPattern.TryFormat(phoneNumber.Number, out result))
									{
										return result;
									}
								}
								PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber.Number);
								CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data, "Failed to format number _PhoneNumber ", new object[0]);
							}
						}
					}
					return null;
				}

				private string GetCountryCode(PhoneNumber number)
				{
					E164Number e164Number;
					if (E164Number.TryParseWithoutFormulating(number.ToDial, out e164Number) && e164Number.CountryCode != null)
					{
						return e164Number.CountryCode;
					}
					PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, number.ToDial);
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data, "Unable to parse E164 number _PhoneNumber to retrieve the country code. ", new object[0]);
					return null;
				}

				private Dictionary<string, PhoneNumber.PhoneNumberFormatter.PatternCollection> countryPatterns;
			}

			protected class DefaultFormatter : PhoneNumber.PhoneNumberFormatter
			{
				protected override string Format(PhoneNumber phoneNumber)
				{
					return phoneNumber.ToDial;
				}
			}

			protected class PatternCollection
			{
				internal PatternCollection(PhoneNumber.PhoneNumberFormatter.FormatterPattern[] national, PhoneNumber.PhoneNumberFormatter.FormatterPattern[] international)
				{
					this.National = national;
					this.International = international;
				}

				internal PhoneNumber.PhoneNumberFormatter.FormatterPattern[] National { get; private set; }

				internal PhoneNumber.PhoneNumberFormatter.FormatterPattern[] International { get; private set; }
			}
		}
	}
}
