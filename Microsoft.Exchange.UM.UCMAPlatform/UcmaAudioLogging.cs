using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal static class UcmaAudioLogging
	{
		public static bool CmdAndControlAudioLoggingEnabled { get; private set; } = UcmaAudioLogging.GetRegistryValue("CommandAndControlAudioLogging");

		public static bool MobileSpeechRecoAudioLoggingEnabled { get; private set; } = UcmaAudioLogging.GetRegistryValue("MobileSpeechRecoAudioLogging");

		private static bool GetRegistryValue(string valueName)
		{
			bool flag = false;
			int num = 0;
			if (Utils.TryReadRegValue("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole", valueName, out num) && num == 1)
			{
				flag = true;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, 0, "UCMA audio logging setting Name='{0}', Value='{1}', Enabled='{2}'", new object[]
			{
				valueName,
				num,
				flag
			});
			return flag;
		}

		private const string CmdAndControlAudioLoggingValueName = "CommandAndControlAudioLogging";

		private const string MobileSpeechRecoAudioLoggingValueName = "MobileSpeechRecoAudioLogging";
	}
}
