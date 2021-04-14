using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class MobileDeviceMailboxPolicyService : DDICodeBehind
	{
		public MobileDeviceMailboxPolicyService()
		{
			base.RegisterRbacDependency("MaxDevicePasswordFailedAttemptsString", new List<string>(new string[]
			{
				"MaxPasswordFailedAttempts"
			}));
		}

		public static void GetListPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (DBNull.Value != dataRow["WhenChangedUTC"])
				{
					dataRow["Modified"] = ((DateTime?)dataRow["WhenChangedUTC"]).UtcToUserDateTimeString();
				}
			}
		}

		public static void GetObjectPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			MobileMailboxPolicy mobileMailboxPolicy = store.GetDataObject("MobileMailboxPolicy") as MobileMailboxPolicy;
			if (dataTable.Rows.Count == 1 && mobileMailboxPolicy != null)
			{
				DataRow dataRow = dataTable.Rows[0];
				dataRow["IsMinPasswordLengthSet"] = (mobileMailboxPolicy.PasswordEnabled && mobileMailboxPolicy.MinPasswordLength != null);
				dataRow["MinPasswordLength"] = ((mobileMailboxPolicy.MinPasswordLength != null) ? ((int)dataRow["MinPasswordLength"]) : 4);
				dataRow["IsMaxPasswordFailedAttemptsSet"] = (mobileMailboxPolicy.PasswordEnabled && !mobileMailboxPolicy.MaxPasswordFailedAttempts.IsUnlimited);
				dataRow["MaxPasswordFailedAttempts"] = (mobileMailboxPolicy.MaxPasswordFailedAttempts.IsUnlimited ? "8" : mobileMailboxPolicy.MaxPasswordFailedAttempts.Value.ToString(CultureInfo.InvariantCulture));
				dataRow["IsMaxInactivityTimeLockSet"] = (mobileMailboxPolicy.PasswordEnabled && !mobileMailboxPolicy.MaxInactivityTimeLock.IsUnlimited);
				dataRow["MaxInactivityTimeLock"] = (mobileMailboxPolicy.MaxInactivityTimeLock.IsUnlimited ? "15" : mobileMailboxPolicy.MaxInactivityTimeLock.Value.TotalMinutes.ToString(CultureInfo.InvariantCulture));
				dataRow["IsPasswordExpirationSet"] = (mobileMailboxPolicy.PasswordEnabled && !mobileMailboxPolicy.PasswordExpiration.IsUnlimited);
				dataRow["PasswordExpiration"] = (mobileMailboxPolicy.PasswordExpiration.IsUnlimited ? "90" : mobileMailboxPolicy.PasswordExpiration.Value.Days.ToString());
				dataRow["PasswordRequirementsString"] = MobileDeviceMailboxPolicyService.PasswordRequirementsString(dataRow);
				dataRow["HasAdditionalCustomSettings"] = MobileDeviceMailboxPolicyService.HasAdditionalCustomSettings(mobileMailboxPolicy);
			}
		}

		private static string PasswordRequirementsString(DataRow row)
		{
			if (!(bool)row["PasswordEnabled"])
			{
				return Strings.PasswordNotRequired;
			}
			if ((bool)row["IsMaxInactivityTimeLockSet"])
			{
				if ((bool)row["AlphanumericPasswordRequired"])
				{
					return string.Format(Strings.RequiredAlphaLockingPassword, (int)row["MinPasswordLength"], (string)row["MaxInactivityTimeLock"]);
				}
				if ((bool)row["IsMinPasswordLengthSet"])
				{
					return string.Format(Strings.RequiredPinLockingPassword, (int)row["MinPasswordLength"], (string)row["MaxInactivityTimeLock"]);
				}
				return string.Format(Strings.RequiredLockingPassword, (string)row["MaxInactivityTimeLock"]);
			}
			else
			{
				if ((bool)row["AlphanumericPasswordRequired"])
				{
					return string.Format(Strings.RequiredAlphaPassword, (int)row["MinPasswordLength"]);
				}
				if ((bool)row["IsMinPasswordLengthSet"])
				{
					return string.Format(Strings.RequiredPinPassword, (int)row["MinPasswordLength"]);
				}
				return Strings.PasswordRequired;
			}
		}

		public static void SetObjectPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			List<string> list = new List<string>();
			if (DBNull.Value.Equals(inputRow["MinPasswordComplexCharacters"]))
			{
				inputRow["MinPasswordComplexCharacters"] = "3";
				list.Add("MinPasswordComplexCharacters");
			}
			if (DBNull.Value != inputRow["MaxPasswordFailedAttempts"] || DBNull.Value != inputRow["IsMaxPasswordFailedAttemptsSet"])
			{
				bool isValueSet = DBNull.Value.Equals(inputRow["IsMaxPasswordFailedAttemptsSet"]) || (bool)inputRow["IsMaxPasswordFailedAttemptsSet"];
				string value = (DBNull.Value != inputRow["MaxPasswordFailedAttempts"]) ? ((string)inputRow["MaxPasswordFailedAttempts"]) : "8";
				object value2;
				if (MobileDeviceMailboxPolicyService.CheckAndParseParams<int>(true, isValueSet, value, (string x) => int.Parse(x), out value2))
				{
					inputRow["MaxPasswordFailedAttempts"] = value2;
					list.Add("MaxPasswordFailedAttempts");
					list.Add("IsMaxPasswordFailedAttemptsSet");
				}
			}
			if (DBNull.Value != inputRow["PasswordExpiration"] || DBNull.Value != inputRow["IsPasswordExpirationSet"])
			{
				bool isValueSet = DBNull.Value.Equals(inputRow["IsPasswordExpirationSet"]) || (bool)inputRow["IsPasswordExpirationSet"];
				string value = (DBNull.Value != inputRow["PasswordExpiration"]) ? ((string)inputRow["PasswordExpiration"]) : "90";
				object value2;
				if (MobileDeviceMailboxPolicyService.CheckAndParseParams<EnhancedTimeSpan>(true, isValueSet, value, (string x) => EnhancedTimeSpan.FromDays(double.Parse(x)), out value2))
				{
					inputRow["PasswordExpiration"] = value2;
					list.Add("PasswordExpiration");
					list.Add("IsPasswordExpirationSet");
				}
			}
			if (DBNull.Value != inputRow["MaxInactivityTimeLock"] || DBNull.Value != inputRow["IsMaxInactivityTimeLockSet"])
			{
				bool isValueSet = DBNull.Value.Equals(inputRow["IsMaxInactivityTimeLockSet"]) || (bool)inputRow["IsMaxInactivityTimeLockSet"];
				string value = (DBNull.Value != inputRow["MaxInactivityTimeLock"]) ? ((string)inputRow["MaxInactivityTimeLock"]) : "15";
				object value2;
				if (MobileDeviceMailboxPolicyService.CheckAndParseParams<EnhancedTimeSpan>(true, isValueSet, value, (string x) => EnhancedTimeSpan.FromMinutes(double.Parse(x)), out value2))
				{
					inputRow["MaxInactivityTimeLock"] = value2;
					list.Add("MaxInactivityTimeLock");
					list.Add("IsMaxInactivityTimeLockSet");
				}
			}
			if (DBNull.Value != inputRow["MinPasswordLength"] || DBNull.Value != inputRow["IsMinPasswordLengthSet"])
			{
				bool isValueSet = DBNull.Value.Equals(inputRow["IsMinPasswordLengthSet"]) || (bool)inputRow["IsMinPasswordLengthSet"];
				string value = (DBNull.Value != inputRow["MinPasswordLength"]) ? ((string)inputRow["MinPasswordLength"]) : 4.ToString();
				object value2;
				if (MobileDeviceMailboxPolicyService.CheckAndParseParams<int>(false, isValueSet, value, (string x) => int.Parse(x), out value2))
				{
					inputRow["MinPasswordLength"] = value2;
					list.Add("MinPasswordLength");
					list.Add("IsMinPasswordLengthSet");
				}
			}
			if (list.Count > 0)
			{
				store.SetModifiedColumns(list);
			}
		}

		private static bool CheckAndParseParams<T>(bool isUnlimited, bool isValueSet, string value, Func<string, T> convert, out object result) where T : struct, IComparable
		{
			isValueSet = (!DBNull.Value.Equals(isValueSet) && isValueSet);
			bool result2 = false;
			result = null;
			if (!isValueSet)
			{
				if (isUnlimited)
				{
					result = Unlimited<T>.UnlimitedString;
				}
				else
				{
					result = null;
				}
				result2 = true;
			}
			else if (isValueSet)
			{
				try
				{
					if (isUnlimited)
					{
						result = new Unlimited<T>(convert(value));
					}
					else
					{
						result = convert(value);
					}
					result2 = true;
				}
				catch (Exception)
				{
					result2 = false;
				}
			}
			return result2;
		}

		private static bool HasAdditionalCustomSettings(MobileMailboxPolicy policy)
		{
			foreach (ADPropertyDefinition adpropertyDefinition in MobileDeviceMailboxPolicyService.AdditionalProperties)
			{
				if ((adpropertyDefinition.Flags & ADPropertyDefinitionFlags.MultiValued) == ADPropertyDefinitionFlags.MultiValued)
				{
					if (adpropertyDefinition.DefaultValue != null)
					{
						throw new NotSupportedException("Non-empty default value for multivalued property is not supported!");
					}
					if (policy[adpropertyDefinition] != null && ((IList)policy[adpropertyDefinition]).Count > 0)
					{
						return true;
					}
				}
				else if (adpropertyDefinition.Type == typeof(bool))
				{
					if ((bool)policy[adpropertyDefinition] != (bool)adpropertyDefinition.DefaultValue)
					{
						return true;
					}
				}
				else if (adpropertyDefinition.Type == typeof(int))
				{
					if ((int)policy[adpropertyDefinition] != (int)adpropertyDefinition.DefaultValue)
					{
						return true;
					}
				}
				else if (adpropertyDefinition.Type == typeof(Unlimited<int>))
				{
					if ((Unlimited<int>)policy[adpropertyDefinition] != (Unlimited<int>)adpropertyDefinition.DefaultValue)
					{
						return true;
					}
				}
				else if (adpropertyDefinition.Type == typeof(Unlimited<EnhancedTimeSpan>))
				{
					if ((Unlimited<EnhancedTimeSpan>)policy[adpropertyDefinition] != (Unlimited<EnhancedTimeSpan>)adpropertyDefinition.DefaultValue)
					{
						return true;
					}
				}
				else if (adpropertyDefinition.Type == typeof(Unlimited<ByteQuantifiedSize>))
				{
					if ((Unlimited<ByteQuantifiedSize>)policy[adpropertyDefinition] != (Unlimited<ByteQuantifiedSize>)adpropertyDefinition.DefaultValue)
					{
						return true;
					}
				}
				else if (policy[adpropertyDefinition] != adpropertyDefinition.DefaultValue)
				{
					return true;
				}
			}
			return false;
		}

		public static void GetDefaultPolicyPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			dataTable.BeginLoadData();
			List<DataRow> list = new List<DataRow>();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				DataRow dataRow = dataTable.Rows[i];
				if (false.Equals(dataRow["IsDefault"]))
				{
					list.Add(dataRow);
				}
				else
				{
					MobileDeviceMailboxPolicyService.UpdateDefaultPolicy(dataRow);
				}
			}
			foreach (DataRow row in list)
			{
				dataTable.Rows.Remove(row);
			}
			dataTable.EndLoadData();
		}

		private static void UpdateDefaultPolicy(DataRow row)
		{
			bool flag = false;
			if (!DBNull.Value.Equals(row["PasswordEnabled"]))
			{
				flag = (bool)row["PasswordEnabled"];
			}
			row["IsMinPasswordLengthSet"] = (flag && !DBNull.Value.Equals(row["MinPasswordLength"]));
			if (DBNull.Value.Equals(row["MinPasswordLength"]))
			{
				row["MinPasswordLength"] = 4;
			}
			if (!DBNull.Value.Equals(row["MaxPasswordFailedAttempts"]))
			{
				Unlimited<int> unlimited = (Unlimited<int>)row["MaxPasswordFailedAttempts"];
				row["IsMaxPasswordFailedAttemptsSet"] = (flag && !unlimited.IsUnlimited);
				row["MaxPasswordFailedAttempts"] = (unlimited.IsUnlimited ? "8" : unlimited.Value.ToString(CultureInfo.InvariantCulture));
			}
			else
			{
				row["MaxPasswordFailedAttempts"] = "8";
				row["IsMaxPasswordFailedAttemptsSet"] = false;
			}
			if (!DBNull.Value.Equals(row["MaxInactivityTimeLock"]))
			{
				Unlimited<EnhancedTimeSpan> unlimited2 = (Unlimited<EnhancedTimeSpan>)row["MaxInactivityTimeLock"];
				row["IsMaxInactivityTimeLockSet"] = (flag && !unlimited2.IsUnlimited);
				row["MaxInactivityTimeLock"] = (unlimited2.IsUnlimited ? "15" : unlimited2.Value.TotalMinutes.ToString(CultureInfo.InvariantCulture));
				return;
			}
			row["MaxInactivityTimeLock"] = "15";
			row["IsMaxInactivityTimeLockSet"] = false;
		}

		internal const string DefaultMaxPasswordFailedAttempts = "8";

		internal const string DefaultPasswordExpiration = "90";

		internal const string DefaultMaxInactivityTimeLock = "15";

		internal const string DefaultMinPasswordComplexCharacters = "3";

		internal const int DefaultMinPasswordLength = 4;

		internal const string IsMinPasswordLengthSetColumnName = "IsMinPasswordLengthSet";

		internal const string MinPasswordLengthColumnName = "MinPasswordLength";

		internal const string IsMaxPasswordFailedAttemptsSetColumnName = "IsMaxPasswordFailedAttemptsSet";

		internal const string HasAdditionalCustomSettingsColumnName = "HasAdditionalCustomSettings";

		internal const string MaxPasswordFailedAttemptsColumnName = "MaxPasswordFailedAttempts";

		internal const string IsMaxInactivityTimeLockSetColumnName = "IsMaxInactivityTimeLockSet";

		internal const string MaxInactivityTimeLockColumnName = "MaxInactivityTimeLock";

		internal const string IsPasswordExpirationSetColumnName = "IsPasswordExpirationSet";

		internal const string PasswordExpirationColumnName = "PasswordExpiration";

		internal const string PasswordRequirementsStringColumnName = "PasswordRequirementsString";

		internal const string PasswordEnabledColumnName = "PasswordEnabled";

		internal const string WhenChangedUTCColumnName = "WhenChangedUTC";

		internal const string ModifiedColumnName = "Modified";

		internal const string AlphanumericPasswordRequiredColumnName = "AlphanumericPasswordRequired";

		internal const string MinPasswordComplexCharactersColumnName = "MinPasswordComplexCharacters";

		private static ADPropertyDefinition[] AdditionalProperties = new ADPropertyDefinition[]
		{
			MobileMailboxPolicySchema.UnapprovedInROMApplicationList,
			MobileMailboxPolicySchema.ADApprovedApplicationList,
			MobileMailboxPolicySchema.AttachmentsEnabled,
			MobileMailboxPolicySchema.RequireStorageCardEncryption,
			MobileMailboxPolicySchema.PasswordRecoveryEnabled,
			MobileMailboxPolicySchema.DevicePolicyRefreshInterval,
			MobileMailboxPolicySchema.MaxAttachmentSize,
			MobileMailboxPolicySchema.WSSAccessEnabled,
			MobileMailboxPolicySchema.UNCAccessEnabled,
			MobileMailboxPolicySchema.DenyApplePushNotifications,
			MobileMailboxPolicySchema.AllowStorageCard,
			MobileMailboxPolicySchema.AllowCamera,
			MobileMailboxPolicySchema.AllowUnsignedApplications,
			MobileMailboxPolicySchema.AllowUnsignedInstallationPackages,
			MobileMailboxPolicySchema.AllowWiFi,
			MobileMailboxPolicySchema.AllowTextMessaging,
			MobileMailboxPolicySchema.AllowPOPIMAPEmail,
			MobileMailboxPolicySchema.AllowIrDA,
			MobileMailboxPolicySchema.RequireManualSyncWhenRoaming,
			MobileMailboxPolicySchema.AllowDesktopSync,
			MobileMailboxPolicySchema.AllowHTMLEmail,
			MobileMailboxPolicySchema.RequireSignedSMIMEMessages,
			MobileMailboxPolicySchema.RequireEncryptedSMIMEMessages,
			MobileMailboxPolicySchema.AllowSMIMESoftCerts,
			MobileMailboxPolicySchema.AllowBrowser,
			MobileMailboxPolicySchema.AllowConsumerEmail,
			MobileMailboxPolicySchema.AllowRemoteDesktop,
			MobileMailboxPolicySchema.AllowInternetSharing,
			MobileMailboxPolicySchema.AllowBluetooth,
			MobileMailboxPolicySchema.MaxCalendarAgeFilter,
			MobileMailboxPolicySchema.MaxEmailAgeFilter,
			MobileMailboxPolicySchema.RequireSignedSMIMEAlgorithm,
			MobileMailboxPolicySchema.RequireEncryptionSMIMEAlgorithm,
			MobileMailboxPolicySchema.AllowSMIMEEncryptionAlgorithmNegotiation,
			MobileMailboxPolicySchema.MaxEmailBodyTruncationSize,
			MobileMailboxPolicySchema.MaxEmailHTMLBodyTruncationSize,
			MobileMailboxPolicySchema.AllowExternalDeviceManagement,
			MobileMailboxPolicySchema.MobileOTAUpdateMode,
			MobileMailboxPolicySchema.AllowMobileOTAUpdate,
			MobileMailboxPolicySchema.IrmEnabled
		};
	}
}
