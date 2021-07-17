using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPopUP : MonoBehaviour
{
    public RightNav nav;
    // Start is called before the first frame update
    void Start()
    {
        nav= MainGameManager.mainGameManager.RightNav.GetComponent<RightNav>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RefreshPopUpContents(int num)
    {
        nav.OpenNav();
        nav.ResetPopUp(num);
    }
    public void BulidingUpPopUp()
    {
        nav.OpenNav();
        nav.ResetPopUp(2);
    }
    public void UpPopUp(int num)
    {
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
