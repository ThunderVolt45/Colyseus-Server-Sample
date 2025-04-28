using System;

[Serializable]
public partial class ColyseusRigidbody {
    public float mass = default;
    public float drag = default;
    public float angularDrag = default;
    public bool gravity = false;
    public bool kinematic = false;
	public ColyseusVector3 velocity = new ColyseusVector3();
	public ColyseusVector3 angularVelocity = new ColyseusVector3();
}
