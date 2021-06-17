using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPopUP : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TouchUpPopUp()
    {
        var nav = MainGameManager.mainGameManager.RightNav.GetComponent<RightNav>();
        nav.OpenNav();
        nav.ResetPopUp(1);
    }
    public void BulidingUpPopUp()
    {
        var nav = MainGameManager.mainGameManager.RightNav.GetComponent<RightNav>();
        nav.OpenNav();
        nav.ResetPopUp(2);
    }
    public void UpPopUp(int num)
    {
        var nav = MainGameManager.mainGameManager.RightNav.GetComponent<RightNav>();
        if (nav.openedType != num)
        {
            nav.OpenNav();
            nav.ResetPopUp(num);
        }
        else
        {
            nav.openedType = -1;
            nav.CloseNav();
        }
    }
}
