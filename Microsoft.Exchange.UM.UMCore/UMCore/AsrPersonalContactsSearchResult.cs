using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AsrPersonalContactsSearchResult : AsrSearchResult
	{
		internal AsrPersonalContactsSearchResult(IUMRecognitionPhrase recognitionPhrase, UMSubscriber subscriber)
		{
			string id = (string)recognitionPhrase["ContactId"];
			this.selectedUser = ContactSearchItem.GetSelectedSearchItemFromId(subscriber, id);
		}

		internal override void SetManagerVariables(ActivityManager manager, BaseUMCallSession vo)
		{
			manager.WriteVariable("resultType", ResultType.PersonalContact);
			manager.WriteVariable("resultTypeString", ResultType.PersonalContact.ToString());
			manager.WriteVariable("selectedUser", this.selectedUser);
			manager.WriteVariable("directorySearchResult", this.selectedUser);
			manager.WriteVariable("selectedPhoneNumber", null);
		}

		private ContactSearchItem selectedUser;
	}
}
