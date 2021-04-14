using System;
using System.Management.Automation;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxLoadBalanceClient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("New", "ConsumerMailbox", SupportsShouldProcess = true, DefaultParameterSetName = "ConsumerPrimaryMailbox")]
	public sealed class NewConsumerMailbox : NewADTaskBase<ADUser>
	{
		private new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public WindowsLiveId WindowsLiveID
		{
			get
			{
				return (WindowsLiveId)base.Fields["WindowsLiveID"];
			}
			set
			{
				base.Fields["WindowsLiveID"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ConsumerPrimaryMailbox")]
		public SwitchParameter MakeExoPrimary
		{
			get
			{
				return (SwitchParameter)(base.Fields["MakeExoPrimary"] ?? true);
			}
			set
			{
				base.Fields["MakeExoPrimary"] = value.ToBool();
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ConsumerSecondaryMailbox")]
		public SwitchParameter MakeExoSecondary
		{
			get
			{
				return (SwitchParameter)(base.Fields["MakeExoSecondary"] ?? false);
			}
			set
			{
				base.Fields["MakeExoSecondary"] = value.ToBool();
			}
		}

		[Parameter(Mandatory = false)]
		public DatabaseIdParameter Database
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["Database"];
			}
			set
			{
				base.Fields["Database"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ConsumerPrimaryMailbox")]
		public SwitchParameter SkipMigration
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipMigration"] ?? false);
			}
			set
			{
				base.Fields["SkipMigration"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Repair { get; set; }

		[Parameter(Mandatory = false)]
		public string Gender
		{
			get
			{
				return (string)base.Fields[ADUserSchema.Gender];
			}
			set
			{
				base.Fields[ADUserSchema.Gender] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Occupation
		{
			get
			{
				return (string)base.Fields[ADUserSchema.Occupation];
			}
			set
			{
				base.Fields[ADUserSchema.Occupation] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Region
		{
			get
			{
				return (string)base.Fields[ADUserSchema.Region];
			}
			set
			{
				base.Fields[ADUserSchema.Region] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Timezone
		{
			get
			{
				return (string)base.Fields[ADUserSchema.Timezone];
			}
			set
			{
				base.Fields[ADUserSchema.Timezone] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTime Birthdate
		{
			get
			{
				return (DateTime)base.Fields[ADUserSchema.Birthdate];
			}
			set
			{
				base.Fields[ADUserSchema.Birthdate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string BirthdayPrecision
		{
			get
			{
				return (string)base.Fields[ADUserSchema.BirthdayPrecision];
			}
			set
			{
				base.Fields[ADUserSchema.BirthdayPrecision] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string NameVersion
		{
			get
			{
				return (string)base.Fields[ADUserSchema.NameVersion];
			}
			set
			{
				base.Fields[ADUserSchema.NameVersion] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AlternateSupportEmailAddresses
		{
			get
			{
				return (string)base.Fields[ADUserSchema.AlternateSupportEmailAddresses];
			}
			set
			{
				base.Fields[ADUserSchema.AlternateSupportEmailAddresses] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PostalCode
		{
			get
			{
				return (string)base.Fields[ADUserSchema.PostalCode];
			}
			set
			{
				base.Fields[ADUserSchema.PostalCode] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OptInUser
		{
			get
			{
				return (string)base.Fields[ADUserSchema.OptInUser];
			}
			set
			{
				base.Fields[ADUserSchema.OptInUser] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string MigrationDryRun
		{
			get
			{
				return (string)base.Fields[ADUserSchema.MigrationDryRun];
			}
			set
			{
				base.Fields[ADUserSchema.MigrationDryRun] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string FirstName
		{
			get
			{
				return (string)base.Fields[ADUserSchema.FirstName];
			}
			set
			{
				base.Fields[ADUserSchema.FirstName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LastName
		{
			get
			{
				return (string)base.Fields[ADUserSchema.LastName];
			}
			set
			{
				base.Fields[ADUserSchema.LastName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CountryInfo UsageLocation
		{
			get
			{
				return (CountryInfo)base.Fields[ADRecipientSchema.UsageLocation];
			}
			set
			{
				base.Fields[ADRecipientSchema.UsageLocation] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				return (ProxyAddressCollection)base.Fields[ADRecipientSchema.EmailAddresses];
			}
			set
			{
				base.Fields[ADRecipientSchema.EmailAddresses] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<int> LocaleID
		{
			get
			{
				return (MultiValuedProperty<int>)base.Fields[ADUserSchema.LocaleID];
			}
			set
			{
				base.Fields[ADUserSchema.LocaleID] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool FblEnabled
		{
			get
			{
				return (bool)base.Fields[ADUserSchema.FblEnabled];
			}
			set
			{
				base.Fields[ADUserSchema.FblEnabled] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewMailboxUser((this.WindowsLiveID.NetId != null) ? this.WindowsLiveID.NetId.ToString() : string.Empty, this.WindowsLiveID.SmtpAddress.ToString(), TemplateTenantConfiguration.TemplateTenantExternalDirectoryOrganizationIdGuid.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return ConsumerMailboxHelper.CreateConsumerOrganizationSession();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return ConsumerMailboxHelper.IsKnownException(exception) || base.IsKnownException(exception);
		}

		protected override void InternalValidate()
		{
			if (this.WindowsLiveID != null && this.WindowsLiveID.NetId != null && this.WindowsLiveID.SmtpAddress != SmtpAddress.Empty)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorCannotProvideNetIDAndSmtpAddress), ExchangeErrorCategory.Client, null);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			if (base.Fields.IsModified("Database"))
			{
				ADObjectId value = this.ResolveDatabaseParameterId((DatabaseIdParameter)base.Fields["Database"]);
				base.Fields[ADMailboxRecipientSchema.Database] = value;
			}
			else
			{
				ILoadBalanceServicePort loadBalanceServicePort = LoadBalanceServiceAdapter.Create(NullAnchorLogger.Instance);
				base.Fields[ADMailboxRecipientSchema.Database] = loadBalanceServicePort.GetDatabaseForNewConsumerMailbox();
			}
			if (this.WindowsLiveID.NetId != null)
			{
				base.Fields[ADUserSchema.NetID] = this.WindowsLiveID.NetId;
				TaskLogger.LogExit();
				return null;
			}
			SmtpAddress smtpAddress = this.WindowsLiveID.SmtpAddress;
			throw new NotImplementedException("See OfficeMain bug# 1505962");
		}

		protected override void InternalProcessRecord()
		{
			LocalizedString message = new LocalizedString("Error creating consumer mailbox. See inner exception for details.");
			try
			{
				this.PrepareDataObject();
				WriteOperationType writeOperationType = this.Repair.ToBool() ? WriteOperationType.RepairCreate : WriteOperationType.Create;
				ConsumerMailboxHelper.CreateOrUpdateConsumerMailbox(writeOperationType, base.Fields, (IRecipientSession)base.DataSession, delegate(string s)
				{
					base.WriteVerbose(new LocalizedString(s));
				}, delegate(string s)
				{
					this.WriteWarning(new LocalizedString(s));
				});
				if (!base.HasErrors && !this.SkipWriteResult)
				{
					this.WriteResult();
				}
			}
			catch (ADNoSuchObjectException innerException)
			{
				throw new ManagementObjectNotFoundException(message, innerException);
			}
			catch (ADObjectAlreadyExistsException innerException2)
			{
				throw new ManagementObjectAlreadyExistsException(message, innerException2);
			}
			catch (ArgumentException innerException3)
			{
				throw new TaskArgumentException(message, innerException3);
			}
			catch (InvalidOperationException innerException4)
			{
				throw new TaskInvalidOperationException(message, innerException4);
			}
		}

		protected override void WriteResult()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Identity
			});
			ConsumerMailbox consumerMailbox = null;
			NetID netID = (NetID)base.Fields[ADUserSchema.NetID];
			try
			{
				using (TaskPerformanceData.ReadResult.StartRequestTimer())
				{
					ADUser aduser = ConsumerMailboxHelper.ReadUser((IRecipientSession)base.DataSession, netID.ToUInt64(), false);
					if (aduser != null)
					{
						consumerMailbox = new ConsumerMailbox(aduser);
					}
				}
			}
			finally
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
			}
			if (consumerMailbox == null)
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(netID.ToString(), typeof(ConsumerMailbox).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), (ErrorCategory)1003, netID.ToString());
			}
			using (TaskPerformanceData.WriteResult.StartRequestTimer())
			{
				this.WriteResult(consumerMailbox);
			}
			TaskLogger.LogExit();
		}

		private ADObjectId ResolveDatabaseParameterId(DatabaseIdParameter dbId)
		{
			MailboxDatabase mailboxDatabase = (MailboxDatabase)base.GetDataObject<MailboxDatabase>(dbId, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(dbId.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(dbId.ToString())));
			return mailboxDatabase.Id;
		}
	}
}
