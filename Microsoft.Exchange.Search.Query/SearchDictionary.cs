using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Microsoft.Ceres.NlpBase.Dictionaries;
using Microsoft.Ceres.NlpBase.DictionaryInterface;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.Mdb;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Query
{
	internal static class SearchDictionary
	{
		public static XElement DiagnosticsDictionaryRetrieval(DiagnosableParameters parameters, string searchDictionaryName, ISearchServiceConfig config, MdbInfo mdbInfo, Guid mailboxGuid)
		{
			Exception ex = null;
			string content = string.Empty;
			try
			{
				ExchangePrincipal exchangePrincipal = XsoUtil.GetExchangePrincipal(config, mdbInfo, mailboxGuid);
				using (MailboxSession mailboxSession = (MailboxSession)XsoUtil.GetStoreSession(config, exchangePrincipal, false, "Client=CI;Action=Diagnostics"))
				{
					using (UserConfiguration searchDictionaryItem = SearchDictionary.GetSearchDictionaryItem(mailboxSession, searchDictionaryName))
					{
						using (Stream stream = searchDictionaryItem.GetStream())
						{
							if (stream == null || stream.Length == 0L)
							{
								return new XElement("Get-Dictionary", "Does Not Exist");
							}
							SearchDictionary.diagnosticsSession.LogDictionaryInfo(DiagnosticsLoggingTag.Informational, 5, Guid.Empty, mailboxSession.MdbGuid, mailboxSession.MailboxGuid, "Dictionary Diagnostics: User: {0}. AllowRestrictedData: {1}. Retrieving the {2} UserConfiguration message.", new object[]
							{
								parameters.UserIdentity,
								parameters.AllowRestrictedData,
								searchDictionaryName
							});
							ExDateTime lastModifiedTime = searchDictionaryItem.LastModifiedTime;
							long length = stream.Length;
							if (searchDictionaryName != null)
							{
								if (!(searchDictionaryName == "Search.TopN"))
								{
									if (!(searchDictionaryName == "Search.QueryHistoryInput"))
									{
										goto IL_2F1;
									}
									QueryHistoryInputDictionary queryHistoryInputDictionary = new QueryHistoryInputDictionary();
									queryHistoryInputDictionary.InitializeFrom(stream);
									StringBuilder stringBuilder = new StringBuilder();
									stringBuilder.AppendLine();
									stringBuilder.AppendLine(string.Format("Dictionary Name: {0}", "Search.QueryHistoryInput"));
									stringBuilder.AppendLine(string.Format("Last Modified: {0}", lastModifiedTime));
									stringBuilder.AppendLine(string.Format("Size in bytes: {0}", length));
									stringBuilder.AppendLine();
									if (parameters.AllowRestrictedData)
									{
										using (IEnumerator<QueryHistoryInputDictionaryEntry> enumerator = queryHistoryInputDictionary.GetEnumerator())
										{
											while (enumerator.MoveNext())
											{
												QueryHistoryInputDictionaryEntry queryHistoryInputDictionaryEntry = enumerator.Current;
												stringBuilder.AppendLine(string.Format("Query: '{0}', Rank: {1}, LastUsed: {2}", queryHistoryInputDictionaryEntry.Query, queryHistoryInputDictionaryEntry.Rank, new DateTime(queryHistoryInputDictionaryEntry.LastUsed)));
											}
											goto IL_2E7;
										}
									}
									stringBuilder.AppendLine("AllowRestrictedData is FALSE. To view dictionary content request elevated permissions for CustomerDataAccess.");
									IL_2E7:
									content = stringBuilder.ToString();
								}
								else
								{
									Stream stream2 = SearchDictionary.InitializeFrom(stream, 1);
									StringBuilder stringBuilder = new StringBuilder();
									stringBuilder.AppendLine();
									stringBuilder.AppendLine(string.Format("Dictionary Name: {0}", "Search.TopN"));
									stringBuilder.AppendLine(string.Format("Last Modified: {0}", lastModifiedTime));
									stringBuilder.AppendLine(string.Format("Size in bytes: {0}", length));
									stringBuilder.AppendLine();
									if (parameters.AllowRestrictedData)
									{
										IStringDictionary stringDictionary = DictionaryLoader.Instance.Get(stream2);
										using (IEnumerator<KeyValuePair<string, IEnumerable<DictionaryMatch>>> enumerator2 = stringDictionary.GetEnumerator())
										{
											while (enumerator2.MoveNext())
											{
												KeyValuePair<string, IEnumerable<DictionaryMatch>> keyValuePair = enumerator2.Current;
												foreach (DictionaryMatch arg in keyValuePair.Value)
												{
													stringBuilder.AppendLine(string.Format("Key: {0}, {1}", keyValuePair.Key, arg));
												}
											}
											goto IL_1F3;
										}
									}
									stringBuilder.AppendLine("AllowRestrictedData is FALSE. To view dictionary content request elevated permissions for CustomerDataAccess.");
									IL_1F3:
									content = stringBuilder.ToString();
								}
								goto IL_31D;
							}
							IL_2F1:
							return new XElement("Error", string.Format("The requested DictionaryStream is not supported: {0}", searchDictionaryName));
						}
						IL_31D:;
					}
				}
			}
			catch (ObjectNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (MailboxInfoStaleException ex3)
			{
				ex = ex3;
			}
			catch (MailboxUnavailableException ex4)
			{
				ex = ex4;
			}
			catch (AccountDisabledException ex5)
			{
				ex = ex5;
			}
			catch (WrongServerException ex6)
			{
				ex = ex6;
			}
			if (ex != null)
			{
				return new XElement("Error", string.Format("Failed to get the DictionaryStream from Store: {0}", ex));
			}
			return new XElement("Get-Dictionary", content);
		}

		public static XElement DiagnosticsDictionaryReset(DiagnosableParameters parameters, string searchDictionaryName, ISearchServiceConfig config, MdbInfo mdbInfo, Guid mailboxGuid)
		{
			Exception ex = null;
			string empty = string.Empty;
			try
			{
				SearchDictionary.diagnosticsSession.LogDictionaryInfo(DiagnosticsLoggingTag.Informational, 6, Guid.Empty, mdbInfo.Guid, mailboxGuid, "Dictionary Diagnostics: User: {0}. Requesting delete of the {1} UserConfiguration message.", new object[]
				{
					parameters.UserIdentity,
					searchDictionaryName
				});
				ExchangePrincipal exchangePrincipal = XsoUtil.GetExchangePrincipal(config, mdbInfo, mailboxGuid);
				using (MailboxSession mailboxSession = (MailboxSession)XsoUtil.GetStoreSession(config, exchangePrincipal, false, "Client=CI;Action=Diagnostics"))
				{
					SearchDictionary.ResetDictionary(mailboxSession, searchDictionaryName, UserConfigurationTypes.Stream, true, false);
				}
			}
			catch (ObjectNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (MailboxInfoStaleException ex3)
			{
				ex = ex3;
			}
			catch (MailboxUnavailableException ex4)
			{
				ex = ex4;
			}
			catch (AccountDisabledException ex5)
			{
				ex = ex5;
			}
			catch (WrongServerException ex6)
			{
				ex = ex6;
			}
			if (ex != null)
			{
				return new XElement("Error", string.Format("Failed to get the DictionaryStream from Store: {0}", ex));
			}
			return new XElement("Reset-Dictionary", "Complete");
		}

		public static UserConfiguration GetSearchDictionaryItem(MailboxSession session, string searchDictionaryName)
		{
			InstantSearch.ThrowOnNullArgument(session, "session");
			UserConfiguration userConfiguration = null;
			bool deleteExisting = false;
			try
			{
				userConfiguration = session.UserConfigurationManager.GetMailboxConfiguration(searchDictionaryName, UserConfigurationTypes.Stream);
			}
			catch (ObjectNotFoundException)
			{
				SearchDictionary.diagnosticsSession.TraceDebug<string, Guid>("No existing {0} UserConfiguration message for MailboxGuid: {1}.", searchDictionaryName, session.MailboxGuid);
			}
			catch (CorruptDataException)
			{
				SearchDictionary.diagnosticsSession.TraceDebug<string, Guid>("Corrupt {0} UserConfiguration message for MailboxGuid: {1}.", searchDictionaryName, session.MailboxGuid);
				deleteExisting = true;
			}
			if (userConfiguration == null)
			{
				userConfiguration = SearchDictionary.ResetDictionary(session, searchDictionaryName, UserConfigurationTypes.Stream, deleteExisting, true);
			}
			return userConfiguration;
		}

		public static UserConfiguration ResetDictionary(MailboxSession session, string searchDictionaryName, UserConfigurationTypes userConfigType, bool deleteExisting, bool createNew)
		{
			InstantSearch.ThrowOnNullArgument(session, "session");
			if (deleteExisting)
			{
				try
				{
					SearchDictionary.diagnosticsSession.LogDictionaryInfo(DiagnosticsLoggingTag.Informational, 3, Guid.Empty, session.MdbGuid, session.MailboxGuid, "Deleting the {0} UserConfiguration message.", new object[]
					{
						searchDictionaryName
					});
					session.UserConfigurationManager.DeleteMailboxConfigurations(new string[]
					{
						searchDictionaryName
					});
				}
				catch (ObjectNotFoundException)
				{
				}
			}
			UserConfiguration userConfiguration = null;
			if (createNew)
			{
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					SearchDictionary.diagnosticsSession.LogDictionaryInfo(DiagnosticsLoggingTag.Informational, 4, Guid.Empty, session.MdbGuid, session.MailboxGuid, "Creating the {0} UserConfiguration message.", new object[]
					{
						searchDictionaryName
					});
					userConfiguration = session.UserConfigurationManager.CreateMailboxConfiguration(searchDictionaryName, userConfigType);
					disposeGuard.Add<UserConfiguration>(userConfiguration);
					userConfiguration.Save();
					disposeGuard.Success();
				}
			}
			return userConfiguration;
		}

		public static Stream InitializeFrom(Stream storeStream, int currentVersion)
		{
			if (storeStream == null)
			{
				SearchDictionary.diagnosticsSession.TraceDebug("The passed in stream is null.", new object[0]);
				return null;
			}
			if (storeStream.Length > 0L)
			{
				using (BinaryReader binaryReader = new BinaryReader(storeStream))
				{
					int num;
					try
					{
						num = binaryReader.ReadInt32();
					}
					catch (EndOfStreamException)
					{
						SearchDictionary.diagnosticsSession.TraceDebug("The dictionary stream content is corrupt. EndOfStreamException encountered while trying to read the version.", new object[0]);
						return null;
					}
					if (num != currentVersion)
					{
						SearchDictionary.diagnosticsSession.TraceDebug("The version of the Dictionary is not correct and it should be rebuilt with the current version.", new object[0]);
						return null;
					}
					using (DisposeGuard disposeGuard = default(DisposeGuard))
					{
						LohFriendlyStream lohFriendlyStream = new LohFriendlyStream((int)storeStream.Length);
						disposeGuard.Add<LohFriendlyStream>(lohFriendlyStream);
						storeStream.CopyTo(lohFriendlyStream);
						lohFriendlyStream.Position = 0L;
						disposeGuard.Success();
						return lohFriendlyStream;
					}
				}
			}
			SearchDictionary.diagnosticsSession.TraceDebug("The passed in stream has a length of 0.", new object[0]);
			return null;
		}

		public static void SerializeTo(Stream storeStream, Stream contentStream, int version)
		{
			InstantSearch.ThrowOnNullArgument(storeStream, "storeStream");
			InstantSearch.ThrowOnNullArgument(contentStream, "contentStream");
			using (BinaryWriter binaryWriter = new BinaryWriter(storeStream))
			{
				binaryWriter.Write(version);
				contentStream.CopyTo(storeStream);
				storeStream.SetLength(storeStream.Position);
			}
		}

		private const string DiagnosticClientInfo = "Client=CI;Action=Diagnostics";

		private static IDiagnosticsSession diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("SearchDictionary", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.SearchDictionaryTracer, 0L);
	}
}
