using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ManagementEndpoint
{
	[Cmdlet("Remove", "ManagementEndpointHook", SupportsShouldProcess = true)]
	public sealed class RemoveManagementEndpointHook : ManagementEndpointBase
	{
		[Parameter(Mandatory = true, Position = 0)]
		[ValidateNotNullOrEmpty]
		public Guid ExternalDirectoryOrganizationId { get; set; }

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public SmtpDomain DomainName { get; set; }

		internal override void ProcessRedirectionEntry(IGlobalDirectorySession session)
		{
			if (this.DomainName == null)
			{
				try
				{
					session.RemoveTenant(this.ExternalDirectoryOrganizationId);
					return;
				}
				catch (GlsTenantNotFoundException ex)
				{
					base.WriteWarning(string.Format("GlsTenantNotFoundException ignored during removal. exception: {0}", ex.Message));
					return;
				}
			}
			try
			{
				session.RemoveAcceptedDomain(this.ExternalDirectoryOrganizationId, this.DomainName.Domain);
			}
			catch (GlsPermanentException ex2)
			{
				if (ex2.Message.IndexOf("DomainBelongsToDifferentTenantException", StringComparison.OrdinalIgnoreCase) == -1 && ex2.Message.IndexOf("DataNotFound", StringComparison.OrdinalIgnoreCase) == -1)
				{
					throw ex2;
				}
				base.WriteWarning(string.Format("Gls exception ignored during removal. exception: {0}", ex2.Message));
			}
			catch (GlsTenantNotFoundException)
			{
			}
		}
	}
}
