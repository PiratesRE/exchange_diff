using System;
using System.ComponentModel;
using System.Data;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class MonadDataTableLoader : DataTableLoader
	{
		public MonadDataTableLoader(DataTable dataTable, string noun)
		{
			if (dataTable == null)
			{
				throw new ArgumentNullException("dataTable");
			}
			this.defaultTable = dataTable;
			base.Table = this.defaultTable;
			base.Table.TableName = noun;
			this.selectCommand = new LoggableMonadCommand();
			base.RefreshArgument = this.selectCommand;
			this.Noun = noun;
		}

		internal MonadParameterCollection Parameters
		{
			get
			{
				return this.selectCommand.Parameters;
			}
		}

		public MonadDataTableLoader(string noun) : this(new DataTable(), noun)
		{
		}

		public MonadDataTableLoader(Type type) : this(new DataTable(), type.Name.ToLowerInvariant())
		{
		}

		public MonadDataTableLoader() : this("")
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.defaultTable.Dispose();
				this.selectCommand.Dispose();
			}
			base.Dispose(disposing);
		}

		protected sealed override DataTable DefaultTable
		{
			get
			{
				return this.defaultTable;
			}
		}

		protected sealed override ICloneable DefaultRefreshArgument
		{
			get
			{
				return this.selectCommand;
			}
		}

		[DefaultValue("")]
		public virtual string Noun
		{
			get
			{
				return this.noun;
			}
			set
			{
				if (value == null)
				{
					value = "";
				}
				this.noun = value;
				this.selectCommand.CommandText = "get-" + value;
				this.toString = "MonadDataTableLoader(" + this.selectCommand.CommandText + ")";
			}
		}

		public override string ToString()
		{
			return this.toString;
		}

		protected override bool TryToGetPartialRefreshArgument(object[] ids, out object partialRefreshArgument)
		{
			if (1 != ids.Length)
			{
				throw new InvalidOperationException();
			}
			MonadCommand monadCommand = this.selectCommand.Clone();
			monadCommand.Parameters.AddWithValue("Identity", ids[0]);
			partialRefreshArgument = monadCommand;
			return true;
		}

		private DataTable defaultTable;

		private MonadCommand selectCommand;

		private string noun = "";

		private string toString;
	}
}
