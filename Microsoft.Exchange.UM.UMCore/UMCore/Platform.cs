using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.Remoting;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class Platform : IPlatformBuilder, IPlatformUtilities
	{
		public static IPlatformBuilder Builder
		{
			get
			{
				return Platform.Instance;
			}
		}

		public static IPlatformUtilities Utilities
		{
			get
			{
				return Platform.Instance;
			}
		}

		private static Platform Instance
		{
			get
			{
				if (Platform.instance == null)
				{
					lock (Platform.staticLock)
					{
						if (Platform.instance == null)
						{
							Platform.instance = Platform.Create(AppConfig.Instance.Service.PlatformType);
						}
					}
				}
				return Platform.instance;
			}
		}

		public abstract BaseUMVoipPlatform CreateVoipPlatform();

		public abstract BaseCallRouterPlatform CreateCallRouterVoipPlatform(LocalizedString serviceName, LocalizedString serverName, UMADSettings config);

		public abstract PlatformSipUri CreateSipUri(string uri);

		public abstract bool TryCreateSipUri(string uriString, out PlatformSipUri sipUri);

		public abstract PlatformSipUri CreateSipUri(SipUriScheme scheme, string user, string host);

		public abstract PlatformSignalingHeader CreateSignalingHeader(string name, string value);

		public abstract bool TryCreateOfflineTranscriber(CultureInfo transcriptionLanguage, out BaseUMOfflineTranscriber transcriber);

		public abstract bool TryCreateMobileRecognizer(Guid requestId, CultureInfo culture, SpeechRecognitionEngineType engineType, int maxAlternates, out IMobileRecognizer recognizer);

		public abstract bool IsTranscriptionLanguageSupported(CultureInfo transcriptionLanguage);

		public abstract IEnumerable<CultureInfo> SupportedTranscriptionLanguages { get; }

		public abstract void CompileGrammar(string grxmlGrammarPath, string compiledGrammarPath, CultureInfo culture);

		public abstract void CheckGrammarEntryFormat(string wordToCheck);

		public abstract ITempWavFile SynthesizePromptsToPcmWavFile(ArrayList prompts);

		public abstract void RecycleServiceDependencies();

		public abstract void InitializeG723Support();

		private static Platform Create(PlatformType platformType)
		{
			Platform result;
			switch (platformType)
			{
			case PlatformType.MSS:
				result = Platform.CreateDynamicPlatform("Microsoft.Exchange.UM.MSSPlatform.dll", "Microsoft.Exchange.UM.MSSPlatform.MSSPlatform");
				break;
			case PlatformType.UCMA:
				result = Platform.CreateDynamicPlatform("Microsoft.Exchange.UM.UcmaPlatform.dll", "Microsoft.Exchange.UM.UcmaPlatform.UcmaPlatform");
				break;
			default:
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Unknown platform type {0}", new object[]
				{
					platformType.ToString(),
					"platform"
				}));
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		[SecurityCritical]
		private static void FirstChanceHandler(object source, FirstChanceExceptionEventArgs e)
		{
			if (Thread.CurrentThread.ManagedThreadId == Platform.platformThreadId)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMWorkerProcessUnhandledException, null, new object[]
				{
					"In First chance handler: " + e.Exception.ToString()
				});
				CallIdTracer.TraceError(ExTraceGlobals.ServiceTracer, 0, "In First chance handler: {0}", new object[]
				{
					e.Exception
				});
				ProcessLog.WriteLine(e.Exception.StackTrace, new object[0]);
				ExceptionHandling.SendWatsonWithExtraData(e.Exception, true);
			}
		}

		private static Platform CreateDynamicPlatform(string assemblyName, string className)
		{
			string text = Path.Combine(Utils.GetExchangeDirectory(), "bin");
			text = Path.Combine(text, assemblyName);
			if (Utils.GetLocalHostFqdn().ToLowerInvariant().EndsWith("extest.microsoft.com") && Process.GetCurrentProcess().ProcessName.ToLowerInvariant().Contains("umworkerprocess"))
			{
				Platform.platformThreadId = Thread.CurrentThread.ManagedThreadId;
				AppDomain.CurrentDomain.FirstChanceException += Platform.FirstChanceHandler;
				try
				{
					ObjectHandle objectHandle = Activator.CreateInstanceFrom(text, className);
					return (Platform)objectHandle.Unwrap();
				}
				finally
				{
					AppDomain.CurrentDomain.FirstChanceException -= Platform.FirstChanceHandler;
				}
			}
			ObjectHandle objectHandle2 = Activator.CreateInstanceFrom(text, className);
			return (Platform)objectHandle2.Unwrap();
		}

		private static Platform instance;

		private static object staticLock = new object();

		private static int platformThreadId;
	}
}
