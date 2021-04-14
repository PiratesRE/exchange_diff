using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ContentFilterPhraseIdParameter : IIdentityParameter
	{
		public ContentFilterPhraseIdParameter(string phrase)
		{
			this.phrase = phrase;
		}

		public ContentFilterPhraseIdParameter(ContentFilterPhrase phrase)
		{
			if (phrase == null)
			{
				throw new ArgumentException(Strings.ExArgumentNullException("phrase"), "phrase");
			}
			this.phrase = phrase.Phrase;
		}

		public ContentFilterPhraseIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
		}

		string IIdentityParameter.RawIdentity
		{
			get
			{
				return this.RawIdentity;
			}
		}

		internal string RawIdentity
		{
			get
			{
				return this.phrase;
			}
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			return this.GetObjects<T>(rootId, session, optionalData, out notFoundReason);
		}

		internal IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return this.GetObjects<T>(rootId, session, null, out localizedString);
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session)
		{
			return this.GetObjects<T>(rootId, session);
		}

		internal IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			if (!(session is ContentFilterPhraseDataProvider))
			{
				throw new ArgumentException(Strings.ErrorInvalidType((session != null) ? session.GetType().Name : "null"), "session");
			}
			notFoundReason = null;
			IConfigurable configurable = session.Read<T>(new ContentFilterPhraseIdentity(this.phrase));
			T[] result;
			if (configurable != null)
			{
				result = new T[]
				{
					(T)((object)configurable)
				};
			}
			else
			{
				result = new T[0];
			}
			return result;
		}

		void IIdentityParameter.Initialize(ObjectId objectId)
		{
			this.Initialize(objectId);
		}

		internal void Initialize(ObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentException(Strings.ErrorInvalidIdentity("null"), "objectId");
			}
			ContentFilterPhraseIdentity contentFilterPhraseIdentity = objectId as ContentFilterPhraseIdentity;
			if (contentFilterPhraseIdentity == null)
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterType("objectId", typeof(ContentFilterPhraseIdentity).Name), "objectId");
			}
			this.phrase = Encoding.Unicode.GetString(contentFilterPhraseIdentity.GetBytes());
		}

		public override string ToString()
		{
			return this.phrase;
		}

		private string phrase;
	}
}
