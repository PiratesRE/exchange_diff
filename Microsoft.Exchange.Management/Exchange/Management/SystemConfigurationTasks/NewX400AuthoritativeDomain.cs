using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "X400AuthoritativeDomain", SupportsShouldProcess = true)]
	public sealed class NewX400AuthoritativeDomain : NewSystemConfigurationObjectTask<X400AuthoritativeDomain>
	{
		[Parameter(Mandatory = true)]
		public X400Domain X400DomainName
		{
			get
			{
				return this.DataObject.X400DomainName;
			}
			set
			{
				this.DataObject.X400DomainName = value;
			}
		}

		[Parameter]
		public bool X400ExternalRelay
		{
			get
			{
				return this.DataObject.X400ExternalRelay;
			}
			set
			{
				this.DataObject.X400ExternalRelay = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewAcceptedDomain(base.Name, this.X400DomainName.ToString());
			}
		}

		internal static void ValidateNoDuplicates(X400AuthoritativeDomain domain, IConfigurationSession session, Task.TaskErrorLoggingDelegate errorWriter)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Guid, domain.Guid),
				new ComparisonFilter(ComparisonOperator.Equal, X400AuthoritativeDomainSchema.DomainName, domain.X400DomainName)
			});
			X400AuthoritativeDomain[] array = session.Find<X400AuthoritativeDomain>(session.GetOrgContainerId().GetDescendantId(domain.ParentPath), QueryScope.SubTree, filter, null, 1);
			if (array.Length > 0)
			{
				errorWriter(new DuplicateX400DomainException(domain.X400DomainName), ErrorCategory.ResourceExists, domain);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			X400AuthoritativeDomain x400AuthoritativeDomain = (X400AuthoritativeDomain)base.PrepareDataObject();
			x400AuthoritativeDomain.SetId(this.ConfigurationSession, base.Name);
			return x400AuthoritativeDomain;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (TopologyProvider.IsAdamTopology())
			{
				base.WriteError(new CannotRunOnEdgeException(), ErrorCategory.InvalidOperation, null);
			}
			NewX400AuthoritativeDomain.ValidateNoDuplicates(this.DataObject, this.ConfigurationSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
			base.InternalValidate();
			TaskLogger.LogExit();
		}
	}
}
