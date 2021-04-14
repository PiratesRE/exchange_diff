using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RepairingIsNotSetSinceMonitorEntryIsNotFoundException : ActiveMonitoringServerTransientException
	{
		public RepairingIsNotSetSinceMonitorEntryIsNotFoundException(string monitorName, string targetResource) : base(ServerStrings.RepairingIsNotSetSinceMonitorEntryIsNotFound(monitorName, targetResource))
		{
			this.monitorName = monitorName;
			this.targetResource = targetResource;
		}

		public RepairingIsNotSetSinceMonitorEntryIsNotFoundException(string monitorName, string targetResource, Exception innerException) : base(ServerStrings.RepairingIsNotSetSinceMonitorEntryIsNotFound(monitorName, targetResource), innerException)
		{
			this.monitorName = monitorName;
			this.targetResource = targetResource;
		}

		protected RepairingIsNotSetSinceMonitorEntryIsNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.monitorName = (string)info.GetValue("monitorName", typeof(string));
			this.targetResource = (string)info.GetValue("targetResource", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("monitorName", this.monitorName);
			info.AddValue("targetResource", this.targetResource);
		}

		public string MonitorName
		{
			get
			{
				return this.monitorName;
			}
		}

		public string TargetResource
		{
			get
			{
				return this.targetResource;
			}
		}

		private readonly string monitorName;

		private readonly string targetResource;
	}
}
