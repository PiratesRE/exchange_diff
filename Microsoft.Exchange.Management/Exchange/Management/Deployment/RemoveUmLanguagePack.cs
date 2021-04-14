using System;
using System.Management.Automation;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("remove", "umlanguagepack", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class RemoveUmLanguagePack : ManageUmLanguagePack
	{
		[Parameter(Mandatory = true)]
		public Guid ProductCode
		{
			get
			{
				return (Guid)base.Fields["ProductCode"];
			}
			set
			{
				base.Fields["ProductCode"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid TeleProductCode
		{
			get
			{
				return (Guid)base.Fields["TeleProductCode"];
			}
			set
			{
				base.Fields["TeleProductCode"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid TransProductCode
		{
			get
			{
				return (Guid)base.Fields["TransProductCode"];
			}
			set
			{
				base.Fields["TransProductCode"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid TtsProductCode
		{
			get
			{
				return (Guid)base.Fields["TtsProductCode"];
			}
			set
			{
				base.Fields["TtsProductCode"] = value;
			}
		}

		public RemoveUmLanguagePack()
		{
			base.Fields["InstallationMode"] = InstallationModes.Uninstall;
		}
	}
}
