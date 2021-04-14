using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal abstract class DirectoryMailboxFileStore
	{
		public DirectoryMailboxFileStore(OrganizationId orgId, Guid mbxGuid, string folderName)
		{
			ValidateArgument.NotNull(orgId, "orgId");
			ValidateArgument.NotNullOrEmpty(folderName, "folderName");
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "DirectoryMailboxFileStore constructor - orgId='{0}', mbxGuid='{1}', folderName='{2}'", new object[]
			{
				orgId,
				mbxGuid,
				folderName
			});
			this.mbxFileStore = new MailboxFileStore(folderName);
			this.OrgId = orgId;
			this.MailboxGuid = mbxGuid;
		}

		private protected OrganizationId OrgId { protected get; private set; }

		private protected Guid MailboxGuid { protected get; private set; }

		public static MailboxSession GetMailboxSession(OrganizationId orgId, Guid mbxGuid)
		{
			ADSessionSettings adSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), orgId, null, false);
			ExchangePrincipal mailboxOwner = ExchangePrincipal.FromMailboxGuid(adSettings, mbxGuid, null);
			return MailboxSession.OpenAsAdmin(mailboxOwner, CultureInfo.InvariantCulture, "Client=UM;Action=DirectoryProcessor");
		}

		protected void UploadFile(string filePath, string fileSetId, MailboxSession mbxSession)
		{
			ValidateArgument.NotNullOrEmpty(filePath, "filePath");
			ValidateArgument.NotNullOrEmpty(fileSetId, "fileSetId");
			ValidateArgument.NotNull(mbxSession, "mbxSession");
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "DirectoryMailboxFileStore.UploadFile - filePath='{0}', fileSetId='{1}'", new object[]
			{
				filePath,
				fileSetId
			});
			string[] sources = new string[]
			{
				filePath
			};
			this.mbxFileStore.Upload(sources, fileSetId, mbxSession);
		}

		protected string DownloadLatestFile(string fileSetId, DateTime threshold, MailboxSession mbxSession)
		{
			ValidateArgument.NotNullOrEmpty(fileSetId, "fileSetId");
			ValidateArgument.NotNull(mbxSession, "mbxSession");
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "DirectoryMailboxFileStore.DownloadLatestFile - fileSetId='{0}', threshold='{1}'", new object[]
			{
				fileSetId,
				threshold
			});
			FileSetItem current = this.mbxFileStore.GetCurrent(fileSetId, mbxSession);
			string text = null;
			if (current != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "DirectoryMailboxFileStore.DownloadLatestFile - Found fileSetId='{0}', time='{1}'", new object[]
				{
					fileSetId,
					current.Time.UniversalTime
				});
				if (current.Time.UniversalTime > threshold)
				{
					List<string> list = this.mbxFileStore.Download(current, mbxSession);
					if (list.Count > 0)
					{
						text = list[0];
						CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "DirectoryMailboxFileStore.DownloadLatestFile - fileSetId='{0}', filePath='{1}'", new object[]
						{
							fileSetId,
							text
						});
					}
				}
			}
			return text;
		}

		protected void DeleteFileFromMailbox(string fileSetId, MailboxSession mailboxSession)
		{
			ValidateArgument.NotNullOrEmpty(fileSetId, "fileSetId");
			ValidateArgument.NotNull(mailboxSession, "mailboxSession");
			ExTraceGlobals.UMGrammarGeneratorTracer.TraceDebug<string>((long)this.GetHashCode(), "DirectoryMailboxFileStore.DeleteFileFromMailbox - fileSetId='{0}'", fileSetId);
			this.mbxFileStore.RemoveAll(fileSetId, mailboxSession);
		}

		protected void DeleteFile(string filePath)
		{
			this.HandleDeleteExceptions(filePath, delegate(string name)
			{
				File.Delete(name);
			});
		}

		protected void DeleteFolder(string folderPath)
		{
			this.HandleDeleteExceptions(folderPath, delegate(string name)
			{
				Directory.Delete(name, true);
			});
		}

		private void HandleDeleteExceptions(string name, Action<string> deleteAction)
		{
			Exception ex = null;
			try
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, 0, "Deleting '{0}'", new object[]
				{
					name
				});
				deleteAction(name);
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, 0, "Failed to delete '{0}'. Exception : '{1}'", new object[]
				{
					name,
					ex
				});
			}
		}

		internal const string MailboxClientString = "Client=UM;Action=DirectoryProcessor";

		private MailboxFileStore mbxFileStore;
	}
}
