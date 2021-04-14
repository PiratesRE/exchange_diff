using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ConfigKeyAccessRuntimeException : AnchorRuntimeException
	{
		public ConfigKeyAccessRuntimeException(string keyname) : base(Strings.ConfigKeyAccessRuntimeError(keyname))
		{
			this.keyname = keyname;
		}

		public ConfigKeyAccessRuntimeException(string keyname, Exception innerException) : base(Strings.ConfigKeyAccessRuntimeError(keyname), innerException)
		{
			this.keyname = keyname;
		}

		protected ConfigKeyAccessRuntimeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.keyname = (string)info.GetValue("keyname", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("keyname", this.keyname);
		}

		public string Keyname
		{
			get
			{
				return this.keyname;
			}
		}

		private readonly string keyname;
	}
}
