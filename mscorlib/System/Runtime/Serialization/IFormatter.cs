using System;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public interface IFormatter
	{
		object Deserialize(Stream serializationStream);

		void Serialize(Stream serializationStream, object graph);

		ISurrogateSelector SurrogateSelector { get; set; }

		SerializationBinder Binder { get; set; }

		StreamingContext Context { get; set; }
	}
}
