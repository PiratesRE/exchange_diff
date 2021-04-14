using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.Authorization
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SerializedAccessTokenParserException : LocalizedException
	{
		public SerializedAccessTokenParserException(int lineNumber, int position, LocalizedString reason) : base(AuthorizationStrings.SerializedAccessTokenParserException(lineNumber, position, reason))
		{
			this.lineNumber = lineNumber;
			this.position = position;
			this.reason = reason;
		}

		public SerializedAccessTokenParserException(int lineNumber, int position, LocalizedString reason, Exception innerException) : base(AuthorizationStrings.SerializedAccessTokenParserException(lineNumber, position, reason), innerException)
		{
			this.lineNumber = lineNumber;
			this.position = position;
			this.reason = reason;
		}

		protected SerializedAccessTokenParserException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.lineNumber = (int)info.GetValue("lineNumber", typeof(int));
			this.position = (int)info.GetValue("position", typeof(int));
			this.reason = (LocalizedString)info.GetValue("reason", typeof(LocalizedString));
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

		public LocalizedString Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly int lineNumber;

		private readonly int position;

		private readonly LocalizedString reason;
	}
}
