using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class DialPermissions
	{
		public static bool Check(PhoneNumber phoneNumber, DialPermissionWrapper dialData, UMDialPlan originatingDialPlan, UMDialPlan targetDialPlan, out PhoneNumber numberToDial)
		{
			PhoneNumber phoneNumber2 = null;
			bool result = false;
			DialGroups dialGroups = new DialGroups();
			MultiValuedProperty<string> multiValuedProperty = null;
			if (originatingDialPlan.URIType == UMUriType.SipName && phoneNumber.UriType == UMUriType.SipName)
			{
				numberToDial = phoneNumber;
				return true;
			}
			if (phoneNumber.Kind == PhoneNumberKind.Extension)
			{
				result = DialPermissions.CheckExtension(phoneNumber, dialData, originatingDialPlan, targetDialPlan, out phoneNumber2);
				numberToDial = phoneNumber2;
				return result;
			}
			switch (phoneNumber.Kind)
			{
			case PhoneNumberKind.National:
				multiValuedProperty = dialData.AllowedInCountryGroups;
				dialGroups.Add(originatingDialPlan.ConfiguredInCountryOrRegionGroups);
				break;
			case PhoneNumberKind.International:
				multiValuedProperty = dialData.AllowedInternationalGroups;
				dialGroups.Add(originatingDialPlan.ConfiguredInternationalGroups);
				break;
			}
			if (multiValuedProperty == null || multiValuedProperty.Count == 0)
			{
				PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber);
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, data, "Access Check Failed: For PhoneNumber _PhoneNumber, did not find any allowed groups.", new object[0]);
				numberToDial = null;
				result = false;
				return result;
			}
			List<DialGroupEntry> list = new List<DialGroupEntry>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("\n");
			bool flag = false;
			foreach (string text in multiValuedProperty)
			{
				List<DialGroupEntry> collection = null;
				if (!dialGroups.TryGetValue(text, out collection))
				{
					stringBuilder.AppendFormat("'{0}'\n", text);
					flag = true;
				}
				else
				{
					list.AddRange(collection);
				}
			}
			if (flag)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MissingDialGroupEntry, null, new object[]
				{
					CommonUtil.ToEventLogString(stringBuilder.ToString()),
					originatingDialPlan.Name
				});
			}
			list.Sort(DialPermissions.DialGroupEntryComparer.StaticInstance);
			result = false;
			phoneNumber2 = null;
			int num = -1;
			PhoneNumber phoneNumber3 = null;
			int num2 = -1;
			foreach (DialGroupEntry entry in list)
			{
				num2 = 0;
				bool flag2 = DialPermissions.CheckAgainstDialGroupEntry(phoneNumber, entry, out phoneNumber2, ref num2);
				if (flag2)
				{
					result = true;
					if (num2 > num)
					{
						num = num2;
						phoneNumber3 = phoneNumber2;
					}
				}
			}
			numberToDial = phoneNumber3;
			return result;
		}

		public static bool CheckAgainstDialGroupEntry(PhoneNumber phoneNumber, DialGroupEntry entry, out PhoneNumber numberToDial, ref int matchLength)
		{
			PhoneNumber phoneNumber2 = null;
			bool result = false;
			numberToDial = null;
			matchLength = 0;
			PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber);
			CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, data, "PhoneNumber: _PhoneNumber DialGroupEntry: {0}.", new object[]
			{
				entry
			});
			string numberMask = entry.NumberMask;
			string text = entry.DialedNumber;
			int num = numberMask.IndexOf("*", StringComparison.InvariantCulture);
			int num2 = numberMask.IndexOf("x", StringComparison.InvariantCultureIgnoreCase);
			data = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber.Number);
			if (phoneNumber.Number.Length < numberMask.Length)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, data, "Number: _PhoneNumber does not match length of numbermask: {0}.", new object[]
				{
					numberMask
				});
				matchLength = 0;
				return false;
			}
			if (num != -1)
			{
				if (string.Compare(numberMask, "*", true, CultureInfo.InvariantCulture) == 0)
				{
					result = true;
					matchLength = phoneNumber.Number.Length;
					if (string.Compare(text, "*", true, CultureInfo.InvariantCulture) == 0)
					{
						phoneNumber2 = phoneNumber;
					}
					else
					{
						phoneNumber2 = DialPermissions.CreateOutputNumber(text, phoneNumber);
					}
				}
				else if (string.Compare(phoneNumber.Number, 0, numberMask, 0, num, StringComparison.InvariantCulture) == 0)
				{
					string newValue = null;
					if (phoneNumber.Number.Length >= num)
					{
						newValue = phoneNumber.Number.Substring(num);
					}
					if (text.IndexOf("*", StringComparison.InvariantCulture) != -1)
					{
						text = text.Replace("*", newValue);
					}
					phoneNumber2 = DialPermissions.CreateOutputNumber(text, phoneNumber);
					result = true;
					matchLength = num;
				}
			}
			else if (num2 != -1)
			{
				if (phoneNumber.Number.Length != numberMask.Length)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, data, "PhoneNumber = _PhoneNumber Mask.Length = {0}.", new object[]
					{
						numberMask
					});
					numberToDial = null;
					matchLength = 0;
					return false;
				}
				if (string.Compare(phoneNumber.Number, 0, numberMask, 0, num2, StringComparison.InvariantCulture) == 0)
				{
					int num3 = text.IndexOf("x", StringComparison.InvariantCultureIgnoreCase);
					if (num3 != -1)
					{
						string str = phoneNumber.Number.Substring(num2);
						string str2 = text.Substring(0, num3);
						phoneNumber2 = DialPermissions.CreateOutputNumber(str2 + str, phoneNumber);
					}
					else if (string.Compare(text, "*", true, CultureInfo.InvariantCulture) == 0)
					{
						phoneNumber2 = phoneNumber;
					}
					else
					{
						phoneNumber2 = DialPermissions.CreateOutputNumber(text, phoneNumber);
					}
					result = true;
					matchLength = num2;
				}
			}
			else if (phoneNumber.Number.Length == numberMask.Length && string.Compare(phoneNumber.Number, 0, numberMask, 0, numberMask.Length, true, CultureInfo.InvariantCulture) == 0)
			{
				phoneNumber2 = DialPermissions.CreateOutputNumber(text, phoneNumber);
				matchLength = numberMask.Length;
				result = true;
			}
			numberToDial = phoneNumber2;
			return result;
		}

		public static bool CheckExtension(PhoneNumber phoneNumber, DialPermissionWrapper dialData, UMDialPlan originatingDialPlan, UMDialPlan targetDialPlan, out PhoneNumber numberToDial)
		{
			PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber);
			CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, data, "CheckExtension Number=_PhoneNumber, Originating Dialplan = {0}, Target DialPlan = {1}.", new object[]
			{
				originatingDialPlan.Name,
				(targetDialPlan != null) ? targetDialPlan.Name : "<null>"
			});
			CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "CheckExtension using DialingPolicy, Type={0}, AllowSubscribers={1}, AllowExtensions={2}.", new object[]
			{
				dialData.Category,
				dialData.DialPlanSubscribersAllowed,
				dialData.ExtensionLengthNumbersAllowed
			});
			PhoneNumber phoneNumber2 = null;
			bool flag = false;
			if (targetDialPlan != null && Utils.IsIdenticalDialPlan(originatingDialPlan, targetDialPlan))
			{
				if (dialData.DialPlanSubscribersAllowed)
				{
					phoneNumber2 = phoneNumber;
					flag = true;
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "AllowDialPlanSubscribers = false for dialing policy: {0} {1}", new object[]
					{
						dialData.Category,
						dialData.Identity
					});
				}
			}
			else if (dialData.ExtensionLengthNumbersAllowed && phoneNumber.Number.Length == originatingDialPlan.NumberOfDigitsInExtension)
			{
				phoneNumber2 = phoneNumber;
				flag = true;
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "AllowExtensions = {0}, ExtensionLength={1} PhoneNumber.Lenght={2} dialpolicy: {3} {4}.", new object[]
				{
					originatingDialPlan.AllowExtensions,
					originatingDialPlan.NumberOfDigitsInExtension,
					phoneNumber.Number.Length,
					dialData.Category,
					dialData.Identity
				});
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, data, "CheckExtension(_PhoneNumber,{0},{1}) returning result={2} number={3}.", new object[]
			{
				originatingDialPlan.Name,
				(targetDialPlan != null) ? targetDialPlan.Name : "<null>",
				flag,
				(phoneNumber2 == null) ? "<null>" : phoneNumber2.ToString()
			});
			numberToDial = phoneNumber2;
			return flag;
		}

		public static bool GetBestOfficeNumber(IADRecipient recipient, UMDialPlan originatingDialPlan, out PhoneNumber phoneNumber)
		{
			phoneNumber = null;
			string text = null;
			IADOrgPerson iadorgPerson = recipient as IADOrgPerson;
			if (iadorgPerson == null)
			{
				return false;
			}
			if (originatingDialPlan.URIType == UMUriType.TelExtn)
			{
				if (DialPermissions.TryGetDialplanExtension(recipient, originatingDialPlan, out text))
				{
					phoneNumber = PhoneNumber.CreateExtension(text);
				}
				else
				{
					text = Utils.TrimSpaces(iadorgPerson.Phone);
					if (!string.IsNullOrEmpty(text))
					{
						IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(iadorgPerson);
						UMDialPlan dialPlanFromRecipient = iadsystemConfigurationLookup.GetDialPlanFromRecipient(iadorgPerson);
						if (dialPlanFromRecipient != null && dialPlanFromRecipient.Guid.Equals(originatingDialPlan.Guid))
						{
							int numberOfDigitsInExtension = dialPlanFromRecipient.NumberOfDigitsInExtension;
							if (text.Length >= numberOfDigitsInExtension)
							{
								phoneNumber = PhoneNumber.CreateExtension(text.Substring(text.Length - numberOfDigitsInExtension));
							}
						}
					}
				}
			}
			else if (Utils.FindSipNameOrE164Extension(originatingDialPlan, recipient, originatingDialPlan.URIType, out text))
			{
				PhoneNumber.TryParse(originatingDialPlan, text, out phoneNumber);
			}
			if (phoneNumber == null)
			{
				text = Utils.TrimSpaces(iadorgPerson.Phone);
				if (!string.IsNullOrEmpty(text))
				{
					if (originatingDialPlan.URIType == UMUriType.TelExtn && text.Length == originatingDialPlan.NumberOfDigitsInExtension)
					{
						phoneNumber = PhoneNumber.CreateExtension(text);
					}
					else
					{
						PhoneNumber.TryParse(originatingDialPlan, text, out phoneNumber);
					}
				}
			}
			return phoneNumber != null;
		}

		public static PhoneNumber Canonicalize(PhoneNumber phoneNumber, UMDialPlan originatingDialPlan, UMDialPlan targetDialPlan)
		{
			return DialPermissions.Canonicalize(phoneNumber, originatingDialPlan, null, targetDialPlan);
		}

		public static PhoneNumber Canonicalize(PhoneNumber phoneNumber, UMDialPlan originatingDialPlan, IADRecipient target, UMDialPlan targetDialPlan)
		{
			OutdialingDiagnostics outdialingDiagnostics = new OutdialingDiagnostics();
			PhoneNumber phoneNumber2 = null;
			PhoneNumber phoneNumber3 = null;
			PhoneNumber result;
			try
			{
				if (PhoneNumber.IsNullOrEmpty(phoneNumber))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "Canonicalize: phoneNumber is empty, returning null.", new object[0]);
					result = null;
				}
				else
				{
					phoneNumber2 = phoneNumber.Clone(originatingDialPlan);
					if (originatingDialPlan.URIType == UMUriType.SipName || originatingDialPlan.URIType == UMUriType.E164)
					{
						phoneNumber3 = phoneNumber2;
						CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "Originating dialplan is {0} and parsed number is {1}. Canonicalization ends here.", new object[]
						{
							originatingDialPlan.URIType,
							phoneNumber3.UriType
						});
						result = phoneNumber3;
					}
					else if (targetDialPlan == null || originatingDialPlan.Guid.Equals(targetDialPlan.Guid))
					{
						phoneNumber3 = DialPermissions.CanonicalizeNoDialplan(phoneNumber2, originatingDialPlan, target, outdialingDiagnostics);
						result = phoneNumber3;
					}
					else if (string.IsNullOrEmpty(phoneNumber2.Number))
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "Canonicalization: Parsed number is empty. Returning null for canonicalized number.", new object[0]);
						result = null;
					}
					else
					{
						if (string.IsNullOrEmpty(originatingDialPlan.CountryOrRegionCode))
						{
							outdialingDiagnostics.AddPropertyNotSetDetail(originatingDialPlan, OutdialingDiagnostics.DialPlanProperty.CountryOrRegionCode);
							CallIdTracer.TraceWarning(ExTraceGlobals.OutdialingTracer, null, "Originating dialplan {0} does not have CountryOrRegionCode set", new object[]
							{
								originatingDialPlan.Name
							});
						}
						if (string.IsNullOrEmpty(targetDialPlan.CountryOrRegionCode))
						{
							outdialingDiagnostics.AddPropertyNotSetDetail(targetDialPlan, OutdialingDiagnostics.DialPlanProperty.CountryOrRegionCode);
							CallIdTracer.TraceWarning(ExTraceGlobals.OutdialingTracer, null, "Target dialplan {0} does not have CountryOrRegionCode set", new object[]
							{
								targetDialPlan.Name
							});
						}
						if (string.IsNullOrEmpty(originatingDialPlan.CountryOrRegionCode) || string.IsNullOrEmpty(targetDialPlan.CountryOrRegionCode) || string.Compare(originatingDialPlan.CountryOrRegionCode, targetDialPlan.CountryOrRegionCode, true, CultureInfo.InvariantCulture) == 0)
						{
							NumberFormat numberFormat = targetDialPlan.InCountryOrRegionNumberFormat;
							string outsideLineAccessCode = originatingDialPlan.OutsideLineAccessCode;
							if (string.IsNullOrEmpty(outsideLineAccessCode))
							{
								outdialingDiagnostics.AddPropertyNotSetDetail(originatingDialPlan, OutdialingDiagnostics.DialPlanProperty.OutsideLineAccessCode);
								CallIdTracer.TraceWarning(ExTraceGlobals.OutdialingTracer, null, "WARNING: OutsideLineAccessCode was not set on dialplan: {0}.", new object[]
								{
									originatingDialPlan.Name
								});
							}
							if (numberFormat == null)
							{
								CallIdTracer.TraceWarning(ExTraceGlobals.OutdialingTracer, null, "WARNING: InCountryOrRegionNumberFormat was not set on dialplan: {0}. This will be treated as nodialplan info available.", new object[]
								{
									originatingDialPlan.Name
								});
								phoneNumber3 = DialPermissions.CanonicalizeNoDialplan(phoneNumber2, originatingDialPlan, target, outdialingDiagnostics);
								outdialingDiagnostics.AddSkipDialPlanDetail();
								outdialingDiagnostics.AddPropertyNotSetDetail(targetDialPlan, OutdialingDiagnostics.DialPlanProperty.InCountryOrRegionNumberFormat);
								result = phoneNumber3;
							}
							else
							{
								string str = null;
								if (!numberFormat.TryMapNumber(phoneNumber2.Number, out str))
								{
									PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber2.Number);
									CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, data, "Unable to map phone number (_PhoneNumber) with NationalNumberFormat {0}.", new object[]
									{
										numberFormat.ToString()
									});
									result = null;
								}
								else
								{
									phoneNumber3 = PhoneNumber.CreateNational(outsideLineAccessCode + str);
									PIIMessage piimessage = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber);
									PIIMessage piimessage2 = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber3);
									PIIMessage[] data2 = new PIIMessage[]
									{
										piimessage,
										piimessage2
									};
									CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, data2, "Canonicalize(_PhoneNumber(1)) returning {_PhoneNumber(2)}.", new object[0]);
									result = phoneNumber3;
								}
							}
						}
						else
						{
							NumberFormat numberFormat = targetDialPlan.InternationalNumberFormat;
							string outsideLineAccessCode2 = originatingDialPlan.OutsideLineAccessCode;
							string internationalAccessCode = originatingDialPlan.InternationalAccessCode;
							if (string.IsNullOrEmpty(outsideLineAccessCode2))
							{
								outdialingDiagnostics.AddPropertyNotSetDetail(originatingDialPlan, OutdialingDiagnostics.DialPlanProperty.OutsideLineAccessCode);
								CallIdTracer.TraceWarning(ExTraceGlobals.OutdialingTracer, null, "WARNING: OutsideLineAccessCode is not set on dialplan: {0}.", new object[]
								{
									originatingDialPlan.Name
								});
							}
							if (string.IsNullOrEmpty(internationalAccessCode))
							{
								outdialingDiagnostics.AddPropertyNotSetDetail(originatingDialPlan, OutdialingDiagnostics.DialPlanProperty.InternationalAccessCode);
								CallIdTracer.TraceWarning(ExTraceGlobals.OutdialingTracer, null, "WARNING: InternationalAccessCode is not set on dialplan: {0}.", new object[]
								{
									originatingDialPlan.Name
								});
							}
							if (numberFormat == null)
							{
								outdialingDiagnostics.AddSkipDialPlanDetail();
								outdialingDiagnostics.AddPropertyNotSetDetail(targetDialPlan, OutdialingDiagnostics.DialPlanProperty.InternationalNumberFormat);
								CallIdTracer.TraceWarning(ExTraceGlobals.OutdialingTracer, null, "WARNING: InternationalNumberFormat was not set on dialplan: {0} This will be treated as nodialplan info available.", new object[]
								{
									targetDialPlan.Name
								});
								phoneNumber3 = DialPermissions.CanonicalizeNoDialplan(phoneNumber2, originatingDialPlan, target, outdialingDiagnostics);
								result = phoneNumber3;
							}
							else
							{
								string str2 = outsideLineAccessCode2 + internationalAccessCode;
								string str3 = null;
								if (!numberFormat.TryMapNumber(phoneNumber2.Number, out str3))
								{
									PIIMessage data3 = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber2.Number);
									CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, data3, "Unable to map phone number (_PhoneNumber) with InternationalNumberFormat {0}.", new object[]
									{
										phoneNumber2.Number,
										numberFormat.ToString()
									});
									result = null;
								}
								else
								{
									phoneNumber3 = PhoneNumber.CreateInternational(str2 + str3);
									PIIMessage piimessage3 = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber);
									PIIMessage piimessage4 = PIIMessage.Create(PIIType._PhoneNumber, phoneNumber3);
									PIIMessage[] data4 = new PIIMessage[]
									{
										piimessage3,
										piimessage4
									};
									CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, data4, "Canonicalize(_PhoneNumber(1)) returning _PhoneNumber(2).", new object[0]);
									result = phoneNumber3;
								}
							}
						}
					}
				}
			}
			finally
			{
				if (outdialingDiagnostics.HasEvents)
				{
					outdialingDiagnostics.LogOutdialingWarning(phoneNumber2, phoneNumber3);
				}
			}
			return result;
		}

		internal static bool Check(PhoneNumber phoneNumber, UMAutoAttendant autoAttendant, UMDialPlan originatingDialPlan, UMDialPlan targetDialPlan, out PhoneNumber numberToDial)
		{
			DialPermissionWrapper dialData = DialPermissionWrapper.CreateFromAutoAttendant(autoAttendant);
			return DialPermissions.Check(phoneNumber, dialData, originatingDialPlan, targetDialPlan, out numberToDial);
		}

		internal static bool Check(PhoneNumber phoneNumber, ADUser subscriber, UMDialPlan originatingDialPlan, UMDialPlan targetDialPlan, out PhoneNumber numberToDial)
		{
			DialPermissionWrapper dialData = DialPermissionWrapper.CreateFromRecipientPolicy(subscriber);
			return DialPermissions.Check(phoneNumber, dialData, originatingDialPlan, targetDialPlan, out numberToDial);
		}

		internal static bool Check(PhoneNumber phoneNumber, UMDialPlan originatingDialPlan, UMDialPlan targetDialPlan, out PhoneNumber numberToDial)
		{
			DialPermissionWrapper dialData = DialPermissionWrapper.CreateFromDialPlan(originatingDialPlan);
			return DialPermissions.Check(phoneNumber, dialData, originatingDialPlan, targetDialPlan, out numberToDial);
		}

		internal static bool TryGetDialplanExtension(IADRecipient recipient, UMDialPlan originatingDialPlan, out string extension)
		{
			extension = null;
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(recipient);
			UMDialPlan dialPlanFromRecipient = iadsystemConfigurationLookup.GetDialPlanFromRecipient(recipient);
			if (!string.IsNullOrEmpty(recipient.UMExtension) && dialPlanFromRecipient != null && Utils.IsIdenticalDialPlan(dialPlanFromRecipient, originatingDialPlan))
			{
				extension = recipient.UMExtension;
			}
			return extension != null;
		}

		private static PhoneNumber CanonicalizeNoDialplan(PhoneNumber parsedNumber, UMDialPlan originatingDialPlan, IADRecipient recipient, OutdialingDiagnostics diagnostics)
		{
			PIIMessage piimessage = PIIMessage.Create(PIIType._PhoneNumber, parsedNumber.Number);
			if (recipient != null)
			{
				PIIMessage piimessage2 = PIIMessage.Create(PIIType._UserDisplayName, recipient.DisplayName);
				PIIMessage[] data = new PIIMessage[]
				{
					piimessage,
					piimessage2
				};
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, data, "CanonicalizeNoDialplan():: Number=_PhoneNumber Recipient=_UserDisplayName.", new object[0]);
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, piimessage, "CanonicalizeNoDialplan():: Number=_PhoneNumber Recipient=<null>.", new object[0]);
			}
			if (parsedNumber.UriType == UMUriType.E164)
			{
				return DialPermissions.CanonicalizeNoDialplan_CanonicalNumber(parsedNumber, originatingDialPlan, recipient, diagnostics);
			}
			string countryOrRegionCode = originatingDialPlan.CountryOrRegionCode;
			string nationalNumberPrefix = originatingDialPlan.NationalNumberPrefix;
			CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "NonCanonical number with no dialplan info. CountryOrRegionCode=[{0}] NNP=[{1}] OutsideLineAccessCode=[{2}].", new object[]
			{
				countryOrRegionCode,
				nationalNumberPrefix,
				originatingDialPlan.OutsideLineAccessCode
			});
			CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, piimessage, "DialPlan.NumberOfDigitsInExtension={0} Phone=[_PhoneNumber,length={1}].", new object[]
			{
				originatingDialPlan.NumberOfDigitsInExtension,
				parsedNumber.Number.Length
			});
			if (parsedNumber.Number.Length == originatingDialPlan.NumberOfDigitsInExtension)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "Number looks like an extension in the originating dialplan. Returning the extension.", new object[0]);
				return PhoneNumber.CreateExtension(parsedNumber.Number);
			}
			if (string.IsNullOrEmpty(countryOrRegionCode))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "WARNING: CanonicalizeNoDialplan - CountryOrRegionCode was not set on dialplan: {0}.", new object[]
				{
					originatingDialPlan.Name
				});
				if (diagnostics != null)
				{
					diagnostics.AddPropertyNotSetDetail(originatingDialPlan, OutdialingDiagnostics.DialPlanProperty.CountryOrRegionCode);
				}
			}
			if (string.IsNullOrEmpty(originatingDialPlan.OutsideLineAccessCode))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "WARNING: CanonicalizeNoDialplan - OutsideLineAccessCode was not set on dialplan: {0}.", new object[]
				{
					originatingDialPlan.Name
				});
				if (diagnostics != null)
				{
					diagnostics.AddPropertyNotSetDetail(originatingDialPlan, OutdialingDiagnostics.DialPlanProperty.OutsideLineAccessCode);
				}
			}
			if (string.IsNullOrEmpty(nationalNumberPrefix))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "WARNING: CanonicalizeNoDialplan - NationalNumberPrefix was not set on dialplan: {0}.", new object[]
				{
					originatingDialPlan.Name
				});
				if (diagnostics != null)
				{
					diagnostics.AddPropertyNotSetDetail(originatingDialPlan, OutdialingDiagnostics.DialPlanProperty.NationalNumberPrefix);
				}
			}
			StringBuilder stringBuilder = new StringBuilder(originatingDialPlan.OutsideLineAccessCode);
			if (!string.IsNullOrEmpty(nationalNumberPrefix) && parsedNumber.Number.Length >= nationalNumberPrefix.Length && string.Compare(parsedNumber.Number, 0, nationalNumberPrefix, 0, nationalNumberPrefix.Length, StringComparison.InvariantCulture) != 0)
			{
				stringBuilder.Append(nationalNumberPrefix);
			}
			stringBuilder.Append(parsedNumber.Number);
			return PhoneNumber.CreateNational(stringBuilder.ToString());
		}

		private static PhoneNumber CanonicalizeNoDialplan_CanonicalNumber(PhoneNumber parsedNumber, UMDialPlan originatingDialPlan, IADRecipient recipient, OutdialingDiagnostics diagnostics)
		{
			PIIMessage piimessage = PIIMessage.Create(PIIType._PhoneNumber, parsedNumber.Number);
			PIIMessage piimessage2 = PIIMessage.Create(PIIType._UserDisplayName, (recipient != null) ? recipient.DisplayName : "<null>");
			PIIMessage[] data = new PIIMessage[]
			{
				piimessage,
				piimessage2
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, data, "CanonicalizeNoDialplan_CanonicalNumber():: Number=_PhoneNumber Recipient=_DisplayName", new object[0]);
			if (string.IsNullOrEmpty(parsedNumber.Number))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "Canonicalization: CanonicalizeNoDialplan - Parsed number is empty. Returning null for canonicalized number.", new object[0]);
				return null;
			}
			string nationalNumberPrefix = originatingDialPlan.NationalNumberPrefix;
			string countryOrRegionCode = originatingDialPlan.CountryOrRegionCode;
			CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "Canonical number with no dialplan info. CountryOrRegionCode=[{0}] NNP=[{1}] OutsideLineAccessCode=[{2}].", new object[]
			{
				countryOrRegionCode,
				nationalNumberPrefix,
				originatingDialPlan.OutsideLineAccessCode
			});
			if (string.IsNullOrEmpty(countryOrRegionCode))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "WARNING: CanonicalizeNoDialplan - CountryOrRegionCode was not set on dialplan: {0}.", new object[]
				{
					originatingDialPlan.Name
				});
				if (diagnostics != null)
				{
					diagnostics.AddPropertyNotSetDetail(originatingDialPlan, OutdialingDiagnostics.DialPlanProperty.CountryOrRegionCode);
				}
			}
			if (string.IsNullOrEmpty(originatingDialPlan.OutsideLineAccessCode))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "WARNING: CanonicalizeNoDialplan - OutsideLineAccessCode was not set on dialplan: {0}.", new object[]
				{
					originatingDialPlan.Name
				});
				if (diagnostics != null)
				{
					diagnostics.AddPropertyNotSetDetail(originatingDialPlan, OutdialingDiagnostics.DialPlanProperty.OutsideLineAccessCode);
				}
			}
			string number;
			if (string.Compare(parsedNumber.Number, 0, countryOrRegionCode, 0, countryOrRegionCode.Length, true, CultureInfo.InvariantCulture) == 0)
			{
				string text = parsedNumber.Number.Substring(countryOrRegionCode.Length);
				if (string.IsNullOrEmpty(text))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, piimessage, "WARNING: CanonicalizeNoDialplan - The number (_PhoneNumber) is not valid.", new object[0]);
					if (diagnostics != null)
					{
						diagnostics.AddInvalidRecipientPhoneLength(recipient, originatingDialPlan.Name);
					}
				}
				if (string.IsNullOrEmpty(nationalNumberPrefix))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "WARNING: CanonicalizeNoDialplan - NationalNumberPrefix was not set on dialplan: {0}.", new object[]
					{
						originatingDialPlan.Name
					});
					if (diagnostics != null)
					{
						diagnostics.AddPropertyNotSetDetail(originatingDialPlan, OutdialingDiagnostics.DialPlanProperty.NationalNumberPrefix);
					}
				}
				else if (string.Compare(text, 0, nationalNumberPrefix, 0, nationalNumberPrefix.Length, true, CultureInfo.InvariantCulture) == 0)
				{
					text = text.Substring(nationalNumberPrefix.Length);
					text = text.Trim();
				}
				number = originatingDialPlan.OutsideLineAccessCode + originatingDialPlan.NationalNumberPrefix + text;
				return PhoneNumber.CreateNational(number);
			}
			if (string.IsNullOrEmpty(originatingDialPlan.InternationalAccessCode))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.OutdialingTracer, null, "WARNING: CanonicalizeNoDialplan - InternationalAccessCode was not set on dialplan: {0}.", new object[]
				{
					originatingDialPlan.Name
				});
				if (diagnostics != null)
				{
					diagnostics.AddPropertyNotSetDetail(originatingDialPlan, OutdialingDiagnostics.DialPlanProperty.InternationalAccessCode);
				}
			}
			number = originatingDialPlan.OutsideLineAccessCode + originatingDialPlan.InternationalAccessCode + parsedNumber.Number;
			return PhoneNumber.CreateInternational(number);
		}

		private static PhoneNumber CreateOutputNumber(string dialedNumber, PhoneNumber input)
		{
			UMUriType uriType = input.UriType;
			if (dialedNumber.StartsWith("+", StringComparison.Ordinal))
			{
				uriType = UMUriType.E164;
				dialedNumber = dialedNumber.Remove(0, "+".Length);
			}
			return new PhoneNumber(dialedNumber, input.Kind, uriType);
		}

		private class DialGroupEntryComparer : IComparer<DialGroupEntry>
		{
			public int Compare(DialGroupEntry ol, DialGroupEntry or)
			{
				if (ol == null)
				{
					if (or == null)
					{
						return 0;
					}
					return -1;
				}
				else
				{
					if (ol != null && or == null)
					{
						return 1;
					}
					int num = ol.NumberMask.IndexOf('x');
					int num2 = or.NumberMask.IndexOf('x');
					num = ((num == -1) ? ol.NumberMask.Length : num);
					num2 = ((num2 == -1) ? or.NumberMask.Length : num2);
					return num - num2;
				}
			}

			internal static readonly DialPermissions.DialGroupEntryComparer StaticInstance = new DialPermissions.DialGroupEntryComparer();
		}
	}
}
