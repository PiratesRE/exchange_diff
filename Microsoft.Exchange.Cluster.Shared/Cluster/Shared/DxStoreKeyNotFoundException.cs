using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreKeyNotFoundException : LocalizedException
	{
		public DxStoreKeyNotFoundException(string keyName) : base(Strings.DxStoreKeyNotFoundException(keyName))
		{
			this.keyName = keyName;
		}

		public DxStoreKeyNotFoundException(string keyName, Exception innerException) : base(Strings.DxStoreKeyNotFoundException(keyName), innerException)
		{
			this.keyName = keyName;
		}

		protected DxStoreKeyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
