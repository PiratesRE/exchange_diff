using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal struct ParseAndProcessResult<TEvent> : IEquatable<ParseAndProcessResult<TEvent>> where TEvent : struct
	{
		public ParseAndProcessResult(ParseResult parseResult, TEvent smtpEvent)
		{
			this = default(ParseAndProcessResult<TEvent>);
			this.parseResult = parseResult;
			this.SmtpEvent = smtpEvent;
		}

		public ParseAndProcessResult(ParsingStatus parsingStatus, SmtpResponse smtpResponse, TEvent smtpEvent, bool disconnectClient = false)
		{
			this = new ParseAndProcessResult<TEvent>(new ParseResult(parsingStatus, smtpResponse, disconnectClient), smtpEvent);
		}

		public ParseResult ParseResult
		{
			get
			{
				return this.parseResult;
			}
		}

		public ParsingStatus ParsingStatus
		{
			get
			{
				return this.parseResult.ParsingStatus;
			}
		}

		public SmtpResponse SmtpResponse
		{
			get
			{
				return this.parseResult.SmtpResponse;
			}
		}

		public TEvent SmtpEvent { get; private set; }

		public override string ToString()
		{
			return string.Format("{0}, {1}", this.parseResult, this.SmtpEvent);
		}

		public override bool Equals(object other)
		{
			return other is ParseAndProcessResult<TEvent> && this.Equals((ParseAndProcessResult<TEvent>)other);
		}

		public override int GetHashCode()
		{
			int num = 17 + 31 * this.parseResult.GetHashCode();
			int num2 = 31;
			TEvent smtpEvent = this.SmtpEvent;
			return num + num2 * smtpEvent.GetHashCode();
		}

		public static bool operator ==(ParseAndProcessResult<TEvent> lhs, ParseAndProcessResult<TEvent> rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ParseAndProcessResult<TEvent> lhs, ParseAndProcessResult<TEvent> rhs)
		{
			return !lhs.Equals(rhs);
		}

		public bool Equals(ParseAndProcessResult<TEvent> other)
		{
			if (this.parseResult.Equals(other.parseResult))
			{
				TEvent smtpEvent = this.SmtpEvent;
				return smtpEvent.Equals(other.SmtpEvent);
			}
			return false;
		}

		private readonly ParseResult parseResult;
	}
}
