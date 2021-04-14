using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UncDocumentLibrary : IDocumentLibrary, IReadOnlyPropertyBag
	{
		private UncDocumentLibrary(UncSession session, UncObjectId id, string description)
		{
			if (id.Path.Segments.Length != 2)
			{
				throw new ArgumentException("id");
			}
			this.shareName = Uri.UnescapeDataString(id.Path.Segments[1]);
			this.description = description;
			this.id = id;
			this.session = session;
			this.directoryInfo = new DirectoryInfo(this.Uri.LocalPath);
			this.directoryInfo.Refresh();
			FileAttributes attributes = this.directoryInfo.Attributes;
		}

		public static UncDocumentLibrary Read(UncSession session, ObjectId objectId)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			UncObjectId uncObjectId = objectId as UncObjectId;
			if (uncObjectId == null || uncObjectId.UriFlags != UriFlags.UncDocumentLibrary)
			{
				throw new ArgumentException("objectId");
			}
			if (!session.Uri.IsBaseOf(uncObjectId.Path))
			{
				throw new ArgumentException("objectId");
			}
			if (uncObjectId.Path.Segments.Length != 2)
			{
				throw new ArgumentException("uncId");
			}
			return Utils.DoUncTask<UncDocumentLibrary>(session.Identity, uncObjectId, false, Utils.MethodType.Read, delegate
			{
				string netName = Uri.UnescapeDataString(uncObjectId.Path.Segments[1].TrimEnd(new char[]
				{
					'\\',
					'/'
				}));
				IntPtr zero = IntPtr.Zero;
				int num = UncSession.NetShareGetInfo(uncObjectId.Path.Host, netName, 1, out zero);
				try
				{
					if (num == 0)
					{
						UncSession.SHARE_INFO_1 share_INFO_ = (UncSession.SHARE_INFO_1)Marshal.PtrToStructure(zero, typeof(UncSession.SHARE_INFO_1));
						try
						{
							return new UncDocumentLibrary(session, uncObjectId, share_INFO_.Remark);
						}
						catch (IOException innerException)
						{
							throw new AccessDeniedException(uncObjectId, Strings.ExAccessDenied(uncObjectId.Path.LocalPath), innerException);
						}
					}
					if (num == 5 || num == 2311)
					{
						throw new AccessDeniedException(uncObjectId, Strings.ExAccessDenied(uncObjectId.Path.LocalPath));
					}
					if (num == 53 || num == 2250)
					{
						throw new ConnectionException(uncObjectId, Strings.ExCannotConnectToMachine(uncObjectId.Path.Host));
					}
					throw new ObjectNotFoundException(uncObjectId, Strings.ExObjectNotFound(uncObjectId.Path.LocalPath));
				}
				finally
				{
					if (zero != IntPtr.Zero)
					{
						UncSession.NetApiBufferFree(zero);
					}
				}
				UncDocumentLibrary result;
				return result;
			});
		}

		public ObjectId Id
		{
			get
			{
				return this.id;
			}
		}

		public string Title
		{
			get
			{
				return this.shareName;
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
		}

		public Uri Uri
		{
			get
			{
				return this.id.Path;
			}
		}

		public List<KeyValuePair<string, Uri>> GetHierarchy()
		{
			List<KeyValuePair<string, Uri>> list = new List<KeyValuePair<string, Uri>>(this.id.Path.Segments.Length - 1);
			string uriString = "\\\\" + this.id.Path.Host;
			list.Add(new KeyValuePair<string, Uri>(this.id.Path.Host, new Uri(uriString)));
			return list;
		}

		IDocumentLibraryItem IDocumentLibrary.Read(ObjectId objectId, params PropertyDefinition[] propsToReturn)
		{
			return this.Read(objectId, propsToReturn);
		}

		public UncDocumentLibraryItem Read(ObjectId objectId, params PropertyDefinition[] propsToReturn)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			UncObjectId uncObjectId = objectId as UncObjectId;
			if (uncObjectId == null)
			{
				throw new ArgumentException("objectId");
			}
			if (uncObjectId.Path.Segments.Length <= 2)
			{
				throw new ArgumentException("objectId");
			}
			if (!this.id.Path.IsBaseOf(uncObjectId.Path))
			{
				throw new ArgumentException("objectId");
			}
			return UncDocumentLibraryItem.Read(this.session, uncObjectId);
		}

		public ITableView GetView(QueryFilter query, SortBy[] sortBy, DocumentLibraryQueryOptions queryOptions, params PropertyDefinition[] propsToReturn)
		{
			return UncDocumentLibraryFolder.InternalGetView(query, sortBy, queryOptions, propsToReturn, this.session, this.directoryInfo, this.id);
		}

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				object obj = this.TryGetProperty(propertyDefinition);
				if (obj is PropertyError)
				{
					throw PropertyErrorException.GetExceptionFromError((PropertyError)obj);
				}
				return obj;
			}
		}

		object[] IReadOnlyPropertyBag.GetProperties(ICollection<PropertyDefinition> propertyDefinitions)
		{
			object[] array = new object[propertyDefinitions.Count];
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
			{
				array[num++] = this.TryGetProperty(propertyDefinition);
			}
			return array;
		}

		private object TryGetProperty(PropertyDefinition propertyDefinition)
		{
			DocumentLibraryPropertyId documentLibraryPropertyId = DocumentLibraryPropertyId.None;
			DocumentLibraryPropertyDefinition documentLibraryPropertyDefinition = propertyDefinition as DocumentLibraryPropertyDefinition;
			if (documentLibraryPropertyDefinition != null)
			{
				documentLibraryPropertyId = documentLibraryPropertyDefinition.PropertyId;
			}
			DocumentLibraryPropertyId documentLibraryPropertyId2 = documentLibraryPropertyId;
			switch (documentLibraryPropertyId2)
			{
			case DocumentLibraryPropertyId.None:
				return new PropertyError(propertyDefinition, PropertyErrorCode.NotFound);
			case DocumentLibraryPropertyId.Uri:
				return this.id.Path;
			default:
				switch (documentLibraryPropertyId2)
				{
				case DocumentLibraryPropertyId.Id:
					return this.id;
				case DocumentLibraryPropertyId.Title:
					return this.shareName;
				default:
					if (documentLibraryPropertyId2 != DocumentLibraryPropertyId.Description)
					{
						return UncDocumentLibraryItem.GetValueFromFileSystemInfo(documentLibraryPropertyDefinition, this.directoryInfo);
					}
					return this.description;
				}
				break;
			}
		}

		private readonly UncObjectId id;

		private readonly string shareName;

		private readonly string description;

		private readonly UncSession session;

		private readonly DirectoryInfo directoryInfo;
	}
}
