using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthCouldNotLogUserForAutodiscoverException : LocalizedException
	{
		public CasHealthCouldNotLogUserForAutodiscoverException(string domain, string userName, string additionalInfo) : base(Strings.CasHealthCouldNotLogUserForAutodiscover(domain, userName, additionalInfo))
		{
			this.domain = domain;
			this.userName = userName;
			this.additionalInfo = additionalInfo;
		}

		public CasHealthCouldNotLogUserForAutodiscoverException(string domain, string userName, string additionalInfo, Exception innerException) : base(Strings.CasHealthCouldNotLogUserForAutodiscover(domain, userName, additionalInfo), innerException)
		{
			this.domain = domain;
			this.userName = userName;
			this.additionalInfo = additionalInfo;
		}

		protected CasHealthCouldNotLogUserForAutodiscoverException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.domain = (string)info.GetValue("domain", typeof(string));
			this.userName = (string)info.GetValue("userName", typeof(string));
			this.additionalInfo = (string)info.GetValue("additionalInfo", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("domain", this.domain);
			info.AddValue("userName", this.userName);
			info.AddValue("additionalInfo", this.additionalInfo);
		}

		public string Domain
		{
			get
			{
				return this.domain;
			}
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
		}

		public string AdditionalInfo
		{
			get
			{
				return this.additionalInfo;
			}
		}

		private readonly string domain;

		private readonly string userName;

		private readonly string additionalInfo;
	}
}
