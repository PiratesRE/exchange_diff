using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ClientSessionInfoTypeParseException : StoragePermanentException
	{
		public ClientSessionInfoTypeParseException(string typeName, string assemblyName) : base(ServerStrings.idClientSessionInfoTypeParseException(typeName, assemblyName))
		{
			this.typeName = typeName;
			this.assemblyName = assemblyName;
		}

		public ClientSessionInfoTypeParseException(string typeName, string assemblyName, Exception innerException) : base(ServerStrings.idClientSessionInfoTypeParseException(typeName, assemblyName), innerException)
		{
			this.typeName = typeName;
			this.assemblyName = assemblyName;
		}

		protected ClientSessionInfoTypeParseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.typeName = (string)info.GetValue("typeName", typeof(string));
			this.assemblyName = (string)info.GetValue("assemblyName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("typeName", this.typeName);
			info.AddValue("assemblyName", this.assemblyName);
		}

		public string TypeName
		{
			get
			{
				return this.typeName;
			}
		}

		public string AssemblyName
		{
			get
			{
				return this.assemblyName;
			}
		}

		private readonly string typeName;

		private readonly string assemblyName;
	}
}
