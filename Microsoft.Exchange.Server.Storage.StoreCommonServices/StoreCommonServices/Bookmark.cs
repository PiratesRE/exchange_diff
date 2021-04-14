using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public struct Bookmark
	{
		private Bookmark(IList<object> keyValues, bool positionedOn, bool positionValid, int position)
		{
			this.keyValues = keyValues;
			this.positionedOn = positionedOn;
			this.positionValid = positionValid;
			this.position = position;
		}

		public Bookmark(IList<object> keyValues, bool positionedOn)
		{
			this = new Bookmark(keyValues, positionedOn, false, -1);
		}

		public Bookmark(IList<object> keyValues, bool positionedOn, int position)
		{
			this = new Bookmark(keyValues, positionedOn, true, position);
		}

		internal Bookmark(SortOrder sortOrder, Reader reader, bool positionedOn, int? position)
		{
			this.positionValid = (position != null);
			this.position = position.GetValueOrDefault(-1);
			this.positionedOn = positionedOn;
			object[] array = new object[sortOrder.Count];
			for (int i = 0; i < sortOrder.Count; i++)
			{
				array[i] = reader.GetValue(sortOrder[i].Column);
			}
			this.keyValues = array;
		}

		public IList<object> KeyValues
		{
			get
			{
				return this.keyValues;
			}
		}

		public bool PositionedOn
		{
			get
			{
				return this.positionedOn;
			}
		}

		public bool PositionValid
		{
			get
			{
				return this.positionValid;
			}
		}

		public int Position
		{
			get
			{
				return this.position;
			}
		}

		public bool IsBOT
		{
			get
			{
				return this.keyValues == null && this.position == 0;
			}
		}

		public bool IsEOT
		{
			get
			{
				return this.keyValues == null && this.position == int.MaxValue;
			}
		}

		public override string ToString()
		{
			if (this.IsBOT)
			{
				return "BOT";
			}
			if (this.IsEOT)
			{
				return "EOT";
			}
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append(this.positionedOn ? "On " : "After ");
			if (this.positionValid)
			{
				stringBuilder.Append("Numeric Position:[");
				stringBuilder.Append(this.position);
				stringBuilder.Append("]");
			}
			else
			{
				stringBuilder.Append("Key Position:[");
				for (int i = 0; i < this.keyValues.Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.AppendAsString(this.keyValues[i]);
				}
				stringBuilder.Append("]");
			}
			return stringBuilder.ToString();
		}

		private readonly int position;

		private readonly bool positionValid;

		private readonly bool positionedOn;

		private IList<object> keyValues;

		public static readonly Bookmark BOT = new Bookmark(null, true, true, 0);

		public static readonly Bookmark EOT = new Bookmark(null, false, false, int.MaxValue);
	}
}
