using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SIPResouceIdConflictWithExistingValue : LocalizedException
	{
		public SIPResouceIdConflictWithExistingValue(string sipResId, string sipProxy) : base(Strings.ExceptionSIPResouceIdConflictWithExistingValue(sipResId, sipProxy))
		{
			this.sipResId = sipResId;
			this.sipProxy = sipProxy;
		}

		public SIPResouceIdConflictWithExistingValue(string sipResId, string sipProxy, Exception innerException) : base(Strings.ExceptionSIPResouceIdConflictWithExistingValue(sipResId, sipProxy), innerException)
		{
			this.sipResId = sipResId;
			this.sipProxy = sipProxy;
		}

		protected SIPResouceIdConflictWithExistingValue(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.sipResId = (string)info.GetValue("sipResId", typeof(string));
			this.sipProxy = (string)info.GetValue("sipProxy", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("sipResId", this.sipResId);
			info.AddValue("sipProxy", this.sipProxy);
		}

		public string SipResId
		{
			get
			{
				return this.sipResId;
			}
		}

		public string SipProxy
		{
			get
			{
				return this.sipProxy;
			}
		}

		private readonly string sipResId;

		private readonly string sipProxy;
	}
}
