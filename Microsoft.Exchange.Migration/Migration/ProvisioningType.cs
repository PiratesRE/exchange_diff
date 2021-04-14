using System;

namespace Microsoft.Exchange.Migration
{
	internal enum ProvisioningType
	{
		Unknown,
		User,
		Contact,
		Group,
		GroupMember,
		UserUpdate,
		ContactUpdate,
		MailEnabledUser,
		MailEnabledUserUpdate,
		XO1User
	}
}
