using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.AirSync;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class SetCASMailboxBase<TIdentity, TPublicObject> : SetRecipientObjectTask<TIdentity, TPublicObject, ADUser> where TIdentity : IIdentityParameter, new() where TPublicObject : IConfigurable, new()
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				TIdentity identity = this.Identity;
				return Strings.ConfirmationMessageSetCASMailbox(identity.ToString());
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADRecipient adrecipient = (ADRecipient)base.ResolveDataObject();
			if (MailboxTaskHelper.ExcludeArbitrationMailbox(adrecipient, false))
			{
				TIdentity identity = this.Identity;
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(identity.ToString(), typeof(ADUser).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), ErrorCategory.InvalidData, this.Identity);
			}
			return adrecipient;
		}

		public SetCASMailboxBase()
		{
		}

		[Parameter(Mandatory = false)]
		public MailboxPolicyIdParameter ActiveSyncMailboxPolicy
		{
			get
			{
				return (MailboxPolicyIdParameter)base.Fields[CASMailboxSchema.ActiveSyncMailboxPolicy];
			}
			set
			{
				base.Fields[CASMailboxSchema.ActiveSyncMailboxPolicy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxPolicyIdParameter OwaMailboxPolicy
		{
			get
			{
				return (MailboxPolicyIdParameter)base.Fields[CASMailboxSchema.OwaMailboxPolicy];
			}
			set
			{
				base.Fields[CASMailboxSchema.OwaMailboxPolicy] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.Fields.IsModified(CASMailboxSchema.ActiveSyncMailboxPolicy))
			{
				if (this.ActiveSyncMailboxPolicy != null)
				{
					MobileMailboxPolicy mobileMailboxPolicy = (MobileMailboxPolicy)base.GetDataObject<MobileMailboxPolicy>(this.ActiveSyncMailboxPolicy, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorMobileMailboxPolicyNotFound(this.ActiveSyncMailboxPolicy.ToString())), new LocalizedString?(Strings.ErrorMobileMailboxPolicyNotUnique(this.ActiveSyncMailboxPolicy.ToString())));
					this.DataObject.ActiveSyncMailboxPolicy = (ADObjectId)mobileMailboxPolicy.Identity;
				}
				else
				{
					this.DataObject.ActiveSyncMailboxPolicy = null;
				}
			}
			if (base.Fields.IsModified(CASMailboxSchema.OwaMailboxPolicy))
			{
				if (this.DataObject.ExchangeVersion.Equals(ExchangeObjectVersion.Exchange2007))
				{
					base.WriteError(new ArgumentException(Strings.ErrorSetOWAMailboxPolicyToE12User), ErrorCategory.InvalidArgument, this.Identity);
				}
				if (this.OwaMailboxPolicy != null)
				{
					OwaMailboxPolicy owaMailboxPolicy = (OwaMailboxPolicy)base.GetDataObject<OwaMailboxPolicy>(this.OwaMailboxPolicy, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorOwaMailboxPolicyNotFound(this.OwaMailboxPolicy.ToString())), new LocalizedString?(Strings.ErrorOwaMailboxPolicyNotUnique(this.OwaMailboxPolicy.ToString())));
					this.DataObject.OwaMailboxPolicy = (ADObjectId)owaMailboxPolicy.Identity;
				}
				else
				{
					this.DataObject.OwaMailboxPolicy = null;
				}
			}
			this.allowedDeviceIDs = this.DataObject.ActiveSyncAllowedDeviceIDs;
			this.blockedDeviceIDs = this.DataObject.ActiveSyncBlockedDeviceIDs;
			bool changed = this.allowedDeviceIDs.Changed;
			bool changed2 = this.blockedDeviceIDs.Changed;
			if (changed || changed2)
			{
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				this.deviceIdListsChanged = true;
				foreach (string text in this.allowedDeviceIDs)
				{
					foreach (string b in this.blockedDeviceIDs)
					{
						if (string.Equals(text, b, StringComparison.OrdinalIgnoreCase))
						{
							if (changed && changed2)
							{
								base.WriteError(new ErrorDeviceIdInBothLists(text), ErrorCategory.InvalidData, this.Identity);
							}
							else if (changed)
							{
								list2.Add(text);
							}
							else
							{
								list.Add(text);
							}
						}
					}
				}
				if (list.Count > 0)
				{
					foreach (string item in list)
					{
						this.allowedDeviceIDs.Remove(item);
					}
					this.DataObject.ActiveSyncAllowedDeviceIDs = this.allowedDeviceIDs;
				}
				if (list2.Count > 0)
				{
					foreach (string item2 in list2)
					{
						this.blockedDeviceIDs.Remove(item2);
					}
					this.DataObject.ActiveSyncBlockedDeviceIDs = this.blockedDeviceIDs;
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			if (this.deviceIdListsChanged)
			{
				this.UpdateActiveSyncDevices(this.allowedDeviceIDs, this.blockedDeviceIDs);
			}
			TaskLogger.LogExit();
		}

		private void UpdateActiveSyncDevices(MultiValuedProperty<string> allowedDeviceIDs, MultiValuedProperty<string> blockedDeviceIDs)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, null, base.OrgWideSessionSettings, ConfigScopes.TenantSubTree, 515, "UpdateActiveSyncDevices", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\cas\\SetCASMailbox.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			ADPagedReader<ActiveSyncDevice> adpagedReader = tenantOrTopologyConfigurationSession.FindPaged<ActiveSyncDevice>(this.DataObject.Id, QueryScope.SubTree, null, null, 0);
			foreach (ActiveSyncDevice activeSyncDevice in adpagedReader)
			{
				bool flag = false;
				foreach (string value in blockedDeviceIDs)
				{
					if (activeSyncDevice.DeviceId.Equals(value, StringComparison.OrdinalIgnoreCase))
					{
						if (activeSyncDevice.DeviceAccessState != DeviceAccessState.Blocked || activeSyncDevice.DeviceAccessStateReason != DeviceAccessStateReason.Individual)
						{
							activeSyncDevice.DeviceAccessState = DeviceAccessState.Blocked;
							activeSyncDevice.DeviceAccessStateReason = DeviceAccessStateReason.Individual;
							tenantOrTopologyConfigurationSession.Save(activeSyncDevice);
						}
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					foreach (string value2 in allowedDeviceIDs)
					{
						if (activeSyncDevice.DeviceId.Equals(value2, StringComparison.OrdinalIgnoreCase))
						{
							if (activeSyncDevice.DeviceAccessState != DeviceAccessState.Allowed || activeSyncDevice.DeviceAccessStateReason != DeviceAccessStateReason.Individual)
							{
								activeSyncDevice.DeviceAccessState = DeviceAccessState.Allowed;
								activeSyncDevice.DeviceAccessStateReason = DeviceAccessStateReason.Individual;
								tenantOrTopologyConfigurationSession.Save(activeSyncDevice);
							}
							flag = true;
							break;
						}
					}
					if (!flag && (activeSyncDevice.DeviceAccessState == DeviceAccessState.Allowed || activeSyncDevice.DeviceAccessState == DeviceAccessState.Blocked) && activeSyncDevice.DeviceAccessStateReason == DeviceAccessStateReason.Individual)
					{
						activeSyncDevice.DeviceAccessState = DeviceAccessState.Unknown;
						activeSyncDevice.DeviceAccessStateReason = DeviceAccessStateReason.Unknown;
						tenantOrTopologyConfigurationSession.Save(activeSyncDevice);
					}
				}
			}
		}

		private MultiValuedProperty<string> allowedDeviceIDs;

		private MultiValuedProperty<string> blockedDeviceIDs;

		private bool deviceIdListsChanged;
	}
}
