using System;

[Serializable]
public partial class ColyseusTransform {
	public ColyseusVector3 position = new ColyseusVector3();
	public ColyseusQuaternion rotation = new ColyseusQuaternion();
	public ColyseusVector3 scale = new ColyseusVector3();
}

