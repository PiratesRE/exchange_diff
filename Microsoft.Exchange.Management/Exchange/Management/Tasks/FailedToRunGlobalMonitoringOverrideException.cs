using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToRunGlobalMonitoringOverrideException : LocalizedException
	{
		public FailedToRunGlobalMonitoringOverrideException(string container) : base(Strings.FailedToRunGlobalMonitoringOverride(container))
		{
			this.container = container;
		}

		public FailedToRunGlobalMonitoringOverrideException(string container, Exception innerException) : base(Strings.FailedToRunGlobalMonitoringOverride(container), innerException)
		{
			this.container = container;
		}

		protected FailedToRunGlobalMonitoringOverrideException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.container = (string)info.GetValue("container", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("container", this.container);
		}

		public string Container
		{
			get
			{
				return this.container;
			}
		}

		private readonly string container;
	}
}
