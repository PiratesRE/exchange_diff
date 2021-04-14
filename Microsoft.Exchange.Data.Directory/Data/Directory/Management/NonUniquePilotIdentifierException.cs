using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NonUniquePilotIdentifierException : LocalizedException
	{
		public NonUniquePilotIdentifierException(string pilotId, string dialPlan) : base(DirectoryStrings.NonUniquePilotIdentifier(pilotId, dialPlan))
		{
			this.pilotId = pilotId;
			this.dialPlan = dialPlan;
		}

		public NonUniquePilotIdentifierException(string pilotId, string dialPlan, Exception innerException) : base(DirectoryStrings.NonUniquePilotIdentifier(pilotId, dialPlan), innerException)
		{
			this.pilotId = pilotId;
			this.dialPlan = dialPlan;
		}

		protected NonUniquePilotIdentifierException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.pilotId = (string)info.GetValue("pilotId", typeof(string));
			this.dialPlan = (string)info.GetValue("dialPlan", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("pilotId", this.pilotId);
			info.AddValue("dialPlan", this.dialPlan);
		}

		public string PilotId
		{
			get
			{
				return this.pilotId;
			}
		}

		public string DialPlan
		{
			get
			{
				return this.dialPlan;
			}
		}

		private readonly string pilotId;

		private readonly string dialPlan;
	}
}
