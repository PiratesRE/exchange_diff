using System;
using System.Diagnostics;
using System.IO;

namespace Microsoft.Exchange.Clients.Owa.Core.Transcoding
{
	internal class TranscodingParameters
	{
		public TranscodingParameters(string sessionId, string documentId, Stream sourceStream, string sourceDocType, int currentPageNumber)
		{
			this.sessionId = sessionId;
			this.documentId = documentId;
			this.sourceStream = sourceStream;
			this.sourceDocType = sourceDocType;
			this.currentPageNumber = currentPageNumber;
			this.errorCode = null;
			this.stopwatch = new Stopwatch();
		}

		public string SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		public string DocumentId
		{
			get
			{
				return this.documentId;
			}
		}

		public Stream SourceStream
		{
			get
			{
				return this.sourceStream;
			}
		}

		public string RewrittenHtmlFileName
		{
			get
			{
				return this.rewrittenHtmlFileName;
			}
			set
			{
				this.rewrittenHtmlFileName = value;
			}
		}

		public string SourceDocType
		{
			get
			{
				return this.sourceDocType;
			}
		}

		public int CurrentPageNumber
		{
			get
			{
				return this.currentPageNumber;
			}
		}

		public int TotalPageNumber
		{
			get
			{
				return this.totalPageNumber;
			}
			set
			{
				this.totalPageNumber = value;
			}
		}

		public Stopwatch Stopwatch
		{
			get
			{
				return this.stopwatch;
			}
		}

		public TranscodeErrorCode? ErrorCode
		{
			get
			{
				return this.errorCode;
			}
			set
			{
				this.errorCode = value;
			}
		}

		public bool IsLeftQueueHandled
		{
			get
			{
				return this.isLeftQueueHandled;
			}
			set
			{
				this.isLeftQueueHandled = value;
			}
		}

		public int SourceDocSize
		{
			get
			{
				return this.sourceDocSize;
			}
			set
			{
				this.sourceDocSize = value;
			}
		}

		private string sessionId;

		private string documentId;

		private Stream sourceStream;

		private string rewrittenHtmlFileName;

		private string sourceDocType;

		private int currentPageNumber;

		private int totalPageNumber;

		private bool isLeftQueueHandled;

		private TranscodeErrorCode? errorCode;

		private Stopwatch stopwatch;

		private int sourceDocSize;
	}
}
