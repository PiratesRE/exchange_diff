using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class QueryResultPropertyBag : PropertyBag
	{
		internal static bool CanBinaryValueBeTruncated(byte[] value)
		{
			return value != null && value.Length == 255;
		}

		internal static bool CanStringValueBeTruncated(string value)
		{
			return value != null && value.Length == 255;
		}

		internal static bool CanValueBeTruncated(object value)
		{
			return QueryResultPropertyBag.CanBinaryValueBeTruncated(value as byte[]) || QueryResultPropertyBag.CanStringValueBeTruncated(value as string);
		}

		public QueryResultPropertyBag(StoreSession session, ICollection<PropertyDefinition> columns)
		{
			this.Context.Session = session;
			if (session != null)
			{
				this.ExTimeZone = session.ExTimeZone;
			}
			this.currentRowValues = null;
			this.returnErrorsOnTruncatedProperties = false;
			this.propertyPositions = QueryResultPropertyBag.CreatePropertyPositionsDictionary(columns);
		}

		internal QueryResultPropertyBag(QueryResultPropertyBag queryPropertyBagHeaderInfo) : base(queryPropertyBagHeaderInfo)
		{
			this.timeZone = queryPropertyBagHeaderInfo.timeZone;
			this.currentRowValues = queryPropertyBagHeaderInfo.currentRowValues;
			this.propertyPositions = queryPropertyBagHeaderInfo.propertyPositions;
		}

		internal QueryResultPropertyBag(IStorePropertyBag storePropertyBag, ICollection<PropertyDefinition> propertyDefinitions, IList<object> propertyValues)
		{
			QueryResultPropertyBag queryResultPropertyBag = (QueryResultPropertyBag)((PropertyBag.StorePropertyBagAdaptor)storePropertyBag).PropertyBag;
			this.Context.Session = queryResultPropertyBag.Context.Session;
			this.timeZone = queryResultPropertyBag.timeZone;
			List<object> list = new List<object>(queryResultPropertyBag.currentRowValues);
			this.propertyPositions = new Dictionary<StorePropertyDefinition, int>(queryResultPropertyBag.propertyPositions);
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
			{
				StorePropertyDefinition key = propertyDefinition as StorePropertyDefinition;
				int index;
				if (!this.propertyPositions.TryGetValue(key, out index))
				{
					this.propertyPositions.Add(key, list.Count);
					list.Add(propertyValues[num]);
				}
				else
				{
					list[index] = propertyValues[num];
				}
				num++;
			}
			this.currentRowValues = list.ToArray();
		}

		internal QueryResultPropertyBag(StoreSession session, Dictionary<StorePropertyDefinition, int> propertyPositionsDictionary)
		{
			this.Context.Session = session;
			if (session != null)
			{
				this.ExTimeZone = session.ExTimeZone;
			}
			this.currentRowValues = null;
			this.returnErrorsOnTruncatedProperties = false;
			this.propertyPositions = propertyPositionsDictionary;
		}

		internal static Dictionary<StorePropertyDefinition, int> CreatePropertyPositionsDictionary(ICollection<PropertyDefinition> columns)
		{
			Dictionary<StorePropertyDefinition, int> dictionary = new Dictionary<StorePropertyDefinition, int>(columns.Count);
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in columns)
			{
				StorePropertyDefinition propertyDefinition2 = (StorePropertyDefinition)propertyDefinition;
				dictionary.Add(InternalSchema.ToStorePropertyDefinition(propertyDefinition2), num++);
			}
			return dictionary;
		}

		internal void SetQueryResultRow(PropValue[] row)
		{
			if (row.Length != this.propertyPositions.Count)
			{
				throw new ArgumentException("An array of values is different in size from an array of columns");
			}
			this.currentRowValues = new object[row.Length];
			foreach (KeyValuePair<StorePropertyDefinition, int> keyValuePair in this.propertyPositions)
			{
				StorePropertyDefinition key = keyValuePair.Key;
				int value = keyValuePair.Value;
				this.currentRowValues[value] = MapiPropertyBag.GetValueFromPropValue(this.Context.Session, this.timeZone, key, row[value]);
			}
		}

		public void SetQueryResultRow(object[] row)
		{
			if (row.Length != this.propertyPositions.Count)
			{
				throw new ArgumentException("An array of values is different in size from an array of columns");
			}
			this.currentRowValues = row;
		}

		public override bool IsDirty
		{
			get
			{
				return false;
			}
		}

		protected override bool IsLoaded(NativeStorePropertyDefinition propertyDefinition)
		{
			return this.propertyPositions.ContainsKey(propertyDefinition);
		}

		protected override bool InternalIsPropertyDirty(AtomicStorePropertyDefinition propertyDefinition)
		{
			return false;
		}

		public override void Load(ICollection<PropertyDefinition> propertyDefinitions)
		{
			throw new NotSupportedException();
		}

		protected override void SetValidatedStoreProperty(StorePropertyDefinition propertyDefinition, object propertyValue)
		{
			throw new NotSupportedException(ServerStrings.MapiCannotSetProps);
		}

		protected override object TryGetStoreProperty(StorePropertyDefinition propertyDefinition)
		{
			int num = 0;
			if (this.propertyPositions.TryGetValue(propertyDefinition, out num))
			{
				object obj = this.currentRowValues[num];
				if (this.returnErrorsOnTruncatedProperties && QueryResultPropertyBag.CanValueBeTruncated(obj))
				{
					obj = new PropertyError(propertyDefinition, PropertyErrorCode.PropertyValueTruncated);
				}
				return obj;
			}
			throw new NotInBagPropertyErrorException(propertyDefinition);
		}

		internal object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitions)
		{
			object[] array = new object[propertyDefinitions.Count];
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
			{
				array[num++] = base.TryGetProperty(propertyDefinition);
			}
			return array;
		}

		protected override void DeleteStoreProperty(StorePropertyDefinition propertyDefinition)
		{
			throw new NotSupportedException(ServerStrings.MapiCannotDeleteProperties);
		}

		internal override ExTimeZone ExTimeZone
		{
			get
			{
				return this.timeZone;
			}
			set
			{
				this.timeZone = value;
			}
		}

		internal bool ReturnErrorsOnTruncatedProperties
		{
			get
			{
				return this.returnErrorsOnTruncatedProperties;
			}
			set
			{
				this.returnErrorsOnTruncatedProperties = value;
			}
		}

		private const int MaxStringLengthFromTable = 255;

		internal const int MaxBinaryLengthFromTable = 255;

		private object[] currentRowValues;

		private readonly Dictionary<StorePropertyDefinition, int> propertyPositions;

		private ExTimeZone timeZone = ExTimeZone.UtcTimeZone;

		private bool returnErrorsOnTruncatedProperties;
	}
}
