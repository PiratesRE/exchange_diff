using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Authorization
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RBACContextParserException : LocalizedException
	{
		public RBACContextParserException(int lineNumber, int position, string reason) : base(Strings.RBACContextParserException(lineNumber, position, reason))
		{
			this.lineNumber = lineNumber;
			this.position = position;
			this.reason = reason;
		}

		public RBACContextParserException(int lineNumber, int position, string reason, Exception innerException) : base(Strings.RBACContextParserException(lineNumber, position, reason), innerException)
		{
			this.lineNumber = lineNumber;
			this.position = position;
			this.reason = reason;
		}

		protected RBACContextParserException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.lineNumber = (int)info.GetValue("lineNumber", typeof(int));
			this.position = (int)info.GetValue("position", typeof(int));
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("lineNumber", this.lineNumber);
			info.AddValue("position", this.position);
			info.AddValue("reason", this.reason);
		}

		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		public int Position
		{
			get
			{
				return this.position;
			}
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly int lineNumber;

		private readonly int position;

		private readonly string reason;
	}
}
