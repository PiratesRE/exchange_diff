using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public sealed class AddressList : AddressListBase
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return AddressList.schema;
			}
		}

		public AddressList()
		{
		}

		public AddressList(AddressBookBase dataObject) : base(dataObject)
		{
		}

		internal static AddressList FromDataObject(AddressBookBase dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new AddressList(dataObject);
		}

		public string Container
		{
			get
			{
				return (string)this[AddressListSchema.Container];
			}
		}

		public string Path
		{
			get
			{
				return (string)this[AddressListSchema.Path];
			}
		}

		[Parameter(Mandatory = false)]
		public string DisplayName
		{
			get
			{
				return (string)this[AddressListSchema.DisplayName];
			}
			set
			{
				this[AddressListSchema.DisplayName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new string Name
		{
			get
			{
				return (string)this[AddressListSchema.Name];
			}
			set
			{
				this[AddressListSchema.Name] = value;
			}
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
			ADObjectNameCharacterConstraint adobjectNameCharacterConstraint = new ADObjectNameCharacterConstraint(new char[]
			{
				'\\'
			});
			ValidationError validationError = adobjectNameCharacterConstraint.Validate(this.Name, AddressBookBaseSchema.Name, this.propertyBag);
			if (validationError != null)
			{
				errors.Add(validationError);
			}
		}

		private static AddressListSchema schema = ObjectSchema.GetInstance<AddressListSchema>();

		public static readonly ADObjectId RdnAlContainerToOrganization = new ADObjectId("CN=All Address Lists,CN=Address Lists Container");
	}
}
