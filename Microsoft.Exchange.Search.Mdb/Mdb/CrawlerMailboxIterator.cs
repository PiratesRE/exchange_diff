using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Search.Mdb
{
	internal class CrawlerMailboxIterator
	{
		public CrawlerMailboxIterator(MdbInfo mdbInfo)
		{
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("CrawlerMailboxIterator", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.MdbCrawlerFeederTracer, (long)this.GetHashCode());
			this.mdbInfo = mdbInfo;
		}

		public IEnumerable<MailboxInfo> GetMailboxes()
		{
			return this.GetMailboxes(Guid.Empty);
		}

		public virtual IEnumerable<MailboxInfo> GetMailboxes(Guid mailboxGuid)
		{
			this.diagnosticsSession.TraceDebug<MdbInfo>("Starting to crawl MDB: {0}", this.mdbInfo);
			PropValue[][] propValues = MapiUtil.TranslateMapiExceptionsWithReturnValue<PropValue[][]>(this.diagnosticsSession, Strings.MdbMailboxQueryFailed(this.mdbInfo.Guid), delegate
			{
				ISearchServiceConfig searchServiceConfig = Factory.Current.CreateSearchServiceConfig();
				PropValue[][] result2;
				using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=CI", (searchServiceConfig.ReadFromPassiveEnabled && !this.mdbInfo.IsLagCopy) ? LocalServer.GetServer().Fqdn : this.mdbInfo.OwningServer, null, null, null))
				{
					if (mailboxGuid == Guid.Empty)
					{
						result2 = exRpcAdmin.GetMailboxTable(this.mdbInfo.Guid, CrawlerMailboxIterator.DefaultPropTags);
					}
					else
					{
						result2 = exRpcAdmin.GetMailboxTableInfo(this.mdbInfo.Guid, mailboxGuid, CrawlerMailboxIterator.DefaultPropTags);
					}
				}
				return result2;
			});
			List<MailboxInfo> archiveMailboxes = new List<MailboxInfo>();
			foreach (PropValue[] props in propValues)
			{
				Guid mbxGuid = Guid.Empty;
				string displayName = null;
				int mailboxNumber = 0;
				bool skipMbx = false;
				bool isArchive = false;
				bool isPublicFolderMailbox = false;
				bool isSharedMailbox = false;
				bool isTeamSiteMailbox = false;
				bool isModernGroupMailbox = false;
				OrganizationId organizationId = null;
				TenantPartitionHint tenantPartitionHint = null;
				foreach (PropValue propValue in props)
				{
					if (!propValue.IsError())
					{
						int num = propValue.PropTag.Id();
						if (num == PropTag.UserGuid.Id())
						{
							mbxGuid = new Guid(propValue.GetBytes());
							if (this.mdbInfo.SystemMailboxGuid == mbxGuid || this.mdbInfo.SystemAttendantGuid == mbxGuid)
							{
								skipMbx = true;
							}
						}
						else if (num == PropTag.MailboxNumber.Id())
						{
							mailboxNumber = propValue.GetInt();
						}
						else if (num == PropTag.DisplayName.Id())
						{
							displayName = propValue.GetString();
						}
						else if (num == PropTag.DateDiscoveredAbsentInDS.Id())
						{
							skipMbx = true;
						}
						else if (num == PropTag.MailboxMiscFlags.Id())
						{
							MailboxMiscFlags @int = (MailboxMiscFlags)propValue.GetInt();
							isArchive = ((@int & MailboxMiscFlags.ArchiveMailbox) == MailboxMiscFlags.ArchiveMailbox);
							skipMbx = (skipMbx || (@int & MailboxMiscFlags.CreatedByMove) == MailboxMiscFlags.CreatedByMove);
						}
						else if (num == PropTag.MailboxType.Id())
						{
							isPublicFolderMailbox = StoreSession.IsPublicFolderMailbox(propValue.GetInt());
						}
						else if (num == PropTag.PersistableTenantPartitionHint.Id())
						{
							tenantPartitionHint = TenantPartitionHint.FromPersistablePartitionHint(propValue.GetBytes());
						}
						else if (num == PropTag.MailboxTypeDetail.Id())
						{
							isTeamSiteMailbox = StoreSession.IsTeamSiteMailbox(propValue.GetInt());
							isSharedMailbox = StoreSession.IsSharedMailbox(propValue.GetInt());
							isModernGroupMailbox = StoreSession.IsGroupMailbox(propValue.GetInt());
						}
					}
				}
				if (!skipMbx && mbxGuid != Guid.Empty && mailboxNumber > 0 && displayName != null)
				{
					if (isPublicFolderMailbox)
					{
						try
						{
							organizationId = ADSessionSettings.FromTenantPartitionHint(tenantPartitionHint).CurrentOrganizationId;
						}
						catch (CannotResolveExternalDirectoryOrganizationIdException arg)
						{
							this.diagnosticsSession.TraceError<Guid, string, CannotResolveExternalDirectoryOrganizationIdException>("Skip crawling mailbox: {0}({1}) due to Exception: {2}", mbxGuid, displayName, arg);
							goto IL_4BB;
						}
					}
					MailboxInfo result = new MailboxInfo
					{
						MailboxGuid = mbxGuid,
						IsArchive = isArchive,
						DisplayName = displayName,
						MailboxNumber = mailboxNumber,
						IsPublicFolderMailbox = isPublicFolderMailbox,
						OrganizationId = organizationId,
						IsModernGroupMailbox = isModernGroupMailbox,
						IsSharedMailbox = isSharedMailbox,
						IsTeamSiteMailbox = isTeamSiteMailbox
					};
					if (isArchive)
					{
						archiveMailboxes.Add(result);
					}
					else
					{
						yield return result;
					}
				}
				else
				{
					this.diagnosticsSession.TraceDebug<Guid, string>("Skip crawling mailbox: {0}({1})", mbxGuid, displayName);
				}
				IL_4BB:;
			}
			foreach (MailboxInfo mailbox in archiveMailboxes)
			{
				yield return mailbox;
			}
			yield break;
		}

		private static readonly PropTag[] DefaultPropTags = new PropTag[]
		{
			PropTag.UserGuid,
			PropTag.DisplayName,
			PropTag.DateDiscoveredAbsentInDS,
			PropTag.MailboxMiscFlags,
			PropTag.MailboxNumber,
			PropTag.MailboxType,
			PropTag.PersistableTenantPartitionHint
		};

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly MdbInfo mdbInfo;
	}
}
