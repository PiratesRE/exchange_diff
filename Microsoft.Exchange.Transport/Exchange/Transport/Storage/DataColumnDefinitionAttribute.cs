using System;

namespace Microsoft.Exchange.Transport.Storage
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	internal sealed class DataColumnDefinitionAttribute : Attribute
	{
		public DataColumnDefinitionAttribute(Type columnType, ColumnAccess columnAccess)
		{
			this.columnType = columnType;
			this.accessMode = columnAccess;
		}

		public Type ColumnType
		{
			get
			{
				return this.columnType;
			}
		}

		public ColumnAccess ColumnAccess
		{
			get
			{
				return this.accessMode;
			}
		}

		public bool PrimaryKey
		{
			get
			{
				return this.primaryKey;
			}
			set
			{
				this.primaryKey = value;
			}
		}

		public bool Required
		{
			get
			{
				return this.required;
			}
			set
			{
				this.required = value;
			}
		}

		public bool AutoIncrement
		{
			get
			{
				return this.autoIncrement;
			}
			set
			{
				this.autoIncrement = value;
			}
		}

		public bool AutoVersioned
		{
			get
			{
				return this.autoVersioned;
			}
			set
			{
				this.autoVersioned = value;
			}
		}

		public bool IntrinsicLV
		{
			get
			{
				return this.intrinsicLV;
			}
			set
			{
				this.intrinsicLV = value;
			}
		}

		public bool MultiValued
		{
			get
			{
				return this.multiValued;
			}
			set
			{
				this.multiValued = value;
			}
		}

		private readonly Type columnType;

		private readonly ColumnAccess accessMode;

		private bool primaryKey;

		private bool required;

		private bool autoIncrement;

		private bool autoVersioned;

		private bool intrinsicLV;

		private bool multiValued;
	}
}
