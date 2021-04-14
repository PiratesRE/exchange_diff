using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreInstanceKeyNotFoundException : DxStoreInstanceServerException
	{
		public DxStoreInstanceKeyNotFoundException(string keyName) : base(Strings.DxStoreInstanceKeyNotFound(keyName))
		{
			this.keyName = keyName;
		}

		public DxStoreInstanceKeyNotFoundException(string keyName, Exception innerException) : base(Strings.DxStoreInstanceKeyNotFound(keyName), innerException)
		{
			this.keyName = keyName;
		}

		protected DxStoreInstanceKeyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
