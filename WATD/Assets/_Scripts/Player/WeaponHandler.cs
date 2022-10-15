using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [field: SerializeField] public List<Weapon> Weapons;
    [field: SerializeField] public Weapon currentWeapon;
    public int attackIndex { get; private set; }
    private float resetTimer;

    void Start()
    {
        if (Weapons.Count > 0)
        {
            currentWeapon = Weapons[0];
        }
    }

    private void Update()
    {
        if (attackIndex > 0)
        {
            resetTimer -= Time.deltaTime;
            if (resetTimer <= 0)
            {
                attackIndex = 0;
            }
        }
    }

    public void SetAttackId(int index, float resetTime)
    {
        attackIndex = index;
        resetTimer = resetTime;
    }

    public void IncrementAttackIndex()
    {
        if (attackIndex >= currentWeapon.weaponData.AttackAnimations.Count)
        {
            attackIndex = 0;
            return;
        }
        attackIndex++;
        // Uncomment this line to make attack index reset after 0.25f
        //SetAttackId(attackIndex, 0.25f);
    }
}
