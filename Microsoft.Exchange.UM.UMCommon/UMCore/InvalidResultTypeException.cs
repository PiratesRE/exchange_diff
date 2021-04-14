using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidResultTypeException : LocalizedException
	{
		public InvalidResultTypeException(string resultType) : base(Strings.InvalidResultTypeException(resultType))
		{
			this.resultType = resultType;
		}

		public InvalidResultTypeException(string resultType, Exception innerException) : base(Strings.InvalidResultTypeException(resultType), innerException)
		{
			this.resultType = resultType;
		}

		protected InvalidResultTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.resultType = (string)info.GetValue("resultType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("resultType", this.resultType);
		}

		public string ResultType
		{
			get
			{
				return this.resultType;
			}
		}

		private readonly string resultType;
	}
}
