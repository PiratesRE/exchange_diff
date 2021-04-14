using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "PublicFolderDatabase", DefaultParameterSetName = "Identity")]
	public sealed class GetPublicFolderDatabase : GetDatabaseTask<PublicFolderDatabase>, IPFTreeTask
	{
		OrganizationId IPFTreeTask.CurrentOrganizationId
		{
			get
			{
				return base.CurrentOrganizationId;
			}
		}

		ADObjectId IPFTreeTask.RootOrgContainerId
		{
			get
			{
				return base.RootOrgContainerId;
			}
		}

		IConfigDataProvider IPFTreeTask.DataSession
		{
			get
			{
				return base.DataSession;
			}
		}

		ITopologyConfigurationSession IPFTreeTask.GlobalConfigSession
		{
			get
			{
				return base.GlobalConfigSession;
			}
		}

		OrganizationId IPFTreeTask.ExecutingUserOrganizationId
		{
			get
			{
				return base.ExecutingUserOrganizationId;
			}
		}

		OrganizationId IPFTreeTask.ResolveCurrentOrganization()
		{
			return OrganizationId.ForestWideOrgId;
		}

		T IPFTreeTask.GetDataObject<T>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, LocalizedString? notFoundError, LocalizedString? multipleFoundError)
		{
			return (T)((object)base.GetDataObject<T>(id, session, rootID, notFoundError, multipleFoundError));
		}

		void IPFTreeTask.WriteVerbose(LocalizedString text)
		{
			base.WriteVerbose(text);
		}

		void IPFTreeTask.WriteWarning(LocalizedString text)
		{
			this.WriteWarning(text);
		}

		void IPFTreeTask.WriteError(Exception exception, ErrorCategory category, object target)
		{
			base.WriteError(exception, category, target);
		}

		[Parameter]
		[ValidateNotNullOrEmpty]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		public override SwitchParameter IncludePreExchange2013
		{
			get
			{
				return true;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				PFTreeManagement pftreeManagement = new PFTreeManagement(this);
				QueryFilter queryFilter;
				if (this.Organization != null)
				{
					if (pftreeManagement.PFTree != null)
					{
						queryFilter = new ComparisonFilter(ComparisonOperator.Equal, PublicFolderDatabaseSchema.PublicFolderHierarchy, pftreeManagement.PFTree.Id);
					}
					else
					{
						queryFilter = new NotFilter(new ExistsFilter(PublicFolderDatabaseSchema.PublicFolderHierarchy));
					}
				}
				else
				{
					queryFilter = new ExistsFilter(PublicFolderDatabaseSchema.PublicFolderHierarchy);
				}
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.InternalFilter,
					queryFilter
				});
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			PublicFolderDatabase publicFolderDatabase = (PublicFolderDatabase)dataObject;
			if (!publicFolderDatabase.IsReadOnly)
			{
				Server server = publicFolderDatabase.GetServer();
				if (server == null)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorDBOwningServerNotFound(publicFolderDatabase.Identity.ToString())), ErrorCategory.InvalidOperation, publicFolderDatabase.Identity);
				}
				publicFolderDatabase.UseCustomReferralServerList = server.UseCustomReferralServerList;
				foreach (ServerCostPair serverCostPair in server.CustomReferralServerList)
				{
					ServerIdParameter serverIdParameter = new ServerIdParameter(new ADObjectId(null, serverCostPair.ServerGuid));
					IEnumerable<Server> objects = serverIdParameter.GetObjects<Server>(null, base.DataSession);
					IEnumerator<Server> enumerator2 = objects.GetEnumerator();
					Server server2 = null;
					if (enumerator2.MoveNext())
					{
						server2 = enumerator2.Current;
						if (enumerator2.MoveNext())
						{
							server2 = null;
						}
					}
					if (server2 == null)
					{
						publicFolderDatabase.CustomReferralServerList.Add(serverCostPair);
					}
					else
					{
						publicFolderDatabase.CustomReferralServerList.Add(new ServerCostPair(server2.Guid, server2.Name, serverCostPair.Cost));
					}
				}
				publicFolderDatabase.ResetChangeTracking();
			}
			base.WriteResult(dataObject);
		}

		Fqdn IPFTreeTask.get_DomainController()
		{
			return base.DomainController;
		}
	}
}
