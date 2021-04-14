using System;
using System.Runtime.Serialization;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class BposNavBarInfo
	{
		public BposNavBarInfo(string version, NavBarData navBarData)
		{
			this.version = version;
			this.navBarData = navBarData;
		}

		[DataMember]
		public NavBarData NavBarData
		{
			get
			{
				return this.navBarData;
			}
			set
			{
				this.navBarData = value;
			}
		}

		public string Version
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
			}
		}

		private NavBarData navBarData;

		private string version;
	}
}
