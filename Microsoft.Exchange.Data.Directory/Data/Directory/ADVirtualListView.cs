using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Text;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	internal sealed class ADVirtualListView : ADGenericReader, ITableView, IPagedView
	{
		internal ADVirtualListView(IDirectorySession session, ADObjectId rootId, ADObjectId[] addressListIds, SortBy sortBy, int rowsToPrefetch, IEnumerable<PropertyDefinition> properties) : base(session, rootId, QueryScope.SubTree, sortBy)
		{
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			if (sortBy == null)
			{
				throw new ArgumentNullException("sortBy");
			}
			if (rowsToPrefetch < 1)
			{
				throw new ArgumentOutOfRangeException("rowsToPrefetch");
			}
			this.rowsToPrefetch = rowsToPrefetch;
			QueryFilter[] array;
			if (addressListIds == null)
			{
				array = new QueryFilter[2];
				array[0] = new ExistsFilter(ADRecipientSchema.AddressListMembership);
			}
			else
			{
				array = new QueryFilter[addressListIds.Length + 1];
				for (int i = 0; i < addressListIds.Length; i++)
				{
					array[i] = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.AddressListMembership, addressListIds[i]);
				}
			}
			array[array.Length - 1] = new ExistsFilter(ADRecipientSchema.DisplayName);
			QueryFilter queryFilter = new AndFilter(array);
			QueryFilter queryFilter2 = (addressListIds == null) ? queryFilter : new ExistsFilter(ADObjectSchema.ObjectClass);
			this.requestedProperties = properties;
			ADScope readScope = session.GetReadScope(rootId, ADVirtualListView.dummyADRawEntry);
			ADObject adobject;
			string[] ldapAttributes;
			session.GetSchemaAndApplyFilter(ADVirtualListView.dummyADRawEntry, readScope, out adobject, out ldapAttributes, ref queryFilter2, ref properties);
			base.LdapAttributes = ldapAttributes;
			this.properties = properties;
			base.LdapFilter = LdapFilterBuilder.LdapFilterFromQueryFilter((addressListIds == null) ? queryFilter2 : queryFilter, false, base.Session.SessionSettings.PartitionSoftLinkMode, base.Session.SessionSettings.IsTenantScoped);
			this.vlvRequestControl = new VlvRequestControl();
			this.vlvRequestControl.AfterCount = this.rowsToPrefetch;
			base.DirectoryControls.Add(this.vlvRequestControl);
		}

		private ADRawEntry[] GetNextResultCollection()
		{
			this.vlvRequestControl.ContextId = base.Cookie;
			DirectoryControl directoryControl;
			SearchResultEntryCollection nextResultCollection = base.GetNextResultCollection(typeof(VlvResponseControl), out directoryControl);
			ADProviderPerf.UpdateProcessCounter(Counter.ProcessRateVlv, UpdateType.Add, 1U);
			ADProviderPerf.UpdateDCCounter(base.PreferredServerName, Counter.DCRateVlv, UpdateType.Add, 1U);
			base.Cookie = null;
			if (directoryControl != null)
			{
				VlvResponseControl vlvResponseControl = (VlvResponseControl)directoryControl;
				base.Cookie = vlvResponseControl.ContextId;
				this.estimatedRowCount = vlvResponseControl.ContentCount;
				this.currentRow = vlvResponseControl.TargetPosition;
			}
			if (nextResultCollection == null)
			{
				return null;
			}
			return base.Session.ObjectsFromEntries<ADRawEntry>(nextResultCollection, base.PreferredServerName, this.properties, ADVirtualListView.dummyADRawEntry);
		}

		public int EstimatedRowCount
		{
			get
			{
				return this.estimatedRowCount;
			}
		}

		public int CurrentRow
		{
			get
			{
				return this.currentRow;
			}
		}

		public bool SeekToCondition(SeekReference seekReference, QueryFilter seekFilter)
		{
			if (seekReference != SeekReference.OriginBeginning)
			{
				throw new ArgumentException(DirectoryStrings.ExceptionInvalidVlvSeekReference(seekReference.ToString()));
			}
			if (seekFilter == null)
			{
				throw new ArgumentNullException("seekFilter");
			}
			TextFilter textFilter = seekFilter as TextFilter;
			if (textFilter == null)
			{
				throw new ArgumentException(DirectoryStrings.ExceptionInvalidVlvFilter(seekFilter.GetType().Name));
			}
			if (textFilter.MatchOptions != MatchOptions.Prefix)
			{
				throw new ArgumentException(DirectoryStrings.ExceptionInvalidVlvFilterOption(textFilter.MatchOptions.ToString()));
			}
			if (textFilter.Property != ADRecipientSchema.DisplayName)
			{
				throw new ArgumentException(DirectoryStrings.ExceptionInvalidVlvFilterProperty(textFilter.Property.Name));
			}
			this.vlvRequestControl.Target = Encoding.UTF8.GetBytes(textFilter.Text);
			this.results = this.GetNextResultCollection();
			return this.results != null;
		}

		public int SeekToOffset(SeekReference seekReference, int offset)
		{
			int num;
			switch (seekReference)
			{
			case SeekReference.OriginBeginning:
				num = offset;
				goto IL_72;
			case SeekReference.OriginCurrent:
				num = this.currentRow + offset;
				goto IL_72;
			case SeekReference.BackwardFromCurrent:
				num = this.currentRow - offset;
				goto IL_72;
			case SeekReference.BackwardFromEnd:
				num = this.estimatedRowCount - offset;
				goto IL_72;
			}
			throw new ArgumentException(DirectoryStrings.ExArgumentException("seekReference", seekReference.ToString()), "seekReference");
			IL_72:
			if (num < 0)
			{
				num = 1;
			}
			else if (this.estimatedRowCount != 0 && num > this.estimatedRowCount * 2)
			{
				num = this.estimatedRowCount * 2;
			}
			this.vlvRequestControl.Offset = num;
			this.vlvRequestControl.Target = null;
			this.results = this.GetNextResultCollection();
			return this.currentRow;
		}

		public object[][] GetRows(int rowCount)
		{
			ADRawEntry[] propertyBags = this.GetPropertyBags(rowCount);
			return ADSession.ConvertToView(propertyBags, this.requestedProperties);
		}

		public ADRawEntry[] GetPropertyBags(int rowCount)
		{
			if (rowCount < 0)
			{
				throw new ArgumentOutOfRangeException("rowCount");
			}
			if (rowCount == 0)
			{
				return Array<ADRawEntry>.Empty;
			}
			if (this.rowsToPrefetch < rowCount)
			{
				this.vlvRequestControl.AfterCount = rowCount;
				try
				{
					this.results = this.GetNextResultCollection();
				}
				finally
				{
					this.vlvRequestControl.AfterCount = this.rowsToPrefetch;
				}
			}
			if (this.results == null)
			{
				return Array<ADRawEntry>.Empty;
			}
			int num = Math.Min(rowCount, this.results.Length);
			ADRawEntry[] array = new ADRawEntry[num];
			Array.Copy(this.results, array, num);
			return array;
		}

		private static ADRawEntry dummyADRawEntry = new ADRawEntry();

		private int estimatedRowCount;

		private int currentRow;

		private int rowsToPrefetch;

		private VlvRequestControl vlvRequestControl;

		private IEnumerable<PropertyDefinition> requestedProperties;

		private IEnumerable<PropertyDefinition> properties;

		private ADRawEntry[] results;
	}
}
