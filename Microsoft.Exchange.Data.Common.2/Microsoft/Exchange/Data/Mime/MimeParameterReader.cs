using System;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Mime
{
	public struct MimeParameterReader
	{
		internal MimeParameterReader(MimeReader reader)
		{
			this.reader = reader;
		}

		public string Name
		{
			get
			{
				this.AssertGood(true);
				return this.reader.ReadParameterName();
			}
		}

		public string Value
		{
			get
			{
				DecodingResults decodingResults;
				string result;
				if (!this.TryGetValue(this.reader.HeaderDecodingOptions, out decodingResults, out result))
				{
					MimeCommon.ThrowDecodingFailedException(ref decodingResults);
				}
				return result;
			}
		}

		public bool ReadNextParameter()
		{
			this.AssertGood(false);
			return this.reader.ReadNextDescendant(true);
		}

		public bool TryGetValue(out string value)
		{
			DecodingResults decodingResults;
			return this.TryGetValue(this.reader.HeaderDecodingOptions, out decodingResults, out value);
		}

		public bool TryGetValue(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string value)
		{
			this.AssertGood(true);
			return this.reader.TryReadParameterValue(decodingOptions, out decodingResults, out value);
		}

		private void AssertGood(bool checkPositionedOnParameter)
		{
			if (this.reader == null)
			{
				throw new NotSupportedException(Strings.ParameterReaderNotInitialized);
			}
			this.reader.AssertGoodToUse(true, true);
			if (this.reader.ReaderState != MimeReaderState.HeaderComplete || this.reader.CurrentHeaderObject == null || !(this.reader.CurrentHeaderObject is ComplexHeader))
			{
				throw new NotSupportedException(Strings.ReaderIsNotPositionedOnHeaderWithParameters);
			}
			if (checkPositionedOnParameter && !this.reader.IsCurrentChildValid(true))
			{
				throw new InvalidOperationException(Strings.ParameterReaderIsNotPositionedOnParameter);
			}
		}

		private MimeReader reader;
	}
}
