// HelperFuncs.cs
// Single-file, dependency-free C# helpers. Compatible with C# 7.3+ and .NET Standard 2.0+.
// Import the shortcuts with: using static MiniHelpers.HelperFuncs;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiniHelpers
{
    public static class HelperFuncs
    {
        private static readonly Random Rng = new Random();
        private static readonly object RngLock = new object();

        // ---- Console (use in console apps; not a substitute for Unity's Debug.Log) ----

        public static void P(object value = null) { Console.WriteLine(value); }
        public static void Print(object value = null) { P(value); }
        public static void W(object value) { Console.Write(value); }
        public static void Write(object value) { W(value); }

        // Returns an empty string if the input stream reaches its end.
        public static string Input(string prompt = "")
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? string.Empty;
        }

        // Invalid input produces fallback rather than crashing or looping forever.
        public static int AskInt(string prompt = "", int fallback = 0)
        {
            return ParseIntOr(Input(prompt), fallback);
        }

        public static double AskDouble(string prompt = "", double fallback = 0)
        {
            return ParseDoubleOr(Input(prompt), fallback);
        }

        public static void Log(object value)
        {
            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + value);
        }

        // ---- Numbers ----

        public static int Clamp(int value, int min, int max)
        {
            if (min > max) throw new ArgumentException("min cannot exceed max.");
            return Math.Max(min, Math.Min(max, value));
        }

        public static float Clamp(float value, float min, float max)
        {
            if (min > max) throw new ArgumentException("min cannot exceed max.");
            return Math.Max(min, Math.Min(max, value));
        }

        public static double Clamp(double value, double min, double max)
        {
            if (min > max) throw new ArgumentException("min cannot exceed max.");
            return Math.Max(min, Math.Min(max, value));
        }

        // t=0 returns a; t=1 returns b. Other t values extrapolate.
        public static double Lerp(double a, double b, double t) { return a + (b - a) * t; }
        public static float Lerp(float a, float b, float t) { return a + (b - a) * t; }

        // Maps value from one numerical range to another (without clamping).
        public static double Map(double value, double fromMin, double fromMax,
                                 double toMin, double toMax)
        {
            if (fromMin == fromMax) throw new ArgumentException("Source range cannot have zero width.");
            return toMin + (value - fromMin) / (fromMax - fromMin) * (toMax - toMin);
        }

        public static bool Near(double a, double b, double tolerance = 1e-9)
        {
            if (double.IsNaN(tolerance) || tolerance < 0)
                throw new ArgumentOutOfRangeException(nameof(tolerance));
            return a == b || Math.Abs(a - b) <= tolerance;
        }

        // ---- Random (pseudo-random; NOT suitable for passwords or security) ----

        // maxExclusive is never returned.
        public static int RandInt(int minInclusive, int maxExclusive)
        {
            if (minInclusive >= maxExclusive)
                throw new ArgumentException("minInclusive must be less than maxExclusive.");
            lock (RngLock) { return Rng.Next(minInclusive, maxExclusive); }
        }

        public static double RandDouble(double minInclusive = 0, double maxExclusive = 1)
        {
            if (double.IsNaN(minInclusive) || double.IsInfinity(minInclusive) ||
                double.IsNaN(maxExclusive) || double.IsInfinity(maxExclusive) ||
                minInclusive >= maxExclusive)
                throw new ArgumentException("Provide a valid, finite, increasing range.");
            double width = maxExclusive - minInclusive;
            if (double.IsInfinity(width))
                throw new ArgumentException("Range width is too large.");
            lock (RngLock) { return minInclusive + Rng.NextDouble() * width; }
        }

        // probability: 0.0 = never; 1.0 = always.
        public static bool Chance(double probability)
        {
            if (double.IsNaN(probability) || probability < 0 || probability > 1)
                throw new ArgumentOutOfRangeException(nameof(probability));
            return RandDouble() < probability;
        }

        public static T Pick<T>(IList<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (items.Count == 0) throw new ArgumentException("Cannot pick from an empty collection.", nameof(items));
            return items[RandInt(0, items.Count)];
        }

        // Fisher-Yates shuffle; modifies the list in place.
        public static void Shuffle<T>(IList<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            for (int i = items.Count - 1; i > 0; i--)
            {
                int j = RandInt(0, i + 1);
                T temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
        }

        // ---- Text and parsing ----

        public static bool Blank(string text) { return string.IsNullOrWhiteSpace(text); }

        public static bool ContainsI(string text, string part)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (part == null) throw new ArgumentNullException(nameof(part));
            return text.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string Cap(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return char.ToUpperInvariant(text[0]) + text.Substring(1);
        }

        public static string Repeat(string text, int count)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < count; i++) result.Append(text);
            return result.ToString();
        }

        public static string Truncate(string text, int maxLength, string suffix = "...")
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (suffix == null) throw new ArgumentNullException(nameof(suffix));
            if (maxLength < 0) throw new ArgumentOutOfRangeException(nameof(maxLength));
            if (text.Length <= maxLength) return text;
            if (suffix.Length >= maxLength) return suffix.Substring(0, maxLength);
            return text.Substring(0, maxLength - suffix.Length) + suffix;
        }

        public static string Join<T>(IEnumerable<T> items, string separator = ", ")
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            return string.Join(separator, items);
        }

        // Uses invariant culture (e.g., decimal point is '.') for predictable results.
        public static int ParseIntOr(string text, int fallback = 0)
        {
            int value;
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value)
                ? value : fallback;
        }

        public static double ParseDoubleOr(string text, double fallback = 0)
        {
            double value;
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                ? value : fallback;
        }

        // ---- Collections and loops ----

        public static bool HasIndex<T>(IList<T> items, int index)
        {
            return items != null && index >= 0 && index < items.Count;
        }

        public static T GetOr<T>(IList<T> items, int index, T fallback = default(T))
        {
            return HasIndex(items, index) ? items[index] : fallback;
        }

        public static TValue GetOr<TKey, TValue>(IDictionary<TKey, TValue> map,
                                                 TKey key, TValue fallback = default(TValue))
        {
            if (map == null) throw new ArgumentNullException(nameof(map));
            TValue value;
            return map.TryGetValue(key, out value) ? value : fallback;
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        // Range(5): 0..4; Range(2, 5): 2..4 (end exclusive).
        public static IEnumerable<int> Range(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            return Range(0, count);
        }

        public static IEnumerable<int> Range(int startInclusive, int endExclusive)
        {
            if (startInclusive > endExclusive)
                throw new ArgumentException("startInclusive must be <= endExclusive.");
            return RangeIterator(startInclusive, endExclusive);
        }

        private static IEnumerable<int> RangeIterator(int startInclusive, int endExclusive)
        {
            for (int i = startInclusive; i < endExclusive; i++) yield return i;
        }

        public static void Times(int count, Action<int> action)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (action == null) throw new ArgumentNullException(nameof(action));
            for (int i = 0; i < count; i++) action(i);
        }

        // ---- File utilities (normal .NET file-system access required) ----

        public static bool FileExists(string path) { return File.Exists(path); }
        public static string ReadText(string path) { return File.ReadAllText(path); }
        public static string[] ReadLines(string path) { return File.ReadAllLines(path); }
        public static void WriteText(string path, string text) { File.WriteAllText(path, text); }
        public static void AppendText(string path, string text) { File.AppendAllText(path, text); }
        public static void EnsureDir(string path) { Directory.CreateDirectory(path); }

        // ---- Time ----

        // Blocks the current thread. For async code, use Delay instead.
        public static void Sleep(int milliseconds)
        {
            if (milliseconds < 0) throw new ArgumentOutOfRangeException(nameof(milliseconds));
            Thread.Sleep(milliseconds);
        }

        public static Task Delay(int milliseconds)
        {
            if (milliseconds < 0) throw new ArgumentOutOfRangeException(nameof(milliseconds));
            return Task.Delay(milliseconds);
        }

        public static TimeSpan Measure(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            Stopwatch watch = Stopwatch.StartNew();
            action();
            watch.Stop();
            return watch.Elapsed;
        }
    }
}
