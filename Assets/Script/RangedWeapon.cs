using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : MonoBehaviour {

    public Projectile projectile;

    public void Shoot(int _mAtk, int _MAtk)//기본에너미 
    {
        projectile.SetAtk(_mAtk, _MAtk);//프로젝타일이 데미지 세팅 하지않고 여기서 데미지를 받아감
        projectile.Shooter = GameManager.inst.player;
         projectile.collisionMask = LayerMask.GetMask("Enemy");
         Instantiate(projectile, transform.position, transform.rotation);
    }
    //아마 적만쓸 프로젝타일형태
    public void Shoot(Projectile _proj)
    {
        projectile = _proj;//프로젝타일대입
        projectile.collisionMask = UnityEngine.LayerMask.GetMask("Player") + LayerMask.GetMask("House");
        Instantiate(projectile, transform.position, transform.rotation);

    }

}
