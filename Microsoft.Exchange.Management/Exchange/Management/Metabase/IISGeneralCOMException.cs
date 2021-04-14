using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IISGeneralCOMException : DataSourceOperationException
	{
		public IISGeneralCOMException(string errorMsg, int code) : base(Strings.IISGeneralCOMException(errorMsg, code))
		{
			this.errorMsg = errorMsg;
			this.code = code;
		}

		public IISGeneralCOMException(string errorMsg, int code, Exception innerException) : base(Strings.IISGeneralCOMException(errorMsg, code), innerException)
		{
			this.errorMsg = errorMsg;
			this.code = code;
		}

		protected IISGeneralCOMException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorMsg = (string)info.GetValue("errorMsg", typeof(string));
			this.code = (int)info.GetValue("code", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorMsg", this.errorMsg);
			info.AddValue("code", this.code);
		}

		public string ErrorMsg
		{
			get
			{
				return this.errorMsg;
			}
		}

		public int Code
		{
			get
			{
				return this.code;
			}
		}

		private readonly string errorMsg;

		private readonly int code;
	}
}
