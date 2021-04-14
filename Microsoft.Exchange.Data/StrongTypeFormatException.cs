using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class StrongTypeFormatException : FormatException
	{
		public StrongTypeFormatException(string message, string paramName) : base(message)
		{
			this.paramName = paramName;
		}

		public string ParamName
		{
			get
			{
				return this.paramName;
			}
		}

		protected StrongTypeFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.paramName = (string)info.GetValue("ParamName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ParamName", this.ParamName);
		}

		private string paramName;
	}
}
