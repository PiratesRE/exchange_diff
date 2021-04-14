using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public class UMAAWelcomePromptRpcRequest : UMAutoAttendantPromptRpcRequest
	{
		private UMAAWelcomePromptRpcRequest(UMAutoAttendant aa, bool businessHoursFlag, string businessName) : this(aa)
		{
			this.MenuFlag = false;
			if (!string.IsNullOrEmpty(businessName))
			{
				base.AutoAttendant.BusinessName = businessName;
			}
			this.BusinessHoursFlag = businessHoursFlag;
		}

		private UMAAWelcomePromptRpcRequest(UMAutoAttendant aa, bool businessHoursFlag, CustomMenuKeyMapping[] keyMapping) : this(aa)
		{
			this.MenuFlag = true;
			if (keyMapping != null && keyMapping.Length != 0)
			{
				if (businessHoursFlag)
				{
					base.AutoAttendant.BusinessHoursKeyMapping = keyMapping;
					base.AutoAttendant.BusinessHoursKeyMappingEnabled = true;
				}
				else
				{
					base.AutoAttendant.AfterHoursKeyMapping = keyMapping;
					base.AutoAttendant.AfterHoursKeyMappingEnabled = true;
				}
			}
			this.BusinessHoursFlag = businessHoursFlag;
		}

		private UMAAWelcomePromptRpcRequest(UMAutoAttendant aa) : base(aa)
		{
			this.MenuFlag = false;
		}

		public bool BusinessHoursFlag { get; private set; }

		public bool MenuFlag { get; private set; }

		public static UMAAWelcomePromptRpcRequest BusinessHoursWithCustomBusinessName(UMAutoAttendant aa, string businessName)
		{
			return new UMAAWelcomePromptRpcRequest(aa, true, businessName);
		}

		public static UMAAWelcomePromptRpcRequest AfterHoursWithCustomBusinessName(UMAutoAttendant aa, string businessName)
		{
			return new UMAAWelcomePromptRpcRequest(aa, false, businessName);
		}

		public static UMAAWelcomePromptRpcRequest BusinessHoursWithCustomKeyMapping(UMAutoAttendant aa, CustomMenuKeyMapping[] keyMapping)
		{
			return new UMAAWelcomePromptRpcRequest(aa, true, keyMapping);
		}

		public static UMAAWelcomePromptRpcRequest AfterHoursWithCustomKeyMapping(UMAutoAttendant aa, CustomMenuKeyMapping[] keyMapping)
		{
			return new UMAAWelcomePromptRpcRequest(aa, false, keyMapping);
		}

		internal override string GetFriendlyName()
		{
			return Strings.AutoAttendantWelcomePromptRequest;
		}
	}
}
