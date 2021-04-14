using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal static class GrammarFileDistributionShare
	{
		public static string GetOrgPath(OrganizationId orgId)
		{
			ValidateArgument.NotNull(orgId, "orgId");
			string tenantFolderName = GrammarFileDistributionShare.GetTenantFolderName(orgId);
			return Path.Combine(GrammarFileDistributionShare.GetDirectoryProcessorFolderPath(), tenantFolderName);
		}

		public static string GetGrammarFolderPath(OrganizationId orgId, Guid mbxGuid)
		{
			return Path.Combine(GrammarFileDistributionShare.GetOrgPath(orgId), mbxGuid.ToString(), "SpeechGrammars");
		}

		public static string GetDtmfMapFolderPath(OrganizationId orgId, Guid mbxGuid)
		{
			return Path.Combine(GrammarFileDistributionShare.GetOrgPath(orgId), mbxGuid.ToString(), "DtmfMap");
		}

		public static string GetGrammarManifestPath(OrganizationId orgId, Guid mbxGuid)
		{
			string grammarFolderPath = GrammarFileDistributionShare.GetGrammarFolderPath(orgId, mbxGuid);
			return Path.Combine(grammarFolderPath, "grammars.xml");
		}

		public static string GetGrammarGenerationRunFolderPath(OrganizationId orgId, Guid mbxGuid, Guid runId)
		{
			string grammarFolderPath = GrammarFileDistributionShare.GetGrammarFolderPath(orgId, mbxGuid);
			return Path.Combine(grammarFolderPath, runId.ToString());
		}

		public static string GetGrammarFileFolderPath(OrganizationId orgId, Guid mbxGuid, Guid runId, CultureInfo culture)
		{
			string grammarGenerationRunFolderPath = GrammarFileDistributionShare.GetGrammarGenerationRunFolderPath(orgId, mbxGuid, runId);
			return Path.Combine(grammarGenerationRunFolderPath, culture.Name);
		}

		public static Guid GetMailboxGuid(OrganizationId orgId)
		{
			ValidateArgument.NotNull(orgId, "orgId");
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, null, "GrammarFileDistributionShare.GetMailboxGuid orgId='{0}'", new object[]
			{
				orgId
			});
			Guid guid = Guid.Empty;
			string orgPath = GrammarFileDistributionShare.GetOrgPath(orgId);
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, null, "GrammarFileDistributionShare.GetMailboxGuid orgPath='{0}'", new object[]
			{
				orgPath
			});
			string[] array = null;
			if (Directory.Exists(orgPath))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, null, "GrammarFileDistributionShare.GetMailboxGuid orgPath='{0}' exists", new object[]
				{
					orgPath
				});
				array = Directory.GetDirectories(orgPath);
			}
			if (array != null && array.Length > 0)
			{
				if (array.Length == 1)
				{
					string text = array[0];
					CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, null, "GrammarFileDistributionShare.GetMailboxGuid - only dir='{0}'", new object[]
					{
						text
					});
					guid = GrammarFileDistributionShare.ExtractFolderGuid(text);
				}
				else
				{
					DateTime t = DateTime.MinValue;
					foreach (string text2 in array)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, null, "GrammarFileDistributionShare.GetMailboxGuid dir='{0}'", new object[]
						{
							text2
						});
						Guid guid2 = GrammarFileDistributionShare.ExtractFolderGuid(text2);
						string grammarManifestPath = GrammarFileDistributionShare.GetGrammarManifestPath(orgId, guid2);
						if (File.Exists(grammarManifestPath))
						{
							DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(grammarManifestPath);
							CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, null, "GrammarFileDistributionShare.GetMailboxGuid manifestPath='{0}', fileLastWriteTimeUtc='{1}'", new object[]
							{
								grammarManifestPath,
								lastWriteTimeUtc
							});
							if (lastWriteTimeUtc > t)
							{
								guid = guid2;
								t = lastWriteTimeUtc;
							}
						}
					}
				}
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, null, "GrammarFileDistributionShare.GetMailboxGuid mailboxGuid='{0}'", new object[]
			{
				guid
			});
			return guid;
		}

		public static bool SpeechGrammarsFolderExists(OrganizationId orgId)
		{
			bool result = false;
			string orgPath = GrammarFileDistributionShare.GetOrgPath(orgId);
			Exception ex = null;
			try
			{
				string[] directories = Directory.GetDirectories(orgPath, "SpeechGrammars", SearchOption.AllDirectories);
				result = (directories.Length > 0);
			}
			catch (UnauthorizedAccessException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			finally
			{
				if (ex != null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, null, "GrammarFileDistributionShare.SpeechGrammarsFolderExists ex='{0}'", new object[]
					{
						ex
					});
				}
			}
			return result;
		}

		public static void CreateDirectoryProcessorFolder()
		{
			string directoryProcessorFolderPath = GrammarFileDistributionShare.GetDirectoryProcessorFolderPath();
			GrammarFileDistributionShare.CreateDirectoryProcessorDirectory(directoryProcessorFolderPath);
		}

		private static string GetTenantFolderName(OrganizationId orgId)
		{
			ValidateArgument.NotNull(orgId, "orgId");
			string result;
			if (orgId.OrganizationalUnit != null)
			{
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(orgId);
				string text = iadsystemConfigurationLookup.GetExternalDirectoryOrganizationId().ToString();
				result = text;
			}
			else
			{
				result = "Enterprise";
			}
			return result;
		}

		private static string GetDirectoryProcessorFolderPath()
		{
			return Path.Combine(Utils.GetExchangeDirectory(), "UnifiedMessaging\\DirectoryProcessor");
		}

		private static Guid ExtractFolderGuid(string dirPath)
		{
			Guid empty = Guid.Empty;
			string[] array = dirPath.Split(new char[]
			{
				Path.DirectorySeparatorChar
			});
			if (!Guid.TryParse(array[array.Length - 1], out empty))
			{
				empty = Guid.Empty;
			}
			return empty;
		}

		private static bool CreateDirectoryProcessorDirectory(string path)
		{
			return Utils.TryDiskOperation(delegate
			{
				Directory.CreateDirectory(path);
				CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, null, "GrammarFileDistributionShare.CreateDirectoryProcessorDirectory succeeded: Path='{0}'", new object[]
				{
					path
				});
			}, delegate(Exception e)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, null, "GrammarFileDistributionShare.CreateDirectoryProcessorDirectory failed: Path='{0}', Error='{1}'", new object[]
				{
					path,
					e
				});
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UnableToCreateDirectoryProcessorDirectory, null, new object[]
				{
					path,
					CommonUtil.ToEventLogString(e)
				});
			});
		}

		private const string DirectoryProcessorFolder = "UnifiedMessaging\\DirectoryProcessor";

		private const string SpeechGrammarsFolderName = "SpeechGrammars";

		private const string DtmfMapFolderName = "DtmfMap";

		private const string EnterpriseGrammarFolderName = "Enterprise";

		private const string GrammarManifestFileName = "grammars.xml";
	}
}
