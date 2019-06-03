namespace Models {
    public class Class {
        public ClassInfo Info { get; private set; }

        public MoveConsumption MoveConsumption {
            get {
                RoleModel model = ModelManager.models.Get<RoleModel>();
                return model.GetOrCreateMoveConsumption(Info.classType);
            }
        }

        public Class(ClassInfo info) {
            Info = info;
        }
    }
}
