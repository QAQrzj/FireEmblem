using System;

namespace Models {
    public interface IEditorConfigSerializer {
        Array EditorGetKeys();
        void EditorSortDatas();
        byte[] EditorSerializeToBytes();
        void EditorDeserializeToObject(byte[] bytes);
    }
}
