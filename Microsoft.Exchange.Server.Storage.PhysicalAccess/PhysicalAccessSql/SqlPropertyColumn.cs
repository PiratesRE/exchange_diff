using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	internal sealed class SqlPropertyColumn : PropertyColumn, ISqlColumn
	{
		public SqlPropertyColumn(string name, Type type, int size, int maxLength, Table table, StorePropTag propTag, Column[] dependOn) : base(name, type, size, maxLength, table, propTag, null, dependOn)
		{
			this.udtMethodName = SqlPropertyColumn.MethodNameFromPropType(propTag.PropType);
		}

		public void AppendExpressionToQuery(SqlQueryModel model, ColumnUse use, SqlCommand command)
		{
			if (base.Table.SpecialCols.PropBag == null)
			{
				command.Append("NULL");
				return;
			}
			command.AppendColumn(base.Table.SpecialCols.PropBag, model, use);
			command.Append(".");
			command.Append(this.udtMethodName);
			command.Append("(");
			command.AppendParameter((int)base.StorePropTag.PropTag);
			command.Append(")");
		}

		public void AppendNameToQuery(SqlCommand command)
		{
			command.Append(this.Name);
		}

		public void AppendQueryText(SqlQueryModel model, SqlCommand command)
		{
			command.AppendColumn(this, model, ColumnUse.Criteria);
		}

		private static string MethodNameFromPropType(PropertyType propType)
		{
			if (propType <= PropertyType.MVInt64)
			{
				if (propType <= PropertyType.SysTime)
				{
					if (propType <= PropertyType.Int64)
					{
						switch (propType)
						{
						case PropertyType.Int16:
							return "GetInt16";
						case PropertyType.Int32:
							return "GetInt32";
						case PropertyType.Real32:
							return "GetSingle";
						case PropertyType.Real64:
							return "GetDouble";
						case PropertyType.Currency:
						case (PropertyType)8:
						case (PropertyType)9:
						case PropertyType.Error:
						case (PropertyType)12:
							break;
						case PropertyType.AppTime:
							return "GetDouble";
						case PropertyType.Boolean:
							return "GetBoolean";
						case PropertyType.Object:
							return "GetBinary";
						default:
							if (propType == PropertyType.Int64)
							{
								return "GetInt64";
							}
							break;
						}
					}
					else
					{
						if (propType == PropertyType.Unicode)
						{
							return "GetString";
						}
						if (propType == PropertyType.SysTime)
						{
							return "GetDateTime";
						}
					}
				}
				else if (propType <= PropertyType.SvrEid)
				{
					if (propType == PropertyType.Guid)
					{
						return "GetGuid";
					}
					if (propType == PropertyType.SvrEid)
					{
						return "GetBinary";
					}
				}
				else
				{
					if (propType == PropertyType.Binary)
					{
						return "GetBinary";
					}
					switch (propType)
					{
					case PropertyType.MVInt16:
						return "GetMVInt16";
					case PropertyType.MVInt32:
						return "GetMVInt32";
					case PropertyType.MVReal32:
						return "GetMVSingle";
					case PropertyType.MVReal64:
						return "GetMVDouble";
					case PropertyType.MVCurrency:
						return "GetMVInt64";
					case PropertyType.MVAppTime:
						return "GetMVDouble";
					default:
						if (propType == PropertyType.MVInt64)
						{
							return "GetMVInt64";
						}
						break;
					}
				}
			}
			else if (propType <= PropertyType.MVIAppTime)
			{
				if (propType <= PropertyType.MVSysTime)
				{
					if (propType == PropertyType.MVUnicode)
					{
						return "GetMVString";
					}
					if (propType == PropertyType.MVSysTime)
					{
						return "GetMVDateTime";
					}
				}
				else
				{
					if (propType == PropertyType.MVGuid)
					{
						return "GetMVGuid";
					}
					if (propType == PropertyType.MVBinary)
					{
						return "GetMVBinary";
					}
					switch (propType)
					{
					case PropertyType.MVIInt16:
						return "GetInt16";
					case PropertyType.MVIInt32:
						return "GetInt32";
					case PropertyType.MVIReal32:
						return "GetSingle";
					case PropertyType.MVIReal64:
						return "GetDouble";
					case PropertyType.MVIAppTime:
						return "GetDouble";
					}
				}
			}
			else if (propType <= PropertyType.MVIUnicode)
			{
				if (propType == PropertyType.MVIInt64)
				{
					return "GetInt64";
				}
				if (propType == PropertyType.MVIUnicode)
				{
					return "GetString";
				}
			}
			else
			{
				if (propType == PropertyType.MVISysTime)
				{
					return "GetDateTime";
				}
				if (propType == PropertyType.MVIGuid)
				{
					return "GetGuid";
				}
				if (propType == PropertyType.MVIBinary)
				{
					return "GetBinary";
				}
			}
			throw new InvalidOperationException("MethodNameFromType: Unexpected prop Type " + propType);
		}

		private readonly string udtMethodName;
	}
}
