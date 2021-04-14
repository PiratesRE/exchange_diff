using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.FreeBusy
{
	internal static class FreeBusyFolder
	{
		public static Item CreateFreeBusyItem(PublicFolderSession session, StoreObjectId freeBusyFolderId, string legacyDN)
		{
			return FreeBusyFolder.RetryOnStorageTransientException<Item>(() => FreeBusyFolder.CreateFreeBusyItemInternal(session, freeBusyFolderId, legacyDN));
		}

		public static StoreObjectId GetFreeBusyFolderId(PublicFolderSession session, string legacyDN, FreeBusyFolderDisposition disposition)
		{
			return FreeBusyFolder.RetryOnStorageTransientException<StoreObjectId>(() => FreeBusyFolder.GetFreeBusyFolderIdInternal(session, legacyDN, disposition));
		}

		internal static T RetryOnStorageTransientException<T>(Func<T> func)
		{
			for (int i = 0; i < 3; i++)
			{
				try
				{
					return func();
				}
				catch (StorageTransientException arg)
				{
					FreeBusyFolder.Tracer.TraceError<StorageTransientException>(0L, "Failed due transient exception, retrying. Exception: {0}.", arg);
				}
			}
			return func();
		}

		internal static StoreObjectId GetFreeBusyRootFolderId(PublicFolderSession session)
		{
			StoreObjectId result;
			try
			{
				StoreObjectId storeObjectId = null;
				using (Folder folder = Folder.Bind(session, session.GetNonIpmSubtreeFolderId()))
				{
					storeObjectId = QueryChildFolderByName.Query(folder, "SCHEDULE+ FREE BUSY");
				}
				if (storeObjectId == null)
				{
					FreeBusyFolder.Tracer.TraceError<string>(0L, "Unable to find folder '{0}' in public folders", "SCHEDULE+ FREE BUSY");
				}
				result = storeObjectId;
			}
			catch (StoragePermanentException arg)
			{
				FreeBusyFolder.Tracer.TraceError<string, StoragePermanentException>(0L, "Failed to get folder '{0}' due exception: {1}", "SCHEDULE+ FREE BUSY", arg);
				result = null;
			}
			return result;
		}

		internal static string GetFreeBusyItemSubject(string legacyDN)
		{
			int num = legacyDN.IndexOf("/cn=", StringComparison.OrdinalIgnoreCase);
			if (num == -1)
			{
				FreeBusyFolder.Tracer.TraceDebug<string>(0L, "Found no /cn= in legacy DN: {0}", legacyDN);
				return null;
			}
			string text = legacyDN.Substring(num);
			return "USER-" + text.ToUpper(CultureInfo.InvariantCulture);
		}

		internal static string GetFreeBusyFolderName(string legacyDN)
		{
			int num = legacyDN.IndexOf("/cn=", StringComparison.OrdinalIgnoreCase);
			if (num == -1)
			{
				num = legacyDN.Length;
			}
			string text = legacyDN.Substring(0, num);
			return "EX:" + text.ToUpper(CultureInfo.InvariantCulture);
		}

		internal static string GetInternalLegacyDN(ADUser user, string targetLegacyDN)
		{
			string x = FreeBusyFolder.GetOrganizationLegacyDN(user.LegacyExchangeDN) + "/ou=External (FYDIBOHF25SPDLT)";
			foreach (ProxyAddress proxyAddress in user.EmailAddresses)
			{
				if (proxyAddress.Prefix == ProxyAddressPrefix.X500)
				{
					string oulegacyDN = FreeBusyFolder.GetOULegacyDN(proxyAddress.AddressString);
					if (oulegacyDN != null && !StringComparer.OrdinalIgnoreCase.Equals(x, oulegacyDN) && FreeBusyFolder.IsGeneratedLegacyDN(proxyAddress.AddressString))
					{
						return proxyAddress.AddressString;
					}
				}
			}
			string oulegacyDN2 = FreeBusyFolder.GetOULegacyDN(targetLegacyDN);
			if (oulegacyDN2 != null)
			{
				return FreeBusyFolder.GenerateInternalLegacyDN(oulegacyDN2);
			}
			return null;
		}

		internal static string GetExternalLegacyDN(ADUser user)
		{
			string y = FreeBusyFolder.GetOrganizationLegacyDN(user.LegacyExchangeDN) + "/ou=External (FYDIBOHF25SPDLT)";
			foreach (ProxyAddress proxyAddress in user.EmailAddresses)
			{
				if (proxyAddress.Prefix == ProxyAddressPrefix.X500)
				{
					string oulegacyDN = FreeBusyFolder.GetOULegacyDN(proxyAddress.AddressString);
					if (oulegacyDN != null && StringComparer.OrdinalIgnoreCase.Equals(oulegacyDN, y))
					{
						return proxyAddress.AddressString;
					}
				}
			}
			return FreeBusyFolder.GenerateExternalLegacyDN(user.LegacyExchangeDN);
		}

		private static string GenerateInternalLegacyDN(string legacyDN)
		{
			return legacyDN + "/cn=Recipients" + FreeBusyFolder.GetNewCnRdn();
		}

		private static string GenerateExternalLegacyDN(string legacyDN)
		{
			return FreeBusyFolder.GetOrganizationLegacyDN(legacyDN) + "/ou=External (FYDIBOHF25SPDLT)/cn=Recipients" + FreeBusyFolder.GetNewCnRdn();
		}

		private static string GetNewCnRdn()
		{
			return "/cn=" + Guid.NewGuid().ToString().Replace("-", string.Empty).ToLower();
		}

		private static bool IsGeneratedLegacyDN(string legacyDN)
		{
			int num = legacyDN.LastIndexOf("/cn=");
			if (num == -1)
			{
				return false;
			}
			string text = legacyDN.Substring(num + "/cn=".Length);
			return text.Length == FreeBusyFolder.GeneratedLegacyDNValueLength && FreeBusyFolder.GeneratedLegacyDNValueValidator.IsMatch(text);
		}

		private static string GetOrganizationLegacyDN(string legacyDN)
		{
			int num = legacyDN.IndexOf("/ou=", StringComparison.OrdinalIgnoreCase);
			if (num == -1)
			{
				return legacyDN;
			}
			return legacyDN.Substring(0, num);
		}

		private static string GetOULegacyDN(string legacyDN)
		{
			if (string.IsNullOrEmpty(legacyDN))
			{
				return null;
			}
			int num = legacyDN.IndexOf("/ou=");
			if (num == -1)
			{
				return null;
			}
			int num2 = legacyDN.IndexOf("/", num + "/ou=".Length);
			if (num2 == -1)
			{
				num2 = legacyDN.Length;
			}
			return legacyDN.Substring(0, num2);
		}

		private static Item CreateFreeBusyItemInternal(PublicFolderSession session, StoreObjectId freeBusyFolderId, string legacyDN)
		{
			string freeBusyItemSubject = FreeBusyFolder.GetFreeBusyItemSubject(legacyDN);
			Item item = Item.Create(session, "IPM.Post", freeBusyFolderId);
			bool flag = false;
			try
			{
				item[ItemSchema.Subject] = freeBusyItemSubject;
				item[FreeBusyItemSchema.ScheduleInfoRecipientLegacyDn] = legacyDN;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					item.Dispose();
				}
			}
			return item;
		}

		private static StoreObjectId GetFreeBusyFolderIdInternal(PublicFolderSession session, string legacyDN, FreeBusyFolderDisposition disposition)
		{
			string freeBusyFolderName = FreeBusyFolder.GetFreeBusyFolderName(legacyDN);
			if (freeBusyFolderName == null)
			{
				FreeBusyFolder.Tracer.TraceError<string>(0L, "Unable to get free/busy folder for mailbox with legacy DN '{0}' because cannot obtain folder name from legacy DN", legacyDN);
				return null;
			}
			StoreObjectId freeBusyRootFolderId = FreeBusyFolder.GetFreeBusyRootFolderId(session);
			if (freeBusyRootFolderId == null)
			{
				return null;
			}
			StoreObjectId result;
			try
			{
				StoreObjectId storeObjectId = null;
				using (Folder folder = Folder.Bind(session, freeBusyRootFolderId))
				{
					storeObjectId = QueryChildFolderByName.Query(folder, freeBusyFolderName);
				}
				if (storeObjectId == null && disposition == FreeBusyFolderDisposition.CreateIfNeeded)
				{
					storeObjectId = FreeBusyFolder.CreateFreeBusyFolder(session, freeBusyRootFolderId, freeBusyFolderName);
				}
				result = storeObjectId;
			}
			catch (StoragePermanentException arg)
			{
				FreeBusyFolder.Tracer.TraceDebug<StoragePermanentException>(0L, "Failed to get free/busy folder due exception: {0}", arg);
				result = null;
			}
			return result;
		}

		private static StoreObjectId CreateFreeBusyFolder(PublicFolderSession session, StoreObjectId parentFolder, string folderName)
		{
			Exception ex = null;
			try
			{
				using (Folder folder = Folder.Create(session, parentFolder, StoreObjectType.Folder, folderName, CreateMode.OpenIfExists))
				{
					folder[FolderSchema.ResolveMethod] = (ResolveMethod.LastWriterWins | ResolveMethod.NoConflictNotification);
					FolderSaveResult folderSaveResult = folder.Save();
					if (folderSaveResult.OperationResult == OperationResult.Succeeded)
					{
						folder.Load();
						StoreObjectId objectId = folder.Id.ObjectId;
						FreeBusyFolder.Tracer.TraceDebug<string, StoreObjectId, StoreObjectId>(0L, "Created free/busy folder '{0}' under folder '{1}' with id '{2}'.", folderName, parentFolder, objectId);
						return objectId;
					}
					ex = folderSaveResult.Exception;
				}
			}
			catch (PropertyErrorException ex2)
			{
				ex = ex2;
			}
			catch (CorruptDataException ex3)
			{
				ex = ex3;
			}
			catch (ObjectNotFoundException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				FreeBusyFolder.Tracer.TraceError<string, StoreObjectId, Exception>(0L, "Failed to create free/busy folder '{0}' under folder '{1}' due exception: {2}", folderName, parentFolder, ex);
			}
			return null;
		}

		public const string OURdnExternalOU = "/ou=External (FYDIBOHF25SPDLT)";

		private const int OpenSessionRetries = 3;

		private const string FreeBusyItemSubjectPrefix = "USER-";

		private const string FreeBusyFolderNamePrefix = "EX:";

		private const string FreeBusyRootFolderName = "SCHEDULE+ FREE BUSY";

		private const int TransientRetries = 3;

		private const string ExternalOU = "External (FYDIBOHF25SPDLT)";

		private const string OuRdn = "/ou=";

		private const string CnRdn = "/cn=";

		private const string RecipientsRdn = "/cn=Recipients";

		private static readonly Trace Tracer = ExTraceGlobals.FreeBusyTracer;

		private static readonly int GeneratedLegacyDNValueLength = Guid.Empty.ToString().Replace("-", string.Empty).Length;

		private static readonly Regex GeneratedLegacyDNValueValidator = new Regex("[0-9a-fA-F]*", RegexOptions.Compiled);
	}
}
