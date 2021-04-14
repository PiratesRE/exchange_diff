using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Microsoft.Exchange.HttpProxy
{
	internal class HttpRangeSpecifier
	{
		public HttpRangeSpecifier()
		{
			this.RangeUnitSpecifier = "bytes";
		}

		public Collection<HttpRange> RangeCollection
		{
			get
			{
				return this.rangeCollection;
			}
		}

		public string RangeUnitSpecifier { get; set; }

		public static HttpRangeSpecifier Parse(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException("value");
			}
			HttpRangeSpecifier httpRangeSpecifier = new HttpRangeSpecifier();
			string message;
			if (!HttpRangeSpecifier.TryParseInternal(value, httpRangeSpecifier, out message))
			{
				throw new ArgumentException(message);
			}
			return httpRangeSpecifier;
		}

		private static bool TryParseInternal(string value, HttpRangeSpecifier specifier, out string parseFailureReason)
		{
			if (specifier == null)
			{
				throw new ArgumentNullException("specifier");
			}
			HttpRangeSpecifier.StrSegment strSegment = new HttpRangeSpecifier.StrSegment(value);
			HttpRangeSpecifier.ParseState parseState = HttpRangeSpecifier.ParseState.Start;
			parseFailureReason = string.Empty;
			int i = 0;
			int length = value.Length;
			long rangeStart = -1L;
			while (i < length)
			{
				char c = value[i];
				switch (parseState)
				{
				case HttpRangeSpecifier.ParseState.Start:
					if (c != ' ' && c != '\t')
					{
						if (strSegment.Start == -1)
						{
							strSegment.Start = i;
						}
						if (c == '=')
						{
							strSegment.SetLengthFromTerminatingIndex(i);
							strSegment.Trim();
							specifier.RangeUnitSpecifier = strSegment.ToString();
							parseState = HttpRangeSpecifier.ParseState.RangeStart;
							rangeStart = -1L;
							strSegment.Reset();
						}
					}
					break;
				case HttpRangeSpecifier.ParseState.RangeStart:
					if (c != ' ' && c != '\t')
					{
						if (strSegment.Start == -1)
						{
							strSegment.Start = i;
						}
						if (c == '-' || c == ',')
						{
							strSegment.SetLengthFromTerminatingIndex(i);
							strSegment.Trim();
							if (c != '-')
							{
								parseFailureReason = "Invalid range, missing '-' character at " + (strSegment.Start + strSegment.Length);
								return false;
							}
							if (strSegment.Length > 0 && !long.TryParse(strSegment.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out rangeStart))
							{
								parseFailureReason = "Could not parse first-byte-pos at " + strSegment.Start;
								return false;
							}
							parseState = HttpRangeSpecifier.ParseState.RangeEnd;
							strSegment.Reset();
						}
					}
					break;
				case HttpRangeSpecifier.ParseState.RangeEnd:
					if (c != ' ' && c != '\t')
					{
						if (strSegment.Start == -1)
						{
							strSegment.Start = i;
						}
						if (c == ',')
						{
							strSegment.SetLengthFromTerminatingIndex(i);
							strSegment.Trim();
							if (!HttpRangeSpecifier.ProcessRangeEnd(specifier, ref parseFailureReason, strSegment, rangeStart))
							{
								return false;
							}
							rangeStart = -1L;
							parseState = HttpRangeSpecifier.ParseState.RangeStart;
							strSegment.Reset();
						}
					}
					break;
				}
				i++;
			}
			if (strSegment.Start != -1)
			{
				strSegment.SetLengthFromTerminatingIndex(i);
				strSegment.Trim();
				if (parseState == HttpRangeSpecifier.ParseState.Start)
				{
					specifier.RangeUnitSpecifier = strSegment.ToString();
				}
				if (parseState == HttpRangeSpecifier.ParseState.RangeStart)
				{
					parseFailureReason = "Invalid range, missing '-' character at " + (strSegment.Start + strSegment.Length);
					return false;
				}
			}
			else
			{
				if (parseState == HttpRangeSpecifier.ParseState.Start)
				{
					parseFailureReason = "Did not find range unit specifier";
					return false;
				}
				if (parseState == HttpRangeSpecifier.ParseState.RangeStart)
				{
					parseFailureReason = "Expected range value at the end.";
					return false;
				}
			}
			if (parseState == HttpRangeSpecifier.ParseState.RangeEnd && !HttpRangeSpecifier.ProcessRangeEnd(specifier, ref parseFailureReason, strSegment, rangeStart))
			{
				return false;
			}
			if (specifier.RangeCollection.Count == 0)
			{
				parseFailureReason = "No ranges found.";
				return false;
			}
			return true;
		}

		private static bool ProcessRangeEnd(HttpRangeSpecifier specifier, ref string parseFailureReason, HttpRangeSpecifier.StrSegment currentSegment, long rangeStart)
		{
			long rangeEnd = -1L;
			if (currentSegment.Start >= 0 && currentSegment.Length > 0 && !long.TryParse(currentSegment.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out rangeEnd))
			{
				parseFailureReason = "Could not parse last-byte-pos at " + currentSegment.Start;
				return false;
			}
			if (!HttpRangeSpecifier.AddRange(specifier, rangeStart, rangeEnd))
			{
				parseFailureReason = "Invalid range specification near " + currentSegment.Start;
				return false;
			}
			return true;
		}

		private static bool AddRange(HttpRangeSpecifier specifier, long rangeStart, long rangeEnd)
		{
			try
			{
				specifier.RangeCollection.Add(new HttpRange(rangeStart, rangeEnd));
			}
			catch (ArgumentOutOfRangeException)
			{
				return false;
			}
			return true;
		}

		private readonly Collection<HttpRange> rangeCollection = new Collection<HttpRange>();

		private enum ParseState
		{
			Start,
			RangeStart,
			RangeEnd
		}

		private class StrSegment
		{
			public StrSegment(string source)
			{
				if (source == null)
				{
					throw new ArgumentNullException("source");
				}
				this.source = source;
				this.Reset();
			}

			public int Start { get; set; }

			public int Length { get; set; }

			public void SetLengthFromTerminatingIndex(int terminatingIndex)
			{
				this.Length = terminatingIndex - this.Start;
			}

			public void Trim()
			{
				if (this.Start + this.Length > this.source.Length)
				{
					throw new InvalidOperationException("Source too short.");
				}
				while (this.Length > 0 && this.Start < this.source.Length)
				{
					if (!char.IsWhiteSpace(this.source[this.Start]))
					{
						break;
					}
					this.Start++;
					this.Length--;
				}
				while (this.Length > 0 && char.IsWhiteSpace(this.source[this.Start + this.Length - 1]))
				{
					this.Length--;
				}
			}

			public void Reset()
			{
				this.Start = -1;
				this.Length = 0;
			}

			public override string ToString()
			{
				return this.source.Substring(this.Start, this.Length);
			}

			private readonly string source;
		}
	}
}
