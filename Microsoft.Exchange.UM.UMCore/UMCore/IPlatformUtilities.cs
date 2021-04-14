using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal interface IPlatformUtilities
	{
		bool IsTranscriptionLanguageSupported(CultureInfo transcriptionLanguage);

		IEnumerable<CultureInfo> SupportedTranscriptionLanguages { get; }

		void CompileGrammar(string grxmlGrammarPath, string compiledGrammarPath, CultureInfo culture);

		void CheckGrammarEntryFormat(string wordToCheck);

		ITempWavFile SynthesizePromptsToPcmWavFile(ArrayList prompts);

		void RecycleServiceDependencies();

		void InitializeG723Support();
	}
}
