using System;
using System.Collections;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface IPassword
	{
		int LockoutCount { get; set; }

		PasswordBlob CurrentPassword { get; set; }

		ExDateTime TimeSet { get; set; }

		ArrayList OldPasswords { get; set; }

		void Commit();
	}
}
