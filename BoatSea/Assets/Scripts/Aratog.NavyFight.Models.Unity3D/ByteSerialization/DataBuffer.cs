using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class DataBuffer {

	private enum DataType : byte {

		Int,
		Float,
		Bool,
		String,

	}

	private class Datum {

		public byte[] Bytes {
			get { return bytes.ToArray(); }
		}

		public int Size {
			get {
				switch (Type) {
					case DataType.Int :
						return sizeof (int);
					case DataType.Float :
						return sizeof (double);
					case DataType.Bool :
						return sizeof (bool);
					case DataType.String :
						return sizeof (int) + ((string) Value).Length * sizeof (char);
				}

				return 0;
			}
		}

		public object Value { get; private set; }
		public DataType Type { get; private set; }

		private readonly List<byte> bytes;

		private Datum () {
			bytes = new List<byte>();
		}

		public Datum (int value) : this() {
			bytes.Add((byte) DataType.Int);
			bytes.AddRange(BitConverter.GetBytes(value));
		}

		public Datum (float value) : this() {
			bytes.Add((byte) DataType.Float);
			bytes.AddRange(BitConverter.GetBytes(Convert.ToDouble(value)));
		}

		public Datum (bool value) : this() {
			bytes.Add((byte) DataType.Bool);
			bytes.AddRange(BitConverter.GetBytes(value));
		}

		public Datum (string value) : this() {
			bytes.Add((byte) DataType.String);
			bytes.AddRange(BitConverter.GetBytes(value.Length));
			bytes.AddRange(Encoding.Unicode.GetBytes(value));
		}

		public Datum (byte[] buffer, int offset) {
			bytes = new List<byte>();

			for (int i = 0; i != buffer.Length - offset; i++) {
				bytes.Add(buffer[i + offset]);
			}

			Value = GetData();
		}

		private object GetData () {
			Type = (DataType) bytes[0];

			switch (Type) {
				case DataType.Int :
					return BitConverter.ToInt32(Bytes, 1);
				case DataType.Float :
					return BitConverter.ToDouble(Bytes, 1);
				case DataType.Bool :
					return BitConverter.ToBoolean(Bytes, 1);
				case DataType.String :
					int length = BitConverter.ToInt32(Bytes, 1);
					return Encoding.Unicode.GetString(Bytes, 1 + sizeof (int), length * sizeof (char));
			}

			return null;
		}

	}

	public bool IsEnd {
		get { return data.Count == 0; }
	}

	public byte[] Bytes {
		get {
			List<byte> bytes = new List<byte>();

			foreach (Datum datum in data) {
				bytes.AddRange(datum.Bytes);
			}

			return bytes.ToArray();
		}
	}

	public string StringFromBytes {
		get {
			BinaryFormatter bf = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			bf.Serialize(stream, Bytes);
			return Convert.ToBase64String(stream.ToArray());
		}
	}

	private readonly List<Datum> data;

	public DataBuffer () {
		data = new List<Datum>();
	}

	public DataBuffer (byte[] buffer) : this() {
		int offset = 0;

		while (offset < buffer.Length) {
			Datum datum = new Datum(buffer, offset);
			data.Add(datum);

			offset += 1 + datum.Size;
		}
	}

	public DataBuffer (string s)
		: this((byte[]) (new BinaryFormatter()).Deserialize(new MemoryStream(Convert.FromBase64String(s)))) {
	}

	public void Write (int value) {
		data.Add(new Datum(value));
	}

	public void Write (float value) {
		data.Add(new Datum(value));
	}

	public void Write (bool value) {
		data.Add(new Datum(value));
	}

	public void Write (string value) {
		data.Add(new Datum(value));
	}

	private void Write (object obj) {
		if (obj is int)
			Write((int) obj);
		else if (obj is float)
			Write((float) obj);
		else if (obj is bool)
			Write((bool) obj);
		else if (obj is string)
			Write(obj as string);
		else if (obj is ISerializable)
			Write(obj as ISerializable);
		else
			Debug.LogError(string.Format("Type '{0}' is not supported", obj.GetType()));
	}

	public void Write<T> (List<T> list) {
		Write(list.Count);

		for (int i = 0; i != list.Count; i++) {
			Write(list[i]);
		}
	}

	public void Write (ISerializable obj) {
		DataBuffer buffer = new DataBuffer();
		obj.Serialize(buffer);

		Write(buffer.StringFromBytes);
	}

	public object Read () {
		object obj = data[0].Value;
		data.RemoveAt(0);

		return obj;
	}

	public int ReadInt () {
		return (int) Read();
	}

	public float ReadFloat () {
		return (float) (double) Read();
	}

	public bool ReadBool () {
		return (bool) Read();
	}

	public string ReadString () {
		return (string) Read();
	}

	public List<T> ReadList<T> () {
		List<T> result = new List<T>();
		int count = ReadInt();

		for (int i = 0; i != count; i++) {
			result.Add((T) Read());
		}

		return result;
	}

	public delegate void BeforeDeserializationFunction (ISerializable obj);

	public List<T> ReadSerializableList<T> (BeforeDeserializationFunction foo = null) where T : ISerializable, new() {
		List<T> result = new List<T>();
		int count = ReadInt();

		for (int i = 0; i != count; i++) {
			result.Add(ReadSerializable<T>(foo));
		}

		return result;
	}

	public T ReadSerializable<T> (BeforeDeserializationFunction foo = null) where T : ISerializable, new() {
		DataBuffer buffer = new DataBuffer(ReadString());

		T result = new T();

		if (foo != null)
			foo(result);

		result.Deserialize(buffer);

		return result;
	}

}