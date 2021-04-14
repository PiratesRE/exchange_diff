using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RegistryParameterReadException : RegistryParameterException
	{
		public RegistryParameterReadException(string valueName, string errMsg) : base(Strings.RegistryParameterReadException(valueName, errMsg))
		{
			this.valueName = valueName;
			this.errMsg = errMsg;
		}

		public RegistryParameterReadException(string valueName, string errMsg, Exception innerException) : base(Strings.RegistryParameterReadException(valueName, errMsg), innerException)
		{
			this.valueName = valueName;
			this.errMsg = errMsg;
		}

		protected RegistryParameterReadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.valueName = (string)info.GetValue("valueName", typeof(string));
			this.errMsg = (string)info.GetValue("errMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("valueName", this.valueName);
			info.AddValue("errMsg", this.errMsg);
		}

		public string ValueName
		{
			get
			{
				return this.valueName;
			}
		}

		public string ErrMsg
		{
			get
			{
				return this.errMsg;
			}
		}

		private readonly string valueName;

		private readonly string errMsg;
	}
}
