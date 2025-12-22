using Brio.Entities.Core;

namespace Brio.Xtension.ui;

public class RootEntityContainers {
    public Entity? Actors { get; init; }
    public Entity? Cameras { get; init; }
    public Entity? Environment { get; init; }

    public RootEntityContainers(Entity rootEntity){
        foreach(Entity ent in rootEntity.Children) {
            if(ent.FriendlyName == "Actors") {
                Actors = ent;
                Brio.Log.Info("XT: Found actors container.");
            }
            else if(ent.FriendlyName == "Cameras") {
                Cameras = ent;
                Brio.Log.Info("XT: Found cameras container.");
            }
            else if(ent.FriendlyName == "Environment") {
                Environment = ent;
                Brio.Log.Info("XT: Found environment container.");
            }
        }

        if(!Initialized()){
            Brio.Log.Error("XT: Failed to initialize root containers.");
        }
    }

    public bool Initialized(){
        return (Actors != null && Cameras != null && Environment != null);
    }
}
