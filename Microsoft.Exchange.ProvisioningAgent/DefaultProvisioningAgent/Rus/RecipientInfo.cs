using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.DefaultProvisioningAgent.Rus
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct RecipientInfo
	{
		public string CommonName;

		public string FirstName;

		public string LastName;

		public string DisplayName;

		public string Initials;
	}
}
