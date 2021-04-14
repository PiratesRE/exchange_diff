using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AsrExtensionSearchResult : AsrSearchResult
	{
		internal AsrExtensionSearchResult(string extension)
		{
			this.selectedPhoneNumber = PhoneNumber.CreateExtension(extension);
		}

		public PhoneNumber Extension
		{
			get
			{
				return this.selectedPhoneNumber;
			}
		}

		internal override void SetManagerVariables(ActivityManager manager, BaseUMCallSession vo)
		{
			manager.WriteVariable("resultType", ResultType.UserExtension);
			manager.WriteVariable("resultTypeString", ResultType.UserExtension.ToString());
			manager.WriteVariable("selectedUser", null);
			manager.WriteVariable("directorySearchResult", null);
			manager.WriteVariable("selectedPhoneNumber", this.selectedPhoneNumber);
		}

		private PhoneNumber selectedPhoneNumber;
	}
}
