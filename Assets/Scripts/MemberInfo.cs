using UnityEngine;

public class MemberInfo : MonoBehaviour
{
    private MemberSpawn.Member memberData;
    private MemberSpawn memberSpawn;

    public void SetMemberData(MemberSpawn.Member member, MemberSpawn spawner)
    {
        memberData = member;
        memberSpawn = spawner;
    }

    void OnMouseDown()
    {
        if (memberSpawn != null && memberData != null)
        {
            memberSpawn.DisplayMemberInfo(memberData);
        }
    }
}
