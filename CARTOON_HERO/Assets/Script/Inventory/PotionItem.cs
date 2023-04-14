using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionItem : GameItem
{
    PlayerController player;
    public override int usebtn_check()
    {
        use_check = 1;
        return use_check;
    }
    public void Item_effect(int Icode)
    {
        if (Icode == 2)
        {
            if (player.playInfo.hp >= player.playInfo.hpMax)
            {
                int HPMAX = player.playInfo.hpMax;
                player.playInfo.hp = HPMAX;
            }
            else
            {
                player.playInfo.hp += 20;
            }
        }
        else if (Icode == 3)
        {
            if (player.playInfo.hp >= player.playInfo.hpMax)
            {
                int HPMAX = player.playInfo.hpMax;
                player.playInfo.hp = HPMAX;
            }
            else
            {
                player.playInfo.hp += 40;
            }
        }
        player.m_hudCtr.Potion_HPReset(player.playInfo.hp);
    }

    public void playerSelect()
    {
    
    }
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
