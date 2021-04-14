using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public class SinkProviderData
	{
		public SinkProviderData(string name)
		{
			this._name = name;
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public IDictionary Properties
		{
			get
			{
				return this._properties;
			}
		}

		public IList Children
		{
			get
			{
				return this._children;
			}
		}

		private string _name;

		private Hashtable _properties = new Hashtable(StringComparer.InvariantCultureIgnoreCase);

		private ArrayList _children = new ArrayList();
	}
}
