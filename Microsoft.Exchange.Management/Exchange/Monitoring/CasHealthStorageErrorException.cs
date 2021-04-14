using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthStorageErrorException : LocalizedException
	{
		public CasHealthStorageErrorException(string serverName, string domain, string user, string errorStr) : base(Strings.CasHealthStorageError(serverName, domain, user, errorStr))
		{
			this.serverName = serverName;
			this.domain = domain;
			this.user = user;
			this.errorStr = errorStr;
		}

		public CasHealthStorageErrorException(string serverName, string domain, string user, string errorStr, Exception innerException) : base(Strings.CasHealthStorageError(serverName, domain, user, errorStr), innerException)
		{
			this.serverName = serverName;
			this.domain = domain;
			this.user = user;
			this.errorStr = errorStr;
		}

		protected CasHealthStorageErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.domain = (string)info.GetValue("domain", typeof(string));
			this.user = (string)info.GetValue("user", typeof(string));
			this.errorStr = (string)info.GetValue("errorStr", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
			info.AddValue("domain", this.domain);
			info.AddValue("user", this.user);
			info.AddValue("errorStr", this.errorStr);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public string Domain
		{
			get
			{
				return this.domain;
			}
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		public string ErrorStr
		{
			get
			{
				return this.errorStr;
			}
		}

		private readonly string serverName;

		private readonly string domain;

		private readonly string user;

		private readonly string errorStr;
	}
}
