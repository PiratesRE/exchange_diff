using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "ClientAccessArray", SupportsShouldProcess = true)]
	public sealed class NewClientAccessArray : NewFixedNameSystemConfigurationObjectTask<ClientAccessArray>
	{
		public NewClientAccessArray()
		{
			this.arrayTaskCommon = new ClientAccessArrayTaskHelper(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Position = 0, Mandatory = true)]
		public string Name
		{
			get
			{
				return this.DataObject.Name;
			}
			set
			{
				this.DataObject.Name = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNull]
		public string ArrayDefinition
		{
			get
			{
				return (string)(base.Fields[ClientAccessArraySchema.ArrayDefinition] ?? string.Empty);
			}
			set
			{
				base.Fields[ClientAccessArraySchema.ArrayDefinition] = value;
			}
		}

		[ValidateRange(0, 2147483646)]
		[Parameter(Mandatory = false)]
		public int ServerCount
		{
			get
			{
				return (int)(base.Fields[ClientAccessArraySchema.ServerCount] ?? 0);
			}
			set
			{
				base.Fields[ClientAccessArraySchema.ServerCount] = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public AdSiteIdParameter Site
		{
			get
			{
				return (AdSiteIdParameter)base.Fields[ClientAccessArraySchema.Site];
			}
			set
			{
				base.Fields[ClientAccessArraySchema.Site] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				string site = (this.Site != null) ? this.Site.ToString() : ((ITopologyConfigurationSession)this.ConfigurationSession).GetLocalSite().Name;
				return Strings.ConfirmationMessageNewClientAccessArray(this.Name, this.ArrayDefinition, this.ServerCount, site);
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.Site != null)
			{
				this.siteObject = this.arrayTaskCommon.GetADSite(this.Site, (ITopologyConfigurationSession)this.ConfigurationSession, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADSite>));
			}
			else
			{
				this.siteObject = ((ITopologyConfigurationSession)this.ConfigurationSession).GetLocalSite();
				if (this.siteObject == null)
				{
					base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorSiteNotSpecifiedAndLocalSiteNotFound), ErrorCategory.ObjectNotFound, null);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			this.arrayTaskCommon.VerifyArrayUniqueness(base.DataSession, this.DataObject);
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ClientAccessArray clientAccessArray = (ClientAccessArray)base.PrepareDataObject();
			ADObjectId childId = ((ADObjectId)this.RootId).GetChildId(this.DataObject.Name);
			clientAccessArray.SetId(childId);
			clientAccessArray.ArrayDefinition = this.ArrayDefinition;
			clientAccessArray.ServerCount = this.ServerCount;
			clientAccessArray.Site = this.siteObject.Id;
			TaskLogger.LogExit();
			return clientAccessArray;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			ClientAccessArray clientAccessArray = (ClientAccessArray)dataObject;
			base.WriteResult(dataObject);
			TaskLogger.LogExit();
		}

		protected override ObjectId RootId
		{
			get
			{
				return ClientAccessArray.GetParentContainer((ITopologyConfigurationSession)this.ConfigurationSession);
			}
		}

		private ADSite siteObject;

		private readonly ClientAccessArrayTaskHelper arrayTaskCommon;
	}
}
