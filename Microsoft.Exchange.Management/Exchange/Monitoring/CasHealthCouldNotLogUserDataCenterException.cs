using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthCouldNotLogUserDataCenterException : LocalizedException
	{
		public CasHealthCouldNotLogUserDataCenterException(string testDomain, string scriptName, string errorString) : base(Strings.CasHealthCouldNotLogUserDataCenter(testDomain, scriptName, errorString))
		{
			this.testDomain = testDomain;
			this.scriptName = scriptName;
			this.errorString = errorString;
		}

		public CasHealthCouldNotLogUserDataCenterException(string testDomain, string scriptName, string errorString, Exception innerException) : base(Strings.CasHealthCouldNotLogUserDataCenter(testDomain, scriptName, errorString), innerException)
		{
			this.testDomain = testDomain;
			this.scriptName = scriptName;
			this.errorString = errorString;
		}

		protected CasHealthCouldNotLogUserDataCenterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.testDomain = (string)info.GetValue("testDomain", typeof(string));
			this.scriptName = (string)info.GetValue("scriptName", typeof(string));
			this.errorString = (string)info.GetValue("errorString", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("testDomain", this.testDomain);
			info.AddValue("scriptName", this.scriptName);
			info.AddValue("errorString", this.errorString);
		}

		public string TestDomain
		{
			get
			{
				return this.testDomain;
			}
		}

		public string ScriptName
		{
			get
			{
				return this.scriptName;
			}
		}

		public string ErrorString
		{
			get
			{
				return this.errorString;
			}
		}

		private readonly string testDomain;

		private readonly string scriptName;

		private readonly string errorString;
	}
}
