using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMAAMenuKeyMapping
	{
		private UMAAMenuKeyMapping(CustomMenuKeyMapping keymapping, Dictionary<string, ADRecipient> legacyToRecipient)
		{
			this.Prompt = keymapping.Description;
			this.Key = keymapping.Key;
			this.ActionPromptFileName = keymapping.PromptFileName;
			this.TransferToExtension = string.Empty;
			if (!string.IsNullOrEmpty(keymapping.Extension))
			{
				this.ActionType = UMAAMenuActionTypeEnum.TransferToExtension;
				this.TransferToExtension = keymapping.Extension;
				this.ActionSummary = Strings.UMKeyMappingActionSummaryTransferToExtension(keymapping.Extension);
				return;
			}
			if (!string.IsNullOrEmpty(keymapping.AutoAttendantName))
			{
				this.TransferToAutoAttendant = new Identity(keymapping.AutoAttendantName, keymapping.AutoAttendantName);
				this.ActionType = UMAAMenuActionTypeEnum.TransferToAutoAttendant;
				this.ActionSummary = Strings.UMKeyMappingActionSummaryTransferToAA(keymapping.AutoAttendantName);
				return;
			}
			if (!string.IsNullOrEmpty(keymapping.LegacyDNToUseForLeaveVoicemailFor))
			{
				this.ActionType = UMAAMenuActionTypeEnum.LeaveVoicemailFor;
				ADRecipient adrecipient;
				if (legacyToRecipient.TryGetValue(keymapping.LegacyDNToUseForLeaveVoicemailFor, out adrecipient))
				{
					this.LeaveVoicemailForUser = new Identity(adrecipient.Id, adrecipient.DisplayName);
					this.ActionSummary = Strings.UMKeyMappingActionSummaryLeaveVM(adrecipient.DisplayName);
					return;
				}
				this.LeaveVoicemailForUser = new Identity(keymapping.LegacyDNToUseForLeaveVoicemailFor, keymapping.LegacyDNToUseForLeaveVoicemailFor);
				this.ActionSummary = Strings.UMKeyMappingActionSummaryLeaveVM(keymapping.LegacyDNToUseForLeaveVoicemailFor);
				return;
			}
			else
			{
				if (!string.IsNullOrEmpty(keymapping.AnnounceBusinessLocation))
				{
					this.ActionType = UMAAMenuActionTypeEnum.AnnounceBusinessLocation;
					this.ActionSummary = Strings.UMKeyMappingActionSummaryAnnounceBusinessLocation;
					return;
				}
				if (!string.IsNullOrEmpty(keymapping.AnnounceBusinessHours))
				{
					this.ActionType = UMAAMenuActionTypeEnum.AnnounceBusinessHours;
					this.ActionSummary = Strings.UMKeyMappingActionSummaryAnnounceBusinessHours;
					return;
				}
				this.ActionType = UMAAMenuActionTypeEnum.None;
				this.ActionSummary = string.Empty;
				return;
			}
		}

		[DataMember]
		public string Prompt { get; private set; }

		[DataMember]
		public UMAAMenuActionTypeEnum ActionType { get; private set; }

		[DataMember]
		public string ActionSummary { get; private set; }

		[DataMember]
		public string Key { get; private set; }

		[DataMember]
		public string ActionPromptFileName { get; private set; }

		[DataMember]
		public string TransferToExtension { get; private set; }

		[DataMember]
		public Identity TransferToAutoAttendant { get; private set; }

		[DataMember]
		public Identity LeaveVoicemailForUser { get; private set; }

		internal static void CreateMappings(MultiValuedProperty<CustomMenuKeyMapping> businessHours, MultiValuedProperty<CustomMenuKeyMapping> afterHours, out List<UMAAMenuKeyMapping> businessHoursContract, out List<UMAAMenuKeyMapping> afterHoursContract)
		{
			businessHoursContract = new List<UMAAMenuKeyMapping>(businessHours.Count);
			afterHoursContract = new List<UMAAMenuKeyMapping>(afterHours.Count);
			Dictionary<string, ADRecipient> legacyToRecipient = UMAAMenuKeyMapping.CreateRecipientMapping(new MultiValuedProperty<CustomMenuKeyMapping>[]
			{
				businessHours,
				afterHours
			});
			foreach (CustomMenuKeyMapping keymapping in businessHours)
			{
				businessHoursContract.Add(new UMAAMenuKeyMapping(keymapping, legacyToRecipient));
			}
			foreach (CustomMenuKeyMapping keymapping2 in afterHours)
			{
				afterHoursContract.Add(new UMAAMenuKeyMapping(keymapping2, legacyToRecipient));
			}
		}

		internal CustomMenuKeyMapping ToCustomKeyMapping()
		{
			this.Validate();
			return new CustomMenuKeyMapping(this.Key, this.Prompt, (this.ActionType == UMAAMenuActionTypeEnum.TransferToExtension) ? this.TransferToExtension : null, (this.ActionType == UMAAMenuActionTypeEnum.TransferToAutoAttendant) ? this.TransferToAutoAttendant.DisplayName : null, this.ActionPromptFileName, null, (this.ActionType == UMAAMenuActionTypeEnum.LeaveVoicemailFor) ? this.LeaveVoicemailForUser.RawIdentity : null, null, (this.ActionType == UMAAMenuActionTypeEnum.AnnounceBusinessLocation) ? "1" : null, (this.ActionType == UMAAMenuActionTypeEnum.AnnounceBusinessHours) ? "1" : null);
		}

		private static Dictionary<string, ADRecipient> CreateRecipientMapping(MultiValuedProperty<CustomMenuKeyMapping>[] customMappings)
		{
			List<string> list = new List<string>();
			Dictionary<string, ADRecipient> dictionary = new Dictionary<string, ADRecipient>(StringComparer.OrdinalIgnoreCase);
			foreach (MultiValuedProperty<CustomMenuKeyMapping> multiValuedProperty in customMappings)
			{
				foreach (CustomMenuKeyMapping customMenuKeyMapping in multiValuedProperty)
				{
					if (!string.IsNullOrEmpty(customMenuKeyMapping.LegacyDNToUseForLeaveVoicemailFor) && !list.Contains(customMenuKeyMapping.LegacyDNToUseForLeaveVoicemailFor))
					{
						list.Add(customMenuKeyMapping.LegacyDNToUseForLeaveVoicemailFor);
					}
				}
			}
			IEnumerable<ADRecipient> enumerable = RecipientObjectResolver.Instance.ResolveLegacyDNs(list);
			if (enumerable != null)
			{
				foreach (ADRecipient adrecipient in enumerable)
				{
					dictionary.Add(adrecipient.LegacyExchangeDN, adrecipient);
				}
			}
			return dictionary;
		}

		private void Validate()
		{
			switch (this.ActionType)
			{
			case UMAAMenuActionTypeEnum.TransferToExtension:
				this.TransferToExtension.FaultIfNullOrEmpty(Strings.UMHolidayScheduleTransferToExtensionNotSet);
				return;
			case UMAAMenuActionTypeEnum.TransferToAutoAttendant:
				this.TransferToAutoAttendant.FaultIfNull(Strings.UMHolidayScheduleTransferToAutoAttendantNotSet);
				return;
			case UMAAMenuActionTypeEnum.LeaveVoicemailFor:
				this.LeaveVoicemailForUser.FaultIfNull(Strings.UMHolidayScheduleLeaveVoicemailForNotSet);
				return;
			default:
				return;
			}
		}

		public const string TimeoutKey = "-";
	}
}
