using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public abstract class DistributionGroupBase : MailEnabledRecipient
	{
		protected DistributionGroupBase()
		{
		}

		protected DistributionGroupBase(ADObject dataObject) : base(dataObject)
		{
		}

		public string ExpansionServer
		{
			get
			{
				return (string)this[DistributionGroupBaseSchema.ExpansionServer];
			}
			set
			{
				this[DistributionGroupBaseSchema.ExpansionServer] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ReportToManagerEnabled
		{
			get
			{
				return (bool)this[DistributionGroupBaseSchema.ReportToManagerEnabled];
			}
			set
			{
				this[DistributionGroupBaseSchema.ReportToManagerEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ReportToOriginatorEnabled
		{
			get
			{
				return (bool)this[DistributionGroupBaseSchema.ReportToOriginatorEnabled];
			}
			set
			{
				this[DistributionGroupBaseSchema.ReportToOriginatorEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SendOofMessageToOriginatorEnabled
		{
			get
			{
				return (bool)this[DistributionGroupBaseSchema.SendOofMessageToOriginatorEnabled];
			}
			set
			{
				this[DistributionGroupBaseSchema.SendOofMessageToOriginatorEnabled] = value;
			}
		}
	}
}
