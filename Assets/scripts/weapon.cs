using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Scriptable Objects/Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public string description;
    public int damage;
    public float range;
    public float attackSpeed = 1f;
    public float attackRate = 1f;
    [Header("Overheat Settings")]
    public float overheatThreshold = 20f;
    public float overheatCoolingTime = 1f;
}
