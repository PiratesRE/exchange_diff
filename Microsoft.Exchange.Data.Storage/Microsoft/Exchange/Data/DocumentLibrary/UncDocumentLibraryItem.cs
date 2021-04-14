using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class UncDocumentLibraryItem : IDocumentLibraryItem, IReadOnlyPropertyBag
	{
		internal UncDocumentLibraryItem(UncSession session, UncObjectId uncObjectId, FileSystemInfo fileSystemInfo, Schema schema)
		{
			this.session = session;
			this.uncId = uncObjectId;
			this.fileSystemInfo = fileSystemInfo;
			this.schema = schema;
			try
			{
				this.fileSystemInfo.Refresh();
				if (this.fileSystemInfo.Attributes == (FileAttributes)(-1))
				{
					throw new ObjectNotFoundException(uncObjectId, Strings.ExObjectNotFound(uncObjectId.Path.LocalPath));
				}
			}
			catch (ArgumentException innerException)
			{
				throw new ObjectNotFoundException(uncObjectId, Strings.ExObjectNotFound(uncObjectId.Path.LocalPath), innerException);
			}
		}

		public static UncDocumentLibraryItem Read(UncSession session, ObjectId id)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			UncObjectId uncObjectId = id as UncObjectId;
			if (uncObjectId == null)
			{
				throw new ArgumentException("id");
			}
			if (uncObjectId.UriFlags != UriFlags.UncDocument && uncObjectId.UriFlags != UriFlags.UncFolder)
			{
				throw new ArgumentException("id");
			}
			if (!session.Uri.IsBaseOf(uncObjectId.Path))
			{
				throw new ArgumentException("objectId");
			}
			return Utils.DoUncTask<UncDocumentLibraryItem>(session.Identity, uncObjectId, false, Utils.MethodType.Read, delegate
			{
				FileSystemInfo fileSystemInfo = new FileInfo(uncObjectId.Path.LocalPath);
				if (fileSystemInfo.Attributes == (FileAttributes)(-1))
				{
					throw new ObjectNotFoundException(uncObjectId, Strings.ExObjectNotFound(uncObjectId.Path.LocalPath));
				}
				if (fileSystemInfo.Exists)
				{
					return new UncDocument(session, uncObjectId);
				}
				if ((fileSystemInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
				{
					return new UncDocumentLibraryFolder(session, uncObjectId);
				}
				throw new ObjectNotFoundException(uncObjectId, Strings.ExObjectNotFound(uncObjectId.Path.LocalPath));
			});
		}

		public virtual string DisplayName
		{
			get
			{
				return this.fileSystemInfo.Name;
			}
		}

		public Uri Uri
		{
			get
			{
				return this.uncId.Path;
			}
		}

		public ObjectId Id
		{
			get
			{
				return this.uncId;
			}
		}

		public bool IsFolder
		{
			get
			{
				return (this.fileSystemInfo.Attributes & FileAttributes.Directory) != (FileAttributes)0;
			}
		}

		IDocumentLibrary IDocumentLibraryItem.Library
		{
			get
			{
				return this.Library;
			}
		}

		IDocumentLibraryFolder IDocumentLibraryItem.Parent
		{
			get
			{
				return this.Parent;
			}
		}

		public List<KeyValuePair<string, Uri>> GetHierarchy()
		{
			List<KeyValuePair<string, Uri>> list = new List<KeyValuePair<string, Uri>>(this.uncId.Path.Segments.Length - 1);
			string text = "\\\\" + this.uncId.Path.Host;
			list.Add(new KeyValuePair<string, Uri>(this.uncId.Path.Host, new Uri(text)));
			for (int i = 1; i < this.uncId.Path.Segments.Length - 1; i++)
			{
				string text2 = this.uncId.Path.Segments[i].TrimEnd(new char[]
				{
					'/',
					'\\'
				});
				text2 = Uri.UnescapeDataString(text2);
				text = Path.Combine(text, text2);
				list.Add(new KeyValuePair<string, Uri>(text2, new Uri(text)));
			}
			return list;
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
			set
			{
				throw new NotSupportedException();
			}
		}

		public object TryGetProperty(PropertyDefinition propertyDefinition)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			DocumentLibraryPropertyDefinition documentLibraryPropertyDefinition = propertyDefinition as DocumentLibraryPropertyDefinition;
			if (documentLibraryPropertyDefinition == null)
			{
				throw new ArgumentException("propertyDefinition");
			}
			return this.TryGetProperty(documentLibraryPropertyDefinition);
		}

		object[] IReadOnlyPropertyBag.GetProperties(ICollection<PropertyDefinition> propertyDefinitions)
		{
			if (propertyDefinitions == null)
			{
				return Array<object>.Empty;
			}
			object[] array = new object[propertyDefinitions.Count];
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
			{
				array[num++] = this.TryGetProperty(propertyDefinition);
			}
			return array;
		}

		public UncDocumentLibrary Library
		{
			get
			{
				if (this.documentLibrary == null)
				{
					string text = Path.Combine("\\\\" + this.uncId.Path.Host, this.uncId.Path.Segments[1].TrimEnd(new char[]
					{
						'\\',
						'/'
					}));
					text = Uri.UnescapeDataString(text);
					UncObjectId objectId = new UncObjectId(new Uri(text), UriFlags.UncDocumentLibrary);
					this.documentLibrary = UncDocumentLibrary.Read(this.session, objectId);
				}
				return this.documentLibrary;
			}
		}

		public UncDocumentLibraryFolder Parent
		{
			get
			{
				if (!this.parentInitialized)
				{
					this.parent = Utils.DoUncTask<UncDocumentLibraryFolder>(this.session.Identity, this.UncId, false, Utils.MethodType.Read, delegate
					{
						string parentDirectoryNameInternal = this.GetParentDirectoryNameInternal();
						if (parentDirectoryNameInternal != null)
						{
							Uri uri = new Uri(parentDirectoryNameInternal);
							if (uri.Segments.Length > 2)
							{
								return new UncDocumentLibraryFolder(this.session, new UncObjectId(new Uri(parentDirectoryNameInternal), UriFlags.UncFolder));
							}
						}
						return null;
					});
					this.parentInitialized = true;
				}
				return this.parent;
			}
		}

		internal static object GetValueFromFileSystemInfo(DocumentLibraryPropertyDefinition propertyDefinition, FileSystemInfo fileSystemInfo)
		{
			FileInfo fileInfo = fileSystemInfo as FileInfo;
			switch (propertyDefinition.PropertyId)
			{
			case DocumentLibraryPropertyId.Uri:
				return new Uri(fileSystemInfo.FullName);
			case DocumentLibraryPropertyId.CreationTime:
				return fileSystemInfo.CreationTimeUtc;
			case DocumentLibraryPropertyId.LastModifiedTime:
				return fileSystemInfo.LastWriteTimeUtc;
			case DocumentLibraryPropertyId.IsFolder:
				return (fileSystemInfo.Attributes & FileAttributes.Directory) != (FileAttributes)0;
			case DocumentLibraryPropertyId.IsHidden:
				return (fileSystemInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
			case DocumentLibraryPropertyId.Id:
				return new UncObjectId(new Uri(fileSystemInfo.FullName), ((fileSystemInfo.Attributes & FileAttributes.Directory) != (FileAttributes)0) ? UriFlags.UncFolder : UriFlags.UncDocument);
			case DocumentLibraryPropertyId.Title:
				return fileSystemInfo.Name;
			case DocumentLibraryPropertyId.FileSize:
				if (fileInfo != null)
				{
					return fileInfo.Length;
				}
				return new PropertyError(propertyDefinition, PropertyErrorCode.NotFound);
			case DocumentLibraryPropertyId.FileType:
				if (fileInfo == null)
				{
					return new PropertyError(propertyDefinition, PropertyErrorCode.NotFound);
				}
				if (!string.IsNullOrEmpty(fileInfo.Extension))
				{
					return ExtensionToContentTypeMapper.Instance.GetContentTypeByExtension(fileInfo.Extension.Substring(1));
				}
				return "application/octet-stream";
			case DocumentLibraryPropertyId.BaseName:
				return fileSystemInfo.Name;
			}
			throw PropertyErrorException.GetExceptionFromError(new PropertyError(propertyDefinition, PropertyErrorCode.NotSupported));
		}

		protected abstract string GetParentDirectoryNameInternal();

		public virtual object TryGetProperty(DocumentLibraryPropertyDefinition propertyDefinition)
		{
			if (this.schema.AllProperties.ContainsKey(propertyDefinition))
			{
				return UncDocumentLibraryItem.GetValueFromFileSystemInfo(propertyDefinition, this.fileSystemInfo);
			}
			return new PropertyError(propertyDefinition, PropertyErrorCode.NotFound);
		}

		internal UncObjectId UncId
		{
			get
			{
				return this.uncId;
			}
		}

		private readonly Schema schema;

		private readonly UncObjectId uncId;

		protected UncSession session;

		private UncDocumentLibraryFolder parent;

		private bool parentInitialized;

		protected FileSystemInfo fileSystemInfo;

		private UncDocumentLibrary documentLibrary;
	}
}
