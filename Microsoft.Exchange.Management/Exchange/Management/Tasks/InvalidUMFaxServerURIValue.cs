using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidUMFaxServerURIValue : LocalizedException
	{
		public InvalidUMFaxServerURIValue(string faxServerUri) : base(Strings.InvalidUMFaxServerURIValue(faxServerUri))
		{
			this.faxServerUri = faxServerUri;
		}

		public InvalidUMFaxServerURIValue(string faxServerUri, Exception innerException) : base(Strings.InvalidUMFaxServerURIValue(faxServerUri), innerException)
		{
			this.faxServerUri = faxServerUri;
		}

		protected InvalidUMFaxServerURIValue(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.faxServerUri = (string)info.GetValue("faxServerUri", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("faxServerUri", this.faxServerUri);
		}

		public string FaxServerUri
		{
			get
			{
				return this.faxServerUri;
			}
		}

		private readonly string faxServerUri;
	}
}
