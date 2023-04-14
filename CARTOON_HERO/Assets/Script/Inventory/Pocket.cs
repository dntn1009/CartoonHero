using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pocket : MonoBehaviour
{
    [System.Serializable]
    public class Mon_Pocket
    {
       public int Slime_Count = 0;
       public int Turtle_Count = 0;

        public Mon_Pocket()
        {
             
        }
        public Mon_Pocket(int Slime_Count, int Turtle_Count)
        {
            this.Slime_Count = Slime_Count;
            this.Turtle_Count = Turtle_Count;
        }

        public void SetSlime()
        {
            this.Slime_Count = this.Slime_Count + 1;
        }

        public void SetTurtle()
        {
            this.Turtle_Count = this.Turtle_Count + 1;
        }

        public void SlimeZero()
        {
            this.Slime_Count = 0;
        }
        public void TurtleZero()
        {
            this.Turtle_Count = 0;
        }
    }

    [System.Serializable]
    public class Shop_Pocket
    {
        public int A_Count = 0;
        public int B_Count = 0;

        public Shop_Pocket()
        {

        }
        public Shop_Pocket(int A_Count, int B_Count)
        {
            this.A_Count = A_Count;
            this.B_Count = B_Count;
        }

        public void SetA(int count)
        {
            this.A_Count = count;
        }

        public void SetB(int count)
        {
            this.B_Count = count;
        }

        public void AZero()
        {
            this.A_Count = 0;
        }
        public void BZero()
        {
            this.B_Count = 0;
        }
    }

    public bool Pocket_check = false;
    public bool Pocket_Shop = false;

    [SerializeField]
    public Mon_Pocket mon_pocket;

    [SerializeField]
    public Shop_Pocket shop_pocket;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
