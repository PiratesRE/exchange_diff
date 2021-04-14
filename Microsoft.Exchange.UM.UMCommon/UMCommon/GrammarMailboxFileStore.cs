using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal sealed class GrammarMailboxFileStore : DirectoryMailboxFileStore
	{
		private GrammarMailboxFileStore(OrganizationId orgId, Guid mbxGuid) : base(orgId, mbxGuid, "SpeechGrammars")
		{
		}

		public static GrammarMailboxFileStore FromMailboxGuid(OrganizationId orgId, Guid mbxGuid)
		{
			return new GrammarMailboxFileStore(orgId, mbxGuid);
		}

		public static ADUser GetOrganizationMailboxForRouting(OrganizationId orgId)
		{
			ValidateArgument.NotNull(orgId, "orgId");
			ADUser organizationMailboxByCapability = GrammarMailboxFileStore.GetOrganizationMailboxByCapability(orgId, OrganizationCapability.UMGrammarReady);
			if (organizationMailboxByCapability == null)
			{
				organizationMailboxByCapability = GrammarMailboxFileStore.GetOrganizationMailboxByCapability(orgId, OrganizationCapability.UMGrammar);
			}
			if (organizationMailboxByCapability != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, 0, "GrammarMailboxFileStore.GetOrganizationMailboxForRouting - orgId='{0}', finalChoice='{1}'", new object[]
				{
					orgId,
					organizationMailboxByCapability.DistinguishedName
				});
			}
			return organizationMailboxByCapability;
		}

		public void UploadGrammars(Dictionary<string, List<string>> grammars, CultureInfo culture, MailboxSession mbxSession, ThrowIfOperationCanceled throwIfOperationCanceled)
		{
			ValidateArgument.NotNull(grammars, "grammars");
			ValidateArgument.NotNull(mbxSession, "mbxSession");
			ValidateArgument.NotNull(throwIfOperationCanceled, "throwIfOperationCanceled");
			string text = (culture != null) ? culture.Name : string.Empty;
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "GrammarMailboxFileStore.UploadGrammars - Culture='{0}'", new object[]
			{
				text
			});
			string text2 = Path.Combine(Path.GetTempPath(), this.GetUniqueTmpFileName());
			Directory.CreateDirectory(text2);
			try
			{
				List<string> list;
				if (!string.IsNullOrEmpty(text))
				{
					list = new List<string>
					{
						text
					};
				}
				else
				{
					list = new List<string>(grammars.Keys);
				}
				foreach (string text3 in list)
				{
					ExTraceGlobals.UMGrammarGeneratorTracer.TraceDebug<string>((long)this.GetHashCode(), "GrammarMailboxFileStore.UploadGrammars - cultureName='{0}'", text3);
					List<string> list2 = grammars[text3];
					foreach (string text4 in list2)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "GrammarMailboxFileStore.UploadGrammars - filePath='{0}'", new object[]
						{
							text4
						});
						string text5;
						string text6;
						this.Compress(text4, text2, out text5, out text6);
						string text7 = string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}", new object[]
						{
							text5,
							text3,
							"1.0"
						});
						CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "GrammarMailboxFileStore.UploadGrammars - compressedFilePath='{0}', compressedFileName='{1}', fileSetId='{2}'", new object[]
						{
							text6,
							text5,
							text7
						});
						base.UploadFile(text6, text7, mbxSession);
						File.Delete(text6);
						throwIfOperationCanceled();
					}
				}
			}
			finally
			{
				base.DeleteFolder(text2);
			}
		}

		public string DownloadGrammar(string fileName, CultureInfo culture, DateTime threshold)
		{
			ValidateArgument.NotNullOrEmpty(fileName, "fileName");
			ValidateArgument.NotNull(culture, "culture");
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "GrammarMailboxFileStore.Download - fileName='{0}', culture='{1}', threshold='{2}'", new object[]
			{
				fileName,
				culture,
				threshold
			});
			string fileSetId = string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}", new object[]
			{
				fileName + ".gz",
				culture.Name,
				"1.0"
			});
			string text = null;
			using (MailboxSession mailboxSession = DirectoryMailboxFileStore.GetMailboxSession(base.OrgId, base.MailboxGuid))
			{
				text = base.DownloadLatestFile(fileSetId, threshold, mailboxSession);
			}
			string result = null;
			if (text != null)
			{
				try
				{
					result = this.Decompress(text);
				}
				finally
				{
					base.DeleteFolder(Path.GetDirectoryName(text));
				}
			}
			return result;
		}

		private void Compress(string filePath, string compressedFileDirectory, out string compressedFileName, out string compressedFilePath)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "GrammarMailboxFileStore.Compress - filePath='{0}', compressedFileDir='{1}'", new object[]
			{
				filePath,
				compressedFileDirectory
			});
			string fileName = Path.GetFileName(filePath);
			compressedFileName = fileName + ".gz";
			compressedFilePath = Path.Combine(compressedFileDirectory, compressedFileName);
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "GrammarMailboxFileStore.Compress - fileName='{0}', compressedFileName='{1}', compressedFilePath='{2}'", new object[]
			{
				fileName,
				compressedFileName,
				compressedFilePath
			});
			try
			{
				using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
				{
					using (FileStream fileStream2 = new FileStream(compressedFilePath, FileMode.Create))
					{
						using (GZipStream gzipStream = new GZipStream(fileStream2, CompressionMode.Compress))
						{
							fileStream.CopyTo(gzipStream);
						}
					}
				}
			}
			catch (IOException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "GrammarMailboxFileStore.Compress - Error compressing to compressedFilePath='{0}', error='{1}'", new object[]
				{
					compressedFilePath,
					ex
				});
				base.DeleteFile(compressedFilePath);
				throw;
			}
		}

		private string Decompress(string compressedFilePath)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "GrammarMailboxFileStore.Decompress - compressedFilePath='{0}'", new object[]
			{
				compressedFilePath
			});
			string text = Path.Combine(Path.GetTempPath(), this.GetUniqueTmpFileName());
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "GrammarMailboxFileStore.Decompress - uncompressedFilePath='{0}'", new object[]
			{
				text
			});
			try
			{
				using (FileStream fileStream = new FileStream(compressedFilePath, FileMode.Open))
				{
					using (FileStream fileStream2 = new FileStream(text, FileMode.Create))
					{
						using (GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
						{
							gzipStream.CopyTo(fileStream2);
						}
					}
				}
			}
			catch (IOException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this.GetHashCode(), "GrammarMailboxFileStore.Decompress - Error decompressing to uncompressedFilePath='{0}', error='{1}'", new object[]
				{
					text,
					ex
				});
				base.DeleteFile(text);
				throw;
			}
			return text;
		}

		private string GetUniqueTmpFileName()
		{
			return string.Format(CultureInfo.InvariantCulture, "UM-{0}", new object[]
			{
				Guid.NewGuid().ToString()
			});
		}

		private static ADUser GetOrganizationMailboxByCapability(OrganizationId orgId, OrganizationCapability capability)
		{
			ValidateArgument.NotNull(orgId, "orgId");
			ADUser result = null;
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, 0, "GrammarMailboxFileStore.GetOrganizationMailboxByCapability - orgId='{0}', Trying {1} flag", new object[]
			{
				orgId,
				capability
			});
			if (CommonConstants.UseDataCenterCallRouting)
			{
				ADUser[] allUsers = OrganizationMailbox.FindByOrganizationId(orgId, capability);
				result = GrammarMailboxFileStore.PickADUser(allUsers);
			}
			else
			{
				List<ADUser> list = null;
				List<ADUser> allUsers2 = null;
				bool flag = OrganizationMailbox.TryFindByOrganizationId(orgId, LocalServer.GetServer().ServerSite, capability, out list, out allUsers2);
				if (flag)
				{
					if (list.Count > 0)
					{
						result = GrammarMailboxFileStore.PickADUser(list);
					}
					else
					{
						result = GrammarMailboxFileStore.PickADUser(allUsers2);
					}
				}
			}
			return result;
		}

		private static ADUser PickADUser(IList<ADUser> allUsers)
		{
			if (allUsers != null && allUsers.Count > 0)
			{
				int index = Interlocked.Increment(ref GrammarMailboxFileStore.mailboxCounter) % allUsers.Count;
				return allUsers[index];
			}
			return null;
		}

		private const string FileSetIdFormat = "{0}_{1}_{2}";

		private const string UniqueTmpFileNameFormat = "UM-{0}";

		private const string CompressedFileExt = ".gz";

		internal const string FolderName = "SpeechGrammars";

		public const string GrammarVersion = "1.0";

		private static int mailboxCounter;
	}
}
