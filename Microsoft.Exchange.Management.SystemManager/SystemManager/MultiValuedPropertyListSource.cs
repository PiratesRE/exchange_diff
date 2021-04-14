using System;
using System.Collections;
using System.ComponentModel;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class MultiValuedPropertyListSource<T> : IListSource
	{
		public MultiValuedPropertyListSource()
		{
		}

		public MultiValuedPropertyListSource(MultiValuedProperty<T> property)
		{
			this.Property = property;
		}

		[DefaultValue(null)]
		public MultiValuedProperty<T> Property
		{
			get
			{
				return this.property;
			}
			set
			{
				this.property = value;
			}
		}

		public IList GetList()
		{
			return this.property;
		}

		public bool ContainsListCollection
		{
			get
			{
				return false;
			}
		}

		private MultiValuedProperty<T> property;
	}
}
