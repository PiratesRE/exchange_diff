using System;
using System.Linq;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExBindingStoreObjectProvider : TenantStoreDataProvider
	{
		public ExBindingStoreObjectProvider(ExPolicyConfigProvider policyConfigProvider) : base(policyConfigProvider.GetOrganizationId())
		{
			ArgumentValidator.ThrowIfNull("policyConfigProvider", policyConfigProvider);
			this.policyConfigProvider = policyConfigProvider;
		}

		protected override FolderId GetDefaultFolder()
		{
			if (this.containerFolderId == null)
			{
				this.containerFolderId = base.GetOrCreateFolder("UnifiedPolicyBindings", new FolderId(10, new Mailbox(base.Mailbox.MailboxInfo.PrimarySmtpAddress.ToString()))).Id;
			}
			return this.containerFolderId;
		}

		public BindingStorage FindBindingStorageByPolicyId(Guid policyId)
		{
			BindingStorage result = null;
			ExBindingStoreObject exBindingStoreObject = this.FindByAlternativeId<ExBindingStoreObject>(policyId.ToString());
			if (exBindingStoreObject != null)
			{
				result = exBindingStoreObject.ToBindingStorage(this.policyConfigProvider);
			}
			return result;
		}

		public BindingStorage FindBindingStorageById(string identity)
		{
			BindingStorage result = null;
			SearchFilter filter = new SearchFilter.IsEqualTo(ExBindingStoreObjectSchema.MasterIdentity.StorePropertyDefinition, identity);
			ExBindingStoreObject exBindingStoreObject = this.InternalFindPaged<ExBindingStoreObject>(filter, this.GetDefaultFolder(), false, null, 1, new ProviderPropertyDefinition[0]).FirstOrDefault<ExBindingStoreObject>();
			if (exBindingStoreObject != null)
			{
				result = exBindingStoreObject.ToBindingStorage(this.policyConfigProvider);
			}
			return result;
		}

		public void SaveBindingStorage(BindingStorage bindingStorage)
		{
			ArgumentValidator.ThrowIfNull("bindingStorage", bindingStorage);
			if (bindingStorage.ObjectState == ObjectState.Unchanged)
			{
				return;
			}
			ExBindingStoreObject exBindingStoreObject = bindingStorage.RawObject as ExBindingStoreObject;
			if (bindingStorage.ObjectState == ObjectState.New)
			{
				exBindingStoreObject = new ExBindingStoreObject();
			}
			if (exBindingStoreObject == null)
			{
				throw new InvalidOperationException("BindingStorage has no associated ExBindingStoreObject to save.");
			}
			exBindingStoreObject.FromBindingStorage(bindingStorage, this.policyConfigProvider);
			this.Save(exBindingStoreObject);
		}

		public void DeleteBindingStorage(BindingStorage bindingStorage)
		{
			ArgumentValidator.ThrowIfNull("bindingStorage", bindingStorage);
			ExBindingStoreObject exBindingStoreObject = bindingStorage.RawObject as ExBindingStoreObject;
			if (exBindingStoreObject == null)
			{
				throw new InvalidOperationException("BindingStorage has no associated ExBindingStoreObject, cannot be deleted.");
			}
			base.Delete(exBindingStoreObject);
		}

		public const string ContainerFolderName = "UnifiedPolicyBindings";

		private FolderId containerFolderId;

		private ExPolicyConfigProvider policyConfigProvider;
	}
}
