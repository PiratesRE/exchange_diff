using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal struct Bookmark
	{
		internal Bookmark(string text, PartType partType, int partNumber, CodingScheme codingScheme, int beginLoc, int endLoc)
		{
			this = new Bookmark(text, partType, partNumber, codingScheme, beginLoc, endLoc, false, false);
		}

		internal Bookmark(string text, PartType partType, int partNumber, CodingScheme codingScheme, int beginLoc, int endLoc, bool incompBegin, bool incompEnd)
		{
			if (string.IsNullOrEmpty(text) && (-1 != beginLoc || -1 != endLoc))
			{
				throw new ArgumentNullException("text");
			}
			if (0 > partNumber)
			{
				throw new ArgumentOutOfRangeException("partNumbe");
			}
			if (-1 != beginLoc && (0 > beginLoc || text.Length <= beginLoc))
			{
				throw new ArgumentOutOfRangeException("beginLoc");
			}
			if (-1 != endLoc && (0 > endLoc || text.Length <= endLoc))
			{
				throw new ArgumentOutOfRangeException("endLoc");
			}
			if (beginLoc > endLoc && -1 != endLoc)
			{
				throw new ArgumentException("endLoc");
			}
			this.text = text;
			this.partType = partType;
			this.partNumber = partNumber;
			this.codingScheme = codingScheme;
			this.beginLocation = beginLoc;
			this.endLocation = endLoc;
			this.incompleteBegin = incompBegin;
			this.incompleteEnd = incompEnd;
			this.literal = null;
		}

		public string FullText
		{
			get
			{
				return this.text;
			}
		}

		public PartType PartType
		{
			get
			{
				return this.partType;
			}
		}

		public int PartNumber
		{
			get
			{
				return this.partNumber;
			}
		}

		public CodingScheme CodingScheme
		{
			get
			{
				return this.codingScheme;
			}
		}

		public int BeginLocation
		{
			get
			{
				return this.beginLocation;
			}
		}

		public int EndLocation
		{
			get
			{
				return this.endLocation;
			}
		}

		public bool IncompleteBegin
		{
			get
			{
				return this.incompleteBegin;
			}
		}

		public bool IncompleteEnd
		{
			get
			{
				return this.incompleteEnd;
			}
		}

		public int CharacterCount
		{
			get
			{
				if (-1 == this.BeginLocation || -1 == this.EndLocation)
				{
					return 0;
				}
				return (this.IncompleteEnd ? (this.EndLocation - 1) : this.EndLocation) - this.BeginLocation + 1;
			}
		}

		public override string ToString()
		{
			if (-1 == this.BeginLocation || -1 == this.EndLocation)
			{
				return string.Empty;
			}
			string result;
			if ((result = this.literal) == null)
			{
				result = (this.literal = this.text.Substring(this.BeginLocation, this.CharacterCount));
			}
			return result;
		}

		public const int InvalidLocation = -1;

		public static readonly Bookmark Empty = new Bookmark(null, PartType.Short, 0, CodingScheme.Neutral, -1, -1, false, false);

		private string text;

		private PartType partType;

		private int partNumber;

		private CodingScheme codingScheme;

		private int beginLocation;

		private int endLocation;

		private bool incompleteBegin;

		private bool incompleteEnd;

		private string literal;
	}
}
