using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal interface IPersonalContactCache
	{
		void BuildCache();

		bool IsContactValid(StoreObjectId contactId);

		PersonalContactInfo GetContact(StoreObjectId contactId);
	}
}
