using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Dsn
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CodeNotADefaultException : LocalizedException
	{
		public CodeNotADefaultException(EnhancedStatusCode code) : base(Strings.DsnCodeNotADefault(code))
		{
			this.code = code;
		}

		public CodeNotADefaultException(EnhancedStatusCode code, Exception innerException) : base(Strings.DsnCodeNotADefault(code), innerException)
		{
			this.code = code;
		}

		protected CodeNotADefaultException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.code = (EnhancedStatusCode)info.GetValue("code", typeof(EnhancedStatusCode));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("code", this.code);
		}

		public EnhancedStatusCode Code
		{
			get
			{
				return this.code;
			}
		}

		private readonly EnhancedStatusCode code;
	}
}
