using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata
{
	[ComVisible(true)]
	public class SoapAttribute : Attribute
	{
		internal void SetReflectInfo(object info)
		{
			this.ReflectInfo = info;
		}

		public virtual string XmlNamespace
		{
			get
			{
				return this.ProtXmlNamespace;
			}
			set
			{
				this.ProtXmlNamespace = value;
			}
		}

		public virtual bool UseAttribute
		{
			get
			{
				return this._bUseAttribute;
			}
			set
			{
				this._bUseAttribute = value;
			}
		}

		public virtual bool Embedded
		{
			get
			{
				return this._bEmbedded;
			}
			set
			{
				this._bEmbedded = value;
			}
		}

		protected string ProtXmlNamespace;

		private bool _bUseAttribute;

		private bool _bEmbedded;

		protected object ReflectInfo;
	}
}
