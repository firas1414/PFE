using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// INHERITS FROM THE ENEMY CLASS, AND OVERRIDES THE AttackTarget METHOD
public class Spiter : Enemy
{
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] Transform launchPoint;

    Vector3 Destination;
    public override void AttackTarget(GameObject target)
    {
        Animator.SetTrigger("Attack");
        Destination = target.transform.position;
    }

    public void Shoot()
    {
        Projectile newProjectile = Instantiate(projectilePrefab, launchPoint.position, launchPoint.rotation);
        newProjectile.Launch(gameObject, Destination);
    }
}
