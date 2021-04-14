using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.PolicyNudges
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NewPolicyTipConfigInvalidNameException : LocalizedException
	{
		public NewPolicyTipConfigInvalidNameException(string supportedLocalesString, string supportedActionsString) : base(Strings.NewPolicyTipConfigInvalidName(supportedLocalesString, supportedActionsString))
		{
			this.supportedLocalesString = supportedLocalesString;
			this.supportedActionsString = supportedActionsString;
		}

		public NewPolicyTipConfigInvalidNameException(string supportedLocalesString, string supportedActionsString, Exception innerException) : base(Strings.NewPolicyTipConfigInvalidName(supportedLocalesString, supportedActionsString), innerException)
		{
			this.supportedLocalesString = supportedLocalesString;
			this.supportedActionsString = supportedActionsString;
		}

		protected NewPolicyTipConfigInvalidNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.supportedLocalesString = (string)info.GetValue("supportedLocalesString", typeof(string));
			this.supportedActionsString = (string)info.GetValue("supportedActionsString", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("supportedLocalesString", this.supportedLocalesString);
			info.AddValue("supportedActionsString", this.supportedActionsString);
		}

		public string SupportedLocalesString
		{
			get
			{
				return this.supportedLocalesString;
			}
		}

		public string SupportedActionsString
		{
			get
			{
				return this.supportedActionsString;
			}
		}

		private readonly string supportedLocalesString;

		private readonly string supportedActionsString;
	}
}
