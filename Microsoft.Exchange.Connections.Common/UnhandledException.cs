using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnhandledException : LocalizedException
	{
		public UnhandledException(string typeName) : base(CXStrings.UnhandledError(typeName))
		{
			this.typeName = typeName;
		}

		public UnhandledException(string typeName, Exception innerException) : base(CXStrings.UnhandledError(typeName), innerException)
		{
			this.typeName = typeName;
		}

		protected UnhandledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.typeName = (string)info.GetValue("typeName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("typeName", this.typeName);
		}

		public string TypeName
		{
			get
			{
				return this.typeName;
			}
		}

		private readonly string typeName;
	}
}
