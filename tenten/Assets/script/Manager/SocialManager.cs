using UnityEngine;
using System.Collections;

public class SocialManager : Singleton<SocialManager> {

    private Base_Social Social;

    public void Init()
    {
#if SOCIAL_FACEBOOK
        Social = new FBModule();
#elif SOCIAL_KAKAO
#else
#endif

        Social.Init();
    }

    public void Login()
    {
        Social.Login();
    }
}
