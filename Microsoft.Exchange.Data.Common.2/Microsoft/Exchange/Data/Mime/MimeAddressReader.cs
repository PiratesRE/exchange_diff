using System;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Mime
{
	public struct MimeAddressReader
	{
		internal MimeAddressReader(MimeReader reader, bool topLevel)
		{
			this.reader = reader;
			this.topLevel = topLevel;
		}

		public bool IsGroup
		{
			get
			{
				this.AssertGood(true);
				return this.topLevel && this.reader.GroupInProgress;
			}
		}

		public MimeAddressReader GroupRecipientReader
		{
			get
			{
				if (!this.IsGroup)
				{
					throw new InvalidOperationException(Strings.AddressReaderIsNotPositionedOnAGroup);
				}
				return new MimeAddressReader(this.reader, false);
			}
		}

		public string DisplayName
		{
			get
			{
				DecodingResults decodingResults;
				string result;
				if (!this.TryGetDisplayName(this.reader.HeaderDecodingOptions, out decodingResults, out result))
				{
					MimeCommon.ThrowDecodingFailedException(ref decodingResults);
				}
				return result;
			}
		}

		public string Email
		{
			get
			{
				this.AssertGood(true);
				return this.reader.ReadRecipientEmail(this.topLevel);
			}
		}

		public bool ReadNextAddress()
		{
			this.AssertGood(false);
			return this.reader.ReadNextDescendant(this.topLevel);
		}

		public bool TryGetDisplayName(out string displayName)
		{
			DecodingResults decodingResults;
			return this.TryGetDisplayName(this.reader.HeaderDecodingOptions, out decodingResults, out displayName);
		}

		public bool TryGetDisplayName(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string displayName)
		{
			this.AssertGood(true);
			return this.reader.TryReadDisplayName(this.topLevel, decodingOptions, out decodingResults, out displayName);
		}

		private void AssertGood(bool checkPositionedOnAddress)
		{
			if (this.reader == null)
			{
				throw new NotSupportedException(Strings.AddressReaderNotInitialized);
			}
			this.reader.AssertGoodToUse(true, true);
			if (this.reader.ReaderState != MimeReaderState.HeaderComplete || this.reader.CurrentHeaderObject == null || !(this.reader.CurrentHeaderObject is AddressHeader))
			{
				throw new NotSupportedException(Strings.ReaderIsNotPositionedOnAddressHeader);
			}
			if (!this.topLevel && !this.reader.GroupInProgress)
			{
				throw new InvalidOperationException(Strings.AddressReaderIsNotPositionedOnAddress);
			}
			if (checkPositionedOnAddress && !this.reader.IsCurrentChildValid(this.topLevel))
			{
				throw new InvalidOperationException(Strings.AddressReaderIsNotPositionedOnAddress);
			}
		}

		private MimeReader reader;

		private bool topLevel;
	}
}
