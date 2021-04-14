using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToAddE12DStoExSPermissionsException : LocalizedException
	{
		public UnableToAddE12DStoExSPermissionsException(string dom, string e12ds, string exs, string exsShort, string localdomdc, string rootdomdc) : base(Strings.UnableToAddE12DStoExSPermissionsException(dom, e12ds, exs, exsShort, localdomdc, rootdomdc))
		{
			this.dom = dom;
			this.e12ds = e12ds;
			this.exs = exs;
			this.exsShort = exsShort;
			this.localdomdc = localdomdc;
			this.rootdomdc = rootdomdc;
		}

		public UnableToAddE12DStoExSPermissionsException(string dom, string e12ds, string exs, string exsShort, string localdomdc, string rootdomdc, Exception innerException) : base(Strings.UnableToAddE12DStoExSPermissionsException(dom, e12ds, exs, exsShort, localdomdc, rootdomdc), innerException)
		{
			this.dom = dom;
			this.e12ds = e12ds;
			this.exs = exs;
			this.exsShort = exsShort;
			this.localdomdc = localdomdc;
			this.rootdomdc = rootdomdc;
		}

		protected UnableToAddE12DStoExSPermissionsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dom = (string)info.GetValue("dom", typeof(string));
			this.e12ds = (string)info.GetValue("e12ds", typeof(string));
			this.exs = (string)info.GetValue("exs", typeof(string));
			this.exsShort = (string)info.GetValue("exsShort", typeof(string));
			this.localdomdc = (string)info.GetValue("localdomdc", typeof(string));
			this.rootdomdc = (string)info.GetValue("rootdomdc", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dom", this.dom);
			info.AddValue("e12ds", this.e12ds);
			info.AddValue("exs", this.exs);
			info.AddValue("exsShort", this.exsShort);
			info.AddValue("localdomdc", this.localdomdc);
			info.AddValue("rootdomdc", this.rootdomdc);
		}

		public string Dom
		{
			get
			{
				return this.dom;
			}
		}

		public string E12ds
		{
			get
			{
				return this.e12ds;
			}
		}

		public string Exs
		{
			get
			{
				return this.exs;
			}
		}

		public string ExsShort
		{
			get
			{
				return this.exsShort;
			}
		}

		public string Localdomdc
		{
			get
			{
				return this.localdomdc;
			}
		}

		public string Rootdomdc
		{
			get
			{
				return this.rootdomdc;
			}
		}

		private readonly string dom;

		private readonly string e12ds;

		private readonly string exs;

		private readonly string exsShort;

		private readonly string localdomdc;

		private readonly string rootdomdc;
	}
}
