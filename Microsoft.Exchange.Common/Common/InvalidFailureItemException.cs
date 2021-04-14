using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidFailureItemException : LocalizedException
	{
		public InvalidFailureItemException(string param) : base(CommonStrings.InvalidFailureItemException(param))
		{
			this.param = param;
		}

		public InvalidFailureItemException(string param, Exception innerException) : base(CommonStrings.InvalidFailureItemException(param), innerException)
		{
			this.param = param;
		}

		protected InvalidFailureItemException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.param = (string)info.GetValue("param", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("param", this.param);
		}

		public string Param
		{
			get
			{
				return this.param;
			}
		}

		private readonly string param;
	}
}
