using UnityEngine;

namespace Framework.Extensions
{
	public static class Vector3Extensions
	{
		public static Vector3 RotateAround(this Vector3 point, Vector3 pivot, Quaternion rotation)
		{
			var direction = point - pivot;
			direction = rotation * direction;
			return direction + pivot;
		}
	}
}
