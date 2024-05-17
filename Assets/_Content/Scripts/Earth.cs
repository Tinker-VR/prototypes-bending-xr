using UnityEngine;

public enum EarthType
{
    Rock,
    Ground
}

public class Earth : MonoBehaviour
{
    public EarthType earthType;

    // Common functionality for both Rock and Ground can go here
}
