using System;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.AsyncTask
{
	internal class AggregateException : OperationFailedException
	{
		internal AggregateException(params ComponentException[] innerExceptions)
		{
			Util.ThrowOnNullOrEmptyArgument<ComponentException>(innerExceptions, "innerExceptions");
			this.innerExceptions = new ReadOnlyCollection<ComponentException>(innerExceptions);
		}

		internal ReadOnlyCollection<ComponentException> InnerExceptions
		{
			get
			{
				return this.innerExceptions;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.Append(base.ToString());
			stringBuilder.AppendLine();
			for (int i = 0; i < this.innerExceptions.Count; i++)
			{
				stringBuilder.AppendFormat(" (Inner Exception #{0}) {1} <---", i, this.innerExceptions[i].ToString());
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}

		private readonly ReadOnlyCollection<ComponentException> innerExceptions;
	}
}
