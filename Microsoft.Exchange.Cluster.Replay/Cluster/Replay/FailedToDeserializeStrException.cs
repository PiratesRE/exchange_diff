using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToDeserializeStrException : DumpsterRedeliveryException
	{
		public FailedToDeserializeStrException(string stringToDeserialize, string typeName) : base(ReplayStrings.FailedToDeserializeStr(stringToDeserialize, typeName))
		{
			this.stringToDeserialize = stringToDeserialize;
			this.typeName = typeName;
		}

		public FailedToDeserializeStrException(string stringToDeserialize, string typeName, Exception innerException) : base(ReplayStrings.FailedToDeserializeStr(stringToDeserialize, typeName), innerException)
		{
			this.stringToDeserialize = stringToDeserialize;
			this.typeName = typeName;
		}

		protected FailedToDeserializeStrException(SerializationInfo info, StreamingContext context) : base(info, context)
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
