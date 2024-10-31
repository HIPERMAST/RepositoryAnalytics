using UnityEngine;

public class BranchInfo : MonoBehaviour
{
    private BranchSpawn.Branch branchData;
    private BranchSpawn spawner;

    public void SetBranchData(BranchSpawn.Branch branch, BranchSpawn spawner)
    {
        this.branchData = branch;
        this.spawner = spawner;
    }

    void OnMouseDown()
    {
        if (spawner != null && branchData != null)
        {
            spawner.DisplayBranchInfo(branchData);
        }
    }
}
