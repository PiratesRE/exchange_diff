using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AsrDepartmentSearchResult : AsrSearchResult
	{
		internal AsrDepartmentSearchResult(CustomMenuKeyMapping menu)
		{
			this.optionType = AutoAttendantCustomOptionType.None;
			if (!string.IsNullOrEmpty(menu.Extension))
			{
				this.optionType = AutoAttendantCustomOptionType.TransferToExtension;
				this.transferTarget = menu.Extension;
			}
			else if (!string.IsNullOrEmpty(menu.AutoAttendantName))
			{
				this.transferTarget = menu.AutoAttendantName;
				this.optionType = AutoAttendantCustomOptionType.TransferToAutoAttendant;
			}
			else if (!string.IsNullOrEmpty(menu.LegacyDNToUseForLeaveVoicemailFor))
			{
				this.transferTarget = menu.LegacyDNToUseForLeaveVoicemailFor;
				this.optionType = AutoAttendantCustomOptionType.TransferToVoicemailDirectly;
			}
			else if (!string.IsNullOrEmpty(menu.LegacyDNToUseForTransferToMailbox))
			{
				this.transferTarget = menu.LegacyDNToUseForTransferToMailbox;
				this.optionType = AutoAttendantCustomOptionType.TransferToVoicemailPAA;
			}
			else if (!string.IsNullOrEmpty(menu.AnnounceBusinessLocation))
			{
				this.transferTarget = menu.AnnounceBusinessLocation;
				this.optionType = AutoAttendantCustomOptionType.ReadBusinessLocation;
			}
			else if (!string.IsNullOrEmpty(menu.AnnounceBusinessHours))
			{
				this.transferTarget = menu.AnnounceBusinessHours;
				this.optionType = AutoAttendantCustomOptionType.ReadBusinessHours;
			}
			this.promptFileName = menu.PromptFileName;
			this.keyPress = menu.MappedKey;
		}

		internal AsrDepartmentSearchResult(IUMRecognitionPhrase recognitionPhrase)
		{
			this.transferTarget = (string)recognitionPhrase["Extension"];
			this.departmentName = (string)recognitionPhrase["DepartmentName"];
			string value = (string)recognitionPhrase["CustomMenuTarget"];
			this.promptFileName = (string)recognitionPhrase["PromptFileName"];
			this.optionType = (AutoAttendantCustomOptionType)Enum.Parse(typeof(AutoAttendantCustomOptionType), value);
			value = (string)recognitionPhrase["MappedKey"];
			this.keyPress = (CustomMenuKey)Enum.Parse(typeof(CustomMenuKey), value);
		}

		public PhoneNumber PhoneNumber
		{
			get
			{
				return this.selectedPhoneNumber;
			}
		}

		public string DepartmentName
		{
			get
			{
				return this.departmentName;
			}
		}

		public CustomMenuKey KeyPress
		{
			get
			{
				return this.keyPress;
			}
		}

		public AutoAttendantCustomOptionType OptionType
		{
			get
			{
				return this.optionType;
			}
		}

		public string PromptFileName
		{
			get
			{
				return this.promptFileName;
			}
		}

		public string TransferTarget
		{
			get
			{
				return this.transferTarget;
			}
		}

		internal override void SetManagerVariables(ActivityManager manager, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "For department {0}, type= {1} returning phone, or aa: {2}.", new object[]
			{
				this.departmentName,
				this.optionType,
				this.transferTarget
			});
			if (this.optionType == AutoAttendantCustomOptionType.TransferToExtension)
			{
				this.selectedPhoneNumber = AutoAttendantCore.GetCustomExtensionNumberToDial(vo.CurrentCallContext, this.transferTarget);
			}
			manager.WriteVariable("resultType", ResultType.Department);
			manager.WriteVariable("resultTypeString", ResultType.Department.ToString());
			manager.WriteVariable("selectedUser", null);
			if (this.selectedPhoneNumber != null)
			{
				manager.WriteVariable("selectedPhoneNumber", this.selectedPhoneNumber);
			}
			bool flag = !string.IsNullOrEmpty(this.promptFileName);
			manager.WriteVariable("haveCustomMenuOptionPrompt", flag);
			if (flag)
			{
				manager.WriteVariable("customMenuOptionPrompt", vo.CurrentCallContext.UMConfigCache.GetPrompt<UMAutoAttendant>(vo.CurrentCallContext.AutoAttendantInfo, this.promptFileName));
			}
			manager.WriteVariable("customMenuOption", this.optionType.ToString());
		}

		private PhoneNumber selectedPhoneNumber;

		private string departmentName;

		private AutoAttendantCustomOptionType optionType;

		private CustomMenuKey keyPress;

		private string transferTarget;

		private string promptFileName;
	}
}
