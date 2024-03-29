﻿using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace AdventOfCSharp;

public struct ProblemDate : IEquatable<ProblemDate>, IComparable<ProblemDate>
{
    private const int dayBitMaskBits = 5;

    private const uint dayBitMask = (1U << dayBitMaskBits) - 1;
    private const uint yearBitMask = ~dayBitMask;

    private uint bits;

    public int Year
    {
        get => (int)((bits & yearBitMask) >> dayBitMaskBits);
        set
        {
            if (value < 2015)
                throw new ArgumentException("The year must be at least 2015.");

            bits = (bits & ~yearBitMask) | ((uint)value << dayBitMaskBits);
        }
    }
    public int Day
    {
        get => (int)(bits & dayBitMask);
        set
        {
            if (value is < 1 or > 25)
                throw new ArgumentException("The day must be between 1 and 25.");

            bits = (bits & ~dayBitMask) | (uint)value;
        }
    }

    public ProblemDate(int year, int day)
        : this()
    {
        Year = year;
        Day = day;
    }

    public static ProblemDate[] Dates(int year, IEnumerable<int> days) => Dates(year, days.ToArray());
    public static ProblemDate[] Dates(int year, params int[] days)
    {
        var result = new ProblemDate[days.Length];
        if (days.Length is 0)
            return result;

        for (int i = 0; i < days.Length; i++)
            result[i] = new(year, days[i]);

        return result;
    }

    public void Deconstruct(out int year, out int day)
    {
        year = Year;
        day = Day;
    }

    // Nice fucking boilerplate
    public static bool operator ==(ProblemDate left, ProblemDate right) => left.bits == right.bits;
    public static bool operator !=(ProblemDate left, ProblemDate right) => left.bits != right.bits;
    public static bool operator <(ProblemDate left, ProblemDate right) => left.bits < right.bits;
    public static bool operator <=(ProblemDate left, ProblemDate right) => left.bits <= right.bits;
    public static bool operator >(ProblemDate left, ProblemDate right) => left.bits > right.bits;
    public static bool operator >=(ProblemDate left, ProblemDate right) => left.bits >= right.bits;

    public int CompareTo(ProblemDate other) => bits.CompareTo(other.bits);
    public bool Equals(ProblemDate other) => bits == other.bits;
    public override bool Equals(object? obj) => obj is ProblemDate date && Equals(date);
    public override int GetHashCode() => (int)bits;

    public override string ToString() => $"{Year}/{Day:D2}";
}
