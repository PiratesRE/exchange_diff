using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon.FaultInjection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UMGrammarGeneratorFaultInjection
	{
		internal static bool TryCreateException(string exceptionType, ref Exception exception)
		{
			if (exceptionType != null && UMGrammarGeneratorFaultInjection.SpeechEngineException.Equals(exceptionType))
			{
				exception = new COMException(new LocalizedString("This is a test purpose exception for testing"));
				return true;
			}
			return false;
		}

		internal const uint SpeechGrammarCompile = 3039178045U;

		internal const uint SpeechEngineNormalize = 2502307133U;

		private static readonly string SpeechEngineException = "COMException";
	}
}
