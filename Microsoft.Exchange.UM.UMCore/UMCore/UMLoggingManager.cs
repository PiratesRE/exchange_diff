using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class UMLoggingManager : DisposableBase
	{
		internal abstract void EnterTurn(string turnName);

		internal abstract void ExitTurn();

		internal abstract void EnterTask(string name);

		internal abstract void ExitTask(UMNavigationState state, string message);

		internal abstract void LogApplicationInformation(string format, params object[] args);
	}
}
