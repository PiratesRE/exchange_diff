using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.Search;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(FindFolderParentWrapper))]
	[KnownType(typeof(FindItemParentWrapper))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public abstract class FindParentWrapperBase
	{
		public FindParentWrapperBase()
		{
		}

		internal FindParentWrapperBase(BasePageResult pageResult)
		{
			IndexedPageResult indexedPageResult = pageResult as IndexedPageResult;
			if (indexedPageResult != null)
			{
				this.Initialize(indexedPageResult.IndexedOffset, indexedPageResult.View.TotalItems, indexedPageResult.View.RetrievedLastItem);
				return;
			}
			FractionalPageResult fractionalPageResult = pageResult as FractionalPageResult;
			if (fractionalPageResult != null)
			{
				this.Initialize(fractionalPageResult.NumeratorOffset, fractionalPageResult.AbsoluteDenominator, fractionalPageResult.View.TotalItems, fractionalPageResult.View.RetrievedLastItem);
				return;
			}
			this.Initialize(pageResult.View.TotalItems, pageResult.View.RetrievedLastItem);
		}

		private void Initialize(int totalItemsInView, bool includesLastItemInRange)
		{
			this.TotalItemsInView = totalItemsInView;
			this.IncludesLastItemInRange = includesLastItemInRange;
		}

		private void Initialize(int indexedPagingOffset, int totalItemsInView, bool includesLastItemInRange)
		{
			this.IndexedPagingOffset = indexedPagingOffset;
			this.TotalItemsInView = totalItemsInView;
			this.IncludesLastItemInRange = includesLastItemInRange;
		}

		private void Initialize(int numeratorOffset, int absoluteDenominator, int totalItemsInView, bool includesLastItemInRange)
		{
			this.NumeratorOffset = numeratorOffset;
			this.AbsoluteDenominator = absoluteDenominator;
			this.TotalItemsInView = totalItemsInView;
			this.IncludesLastItemInRange = includesLastItemInRange;
		}

		[IgnoreDataMember]
		[XmlAttribute]
		public int IndexedPagingOffset
		{
			get
			{
				return this.indexedPagingOffset;
			}
			set
			{
				this.IndexedPagingOffsetSpecified = true;
				this.indexedPagingOffset = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "IndexedPagingOffset", EmitDefaultValue = false)]
		public int? IndexedPagingOffsetNullable
		{
			get
			{
				if (this.IndexedPagingOffsetSpecified)
				{
					return new int?(this.IndexedPagingOffset);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.IndexedPagingOffset = value.Value;
					return;
				}
				this.IndexedPagingOffsetSpecified = false;
			}
		}

		[IgnoreDataMember]
		[XmlAttribute]
		public int NumeratorOffset
		{
			get
			{
				return this.numeratorOffset;
			}
			set
			{
				this.NumeratorOffsetSpecified = true;
				this.numeratorOffset = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "NumeratorOffset", EmitDefaultValue = false)]
		public int? NumeratorOffsetNullable
		{
			get
			{
				if (this.NumeratorOffsetSpecified)
				{
					return new int?(this.NumeratorOffset);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.NumeratorOffset = value.Value;
					return;
				}
				this.NumeratorOffsetSpecified = false;
			}
		}

		[XmlAttribute]
		[IgnoreDataMember]
		public int AbsoluteDenominator
		{
			get
			{
				return this.absoluteDenominator;
			}
			set
			{
				this.AbsoluteDenominatorSpecified = true;
				this.absoluteDenominator = value;
			}
		}

		[DataMember(Name = "AbsoluteDenominator", EmitDefaultValue = false)]
		[XmlIgnore]
		public int? AbsoluteDenominatorNullable
		{
			get
			{
				if (this.AbsoluteDenominatorSpecified)
				{
					return new int?(this.AbsoluteDenominator);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.AbsoluteDenominator = value.Value;
					return;
				}
				this.AbsoluteDenominatorSpecified = false;
			}
		}

		[XmlAttribute]
		[IgnoreDataMember]
		public int TotalItemsInView
		{
			get
			{
				return this.totalItemsInView;
			}
			set
			{
				this.TotalItemsInViewSpecified = true;
				this.totalItemsInView = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "TotalItemsInView", EmitDefaultValue = false)]
		public int? TotalItemsInViewNullable
		{
			get
			{
				if (this.TotalItemsInViewSpecified)
				{
					return new int?(this.TotalItemsInView);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.TotalItemsInView = value.Value;
					return;
				}
				this.TotalItemsInViewSpecified = false;
			}
		}

		[XmlAttribute]
		[IgnoreDataMember]
		public bool IncludesLastItemInRange
		{
			get
			{
				return this.includesLastItemInRange;
			}
			set
			{
				this.IncludesLastItemInRangeSpecified = true;
				this.includesLastItemInRange = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "IncludesLastItemInRange", EmitDefaultValue = false)]
		public bool? IncludesLastItemInRangeNullable
		{
			get
			{
				if (this.IncludesLastItemInRangeSpecified)
				{
					return new bool?(this.IncludesLastItemInRange);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.IncludesLastItemInRangeSpecified = value.Value;
					return;
				}
				this.IncludesLastItemInRangeSpecified = false;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool TotalItemsInViewSpecified { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IndexedPagingOffsetSpecified { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public bool NumeratorOffsetSpecified { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public bool AbsoluteDenominatorSpecified { get; set; }

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IncludesLastItemInRangeSpecified { get; set; }

		protected int indexedPagingOffset;

		protected int numeratorOffset;

		protected int absoluteDenominator;

		protected bool includesLastItemInRange;

		protected int totalItemsInView;
	}
}
