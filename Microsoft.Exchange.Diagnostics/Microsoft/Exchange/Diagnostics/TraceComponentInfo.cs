using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics
{
	public class TraceComponentInfo
	{
		public TraceComponentInfo(string prettyName, Guid componentGuid, TraceTagInfo[] tagInfoList)
		{
			this.prettyName = prettyName;
			this.componentGuid = componentGuid;
			this.tagInfoList = tagInfoList;
			this.faultInjectionComponentConfig = new FaultInjectionComponentConfig();
		}

		public string PrettyName
		{
			get
			{
				return this.prettyName;
			}
		}

		public Guid ComponentGuid
		{
			get
			{
				return this.componentGuid;
			}
		}

		public TraceTagInfo[] TagInfoList
		{
			get
			{
				return this.tagInfoList;
			}
			set
			{
				this.tagInfoList = value;
			}
		}

		internal FaultInjectionComponentConfig FaultInjectionComponentConfig
		{
			get
			{
				return this.faultInjectionComponentConfig;
			}
		}

		private string prettyName;

		private Guid componentGuid;

		private TraceTagInfo[] tagInfoList;

		private FaultInjectionComponentConfig faultInjectionComponentConfig;
	}
}
