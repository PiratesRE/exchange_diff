using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class BaseOrganization : SetMultitenancySingletonSystemConfigurationObjectTask<ADOrganizationConfig>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetOrganizationConfig;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		[Parameter(Mandatory = false)]
		public int SCLJunkThreshold
		{
			get
			{
				return (int)base.Fields["SCLJunkThreshold"];
			}
			set
			{
				base.Fields["SCLJunkThreshold"] = value;
			}
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			Organization organization = (Organization)dataObject;
			if (base.Fields.IsModified("SCLJunkThreshold"))
			{
				this.uceContentFilter = this.FindUceContentFilter(organization.Identity);
				if (this.uceContentFilter != null)
				{
					organization.SCLJunkThreshold = this.uceContentFilter.SCLJunkThreshold;
					organization.ResetChangeTracking();
				}
				else
				{
					base.WriteVerbose(Strings.UceContentFilterObjectNotFound);
				}
			}
			base.StampChangesOn(dataObject);
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			Organization organization = (Organization)base.PrepareDataObject();
			if (base.Fields.IsModified("SCLJunkThreshold") && this.uceContentFilter != null)
			{
				organization.SCLJunkThreshold = this.SCLJunkThreshold;
				this.uceContentFilter.SCLJunkThreshold = this.SCLJunkThreshold;
			}
			TaskLogger.LogExit();
			return organization;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (base.Fields.IsModified("SCLJunkThreshold") && this.uceContentFilter != null)
			{
				base.DataSession.Save(this.uceContentFilter);
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private UceContentFilter FindUceContentFilter(ObjectId rootId)
		{
			UceContentFilter result = null;
			IConfigurable[] array = base.DataSession.Find<UceContentFilter>(null, rootId, true, null);
			if (array != null && array.Length > 0)
			{
				result = (array[0] as UceContentFilter);
			}
			return result;
		}

		private const string SCLJunkThresholdParameterName = "SCLJunkThreshold";

		private UceContentFilter uceContentFilter;
	}
}
