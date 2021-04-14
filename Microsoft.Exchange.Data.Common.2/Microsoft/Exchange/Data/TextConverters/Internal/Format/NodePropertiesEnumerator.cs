using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal struct NodePropertiesEnumerator : IEnumerable<Property>, IEnumerable, IEnumerator<Property>, IDisposable, IEnumerator
	{
		public NodePropertiesEnumerator(FormatNode node)
		{
			this.FlagProperties = node.FlagProperties;
			this.Properties = node.Properties;
			this.CurrentPropertyIndex = 0;
			this.CurrentProperty = Property.Null;
		}

		public Property Current
		{
			get
			{
				return this.CurrentProperty;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.CurrentProperty;
			}
		}

		public IEnumerator<Property> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}

		public bool MoveNext()
		{
			while (this.CurrentPropertyIndex < 16)
			{
				this.CurrentPropertyIndex++;
				if (this.FlagProperties.IsDefined((PropertyId)this.CurrentPropertyIndex))
				{
					this.CurrentProperty.Set((PropertyId)this.CurrentPropertyIndex, new PropertyValue(this.FlagProperties.IsOn((PropertyId)this.CurrentPropertyIndex)));
					return true;
				}
			}
			if (this.Properties != null && this.CurrentPropertyIndex < this.Properties.Length + 17)
			{
				this.CurrentPropertyIndex++;
				if (this.CurrentPropertyIndex < this.Properties.Length + 17)
				{
					this.CurrentProperty = this.Properties[this.CurrentPropertyIndex - 17];
					return true;
				}
			}
			this.CurrentProperty = Property.Null;
			return false;
		}

		public void Reset()
		{
			this.CurrentPropertyIndex = 0;
			this.CurrentProperty = Property.Null;
		}

		public void Dispose()
		{
		}

		internal FlagProperties FlagProperties;

		internal Property[] Properties;

		internal int CurrentPropertyIndex;

		internal Property CurrentProperty;
	}
}
