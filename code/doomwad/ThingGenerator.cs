using Sandbox;

public class ThingGenerator {
    public static void GenerateThings(){
        foreach(var thing in MapLoader.things){
            GenerateThing(thing);
        }
    }

    public static void GenerateThing(Thing thing){
        thing.doesRespawn = true;
        thing.awaitingRespawn = false;
        if((thing.flags & 0x004) == 0){
            thing.doesRespawn = false;
            return; // Doesn't show up on Ultra Violence.
        }
        if(AnimatedThings.animatedThings.ContainsKey(thing.thingType)){
            var ob=new ObstacleThing(){
                Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                Rotation = Rotation.FromYaw(thing.facing),
                decoration = AnimatedThings.animatedThings[thing.thingType]
            };
            ob.SpriteName = ob.decoration.Name + ob.decoration.Sequence[0] + "0";
            ob.SetupPhysicsFromAABB(ob.decoration.Class.Contains('*')?PhysicsMotionType.Dynamic:PhysicsMotionType.Static, new Vector3(-ob.Radius, -ob.Radius, 0) * WadLoader.mapScale, new Vector3(ob.Radius, ob.Radius, ob.Height) * WadLoader.mapScale);
            if(!ob.decoration.Class.Contains('O')){
                ob.CollisionGroup = CollisionGroup.Never;
            }
            thing.ent = ob;
            return;
        }
        switch(thing.thingType){
            case 1: case 2: case 3: case 4: case 11: // Player spawns
                thing.doesRespawn = false;
                break;
            // Monsters
            case 0x009:
                if(DoomGame.DEATHMATCH)break;
                thing.ent=new ShotgunGuy(){
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing),
                    isAmbusher = (thing.flags & 0x08) > 0
                };
                break;
            case 0xBB9:
                if(DoomGame.DEATHMATCH)break;
                thing.ent=new Imp(){
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing),
                    isAmbusher = (thing.flags & 0x08) > 0
                };
                break;
            case 0xBBC:
                if(DoomGame.DEATHMATCH)break;
                thing.ent=new ZombieMan(){
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing),
                    isAmbusher = (thing.flags & 0x08) > 0
                };
                break;                
            // Pickups
            case 0x5: // BlueKeycard
                thing.ent=new BlueKeycard(){
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing)
                };
                break;
            case 0xD: // RedKeycard
                thing.ent=new RedKeycard(){
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing)
                };
                break;
            case 0x6: // YellowKeycard
                thing.ent=new YellowKeycard(){
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing)
                };
                break;
            case 0x7D1: // Shotgun
                thing.ent=new ShotgunPickup(){
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing)
                };
                break;
            case 0x7D2: // Chaingun
                thing.ent=new ChaingunPickup(){
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing)
                };
                break;
            case 0x7D7: // Clip
                thing.ent=new ClipPickup(){
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing)
                };
                break;
            case 0x7D8: // Shells
                thing.ent=new ShellPickup(){
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing)
                };
                break;
            case 0x7DB: // Stimpack
                thing.ent=new Stimpack(){
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing)
                };
                break;
            case 0x7DC: // Medkit
                thing.ent=new Medkit(){
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing)
                };
                break;
            case 0x7DE: // Health Bonus
                thing.ent=new HealthBonus(){ 
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing)
                };
                break;
            case 0x7DF: // Armor Bonus
                thing.ent=new ArmorBonus(){ 
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing)
                };
                break;
            case 0x7E2: // Security Armor
                thing.ent=new SecurityArmor(){ 
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing)
                };
                break;
            case 0x7E3: // Mega Armor
                thing.ent=new MegaArmor(){ 
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing)
                };
                break;
            case 0x7F3: // Barrel
                thing.ent=new Barrel(){ 
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing)
                };
                break;
            case 0x800: // Box of Clip ammo
                thing.ent=new BoxClipPickup(){ 
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing)
                };
                break;
            case 0x801: // Box of Shotgun Shells
                thing.ent=new BoxShellPickup(){ 
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing)
                };
                break;
            default:
                thing.doesRespawn = false;
                Log.Info($"Unknown thing with ID: {thing.thingType} ({thing.thingType:%x})");
                return;
        }
    }

    public static void CheckItemRespawns(){
        foreach(Thing t in MapLoader.things){
            if(t.doesRespawn && (t.ent==null || !t.ent.IsValid)){
                if(!t.awaitingRespawn){
                    t.awaitingRespawn = true;
                    t.respawnTime = -(8f+(DoomGame.RNG.NextSingle() * 292f));
                }else if(t.respawnTime >= 0f){ // Respawn this.
                    GenerateThing(t);
                    // TODO: Respawn flash.
                    if(t.ent!=null && t.ent.IsValid){
                        SoundLoader.PlaySound("DSITMBK", t.ent.CollisionWorldSpaceCenter);
                        _=new ItemRespawnFlash(){
                            Position = t.ent.CollisionWorldSpaceCenter
                        };
                    }
                }
            }
        }
    }
}