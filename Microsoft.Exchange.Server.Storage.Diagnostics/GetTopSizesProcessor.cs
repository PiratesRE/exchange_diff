using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal abstract class GetTopSizesProcessor : Processor
	{
		protected GetTopSizesProcessor(string prefix, int pairs, IList<Column> arguments) : base(arguments)
		{
			string arg = prefix ?? "Item";
			int num = Math.Max(pairs, 1);
			this.generated = new List<Processor.ColumnDefinition>(num * 2);
			this.getters = new Dictionary<string, Tuple<Func<int, object>, int>>(num * 2);
			for (int i = 0; i < num; i++)
			{
				string text = string.Format("{0}Name{1}", arg, i + 1);
				string text2 = string.Format("{0}Size{1}", arg, i + 1);
				this.generated.Add(new Processor.ColumnDefinition(text, typeof(string), Visibility.Public));
				this.generated.Add(new Processor.ColumnDefinition(text2, typeof(int), Visibility.Public));
				this.getters[text] = new Tuple<Func<int, object>, int>(new Func<int, object>(this.GetIdentifier), i);
				this.getters[text2] = new Tuple<Func<int, object>, int>(new Func<int, object>(this.GetSize), i);
			}
		}

		public override IEnumerable<Processor.ColumnDefinition> GetGeneratedColumns()
		{
			return this.generated;
		}

		public override void OnBeginRow()
		{
			this.data = null;
		}

		public override object GetValue(SimpleQueryOperator qop, Reader reader, Column column)
		{
			if (this.data == null)
			{
				this.data = this.GetData(qop, reader);
				this.data.Sort(GetTopSizesProcessor.sizeComparer);
			}
			Tuple<Func<int, object>, int> tuple;
			if (column != null && this.getters.TryGetValue(column.Name, out tuple))
			{
				return tuple.Item1(tuple.Item2);
			}
			return null;
		}

		public abstract List<Tuple<string, int>> GetData(SimpleQueryOperator qop, Reader reader);

		private object GetIdentifier(int index)
		{
			if (this.data != null && this.data.Count > index)
			{
				return this.data[index].Item1;
			}
			return null;
		}

		private object GetSize(int index)
		{
			if (this.data != null && this.data.Count > index)
			{
				return this.data[index].Item2;
			}
			return null;
		}

		private static Comparison<Tuple<string, int>> sizeComparer = (Tuple<string, int> x, Tuple<string, int> y) => y.Item2.CompareTo(x.Item2);

		private readonly IList<Processor.ColumnDefinition> generated;

		private readonly IDictionary<string, Tuple<Func<int, object>, int>> getters;

		private List<Tuple<string, int>> data;
	}
}
