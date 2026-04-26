using System;
using UnityEngine;


public enum ComboStatus{None, Partial, Press};

public struct KeyCombo
{
    public KeyCode Primary;
    public KeyCode Secondary;

    public static KeyCombo None = new(KeyCode.None);

    public KeyCombo(KeyCode primary, KeyCode secondary = KeyCode.None)
    {
        if (secondary != KeyCode.None && (int)primary > (int)secondary)
        {
            Primary = secondary;
            Secondary = primary;
        }
        else
        {
            Primary = primary;
            Secondary = secondary;
        }
    }

    public readonly ComboStatus GetStatus()
    {
        bool p1 = Input.GetKey(Primary);
        if (Secondary == KeyCode.None)
        {
            return p1 ? ComboStatus.Press : ComboStatus.None;
        }

        bool p2 = Input.GetKey(Secondary);
        if (p1 && p2) return ComboStatus.Press;

        return (p1 || p2) ? ComboStatus.Partial : ComboStatus.None;
    }

    public readonly bool Equals(KeyCombo other)
    {
        return Primary == other.Primary && Secondary == other.Secondary;
    }
    
    public override readonly bool Equals(object obj)
    {
        return obj is KeyCombo other && Equals(other);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Primary,Secondary);
    }

    public static bool operator ==(KeyCombo left, KeyCombo right) => left.Equals(right);
    public static bool operator !=(KeyCombo left, KeyCombo right) => !left.Equals(right);

}

