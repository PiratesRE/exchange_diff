using System;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConversionLimits : ICloneable
	{
		public ConversionLimits(bool isInboundConversion)
		{
			this.maxMimeTextHeaderLength = 2000;
			this.maxMimeSubjectLength = 255;
			this.maxSize = int.MaxValue;
			this.maxMimeRecipients = 12288;
			this.maxBodyPartsTotal = 250;
			this.maxEmbeddedMessageDepth = (isInboundConversion ? 30 : 100);
			this.exemptPFReplicationMessages = true;
			this.mimeLimits = MimeLimits.Default;
		}

		public ConversionLimits(ConversionLimits origin)
		{
			this.maxMimeTextHeaderLength = origin.maxMimeTextHeaderLength;
			this.maxMimeSubjectLength = origin.maxMimeSubjectLength;
			this.maxSize = origin.maxSize;
			this.maxMimeRecipients = origin.maxMimeRecipients;
			this.maxBodyPartsTotal = origin.maxBodyPartsTotal;
			this.maxEmbeddedMessageDepth = origin.maxEmbeddedMessageDepth;
			this.exemptPFReplicationMessages = origin.exemptPFReplicationMessages;
			this.mimeLimits = origin.mimeLimits;
		}

		public override string ToString()
		{
			return string.Format("ConversionLimits:\r\n- maxMimeTextHeaderLength: {0}\r\n- maxMimeSubjectLength: {1}\r\n- maxSize: {2}\r\n- maxMimeRecipients: {3}\r\n- maxBodyPartsTotal: {4}\r\n- maxEmbeddedMessageDepth: {5}\r\n- exemptPFReplicationMessages: {6}\r\n", new object[]
			{
				this.maxMimeTextHeaderLength,
				this.maxMimeSubjectLength,
				this.maxSize,
				this.maxMimeRecipients,
				this.maxBodyPartsTotal,
				this.maxEmbeddedMessageDepth,
				this.exemptPFReplicationMessages
			});
		}

		public object Clone()
		{
			return new ConversionLimits(this);
		}

		public int MaxMimeTextHeaderLength
		{
			get
			{
				return this.maxMimeTextHeaderLength;
			}
			set
			{
				this.SetMaxMimeTextHeaderLength(value);
			}
		}

		public int MaxMimeSubjectLength
		{
			get
			{
				return this.maxMimeSubjectLength;
			}
			set
			{
				this.SetMaxMimeSubjectLength(value);
			}
		}

		public int MaxSize
		{
			get
			{
				return this.maxSize;
			}
			set
			{
				this.SetMaxSize(value);
			}
		}

		public int MaxMimeRecipients
		{
			get
			{
				return this.maxMimeRecipients;
			}
			set
			{
				this.SetMaxMimeRecipients(value);
			}
		}

		public int MaxBodyPartsTotal
		{
			get
			{
				return this.maxBodyPartsTotal;
			}
			set
			{
				this.SetMaxBodyPartsTotal(value);
			}
		}

		public int MaxEmbeddedMessageDepth
		{
			get
			{
				return this.maxEmbeddedMessageDepth;
			}
			set
			{
				this.SetMaxEmbeddedMessageDepth(value);
			}
		}

		public bool ExemptPFReplicationMessages
		{
			get
			{
				return this.exemptPFReplicationMessages;
			}
			set
			{
				this.exemptPFReplicationMessages = value;
			}
		}

		public MimeLimits MimeLimits
		{
			get
			{
				return this.mimeLimits;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("MimeLimits");
				}
				this.mimeLimits = value;
			}
		}

		private void SetMaxMimeTextHeaderLength(int maxMimeTextHeaderLength)
		{
			if (maxMimeTextHeaderLength < 78)
			{
				throw new ArgumentOutOfRangeException("maxMimeTextHeaderLength");
			}
			this.maxMimeTextHeaderLength = maxMimeTextHeaderLength;
		}

		private void SetMaxMimeSubjectLength(int maxMimeSubjectLength)
		{
			if (maxMimeSubjectLength < 78)
			{
				throw new ArgumentOutOfRangeException("maxMimeSubjectLength");
			}
			this.maxMimeSubjectLength = maxMimeSubjectLength;
		}

		private void SetMaxSize(int maxSize)
		{
			if (maxSize < 1024)
			{
				throw new ArgumentOutOfRangeException("maxSize");
			}
			this.maxSize = maxSize;
		}

		private void SetMaxMimeRecipients(int maxMimeRecipients)
		{
			if (maxMimeRecipients < 0)
			{
				throw new ArgumentOutOfRangeException("maxMimeRecipients");
			}
			this.maxMimeRecipients = maxMimeRecipients;
		}

		private void SetMaxBodyPartsTotal(int maxBodyPartsTotal)
		{
			if (maxBodyPartsTotal < 1)
			{
				throw new ArgumentOutOfRangeException("maxBodyPartsTotal");
			}
			this.maxBodyPartsTotal = maxBodyPartsTotal;
		}

		private void SetMaxEmbeddedMessageDepth(int maxEmbeddedMessageDepth)
		{
			if (maxEmbeddedMessageDepth < 0)
			{
				throw new ArgumentOutOfRangeException("maxEmbeddedMessageDepth");
			}
			this.maxEmbeddedMessageDepth = maxEmbeddedMessageDepth;
		}

		private int maxMimeTextHeaderLength;

		private int maxMimeSubjectLength;

		private int maxSize;

		private int maxMimeRecipients;

		private int maxBodyPartsTotal;

		private int maxEmbeddedMessageDepth;

		private bool exemptPFReplicationMessages;

		private MimeLimits mimeLimits;
	}
}
