using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal class SearchException : Exception
	{
		public SearchException()
		{
			this.Parameters = new List<object>();
		}

		public SearchException(Exception exception) : base(null, exception)
		{
			SearchException ex = exception as SearchException;
			if (ex != null)
			{
				this.Error = ex.Error;
				this.Parameters = ex.Parameters;
				return;
			}
			this.Error = KnownError.NA;
			this.Parameters = new List<object>();
		}

		public SearchException(KnownError error)
		{
			this.Error = error;
			this.Parameters = new List<object>();
		}

		public SearchException(KnownError error, params object[] parameters)
		{
			this.Error = error;
			this.Parameters = new List<object>();
			this.Parameters.AddRange(parameters);
		}

		public SearchException(KnownError error, Exception innerException) : base(error.ToString(), innerException)
		{
			this.Error = error;
			this.Parameters = new List<object>();
		}

		public SearchSource ErrorSource { get; set; }

		public KnownError Error { get; set; }

		public List<object> Parameters { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Error: {0}\r\n", this.Error);
			if (this.ErrorSource != null)
			{
				stringBuilder.AppendFormat("SourceId: {0}\r\n", this.ErrorSource.ReferenceId);
				stringBuilder.AppendFormat("SourceType: {0}\r\n", this.ErrorSource.SourceType);
				stringBuilder.AppendFormat("SourceLocation: {0}\r\n", this.ErrorSource.SourceLocation);
				stringBuilder.AppendFormat("HasRecipient: {0}\r\n", this.ErrorSource.Recipient != null);
				stringBuilder.AppendFormat("HasMailbox: {0}\r\n", this.ErrorSource.MailboxInfo != null);
				stringBuilder.AppendFormat("HasExtendedAttributes: {0}\r\n", this.ErrorSource.ExtendedAttributes != null && this.ErrorSource.ExtendedAttributes.Count > 0);
			}
			if (this.Parameters != null)
			{
				for (int i = 0; i < this.Parameters.Count; i++)
				{
					stringBuilder.AppendFormat("Parameter{0}: {1}\r\n", i, this.Parameters[i]);
				}
			}
			stringBuilder.AppendLine(base.ToString());
			return stringBuilder.ToString();
		}
	}
}
