using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Dsn
{
	[Cmdlet("New", "SystemMessage", DefaultParameterSetName = "Dsn", SupportsShouldProcess = true)]
	public sealed class NewSystemMessage : NewFixedNameSystemConfigurationObjectTask<SystemMessage>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Dsn")]
		public EnhancedStatusCode DsnCode
		{
			get
			{
				return (EnhancedStatusCode)base.Fields["DsnCode"];
			}
			set
			{
				base.Fields["DsnCode"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Quota")]
		public QuotaMessageType QuotaMessageType
		{
			get
			{
				return (QuotaMessageType)base.Fields["QuotaMessageType"];
			}
			set
			{
				base.Fields["QuotaMessageType"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Quota")]
		[Parameter(Mandatory = true, ParameterSetName = "Dsn")]
		public string Text
		{
			get
			{
				return this.DataObject.Text;
			}
			set
			{
				this.DataObject.Text = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Dsn")]
		public bool Internal
		{
			get
			{
				return (bool)base.Fields["Internal"];
			}
			set
			{
				base.Fields["Internal"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Dsn")]
		[Parameter(Mandatory = true, ParameterSetName = "Quota")]
		public CultureInfo Language
		{
			get
			{
				return (CultureInfo)base.Fields["Language"];
			}
			set
			{
				base.Fields["Language"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.DsnCode != null)
				{
					return Strings.ConfirmationMessageNewSystemMessageDsn(this.DsnCode.ToString(), this.Text, this.Internal.ToString(), this.Language.ToString());
				}
				return Strings.ConfirmationMessageNewSystemMessageQuota(this.QuotaMessageType.ToString(), this.Text, this.Language.ToString());
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				ADObjectId currentOrgContainerId = base.CurrentOrgContainerId;
				return SystemMessage.GetDsnCustomizationContainer(currentOrgContainerId);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			ADObjectId adobjectId = this.RootId as ADObjectId;
			ADObjectId childId = adobjectId.GetChildId(this.Language.LCID.ToString(NumberFormatInfo.InvariantInfo));
			ADObjectId childId3;
			if (this.DsnCode != null)
			{
				ADObjectId childId2 = childId.GetChildId(this.Internal ? "internal" : "external");
				childId3 = childId2.GetChildId(this.DsnCode.Value);
			}
			else
			{
				childId3 = childId.GetChildId(this.QuotaMessageType.ToString());
			}
			this.DataObject.SetId(childId3);
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		private const string DsnParameterSetName = "Dsn";

		private const string QuotaParameterSetName = "Quota";
	}
}
