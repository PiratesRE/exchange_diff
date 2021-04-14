using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("New", "ActiveSyncDeviceAccessRule", SupportsShouldProcess = true)]
	public sealed class NewActiveSyncDeviceAccessRule : NewMultitenancyFixedNameSystemConfigurationObjectTask<ActiveSyncDeviceAccessRule>
	{
		[Parameter(Mandatory = true)]
		public DeviceAccessLevel AccessLevel
		{
			get
			{
				return this.DataObject.AccessLevel;
			}
			set
			{
				this.DataObject.AccessLevel = value;
			}
		}

		[Parameter(Mandatory = true)]
		public DeviceAccessCharacteristic Characteristic
		{
			get
			{
				return this.DataObject.Characteristic;
			}
			set
			{
				this.DataObject.Characteristic = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string QueryString
		{
			get
			{
				return this.DataObject.QueryString;
			}
			set
			{
				this.DataObject.QueryString = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewActiveSyncDeviceAccessRule(this.BuildRuleName());
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			this.DataObject.Name = this.BuildRuleName();
			ActiveSyncDeviceAccessRule activeSyncDeviceAccessRule = (ActiveSyncDeviceAccessRule)base.PrepareDataObject();
			activeSyncDeviceAccessRule.SetId((IConfigurationSession)base.DataSession, this.DataObject.Name);
			return activeSyncDeviceAccessRule;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (Datacenter.IsMultiTenancyEnabled() && this.DataObject.OrganizationId == OrganizationId.ForestWideOrgId && this.AccessLevel != DeviceAccessLevel.Block)
			{
				base.WriteError(new ArgumentException(Strings.ErrorOnlyForestWideBlockIsAllowed), ErrorCategory.InvalidArgument, null);
			}
		}

		private string BuildRuleName()
		{
			string text = string.Format(CultureInfo.InvariantCulture, "{0} ({1})", new object[]
			{
				this.QueryString,
				this.Characteristic
			});
			if (text.Length > 64)
			{
				string text2 = string.Format(CultureInfo.InvariantCulture, "{0} ({1})", new object[]
				{
					text.GetHashCode().ToString(),
					this.Characteristic
				});
				text = text.Substring(0, 64 - text2.Length) + text2;
			}
			return text;
		}

		private const int MaxRuleNameLength = 64;
	}
}
