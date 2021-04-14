using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RepairingIsNotApplicableForCurrentMonitorStateException : ActiveMonitoringServerTransientException
	{
		public RepairingIsNotApplicableForCurrentMonitorStateException(string monitorName, string targetResource, string alertState) : base(ServerStrings.RepairingIsNotApplicableForCurrentMonitorState(monitorName, targetResource, alertState))
		{
			this.monitorName = monitorName;
			this.targetResource = targetResource;
			this.alertState = alertState;
		}

		public RepairingIsNotApplicableForCurrentMonitorStateException(string monitorName, string targetResource, string alertState, Exception innerException) : base(ServerStrings.RepairingIsNotApplicableForCurrentMonitorState(monitorName, targetResource, alertState), innerException)
		{
			this.monitorName = monitorName;
			this.targetResource = targetResource;
			this.alertState = alertState;
		}

		protected RepairingIsNotApplicableForCurrentMonitorStateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.monitorName = (string)info.GetValue("monitorName", typeof(string));
			this.targetResource = (string)info.GetValue("targetResource", typeof(string));
			this.alertState = (string)info.GetValue("alertState", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("monitorName", this.monitorName);
			info.AddValue("targetResource", this.targetResource);
			info.AddValue("alertState", this.alertState);
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

		public string AlertState
		{
			get
			{
				return this.alertState;
			}
		}

		private readonly string monitorName;

		private readonly string targetResource;

		private readonly string alertState;
	}
}
