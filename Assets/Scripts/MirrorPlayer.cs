using UnityEngine;
using System.Collections.Generic;

public class MirrorPlayer : MonoBehaviour
{
    private MirrorControl mirrorControl;
    public GameObject playerPrefab;
    private List<GameObject> clones = new List<GameObject>();
    private bool isClone = false; // Track if this is the original player
    private int lastMirrorMode = 0; // Track the last mode to detect changes


    void Start()
    {
        mirrorControl = FindFirstObjectByType<MirrorControl>();

        // Prevent clones from creating more clones
        if (isClone)
            return;

        // Create clones only if this is the original player
        for (int i = 0; i < 15; i++)
        {
            GameObject clone = Instantiate(playerPrefab, transform.position, transform.rotation);
            clone.SetActive(false);
            clone.GetComponent<MirrorPlayer>().isClone = true; // Mark clones as non-original
            clone.GetComponent<PlayerMovement>().enabled = false; // Disable movement on clones
            clones.Add(clone);
        }
    }

    void Update()
    {
        if (isClone) return; // Clones don't run Update()

        int mirrorMode = mirrorControl != null ? mirrorControl.mirrorMode : 0; // Default to mode 0

        if (mirrorMode != lastMirrorMode)
        {
            UpdateCloneVisibility(mirrorMode);
            lastMirrorMode = mirrorMode; // Save last mode
        }

        Vector3 originalPos = transform.position;
        Quaternion originalRot = transform.rotation;
        for (int i = 0; i < clones.Count; i++)
        {
            if (clones[i].activeSelf)
            {
                clones[i].transform.position = GetMirroredPosition(originalPos, i + 1);
                clones[i].transform.rotation = GetMirroredRotation(originalRot, i + 1);
            }
        }
    }

    void UpdateCloneVisibility(int mirrorMode)
    {
        for (int i = 0; i < clones.Count; i++)
        {
            clones[i].SetActive(i < mirrorMode - 1); // Only enable the needed clones
        }
    }

    Vector3 GetMirroredPosition(Vector3 originalPos, int index)
    {
        Vector3 mirroredPos = originalPos;
        Vector3 center = Vector3.zero;

        if (mirrorControl.mirrorMode == 2) // Formerly mode 1
        {
            if (index == 1) mirroredPos.x = -originalPos.x;
        }
        else if (mirrorControl.mirrorMode == 4)
        {
            if (index == 1) mirroredPos.x = -originalPos.x;
            if (index == 2) mirroredPos.y = -originalPos.y;
            if (index == 3) mirroredPos = new Vector3(-originalPos.x, -originalPos.y, originalPos.z);
        }
        else if (mirrorControl.mirrorMode == 8)
        {
            // Determine quadrant like in the shader
            bool flipX = (index & 1) != 0;
            bool flipY = (index & 2) != 0;
            bool flipDiagonal = (index & 4) != 0; // New diagonal mirroring for second set

            if (flipX) mirroredPos.x = -mirroredPos.x;
            if (flipY) mirroredPos.y = -mirroredPos.y;
            if (flipDiagonal)
            {
                float temp = mirroredPos.x;
                mirroredPos.x = mirroredPos.y;
                mirroredPos.y = temp;
            }
        }
        else if (mirrorControl.mirrorMode == 16)
        {
            {
                float angle = Mathf.PI / 8 * index; // 22.5-degree segments
                mirroredPos = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg) * (originalPos - center) + center;
                if (index % 2 == 1) mirroredPos.x = -mirroredPos.x; // Flip every other segment
            }
        }

        return mirroredPos;
    }

    Quaternion GetMirroredRotation(Quaternion originalRot, int index)
    {
        Quaternion mirroredRot = originalRot;

        if (mirrorControl.mirrorMode == 2)
        {
            if (index == 1) mirroredRot = Quaternion.Euler(0, 0, -originalRot.eulerAngles.z + 60);
        }
        else if (mirrorControl.mirrorMode == 4)
        {
            if (index == 1) mirroredRot = Quaternion.Euler(0, 0, -originalRot.eulerAngles.z + 60);
            if (index == 2) mirroredRot = Quaternion.Euler(0, 0, 180 - originalRot.eulerAngles.z + 60);
            if (index == 3) mirroredRot = Quaternion.Euler(0, 0, originalRot.eulerAngles.z + 180);
        }
        else if (mirrorControl.mirrorMode == 8)
        {
            bool flipX = (index & 1) != 0;
            bool flipY = (index & 2) != 0;
            bool flipDiagonal = (index & 4) != 0;

            if (flipX) mirroredRot = Quaternion.Euler(0, 0, -mirroredRot.eulerAngles.z);
            if (flipY) mirroredRot = Quaternion.Euler(0, 0, 180 - mirroredRot.eulerAngles.z);
            if (flipDiagonal) mirroredRot = Quaternion.Euler(0, 0, mirroredRot.eulerAngles.z + 180);
        }
        else if (mirrorControl.mirrorMode == 16)
        {
            float angle = Mathf.PI / 8 * index; // 22.5-degree segments
            mirroredRot = Quaternion.Euler(0, 0, originalRot.eulerAngles.z + angle * Mathf.Rad2Deg);
            if (index % 2 == 1) mirroredRot = Quaternion.Euler(0, 0, -mirroredRot.eulerAngles.z);
        }

        return mirroredRot;
    }
}