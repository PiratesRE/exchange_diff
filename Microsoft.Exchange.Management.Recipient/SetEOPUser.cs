using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PswsClient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "EOPUser", DefaultParameterSetName = "Identity")]
	public sealed class SetEOPUser : EOPTask
	{
		[Parameter(Mandatory = false)]
		public UserIdParameter Identity { get; set; }

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public string ExternalDirectoryObjectId { get; set; }

		[Parameter(Mandatory = false)]
		public string City { get; set; }

		[Parameter(Mandatory = false)]
		public string Company { get; set; }

		[Parameter(Mandatory = false)]
		public CountryInfo CountryOrRegion { get; set; }

		[Parameter(Mandatory = false)]
		public string Department { get; set; }

		[Parameter(Mandatory = false)]
		public string DisplayName { get; set; }

		[Parameter(Mandatory = false)]
		public string Fax { get; set; }

		[Parameter(Mandatory = false)]
		public string FirstName { get; set; }

		[Parameter(Mandatory = false)]
		public string HomePhone { get; set; }

		[Parameter(Mandatory = false)]
		public string Initials { get; set; }

		[Parameter(Mandatory = false)]
		public string LastName { get; set; }

		[Parameter(Mandatory = false)]
		public string MobilePhone { get; set; }

		[Parameter(Mandatory = false)]
		public string Notes { get; set; }

		[Parameter(Mandatory = false)]
		public string Office { get; set; }

		[Parameter(Mandatory = false)]
		public string Phone { get; set; }

		[Parameter(Mandatory = false)]
		public string PostalCode { get; set; }

		[Parameter(Mandatory = false)]
		public string StateOrProvince { get; set; }

		[Parameter(Mandatory = false)]
		public string StreetAddress { get; set; }

		[Parameter(Mandatory = false)]
		public string Title { get; set; }

		[Parameter(Mandatory = false)]
		public string WebPage { get; set; }

		protected override void InternalProcessRecord()
		{
			try
			{
				SetUserCmdlet setUserCmdlet = new SetUserCmdlet();
				ADObjectId executingUserId;
				base.ExchangeRunspaceConfig.TryGetExecutingUserId(out executingUserId);
				setUserCmdlet.Authenticator = Authenticator.Create(base.CurrentOrganizationId, executingUserId);
				setUserCmdlet.HostServerName = EOPRecipient.GetPsWsHostServerName();
				if (string.IsNullOrEmpty(this.ExternalDirectoryObjectId) && this.Identity == null)
				{
					base.ThrowTaskError(new ArgumentException(CoreStrings.MissingIdentityParameter.ToString()));
				}
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.Identity, string.IsNullOrEmpty(this.ExternalDirectoryObjectId) ? this.Identity.ToString() : this.ExternalDirectoryObjectId);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.DisplayName, this.DisplayName);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.FirstName, this.FirstName);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.LastName, this.LastName);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.Initials, this.Initials);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.City, this.City);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.Office, this.Office);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.StateOrProvince, this.StateOrProvince);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.PostalCode, this.PostalCode);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.CountryOrRegion, this.CountryOrRegion);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.Phone, this.Phone);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.Notes, this.Notes);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.Title, this.Title);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.Department, this.Department);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.Company, this.Company);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.StreetAddress, this.StreetAddress);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.MobilePhone, this.MobilePhone);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.Fax, this.Fax);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.HomePhone, this.HomePhone);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.WebPage, this.WebPage);
				EOPRecipient.SetProperty(setUserCmdlet, Parameters.Organization, base.Organization);
				setUserCmdlet.Run();
				EOPRecipient.CheckForError(this, setUserCmdlet);
			}
			catch (Exception e)
			{
				base.ThrowAndLogTaskError(e);
			}
		}
	}
}
