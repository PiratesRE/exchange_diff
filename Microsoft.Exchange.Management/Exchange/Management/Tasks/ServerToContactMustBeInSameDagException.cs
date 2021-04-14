using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServerToContactMustBeInSameDagException : LocalizedException
	{
		public ServerToContactMustBeInSameDagException(string serverToContact, string expectedDag, string actualDag) : base(Strings.ServerToContactMustBeInSameDagException(serverToContact, expectedDag, actualDag))
		{
			this.serverToContact = serverToContact;
			this.expectedDag = expectedDag;
			this.actualDag = actualDag;
		}

		public ServerToContactMustBeInSameDagException(string serverToContact, string expectedDag, string actualDag, Exception innerException) : base(Strings.ServerToContactMustBeInSameDagException(serverToContact, expectedDag, actualDag), innerException)
		{
			this.serverToContact = serverToContact;
			this.expectedDag = expectedDag;
			this.actualDag = actualDag;
		}

		protected ServerToContactMustBeInSameDagException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverToContact = (string)info.GetValue("serverToContact", typeof(string));
			this.expectedDag = (string)info.GetValue("expectedDag", typeof(string));
			this.actualDag = (string)info.GetValue("actualDag", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverToContact", this.serverToContact);
			info.AddValue("expectedDag", this.expectedDag);
			info.AddValue("actualDag", this.actualDag);
		}

		public string ServerToContact
		{
			get
			{
				return this.serverToContact;
			}
		}

		public string ExpectedDag
		{
			get
			{
				return this.expectedDag;
			}
		}

		public string ActualDag
		{
			get
			{
				return this.actualDag;
			}
		}

		private readonly string serverToContact;

		private readonly string expectedDag;

		private readonly string actualDag;
	}
}
