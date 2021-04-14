using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreBindingNotSupportedException : DxStoreServerException
	{
		public DxStoreBindingNotSupportedException(string bindingStr) : base(Strings.DxStoreBindingNotSupportedException(bindingStr))
		{
			this.bindingStr = bindingStr;
		}

		public DxStoreBindingNotSupportedException(string bindingStr, Exception innerException) : base(Strings.DxStoreBindingNotSupportedException(bindingStr), innerException)
		{
			this.bindingStr = bindingStr;
		}

		protected DxStoreBindingNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.bindingStr = (string)info.GetValue("bindingStr", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("bindingStr", this.bindingStr);
		}

		public string BindingStr
		{
			get
			{
				return this.bindingStr;
			}
		}

		private readonly string bindingStr;
	}
}
