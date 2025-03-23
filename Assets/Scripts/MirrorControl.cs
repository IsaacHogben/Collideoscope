using UnityEngine;

public class MirrorControl : MonoBehaviour
{
    public Material mirrorMaterial; // Assign MirrorMaterial in Inspector
    public int mirrorMode = 0; // defines
    private int reflectionCount = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) mirrorMode = 0; // No Mirror
        if (Input.GetKeyDown(KeyCode.Alpha2)) mirrorMode = 2; // One mirrored half
        if (Input.GetKeyDown(KeyCode.Alpha3)) mirrorMode = 4; // Four mirrored quadrants
        if (Input.GetKeyDown(KeyCode.Alpha4)) mirrorMode = 8; // Eight-way true kaleidoscope
        if (Input.GetKeyDown(KeyCode.Alpha5)) mirrorMode = 16; // Double Eight-way true kaleidoscope

        //mirrorMaterial.SetFloat("_MirrorMode", mirrorMode);*/
    }

    public void IncrementMirrorMode(int amount)
    {
        reflectionCount += amount;
        reflectionCount = Mathf.Clamp(reflectionCount, 0, 4);
        if (reflectionCount == 0)
        {
            mirrorMode = 0;
            return;
        }
        mirrorMode = Mathf.RoundToInt(Mathf.Pow(2f, reflectionCount));
    }

    public void SetMirrorMode(int mode)
    {
        reflectionCount = mode - 1;
        reflectionCount = Mathf.Clamp(reflectionCount, 0, 4);
        if (reflectionCount == 0)
        {
            mirrorMode = 0;
            return;
        }
        mirrorMode = Mathf.RoundToInt(Mathf.Pow(2f, reflectionCount));
    }
}