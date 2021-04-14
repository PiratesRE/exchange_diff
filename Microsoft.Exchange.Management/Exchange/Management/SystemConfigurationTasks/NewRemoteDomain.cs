using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "RemoteDomain", SupportsShouldProcess = true)]
	public class NewRemoteDomain : NewMultitenancySystemConfigurationObjectTask<DomainContentConfig>
	{
		[Parameter(Mandatory = true)]
		public SmtpDomainWithSubdomains DomainName
		{
			get
			{
				return this.DataObject.DomainName;
			}
			set
			{
				this.DataObject.DomainName = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewRemoteDomain(base.Name.ToString(), this.DomainName.ToString());
			}
		}

		internal static void ValidateNoDuplicates(DomainContentConfig domain, IConfigurationSession session, Task.TaskErrorLoggingDelegate errorWriter)
		{
			string domain2 = domain.DomainName.Domain;
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Guid, domain.Guid),
				new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, EdgeDomainContentConfigSchema.DomainName, domain2),
					new ComparisonFilter(ComparisonOperator.Equal, EdgeDomainContentConfigSchema.DomainName, "*." + domain2)
				})
			});
			DomainContentConfig[] array = session.Find<DomainContentConfig>(session.GetOrgContainerId().GetDescendantId(domain.ParentPath), QueryScope.SubTree, filter, null, 1);
			if (array.Length > 0)
			{
				errorWriter(new DuplicateRemoteDomainException(domain2), ErrorCategory.ResourceExists, domain);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			DomainContentConfig domainContentConfig = (DomainContentConfig)base.PrepareDataObject();
			domainContentConfig.SetId(base.DataSession as IConfigurationSession, base.Name);
			return domainContentConfig;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.LimitRemoteDomains.Enabled)
			{
				DomainContentConfig[] array = base.DataSession.FindPaged<DomainContentConfig>(null, null, true, null, 0).ToArray<DomainContentConfig>();
				if (array != null && array.Length >= 200)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorTooManyRemoteDomains(200)), ErrorCategory.InvalidOperation, null);
				}
			}
			if (Server.IsSubscribedGateway(base.GlobalConfigSession))
			{
				base.WriteError(new CannotRunOnSubscribedEdgeException(), ErrorCategory.InvalidOperation, null);
			}
			NewRemoteDomain.ValidateNoDuplicates(this.DataObject, this.ConfigurationSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			FfoDualWriter.SaveToFfo<DomainContentConfig>(this, this.DataObject, null);
			TaskLogger.LogExit();
		}

		private const int MaxRemoteDomains = 200;
	}
}
