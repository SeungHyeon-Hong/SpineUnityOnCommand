using UnityEngine;
using Spine.Unity;

public class Example : MonoBehaviour
{
    public SkeletonAnimation skel;
    public void GetSkeleton(string keyName, string spineFolderPath, string animationName = null)
    {
        // Load skeletondata and add to dictionary.
        Spine.OnCommand.SpineUnityOnCommand.LoadSkeletonDataAsset(keyName, spineFolderPath);

        // Get skeletondata by keyName from dictionary.
        skel.skeletonDataAsset = Spine.OnCommand.SpineUnityOnCommand.GetCommandSkeletonDataAsset(keyName);

        if (animationName != null)
            skel.AnimationState.SetAnimation(0, animationName, true);
    }
}
