using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TextMatchingParsingException : LocalizedException
	{
		public TextMatchingParsingException(string diagnostic) : base(TextMatchingStrings.RegexPatternParsingError(diagnostic))
		{
			this.diagnostic = diagnostic;
		}

		public TextMatchingParsingException(string diagnostic, Exception innerException) : base(TextMatchingStrings.RegexPatternParsingError(diagnostic), innerException)
		{
			this.diagnostic = diagnostic;
		}

		protected TextMatchingParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.diagnostic = (string)info.GetValue("diagnostic", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("diagnostic", this.diagnostic);
		}

		public string Diagnostic
		{
			get
			{
				return this.diagnostic;
			}
		}

		private readonly string diagnostic;
	}
}
