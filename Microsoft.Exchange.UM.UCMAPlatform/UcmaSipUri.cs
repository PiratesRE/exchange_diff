using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Rtc.Signaling;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class UcmaSipUri : PlatformSipUri
	{
		public UcmaSipUri(string uri)
		{
			this.impl = new SipUriParser(uri);
		}

		public UcmaSipUri(SipUriScheme scheme, string user, string host)
		{
			this.impl = (string.IsNullOrEmpty(user) ? new SipUriParser(scheme.ToString().ToLowerInvariant(), host) : new SipUriParser(scheme.ToString().ToLowerInvariant(), user, host));
		}

		private UcmaSipUri(SipUriParser impl)
		{
			this.impl = impl;
		}

		public override string Host
		{
			get
			{
				return this.impl.Host;
			}
			set
			{
				this.impl.Host = value;
			}
		}

		public override int Port
		{
			get
			{
				return this.impl.Port;
			}
			set
			{
				this.impl.Port = value;
			}
		}

		public override string User
		{
			get
			{
				return this.impl.User;
			}
			set
			{
				this.impl.User = value;
			}
		}

		public override string SimplifiedUri
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}@{1}", new object[]
				{
					this.User,
					this.Host
				});
			}
		}

		public override UserParameter UserParameter
		{
			get
			{
				if (this.impl.UserParameter == null)
				{
					return UserParameter.None;
				}
				return (UserParameter)Enum.Parse(typeof(UserParameter), this.impl.UserParameter, true);
			}
			set
			{
				if (value == UserParameter.None)
				{
					this.impl.UserParameter = null;
					return;
				}
				this.impl.UserParameter = value.ToString().ToLowerInvariant();
			}
		}

		public override TransportParameter TransportParameter
		{
			get
			{
				if (this.impl.TransportParameter == null)
				{
					return TransportParameter.None;
				}
				return (TransportParameter)Enum.Parse(typeof(TransportParameter), this.impl.TransportParameter, true);
			}
			set
			{
				if (value == TransportParameter.None)
				{
					this.impl.TransportParameter = null;
					return;
				}
				this.impl.TransportParameter = value.ToString().ToLowerInvariant();
			}
		}

		public static bool TryParse(string uriString, out PlatformSipUri sipUri)
		{
			SipUriParser sipUriParser = null;
			sipUri = null;
			if (SipUriParser.TryParse(uriString, ref sipUriParser))
			{
				sipUri = new UcmaSipUri(sipUriParser);
			}
			return null != sipUri;
		}

		public override void AddParameter(string name, string value)
		{
			SipUriParameter sipUriParameter = new SipUriParameter(name, value);
			this.impl.AddParameter(sipUriParameter);
		}

		public override string FindParameter(string name)
		{
			string result = null;
			SipUriParameter sipUriParameter = this.impl.FindParameter(name);
			if (sipUriParameter != null)
			{
				result = sipUriParameter.Value;
			}
			return result;
		}

		public override void RemoveParameter(string name)
		{
			SipUriParameter sipUriParameter = this.impl.FindParameter(name);
			if (sipUriParameter != null)
			{
				this.impl.RemoveParameter(sipUriParameter);
			}
		}

		public override IEnumerable<PlatformSipUriParameter> GetParametersThatHaveValues()
		{
			List<PlatformSipUriParameter> list = new List<PlatformSipUriParameter>(8);
			foreach (SipUriParameter sipUriParameter in this.impl.GetParameters())
			{
				if (sipUriParameter != null && sipUriParameter.IsSet)
				{
					list.Add(new PlatformSipUriParameter(sipUriParameter.Name, sipUriParameter.Value));
				}
			}
			return list;
		}

		public override string ToString()
		{
			return this.impl.ToString();
		}

		private SipUriParser impl;
	}
}
