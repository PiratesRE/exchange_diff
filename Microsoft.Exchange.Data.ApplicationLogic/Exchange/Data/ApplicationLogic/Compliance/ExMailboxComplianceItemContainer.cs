using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Office.CompliancePolicy;
using Microsoft.Office.CompliancePolicy.ComplianceData;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Data.ApplicationLogic.Compliance
{
	internal class ExMailboxComplianceItemContainer : ExComplianceItemContainer
	{
		internal ExMailboxComplianceItemContainer(MailboxSession session)
		{
			this.mailboxSession = session;
		}

		internal ExMailboxComplianceItemContainer(IRecipientSession recipientSession, string externalDirectoryObjectId)
		{
			this.recipientSession = recipientSession;
			this.externalDirectoryObjectId = externalDirectoryObjectId;
		}

		public override bool HasItems
		{
			get
			{
				return false;
			}
		}

		public override string Id
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override List<ComplianceItemContainer> Ancestors
		{
			get
			{
				return null;
			}
		}

		public override bool SupportsAssociation
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override bool SupportsBinding
		{
			get
			{
				return true;
			}
		}

		public override string Template
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal string ExternalDirectoryObjectId
		{
			get
			{
				if (string.IsNullOrEmpty(this.externalDirectoryObjectId))
				{
					this.PopulateExternalDirectoryObjectIdAndRecipientSessionFromMailboxSession();
				}
				return this.externalDirectoryObjectId;
			}
		}

		internal IRecipientSession RecipientSession
		{
			get
			{
				if (this.recipientSession == null)
				{
					this.PopulateExternalDirectoryObjectIdAndRecipientSessionFromMailboxSession();
				}
				return this.recipientSession;
			}
		}

		internal override MailboxSession Session
		{
			get
			{
				if (this.mailboxSession == null)
				{
					if (this.recipientSession == null || string.IsNullOrEmpty(this.externalDirectoryObjectId))
					{
						throw new ArgumentException("Both mailboxSession and recipientSession are null");
					}
					ADUser adUser = ExMailboxComplianceItemContainer.GetAdUser(this.RecipientSession, this.ExternalDirectoryObjectId, true);
					ExchangePrincipal mailboxOwner = ExchangePrincipal.FromADUser(adUser.OrganizationId.ToADSessionSettings(), adUser, RemotingOptions.LocalConnectionsOnly);
					this.mailboxSession = MailboxSession.OpenAsSystemService(mailboxOwner, CultureInfo.CurrentCulture, "Client=UnifiedPolicy");
				}
				return this.mailboxSession;
			}
		}

		internal override ComplianceItemPagedReader ComplianceItemPagedReader
		{
			get
			{
				return new ExMailboxSearchComplianceItemPagedReader(this);
			}
		}

		public override bool SupportsPolicy(PolicyScenario scenario)
		{
			throw new NotImplementedException();
		}

		public override void ForEachChildContainer(Action<ComplianceItemContainer> containerHandler, Func<ComplianceItemContainer, Exception, bool> exHandler)
		{
			using (Folder folder = Folder.Bind(this.Session, DefaultFolderType.Root, ExMailboxComplianceItemContainer.FolderDataColumns))
			{
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, null, null, ExMailboxComplianceItemContainer.FolderDataColumns))
				{
					using (FolderEnumerator folderEnumerator = new FolderEnumerator(queryResult, folder, folder.GetProperties(ExMailboxComplianceItemContainer.FolderDataColumns)))
					{
						while (folderEnumerator != null && folderEnumerator.MoveNext())
						{
							for (int i = 0; i < folderEnumerator.Current.Count; i++)
							{
								VersionedId versionedId = folderEnumerator.Current[i][0] as VersionedId;
								if (versionedId != null)
								{
									using (Folder folder2 = Folder.Bind(this.Session, versionedId.ObjectId))
									{
										ExFolderComplianceItemContainer exFolderComplianceItemContainer = new ExFolderComplianceItemContainer(this.Session, this, folder2);
										try
										{
											containerHandler(exFolderComplianceItemContainer);
										}
										catch (Exception arg)
										{
											exHandler(exFolderComplianceItemContainer, arg);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public override void UpdatePolicy(Dictionary<PolicyScenario, List<PolicyRuleConfig>> rules)
		{
			throw new NotImplementedException();
		}

		public override void AddPolicy(PolicyDefinitionConfig definition, PolicyRuleConfig rule)
		{
			if (definition.Scenario == PolicyScenario.Hold)
			{
				this.UpdateHold(definition.Identity, true);
			}
		}

		public override void RemovePolicy(Guid id, PolicyScenario scenario)
		{
			if (scenario == PolicyScenario.Hold)
			{
				this.UpdateHold(id, false);
			}
		}

		public override bool HasPolicy(Guid id, PolicyScenario scenario)
		{
			if (scenario == PolicyScenario.Hold)
			{
				try
				{
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.PartiallyConsistent, this.RecipientSession.SessionSettings, 327, "HasPolicy", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Compliance\\ExMailboxComplianceItemContainer.cs");
					ADRecipient adUser = ExMailboxComplianceItemContainer.GetAdUser(tenantOrRootOrgRecipientSession, this.ExternalDirectoryObjectId, true);
					string holdId = ExMailboxComplianceItemContainer.GetHoldId(id);
					return adUser.InPlaceHolds.Contains(holdId);
				}
				catch (Exception arg)
				{
					ExTraceGlobals.StorageTracer.TraceError<string, Exception>(0L, "Failed to find out if mailbox '{0}' has hold. Exception: {1}", this.ExternalDirectoryObjectId, arg);
					throw;
				}
			}
			throw new NotImplementedException("Scenario is not supported: " + scenario.ToString());
		}

		internal static string GetHoldId(Guid id)
		{
			return "UniH" + id.ToString();
		}

		private static ADUser GetAdUser(IRecipientSession recipientSession, string scope, bool throwIfNotFound)
		{
			LegacyDN legacyDN;
			ADUser aduser;
			if (LegacyDN.TryParse(scope, out legacyDN))
			{
				aduser = (recipientSession.FindByLegacyExchangeDN(scope) as ADUser);
			}
			else
			{
				aduser = recipientSession.FindADUserByExternalDirectoryObjectId(scope);
			}
			if (aduser == null && throwIfNotFound)
			{
				throw new ComplianceTaskPermanentException("Recipient not found: " + scope, UnifiedPolicyErrorCode.FailedToOpenContainer);
			}
			return aduser;
		}

		protected override void Dispose(bool disposing)
		{
		}

		private void PopulateExternalDirectoryObjectIdAndRecipientSessionFromMailboxSession()
		{
			if (this.mailboxSession == null)
			{
				throw new ArgumentException("Both recipientSession and mailboxSession are null");
			}
			this.recipientSession = this.mailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid);
			this.externalDirectoryObjectId = this.mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
		}

		private void UpdateHold(Guid id, bool add)
		{
			try
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.PartiallyConsistent, this.RecipientSession.SessionSettings, 418, "UpdateHold", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\Compliance\\ExMailboxComplianceItemContainer.cs");
				ADRecipient adUser = ExMailboxComplianceItemContainer.GetAdUser(tenantOrRootOrgRecipientSession, this.ExternalDirectoryObjectId, add);
				string holdId = ExMailboxComplianceItemContainer.GetHoldId(id);
				if (add)
				{
					MailboxDiscoverySearch.AddInPlaceHold(adUser, holdId, tenantOrRootOrgRecipientSession);
				}
				else
				{
					MailboxDiscoverySearch.RemoveInPlaceHold(adUser, holdId, tenantOrRootOrgRecipientSession);
				}
			}
			catch (ComplianceTaskPermanentException)
			{
				throw;
			}
			catch (Exception ex)
			{
				ExTraceGlobals.StorageTracer.TraceError<string, Exception, string>(0L, "Failed to {2} hold on mailbox '{0}'. Exception: {1}", this.ExternalDirectoryObjectId, ex, add ? "place" : "remove");
				throw new ComplianceTaskPermanentException(string.Format("Failed to {0} hold on mailbox '{1}'", add ? "place" : "remove", this.ExternalDirectoryObjectId), ex, UnifiedPolicyErrorCode.Unknown);
			}
		}

		internal const int FolderVersionedId = 0;

		internal const string UnifiedHoldIdPrefix = "UniH";

		internal static readonly PropertyDefinition[] FolderDataColumns = new PropertyDefinition[]
		{
			FolderSchema.Id,
			StoreObjectSchema.ParentItemId,
			StoreObjectSchema.DisplayName
		};

		private string externalDirectoryObjectId;

		private IRecipientSession recipientSession;

		private MailboxSession mailboxSession;
	}
}
