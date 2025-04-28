using System;
using UnityEngine;

[Serializable]
public partial class ColyseusQuaternion {
	public int x = default;
	public int y = default;
	public int z = default;
	public int w = default;
	public int prec = 3;

	#region Initialize
	public ColyseusQuaternion(){}

	public ColyseusQuaternion(Quaternion quaternion, int precision = 3)
	{
		prec = precision;
		var pow = Mathf.Pow(10, prec);

		x = (int)(quaternion.x * pow);
		y = (int)(quaternion.y * pow);
		z = (int)(quaternion.z * pow);
		w = (int)(quaternion.w * pow);
	}
	#endregion

	#region Operator
	public Quaternion ToQuaternion()
	{
		var pow = Mathf.Pow(10, prec);
		return new Quaternion(x / pow, y / pow, z / pow, w / pow);
	}

	// public static explicit operator ColyseusQuaternion(Quaternion quaternion)
	// {
	// 	return new ColyseusQuaternion(quaternion);
	// }

	public static implicit operator Quaternion(ColyseusQuaternion colyseusQuaternion)
	{
		return colyseusQuaternion.ToQuaternion();
	}
	#endregion
}

