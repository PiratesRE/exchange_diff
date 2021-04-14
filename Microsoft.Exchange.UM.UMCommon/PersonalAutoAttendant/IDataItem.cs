using System;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal interface IDataItem
	{
		bool Validate(IDataValidator dataValidator);
	}
}
