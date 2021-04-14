using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class PagedPositionInfo : IConfigurable
	{
		public PagedPositionInfo(int offset, int totalCount)
		{
			if (0 > offset)
			{
				throw new ArgumentException("Offset cannot be less than 0", "offset");
			}
			if (0 > totalCount)
			{
				throw new ArgumentException("Total count cannot be less than 0", "totalCount");
			}
			this.pageOffset = offset;
			this.totalCount = totalCount;
		}

		public int PageOffset
		{
			get
			{
				return this.pageOffset;
			}
		}

		public int TotalCount
		{
			get
			{
				return this.totalCount;
			}
		}

		ObjectId IConfigurable.Identity
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		ValidationError[] IConfigurable.Validate()
		{
			throw new NotImplementedException();
		}

		bool IConfigurable.IsValid
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		void IConfigurable.CopyChangesFrom(IConfigurable source)
		{
			throw new NotImplementedException();
		}

		void IConfigurable.ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		private int pageOffset;

		private int totalCount;
	}
}
