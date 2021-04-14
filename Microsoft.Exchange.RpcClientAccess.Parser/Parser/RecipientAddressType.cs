using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal enum RecipientAddressType : ushort
	{
		None,
		Exchange,
		MicrosoftMail,
		Smtp,
		Fax,
		ProfessionalOfficeSystem,
		MapiPrivateDistributionList,
		DosPrivateDistributionList,
		Other = 32768,
		ValidMask = 32775
	}
}
