using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ExpressionSyntaxException : LocalizedException
	{
		public ExpressionSyntaxException(string error) : base(Strings.ExpressionSyntaxException(error))
		{
			this.error = error;
		}

		public ExpressionSyntaxException(string error, Exception innerException) : base(Strings.ExpressionSyntaxException(error), innerException)
		{
			this.error = error;
		}

		protected ExpressionSyntaxException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
