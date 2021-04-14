using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class NewIPListProvider<TDataObject> : NewSystemConfigurationObjectTask<TDataObject> where TDataObject : IPListProvider, new()
	{
		[Parameter(Mandatory = true)]
		public SmtpDomain LookupDomain
		{
			get
			{
				TDataObject dataObject = this.DataObject;
				return dataObject.LookupDomain;
			}
			set
			{
				TDataObject dataObject = this.DataObject;
				dataObject.LookupDomain = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				TDataObject dataObject = this.DataObject;
				return dataObject.Enabled;
			}
			set
			{
				TDataObject dataObject = this.DataObject;
				dataObject.Enabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AnyMatch
		{
			get
			{
				TDataObject dataObject = this.DataObject;
				return dataObject.AnyMatch;
			}
			set
			{
				TDataObject dataObject = this.DataObject;
				dataObject.AnyMatch = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IPAddress BitmaskMatch
		{
			get
			{
				TDataObject dataObject = this.DataObject;
				return dataObject.BitmaskMatch;
			}
			set
			{
				TDataObject dataObject = this.DataObject;
				dataObject.BitmaskMatch = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<IPAddress> IPAddressesMatch
		{
			get
			{
				TDataObject dataObject = this.DataObject;
				return dataObject.IPAddressesMatch;
			}
			set
			{
				TDataObject dataObject = this.DataObject;
				dataObject.IPAddressesMatch = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int Priority
		{
			get
			{
				TDataObject dataObject = this.DataObject;
				return dataObject.Priority;
			}
			set
			{
				TDataObject dataObject = this.DataObject;
				dataObject.Priority = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TDataObject tdataObject = (TDataObject)((object)base.PrepareDataObject());
			tdataObject.SetId((IConfigurationSession)base.DataSession, base.Name);
			return tdataObject;
		}

		protected override void InternalProcessRecord()
		{
			IConfigurationSession session = (IConfigurationSession)base.DataSession;
			NewIPListProvider<TDataObject>.AdjustPriorities(session, this.DataObject, false);
			base.InternalProcessRecord();
		}

		protected override void TranslateException(ref Exception e, out ErrorCategory category)
		{
			category = (ErrorCategory)1001;
			if (typeof(ADObjectAlreadyExistsException).IsInstanceOfType(e))
			{
				e = new LocalizedException(Strings.IPListProviderNameExists(base.Name), e);
				return;
			}
			base.TranslateException(ref e, out category);
		}

		internal static void AdjustPriorities(IConfigurationSession session, TDataObject newProvider, bool providerAlreadyExists)
		{
			ADPagedReader<TDataObject> adpagedReader = session.FindAllPaged<TDataObject>();
			TDataObject[] array = adpagedReader.ReadAllPages();
			int num = int.MaxValue;
			if (providerAlreadyExists)
			{
				foreach (TDataObject tdataObject in array)
				{
					if (tdataObject.Id.ObjectGuid.Equals(newProvider.Id.ObjectGuid))
					{
						num = tdataObject.Priority;
						break;
					}
				}
			}
			int num2 = 0;
			foreach (TDataObject tdataObject2 in array)
			{
				if (tdataObject2.Priority > num2)
				{
					num2 = tdataObject2.Priority;
				}
				if (tdataObject2.Priority >= newProvider.Priority && tdataObject2.Priority < num)
				{
					tdataObject2.Priority++;
					session.Save(tdataObject2);
				}
				else if (tdataObject2.Priority <= newProvider.Priority && tdataObject2.Priority > num)
				{
					tdataObject2.Priority--;
					session.Save(tdataObject2);
				}
			}
			if (num2 < newProvider.Priority)
			{
				newProvider.Priority = (providerAlreadyExists ? num2 : (num2 + 1));
			}
		}
	}
}
