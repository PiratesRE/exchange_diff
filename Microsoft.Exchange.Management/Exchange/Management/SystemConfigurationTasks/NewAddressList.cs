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
	[Cmdlet("New", "AddressList", SupportsShouldProcess = true, DefaultParameterSetName = "PrecannedFilter")]
	public sealed class NewAddressList : NewAddressBookBase
	{
		protected override int MaxAddressLists
		{
			get
			{
				int result;
				try
				{
					result = checked(base.MaxAddressLists * 4);
				}
				catch (OverflowException)
				{
					result = int.MaxValue;
				}
				return result;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewAddressList(this.Name.ToString());
			}
		}

		[Parameter]
		public string DisplayName
		{
			get
			{
				return this.DataObject.DisplayName;
			}
			set
			{
				this.DataObject.DisplayName = value;
			}
		}

		[Parameter(Mandatory = true, Position = 0)]
		public new string Name
		{
			get
			{
				return (string)this.DataObject[AddressBookBaseSchema.Name];
			}
			set
			{
				this.DataObject[AddressBookBaseSchema.Name] = value;
			}
		}

		[Parameter]
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

		protected override ADObjectId GetContainerId()
		{
			ADObjectId result = null;
			if (this.Container != null)
			{
				IConfigurable dataObject = base.GetDataObject<AddressBookBase>(this.Container, base.DataSession, this.RootId, new LocalizedString?(Strings.ErrorAddressListNotFound(this.Container.ToString())), new LocalizedString?(Strings.ErrorAddressListNotUnique(this.Container.ToString())));
				if (!base.HasErrors)
				{
					result = (ADObjectId)dataObject.Identity;
				}
				return result;
			}
			return base.CurrentOrgContainerId.GetDescendantId(AddressList.RdnAlContainerToOrganization);
		}

		protected override void WriteResult(IConfigurable result)
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Identity
			});
			base.WriteResult(new AddressList((AddressBookBase)result));
			TaskLogger.LogExit();
		}

		protected override string ClonableTypeName
		{
			get
			{
				return typeof(AddressList).FullName;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return AddressList.FromDataObject((AddressBookBase)dataObject);
		}
	}
}
