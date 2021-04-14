using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class BackupKeyIsWrongTypeException : LocalizedException
	{
		public BackupKeyIsWrongTypeException(string keyName, string valueName) : base(Strings.BackupKeyIsWrongType(keyName, valueName))
		{
			this.keyName = keyName;
			this.valueName = valueName;
		}

		public BackupKeyIsWrongTypeException(string keyName, string valueName, Exception innerException) : base(Strings.BackupKeyIsWrongType(keyName, valueName), innerException)
		{
			this.keyName = keyName;
			this.valueName = valueName;
		}

		protected BackupKeyIsWrongTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.keyName = (string)info.GetValue("keyName", typeof(string));
			this.valueName = (string)info.GetValue("valueName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("keyName", this.keyName);
			info.AddValue("valueName", this.valueName);
		}

		public string KeyName
		{
			get
			{
				return this.keyName;
			}
		}

		public string ValueName
		{
			get
			{
				return this.valueName;
			}
		}

		private readonly string keyName;

		private readonly string valueName;
	}
}
