using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal sealed class NormalizationCacheMailboxFileStore : DirectoryMailboxFileStore, INormalizationCacheFileStore
	{
		private NormalizationCacheMailboxFileStore(OrganizationId orgId, Guid mbxGuid) : base(orgId, mbxGuid, "SpeechNormalizationCaches")
		{
		}

		public static NormalizationCacheMailboxFileStore FromMailboxGuid(OrganizationId orgId, Guid mbxGuid)
		{
			NormalizationCacheMailboxFileStore result = null;
			try
			{
				result = new NormalizationCacheMailboxFileStore(orgId, mbxGuid);
			}
			catch (Exception ex)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UnableToAccessOrganizationMailbox, null, new object[]
				{
					orgId,
					mbxGuid,
					CommonUtil.ToEventLogString(ex)
				});
				if (!NormalizationCacheMailboxFileStore.IsExpectedException(ex))
				{
					throw;
				}
			}
			return result;
		}

		public bool UploadCache(string filePath, string fileNamePrefix, CultureInfo culture, string version, MailboxSession mbxSession)
		{
			ValidateArgument.NotNullOrEmpty(filePath, "filePath");
			ValidateArgument.NotNullOrEmpty(fileNamePrefix, "fileNamePrefix");
			ValidateArgument.NotNull(culture, "culture");
			ValidateArgument.NotNullOrEmpty(version, "version");
			ValidateArgument.NotNull(mbxSession, "mbxSession");
			bool result = false;
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "NormalizationCacheMailboxFileStore.UploadCache - filePath='{0}', fileNamePrefix='{1}', culture='{2}', version='{3}'", new object[]
			{
				filePath,
				fileNamePrefix,
				culture,
				version
			});
			string text = string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}", new object[]
			{
				fileNamePrefix,
				culture.Name,
				version
			});
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "NormalizationCacheMailboxFileStore.UploadCache - fileSetId='{0}'", new object[]
			{
				text
			});
			try
			{
				base.UploadFile(filePath, text, mbxSession);
				result = true;
			}
			catch (Exception ex)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UploadNormalizationCacheFailed, null, new object[]
				{
					base.OrgId,
					filePath,
					culture,
					CommonUtil.ToEventLogString(ex)
				});
				if (!NormalizationCacheMailboxFileStore.IsExpectedException(ex))
				{
					throw;
				}
			}
			return result;
		}

		public bool DownloadCache(string destinationFilePath, string fileNamePrefix, CultureInfo culture, string version)
		{
			ValidateArgument.NotNullOrEmpty(destinationFilePath, "destinationFilePath");
			ValidateArgument.NotNullOrEmpty(fileNamePrefix, "fileNamePrefix");
			ValidateArgument.NotNull(culture, "culture");
			ValidateArgument.NotNullOrEmpty(version, "version");
			bool result = false;
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "NormalizationCacheMailboxFileStore.DownloadCache - destinationFilePath='{0}', fileNamePrefix='{1}', culture='{2}', version='{3}'", new object[]
			{
				destinationFilePath,
				fileNamePrefix,
				culture,
				version
			});
			string text = string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}", new object[]
			{
				fileNamePrefix,
				culture.Name,
				version
			});
			try
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "NormalizationCacheMailboxFileStore.DownloadCache - fileSetId='{0}'", new object[]
				{
					text
				});
				string text2 = null;
				using (MailboxSession mailboxSession = DirectoryMailboxFileStore.GetMailboxSession(base.OrgId, base.MailboxGuid))
				{
					text2 = base.DownloadLatestFile(text, DateTime.MinValue, mailboxSession);
				}
				if (text2 != null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "NormalizationCacheMailboxFileStore.DownloadCache - Copying downloaded cache '{0}' to '{1}'", new object[]
					{
						text2,
						destinationFilePath
					});
					string directoryName = Path.GetDirectoryName(destinationFilePath);
					Directory.CreateDirectory(directoryName);
					File.Copy(text2, destinationFilePath, true);
					File.Delete(text2);
					result = true;
				}
			}
			catch (Exception ex)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_DownloadNormalizationCacheFailed, null, new object[]
				{
					base.OrgId,
					destinationFilePath,
					culture,
					CommonUtil.ToEventLogString(ex)
				});
				if (!NormalizationCacheMailboxFileStore.IsExpectedException(ex))
				{
					throw;
				}
			}
			return result;
		}

		private static bool IsExpectedException(Exception e)
		{
			return e is IOException || e is UnauthorizedAccessException || e is LocalizedException;
		}

		private const string FileSetIdFormat = "{0}_{1}_{2}";

		internal const string FolderName = "SpeechNormalizationCaches";
	}
}
