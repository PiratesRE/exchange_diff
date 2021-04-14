using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DlpPolicyParsingException : LocalizedException
	{
		public DlpPolicyParsingException(string error) : base(Strings.DlpPolicyParsingError(error))
		{
			this.error = error;
		}

		public DlpPolicyParsingException(string error, Exception innerException) : base(Strings.DlpPolicyParsingError(error), innerException)
		{
			this.error = error;
		}

		protected DlpPolicyParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
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
