using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidOrganizationNameException : LocalizedException
	{
		public InvalidOrganizationNameException(string name) : base(Strings.InvalidOrganizationName(name))
		{
			this.name = name;
		}

		public InvalidOrganizationNameException(string name, Exception innerException) : base(Strings.InvalidOrganizationName(name), innerException)
		{
			this.name = name;
		}

		protected InvalidOrganizationNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
