using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreKeyApiOperationException : LocalizedException
	{
		public DxStoreKeyApiOperationException(string operationName, string keyName) : base(Strings.DxStoreKeyApiOperationException(operationName, keyName))
		{
			this.operationName = operationName;
			this.keyName = keyName;
		}

		public DxStoreKeyApiOperationException(string operationName, string keyName, Exception innerException) : base(Strings.DxStoreKeyApiOperationException(operationName, keyName), innerException)
		{
			this.operationName = operationName;
			this.keyName = keyName;
		}

		protected DxStoreKeyApiOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.operationName = (string)info.GetValue("operationName", typeof(string));
			this.keyName = (string)info.GetValue("keyName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("operationName", this.operationName);
			info.AddValue("keyName", this.keyName);
		}

		public string OperationName
		{
			get
			{
				return this.operationName;
			}
		}

		public string KeyName
		{
			get
			{
				return this.keyName;
			}
		}

		private readonly string operationName;

		private readonly string keyName;
	}
}
