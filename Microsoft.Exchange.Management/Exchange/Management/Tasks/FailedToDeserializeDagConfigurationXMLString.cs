using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToDeserializeDagConfigurationXMLString : LocalizedException
	{
		public FailedToDeserializeDagConfigurationXMLString(string stringToDeserialize, string typeName) : base(Strings.FailedToDeserializeDagConfigurationXMLString(stringToDeserialize, typeName))
		{
			this.stringToDeserialize = stringToDeserialize;
			this.typeName = typeName;
		}

		public FailedToDeserializeDagConfigurationXMLString(string stringToDeserialize, string typeName, Exception innerException) : base(Strings.FailedToDeserializeDagConfigurationXMLString(stringToDeserialize, typeName), innerException)
		{
			this.stringToDeserialize = stringToDeserialize;
			this.typeName = typeName;
		}

		protected FailedToDeserializeDagConfigurationXMLString(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.stringToDeserialize = (string)info.GetValue("stringToDeserialize", typeof(string));
			this.typeName = (string)info.GetValue("typeName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("stringToDeserialize", this.stringToDeserialize);
			info.AddValue("typeName", this.typeName);
		}

		public string StringToDeserialize
		{
			get
			{
				return this.stringToDeserialize;
			}
		}

		public string TypeName
		{
			get
			{
				return this.typeName;
			}
		}

		private readonly string stringToDeserialize;

		private readonly string typeName;
	}
}
