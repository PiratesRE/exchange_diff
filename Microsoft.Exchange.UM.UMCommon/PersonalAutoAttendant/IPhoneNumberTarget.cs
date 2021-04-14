using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal interface IPhoneNumberTarget
	{
		PhoneNumber GetDialableNumber();
	}
}
