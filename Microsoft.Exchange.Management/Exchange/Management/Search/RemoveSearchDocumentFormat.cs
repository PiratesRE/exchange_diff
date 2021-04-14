using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Search.Fast;

namespace Microsoft.Exchange.Management.Search
{
	[Cmdlet("Remove", "SearchDocumentFormat", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class RemoveSearchDocumentFormat : DataAccessTask<Server>
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		[ValidateNotNullOrEmpty]
		public SearchDocumentFormatId Identity
		{
			get
			{
				return (SearchDocumentFormatId)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 61, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ContentIndex\\RemoveSearchDocumentFormat.cs");
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveSearchDocumentFormat;
			}
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				DocumentFormatManager documentFormatManager = new DocumentFormatManager((this.Server == null) ? "localhost" : this.Server.Fqdn);
				documentFormatManager.RemoveFormat(this.Identity.ToString());
			}
			catch (PerformingFastOperationException exception)
			{
				base.WriteError(exception, ErrorCategory.NotSpecified, null);
			}
		}

		protected override void InternalValidate()
		{
			if (this.Server != null)
			{
				base.GetDataObject<Server>(this.Server, base.GlobalConfigSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server)), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server)));
			}
		}
	}
}
