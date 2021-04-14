using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ExArgumentNullException : LocalizedException
	{
		public ExArgumentNullException(string paramName) : base(DataStrings.ExArgumentNullException(paramName))
		{
			this.paramName = paramName;
		}

		public ExArgumentNullException(string paramName, Exception innerException) : base(DataStrings.ExArgumentNullException(paramName), innerException)
		{
			this.paramName = paramName;
		}

		protected ExArgumentNullException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.paramName = (string)info.GetValue("paramName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("paramName", this.paramName);
		}

		public string ParamName
		{
			get
			{
				return this.paramName;
			}
		}

		private readonly string paramName;
	}
}
