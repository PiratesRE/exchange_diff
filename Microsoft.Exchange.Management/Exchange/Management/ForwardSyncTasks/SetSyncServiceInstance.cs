using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.Sync.CookieManager;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("Set", "SyncServiceInstance", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class SetSyncServiceInstance : SetADTaskBase<ServiceInstanceIdParameter, SyncServiceInstance, SyncServiceInstance>
	{
		[Parameter(Mandatory = false)]
		public AccountPartitionIdParameter AccountPartition
		{
			get
			{
				return (AccountPartitionIdParameter)base.Fields[SyncServiceInstanceSchema.AccountPartition];
			}
			set
			{
				base.Fields[SyncServiceInstanceSchema.AccountPartition] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force { get; set; }

		protected override ObjectId RootId
		{
			get
			{
				return SyncServiceInstance.GetMsoSyncRootContainer();
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetSyncServiceInstance(this.DataObject.Name);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return ForwardSyncDataAccessHelper.CreateSession(false);
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			SyncServiceInstance syncServiceInstance = (SyncServiceInstance)base.PrepareDataObject();
			if (syncServiceInstance.IsChanged(ADObjectSchema.Name))
			{
				string text;
				syncServiceInstance.TryGetOriginalValue<string>(ADObjectSchema.Name, out text);
				if (!this.Force && !SetSyncServiceInstance.IsServiceInstanceEmpty(syncServiceInstance))
				{
					base.WriteError(new InvalidOperationException(Strings.CannotChangeServiceInstanceNameError(text)), ErrorCategory.InvalidOperation, text);
				}
			}
			if (syncServiceInstance.IsChanged(SyncServiceInstanceSchema.ForwardSyncConfigurationXML))
			{
				string forwardSyncConfigurationXML = syncServiceInstance.ForwardSyncConfigurationXML;
				try
				{
					XElement.Parse(forwardSyncConfigurationXML);
				}
				catch (XmlException ex)
				{
					base.WriteError(new InvalidOperationException(Strings.InvalidForwardSyncConfigurationError(ex.Message)), ErrorCategory.InvalidOperation, ex.Message);
				}
			}
			if (base.Fields.IsModified(SyncServiceInstanceSchema.AccountPartition))
			{
				if (this.AccountPartition != null)
				{
					AccountPartition accountPartition = (AccountPartition)base.GetDataObject<AccountPartition>(this.AccountPartition, this.ConfigurationSession, null, null, null);
					syncServiceInstance.AccountPartition = accountPartition.Id;
				}
				else
				{
					syncServiceInstance.AccountPartition = null;
				}
			}
			TaskLogger.LogExit();
			return syncServiceInstance;
		}

		internal static bool IsServiceInstanceEmpty(SyncServiceInstance syncServiceInstance)
		{
			TaskLogger.LogEnter();
			CookieManager cookieManager = CookieManagerFactory.Default.GetCookieManager(ForwardSyncCookieType.RecipientIncremental, syncServiceInstance.Name, 1, TimeSpan.FromMinutes(30.0));
			byte[] array = cookieManager.ReadCookie();
			if (array != null && array.Length > 0)
			{
				return false;
			}
			CookieManager cookieManager2 = CookieManagerFactory.Default.GetCookieManager(ForwardSyncCookieType.CompanyIncremental, syncServiceInstance.Name, 1, TimeSpan.FromMinutes(30.0));
			array = cookieManager2.ReadCookie();
			if (array != null && array.Length > 0)
			{
				return false;
			}
			ForwardSyncDataAccessHelper forwardSyncDataAccessHelper = new ForwardSyncDataAccessHelper(syncServiceInstance.Name);
			IEnumerable<FailedMSOSyncObject> source = forwardSyncDataAccessHelper.FindDivergence(null);
			if (source.Any<FailedMSOSyncObject>())
			{
				return false;
			}
			TaskLogger.LogExit();
			return true;
		}
	}
}
