using System;
using System.Management.Automation;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class WindowsGroup : ADPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return WindowsGroup.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public WindowsGroup()
		{
		}

		public WindowsGroup(ADGroup dataObject) : base(dataObject)
		{
		}

		internal static WindowsGroup FromDataObject(ADGroup dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new WindowsGroup(dataObject);
		}

		[Parameter(Mandatory = false)]
		public string DisplayName
		{
			get
			{
				return (string)this[WindowsGroupSchema.DisplayName];
			}
			set
			{
				this[WindowsGroupSchema.DisplayName] = value;
			}
		}

		public GroupTypeFlags GroupType
		{
			get
			{
				return (GroupTypeFlags)this[WindowsGroupSchema.GroupType];
			}
		}

		public MultiValuedProperty<ADObjectId> ManagedBy
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[WindowsGroupSchema.ManagedBy];
			}
			set
			{
				this[WindowsGroupSchema.ManagedBy] = value;
			}
		}

		public string SamAccountName
		{
			get
			{
				return (string)this[WindowsGroupSchema.SamAccountName];
			}
			set
			{
				this[WindowsGroupSchema.SamAccountName] = value;
			}
		}

		public SecurityIdentifier Sid
		{
			get
			{
				return (SecurityIdentifier)this[WindowsGroupSchema.Sid];
			}
		}

		public MultiValuedProperty<SecurityIdentifier> SidHistory
		{
			get
			{
				return (MultiValuedProperty<SecurityIdentifier>)this[WindowsGroupSchema.SidHistory];
			}
		}

		[Parameter(Mandatory = false)]
		public string SimpleDisplayName
		{
			get
			{
				return (string)this[WindowsGroupSchema.SimpleDisplayName];
			}
			set
			{
				this[WindowsGroupSchema.SimpleDisplayName] = value;
			}
		}

		public RecipientType RecipientType
		{
			get
			{
				return (RecipientType)this[WindowsGroupSchema.RecipientType];
			}
		}

		public RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails)this[WindowsGroupSchema.RecipientTypeDetails];
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress WindowsEmailAddress
		{
			get
			{
				return (SmtpAddress)this[WindowsGroupSchema.WindowsEmailAddress];
			}
			set
			{
				this[WindowsGroupSchema.WindowsEmailAddress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Notes
		{
			get
			{
				return (string)this[WindowsGroupSchema.Notes];
			}
			set
			{
				this[WindowsGroupSchema.Notes] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> Members
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[WindowsGroupSchema.Members];
			}
			set
			{
				this[WindowsGroupSchema.Members] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PhoneticDisplayName
		{
			get
			{
				return (string)this[WindowsGroupSchema.PhoneticDisplayName];
			}
			set
			{
				this[WindowsGroupSchema.PhoneticDisplayName] = value;
			}
		}

		public string OrganizationalUnit
		{
			get
			{
				return (string)this[WindowsGroupSchema.OrganizationalUnit];
			}
		}

		[Parameter(Mandatory = false)]
		public int? SeniorityIndex
		{
			get
			{
				return (int?)this[WindowsGroupSchema.SeniorityIndex];
			}
			set
			{
				this[WindowsGroupSchema.SeniorityIndex] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsHierarchicalGroup
		{
			get
			{
				return (bool)this[WindowsGroupSchema.IsHierarchicalGroup];
			}
			set
			{
				this[WindowsGroupSchema.IsHierarchicalGroup] = value;
			}
		}

		private static WindowsGroupSchema schema = ObjectSchema.GetInstance<WindowsGroupSchema>();
	}
}
