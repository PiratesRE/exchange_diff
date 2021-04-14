using System;
using System.Management.Automation;
using System.ServiceModel;
using System.ServiceModel.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "AcceptedDomain", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveAcceptedDomain : RemoveSystemConfigurationObjectTask<AcceptedDomainIdParameter, AcceptedDomain>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveAcceptedDomain(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (Server.IsSubscribedGateway(base.GlobalConfigSession))
			{
				base.WriteError(new CannotRunOnSubscribedEdgeException(), ErrorCategory.InvalidOperation, null);
			}
			base.InternalValidate();
			RemoveAcceptedDomain.CheckDomainForRemoval(base.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			RemoveAcceptedDomain.DomainRemovalValidator domainRemovalValidator = new RemoveAcceptedDomain.DomainRemovalValidator(this);
			domainRemovalValidator.ValidateAllPolicies();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			if (base.DataObject.IsCoexistenceDomain)
			{
				try
				{
					AcceptedDomainUtility.DeregisterCoexistenceDomain(base.DataObject.DomainName.Domain);
				}
				catch (TimeoutException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidArgument, null);
				}
				catch (InvalidOperationException exception2)
				{
					base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
				}
				catch (SecurityAccessDeniedException exception3)
				{
					base.WriteError(exception3, ErrorCategory.InvalidArgument, null);
				}
				catch (CommunicationException exception4)
				{
					base.WriteError(exception4, ErrorCategory.InvalidArgument, null);
				}
			}
			base.InternalProcessRecord();
		}

		internal static void CheckDomainForRemoval(AcceptedDomain acceptedDomain, Task.TaskErrorLoggingDelegate writeError)
		{
			if (acceptedDomain.Default)
			{
				writeError(new CannotRemoveDefaultAcceptedDomainException(), ErrorCategory.InvalidOperation, acceptedDomain);
			}
		}

		private class DomainRemovalValidator : SetAcceptedDomain.DomainEditValidator
		{
			public DomainRemovalValidator(RemoveAcceptedDomain task) : base(new Task.TaskErrorLoggingDelegate(task.WriteError), (IConfigurationSession)task.DataSession, task.DataObject, null)
			{
			}

			protected override void WriteInvalidTemplate(SmtpProxyAddressTemplate template)
			{
				if (base.IsUsedBy(template))
				{
					base.ErrorWriter(new LocalizedException(Strings.AcceptedDomainIsReferencedByAddressTemplate(base.OldDomain.DomainName, template)), ErrorCategory.InvalidOperation, base.OldDomain.Identity);
				}
			}
		}
	}
}
