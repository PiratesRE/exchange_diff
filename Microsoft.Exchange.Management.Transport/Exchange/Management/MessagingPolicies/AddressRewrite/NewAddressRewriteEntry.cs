using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MessagingPolicies.AddressRewrite
{
	[Cmdlet("new", "AddressRewriteEntry", SupportsShouldProcess = true)]
	public class NewAddressRewriteEntry : NewSystemConfigurationObjectTask<AddressRewriteEntry>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewAddressrewriteentry(base.Name.ToString(), this.InternalAddress.ToString(), this.ExternalAddress.ToString());
			}
		}

		public NewAddressRewriteEntry()
		{
			this.OutboundOnly = false;
		}

		[Parameter(Mandatory = true)]
		public string InternalAddress
		{
			get
			{
				return (string)base.Fields["InternalAddress"];
			}
			set
			{
				base.Fields["InternalAddress"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string ExternalAddress
		{
			get
			{
				return (string)base.Fields["ExternalAddress"];
			}
			set
			{
				base.Fields["ExternalAddress"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OutboundOnly
		{
			get
			{
				return (bool)base.Fields["OutboundOnly"];
			}
			set
			{
				base.Fields["OutboundOnly"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExceptionList
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["ExceptionList"];
			}
			set
			{
				base.Fields["ExceptionList"] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			AddressRewriteEntry addressRewriteEntry = (AddressRewriteEntry)base.PrepareDataObject();
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			configurationSession.UseConfigNC = false;
			ADObjectId adobjectId;
			try
			{
				adobjectId = Utils.ValidateEntryAddresses(this.InternalAddress, this.ExternalAddress, this.OutboundOnly, this.ExceptionList, configurationSession, null);
			}
			catch (ArgumentException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
				return null;
			}
			catch (FormatException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
				return null;
			}
			addressRewriteEntry.InternalAddress = this.InternalAddress;
			addressRewriteEntry.ExternalAddress = this.ExternalAddress;
			addressRewriteEntry.OutboundOnly = this.OutboundOnly;
			addressRewriteEntry.ExceptionList = this.ExceptionList;
			ADObjectId childId = adobjectId.GetChildId(base.Name);
			addressRewriteEntry.SetId(childId);
			return addressRewriteEntry;
		}
	}
}
