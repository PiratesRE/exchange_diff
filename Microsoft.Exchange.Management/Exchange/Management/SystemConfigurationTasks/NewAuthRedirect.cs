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
	[Cmdlet("New", "AuthRedirect", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class NewAuthRedirect : NewSystemConfigurationObjectTask<AuthRedirect>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewAuthRedirect(this.Name);
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[Parameter(Mandatory = true)]
		public AuthScheme AuthScheme
		{
			get
			{
				return this.DataObject.AuthScheme;
			}
			set
			{
				this.DataObject.AuthScheme = value;
			}
		}

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string TargetUrl
		{
			get
			{
				return this.DataObject.TargetUrl;
			}
			set
			{
				this.DataObject.TargetUrl = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			this.Name = string.Format("AuthRedirect-{0}-{1}", this.AuthScheme.ToString(), AuthRedirect.AuthRedirectKeywords);
			AuthRedirect authRedirect = (AuthRedirect)base.PrepareDataObject();
			ADObjectId childId = this.ConfigurationSession.GetOrgContainerId().GetChildId(ServiceEndpointContainer.DefaultName);
			ADObjectId childId2 = childId.GetChildId(this.Name);
			authRedirect.SetId(childId2);
			return authRedirect;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.AuthScheme == AuthScheme.Unknown)
			{
				base.WriteError(new TaskArgumentException(Strings.ErrorInvalidAuthScheme(this.AuthScheme.ToString())), ErrorCategory.InvalidArgument, null);
			}
			AuthRedirect[] array = this.ConfigurationSession.Find<AuthRedirect>(this.ConfigurationSession.GetOrgContainerId().GetChildId(ServiceEndpointContainer.DefaultName), QueryScope.OneLevel, new ComparisonFilter(ComparisonOperator.Equal, AuthRedirectSchema.AuthScheme, this.AuthScheme), null, 1);
			if (array.Length > 0)
			{
				base.WriteError(new TaskArgumentException(Strings.ErrorAuthSchemeExists(array[0].Id.ToString(), this.AuthScheme.ToString())), ErrorCategory.InvalidArgument, null);
			}
		}
	}
}
