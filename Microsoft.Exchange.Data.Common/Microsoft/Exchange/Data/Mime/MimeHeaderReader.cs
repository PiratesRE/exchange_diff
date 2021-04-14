using System;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Mime
{
	public struct MimeHeaderReader
	{
		internal MimeHeaderReader(MimeReader reader)
		{
			this.reader = reader;
		}

		internal MimeReader MimeReader
		{
			get
			{
				return this.reader;
			}
		}

		public HeaderId HeaderId
		{
			get
			{
				this.AssertGood(true);
				return this.reader.HeaderId;
			}
		}

		public string Name
		{
			get
			{
				this.AssertGood(true);
				return this.reader.HeaderName;
			}
		}

		public bool IsAddressHeader
		{
			get
			{
				this.AssertGood(true);
				return Header.TypeFromHeaderId(this.HeaderId) == typeof(AddressHeader);
			}
		}

		public MimeAddressReader AddressReader
		{
			get
			{
				if (!this.IsAddressHeader)
				{
					throw new InvalidOperationException(Strings.HeaderCannotHaveAddresses);
				}
				if (this.reader.ReaderState == MimeReaderState.HeaderStart)
				{
					this.reader.TryCompleteCurrentHeader(true);
				}
				return new MimeAddressReader(this.reader, true);
			}
		}

		public bool CanHaveParameters
		{
			get
			{
				this.AssertGood(true);
				Type left = Header.TypeFromHeaderId(this.HeaderId);
				return left == typeof(ContentTypeHeader) || left == typeof(ContentDispositionHeader);
			}
		}

		public MimeParameterReader ParameterReader
		{
			get
			{
				if (!this.CanHaveParameters)
				{
					throw new InvalidOperationException(Strings.HeaderCannotHaveParameters);
				}
				if (this.reader.ReaderState == MimeReaderState.HeaderStart)
				{
					this.reader.TryCompleteCurrentHeader(true);
				}
				return new MimeParameterReader(this.reader);
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

		public DateTime ReadValueAsDateTime()
		{
			this.AssertGood(true);
			if (this.reader.ReaderState == MimeReaderState.HeaderStart)
			{
				this.reader.TryCompleteCurrentHeader(true);
			}
			if (this.reader.CurrentHeaderObject == null)
			{
				return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
			}
			DateHeader dateHeader = this.reader.CurrentHeaderObject as DateHeader;
			if (dateHeader != null)
			{
				return dateHeader.DateTime;
			}
			return DateHeader.ParseDateHeaderValue(this.reader.CurrentHeaderObject.Value);
		}

		public bool ReadNextHeader()
		{
			this.AssertGood(false);
			while (this.reader.ReadNextHeader())
			{
				if (this.reader.HeaderName != null)
				{
					return true;
				}
			}
			return false;
		}

		public bool TryGetValue(out string value)
		{
			DecodingResults decodingResults;
			return this.TryGetValue(this.reader.HeaderDecodingOptions, out decodingResults, out value);
		}

		public bool TryGetValue(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string value)
		{
			this.AssertGood(true);
			if (this.reader.ReaderState == MimeReaderState.HeaderStart)
			{
				this.reader.TryCompleteCurrentHeader(true);
			}
			if (this.reader.CurrentHeaderObject != null)
			{
				TextHeader textHeader = this.reader.CurrentHeaderObject as TextHeader;
				if (textHeader != null)
				{
					value = textHeader.GetDecodedValue(decodingOptions, out decodingResults);
					if (decodingResults.DecodingFailed)
					{
						value = null;
						return false;
					}
					return true;
				}
				else
				{
					value = this.reader.CurrentHeaderObject.Value;
				}
			}
			else
			{
				value = null;
			}
			decodingResults = default(DecodingResults);
			return true;
		}

		private void AssertGood(bool checkPositionedOnHeader)
		{
			if (this.reader == null)
			{
				throw new NotSupportedException(Strings.HeaderReaderNotInitialized);
			}
			this.reader.AssertGoodToUse(true, true);
			if (!MimeReader.StateIsOneOf(this.reader.ReaderState, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderIncomplete | MimeReaderState.HeaderComplete | MimeReaderState.EndOfHeaders | MimeReaderState.InlineStart))
			{
				throw new NotSupportedException(Strings.HeaderReaderCannotBeUsedInThisState);
			}
			if (checkPositionedOnHeader && MimeReader.StateIsOneOf(this.reader.ReaderState, MimeReaderState.PartStart | MimeReaderState.EndOfHeaders))
			{
				throw new InvalidOperationException(Strings.HeaderReaderIsNotPositionedOnAHeader);
			}
		}

		private MimeReader reader;
	}
}
