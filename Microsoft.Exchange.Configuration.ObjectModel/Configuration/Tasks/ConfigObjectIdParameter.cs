using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ConfigObjectIdParameter : IIdentityParameter
	{
		public ConfigObjectIdParameter()
		{
		}

		public ConfigObjectIdParameter(string identity)
		{
			this.identity = identity;
		}

		internal ConfigObjectIdParameter(ConfigObjectId objectId)
		{
			this.Initialize(objectId);
		}

		public string RawIdentity
		{
			get
			{
				return this.identity;
			}
		}

		public static ConfigObjectIdParameter Parse(string identity)
		{
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentNullException("identity");
			}
			return new ConfigObjectIdParameter(identity);
		}

		public static implicit operator string(ConfigObjectIdParameter objectId)
		{
			if (objectId != null)
			{
				return objectId.identity;
			}
			return null;
		}

		public void Initialize(ObjectId objectId)
		{
			if (objectId != null)
			{
				this.identity = objectId.ToString();
				return;
			}
			this.identity = null;
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return this.GetObjects<T>(rootId, session, null, out localizedString);
		}

		public virtual IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			ConfigObject[] array = null;
			DataSourceManager dataSourceManager = (DataSourceManager)session;
			notFoundReason = null;
			string text = null;
			if (this.identity.StartsWith("CN=", StringComparison.OrdinalIgnoreCase) || this.identity.StartsWith("OU=", StringComparison.OrdinalIgnoreCase))
			{
				text = string.Format("Identity='{0}'", this.identity);
			}
			else
			{
				try
				{
					new Guid(this.identity);
					text = string.Format("Identity='<GUID={0}>'", this.identity);
				}
				catch (FormatException)
				{
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				array = dataSourceManager.Find(typeof(T), text, true, null);
			}
			if (array == null && rootId != null)
			{
				text = string.Format("Identity='CN={0},{1}'", this.identity, rootId.ToString());
				array = dataSourceManager.Find(typeof(T), text, true, null);
			}
			if (array == null)
			{
				array = new ConfigObject[0];
			}
			return (IEnumerable<T>)((IEnumerable<IConfigurable>)array);
		}

		private string identity;
	}
}
