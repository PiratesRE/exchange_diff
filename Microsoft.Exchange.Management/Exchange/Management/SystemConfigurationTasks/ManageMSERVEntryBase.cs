using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public class ManageMSERVEntryBase : Task
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, ParameterSetName = "ExternalDirectoryOrganizationIdParameterSet")]
		[ValidateNotNull]
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

		[Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, ParameterSetName = "DomainNameParameterSet")]
		[ValidateNotNull]
		public SmtpDomain DomainName
		{
			get
			{
				return (SmtpDomain)base.Fields["DomainName"];
			}
			set
			{
				base.Fields["DomainName"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, ParameterSetName = "AddressParameterSet")]
		[ValidateNotNullOrEmpty]
		public string Address
		{
			get
			{
				return (string)base.Fields["Address"];
			}
			set
			{
				base.Fields["Address"] = value;
			}
		}

		protected MSERVEntry ProcessExternalOrgIdParameter(Func<string, int, int> invoke)
		{
			return this.ProcessExternalOrgIdParameter(0, 0, invoke);
		}

		protected MSERVEntry ProcessExternalOrgIdParameter(int partnerId, int minorPartnerId, Func<string, int, int> invoke)
		{
			MSERVEntry mserventry = new MSERVEntry();
			Guid guid = (Guid)base.Fields["ExternalDirectoryOrganizationId"];
			mserventry.ExternalDirectoryOrganizationId = guid;
			if (partnerId > -1)
			{
				string text = string.Format("43BA6209CC0F4542958F65F8BF1CDED6@{0}.exchangereserved", guid);
				mserventry.AddressForPartnerId = text;
				mserventry.PartnerId = invoke(text, partnerId);
				MServDirectorySession mservDirectorySession = new MServDirectorySession(null);
				string forest;
				if (mservDirectorySession.TryGetForestFqdnFromPartnerId(mserventry.PartnerId, out forest))
				{
					mserventry.Forest = forest;
				}
			}
			if (minorPartnerId > -1)
			{
				string text = string.Format("3da19c7b44a74bd3896daaf008594b6c@{0}.exchangereserved", guid);
				mserventry.AddressForMinorPartnerId = text;
				mserventry.MinorPartnerId = invoke(text, minorPartnerId);
			}
			return mserventry;
		}

		protected MSERVEntry ProcessDomainNameParameter(Func<string, int, int> invoke)
		{
			return this.ProcessDomainNameParameter(0, 0, invoke);
		}

		protected MSERVEntry ProcessDomainNameParameter(int partnerId, int minorPartnerId, Func<string, int, int> invoke)
		{
			MSERVEntry mserventry = new MSERVEntry();
			string domain = ((SmtpDomain)base.Fields["DomainName"]).Domain;
			mserventry.DomainName = domain;
			if (partnerId > -1)
			{
				string text = string.Format("E5CB63F56E8B4b69A1F70C192276D6AD@{0}", domain);
				mserventry.AddressForPartnerId = text;
				mserventry.PartnerId = invoke(text, partnerId);
				MServDirectorySession mservDirectorySession = new MServDirectorySession(null);
				string forest;
				if (mservDirectorySession.TryGetForestFqdnFromPartnerId(mserventry.PartnerId, out forest))
				{
					mserventry.Forest = forest;
				}
			}
			if (minorPartnerId > -1)
			{
				string text = string.Format("7f66cd009b304aeda37ffdeea1733ff6@{0}", domain);
				mserventry.AddressForMinorPartnerId = text;
				mserventry.MinorPartnerId = invoke(text, minorPartnerId);
			}
			return mserventry;
		}

		protected MSERVEntry ProcessAddressParameter(Func<string, int, int> invoke)
		{
			return this.ProcessAddressParameter(0, invoke);
		}

		protected MSERVEntry ProcessAddressParameter(int id, Func<string, int, int> invoke)
		{
			MSERVEntry mserventry = new MSERVEntry();
			string text = (string)base.Fields["Address"];
			if (this.ShouldProcessPartnerId(text))
			{
				mserventry.AddressForPartnerId = text;
				mserventry.PartnerId = invoke(text, id);
				MServDirectorySession mservDirectorySession = new MServDirectorySession(null);
				string forest;
				if (mservDirectorySession.TryGetForestFqdnFromPartnerId(mserventry.PartnerId, out forest))
				{
					mserventry.Forest = forest;
				}
			}
			else
			{
				mserventry.AddressForMinorPartnerId = text;
				mserventry.MinorPartnerId = invoke(text, id);
			}
			return mserventry;
		}

		protected bool ShouldProcessPartnerId(string address)
		{
			string text = address.Split(new char[]
			{
				'@'
			})[0];
			return "E5CB63F56E8B4b69A1F70C192276D6AD@{0}".ToLower().Contains(text.ToLower()) || "43BA6209CC0F4542958F65F8BF1CDED6@{0}.exchangereserved".ToLower().Contains(text.ToLower());
		}

		protected void ValidateAddressParameter(string address)
		{
			if (this.ShouldProcessPartnerId(address))
			{
				if (!base.Fields.IsModified("PartnerId"))
				{
					base.WriteError(new ParameterBindingException("PartnerId is not specified"), ErrorCategory.InvalidArgument, null);
					return;
				}
			}
			else if (!base.Fields.IsModified("MinorPartnerId"))
			{
				base.WriteError(new ParameterBindingException("MinorPartnerId is not specified"), ErrorCategory.InvalidArgument, null);
			}
		}

		protected void ValidateMservIdValue(int id)
		{
			if (id == -1 || id < 50000 || id > 59999)
			{
				base.WriteError(new InvalidPartnerIdException(Strings.ErrorInvalidPartnerId(id)), ErrorCategory.InvalidData, null);
			}
		}

		protected int ReadMservEntry(string address)
		{
			return MServDirectorySession.ReadMservEntry(address);
		}

		protected int AddMservEntry(string address, int newPartnerId)
		{
			MServDirectorySession.AddMserveEntry(address, newPartnerId);
			return newPartnerId;
		}

		protected int RemoveMservEntry(string address)
		{
			int num = MServDirectorySession.ReadMservEntry(address);
			if (num != -1)
			{
				MServDirectorySession.RemoveMserveEntry(address, num);
			}
			else
			{
				base.WriteError(new TenantNotFoundException(DirectoryStrings.TenantNotFoundInMservError(address)), ExchangeErrorCategory.Client, null);
			}
			return num;
		}

		protected int UpdateMservEntry(string address, int newPartnerId)
		{
			int num = MServDirectorySession.ReadMservEntry(address);
			if (num != -1)
			{
				MServDirectorySession.RemoveMserveEntry(address, num);
				MServDirectorySession.AddMserveEntry(address, newPartnerId);
			}
			else
			{
				base.WriteError(new TenantNotFoundException(DirectoryStrings.TenantNotFoundInMservError(address)), ExchangeErrorCategory.Client, null);
			}
			return newPartnerId;
		}

		internal const string ExternalDirectoryOrganizationIdParameterName = "ExternalDirectoryOrganizationId";

		internal const string ExternalDirectoryOrganizationIdParameterSetName = "ExternalDirectoryOrganizationIdParameterSet";

		internal const string DomainNameParameterName = "DomainName";

		internal const string DomainNameParameterSetName = "DomainNameParameterSet";

		internal const string AddressParameterName = "Address";

		internal const string AddressParameterSetName = "AddressParameterSet";

		internal const string PartnerIdParameterName = "PartnerId";

		internal const string MinorPartnerIdParameterName = "MinorPartnerId";
	}
}
