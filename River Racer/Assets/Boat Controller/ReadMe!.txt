Note:
Boat prefab have 2 child objects (Boat(Body) and 
Water Surface Splash) which can be replaced with you own model/particle system.
Water area is an object with box collider attached, so boat script know where is "floatable" zone.
WaterArea child object called (MeshIndicator) used just for visually indicate and area, it can be 
deactivated after placing prefab in scene.
Example scene shows how to setup boat and WaterArea