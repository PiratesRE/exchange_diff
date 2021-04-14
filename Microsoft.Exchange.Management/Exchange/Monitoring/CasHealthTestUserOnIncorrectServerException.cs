using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthTestUserOnIncorrectServerException : LocalizedException
	{
		public CasHealthTestUserOnIncorrectServerException(string domain, string userName, string foundOn, string shouldBeOn) : base(Strings.CasHealthTestUserOnIncorrectServer(domain, userName, foundOn, shouldBeOn))
		{
			this.domain = domain;
			this.userName = userName;
			this.foundOn = foundOn;
			this.shouldBeOn = shouldBeOn;
		}

		public CasHealthTestUserOnIncorrectServerException(string domain, string userName, string foundOn, string shouldBeOn, Exception innerException) : base(Strings.CasHealthTestUserOnIncorrectServer(domain, userName, foundOn, shouldBeOn), innerException)
		{
			this.domain = domain;
			this.userName = userName;
			this.foundOn = foundOn;
			this.shouldBeOn = shouldBeOn;
		}

		protected CasHealthTestUserOnIncorrectServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.domain = (string)info.GetValue("domain", typeof(string));
			this.userName = (string)info.GetValue("userName", typeof(string));
			this.foundOn = (string)info.GetValue("foundOn", typeof(string));
			this.shouldBeOn = (string)info.GetValue("shouldBeOn", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("domain", this.domain);
			info.AddValue("userName", this.userName);
			info.AddValue("foundOn", this.foundOn);
			info.AddValue("shouldBeOn", this.shouldBeOn);
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

		public string FoundOn
		{
			get
			{
				return this.foundOn;
			}
		}

		public string ShouldBeOn
		{
			get
			{
				return this.shouldBeOn;
			}
		}

		private readonly string domain;

		private readonly string userName;

		private readonly string foundOn;

		private readonly string shouldBeOn;
	}
}
