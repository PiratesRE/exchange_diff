using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IAutoAttendantUI
	{
		object ReadProperty(string name);

		void WriteProperty(string name, object value);

		object ReadGlobalProperty(string name);

		void WriteGlobalProperty(string name, object value);

		void SetTextPrompt(string name, string promptText);

		void SetWavePrompt(string name, ITempWavFile promptFile);
	}
}
