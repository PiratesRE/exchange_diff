using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Office.CompliancePolicy.ComplianceData;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Data.ApplicationLogic.Compliance
{
	internal class ExFolderComplianceItemContainer : ExComplianceItemContainer
	{
		internal ExFolderComplianceItemContainer(MailboxSession session, ComplianceItemContainer parent, Folder folder)
		{
			this.session = session;
			this.folder = folder;
			this.parent = parent;
		}

		public override bool HasItems
		{
			get
			{
				return this.folder.ItemCount > 0;
			}
		}

		public override string Id
		{
			get
			{
				return this.folder.StoreObjectId.ToString();
			}
		}

		public override List<ComplianceItemContainer> Ancestors
		{
			get
			{
				if (this.parents == null)
				{
					this.parents = new List<ComplianceItemContainer>();
					for (ExFolderComplianceItemContainer exFolderComplianceItemContainer = this; exFolderComplianceItemContainer != null; exFolderComplianceItemContainer = (exFolderComplianceItemContainer.parent as ExFolderComplianceItemContainer))
					{
						this.parents.Add(this.parent);
					}
				}
				return this.parents;
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
				throw new NotImplementedException();
			}
		}

		public override string Template
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal override ComplianceItemPagedReader ComplianceItemPagedReader
		{
			get
			{
				return new ExFolderSearchComplianceItemPagedReader(this);
			}
		}

		internal override MailboxSession Session
		{
			get
			{
				return this.session;
			}
		}

		internal Folder Folder
		{
			get
			{
				return this.folder;
			}
		}

		public override void ForEachChildContainer(Action<ComplianceItemContainer> containerHandler, Func<ComplianceItemContainer, Exception, bool> exHandler)
		{
			if (this.folder.HasSubfolders)
			{
				QueryResult queryResult = this.folder.FolderQuery(FolderQueryFlags.None, null, null, ExMailboxComplianceItemContainer.FolderDataColumns);
				using (FolderEnumerator folderEnumerator = new FolderEnumerator(queryResult, this.folder, this.folder.GetProperties(ExMailboxComplianceItemContainer.FolderDataColumns)))
				{
					while (folderEnumerator != null && folderEnumerator.MoveNext())
					{
						for (int i = 0; i < folderEnumerator.Current.Count; i++)
						{
							VersionedId versionedId = folderEnumerator.Current[i][0] as VersionedId;
							if (versionedId != null)
							{
								Folder folder = Folder.Bind(this.session, versionedId.ObjectId);
								if (this.folder.StoreObjectId != folder.StoreObjectId)
								{
									using (ExFolderComplianceItemContainer exFolderComplianceItemContainer = new ExFolderComplianceItemContainer(this.session, this, folder))
									{
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

		public override bool SupportsPolicy(PolicyScenario scenario)
		{
			throw new NotImplementedException();
		}

		public override void UpdatePolicy(Dictionary<PolicyScenario, List<PolicyRuleConfig>> rules)
		{
			throw new NotImplementedException();
		}

		public override void AddPolicy(PolicyDefinitionConfig definition, PolicyRuleConfig rule)
		{
			throw new NotImplementedException();
		}

		public override void RemovePolicy(Guid id, PolicyScenario scenario)
		{
			throw new NotImplementedException();
		}

		public override bool HasPolicy(Guid id, PolicyScenario scenario)
		{
			throw new NotImplementedException();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.folder != null)
			{
				this.folder.Dispose();
			}
		}

		private MailboxSession session;

		private Folder folder;

		private ComplianceItemContainer parent;

		private List<ComplianceItemContainer> parents;
	}
}
