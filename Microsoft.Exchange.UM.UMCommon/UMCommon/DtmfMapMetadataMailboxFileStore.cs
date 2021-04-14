using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal sealed class DtmfMapMetadataMailboxFileStore : DirectoryMailboxFileStore
	{
		private DtmfMapMetadataMailboxFileStore(OrganizationId orgId, Guid mbxGuid) : base(orgId, mbxGuid, "DtmfMapMetadata")
		{
		}

		public static DtmfMapMetadataMailboxFileStore FromMailboxGuid(OrganizationId orgId, Guid mbxGuid)
		{
			DtmfMapMetadataMailboxFileStore result = null;
			try
			{
				result = new DtmfMapMetadataMailboxFileStore(orgId, mbxGuid);
			}
			catch (Exception ex)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UnableToAccessOrganizationMailbox, null, new object[]
				{
					orgId,
					mbxGuid,
					CommonUtil.ToEventLogString(ex)
				});
				if (!DtmfMapMetadataMailboxFileStore.IsExpectedException(ex))
				{
					throw;
				}
			}
			return result;
		}

		public void UploadMetadata(string filePath, string fileNamePrefix, string version)
		{
			ValidateArgument.NotNullOrEmpty(filePath, "filePath");
			ValidateArgument.NotNullOrEmpty(fileNamePrefix, "fileNamePrefix");
			ValidateArgument.NotNullOrEmpty(version, "version");
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "DtmfMapMetadataMailboxFileStore.UploadMetadata - filePath='{0}', fileNamePrefix='{1}', version='{2}'", new object[]
			{
				filePath,
				fileNamePrefix,
				version
			});
			string text = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[]
			{
				fileNamePrefix,
				version
			});
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "DtmfMapMetadataMailboxFileStore.UploadMetadata - fileSetId='{0}'", new object[]
			{
				text
			});
			try
			{
				using (MailboxSession mailboxSession = DirectoryMailboxFileStore.GetMailboxSession(base.OrgId, base.MailboxGuid))
				{
					base.UploadFile(filePath, text, mailboxSession);
				}
			}
			catch (Exception ex)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UploadDtmfMapMetadataFailed, null, new object[]
				{
					base.OrgId,
					filePath,
					CommonUtil.ToEventLogString(ex)
				});
				if (!DtmfMapMetadataMailboxFileStore.IsExpectedException(ex))
				{
					throw;
				}
			}
		}

		public bool DownloadMetadata(string destinationFilePath, string fileNamePrefix, string version, DateTime threshold)
		{
			ValidateArgument.NotNullOrEmpty(destinationFilePath, "destinationFilePath");
			ValidateArgument.NotNullOrEmpty(fileNamePrefix, "fileNamePrefix");
			ValidateArgument.NotNullOrEmpty(version, "version");
			bool result = false;
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "DtmfMapMetadataMailboxFileStore.DownloadMetadata - destinationFilePath='{0}', fileNamePrefix='{1}', version='{2}', threshold='{3}'", new object[]
			{
				destinationFilePath,
				fileNamePrefix,
				version,
				threshold
			});
			string text = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[]
			{
				fileNamePrefix,
				version
			});
			try
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "DtmfMapMetadataMailboxFileStore.DownloadMetadata - fileSetId='{0}'", new object[]
				{
					text
				});
				string text2 = null;
				using (MailboxSession mailboxSession = DirectoryMailboxFileStore.GetMailboxSession(base.OrgId, base.MailboxGuid))
				{
					text2 = base.DownloadLatestFile(text, threshold, mailboxSession);
				}
				if (text2 != null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "DtmfMapMetadataMailboxFileStore.DownloadMetadata - Copying downloaded metadata '{0}' to '{1}'", new object[]
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
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_DownloadDtmfMapMetadataFailed, null, new object[]
				{
					base.OrgId,
					destinationFilePath,
					CommonUtil.ToEventLogString(ex)
				});
				if (!DtmfMapMetadataMailboxFileStore.IsExpectedException(ex))
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

		private const string FileSetIdFormat = "{0}_{1}";

		internal const string FolderName = "DtmfMapMetadata";
	}
}
