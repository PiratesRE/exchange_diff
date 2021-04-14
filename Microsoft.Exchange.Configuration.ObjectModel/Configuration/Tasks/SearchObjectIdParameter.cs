using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class SearchObjectIdParameter : IIdentityParameter
	{
		public bool IsFullyQualified
		{
			get
			{
				return this.isFullyQualified;
			}
		}

		private void Parse(string rawString)
		{
			if (string.IsNullOrEmpty(rawString))
			{
				return;
			}
			this.identifier = rawString.Trim();
			SearchObjectId searchObjectId;
			if (SearchObjectId.TryParse(this.identifier, out searchObjectId))
			{
				this.objectIdentifier = searchObjectId;
				return;
			}
			if (this.identifier[this.identifier.Length - 1] == '*')
			{
				this.isFullyQualified = false;
				this.identifier = this.identifier.TrimEnd(new char[]
				{
					'*'
				});
			}
		}

		public SearchObjectIdParameter()
		{
		}

		public SearchObjectIdParameter(string identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (identity.Length == 0)
			{
				throw new ArgumentException(Strings.ErrorEmptyParameter(base.GetType().ToString()), "identity");
			}
			this.rawIdentity = identity;
			this.Parse(identity);
		}

		public SearchObjectIdParameter(ObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			((IIdentityParameter)this).Initialize(objectId);
		}

		public SearchObjectIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
			this.rawIdentity = namedIdentity.DisplayName;
		}

		internal virtual void Initialize(ObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			SearchObjectId searchObjectId = objectId as SearchObjectId;
			if (searchObjectId == null)
			{
				throw new ArgumentException("objectId");
			}
			this.objectIdentifier = searchObjectId;
			this.identifier = objectId.ToString();
			this.rawIdentity = objectId.ToString();
		}

		void IIdentityParameter.Initialize(ObjectId objectId)
		{
			this.Initialize(objectId);
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return ((IIdentityParameter)this).GetObjects<T>(rootId, session, null, out localizedString);
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			MailboxDataProvider mailboxDataProvider = (MailboxDataProvider)session;
			if (mailboxDataProvider == null)
			{
				throw new ArgumentNullException("session");
			}
			if (this.objectIdentifier == null)
			{
				QueryFilter queryFilter = new TextFilter(SearchObjectBaseSchema.Name, this.identifier, this.IsFullyQualified ? MatchOptions.FullString : MatchOptions.Prefix, MatchFlags.IgnoreCase);
				if (this.IsFullyQualified)
				{
					notFoundReason = new LocalizedString?(Strings.ErrorManagementObjectNotFound(this.ToString()));
				}
				else
				{
					notFoundReason = null;
				}
				if (optionalData != null && optionalData.AdditionalFilter != null)
				{
					queryFilter = QueryFilter.AndTogether(new QueryFilter[]
					{
						queryFilter,
						optionalData.AdditionalFilter
					});
				}
				return mailboxDataProvider.FindPaged<T>(queryFilter, rootId, false, null, 0);
			}
			notFoundReason = new LocalizedString?(Strings.ErrorManagementObjectNotFound(this.ToString()));
			SearchObjectId identity = this.objectIdentifier;
			if (optionalData != null && optionalData.AdditionalFilter != null)
			{
				throw new NotSupportedException("Supplying Additional Filters and an ObjectIdentifier is not currently supported by this IdParameter.");
			}
			int num = this.identifier.IndexOf('\\');
			if (num == -1 || string.IsNullOrEmpty(this.identifier.Substring(0, num)))
			{
				SearchObjectBase searchObjectBase = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T)) as SearchObjectBase;
				if (searchObjectBase == null)
				{
					throw new ArgumentException("The generic type must be a SearchObjectBase");
				}
				identity = new SearchObjectId(identity, searchObjectBase.ObjectType);
			}
			T t = (T)((object)mailboxDataProvider.Read<T>(identity));
			if (t != null)
			{
				return new T[]
				{
					t
				};
			}
			return new T[0];
		}

		public string RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
		}

		public override string ToString()
		{
			return this.rawIdentity;
		}

		private string identifier;

		private string rawIdentity;

		private SearchObjectId objectIdentifier;

		private bool isFullyQualified = true;
	}
}
