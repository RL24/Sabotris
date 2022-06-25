using System;
using Steamworks;
using UnityEngine;

namespace Sabotris.Util
{
    public static class Extensions
    {
        private const float Tolerance = 0.1f;

        private static bool IsLessThan(this Vector3Int self, Vector3Int pos) => self.x < pos.x || self.y < pos.y || self.z < pos.z;

        private static bool IsGreaterThan(this Vector3Int self, Vector3Int pos) => self.x > pos.x || self.y > pos.y || self.z > pos.z;

        public static bool IsOutside(this Vector3Int self, Vector3Int bottomLeft, Vector3Int topRight) => self.IsLessThan(bottomLeft) || self.IsGreaterThan(topRight);

        public static Vector3Int Round(this Vector3 self, int round)
        {
            var num = 1.0 / round;
            return new Vector3Int(
                (int) (Math.Round(self.x * num) * round),
                (int) (Math.Round(self.y * num) * round),
                (int) (Math.Round(self.z * num) * round)
            );
        }

        public static Vector3 Horizontal(this Vector3 self, bool normalized)
        {
            var vec = new Vector3(self.x, 0, self.z);
            if (normalized)
                vec.Normalize();
            return vec.normalized;
        }
        
        public static Vector3Int Horizontal(this Vector3Int self) => new Vector3Int(self.x, 0, self.z);

        public static bool Same(this float self, float compare, float tolerance = Tolerance) => Math.Abs(self - compare) <= tolerance;

        public static bool Same(this Vector3 self, Vector3 compare, float tolerance = Tolerance) => self.x.Same(compare.x) && self.y.Same(compare.y) && self.z.Same(compare.z);

        public static bool Same(this Vector3Int self, Vector3Int compare) => self.x == compare.x && self.y == compare.y && self.z == compare.z;

        public static bool Same(this Vector3Int[] self, Vector3Int[] compare)
        {
            var allMatches = true;
            foreach (var s in self)
            {
                var isMatch = false;
                foreach (var c in compare)
                    if (s.Same(c))
                        isMatch = true;
                if (!isMatch)
                    allMatches = false;
            }

            return allMatches;
        }

        public static Vector3Int Copy(this Vector3Int self) => new Vector3Int(self.x, self.y, self.z);

        public static float GetMinValue(this Vector3 self) => Mathf.Min(Math.Abs(self.x), Math.Abs(self.y), Math.Abs(self.z));

        public static float Lerp(this float self, float target, float time) => (target - self) * time;

        public static int Int(this bool self) => self ? 1 : 0;

        public static CSteamID ToSteamID(this ulong self) => new CSteamID(self);

        public static bool IsLocalClient(this HSteamNetConnection self) => self.m_HSteamNetConnection == 0;

        public static void PlayModifiedSound(this AudioSource self, float? volume = null, float? pitch = null)
        {
            self.volume = volume ?? self.volume;
            self.pitch = pitch ?? self.pitch;
            self.Play();
        }

        public static Vector3Int[] RelativeTo(this Vector3Int[] self, Vector3Int absolute)
        {
            var relative = new Vector3Int[self.Length];
            for (var i = 0; i < self.Length; i++)
                relative[i] = self[i] + absolute;
            return relative;
        }

        public static Vector3 MinVec(this Vector3[] self)
        {
            var value = self[0];
            foreach (var vec in self)
                if (vec.x < value.x || vec.y < value.y || vec.z < value.z)
                    value = vec;
            return value;
        }

        public static Vector3 MaxVec(this Vector3[] self)
        {
            var value = self[0];
            foreach (var vec in self)
                if (vec.x > value.x || vec.y > value.y || vec.z > value.z)
                    value = vec;
            return value;
        }

        public static Vector3Int MinVec(this Vector3Int[] self)
        {
            var value = self[0];
            foreach (var vec in self)
                if (vec.x < value.x || vec.y < value.y || vec.z < value.z)
                    value = vec;
            return value;
        }

        public static Vector3Int MaxVec(this Vector3Int[] self)
        {
            var value = self[0];
            foreach (var vec in self)
                if (vec.x > value.x || vec.y > value.y || vec.z > value.z)
                    value = vec;
            return value;
        }

        public static (Vector3Int, Vector3Int) MinMax(this Vector3Int[] self)
        {
            var minX = self[0].x;
            var minY = self[0].y;
            var minZ = self[0].z;

            var maxX = self[0].x;
            var maxY = self[0].y;
            var maxZ = self[0].z;
            
            foreach (var vec in self)
            {
                minX = Math.Min(minX, vec.x);
                minY = Math.Min(minY, vec.y);
                minZ = Math.Min(minZ, vec.z);
                
                maxX = Math.Max(maxX, vec.x);
                maxY = Math.Max(maxY, vec.y);
                maxZ = Math.Max(maxZ, vec.z);
            }

            return (new Vector3Int(minX, minY, minZ), new Vector3Int(maxX, maxY, maxZ));
        }
        
        public static Vector3Int Height(this Vector3Int[] self)
        {
            var lowest = self[0];
            var highest = self[0];
            foreach (var vec in self)
            {
                if (vec.y < lowest.y)
                    lowest = vec;
                if (vec.y > highest.y)
                    highest = vec;
            }

            return highest - lowest;
        }

        public static float Delta(this float self) => self * (Time.deltaTime * 50);

        public static float FixedDelta(this float self) => self * (Time.fixedDeltaTime * 50);

        public static Quaternion Yaw(this Quaternion self) => Quaternion.Euler(self.eulerAngles.Round(1) * Vector3Int.up);
        
        public static float Pitch(this Quaternion self) => RadiansToDegrees(Mathf.Atan2(2 * self.x * self.w - 2 * self.y * self.z, 1 - 2 * self.x * self.x - 2 * self.z * self.z));

        public static (float, float) AngleToVec(this float angle)
        {
            var radians = DegreesToRadians(-angle);
            return ((float) Math.Cos(radians), (float) Math.Sin(radians));
        }
        
        private static float DegreesToRadians(float radians) => (float) (radians * Math.PI / 180f);
        
        private static float RadiansToDegrees(float radians) => (float) (radians / Math.PI * 180f);
    }
}