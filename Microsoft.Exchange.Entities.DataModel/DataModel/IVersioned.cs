using System;

namespace Microsoft.Exchange.Entities.DataModel
{
	public interface IVersioned
	{
		string ChangeKey { get; set; }
	}
}
