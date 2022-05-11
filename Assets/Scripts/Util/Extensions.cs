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

        public static Vector3 Horizontal(this Vector3 self) => new Vector3(self.x, 0, self.z);

        public static Vector3Int Horizontal(this Vector3Int self) => new Vector3Int(self.x, 0, self.z);

        public static Vector3Int Copy(this Vector3Int self) => new Vector3Int(self.x, self.y, self.z);

        public static float GetMinValue(this Vector3 self) => Mathf.Min(Math.Abs(self.x), Math.Abs(self.y), Math.Abs(self.z));

        public static double Lerp(this double self, double target, double time) => (target - self) * time;

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

        public static Vector3Int Size(this Vector3Int[] self)
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
    }
}