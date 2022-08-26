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

    // PotionItem 이면 아이템 사용시 회복 가능
    public void Item_effect(int Icode)
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        if (Icode == 2)
        {
            if (player.playInfo.hp == player.playInfo.hpMax)
            {
                player.playInfo.hp += 0;
            }
            else
            {
                player.playInfo.hp += 20;
                if (player.playInfo.hp >= player.playInfo.hpMax)
                    player.playInfo.hp = player.playInfo.hpMax;
            }
        }
        else if (Icode == 3)
            player.playInfo.hp += 40;
    }

    public void playerSelect()
    {
    
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
