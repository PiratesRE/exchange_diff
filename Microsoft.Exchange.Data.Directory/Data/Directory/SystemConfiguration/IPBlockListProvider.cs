using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class IPBlockListProvider : IPListProvider
	{
		internal override ADObjectId ParentPath
		{
			get
			{
				return IPBlockListProvider.parentPath;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return IPBlockListProvider.mostDerivedClass;
			}
		}

		[Parameter(Mandatory = false)]
		public AsciiString RejectionResponse
		{
			get
			{
				return (AsciiString)this[IPListProviderSchema.RejectionMessage];
			}
			set
			{
				this[IPListProviderSchema.RejectionMessage] = value;
			}
		}

		private static string mostDerivedClass = "msExchMessageHygieneIPBlockListProvider";

		private static ADObjectId parentPath = new ADObjectId("CN=IPBlockListProviderConfig,CN=Message Hygiene,CN=Transport Settings");
	}
}
