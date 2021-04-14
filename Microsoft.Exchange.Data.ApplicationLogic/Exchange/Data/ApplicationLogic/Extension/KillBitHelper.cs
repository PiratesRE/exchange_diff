using System;
using System.IO;
using System.Security;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	public class KillBitHelper
	{
		public static void ReadKillBitFromFile(object source, FileSystemEventArgs e)
		{
			int refreshRate;
			DateTime dateTime;
			if (KillBitHelper.TryReadKillBitFile(out refreshRate, out dateTime))
			{
				KillBitTimer.Singleton.UpdateTimerWithRefreshRate(refreshRate);
			}
		}

		public static bool TryReadKillBitFile(out int refreshRate, out DateTime lastModifiedTime)
		{
			refreshRate = -1;
			lastModifiedTime = DateTime.UtcNow;
			Exception ex = null;
			if (File.Exists(KillBitHelper.KillBitFilePath))
			{
				try
				{
					using (Stream stream = File.Open(KillBitHelper.KillBitFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						if (stream.Length == 0L)
						{
							KillBitHelper.Tracer.TraceError(0L, "The local killbit file is empty.");
							ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_EmptyKillbitListLocalFile, null, new object[]
							{
								"ProcessKillBit"
							});
						}
						else
						{
							using (XmlReader xmlReader = XmlReader.Create(stream))
							{
								lastModifiedTime = File.GetLastWriteTimeUtc(KillBitHelper.KillBitFilePath);
								return KillBitHelper.ReadKillBitXmlContent(xmlReader, out refreshRate);
							}
						}
					}
				}
				catch (SecurityException ex2)
				{
					ex = ex2;
				}
				catch (FileNotFoundException ex3)
				{
					ex = ex3;
				}
				catch (UriFormatException ex4)
				{
					ex = ex4;
				}
				catch (IOException ex5)
				{
					ex = ex5;
				}
				catch (XmlException ex6)
				{
					ex = ex6;
				}
				if (ex != null)
				{
					KillBitHelper.Tracer.TraceError<Exception>(0L, "Cannot read killbit refresh rate due to Exception {0}", ex);
					ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_FailedToReadKillbitList, null, new object[]
					{
						"ProcessKillBit",
						ExtensionDiagnostics.GetLoggedExceptionString(ex)
					});
					return false;
				}
				return false;
			}
			return false;
		}

		public static void DownloadKillBitList(object data)
		{
			if (KillBitHelper.getKillBit == null)
			{
				KillBitHelper.getKillBit = new GetKillBit(OmexWebServiceUrlsCache.Singleton);
			}
			KillBitHelper.getKillBit.Execute(new GetKillBit.SuccessCallback(KillBitHelper.GetKillBitSuccessCallback), new BaseAsyncCommand.FailureCallback(KillBitHelper.GetKillBitFailureCallback));
		}

		public static bool ReadKillBitXmlContent(XmlReader reader, out int refreshRate)
		{
			refreshRate = -1;
			if (reader.ReadToFollowing("o:assets") && reader.MoveToAttribute("o:rr") && int.TryParse(reader.Value, out refreshRate))
			{
				refreshRate = ((refreshRate > 6) ? refreshRate : 6);
				if (reader.ReadToFollowing("o:asset"))
				{
					KillBitList.Singleton.Clear();
					do
					{
						string attribute = reader.GetAttribute("o:assetid");
						string attribute2 = reader.GetAttribute("o:pid");
						if (string.IsNullOrWhiteSpace(attribute2))
						{
							KillBitHelper.Tracer.TraceError(0L, "The extension id is missing in the killbit entry.");
							ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_AppIdMissingInKillbitEntry, null, new object[]
							{
								"ProcessKillBit"
							});
						}
						else if (string.IsNullOrWhiteSpace(attribute))
						{
							KillBitHelper.Tracer.TraceError(0L, "The asset id is missing in the killbit entry.");
							ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_AssetIdMissingInKillbitEntry, null, new object[]
							{
								"ProcessKillBit"
							});
						}
						else
						{
							KilledExtensionEntry entry = new KilledExtensionEntry(ExtensionDataHelper.FormatExtensionId(attribute2), attribute);
							KillBitList.Singleton.Add(entry);
							ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_EntryAddedToKillbitList, attribute2, new object[]
							{
								"ProcessKillBit",
								attribute2,
								attribute
							});
						}
					}
					while (reader.ReadToNextSibling("o:asset"));
				}
				return true;
			}
			KillBitHelper.Tracer.TraceError(0L, "Cannot find KillBit asset tag or refresh rate in the file, will download killbit list again after one hour.");
			ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_KillbitAssetTagRefreshRateNotFound, null, new object[]
			{
				"ProcessKillBit"
			});
			return false;
		}

		private static void GetKillBitSuccessCallback()
		{
			KillBitHelper.Tracer.TraceDebug(0L, "GetKillBit call success.");
			ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_DownloadKillbitListSuccessed, null, new object[]
			{
				"GetKillBit"
			});
		}

		private static void GetKillBitFailureCallback(Exception exception)
		{
			if (exception != null)
			{
				KillBitHelper.Tracer.TraceWarning<Exception>(0L, "GetKillBit failed with exception: {0}", exception);
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_DownloadKillbitListFailed, null, new object[]
				{
					"GetKillBit",
					ExtensionDiagnostics.GetLoggedExceptionString(exception)
				});
			}
		}

		private const string ScenarioGetKillBitList = "GetKillBit";

		public const string ScenarioProcessKillBitList = "ProcessKillBit";

		public const string KillBitAssetsTagName = "o:assets";

		public const string KillBitRefreshRateAttributeName = "o:rr";

		public const string KillBitAssetTagName = "o:asset";

		public const string KillBitAssetIdAttributeName = "o:assetid";

		public const string KillBitPidAttributeName = "o:pid";

		public const int RefreshRateLowerBound = 6;

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;

		private static GetKillBit getKillBit;

		public static readonly string KillBitDirectory = ExchangeSetupContext.InstallPath + "ClientAccess\\owa\\" + string.Format("\\prem\\{0}\\ext\\killbit\\", DefaultExtensionTable.GetInstalledOwaVersion());

		public static readonly string KillBitFilePath = KillBitHelper.KillBitDirectory + "killbit.xml";
	}
}
