using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Threading;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.ProvisioningMonitoring;

namespace Microsoft.Exchange.Management.PowerShell
{
	internal static class ProvisioningMonitoringConfigInit
	{
		internal static void PopulateMonitoringWhiteLists()
		{
			ProvisioningMonitoringConfig.AddToCommonWhiteList(new CmdletErrorContext(typeof(DataValidationException)));
			ProvisioningMonitoringConfig.AddToCommonWhiteList(new CmdletErrorContext(typeof(PipelineStoppedException)));
			ProvisioningMonitoringConfig.AddToCommonWhiteList(new CmdletErrorContext(typeof(ProvisioningValidationException)));
			ProvisioningMonitoringConfig.AddToCommonWhiteList(new CmdletErrorContext(typeof(ParameterBindingException)));
			ProvisioningMonitoringConfig.AddToCommonWhiteList(new CmdletErrorContext(typeof(CmdletAccessDeniedException)));
			ProvisioningMonitoringConfig.AddToCommonWhiteList(new CmdletErrorContext(typeof(NameConversionException)));
			ProvisioningMonitoringConfig.AddToCommonWhiteList(new CmdletErrorContext(typeof(PropertyValueExistsException)));
			ProvisioningMonitoringConfig.AddToCommonWhiteList(new CmdletErrorContext(typeof(NotAcceptedDomainException)));
			ProvisioningMonitoringConfig.AddToCommonWhiteList(new CmdletErrorContext(typeof(ProxyAddressExistsException)));
			ProvisioningMonitoringConfig.AddToCommonWhiteList(new CmdletErrorContext(typeof(OverBudgetException)));
			ProvisioningMonitoringConfig.AddToCommonWhiteList(new CmdletErrorContext(typeof(ThreadAbortException)));
			ProvisioningMonitoringConfig.AddToCommonWhiteList(new CmdletErrorContext(typeof(HostException)));
			ProvisioningMonitoringConfig.AddToCommonWhiteList(new List<CmdletErrorContext>
			{
				new CmdletErrorContext(typeof(WLCDInvalidMemberNameException)),
				new CmdletErrorContext(typeof(WLCDPasswordBlankException)),
				new CmdletErrorContext(typeof(WLCDPasswordTooShortException)),
				new CmdletErrorContext(typeof(WLCDPasswordTooLongException)),
				new CmdletErrorContext(typeof(WLCDPasswordIncludesMemberNameException)),
				new CmdletErrorContext(typeof(WLCDPasswordIncludesInvalidCharsException)),
				new CmdletErrorContext(typeof(WLCDSecretQuestionContainsPassword)),
				new CmdletErrorContext(typeof(WLCDSecretAnswerContainsPassword)),
				new CmdletErrorContext(typeof(WLCDManagedMemberExistsException)),
				new CmdletErrorContext(typeof(WLCDInvalidMemberNameException)),
				new CmdletErrorContext(typeof(UserInputInvalidException)),
				new CmdletErrorContext(typeof(WLCDUnmanagedMemberExistsException)),
				new CmdletErrorContext(typeof(WLCDPasswordInvalidException)),
				new CmdletErrorContext(typeof(WLCDMemberNameInUseException))
			});
			List<CmdletErrorContext> list = new List<CmdletErrorContext>();
			list.Add(new CmdletErrorContext(typeof(NameNotAvailableException)));
			list.Add(new CmdletErrorContext(typeof(WindowsLiveIdAlreadyUsedException)));
			list.Add(new CmdletErrorContext(typeof(UserWithMatchingNetIdAndDifferentWindowsLiveIdExistsException)));
			list.Add(new CmdletErrorContext(typeof(UserWithMatchingWindowsLiveIdAndDifferentNetIdExistsException)));
			list.Add(new CmdletErrorContext(typeof(UserWithMatchingWindowsLiveIdExistsException)));
			ProvisioningMonitoringConfig.AddToCmdletWhiteList("new-mailbox", list);
			ProvisioningMonitoringConfig.AddToCmdletWhiteList("new-syncmailbox", list);
			List<CmdletErrorContext> list2 = new List<CmdletErrorContext>();
			list2.Add(new CmdletErrorContext(typeof(UserWithMatchingWindowsLiveIdExistsException)));
			list2.Add(new CmdletErrorContext(typeof(ManagementObjectAmbiguousException)));
			list2.Add(new CmdletErrorContext(typeof(ManagementObjectNotFoundException)));
			list2.Add(new CmdletErrorContext(typeof(ADObjectAlreadyExistsException)));
			ProvisioningMonitoringConfig.AddToCmdletWhiteList("set-mailbox", list2);
			ProvisioningMonitoringConfig.AddToCmdletWhiteList("set-syncmailbox", list2);
			List<CmdletErrorContext> list3 = new List<CmdletErrorContext>();
			list3.Add(new CmdletErrorContext(typeof(ManagementObjectAmbiguousException)));
			list3.Add(new CmdletErrorContext(typeof(ManagementObjectNotFoundException)));
			list3.Add(new CmdletErrorContext(typeof(CannotRemoveLastOrgAdminException)));
			ProvisioningMonitoringConfig.AddToCmdletWhiteList("remove-mailbox", list3);
			ProvisioningMonitoringConfig.AddToCmdletWhiteList("remove-syncmailbox", list3);
			List<CmdletErrorContext> list4 = new List<CmdletErrorContext>();
			list4.Add(new CmdletErrorContext(typeof(ManagementObjectNotFoundException)));
			ProvisioningMonitoringConfig.AddToCmdletWhiteList("get-mailbox", list4);
			ProvisioningMonitoringConfig.AddToCmdletWhiteList("get-syncmailbox", list4);
			ProvisioningMonitoringConfig.AddToCmdletWhiteList("get-managementendpoint", new List<CmdletErrorContext>
			{
				new CmdletErrorContext(typeof(InvalidUserInputException)),
				new CmdletErrorContext(typeof(RedirectionEntryManagerTransientException))
			});
			ProvisioningMonitoringConfig.AddToCmdletWhiteList("new-organization", new List<CmdletErrorContext>
			{
				new CmdletErrorContext(typeof(OrganizationValidationException)),
				new CmdletErrorContext(typeof(ArgumentException)),
				new CmdletErrorContext(typeof(OrganizationExistsException)),
				new CmdletErrorContext(typeof(OrganizationPendingOperationException)),
				new CmdletErrorContext(typeof(ArgumentNullException)),
				new CmdletErrorContext(typeof(ScriptExecutionException), typeof(ExchangeNotAuthorizedForDomainException))
			});
			ProvisioningMonitoringConfig.AddToCmdletWhiteList("remove-organization", new List<CmdletErrorContext>
			{
				new CmdletErrorContext(typeof(OrganizationValidationException)),
				new CmdletErrorContext(typeof(OrganizationPendingOperationException)),
				new CmdletErrorContext(typeof(OrganizationDoesNotExistException))
			});
			ProvisioningMonitoringConfig.AddToCmdletWhiteList("add-secondaryDomain", new List<CmdletErrorContext>
			{
				new CmdletErrorContext(typeof(OrganizationValidationException)),
				new CmdletErrorContext(typeof(ArgumentException)),
				new CmdletErrorContext(typeof(OrganizationTaskException)),
				new CmdletErrorContext(typeof(OrganizationPendingOperationException)),
				new CmdletErrorContext(typeof(ManagementObjectAlreadyExistsException))
			});
			ProvisioningMonitoringConfig.AddToCmdletWhiteList("remove-secondaryDomain", new List<CmdletErrorContext>
			{
				new CmdletErrorContext(typeof(ManagementObjectAmbiguousException)),
				new CmdletErrorContext(typeof(OrganizationPendingOperationException)),
				new CmdletErrorContext(typeof(ManagementObjectNotFoundException)),
				new CmdletErrorContext(typeof(ArgumentNullException))
			});
			List<CmdletErrorContext> list5 = new List<CmdletErrorContext>();
			list5.Add(new CmdletErrorContext(typeof(InvalidOperationException)));
			list5.Add(new CmdletErrorContext(typeof(ArgumentException)));
			list5.Add(new CmdletErrorContext(typeof(IOException)));
			list5.Add(new CmdletErrorContext(typeof(SharedConfigurationValidationException)));
			ProvisioningMonitoringConfig.AddToCmdletWhiteList("start-organizationupgrade", list5);
			ProvisioningMonitoringConfig.AddToCmdletWhiteList("complete-organizationupgrade", list5);
		}
	}
}
