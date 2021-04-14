using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class MonadParameter : DbParameter, ICloneable
	{
		public MonadParameter()
		{
		}

		public MonadParameter(string parameterName) : this()
		{
			this.parameterName = parameterName;
		}

		public MonadParameter(string parameterName, object value) : this(parameterName)
		{
			this.value = value;
		}

		public MonadParameter(string parameterName, DbType dbType, string sourceColumn) : this(parameterName)
		{
			this.dbType = dbType;
			this.sourceColumn = sourceColumn;
		}

		public override void ResetDbType()
		{
			this.dbType = DbType.Object;
		}

		[DefaultValue(DbType.Object)]
		public override DbType DbType
		{
			get
			{
				return this.dbType;
			}
			set
			{
				this.dbType = value;
			}
		}

		public override string ParameterName
		{
			get
			{
				return this.parameterName;
			}
			set
			{
				this.parameterName = value;
			}
		}

		public bool IsSwitch
		{
			get
			{
				return this.isSwitch;
			}
			set
			{
				this.isSwitch = value;
			}
		}

		public override object Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (value is DBNull)
				{
					this.value = null;
					return;
				}
				this.value = value;
			}
		}

		public override ParameterDirection Direction
		{
			get
			{
				return ParameterDirection.Input;
			}
			set
			{
				if (ParameterDirection.Input != value)
				{
					throw new InvalidOperationException("ParameterDirection.Input is the only valid value.");
				}
			}
		}

		public override string SourceColumn
		{
			get
			{
				return this.sourceColumn;
			}
			set
			{
				this.sourceColumn = value;
			}
		}

		public override bool IsNullable
		{
			get
			{
				return this.isNullable;
			}
			set
			{
				this.isNullable = value;
			}
		}

		public override DataRowVersion SourceVersion
		{
			get
			{
				return this.sourceVersion;
			}
			set
			{
				this.sourceVersion = value;
			}
		}

		public override bool SourceColumnNullMapping
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public override int Size
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		private MonadParameter(MonadParameter source)
		{
			source.CopyTo(this);
			ICloneable cloneable = this.value as ICloneable;
			if (cloneable != null)
			{
				this.value = cloneable.Clone();
			}
		}

		private void CopyTo(MonadParameter destination)
		{
			destination.parameterName = this.parameterName;
			destination.value = this.value;
			destination.sourceColumn = this.sourceColumn;
			destination.sourceVersion = this.sourceVersion;
			destination.isNullable = this.isNullable;
			destination.dbType = this.dbType;
			destination.isSwitch = this.isSwitch;
		}

		public MonadParameter Clone()
		{
			return new MonadParameter(this);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		private string parameterName;

		private object value;

		private string sourceColumn;

		private DbType dbType = DbType.Object;

		private bool isNullable;

		private bool isSwitch;

		private DataRowVersion sourceVersion = DataRowVersion.Current;
	}
}
