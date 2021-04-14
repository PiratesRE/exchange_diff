using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MonitoringItemAlreadyHasLocalOverrideException : LocalizedException
	{
		public MonitoringItemAlreadyHasLocalOverrideException(string workitem, string overrideName, string workitemType) : base(Strings.MonitoringItemAlreadyHasLocalOverride(workitem, overrideName, workitemType))
		{
			this.workitem = workitem;
			this.overrideName = overrideName;
			this.workitemType = workitemType;
		}

		public MonitoringItemAlreadyHasLocalOverrideException(string workitem, string overrideName, string workitemType, Exception innerException) : base(Strings.MonitoringItemAlreadyHasLocalOverride(workitem, overrideName, workitemType), innerException)
		{
			this.workitem = workitem;
			this.overrideName = overrideName;
			this.workitemType = workitemType;
		}

		protected MonitoringItemAlreadyHasLocalOverrideException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.workitem = (string)info.GetValue("workitem", typeof(string));
			this.overrideName = (string)info.GetValue("overrideName", typeof(string));
			this.workitemType = (string)info.GetValue("workitemType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("workitem", this.workitem);
			info.AddValue("overrideName", this.overrideName);
			info.AddValue("workitemType", this.workitemType);
		}

		public string Workitem
		{
			get
			{
				return this.workitem;
			}
		}

		public string OverrideName
		{
			get
			{
				return this.overrideName;
			}
		}

		public string WorkitemType
		{
			get
			{
				return this.workitemType;
			}
		}

		private readonly string workitem;

		private readonly string overrideName;

		private readonly string workitemType;
	}
}
