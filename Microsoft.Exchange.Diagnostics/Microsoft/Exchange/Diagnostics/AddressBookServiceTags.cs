using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct AddressBookServiceTags
	{
		public const int General = 0;

		public const int Nspi = 1;

		public const int Referral = 2;

		public const int PropertyMapper = 3;

		public const int ModCache = 4;

		public const int NspiConnection = 5;

		public static Guid guid = new Guid("583dfb2d-4ab4-4416-848b-88cc74d39e1f");
	}
}
