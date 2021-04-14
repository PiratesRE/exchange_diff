using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class CorruptDataRecoveryStrategy
	{
		internal CorruptDataRecoveryStrategy()
		{
		}

		internal abstract void Recover(DefaultFolder defaultFolder, Exception e, ref DefaultFolderData defaultFolderData);

		internal static NoCorruptDataRecoveryStrategy DoNothing = new NoCorruptDataRecoveryStrategy();

		internal static RecreateCorruptDataRecoveryStrategy Recreate = new RecreateCorruptDataRecoveryStrategy();

		internal static ThrowCorruptDataRecoveryStrategy Throw = new ThrowCorruptDataRecoveryStrategy();

		internal static LegalHoldRecreateCorruptRecoveryStrategy LegalHold = new LegalHoldRecreateCorruptRecoveryStrategy();

		internal static RepairCorruptRecoveryStrategy Repair = new RepairCorruptRecoveryStrategy();
	}
}
