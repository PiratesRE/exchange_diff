using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Enable", "RemoteMailbox", SupportsShouldProcess = true, DefaultParameterSetName = "EnabledUser")]
	public sealed class EnableRemoteMailbox : EnableMailUserBase
	{
		[Parameter(Mandatory = false, ParameterSetName = "Shared")]
		[Parameter(Mandatory = false, ParameterSetName = "Room")]
		[Parameter(Mandatory = false, ParameterSetName = "EnabledUser")]
		[Parameter(Mandatory = false, ParameterSetName = "Equipment")]
		public ProxyAddress RemoteRoutingAddress
		{
			get
			{
				return (ProxyAddress)base.Fields[ADRecipientSchema.ExternalEmailAddress];
			}
			set
			{
				base.Fields[ADRecipientSchema.ExternalEmailAddress] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Archive")]
		public SwitchParameter Archive
		{
			get
			{
				return (SwitchParameter)(base.Fields["Archive"] ?? false);
			}
			set
			{
				base.Fields["Archive"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Archive")]
		public MultiValuedProperty<string> ArchiveName
		{
			get
			{
				return base.Fields[ADUserSchema.ArchiveName] as MultiValuedProperty<string>;
			}
			set
			{
				base.Fields[ADUserSchema.ArchiveName] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Room")]
		public SwitchParameter Room
		{
			get
			{
				return (SwitchParameter)(base.Fields["Room"] ?? false);
			}
			set
			{
				base.Fields["Room"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Equipment")]
		public SwitchParameter Equipment
		{
			get
			{
				return (SwitchParameter)(base.Fields["Equipment"] ?? false);
			}
			set
			{
				base.Fields["Equipment"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Shared")]
		public SwitchParameter Shared
		{
			get
			{
				return (SwitchParameter)(base.Fields["Shared"] ?? false);
			}
			set
			{
				base.Fields["Shared"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ACLableSyncedObjectEnabled
		{
			get
			{
				return (SwitchParameter)(base.Fields["ACLableSyncedObjectEnabled"] ?? false);
			}
			set
			{
				base.Fields["ACLableSyncedObjectEnabled"] = value;
			}
		}

		private new SwitchParameter OverrideRecipientQuotas
		{
			get
			{
				return base.OverrideRecipientQuotas;
			}
		}

		public override Capability SKUCapability
		{
			get
			{
				return base.SKUCapability;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override MultiValuedProperty<Capability> AddOnSKUCapability
		{
			get
			{
				return base.AddOnSKUCapability;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override bool SKUAssigned
		{
			get
			{
				return base.SKUAssigned;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		protected override bool IsValidUser(ADUser user)
		{
			return ("Archive" != base.ParameterSetName && (RecipientType.User == user.RecipientType || RecipientType.MailUser == user.RecipientType)) || ("Archive" == base.ParameterSetName && RecipientType.MailUser == user.RecipientType);
		}

		protected override void PrepareRecipientObject(ref ADUser user)
		{
			TaskLogger.LogEnter();
			base.PrepareRecipientObject(ref user);
			if ("EnabledUser" == base.ParameterSetName || "Room" == base.ParameterSetName || "Equipment" == base.ParameterSetName || "Shared" == base.ParameterSetName)
			{
				if (RemoteMailbox.IsRemoteMailbox(user.RecipientTypeDetails))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorInvalidRecipientType(user.Identity.ToString(), user.RecipientTypeDetails.ToString())), ErrorCategory.InvalidArgument, user.Id);
				}
				if (null == this.RemoteRoutingAddress)
				{
					if (user.RecipientType == RecipientType.User)
					{
						if (this.remoteRoutingAddressGenerator == null)
						{
							this.remoteRoutingAddressGenerator = new RemoteRoutingAddressGenerator(this.ConfigurationSession);
						}
						user.ExternalEmailAddress = this.remoteRoutingAddressGenerator.GenerateRemoteRoutingAddress(user.Alias, new Task.ErrorLoggerDelegate(base.WriteError));
					}
				}
				else
				{
					user.ExternalEmailAddress = this.RemoteRoutingAddress;
				}
				user.RemoteRecipientType = RemoteRecipientType.ProvisionMailbox;
				RemoteMailboxType remoteMailboxType = (RemoteMailboxType)((ulong)int.MinValue);
				if (this.Room.IsPresent)
				{
					remoteMailboxType = RemoteMailboxType.Room;
				}
				else if (this.Equipment.IsPresent)
				{
					remoteMailboxType = RemoteMailboxType.Equipment;
				}
				else if (this.Shared.IsPresent)
				{
					remoteMailboxType = RemoteMailboxType.Shared;
				}
				user.UpdateRemoteMailboxType(remoteMailboxType, this.ACLableSyncedObjectEnabled);
				user.SetExchangeVersion(ExchangeObjectVersion.Current);
			}
			if ("Archive" == base.ParameterSetName)
			{
				if (!RemoteMailbox.IsRemoteMailbox(user.RecipientTypeDetails))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorInvalidRecipientType(user.Identity.ToString(), user.RecipientTypeDetails.ToString())), ErrorCategory.InvalidArgument, user.Id);
				}
				if ((user.RemoteRecipientType & RemoteRecipientType.ProvisionArchive) != RemoteRecipientType.ProvisionArchive)
				{
					if (user.ArchiveGuid == Guid.Empty)
					{
						if (user.DisabledArchiveGuid != Guid.Empty)
						{
							user.ArchiveGuid = user.DisabledArchiveGuid;
						}
						else
						{
							user.ArchiveGuid = Guid.NewGuid();
						}
					}
					if (this.ArchiveName == null)
					{
						if (user.ArchiveName == null || user.ArchiveName.Count == 0)
						{
							user.ArchiveName = new MultiValuedProperty<string>(Strings.ArchiveNamePrefix + user.DisplayName);
						}
					}
					else
					{
						user.ArchiveName = this.ArchiveName;
					}
					user.RemoteRecipientType = ((user.RemoteRecipientType &= ~RemoteRecipientType.DeprovisionArchive) | RemoteRecipientType.ProvisionArchive);
					TaskLogger.LogExit();
					return;
				}
				base.WriteError(new RecipientTaskException(Strings.ErrorArchiveAlreadyPresent(this.Identity.ToString())), (ErrorCategory)1003, null);
			}
			TaskLogger.LogExit();
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if ("Archive" == base.ParameterSetName)
				{
					return Strings.ConfirmationMessageEnableMailUserArchive(this.Identity.ToString());
				}
				return Strings.ConfirmationMessageEnableRemoteMailbox(this.Identity.ToString());
			}
		}

		protected override void WriteResult()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Id
			});
			RemoteMailbox sendToPipeline = new RemoteMailbox(this.DataObject);
			base.WriteObject(sendToPipeline);
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return RemoteMailbox.FromDataObject((ADUser)dataObject);
		}

		public const string ParameterACLableSyncedEnabled = "ACLableSyncedObjectEnabled";

		private RemoteRoutingAddressGenerator remoteRoutingAddressGenerator;
	}
}
