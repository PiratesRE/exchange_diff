using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class StoreHelper
	{
		internal static void SetStoreUserInformationReader(IStoreUserInformationReader storeUserInformationReader)
		{
			MbxRecipientSession.StoreUserInformationReader = storeUserInformationReader;
		}
	}
}
