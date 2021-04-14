using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class ADDomainController : ADPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return ADDomainController.schema;
			}
		}

		public ADDomainController()
		{
		}

		public ADDomainController(ADServer dataObject) : base(dataObject)
		{
		}

		public new string Name
		{
			get
			{
				return (string)this[ADObjectSchema.Name];
			}
		}

		public string DnsHostName
		{
			get
			{
				return (string)this[ADServerSchema.DnsHostName];
			}
		}

		public ADObjectId ADSite
		{
			get
			{
				return ((ADServer)base.DataObject).Site;
			}
		}

		private static ADDomainControllerSchema schema = ObjectSchema.GetInstance<ADDomainControllerSchema>();
	}
}
