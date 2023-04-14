using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : DonDestory<ItemData>
{
    //FireBase 아이템 리스트
    [System.Serializable]
    public class Itemlist
    {
        public string[] Item;

        public Itemlist(string[] Item)
        {
            this.Item = Item;
        }
    }

    [Header("아이템 리스트")]
    [SerializeField]
    public Itemlist item;

    [SerializeField]
    public Sprite[] m_iconSprites;

    public void Itemlist_database()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance
     .GetReference("Item")
     .GetValueAsync().ContinueWithOnMainThread(task => {
         if (task.IsFaulted)
         {
             // Handle the error...
         }
         else if (task.IsCompleted)
         {
             DataSnapshot snapshot = task.Result;
             foreach (DataSnapshot data in snapshot.Children)
             {
                 string name = data.Key.ToString();
                 string myData = null;
                 if(name.Equals("Itemlist"))
                 {
                     myData = data.GetRawJsonValue();
                     item = JsonUtility.FromJson<Itemlist>(myData);
                 }
             }
         }

     });
    }

    // Start is called before the first frame update
    void Start()
    {
        Itemlist_database();
    }

}
