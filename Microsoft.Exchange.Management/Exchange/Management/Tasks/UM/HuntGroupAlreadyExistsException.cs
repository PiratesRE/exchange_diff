using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class HuntGroupAlreadyExistsException : LocalizedException
	{
		public HuntGroupAlreadyExistsException(string ipGateway, string pilotIdentifier) : base(Strings.ExceptionHuntGroupAlreadyExists(ipGateway, pilotIdentifier))
		{
			this.ipGateway = ipGateway;
			this.pilotIdentifier = pilotIdentifier;
		}

		public HuntGroupAlreadyExistsException(string ipGateway, string pilotIdentifier, Exception innerException) : base(Strings.ExceptionHuntGroupAlreadyExists(ipGateway, pilotIdentifier), innerException)
		{
			this.ipGateway = ipGateway;
			this.pilotIdentifier = pilotIdentifier;
		}

		protected HuntGroupAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ipGateway = (string)info.GetValue("ipGateway", typeof(string));
			this.pilotIdentifier = (string)info.GetValue("pilotIdentifier", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ipGateway", this.ipGateway);
			info.AddValue("pilotIdentifier", this.pilotIdentifier);
		}

		public string IpGateway
		{
			get
			{
				return this.ipGateway;
			}
		}

		public string PilotIdentifier
		{
			get
			{
				return this.pilotIdentifier;
			}
		}

		private readonly string ipGateway;

		private readonly string pilotIdentifier;
	}
}
