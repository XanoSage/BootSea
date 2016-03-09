public interface ISerializable {
        void Serialize (DataBuffer buffer);
        void Deserialize (DataBuffer buffer);
}