using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ProvisioningValidationException : LocalizedException
	{
		public ProvisioningValidationException(string description, string agentName) : base(Strings.ErrorProvisioningValidation(description, agentName))
		{
			this.description = description;
			this.agentName = agentName;
		}

		public ProvisioningValidationException(string description, string agentName, Exception innerException) : base(Strings.ErrorProvisioningValidation(description, agentName), innerException)
		{
			this.description = description;
			this.agentName = agentName;
		}

		protected ProvisioningValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.description = (string)info.GetValue("description", typeof(string));
			this.agentName = (string)info.GetValue("agentName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("description", this.description);
			info.AddValue("agentName", this.agentName);
		}

		public string Description
		{
			get
			{
				return this.description;
			}
		}

		public string AgentName
		{
			get
			{
				return this.agentName;
			}
		}

		private readonly string description;

		private readonly string agentName;
	}
}
