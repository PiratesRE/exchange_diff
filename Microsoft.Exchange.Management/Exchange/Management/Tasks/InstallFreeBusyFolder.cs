using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.FreeBusy;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "FreeBusyFolder")]
	public sealed class InstallFreeBusyFolder : Task
	{
		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			try
			{
				Server server = LocalServer.GetServer();
				PublicFolderDatabase[] e14SP1PublicFolderDatabases = this.GetE14SP1PublicFolderDatabases();
				if (e14SP1PublicFolderDatabases == null)
				{
					base.WriteVerbose(Strings.InstallFreeBusyFolderNoPublicFolderDatabase);
				}
				else if (!this.HasPublicFolderDatabase(server, e14SP1PublicFolderDatabases))
				{
					base.WriteVerbose(Strings.InstallFreeBusyFolderNoPublicFolderDatabase);
				}
				else
				{
					this.EnsureExternalFreeBusyFolder(server, e14SP1PublicFolderDatabases);
				}
			}
			catch (LocalizedException exception)
			{
				base.WriteVerbose(Strings.InstallFreeBusyFolderGeneralFailure(this.GetExceptionString(exception)));
			}
		}

		private string GetExceptionString(Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder(1000);
			while (exception != null)
			{
				stringBuilder.AppendLine(exception.ToString());
				stringBuilder.AppendLine();
				exception = exception.InnerException;
			}
			return stringBuilder.ToString();
		}

		private bool HasPublicFolderDatabase(Server server, PublicFolderDatabase[] publicFolderDatabases)
		{
			foreach (PublicFolderDatabase publicFolderDatabase in publicFolderDatabases)
			{
				if (publicFolderDatabase.Server.Equals(server.Id))
				{
					return true;
				}
			}
			return false;
		}

		private void EnsureExternalFreeBusyFolder(Server localServer, PublicFolderDatabase[] publicFolderDatabases)
		{
			Organization orgContainer = this.configurationSession.GetOrgContainer();
			if (orgContainer == null)
			{
				base.WriteVerbose(Strings.InstallFreeBusyFolderCannotGetOrganizationContainer);
				return;
			}
			string legacyDN = orgContainer.LegacyExchangeDN + "/ou=External (FYDIBOHF25SPDLT)";
			if (this.IsExternalFreeBusyFolderCreated(legacyDN, publicFolderDatabases))
			{
				base.WriteVerbose(Strings.InstallFreeBusyFolderAlreadyExists);
				return;
			}
			using (PublicFolderSession publicFolderSession = this.GetPublicFolderSession(localServer))
			{
				StoreObjectId freeBusyFolderId = FreeBusyFolder.GetFreeBusyFolderId(publicFolderSession, legacyDN, FreeBusyFolderDisposition.CreateIfNeeded);
				if (freeBusyFolderId == null)
				{
					base.WriteVerbose(Strings.InstallFreeBusyFolderUnableToCreateFolder);
				}
				else
				{
					base.WriteVerbose(Strings.InstallFreeBusyFolderCreatedFolder(freeBusyFolderId.ToString()));
					using (Folder folder = Folder.Bind(publicFolderSession, freeBusyFolderId, new PropertyDefinition[]
					{
						FolderSchema.ReplicaList
					}))
					{
						string[] array = Array.ConvertAll<PublicFolderDatabase, string>(publicFolderDatabases, (PublicFolderDatabase database) => database.ExchangeLegacyDN);
						string[] secondArray = (string[])folder[FolderSchema.ReplicaList];
						if (!this.IsEqualsArrayOfLegacyDN(array, secondArray))
						{
							folder[FolderSchema.ReplicaList] = array;
							folder.Save();
							folder.Load();
						}
					}
				}
			}
		}

		private bool IsEqualsArrayOfLegacyDN(string[] firstArray, string[] secondArray)
		{
			HashSet<string> hashSet = new HashSet<string>(firstArray, StringComparer.InvariantCultureIgnoreCase);
			HashSet<string> equals = new HashSet<string>(secondArray, StringComparer.InvariantCultureIgnoreCase);
			return hashSet.SetEquals(equals);
		}

		private bool IsExternalFreeBusyFolderCreated(string legacyDN, PublicFolderDatabase[] publicFolderDatabases)
		{
			foreach (PublicFolderDatabase publicFolderDatabase in publicFolderDatabases)
			{
				if (this.IsExternalFreeBusyFolderCreated(legacyDN, publicFolderDatabase))
				{
					base.WriteVerbose(Strings.InstallFreeBusyFolderAlreadyExistsInDatabase(publicFolderDatabase.Id.ToString()));
					return true;
				}
			}
			return false;
		}

		private bool IsExternalFreeBusyFolderCreated(string legacyDN, PublicFolderDatabase publicFolderDatabase)
		{
			Server server = publicFolderDatabase.GetServer();
			if (server == null)
			{
				return false;
			}
			bool result;
			try
			{
				using (PublicFolderSession publicFolderSession = this.GetPublicFolderSession(server))
				{
					StoreObjectId freeBusyFolderId = FreeBusyFolder.GetFreeBusyFolderId(publicFolderSession, legacyDN, FreeBusyFolderDisposition.None);
					result = (freeBusyFolderId != null);
				}
			}
			catch (LocalizedException)
			{
				base.WriteVerbose(Strings.InstallFreeBusyFolderUnableToCheckDatabase(publicFolderDatabase.Id.ToString()));
				result = false;
			}
			return result;
		}

		private PublicFolderSession GetPublicFolderSession(Server server)
		{
			return FreeBusyFolder.RetryOnStorageTransientException<PublicFolderSession>(() => PublicFolderSession.OpenAsAdmin(OrganizationId.ForestWideOrgId, null, Guid.Empty, null, CultureInfo.CurrentCulture, "Client=management;Action=Install-FreeBusyFolder", null));
		}

		private PublicFolderDatabase[] GetE14SP1PublicFolderDatabases()
		{
			PublicFolderDatabase[] array = this.configurationSession.Find<PublicFolderDatabase>(null, QueryScope.SubTree, null, null, 1000);
			if (array != null)
			{
				List<PublicFolderDatabase> list = new List<PublicFolderDatabase>(array.Length);
				foreach (PublicFolderDatabase publicFolderDatabase in array)
				{
					Server server = publicFolderDatabase.GetServer();
					if (server != null && server.IsE14Sp1OrLater)
					{
						list.Add(publicFolderDatabase);
					}
				}
				if (list.Count > 0)
				{
					return list.ToArray();
				}
			}
			return null;
		}

		private IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 30, "configurationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\StoreTasks\\InstallFreeBusyFolder.cs");
	}
}
