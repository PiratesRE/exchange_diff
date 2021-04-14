using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RuleParsingException : LocalizedException
	{
		public RuleParsingException(string diagnostic, int lineNumber, int linePosition) : base(RulesStrings.RuleParsingError(diagnostic, lineNumber, linePosition))
		{
			this.diagnostic = diagnostic;
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
		}

		public RuleParsingException(string diagnostic, int lineNumber, int linePosition, Exception innerException) : base(RulesStrings.RuleParsingError(diagnostic, lineNumber, linePosition), innerException)
		{
			this.diagnostic = diagnostic;
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
		}

		protected RuleParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.diagnostic = (string)info.GetValue("diagnostic", typeof(string));
			this.lineNumber = (int)info.GetValue("lineNumber", typeof(int));
			this.linePosition = (int)info.GetValue("linePosition", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("diagnostic", this.diagnostic);
			info.AddValue("lineNumber", this.lineNumber);
			info.AddValue("linePosition", this.linePosition);
		}

		public string Diagnostic
		{
			get
			{
				return this.diagnostic;
			}
		}

		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		private readonly string diagnostic;

		private readonly int lineNumber;

		private readonly int linePosition;
	}
}
