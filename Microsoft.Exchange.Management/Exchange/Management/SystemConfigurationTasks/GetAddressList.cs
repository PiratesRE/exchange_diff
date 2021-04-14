using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "AddressList", DefaultParameterSetName = "Identity")]
	public sealed class GetAddressList : GetMultitenancySystemConfigurationObjectTask<AddressListIdParameter, AddressBookBase>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Container")]
		public AddressListIdParameter Container
		{
			get
			{
				return (AddressListIdParameter)base.Fields["Container"];
			}
			set
			{
				base.Fields["Container"] = value;
			}
		}

		[Parameter(ParameterSetName = "SearchSet")]
		public string SearchText { get; set; }

		protected override ObjectId RootId
		{
			get
			{
				if (this.Identity == null)
				{
					return this.rootId ?? base.RootId;
				}
				return null;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				if (this.Identity == null)
				{
					return new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Id, base.CurrentOrgContainerId.GetDescendantId(AddressList.RdnAlContainerToOrganization));
				}
				return null;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.Container != null)
			{
				this.containerAddressBook = (AddressBookBase)base.GetDataObject<AddressBookBase>(this.Container, this.ConfigurationSession, this.RootId, new LocalizedString?(Strings.ErrorAddressListNotFound(this.Container.ToString())), new LocalizedString?(Strings.ErrorAddressListNotUnique(this.Container.ToString())));
				this.rootId = this.containerAddressBook.Id;
			}
			else
			{
				this.rootId = AddressListIdParameter.GetRootContainerId(this.ConfigurationSession, base.CurrentOrganizationId);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			if (!base.Stopping && this.Container != null)
			{
				this.WriteResult(this.containerAddressBook);
			}
			base.InternalProcessRecord();
		}

		protected override bool DeepSearch
		{
			get
			{
				return this.Container == null;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			AddressBookBase addressBookBase = (AddressBookBase)dataObject;
			bool flag = this.SearchText == null || addressBookBase.DisplayName.IndexOf(this.SearchText, StringComparison.OrdinalIgnoreCase) > 0 || addressBookBase.Path.IndexOf(this.SearchText, StringComparison.OrdinalIgnoreCase) > 0;
			if (flag)
			{
				base.WriteResult(new AddressList(addressBookBase));
			}
			TaskLogger.LogExit();
		}

		private ADObjectId rootId;

		private AddressBookBase containerAddressBook;
	}
}
