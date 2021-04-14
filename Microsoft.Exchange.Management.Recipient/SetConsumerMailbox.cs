using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "ConsumerMailbox", SupportsShouldProcess = true)]
	public sealed class SetConsumerMailbox : SetADTaskBase<ConsumerMailboxIdParameter, ADUser, ADUser>
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

		[Parameter(Mandatory = true, ParameterSetName = "ConsumerPrimaryMailbox", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "ConsumerSecondaryMailbox", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		public override ConsumerMailboxIdParameter Identity
		{
			get
			{
				return (ConsumerMailboxIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ConsumerPrimaryMailbox")]
		public SwitchParameter MakeExoPrimary
		{
			get
			{
				return (SwitchParameter)(base.Fields["MakeExoPrimary"] ?? false);
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
		public string Description
		{
			get
			{
				return (string)base.Fields[ADRecipientSchema.Description];
			}
			set
			{
				base.Fields[ADRecipientSchema.Description] = value;
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
				return Strings.ConfirmationMessageSetMailbox(this.Identity.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigDataProvider configDataProvider = ConsumerMailboxHelper.CreateConsumerOrganizationSession();
			((IAggregateSession)configDataProvider).MbxReadMode = MbxReadMode.NoMbxRead;
			return configDataProvider;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return ConsumerMailboxHelper.IsKnownException(exception) || base.IsKnownException(exception);
		}

		protected override void InternalValidate()
		{
			if (base.Fields.IsModified("MakeExoPrimary") && base.Fields.IsModified("MakeExoSecondary"))
			{
				base.ThrowTerminatingError(new ArgumentException("Cannot specify both -MakeExoPrimary and -MakeExoSecondary parameters"), ErrorCategory.InvalidArgument, this.Identity.ToString());
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			this.DataObject = (ADUser)base.PrepareDataObject();
			ulong puidNum;
			if (ConsumerIdentityHelper.TryGetPuidFromGuid(this.DataObject.ExchangeGuid, out puidNum))
			{
				base.Fields[ADUserSchema.NetID] = new NetID(ConsumerIdentityHelper.ConvertPuidNumberToPuidString(puidNum));
				TaskLogger.LogExit();
				return null;
			}
			throw new TaskInvalidOperationException(new LocalizedString(string.Format(CultureInfo.CurrentUICulture, "Could not extract puid from ExchangeGuid for this user", new object[0])));
		}

		protected override void InternalProcessRecord()
		{
			LocalizedString message = new LocalizedString("Error updating consumer mailbox. See inner exception for details.");
			try
			{
				this.PrepareDataObject();
				WriteOperationType writeOperationType = this.Repair.ToBool() ? WriteOperationType.RepairUpdate : WriteOperationType.Update;
				ConsumerMailboxHelper.CreateOrUpdateConsumerMailbox(writeOperationType, base.Fields, (IRecipientSession)base.DataSession, delegate(string s)
				{
					base.WriteVerbose(new LocalizedString(s));
				}, delegate(string s)
				{
					this.WriteWarning(new LocalizedString(s));
				});
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
	}
}
