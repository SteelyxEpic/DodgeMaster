using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Scriptable Objects/Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public string description;
    public int damage;
    public float range;
    public Vector2 rangeOffset;
    public Vector2 knockoutForce;
    public float attackSpeed = 1f;
    public float attackRate = 1f;
    public float stunDuration = 0.5f;
    [Header("Combo Settings")]
    public float[] comboValues = {0.5f};
    public float[] multiplierValues = {1f};
    [Header("Dash Settings")]
    public float dashDistance = 5f;
    public float Dtime = 0.1f;
    [Header("Smash Settings")]
    public float smashRadius = 3f;
    public float Stime = 0.5f;

    [Header("Overheat Settings")]
    public float overheatThreshold = 20f;
    public float overheatCoolingTime = 1f;
}
