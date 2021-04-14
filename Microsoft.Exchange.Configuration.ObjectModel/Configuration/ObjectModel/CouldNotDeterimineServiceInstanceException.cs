using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotDeterimineServiceInstanceException : LocalizedException
	{
		public CouldNotDeterimineServiceInstanceException(string domainName) : base(Strings.CouldNotDeterimineServiceInstanceException(domainName))
		{
			this.domainName = domainName;
		}

		public CouldNotDeterimineServiceInstanceException(string domainName, Exception innerException) : base(Strings.CouldNotDeterimineServiceInstanceException(domainName), innerException)
		{
			this.domainName = domainName;
		}

		protected CouldNotDeterimineServiceInstanceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.domainName = (string)info.GetValue("domainName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("domainName", this.domainName);
		}

		public string DomainName
		{
			get
			{
				return this.domainName;
			}
		}

		private readonly string domainName;
	}
}
