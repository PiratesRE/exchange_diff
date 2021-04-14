using System;
using Microsoft.Exchange.Data.ApplicationLogic.FreeBusy;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.Data.ApplicationLogic.UM
{
	internal abstract class UMStagingFolder
	{
		internal static Folder OpenOrCreateUMStagingFolder(MailboxSession session)
		{
			return UMStagingFolder.OpenOrCreate(session, "UM Staging");
		}

		internal static bool TryOpenUMReportingFolder(MailboxSession session, out Folder umReportingFolder)
		{
			return UMStagingFolder.TryOpenFolder(session, "UMReportingData", out umReportingFolder);
		}

		internal static bool TryOpenUMGrammarsFolder(MailboxSession session, out Folder umGrammarsFolder)
		{
			return UMStagingFolder.TryOpenFolder(session, "UMGrammars", out umGrammarsFolder);
		}

		private static bool TryOpenFolder(MailboxSession session, string folderName, out Folder folder)
		{
			folder = null;
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			bool result;
			using (Folder folder2 = Folder.Bind(session, DefaultFolderType.Configuration))
			{
				StoreObjectId storeObjectId = QueryChildFolderByName.Query(folder2, folderName);
				if (storeObjectId == null)
				{
					result = false;
				}
				else
				{
					folder = Folder.Bind(session, storeObjectId);
					result = true;
				}
			}
			return result;
		}

		internal static Folder OpenOrCreateUMReportingFolder(MailboxSession session)
		{
			return UMStagingFolder.OpenOrCreate(session, "UMReportingData");
		}

		internal static Folder OpenOrCreateUMGrammarsFolder(MailboxSession session)
		{
			return UMStagingFolder.OpenOrCreate(session, "UMGrammars");
		}

		private static Folder OpenOrCreate(MailboxSession session, string folderName)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			UMStagingFolder.Tracer.TraceDebug<string>(0L, "Attempting to open or create UM Staging folder for {0}", session.MailboxOwnerLegacyDN);
			Folder result;
			using (Folder folder = Folder.Bind(session, DefaultFolderType.Configuration))
			{
				Folder folder2 = null;
				bool flag = false;
				try
				{
					folder2 = Folder.Create(session, folder.Id, StoreObjectType.Folder, folderName, CreateMode.OpenIfExists);
					folder2.Save();
					folder2.Load(new PropertyDefinition[]
					{
						ItemSchema.Id,
						StoreObjectSchema.DisplayName
					});
					UMStagingFolder.Tracer.TraceDebug<string, string>(0L, "UM Staging folder opened successfully Id:{0}, Owner:{1}", folder2.Id.ObjectId.ToBase64String(), session.MailboxOwnerLegacyDN);
					flag = true;
				}
				finally
				{
					if (folder2 != null && !flag)
					{
						folder2.Dispose();
						folder2 = null;
					}
				}
				result = folder2;
			}
			return result;
		}

		internal const string StagingFolderName = "UM Staging";

		internal const string UMReportingFolderName = "UMReportingData";

		internal const string UMGrammarsFolderName = "UMGrammars";

		private static readonly Trace Tracer = ExTraceGlobals.UMPartnerMessageTracer;
	}
}
