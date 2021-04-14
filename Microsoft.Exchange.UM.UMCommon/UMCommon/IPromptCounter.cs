using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface IPromptCounter
	{
		void SetPromptCount(string promptId, int newCount);

		int GetPromptCount(string promptId);

		void SavePromptCount();
	}
}
