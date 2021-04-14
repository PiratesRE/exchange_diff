using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class BackupKeyInaccessibleException : LocalizedException
	{
		public BackupKeyInaccessibleException(string keyName) : base(Strings.BackupKeyInaccessible(keyName))
		{
			this.keyName = keyName;
		}

		public BackupKeyInaccessibleException(string keyName, Exception innerException) : base(Strings.BackupKeyInaccessible(keyName), innerException)
		{
			this.keyName = keyName;
		}

		protected BackupKeyInaccessibleException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.keyName = (string)info.GetValue("keyName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("keyName", this.keyName);
		}

		public string KeyName
		{
			get
			{
				return this.keyName;
			}
		}

		private readonly string keyName;
	}
}
