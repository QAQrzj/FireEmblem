namespace Models {
    public class Character {
        public CharacterInfo Info { get; private set; }

        public Character(CharacterInfo info) {
            Info = info;
        }
    }
}
