using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public static class TableDecorator
	{
		public static string CreateTable(string[] headers, IEnumerable<IEnumerable<string>> rows)
		{
			return TableDecorator.CreateTable<IEnumerable<string>>(headers, rows, (IEnumerable<string> row) => row.ToArray<string>());
		}

		public static string CreateTable<T>(string[] headers, IEnumerable<T> rowData, Func<T, string[]> getRowCellsDelegate)
		{
			if (headers == null || headers.Length == 0)
			{
				throw new ArgumentException("Headers cannot be null or 0 length", "headers");
			}
			if (getRowCellsDelegate == null)
			{
				throw new ArgumentNullException("getRowCellsDelegate");
			}
			if (rowData == null)
			{
				throw new ArgumentNullException("rowData");
			}
			StringBuilder stringBuilder = new StringBuilder(5120, 102400);
			StringBuilder stringBuilder2 = new StringBuilder(1024);
			try
			{
				foreach (string arg in headers)
				{
					stringBuilder2.AppendFormat(TableDecorator.TableHeaderCell, arg);
				}
				stringBuilder.AppendFormat(TableDecorator.TableStartHtml, stringBuilder2.ToString());
				string text = TableDecorator.RowOneColour;
				foreach (T arg2 in rowData)
				{
					string[] array = getRowCellsDelegate(arg2);
					if (array.Length != headers.Length)
					{
						throw new ArgumentException(string.Format("Cell's array length ({0}) doesn't match length of header rows ({1})", array.Length, headers.Length));
					}
					StringBuilder stringBuilder3 = new StringBuilder(5120);
					foreach (string arg3 in array)
					{
						stringBuilder3.AppendFormat(TableDecorator.TableRowCell, arg3);
					}
					stringBuilder.AppendFormat(TableDecorator.TableRowHtml, text, stringBuilder3.ToString());
					text = (text.Equals(TableDecorator.RowOneColour) ? TableDecorator.RowTwoColour : TableDecorator.RowOneColour);
				}
			}
			catch (ArgumentOutOfRangeException)
			{
				stringBuilder.Remove(stringBuilder.Length - 1 - TableDecorator.TableEndHtml.Length, TableDecorator.TableEndHtml.Length);
			}
			finally
			{
				stringBuilder.Append(TableDecorator.TableEndHtml);
			}
			return stringBuilder.ToString();
		}

		private const int StartingTableSize = 5120;

		private const int MaxTableSize = 102400;

		private const int MaxRowSize = 5120;

		private static readonly string RowOneColour = "#fff";

		private static readonly string RowTwoColour = "#f2f2f2";

		private static readonly string TableStartHtml = "<table cellpadding=\"3\" cellspacing=\"0\" style=\"border: 1px solid #ccc;\"><tr style=\"background: #00223B; color: #fefefe; font-weight: bold\">{0}</tr>";

		private static readonly string TableHeaderCell = "<th>{0}</th>";

		private static readonly string TableRowCell = "<td>{0}</td>";

		private static readonly string TableRowHtml = "<tr style=\"background: {0}\">{1}</tr>";

		private static readonly string TableEndHtml = "</table>";
	}
}
