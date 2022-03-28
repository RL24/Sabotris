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
        
        public static float GetMinValue(this Vector3 self) => Mathf.Min(Math.Abs(self.x), Math.Abs(self.y), Math.Abs(self.z));

        public static double Lerp(this double self, double target, double time)
        {
            return (target - self) * time;
        }

        public static float Lerp(this float self, float target, float time)
        {
            return (target - self) * time;
        }

        public static int Int(this bool self)
        {
            return self ? 1 : 0;
        }

        public static CSteamID ToSteamID(this ulong self)
        {
            return new CSteamID(self);
        }

        public static bool IsLocalClient(this HSteamNetConnection self)
        {
            return self.m_HSteamNetConnection == 0;
        }

        public static void PlayModifiedSound(this AudioSource self, float? volume = null, float? pitch = null)
        {
            self.volume = volume ?? self.volume;
            self.pitch = pitch ?? self.pitch;
            self.Play();
        }

        public static float Delta(this float self)
        {
            return self * (Time.deltaTime * 50);
        }

        public static float FixedDelta(this float self)
        {
            return self * (Time.fixedDeltaTime * 50);
        }
    }
}