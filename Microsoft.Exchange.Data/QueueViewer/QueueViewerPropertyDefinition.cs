using System;
using Microsoft.Exchange.Rpc.QueueViewer;

namespace Microsoft.Exchange.Data.QueueViewer
{
	[Serializable]
	internal sealed class QueueViewerPropertyDefinition<ObjectType> : ProviderPropertyDefinition where ObjectType : PagedDataObject
	{
		internal QueueViewerPropertyDefinition(string name, Type type, object defaultValue, bool isBasic, CompareFieldWithValueDelegate<ObjectType> comparer1, CompareFieldWithFieldDelegate<ObjectType> comparer2) : this(name, type, defaultValue, isBasic, comparer1, comparer2, ExchangeObjectVersion.Exchange2003)
		{
		}

		internal QueueViewerPropertyDefinition(string name, Type type, object defaultValue, bool isBasic, CompareFieldWithValueDelegate<ObjectType> comparer1, CompareFieldWithFieldDelegate<ObjectType> comparer2, ExchangeObjectVersion version) : this(name, type, defaultValue, isBasic, comparer1, comparer2, QueueViewerPropertyDefinition<ObjectType>.TextFilterNotSupportedMatcher, version)
		{
		}

		internal QueueViewerPropertyDefinition(string name, Type type, object defaultValue, bool isBasic, CompareFieldWithValueDelegate<ObjectType> comparer1, CompareFieldWithFieldDelegate<ObjectType> comparer2, MatchFieldWithTextDelegate<ObjectType> matcher) : this(name, type, defaultValue, isBasic, comparer1, comparer2, matcher, ExchangeObjectVersion.Exchange2003)
		{
		}

		internal QueueViewerPropertyDefinition(string name, Type type, object defaultValue, bool isBasic, CompareFieldWithValueDelegate<ObjectType> comparer1, CompareFieldWithFieldDelegate<ObjectType> comparer2, MatchFieldWithTextDelegate<ObjectType> matcher, ExchangeObjectVersion version) : base(name, version, type, defaultValue)
		{
			this.isBasic = isBasic;
			this.comparer1 = comparer1;
			this.comparer2 = comparer2;
			this.matcher = matcher;
		}

		public override bool IsMultivalued
		{
			get
			{
				return false;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override bool IsCalculated
		{
			get
			{
				return false;
			}
		}

		public override bool IsFilterOnly
		{
			get
			{
				return false;
			}
		}

		public override bool IsMandatory
		{
			get
			{
				return false;
			}
		}

		public override bool PersistDefaultValue
		{
			get
			{
				return true;
			}
		}

		public override bool IsWriteOnce
		{
			get
			{
				return false;
			}
		}

		public override bool IsBinary
		{
			get
			{
				return false;
			}
		}

		public bool isBasic;

		internal CompareFieldWithValueDelegate<ObjectType> comparer1;

		internal CompareFieldWithFieldDelegate<ObjectType> comparer2;

		internal MatchFieldWithTextDelegate<ObjectType> matcher;

		internal static MatchFieldWithTextDelegate<ObjectType> TextFilterNotSupportedMatcher = delegate(ObjectType dataObject, object matchPattern, MatchOptions matchOptions)
		{
			throw new QueueViewerException(QVErrorCode.QV_E_TEXT_MATCHING_NOT_SUPPORTED);
		};
	}
}
