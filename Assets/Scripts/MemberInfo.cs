using UnityEngine;

public class MemberInfo : MonoBehaviour
{
    private SpawnerScript.Member memberData; // Store member data

    // Set the member data when the cube is instantiated
    public void SetMemberData(SpawnerScript.Member data)
    {
        memberData = data;
    }

    void OnMouseDown()
    {
        // This method is triggered when the cube is clicked
        ShowMemberInfo();
    }

    // Display the member's information in the console (or update UI elements)
    private void ShowMemberInfo()
    {
        Debug.Log($"Member Info:\n" +
                  $"Login: {memberData.login}\n" +
                  $"ID: {memberData.id}\n" +
                  $"Avatar URL: {memberData.avatar_url}");
    }
}