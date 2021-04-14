using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ExchangeCertificateIdParameter : IIdentityParameter
	{
		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			throw new NotImplementedException();
		}

		public void Initialize(ObjectId objectId)
		{
			throw new NotImplementedException();
		}

		public string RawIdentity
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ExchangeCertificateIdParameter(INamedIdentity id) : this(id.Identity)
		{
		}

		public ExchangeCertificateIdParameter(string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (id.Length == 0)
			{
				throw new ArgumentException(Strings.ErrorEmptyParameter(base.GetType().ToString()), "identity");
			}
			if (!id.Contains("\\"))
			{
				this.Thumbprint = id;
				return;
			}
			this.Thumbprint = id.Remove(0, id.Split(new char[]
			{
				'\\'
			})[0].Length + 1);
			this.ServerIdParameter = ServerIdParameter.Parse(id.Split(new char[]
			{
				'\\'
			})[0]);
		}

		public ServerIdParameter ServerIdParameter { get; private set; }

		public string Thumbprint { get; private set; }

		public static ExchangeCertificateIdParameter Parse(string id)
		{
			return new ExchangeCertificateIdParameter(id);
		}

		public override string ToString()
		{
			if (this.ServerIdParameter != null)
			{
				return this.ServerIdParameter.ToString() + "\\" + this.Thumbprint;
			}
			return this.Thumbprint;
		}
	}
}
