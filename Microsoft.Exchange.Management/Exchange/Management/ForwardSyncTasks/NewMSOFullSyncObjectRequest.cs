using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("New", "MSOFullSyncObjectRequest", SupportsShouldProcess = true)]
	public sealed class NewMSOFullSyncObjectRequest : NewTaskBase<FullSyncObjectRequest>
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public SyncObjectId ObjectId
		{
			get
			{
				return (SyncObjectId)base.Fields["SyncObjectIdParameter"];
			}
			set
			{
				base.Fields["SyncObjectIdParameter"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public ServiceInstanceId ServiceInstanceId
		{
			get
			{
				return (ServiceInstanceId)base.Fields["ServiceInstanceIdParameter"];
			}
			set
			{
				base.Fields["ServiceInstanceIdParameter"] = value;
			}
		}

		[Parameter]
		public FullSyncObjectRequestOptions Options
		{
			get
			{
				return (FullSyncObjectRequestOptions)(base.Fields["OptionsParameter"] ?? FullSyncObjectRequestOptions.None);
			}
			set
			{
				base.Fields["OptionsParameter"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewMSOFullSyncRequest(this.DataObject.Identity.ToString(), this.DataObject.ServiceInstanceId, this.DataObject.Options.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return new FullSyncObjectRequestDataProvider(false, this.ServiceInstanceId.InstanceId);
		}

		protected override IConfigurable PrepareDataObject()
		{
			FullSyncObjectRequest fullSyncObjectRequest = (FullSyncObjectRequest)base.PrepareDataObject();
			fullSyncObjectRequest.SetIdentity(this.ObjectId);
			fullSyncObjectRequest.ServiceInstanceId = this.ServiceInstanceId.InstanceId;
			fullSyncObjectRequest.Options = this.Options;
			fullSyncObjectRequest.CreationTime = ExDateTime.UtcNow;
			return fullSyncObjectRequest;
		}

		protected override void InternalValidate()
		{
			Exception innerException;
			if (!NewMSOFullSyncObjectRequest.IsValidGuid(this.ObjectId.ContextId, out innerException) || !NewMSOFullSyncObjectRequest.IsValidGuid(this.ObjectId.ObjectId, out innerException))
			{
				base.WriteError(new LocalizedException(DirectoryStrings.ExArgumentException("ObjectId", this.ObjectId), innerException), ExchangeErrorCategory.Client, null);
			}
			Guid externalDirectoryOrganizationId = new Guid(this.ObjectId.ContextId);
			ITenantConfigurationSession tenantConfigurationSession = null;
			try
			{
				tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromExternalDirectoryOrganizationId(externalDirectoryOrganizationId), 134, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ForwardSync\\NewMSOFullSyncObjectRequest.cs");
			}
			catch (CannotResolveExternalDirectoryOrganizationIdException)
			{
			}
			if (tenantConfigurationSession != null)
			{
				ExchangeConfigurationUnit exchangeConfigurationUnitByExternalId = tenantConfigurationSession.GetExchangeConfigurationUnitByExternalId(this.ObjectId.ContextId);
				if (exchangeConfigurationUnitByExternalId != null && !StringComparer.OrdinalIgnoreCase.Equals(exchangeConfigurationUnitByExternalId.DirSyncServiceInstance, this.ServiceInstanceId.InstanceId))
				{
					base.WriteError(new ServiceInstanceNotMatchException(this.ObjectId.ToString(), this.ServiceInstanceId.InstanceId, exchangeConfigurationUnitByExternalId.DirSyncServiceInstance), ExchangeErrorCategory.Client, this.ObjectId);
				}
			}
			base.InternalValidate();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is RidMasterConfigException;
		}

		private static bool IsValidGuid(string guidString, out Exception parseException)
		{
			parseException = null;
			try
			{
				new Guid(guidString);
			}
			catch (FormatException ex)
			{
				parseException = ex;
				return false;
			}
			catch (OverflowException ex2)
			{
				parseException = ex2;
				return false;
			}
			return true;
		}

		private const string SyncObjectIdParameter = "SyncObjectIdParameter";

		private const string ServiceInstanceIdParameter = "ServiceInstanceIdParameter";

		private const string OptionsParameter = "OptionsParameter";
	}
}
