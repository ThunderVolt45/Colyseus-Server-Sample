using System;
using UnityEngine;

[Serializable]
public partial class ColyseusVector3 {
	public long x = default;
	public long y = default;
	public long z = default;
	public int prec = 3;

	#region Initialize
	public ColyseusVector3(){}

	public ColyseusVector3(Vector3 vector3, int precision = 3)
	{
		prec = precision;
		var pow = Mathf.Pow(10, prec);

		x = (long)(vector3.x * pow);
		y = (long)(vector3.y * pow);
		z = (long)(vector3.z * pow);
	}
	#endregion

	#region Operator
	public Vector3 ToVector3()
	{
		var pow = Mathf.Pow(10, prec);
		return new Vector3(x / pow, y / pow, z / pow);
	}

	// public static explicit operator ColyseusVector3(Vector3 vector3)
	// {
	// 	return new ColyseusVector3(vector3);
	// }

	public static implicit operator Vector3(ColyseusVector3 colyseusVector3)
	{
		return colyseusVector3.ToVector3();
	}
	#endregion
}

