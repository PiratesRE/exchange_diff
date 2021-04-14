using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetUserPhotoRequestType : BaseRequestType
	{
		public string Email
		{
			get
			{
				return this.email;
			}
			set
			{
				this.email = value;
			}
		}

		public UserPhotoSize SizeRequested
		{
			get
			{
				return this.sizeRequested;
			}
			set
			{
				this.sizeRequested = value;
			}
		}

		private string email;

		private UserPhotoSize sizeRequested;
	}
}
