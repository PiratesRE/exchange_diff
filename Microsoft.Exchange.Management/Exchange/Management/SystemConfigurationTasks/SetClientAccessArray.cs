using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "ClientAccessArray", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetClientAccessArray : SetTopologySystemConfigurationObjectTask<ClientAccessArrayIdParameter, ClientAccessArray>
	{
		public SetClientAccessArray()
		{
			this.arrayTaskCommon = new ClientAccessArrayTaskHelper(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskErrorLoggingDelegate(base.WriteError));
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNull]
		public string ArrayDefinition
		{
			get
			{
				return (string)base.Fields[ClientAccessArraySchema.ArrayDefinition];
			}
			set
			{
				base.Fields[ClientAccessArraySchema.ArrayDefinition] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateRange(0, 2147483646)]
		public int ServerCount
		{
			get
			{
				return (int)base.Fields[ClientAccessArraySchema.ServerCount];
			}
			set
			{
				base.Fields[ClientAccessArraySchema.ServerCount] = value;
			}
		}

		[Parameter(Mandatory = false)]
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
				return Strings.ConfirmationMessageSetClientAccessArray(this.Identity.ToString());
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ClientAccessArray clientAccessArray = (ClientAccessArray)base.PrepareDataObject();
			if (this.siteObject != null)
			{
				clientAccessArray.Site = this.siteObject.Id;
			}
			if (base.Fields.IsModified(ClientAccessArraySchema.ArrayDefinition))
			{
				clientAccessArray.ArrayDefinition = this.ArrayDefinition;
			}
			if (base.Fields.IsModified(ClientAccessArraySchema.ServerCount))
			{
				clientAccessArray.ServerCount = this.ServerCount;
			}
			TaskLogger.LogExit();
			return clientAccessArray;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.Site != null)
			{
				this.siteObject = this.arrayTaskCommon.GetADSite(this.Site, (ITopologyConfigurationSession)this.ConfigurationSession, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADSite>));
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.DataObject.ExchangeVersion.IsOlderThan(ClientAccessArray.MinimumSupportedExchangeObjectVersion))
			{
				base.WriteError(new TaskException(Strings.ErrorCannotChangeBecauseTooOld(this.DataObject.ExchangeVersion.ToString(), ClientAccessArray.MinimumSupportedExchangeObjectVersion.ToString())), ErrorCategory.InvalidArgument, null);
			}
			this.arrayTaskCommon.VerifyArrayUniqueness(base.DataSession, this.DataObject);
			TaskLogger.LogExit();
		}

		private ADSite siteObject;

		private readonly ClientAccessArrayTaskHelper arrayTaskCommon;
	}
}
