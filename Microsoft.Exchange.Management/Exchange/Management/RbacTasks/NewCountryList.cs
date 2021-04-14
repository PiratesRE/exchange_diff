using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("New", "CountryList", SupportsShouldProcess = true)]
	public sealed class NewCountryList : NewFixedNameSystemConfigurationObjectTask<CountryList>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewCountryList(this.Name);
			}
		}

		[Parameter(Mandatory = true, Position = 0)]
		public string Name
		{
			get
			{
				return this.DataObject.Name;
			}
			set
			{
				this.DataObject.Name = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<CountryInfo> Countries
		{
			get
			{
				return this.DataObject.Countries;
			}
			set
			{
				this.DataObject.Countries = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			this.DataObject = (CountryList)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			this.DataObject.SetId(this.ConfigurationSession, CountryList.RdnContainer, this.DataObject.Name);
			TaskLogger.LogExit();
			return this.DataObject;
		}
	}
}
