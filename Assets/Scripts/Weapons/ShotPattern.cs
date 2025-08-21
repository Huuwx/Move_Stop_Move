using System.Collections.Generic;
using UnityEngine;

namespace ZombieCity.Abilities
{
    public interface IShotPattern
    {
        // Trả về các hướng bắn cho 1 lần bắn, dựa vào hướng forward hiện tại
        List<Vector2> GetDirections(Vector2 forward);
        string Id { get; }
    }

    public class BasicForwardPattern : IShotPattern
    {
        public string Id => "basic";
        public List<Vector2> GetDirections(Vector2 forward) => new() { forward.normalized };
    }

    /// Decorator: thêm hướng phụ
    public abstract class PatternDecorator : IShotPattern
    {
        protected readonly IShotPattern inner;
        protected PatternDecorator(IShotPattern inner) => this.inner = inner;
        public abstract string Id { get; }
        public abstract List<Vector2> GetDirections(Vector2 forward);
    }

    public class AddBackPattern : PatternDecorator
    {
        public AddBackPattern(IShotPattern inner) : base(inner) { }
        public override string Id => "back";
        public override List<Vector2> GetDirections(Vector2 f)
        {
            var dirs = inner.GetDirections(f);
            dirs.Add(-f.normalized);
            return dirs;
        }
    }

    public class AddDiagonalPattern : PatternDecorator
    {
        public AddDiagonalPattern(IShotPattern inner) : base(inner) { }
        public override string Id => "diag";
        public override List<Vector2> GetDirections(Vector2 f)
        {
            var dirs = inner.GetDirections(f);
            var right = new Vector2(f.y, -f.x).normalized;
            dirs.Add((f + right).normalized);
            dirs.Add((f - right).normalized);
            return dirs;
        }
    }

    public class FanExtraProjectilesPattern : PatternDecorator
    {
        private readonly int extra;         // số mũi phụ
        private readonly float spreadDeg;   // góc mở quạt
        public FanExtraProjectilesPattern(IShotPattern inner, int extra, float spreadDeg = 30f) : base(inner)
        { this.extra = extra; this.spreadDeg = spreadDeg; }

        public override string Id => $"fan+{extra}";
        public override List<Vector2> GetDirections(Vector2 f)
        {
            var dirs = inner.GetDirections(f);
            if (extra <= 0) return dirs;

            float step = spreadDeg / (extra + 1);
            float start = -spreadDeg * 0.5f;
            var baseAngle = Mathf.Atan2(f.y, f.x) * Mathf.Rad2Deg;

            for (int i = 1; i <= extra; i++)
            {
                float ang = baseAngle + start + step * i;
                float rad = ang * Mathf.Deg2Rad;
                dirs.Add(new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)));
            }
            return dirs;
        }
    }
}
