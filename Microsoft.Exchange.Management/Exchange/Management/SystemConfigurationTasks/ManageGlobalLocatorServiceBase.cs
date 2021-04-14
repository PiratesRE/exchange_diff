using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public class ManageGlobalLocatorServiceBase : Task
	{
		[ValidateNotNull]
		[Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, ParameterSetName = "ExternalDirectoryOrganizationIdParameterSet")]
		[Parameter(Mandatory = false, ParameterSetName = "MsaUserNetIDParameterSet")]
		public Guid ExternalDirectoryOrganizationId
		{
			get
			{
				return (Guid)base.Fields["ExternalDirectoryOrganizationId"];
			}
			set
			{
				base.Fields["ExternalDirectoryOrganizationId"] = value;
			}
		}

		internal void WriteGlsTenantNotFoundError(Guid orgGuid)
		{
			this.WriteGlsTenantNotFoundError(orgGuid.ToString());
		}

		internal void WriteGlsTenantNotFoundError(string orgGuidOrDomain)
		{
			base.WriteError(new GlsTenantNotFoundException(DirectoryStrings.TenantNotFoundInGlsError(orgGuidOrDomain)), ExchangeErrorCategory.Client, null);
		}

		internal void WriteGlsDomainNotFoundError(string domain)
		{
			base.WriteError(new GlsDomainNotFoundException(DirectoryStrings.DomainNotFoundInGlsError(domain)), ExchangeErrorCategory.Client, null);
		}

		internal void WriteGlsMsaUserNotFoundError(string msaUserNetId)
		{
			base.WriteError(new GlsMsaUserNotFoundException(DirectoryStrings.MsaUserNotFoundInGlsError(msaUserNetId)), ExchangeErrorCategory.Client, null);
		}

		internal void WriteGlsMsaUserAlreadyExistsError(string msaUserNetId)
		{
			base.WriteError(new GlsMsaUserAlreadyExistsException(DirectoryStrings.MsaUserAlreadyExistsInGlsError(msaUserNetId)), ExchangeErrorCategory.Client, null);
		}

		internal void WriteInvalidFqdnError(string fqdn)
		{
			base.WriteError(new ArgumentException(DirectoryStrings.InvalidPartitionFqdn(fqdn)), ErrorCategory.InvalidArgument, null);
		}

		internal const string ExternalDirectoryOrganizationIdParameterName = "ExternalDirectoryOrganizationId";

		internal const string ExternalDirectoryOrganizationIdParameterSetName = "ExternalDirectoryOrganizationIdParameterSet";

		internal const string DomainNameParameterName = "DomainName";

		internal const string DomainNameParameterSetName = "DomainNameParameterSet";

		internal const string ResourceForestParameterName = "ResourceForest";

		internal const string AccountForestParameterName = "AccountForest";

		internal const string PrimarySiteParameterName = "PrimarySite";

		internal const string SmtpNextHopDomainParameterName = "SmtpNextHopDomain";

		internal const string TenantFlagsParameterName = "TenantFlags";

		internal const string TenantContainerCNParameterName = "TenantContainerCN";

		internal const string DomainFlagsParameterName = "DomainFlags";

		internal const string DomainTypeParameterName = "DomainType";

		internal const string DomainInUseParameterName = "DomainInUse";

		internal const string ShowDomainNamesParameterName = "ShowDomainNames";

		internal const string UseOfflineGLSParameterName = "UseOfflineGLS";

		internal const string MsaUserMemberNameParameterName = "MsaUserMemberName";

		internal const string MsaUserNetIdParameterName = "MsaUserNetID";

		internal const string MsaUserNetIdParameterSetName = "MsaUserNetIDParameterSet";

		internal const string DomainNameSuffix = "{0}:{1}";

		internal const string ADOnlySuffix = "ADOnly";

		internal const string ADAndGLSSuffix = "ADAndGLS";

		internal const string GlsOnlySuffix = "GlsOnly";
	}
}
