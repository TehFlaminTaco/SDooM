public class ThingGenerator {
    public static void GenerateThings(){
        foreach(var thing in MapLoader.things){
            if((thing.flags & (0x001 | 0x002 | 0x004)) == 0) return; // Doesn't show up on Ultra Violence.
            if(AnimatedThings.animatedThings.ContainsKey(thing.thingType)){
                var ob=new ObstacleThing(){
                    Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                    Rotation = Rotation.FromYaw(thing.facing),
                    decoration = AnimatedThings.animatedThings[thing.thingType]
                };
                ob.SpriteName = ob.decoration.Name + ob.decoration.Sequence[0] + "0";
                ob.SetupPhysicsFromAABB(ob.decoration.Class.Contains('*')?Sandbox.PhysicsMotionType.Dynamic:Sandbox.PhysicsMotionType.Static, new Vector3(-ob.Radius, -ob.Radius, 0) * WadLoader.mapScale, new Vector3(ob.Radius, ob.Radius, ob.Height) * WadLoader.mapScale);
                if(!ob.decoration.Class.Contains('O')){
                    ob.CollisionGroup = Sandbox.CollisionGroup.Never;
                }
                continue;
            }
            switch(thing.thingType){
                case 1: case 2: case 3: case 4: case 11: // Player spawns
                    break;
                // Monsters
                case 0x009:
                    _=new ShotgunGuy(){
                        Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                        Rotation = Rotation.FromYaw(thing.facing),
                        isAmbusher = (thing.flags & 0x08) > 0
                    };
                    break;
                case 0xBB9:
                    _=new Imp(){
                        Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                        Rotation = Rotation.FromYaw(thing.facing),
                        isAmbusher = (thing.flags & 0x08) > 0
                    };
                    break;
                case 0xBBC:
                    _=new ZombieMan(){
                        Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                        Rotation = Rotation.FromYaw(thing.facing),
                        isAmbusher = (thing.flags & 0x08) > 0
                    };
                    break;                
                // Pickups
                case 0x7D1: // Shotgun
                    _=new ShotgunPickup(){
                        Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                        Rotation = Rotation.FromYaw(thing.facing)
                    };
                    break;
                case 0x7D2: // Chaingun
                    _=new ChaingunPickup(){
                        Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                        Rotation = Rotation.FromYaw(thing.facing)
                    };
                    break;
                case 0x7D7: // Clip
                    _=new ClipPickup(){
                        Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                        Rotation = Rotation.FromYaw(thing.facing)
                    };
                    break;
                case 0x7D8: // Shells
                    _=new ShellPickup(){
                        Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                        Rotation = Rotation.FromYaw(thing.facing)
                    };
                    break;
                case 0x7DB: // Stimpack
                    _=new Stimpack(){
                        Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                        Rotation = Rotation.FromYaw(thing.facing)
                    };
                    break;
                case 0x7DC: // Medkit
                    _=new Medkit(){
                        Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                        Rotation = Rotation.FromYaw(thing.facing)
                    };
                    break;
                case 0x7DE: // Health Bonus
                    _=new HealthBonus(){ 
                        Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                        Rotation = Rotation.FromYaw(thing.facing)
                    };
                    break;
                case 0x7DF: // Armor Bonus
                    _=new ArmorBonus(){ 
                        Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                        Rotation = Rotation.FromYaw(thing.facing)
                    };
                    break;
                case 0x7E2: // Security Armor
                    _=new SecurityArmor(){ 
                        Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                        Rotation = Rotation.FromYaw(thing.facing)
                    };
                    break;
                case 0x7E3: // Mega Armor
                    _=new MegaArmor(){ 
                        Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                        Rotation = Rotation.FromYaw(thing.facing)
                    };
                    break;
                case 0x7F3: // Barrel
                    _=new Barrel(){ 
                        Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                        Rotation = Rotation.FromYaw(thing.facing)
                    };
                    break;
                case 0x800: // Box of Clip ammo
                    _=new BoxClipPickup(){ 
                        Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                        Rotation = Rotation.FromYaw(thing.facing)
                    };
                    break;
                case 0x801: // Box of Shotgun Shells
                    _=new BoxShellPickup(){ 
                        Position = DoomMap.ToMapLocation(new Vector2(thing.posX, thing.posY)),
                        Rotation = Rotation.FromYaw(thing.facing)
                    };
                    break;
                default:
                    Log.Info($"Unknown thing with ID: {thing.thingType} ({thing.thingType:%x})");
                    continue;
            }
        }
    }
}