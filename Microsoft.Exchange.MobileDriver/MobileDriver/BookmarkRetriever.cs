using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class BookmarkRetriever
	{
		internal BookmarkRetriever(PartType partType, IList<Bookmark> segments)
		{
			if (segments == null)
			{
				throw new ArgumentNullException("segments");
			}
			this.PartType = partType;
			if (!segments.IsReadOnly)
			{
				segments = new ReadOnlyCollection<Bookmark>(segments);
			}
			this.Segments = segments;
		}

		public PartType PartType { get; private set; }

		public IList<Bookmark> Segments { get; private set; }

		public IList<Bookmark> Parts
		{
			get
			{
				if (this.parts == null)
				{
					this.BuildParts();
				}
				return this.parts;
			}
		}

		public IList<Bookmark> GetPartSegments(int partNumber)
		{
			if (this.Segments[this.Segments.Count - 1].PartNumber >= partNumber)
			{
				throw new ArgumentOutOfRangeException("partNumber");
			}
			List<Bookmark> list = new List<Bookmark>();
			int num = 0;
			while (this.Segments.Count > num)
			{
				if (this.Segments[num].PartNumber == partNumber)
				{
					list.Add(this.Segments[num]);
				}
				num++;
			}
			return list.AsReadOnly();
		}

		private void BuildParts()
		{
			if (this.PartType == PartType.Short || this.Segments.Count == 0)
			{
				this.parts = this.Segments;
				return;
			}
			if (1 == this.Segments.Count && this.Segments[this.Segments.Count - 1].CharacterCount == 0)
			{
				this.parts = this.Segments;
				return;
			}
			List<Bookmark> list = new List<Bookmark>();
			if (0 < this.Segments.Count)
			{
				if (this.parts != null && 0 < this.parts.Count && this.Segments[this.Segments.Count - 1].EndLocation == this.parts[this.parts.Count - 1].EndLocation)
				{
					return;
				}
				list.Capacity = this.Segments[this.Segments.Count - 1].PartNumber + 1;
				int num = -1;
				int num2 = 0;
				while (this.Segments.Count > num2)
				{
					if (this.Segments[num2].PartNumber != num)
					{
						if (0 < list.Count)
						{
							list[list.Count - 1] = new Bookmark(list[list.Count - 1].FullText, list[list.Count - 1].PartType, list[list.Count - 1].PartNumber, list[list.Count - 1].CodingScheme, list[list.Count - 1].BeginLocation, this.Segments[num2 - 1].EndLocation);
						}
						list.Add(new Bookmark(this.Segments[num2].FullText, this.Segments[num2].PartType, this.Segments[num2].PartNumber, this.Segments[num2].CodingScheme, this.Segments[num2].BeginLocation, -1));
						num = this.Segments[num2].PartNumber;
					}
					num2++;
				}
				if (0 < list.Count && -1 == list[list.Count - 1].EndLocation)
				{
					list[list.Count - 1] = new Bookmark(list[list.Count - 1].FullText, list[list.Count - 1].PartType, list[list.Count - 1].PartNumber, list[list.Count - 1].CodingScheme, list[list.Count - 1].BeginLocation, this.Segments[num2 - 1].EndLocation);
				}
			}
			this.parts = list.AsReadOnly();
		}

		private IList<Bookmark> parts;
	}
}
