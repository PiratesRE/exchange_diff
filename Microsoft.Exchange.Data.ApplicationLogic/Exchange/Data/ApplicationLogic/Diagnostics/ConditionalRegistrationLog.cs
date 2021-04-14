using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	internal class ConditionalRegistrationLog
	{
		internal static Action<XElement, Exception> OnFailedHydration { get; set; }

		public static string ProtocolName
		{
			get
			{
				return ConditionalRegistrationLog.protocolName;
			}
			set
			{
				ConditionalRegistrationLog.protocolName = value;
				ConditionalRegistrationLog.LoggingDirectory = Path.Combine(ConditionalRegistrationLog.LoggingRootPath, ConditionalRegistrationLog.protocolName);
				ConditionalRegistrationLog.CreateLogFilePath();
			}
		}

		static ConditionalRegistrationLog()
		{
			ConditionalRegistrationLog.LoggingRootPath = Path.Combine(ExchangeSetupContext.LoggingPath, "ConditionalHandlers");
			ConditionalRegistrationLog.CreateLogFilePath();
			ConditionalRegistrationLog.writerSettings = new XmlWriterSettings();
			ConditionalRegistrationLog.writerSettings.CheckCharacters = false;
			ConditionalRegistrationLog.writerSettings.CloseOutput = true;
			ConditionalRegistrationLog.writerSettings.ConformanceLevel = ConformanceLevel.Fragment;
			ConditionalRegistrationLog.writerSettings.Indent = true;
			ConditionalRegistrationLog.readerSettings = new XmlReaderSettings();
			ConditionalRegistrationLog.readerSettings.CheckCharacters = false;
			ConditionalRegistrationLog.readerSettings.ConformanceLevel = ConformanceLevel.Fragment;
			ConditionalRegistrationLog.readerSettings.DtdProcessing = DtdProcessing.Prohibit;
		}

		public static string Save(ConditionalRegistration registration)
		{
			string text = Path.Combine(ConditionalRegistrationLog.GetConditionalRegistrationsDirectory(), string.Format("{0}.{1}", registration.Cookie, "xml"));
			ExTraceGlobals.DiagnosticHandlersTracer.TraceDebug<string, string>(0L, "[ConditionalRegistrationLog.Save] Logging registration for '{0}' of user '{1}'", registration.Cookie, registration.User);
			using (XmlWriter xmlWriter = XmlWriter.Create(text, ConditionalRegistrationLog.writerSettings))
			{
				XElement xelement = new XElement("ConditionalRegistration");
				XElement content = new XElement("Description")
				{
					Value = registration.Description
				};
				xelement.Add(content);
				XElement content2 = new XElement("CreatedTime")
				{
					Value = registration.Created.ToString()
				};
				xelement.Add(content2);
				XElement content3 = new XElement("User")
				{
					Value = registration.User
				};
				xelement.Add(content3);
				XElement content4 = new XElement("Cookie")
				{
					Value = registration.Cookie
				};
				xelement.Add(content4);
				XElement content5 = new XElement("PropertiesToFetch")
				{
					Value = registration.OriginalPropertiesToFetch
				};
				xelement.Add(content5);
				XElement content6 = new XElement("Filter")
				{
					Value = registration.OriginalFilter
				};
				xelement.Add(content6);
				XElement content7 = new XElement("TimeToLive")
				{
					Value = registration.TimeToLive.ToString()
				};
				xelement.Add(content7);
				XElement content8 = new XElement("MaxHits")
				{
					Value = registration.MaxHits.ToString()
				};
				xelement.Add(content8);
				xelement.WriteTo(xmlWriter);
				xmlWriter.Flush();
			}
			return text;
		}

		public static void DeleteRegistration(string cookie)
		{
			string path = Path.Combine(ConditionalRegistrationLog.GetConditionalRegistrationsDirectory(), string.Format("{0}.{1}", cookie, "xml"));
			ExTraceGlobals.DiagnosticHandlersTracer.TraceDebug<string>(0L, "[ConditionalRegistrationLog.DeleteRegistration] Deleting registration for '{0}'", cookie);
			if (File.Exists(path))
			{
				try
				{
					File.Delete(path);
				}
				catch (Exception ex)
				{
					ExTraceGlobals.DiagnosticHandlersTracer.TraceDebug<string, string>(0L, "[ConditionalRegistrationLog.DeleteRegistration] failure deleting registration for '{0}'. Exception: {1}", cookie, ex.ToString());
				}
			}
		}

		public static string Save(ConditionalResults hit)
		{
			string fullPathForCookie = ConditionalRegistrationLog.GetFullPathForCookie(hit.Registration.User.Replace("/", "-"), hit.Registration.Cookie);
			ConditionalRegistrationLog.LimitFileCount(fullPathForCookie);
			ExTraceGlobals.DiagnosticHandlersTracer.TraceDebug<string, string>(0L, "[ConditionalRegistrationLog.Log] Logging registration results for '{0}' to path '{1}'", hit.Registration.Cookie, fullPathForCookie);
			try
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(fullPathForCookie, ConditionalRegistrationLog.writerSettings))
				{
					hit.GetXmlResults().WriteTo(xmlWriter);
					xmlWriter.Flush();
				}
			}
			catch (Exception arg)
			{
				ExTraceGlobals.DiagnosticHandlersTracer.TraceDebug<string, Exception>(0L, "[ConditionalRegistrationLog.Log] Caught exception trying to save hit for cookie: {0}.  Exception: {1}", hit.Registration.Cookie, arg);
			}
			return fullPathForCookie;
		}

		public static void LogFailedHydration(XElement childNode, Exception exception)
		{
			if (ConditionalRegistrationLog.OnFailedHydration != null)
			{
				ConditionalRegistrationLog.OnFailedHydration(childNode, exception);
				return;
			}
			ConditionalRegistrationLog.EventLog.LogEvent(ApplicationLogicEventLogConstants.Tuple_PersistentHandlerRegistrationFailed, null, new object[]
			{
				childNode.ToString(SaveOptions.None),
				ConditionalRegistrationLog.ProtocolName,
				exception.ToString()
			});
		}

		private static void LimitFileCount(string path)
		{
			string directoryName = Path.GetDirectoryName(path);
			DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
			string extension = Path.GetExtension(path);
			FileInfo[] files = directoryInfo.GetFiles("*" + extension);
			int num = files.Length - ConditionalRegistrationLog.MaxFiles.Value + 1;
			if (num > 0)
			{
				ExTraceGlobals.DiagnosticHandlersTracer.TraceDebug<int, string>(0L, "[ConditionalRegistrationLog.LimitFileCount] Deleting {0} old files from directory: {1}", num, directoryName);
				Array.Sort<FileInfo>(files, (FileInfo file1, FileInfo file2) => file1.CreationTimeUtc.CompareTo(file2.CreationTimeUtc));
				for (int i = 0; i < num; i++)
				{
					try
					{
						files[i].Delete();
					}
					catch (Exception ex)
					{
						ExTraceGlobals.DiagnosticHandlersTracer.TraceError<string, string>(0L, "[ConditionalRegistrationLog.LimitFileCount] Failed to delete file '{0}' with exception '{1}'.", files[i].FullName, ex.ToString());
					}
				}
			}
		}

		public static List<ConditionalRegistrationLog.ConditionalRegistrationHitMetadata> GetHitsMetadata(string userIdentity = "")
		{
			List<ConditionalRegistrationLog.ConditionalRegistrationHitMetadata> list = null;
			string text = Path.Combine(ConditionalRegistrationLog.LoggingDirectory, userIdentity);
			if (!string.IsNullOrEmpty(userIdentity))
			{
				list = ConditionalRegistrationLog.GetHitsMetadataForUser(text, userIdentity);
			}
			else
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(text);
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				foreach (DirectoryInfo directoryInfo2 in directories)
				{
					List<ConditionalRegistrationLog.ConditionalRegistrationHitMetadata> hitsMetadataForUser = ConditionalRegistrationLog.GetHitsMetadataForUser(directoryInfo2.FullName, directoryInfo2.Name);
					if (hitsMetadataForUser != null)
					{
						if (list == null)
						{
							list = new List<ConditionalRegistrationLog.ConditionalRegistrationHitMetadata>();
						}
						list.AddRange(hitsMetadataForUser);
					}
				}
			}
			return list;
		}

		public static List<ConditionalRegistrationLog.ConditionalRegistrationHitMetadata> GetHitsMetadataForUser(string loggingDirectory, string userIdentity)
		{
			List<ConditionalRegistrationLog.ConditionalRegistrationHitMetadata> list = null;
			DirectoryInfo directoryInfo = new DirectoryInfo(loggingDirectory);
			if (directoryInfo.Exists)
			{
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				foreach (DirectoryInfo directoryInfo2 in directories)
				{
					if (list == null)
					{
						list = new List<ConditionalRegistrationLog.ConditionalRegistrationHitMetadata>();
					}
					list.Add(new ConditionalRegistrationLog.ConditionalRegistrationHitMetadata(directoryInfo2.Name, userIdentity));
				}
			}
			return list;
		}

		public static ConditionalRegistrationLog.ConditionalRegistrationHitMetadata GetHitsForCookie(string userIdentity, string cookie)
		{
			string path = Path.Combine(ConditionalRegistrationLog.LoggingDirectory, userIdentity, cookie);
			if (Directory.Exists(path))
			{
				return new ConditionalRegistrationLog.ConditionalRegistrationHitMetadata(cookie, userIdentity);
			}
			return null;
		}

		public static string GetFullPathForCookie(string userIdentity, string cookie)
		{
			return Path.Combine(ConditionalRegistrationLog.GetOrCreateLogFilePathForCookie(userIdentity, cookie, true), string.Format("{0}_{1}.xml", DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss.fff"), Thread.CurrentThread.ManagedThreadId));
		}

		public static string GetConditionalRegistrationsDirectory()
		{
			string text = Path.Combine(ConditionalRegistrationLog.LoggingDirectory, "ConditionalRegistrations");
			string result;
			try
			{
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				result = text;
			}
			catch (Exception arg)
			{
				ExTraceGlobals.DiagnosticHandlersTracer.TraceError<string, Exception>(0L, "[ConditionalRegistrationLog.GetConditionalRegistrationsDirectory] Could not create folder '{0}' due to exception: {1}", text, arg);
				result = null;
			}
			return result;
		}

		private static void CreateLogFilePath()
		{
			try
			{
				if (!Directory.Exists(ConditionalRegistrationLog.LoggingDirectory))
				{
					Directory.CreateDirectory(ConditionalRegistrationLog.LoggingDirectory);
				}
			}
			catch (Exception arg)
			{
				ExTraceGlobals.DiagnosticHandlersTracer.TraceError<string, Exception>(0L, "[ConditionalRegistrationLog.CreateLogFilePath] Could not create folder '{0}' due to exception: {1}", ConditionalRegistrationLog.LoggingDirectory, arg);
			}
		}

		public static string GetOrCreateLogFilePathForCookie(string userIdentity, string cookie, bool create)
		{
			string text = Path.Combine(ConditionalRegistrationLog.LoggingDirectory, userIdentity, cookie);
			if (create)
			{
				try
				{
					if (!Directory.Exists(text))
					{
						Directory.CreateDirectory(text);
					}
				}
				catch (Exception arg)
				{
					ExTraceGlobals.DiagnosticHandlersTracer.TraceError<string, Exception>(0L, "[ConditionalRegistrationLog.CreateLogFilePathForCookie] Could not create folder '{0}' due to exception: {1}", text, arg);
					return text;
				}
				return text;
			}
			return text;
		}

		private const string ConditionalRegistrationDirectory = "ConditionalRegistrations";

		private static readonly string LoggingRootPath;

		private static string LoggingDirectory;

		private static readonly XmlWriterSettings writerSettings;

		private static readonly XmlReaderSettings readerSettings;

		private static string protocolName;

		private static IntAppSettingsEntry MaxFiles = new IntAppSettingsEntry("MaxDiagnosticFiles", 100, ExTraceGlobals.DiagnosticHandlersTracer);

		public static readonly ExEventLog EventLog = new ExEventLog(ExTraceGlobals.DiagnosticHandlersTracer.Category, "MSExchangeApplicationLogic");

		public class ConditionalRegistrationHitMetadata
		{
			public ConditionalRegistrationHitMetadata(string cookie, string user)
			{
				this.Cookie = cookie;
				this.HitFiles = ConditionalRegistrationLog.ConditionalRegistrationHitMetadata.GetHits(cookie, user);
			}

			public string Cookie { get; private set; }

			public FileInfo[] HitFiles { get; private set; }

			private static FileInfo[] GetHits(string cookie, string user)
			{
				string orCreateLogFilePathForCookie = ConditionalRegistrationLog.GetOrCreateLogFilePathForCookie(user, cookie, false);
				DirectoryInfo directoryInfo = new DirectoryInfo(orCreateLogFilePathForCookie);
				if (!directoryInfo.Exists)
				{
					return null;
				}
				return directoryInfo.GetFiles("*.xml", SearchOption.TopDirectoryOnly);
			}
		}
	}
}
