using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Microsoft.Exchange.AirSync
{
	internal class AnnotationsManager
	{
		public AnnotationsManager()
		{
			this.annotationsStore = new Dictionary<string, Dictionary<string, string>>();
		}

		public void ParseWLAnnotations(XmlNode annotationsNode, string annotationGroup = null)
		{
			if (annotationsNode == null)
			{
				return;
			}
			using (XmlNodeList childNodes = annotationsNode.ChildNodes)
			{
				foreach (object obj in childNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					string innerText = xmlNode["Name", "WindowsLive:"].InnerText;
					string value = null;
					if (xmlNode["Value", "WindowsLive:"] != null)
					{
						value = xmlNode["Value", "WindowsLive:"].InnerText;
					}
					this.AddAnnotationToStore(innerText, value, annotationGroup);
				}
			}
		}

		public void ParseWLAnnotations(XmlNode annotationsNode, string collectionId, string optionsClass)
		{
			this.ParseWLAnnotations(annotationsNode, AnnotationsManager.GetOptionsAnnotationsGroup(collectionId, optionsClass));
		}

		public bool ContainsAnnotation(string name, string group = null)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			group = (group ?? string.Empty);
			Dictionary<string, string> dictionary;
			return this.annotationsStore.ContainsKey(group) && this.annotationsStore.TryGetValue(group, out dictionary) && dictionary.ContainsKey(name);
		}

		public bool ContainsAnnotation(string name, string collectionId, string optionsClass)
		{
			return this.ContainsAnnotation(name, AnnotationsManager.GetOptionsAnnotationsGroup(collectionId, optionsClass));
		}

		public string FetchAnnotation(string name, string group = null)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			group = (group ?? string.Empty);
			Dictionary<string, string> dictionary = this.annotationsStore[group];
			return dictionary[name];
		}

		public string FetchAnnotation(string name, string collectionId, string optionsClass)
		{
			return this.FetchAnnotation(name, AnnotationsManager.GetOptionsAnnotationsGroup(collectionId, optionsClass));
		}

		public Dictionary<string, string> GetAnnotationGroup(string group)
		{
			group = (group ?? string.Empty);
			if (!this.annotationsStore.ContainsKey(group))
			{
				return null;
			}
			return this.annotationsStore[group];
		}

		public Dictionary<string, string> GetAnnotationGroup(string collectionId, string optionsClass)
		{
			return this.GetAnnotationGroup(AnnotationsManager.GetOptionsAnnotationsGroup(collectionId, optionsClass));
		}

		public static string GetCommandAnnotationGroup(string collectionId, string itemId)
		{
			if (collectionId == null)
			{
				throw new ArgumentNullException("collectionId");
			}
			if (itemId == null)
			{
				throw new ArgumentNullException("itemId");
			}
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				collectionId,
				itemId
			});
		}

		public void RemoveAnnotationGroup(string group)
		{
			if (group != null)
			{
				this.annotationsStore.Remove(group);
			}
		}

		public void RemoveAnnotationGroup(string collectionId, string optionsClass)
		{
			this.RemoveAnnotationGroup(AnnotationsManager.GetOptionsAnnotationsGroup(collectionId, optionsClass));
		}

		public void AddAnnotationToStore(string name, string value, string group)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			group = (group ?? string.Empty);
			Dictionary<string, string> dictionary = null;
			if (!this.annotationsStore.TryGetValue(group, out dictionary))
			{
				dictionary = new Dictionary<string, string>();
				this.annotationsStore.Add(group, dictionary);
			}
			if (!dictionary.ContainsKey(name))
			{
				dictionary.Add(name, value);
			}
		}

		public void AddAnnotationToStore(string name, string value, string collectionId, string optionsClass)
		{
			this.AddAnnotationToStore(name, value, AnnotationsManager.GetOptionsAnnotationsGroup(collectionId, optionsClass));
		}

		private static string GetOptionsAnnotationsGroup(string collectionId, string optionsClass)
		{
			if (collectionId == null)
			{
				throw new ArgumentNullException("collectionId");
			}
			if (optionsClass == null)
			{
				throw new ArgumentNullException("optionsClass");
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				collectionId,
				optionsClass
			});
		}

		private Dictionary<string, Dictionary<string, string>> annotationsStore;
	}
}
