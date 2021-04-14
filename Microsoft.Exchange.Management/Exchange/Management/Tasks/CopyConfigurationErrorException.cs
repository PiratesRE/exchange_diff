using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CopyConfigurationErrorException : LocalizedException
	{
		public CopyConfigurationErrorException(string exception) : base(Strings.CopyConfigurationErrorException(exception))
		{
			this.exception = exception;
		}

		public CopyConfigurationErrorException(string exception, Exception innerException) : base(Strings.CopyConfigurationErrorException(exception), innerException)
		{
			this.exception = exception;
		}

		protected CopyConfigurationErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.exception = (string)info.GetValue("exception", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("exception", this.exception);
		}

		public string Exception
		{
			get
			{
				return this.exception;
			}
		}

		private readonly string exception;
	}
}
