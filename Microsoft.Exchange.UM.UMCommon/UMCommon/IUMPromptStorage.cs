using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface IUMPromptStorage : IDisposeTrackable, IDisposable
	{
		void CreatePrompt(string promptName, string audioBytes);

		string GetPrompt(string promptName);

		string[] GetPromptNames();

		string[] GetPromptNames(TimeSpan timeSinceLastModified);

		void DeletePrompts(string[] prompts);

		void DeleteAllPrompts();
	}
}
