using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExtensionNotUniqueException : LocalizedException
	{
		public ExtensionNotUniqueException(string s, string dialPlan) : base(DirectoryStrings.ExtensionNotUnique(s, dialPlan))
		{
			this.s = s;
			this.dialPlan = dialPlan;
		}

		public ExtensionNotUniqueException(string s, string dialPlan, Exception innerException) : base(DirectoryStrings.ExtensionNotUnique(s, dialPlan), innerException)
		{
			this.s = s;
			this.dialPlan = dialPlan;
		}

		protected ExtensionNotUniqueException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.s = (string)info.GetValue("s", typeof(string));
			this.dialPlan = (string)info.GetValue("dialPlan", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("s", this.s);
			info.AddValue("dialPlan", this.dialPlan);
		}

		public string S
		{
			get
			{
				return this.s;
			}
		}

		public string DialPlan
		{
			get
			{
				return this.dialPlan;
			}
		}

		private readonly string s;

		private readonly string dialPlan;
	}
}
