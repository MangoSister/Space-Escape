using UnityEngine;
using System.Collections;

namespace PSMoveSharp {

	public static class PSMoveUtil {
	
		public static Vector3 Float4ToVector3(Float4 vec) {
			return new Vector3(vec.x, vec.y, vec.z);
		}
		
		public static Quaternion Float4ToQuaternion(Float4 vec) {
			return new Quaternion(vec.x, vec.y, vec.z, vec.w);
		}
		
		public static int GetHueFromColor(Color color) {
			float r, g, b;
			int h = 0;
			r = color.r;
			g = color.g;
			b = color.b;
			if(r >= g && g >= b) {
				h = (int)(60 * GetFraction(r,g,b));
			}
			else if(g > r && r >= b) {
				h = (int)(60 * (2-GetFraction(g,r,b)));
			}
			else if(g >= b && b > r) {
				h = (int)(60 * (2+GetFraction(g,b,r)));
			}
			else if(b > g && g > r) {
				h = (int)(60 * (4-GetFraction(b,g,r)));
			}
			else if(b > r && r >= g) {
				h = (int)(60 * (4+GetFraction(b,r,g)));
			}
			else if(r >= b && b > g) {
				h = (int)(60 * (6-GetFraction(r,b,g)));
			}
			return h;
		}
		
		private static float GetFraction(float h, float m, float l) {
			if(h == l) {
				return 0;
			}
			return (m-l)/(h-l);
		}
	}
}
