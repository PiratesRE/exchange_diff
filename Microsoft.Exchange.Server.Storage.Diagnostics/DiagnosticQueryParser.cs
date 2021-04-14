using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Diagnostics.Generated;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class DiagnosticQueryParser
	{
		private DiagnosticQueryParser(string query)
		{
			if (string.IsNullOrEmpty(query))
			{
				throw new DiagnosticQueryParserException(DiagnosticQueryStrings.QueryNull());
			}
			this.parser = new Parser(query);
		}

		public static string AllColumns
		{
			get
			{
				return "*";
			}
		}

		public DiagnosticQueryParser.QueryType Type
		{
			get
			{
				return this.parser.Type;
			}
		}

		public IList<DiagnosticQueryParser.Column> Select
		{
			get
			{
				return this.parser.Select;
			}
		}

		public DiagnosticQueryParser.Context From
		{
			get
			{
				return this.parser.From;
			}
		}

		public IDictionary<string, string> Set
		{
			get
			{
				return this.parser.Set;
			}
		}

		public DiagnosticQueryCriteria Where
		{
			get
			{
				return this.parser.Where;
			}
		}

		public IList<DiagnosticQueryParser.SortColumn> OrderBy
		{
			get
			{
				return this.parser.OrderBy;
			}
		}

		public bool IsCountQuery
		{
			get
			{
				return this.parser.IsCountQuery;
			}
		}

		public int MaxRows
		{
			get
			{
				return this.parser.MaxRows;
			}
		}

		public static DiagnosticQueryParser Create(string query)
		{
			return new DiagnosticQueryParser(query);
		}

		private const string AllColumnsCharacter = "*";

		private readonly Parser parser;

		public enum QueryType
		{
			Unspecified,
			Select,
			Update,
			Insert,
			Delete
		}

		public struct SortColumn : IEquatable<DiagnosticQueryParser.SortColumn>
		{
			public SortColumn(string name)
			{
				this = new DiagnosticQueryParser.SortColumn(name, true);
			}

			public SortColumn(string name, bool asc)
			{
				this.Name = name;
				this.Ascending = asc;
				this.hashCode = (this.Name.GetHashCode() ^ this.Ascending.GetHashCode());
			}

			public override int GetHashCode()
			{
				return this.hashCode;
			}

			public override bool Equals(object obj)
			{
				if (obj is DiagnosticQueryParser.SortColumn)
				{
					DiagnosticQueryParser.SortColumn sortColumn = (DiagnosticQueryParser.SortColumn)obj;
					return this.Equals(sortColumn);
				}
				return false;
			}

			public bool Equals(DiagnosticQueryParser.SortColumn sortColumn)
			{
				return this.Name.Equals(sortColumn.Name) && this.Ascending.Equals(sortColumn.Ascending);
			}

			public readonly string Name;

			public readonly bool Ascending;

			private readonly int hashCode;
		}

		public class Context : IEquatable<DiagnosticQueryParser.Context>
		{
			private Context(string database, string schema, DiagnosticQueryParser.TableInfo table)
			{
				this.database = (database ?? string.Empty);
				this.schema = (schema ?? string.Empty);
				this.table = (table ?? DiagnosticQueryParser.TableInfo.Create(string.Empty));
			}

			public string Database
			{
				get
				{
					return this.database;
				}
			}

			public string Schema
			{
				get
				{
					return this.schema;
				}
			}

			public DiagnosticQueryParser.TableInfo Table
			{
				get
				{
					return this.table;
				}
			}

			public static DiagnosticQueryParser.Context Create(string database, string schema, DiagnosticQueryParser.TableInfo table)
			{
				return new DiagnosticQueryParser.Context(database, schema, table);
			}

			public static DiagnosticQueryParser.Context Create(string database, DiagnosticQueryParser.TableInfo table)
			{
				return new DiagnosticQueryParser.Context(database, null, table);
			}

			public static DiagnosticQueryParser.Context Create(DiagnosticQueryParser.TableInfo table)
			{
				return new DiagnosticQueryParser.Context(null, null, table);
			}

			public override string ToString()
			{
				if (this.readableFormat == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					string[] array = new string[]
					{
						this.Database,
						this.Schema,
						this.Table.Name
					};
					foreach (string text in array)
					{
						if (!string.IsNullOrEmpty(text))
						{
							stringBuilder.AppendFormat("{0}[{1}]", (stringBuilder.Length > 0) ? "." : string.Empty, text);
						}
					}
					if (this.Table.Parameters != null)
					{
						string arg = string.Empty;
						stringBuilder.Append("(");
						foreach (string arg2 in this.table.Parameters)
						{
							stringBuilder.AppendFormat("{0}{1}", arg, arg2);
							arg = ", ";
						}
						stringBuilder.Append(")");
					}
					this.readableFormat = stringBuilder.ToString();
				}
				return this.readableFormat;
			}

			public override int GetHashCode()
			{
				if (this.hashCode == null)
				{
					this.hashCode = new int?(this.Database.GetHashCode() ^ this.Schema.GetHashCode() ^ this.Table.GetHashCode());
				}
				return this.hashCode.Value;
			}

			public override bool Equals(object obj)
			{
				DiagnosticQueryParser.Context context = obj as DiagnosticQueryParser.Context;
				return this.Equals(context);
			}

			public bool Equals(DiagnosticQueryParser.Context context)
			{
				return context != null && (this.Database.Equals(context.Database) && this.Schema.Equals(context.Schema)) && this.Table.Equals(context.Table);
			}

			private readonly string database;

			private readonly string schema;

			private readonly DiagnosticQueryParser.TableInfo table;

			private int? hashCode;

			private string readableFormat;
		}

		public class TableInfo : IEquatable<DiagnosticQueryParser.TableInfo>
		{
			private TableInfo(string name, List<string> parameters)
			{
				this.name = (name ?? string.Empty);
				this.parameters = ((parameters == null) ? null : parameters.ToArray());
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public string[] Parameters
			{
				get
				{
					return this.parameters;
				}
			}

			public static DiagnosticQueryParser.TableInfo Create(string name)
			{
				return new DiagnosticQueryParser.TableInfo(name, null);
			}

			public static DiagnosticQueryParser.TableInfo Create(string name, List<string> parameters)
			{
				return new DiagnosticQueryParser.TableInfo(name, parameters);
			}

			public override int GetHashCode()
			{
				if (this.hashCode == null)
				{
					this.hashCode = new int?(this.Name.GetHashCode());
					if (this.Parameters != null)
					{
						foreach (string text in this.Parameters)
						{
							if (text != null)
							{
								this.hashCode ^= text.GetHashCode();
							}
						}
					}
				}
				return this.hashCode.Value;
			}

			public override bool Equals(object obj)
			{
				DiagnosticQueryParser.TableInfo tableInfo = obj as DiagnosticQueryParser.TableInfo;
				return this.Equals(tableInfo);
			}

			public bool Equals(DiagnosticQueryParser.TableInfo tableInfo)
			{
				if (tableInfo == null)
				{
					return false;
				}
				if (!this.Name.Equals(tableInfo.Name))
				{
					return false;
				}
				if (this.Parameters == null != (tableInfo.Parameters == null))
				{
					return false;
				}
				if (this.Parameters.Length != tableInfo.Parameters.Length)
				{
					return false;
				}
				for (int i = 0; i < this.Parameters.Length; i++)
				{
					if (!this.Parameters[i].Equals(tableInfo.Parameters[i]))
					{
						return false;
					}
				}
				return true;
			}

			private readonly string name;

			private readonly string[] parameters;

			private int? hashCode;
		}

		public class Column : IEquatable<DiagnosticQueryParser.Column>
		{
			protected Column(string identifier, bool subtractor)
			{
				this.identifier = identifier;
				this.subtractor = subtractor;
			}

			public string Identifier
			{
				get
				{
					return this.identifier;
				}
			}

			public bool IsSubtraction
			{
				get
				{
					return this.subtractor;
				}
			}

			public static DiagnosticQueryParser.Column Create(string identifier, bool subtractor)
			{
				return new DiagnosticQueryParser.Column(identifier, subtractor);
			}

			public override bool Equals(object obj)
			{
				DiagnosticQueryParser.Column column = obj as DiagnosticQueryParser.Column;
				return this.Equals(column);
			}

			public override int GetHashCode()
			{
				return this.Identifier.GetHashCode();
			}

			public bool Equals(DiagnosticQueryParser.Column column)
			{
				return column != null && this.Identifier.Equals(column.Identifier) && this.IsSubtraction.Equals(column.IsSubtraction);
			}

			private readonly string identifier;

			private readonly bool subtractor;
		}

		public class Processor : DiagnosticQueryParser.Column, IEquatable<DiagnosticQueryParser.Processor>
		{
			private Processor(string identifier, IList<DiagnosticQueryParser.Column> arguments) : base(identifier, false)
			{
				this.arguments = arguments;
			}

			public IList<DiagnosticQueryParser.Column> Arguments
			{
				get
				{
					return this.arguments;
				}
			}

			public static DiagnosticQueryParser.Column Create(string identifier)
			{
				return DiagnosticQueryParser.Processor.Create(identifier, Array<DiagnosticQueryParser.Column>.Empty);
			}

			public static DiagnosticQueryParser.Column Create(string identifier, IList<DiagnosticQueryParser.Column> arguments)
			{
				return new DiagnosticQueryParser.Processor(identifier, arguments);
			}

			public override bool Equals(object obj)
			{
				DiagnosticQueryParser.Processor processor = obj as DiagnosticQueryParser.Processor;
				return this.Equals(processor);
			}

			public override int GetHashCode()
			{
				if (this.hashCode == null)
				{
					this.hashCode = new int?(base.Identifier.GetHashCode());
					if (this.Arguments != null)
					{
						foreach (DiagnosticQueryParser.Column column in this.Arguments)
						{
							if (column != null)
							{
								this.hashCode ^= column.GetHashCode();
							}
						}
					}
				}
				return this.hashCode.Value;
			}

			public bool Equals(DiagnosticQueryParser.Processor processor)
			{
				if (processor == null)
				{
					return false;
				}
				if (!base.Identifier.Equals(processor.Identifier))
				{
					return false;
				}
				if (this.Arguments == null != (processor.Arguments == null))
				{
					return false;
				}
				if (this.Arguments == null)
				{
					return true;
				}
				if (this.Arguments.Count != processor.Arguments.Count)
				{
					return false;
				}
				for (int i = 0; i < this.Arguments.Count; i++)
				{
					if (!this.Arguments[i].Equals(processor.Arguments[i]))
					{
						return false;
					}
				}
				return true;
			}

			private readonly IList<DiagnosticQueryParser.Column> arguments;

			private int? hashCode;
		}
	}
}
