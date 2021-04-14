using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DuplicateTPDKeyIdException : LocalizedException
	{
		public DuplicateTPDKeyIdException(string keyIdType, string keyId) : base(Strings.DuplicateTPDKeyId(keyIdType, keyId))
		{
			this.keyIdType = keyIdType;
			this.keyId = keyId;
		}

		public DuplicateTPDKeyIdException(string keyIdType, string keyId, Exception innerException) : base(Strings.DuplicateTPDKeyId(keyIdType, keyId), innerException)
		{
			this.keyIdType = keyIdType;
			this.keyId = keyId;
		}

		protected DuplicateTPDKeyIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.keyIdType = (string)info.GetValue("keyIdType", typeof(string));
			this.keyId = (string)info.GetValue("keyId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("keyIdType", this.keyIdType);
			info.AddValue("keyId", this.keyId);
		}

		public string KeyIdType
		{
			get
			{
				return this.keyIdType;
			}
		}

		public string KeyId
		{
			get
			{
				return this.keyId;
			}
		}

		private readonly string keyIdType;

		private readonly string keyId;
	}
}
