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
	[Cmdlet("Remove", "ConsumerMailbox", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveConsumerMailbox : RemoveADTaskBase<ConsumerMailboxIdParameter, ADUser>
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

		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "ConsumerSecondaryMailbox", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "ConsumerPrimaryMailbox", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
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
		public SwitchParameter RemoveExoPrimary
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoveExoPrimary"] ?? false);
			}
			set
			{
				base.Fields["RemoveExoPrimary"] = value.ToBool();
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ConsumerPrimaryMailbox")]
		public SwitchParameter SwitchToSecondary
		{
			get
			{
				return (SwitchParameter)(base.Fields["SwitchToSecondary"] ?? false);
			}
			set
			{
				base.Fields["SwitchToSecondary"] = value.ToBool();
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ConsumerSecondaryMailbox")]
		public SwitchParameter RemoveExoSecondary
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoveExoSecondary"] ?? false);
			}
			set
			{
				base.Fields["RemoveExoSecondary"] = value.ToBool();
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveMailboxIdentityAndNotLiveId(this.Identity.ToString(), "<UNKNOWN>");
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
			TaskLogger.LogEnter();
			if (base.Fields.IsModified("RemoveExoPrimary") && base.Fields.IsModified("RemoveExoSecondary"))
			{
				base.ThrowTerminatingError(new ArgumentException("Cannot specify both -RemoveExoPrimary and -RemoveExoSecondary parameters"), ErrorCategory.InvalidArgument, this.Identity.ToString());
			}
			TaskLogger.LogExit();
		}

		private IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			base.DataObject = (ADUser)base.ResolveDataObject();
			ulong puidNum;
			if (ConsumerIdentityHelper.TryGetPuidByExternalDirectoryObjectId(base.DataObject.ExchangeGuid.ToString(), out puidNum))
			{
				base.Fields[ADUserSchema.NetID] = new NetID(ConsumerIdentityHelper.ConvertPuidNumberToPuidString(puidNum));
				TaskLogger.LogExit();
				return null;
			}
			throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "Could not extract puid from ExchangeGuid for this user", new object[0]));
		}

		protected override void InternalProcessRecord()
		{
			LocalizedString message = new LocalizedString("Error removing consumer mailbox. See inner exception for details.");
			try
			{
				this.PrepareDataObject();
				ConsumerMailboxHelper.RemoveConsumerMailbox(base.Fields, (IRecipientSession)base.DataSession, delegate(string s)
				{
					base.WriteVerbose(new LocalizedString(s));
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
