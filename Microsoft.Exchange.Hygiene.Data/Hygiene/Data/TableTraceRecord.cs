using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class TableTraceRecord : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(string.Format("T:{0}-S:{1}-D:{2}", this.TableName, this.SourceDatabase, this.DestinationDatabase));
			}
		}

		public string TableName
		{
			get
			{
				return this[TableTraceRecord.TableNameProp] as string;
			}
			set
			{
				this[TableTraceRecord.TableNameProp] = value;
			}
		}

		public string SourceDatabase
		{
			get
			{
				return this[TableTraceRecord.SourceDatabaseProp] as string;
			}
			set
			{
				this[TableTraceRecord.SourceDatabaseProp] = value;
			}
		}

		public string DestinationDatabase
		{
			get
			{
				return this[TableTraceRecord.DestinationDatabaseProp] as string;
			}
			set
			{
				this[TableTraceRecord.DestinationDatabaseProp] = value;
			}
		}

		public DateTime SourceDatetime
		{
			get
			{
				return (DateTime)this[TableTraceRecord.SourceDatetimeProp];
			}
			set
			{
				this[TableTraceRecord.SourceDatetimeProp] = value;
			}
		}

		public DateTime DestinationDatetime
		{
			get
			{
				return (DateTime)this[TableTraceRecord.DestinationDatetimeProp];
			}
			set
			{
				this[TableTraceRecord.DestinationDatetimeProp] = value;
			}
		}

		public static readonly HygienePropertyDefinition TableNameProp = new HygienePropertyDefinition("TableName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition SourceDatabaseProp = new HygienePropertyDefinition("SourceDatabase", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition DestinationDatabaseProp = new HygienePropertyDefinition("DestinationDatabase", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition SourceDatetimeProp = new HygienePropertyDefinition("SourceDatetime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition DestinationDatetimeProp = new HygienePropertyDefinition("DestinationDatetime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
