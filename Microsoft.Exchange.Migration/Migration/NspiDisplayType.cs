using System;

namespace Microsoft.Exchange.Migration
{
	internal enum NspiDisplayType
	{
		MailUser,
		DistList,
		Forum,
		Agent,
		Organization,
		DistList_Private,
		Contact,
		Container = 256,
		Template,
		AddressTemplate,
		Search = 512,
		None = 65535
	}
}
