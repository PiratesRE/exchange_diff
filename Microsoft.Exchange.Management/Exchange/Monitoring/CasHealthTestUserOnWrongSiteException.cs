using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthTestUserOnWrongSiteException : LocalizedException
	{
		public CasHealthTestUserOnWrongSiteException(string userName, string foundOn, string shouldBeOn) : base(Strings.CasHealthTestUserOnWrongSite(userName, foundOn, shouldBeOn))
		{
			this.userName = userName;
			this.foundOn = foundOn;
			this.shouldBeOn = shouldBeOn;
		}

		public CasHealthTestUserOnWrongSiteException(string userName, string foundOn, string shouldBeOn, Exception innerException) : base(Strings.CasHealthTestUserOnWrongSite(userName, foundOn, shouldBeOn), innerException)
		{
			this.userName = userName;
			this.foundOn = foundOn;
			this.shouldBeOn = shouldBeOn;
		}

		protected CasHealthTestUserOnWrongSiteException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userName = (string)info.GetValue("userName", typeof(string));
			this.foundOn = (string)info.GetValue("foundOn", typeof(string));
			this.shouldBeOn = (string)info.GetValue("shouldBeOn", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userName", this.userName);
			info.AddValue("foundOn", this.foundOn);
			info.AddValue("shouldBeOn", this.shouldBeOn);
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

		private readonly string userName;

		private readonly string foundOn;

		private readonly string shouldBeOn;
	}
}
