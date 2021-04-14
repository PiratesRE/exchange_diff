using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class CASMailboxHelper
	{
		private static string GetSettingsString(IPropertyBag propertyBag, string protocolName, int position, string defaultValue)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.ProtocolSettings];
			foreach (string text in multiValuedProperty)
			{
				if (text.StartsWith(protocolName, StringComparison.OrdinalIgnoreCase))
				{
					string[] array = text.Split(new char[]
					{
						'§'
					});
					if (array.Length <= position || string.IsNullOrEmpty(array[position]))
					{
						return defaultValue;
					}
					return array[position];
				}
			}
			return defaultValue;
		}

		private static string GetSettingsString(IPropertyBag propertyBag, string protocolName, int position, ADPropertyDefinition propertyDefinition)
		{
			return CASMailboxHelper.GetSettingsString(propertyBag, protocolName, position, (string)propertyDefinition.DefaultValue);
		}

		private static bool GetSettingsBool(IPropertyBag propertyBag, string protocolName, int position, ADPropertyDefinition propertyDefinition)
		{
			string defaultValue = ((bool)propertyDefinition.DefaultValue) ? "1" : "0";
			string settingsString = CASMailboxHelper.GetSettingsString(propertyBag, protocolName, position, defaultValue);
			return string.Compare(settingsString, "1", StringComparison.OrdinalIgnoreCase) == 0 || (string.Compare(settingsString, "0", StringComparison.OrdinalIgnoreCase) != 0 && (bool)propertyDefinition.DefaultValue);
		}

		private static int GetSettingsInt(IPropertyBag propertyBag, string protocolName, int position, ADPropertyDefinition propertyDefinition)
		{
			int result = (propertyDefinition.DefaultValue != null) ? ((int)propertyDefinition.DefaultValue) : 0;
			string settingsString = CASMailboxHelper.GetSettingsString(propertyBag, protocolName, position, result.ToString());
			int result2;
			if (!int.TryParse(settingsString, out result2))
			{
				return result;
			}
			return result2;
		}

		private static MimeTextFormat GetSettingsMimeTextFormat(IPropertyBag propertyBag, string protocolName, int position, ADPropertyDefinition propertyDefinition)
		{
			return (MimeTextFormat)CASMailboxHelper.GetSettingsInt(propertyBag, protocolName, position, propertyDefinition);
		}

		private static bool? EwsGetTupleBool(IPropertyBag propertyBag, string wellKnownApplicationName)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[CASMailboxSchema.EwsWellKnownApplicationAccessPolicies];
			foreach (string text in multiValuedProperty)
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					ExTraceGlobals.GetCASMailboxTracer.TraceDebug<string>(0L, "Get-CASMailbox: policy must be in Allow:Application/Block:Application form. Skipping policy '{0}'.", text);
				}
				else if (string.Compare(array[1], wellKnownApplicationName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					if (string.Compare(array[0], "Allow", StringComparison.OrdinalIgnoreCase) == 0)
					{
						return new bool?(true);
					}
					if (string.Compare(array[0], "Block", StringComparison.OrdinalIgnoreCase) == 0)
					{
						return new bool?(false);
					}
					ExTraceGlobals.GetCASMailboxTracer.TraceDebug<string>(0L, "Get-CASMailbox: policy must be in Allow:Application/Block:Application form. Skipping policy '{0}'.", text);
				}
			}
			return null;
		}

		private static void SetSettingsString(IPropertyBag propertyBag, string protocolName, int position, string value, int totalNumberOfFields)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADRecipientSchema.ProtocolSettings];
			string[] array = null;
			foreach (string text in multiValuedProperty)
			{
				if (text.StartsWith(protocolName, StringComparison.OrdinalIgnoreCase))
				{
					multiValuedProperty.Remove(text);
					array = text.Split(new char[]
					{
						'§'
					});
					break;
				}
			}
			for (int i = 0; i < multiValuedProperty.Count; i++)
			{
				if (multiValuedProperty[i].StartsWith(protocolName, StringComparison.OrdinalIgnoreCase))
				{
					multiValuedProperty.RemoveAt(i);
					i--;
				}
			}
			if (array == null)
			{
				array = new string[totalNumberOfFields];
				array[0] = protocolName;
			}
			else if (array.Length < totalNumberOfFields)
			{
				Array.Resize<string>(ref array, totalNumberOfFields);
			}
			array[position] = value;
			StringBuilder stringBuilder = new StringBuilder();
			for (int j = 0; j < array.Length; j++)
			{
				if (j > 0)
				{
					stringBuilder.Append('§');
				}
				if (!string.IsNullOrEmpty(array[j]))
				{
					stringBuilder.Append(array[j]);
				}
			}
			multiValuedProperty.Add(stringBuilder.ToString());
		}

		private static void SetSettingsBool(IPropertyBag propertyBag, string protocolName, int position, bool value, int totalNumberOfFields)
		{
			string value2 = value ? "1" : "0";
			CASMailboxHelper.SetSettingsString(propertyBag, protocolName, position, value2, totalNumberOfFields);
		}

		private static void SetSettingsInt(IPropertyBag propertyBag, string protocolName, int position, int intValue, int totalNumberOfFields)
		{
			string value = intValue.ToString(CultureInfo.InvariantCulture);
			CASMailboxHelper.SetSettingsString(propertyBag, protocolName, position, value, totalNumberOfFields);
		}

		private static void SetSettingsMimeTextFormat(IPropertyBag propertyBag, string protocolName, int position, MimeTextFormat value, int totalNumberOfFields)
		{
			CASMailboxHelper.SetSettingsInt(propertyBag, protocolName, position, (int)value, totalNumberOfFields);
		}

		private static void EwsSetTupleBool(IPropertyBag propertyBag, string wellKnownApplicationName, bool? allow)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[CASMailboxSchema.EwsWellKnownApplicationAccessPolicies];
			foreach (string text in multiValuedProperty)
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (string.Compare(array[1], wellKnownApplicationName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					multiValuedProperty.Remove(text);
					break;
				}
			}
			if (allow != null)
			{
				string arg = (allow == true) ? "Allow" : "Block";
				multiValuedProperty.Add(string.Format("{0}:{1}", arg, wellKnownApplicationName));
			}
		}

		internal static GetterDelegate RemotePowerShellEnabledGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsBool(propertyBag, "RemotePowerShell", 1, ADRecipientSchema.RemotePowerShellEnabled);
		}

		internal static SetterDelegate RemotePowerShellEnabledSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				value = ((value == null) ? false : value);
				CASMailboxHelper.SetSettingsBool(propertyBag, "RemotePowerShell", 1, (bool)value, 2);
			};
		}

		internal static GetterDelegate ECPEnabledGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsBool(propertyBag, "ECP", 1, ADRecipientSchema.ECPEnabled);
		}

		internal static SetterDelegate ECPEnabledSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				CASMailboxHelper.SetSettingsBool(propertyBag, "ECP", 1, (bool)value, 2);
			};
		}

		internal static GetterDelegate PopEnabledGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsBool(propertyBag, "POP3", 1, ADRecipientSchema.PopEnabled);
		}

		internal static SetterDelegate PopEnabledSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				CASMailboxHelper.SetSettingsBool(propertyBag, "POP3", 1, (bool)value, 14);
			};
		}

		internal static GetterDelegate PopUseProtocolDefaultsGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsBool(propertyBag, "POP3", 2, ADRecipientSchema.PopUseProtocolDefaults);
		}

		internal static SetterDelegate PopUseProtocolDefaultsSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				CASMailboxHelper.SetSettingsBool(propertyBag, "POP3", 2, (bool)value, 14);
			};
		}

		internal static GetterDelegate PopMessagesRetrievalMimeFormatGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsMimeTextFormat(propertyBag, "POP3", 9, ADRecipientSchema.PopMessagesRetrievalMimeFormat);
		}

		internal static SetterDelegate PopMessagesRetrievalMimeFormatSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				CASMailboxHelper.SetSettingsMimeTextFormat(propertyBag, "POP3", 9, (MimeTextFormat)value, 14);
			};
		}

		internal static GetterDelegate PopEnableExactRFC822SizeGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsBool(propertyBag, "POP3", 10, ADRecipientSchema.PopEnableExactRFC822Size);
		}

		internal static SetterDelegate PopEnableExactRFC822SizeSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				CASMailboxHelper.SetSettingsBool(propertyBag, "POP3", 10, (bool)value, 14);
			};
		}

		internal static GetterDelegate PopProtocolLoggingEnabledGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsInt(propertyBag, "POP3", 11, ADRecipientSchema.PopProtocolLoggingEnabled);
		}

		internal static SetterDelegate PopProtocolLoggingEnabledSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				if (value == null)
				{
					CASMailboxHelper.SetSettingsString(propertyBag, "POP3", 11, string.Empty, 14);
					return;
				}
				CASMailboxHelper.SetSettingsInt(propertyBag, "POP3", 11, (int)value, 14);
			};
		}

		internal static GetterDelegate PopSuppressReadReceiptGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsBool(propertyBag, "POP3", 12, ADRecipientSchema.PopSuppressReadReceipt);
		}

		internal static SetterDelegate PopSuppressReadReceiptSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				if (value == null)
				{
					CASMailboxHelper.SetSettingsString(propertyBag, "POP3", 12, string.Empty, 14);
					return;
				}
				CASMailboxHelper.SetSettingsBool(propertyBag, "POP3", 12, (bool)value, 14);
			};
		}

		internal static GetterDelegate PopForceICalForCalendarRetrievalOptionGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsBool(propertyBag, "POP3", 13, ADRecipientSchema.PopForceICalForCalendarRetrievalOption);
		}

		internal static SetterDelegate PopForceICalForCalendarRetrievalOptionSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				if (value == null)
				{
					CASMailboxHelper.SetSettingsString(propertyBag, "POP3", 13, string.Empty, 14);
					return;
				}
				CASMailboxHelper.SetSettingsBool(propertyBag, "POP3", 13, (bool)value, 14);
			};
		}

		internal static GetterDelegate ImapEnabledGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsBool(propertyBag, "IMAP4", 1, ADRecipientSchema.ImapEnabled);
		}

		internal static SetterDelegate ImapEnabledSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				CASMailboxHelper.SetSettingsBool(propertyBag, "IMAP4", 1, (bool)value, 14);
			};
		}

		internal static GetterDelegate ImapUseProtocolDefaultsGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsBool(propertyBag, "IMAP4", 2, ADRecipientSchema.ImapUseProtocolDefaults);
		}

		internal static SetterDelegate ImapUseProtocolDefaultsSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				CASMailboxHelper.SetSettingsBool(propertyBag, "IMAP4", 2, (bool)value, 14);
			};
		}

		internal static GetterDelegate ImapMessagesRetrievalMimeFormatGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsMimeTextFormat(propertyBag, "IMAP4", 9, ADRecipientSchema.ImapMessagesRetrievalMimeFormat);
		}

		internal static SetterDelegate ImapMessagesRetrievalMimeFormatSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				CASMailboxHelper.SetSettingsMimeTextFormat(propertyBag, "IMAP4", 9, (MimeTextFormat)value, 14);
			};
		}

		internal static GetterDelegate ImapEnableExactRFC822SizeGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsBool(propertyBag, "IMAP4", 10, ADRecipientSchema.ImapEnableExactRFC822Size);
		}

		internal static SetterDelegate ImapEnableExactRFC822SizeSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				CASMailboxHelper.SetSettingsBool(propertyBag, "IMAP4", 10, (bool)value, 14);
			};
		}

		internal static GetterDelegate ImapProtocolLoggingEnabledGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsInt(propertyBag, "IMAP4", 11, ADRecipientSchema.ImapProtocolLoggingEnabled);
		}

		internal static SetterDelegate ImapProtocolLoggingEnabledSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				if (value == null)
				{
					CASMailboxHelper.SetSettingsString(propertyBag, "IMAP4", 11, string.Empty, 14);
					return;
				}
				CASMailboxHelper.SetSettingsInt(propertyBag, "IMAP4", 11, (int)value, 14);
			};
		}

		internal static GetterDelegate ImapSuppressReadReceiptGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsBool(propertyBag, "IMAP4", 12, ADRecipientSchema.ImapSuppressReadReceipt);
		}

		internal static SetterDelegate ImapSuppressReadReceiptSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				if (value == null)
				{
					CASMailboxHelper.SetSettingsString(propertyBag, "IMAP4", 12, string.Empty, 14);
					return;
				}
				CASMailboxHelper.SetSettingsBool(propertyBag, "IMAP4", 12, (bool)value, 14);
			};
		}

		internal static GetterDelegate ImapForceICalForCalendarRetrievalOptionGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsBool(propertyBag, "IMAP4", 13, ADRecipientSchema.ImapForceICalForCalendarRetrievalOption);
		}

		internal static SetterDelegate ImapForceICalForCalendarRetrievalOptionSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				if (value == null)
				{
					CASMailboxHelper.SetSettingsString(propertyBag, "IMAP4", 13, string.Empty, 14);
					return;
				}
				CASMailboxHelper.SetSettingsBool(propertyBag, "IMAP4", 13, (bool)value, 14);
			};
		}

		internal static GetterDelegate MAPIEnabledGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsBool(propertyBag, "MAPI", 1, ADRecipientSchema.MAPIEnabled);
		}

		internal static SetterDelegate MAPIEnabledSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				CASMailboxHelper.SetSettingsBool(propertyBag, "MAPI", 1, (bool)value, 11);
			};
		}

		internal static GetterDelegate MapiHttpEnabledGetterDelegate()
		{
			return delegate(IPropertyBag propertyBag)
			{
				string settingsString = CASMailboxHelper.GetSettingsString(propertyBag, "MAPI", 9, "U");
				if (settingsString == "U")
				{
					return null;
				}
				return new bool?(settingsString == "Y");
			};
		}

		internal static SetterDelegate MapiHttpEnabledSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				string value2 = "U";
				if (value != null)
				{
					value2 = (((bool?)value).Value ? "Y" : "N");
				}
				CASMailboxHelper.SetSettingsString(propertyBag, "MAPI", 9, value2, 11);
			};
		}

		internal static GetterDelegate MAPIBlockOutlookNonCachedModeGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsBool(propertyBag, "MAPI", 2, ADRecipientSchema.MAPIBlockOutlookNonCachedMode);
		}

		internal static SetterDelegate MAPIBlockOutlookNonCachedModeSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				CASMailboxHelper.SetSettingsBool(propertyBag, "MAPI", 2, (bool)value, 11);
			};
		}

		internal static GetterDelegate MAPIBlockOutlookVersionsGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsString(propertyBag, "MAPI", 4, ADRecipientSchema.MAPIBlockOutlookVersions);
		}

		internal static SetterDelegate MAPIBlockOutlookVersionsSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				CASMailboxHelper.SetSettingsString(propertyBag, "MAPI", 4, (string)value, 11);
			};
		}

		internal static GetterDelegate MAPIBlockOutlookRpcHttpGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsBool(propertyBag, "MAPI", 5, ADRecipientSchema.MAPIBlockOutlookRpcHttp);
		}

		internal static SetterDelegate MAPIBlockOutlookRpcHttpSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				CASMailboxHelper.SetSettingsBool(propertyBag, "MAPI", 5, (bool)value, 11);
			};
		}

		internal static GetterDelegate MAPIBlockOutlookExternalConnectivityGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.GetSettingsBool(propertyBag, "MAPI", 10, ADRecipientSchema.MAPIBlockOutlookExternalConnectivity);
		}

		internal static SetterDelegate MAPIBlockOutlookExternalConnectivitySetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				CASMailboxHelper.SetSettingsBool(propertyBag, "MAPI", 10, (bool)value, 11);
			};
		}

		internal static GetterDelegate EwsOutlookAccessPoliciesGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.EwsGetTupleBool(propertyBag, "Outlook");
		}

		internal static SetterDelegate EwsOutlookAccessPoliciesSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				CASMailboxHelper.EwsSetTupleBool(propertyBag, "Outlook", (bool?)value);
			};
		}

		internal static GetterDelegate EwsMacOutlookAccessPoliciesGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.EwsGetTupleBool(propertyBag, "MacOutlook");
		}

		internal static SetterDelegate EwsMacOutlookAccessPoliciesSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				CASMailboxHelper.EwsSetTupleBool(propertyBag, "MacOutlook", (bool?)value);
			};
		}

		internal static GetterDelegate EwsEntourageAccessPoliciesGetterDelegate()
		{
			return (IPropertyBag propertyBag) => CASMailboxHelper.EwsGetTupleBool(propertyBag, "Entourage");
		}

		internal static SetterDelegate EwsEntourageAccessPoliciesSetterDelegate()
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				CASMailboxHelper.EwsSetTupleBool(propertyBag, "Entourage", (bool?)value);
			};
		}

		internal static bool? ToBooleanNullable(int? value)
		{
			if (value == null)
			{
				return null;
			}
			return new bool?(Convert.ToBoolean(value.Value));
		}

		internal static int? ToInt32Nullable(bool? value)
		{
			if (value == null)
			{
				return null;
			}
			return new int?(Convert.ToInt32(value.Value));
		}

		internal static int? ToInt32Nullable(EwsApplicationAccessPolicy? value)
		{
			if (value == null)
			{
				return null;
			}
			return new int?((int)value.Value);
		}

		private const int IdxPopImapEnabled = 1;

		private const int IdxPopImapUseProtocolDefaults = 2;

		private const int IdxPopImapMessagesRetrievalMimeFormat = 9;

		private const int IdxPopImapEnableExactRFC822Size = 10;

		private const int IdxPopImapProtocolLoggingEnabled = 11;

		private const int IdxPopImapSuppressReadReceipt = 12;

		private const int IdxPopImapForceICalForCalendarRetrievalOption = 13;

		private const int PopImapTotalNumberOfFields = 14;

		private const int IdxMAPIEnabled = 1;

		private const int IdxMAPIBlockOutlookNonCachedMode = 2;

		private const int IdxMAPIBlockOutlookVersions = 4;

		private const int IdxMAPIBlockOutlookRpcHttp = 5;

		private const int IdxMapiHttpEnabled = 9;

		private const int IdxMAPIBlockOutlookExternalConnectivity = 10;

		private const int MAPIProtocolSettingTotalNumberOfFields = 11;

		private const char SectionSymbol = '§';

		private const string OutlookName = "Outlook";

		private const string MacOutlookName = "MacOutlook";

		private const string EntourageName = "Entourage";

		internal static string MAPIBlockOutlookVersionsPattern = "^((((((\\d+(\\.\\d+){2})?\\-(\\d+(\\.\\d+){2})?)|(\\d+(\\.\\d+){2}))([,;] ?(((\\d+(\\.\\d+){2})?\\-(\\d+(\\.\\d+){2})?)|(\\d+(\\.\\d+){2})))*)?)|([0]))$";
	}
}
