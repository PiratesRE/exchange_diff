using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreKeyInvalidKeyException : ClusterApiException
	{
		public DxStoreKeyInvalidKeyException(string keyName) : base(Strings.DxStoreKeyInvalidKeyException(keyName))
		{
			this.keyName = keyName;
		}

		public DxStoreKeyInvalidKeyException(string keyName, Exception innerException) : base(Strings.DxStoreKeyInvalidKeyException(keyName), innerException)
		{
			this.keyName = keyName;
		}

		protected DxStoreKeyInvalidKeyException(SerializationInfo info, StreamingContext context) : base(info, context)
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
