using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthCouldNotLogUserDataCenterNoDetailedInfoException : LocalizedException
	{
		public CasHealthCouldNotLogUserDataCenterNoDetailedInfoException(string testDomain, string scriptName) : base(Strings.CasHealthCouldNotLogUserDataCenterNoDetailedInfo(testDomain, scriptName))
		{
			this.testDomain = testDomain;
			this.scriptName = scriptName;
		}

		public CasHealthCouldNotLogUserDataCenterNoDetailedInfoException(string testDomain, string scriptName, Exception innerException) : base(Strings.CasHealthCouldNotLogUserDataCenterNoDetailedInfo(testDomain, scriptName), innerException)
		{
			this.testDomain = testDomain;
			this.scriptName = scriptName;
		}

		protected CasHealthCouldNotLogUserDataCenterNoDetailedInfoException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.testDomain = (string)info.GetValue("testDomain", typeof(string));
			this.scriptName = (string)info.GetValue("scriptName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("testDomain", this.testDomain);
			info.AddValue("scriptName", this.scriptName);
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

		private readonly string testDomain;

		private readonly string scriptName;
	}
}
